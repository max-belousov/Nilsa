using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Enums;
using VkNet.Enums.Filters;

namespace Nilsa
{
    public class ImportContactsFromGroup
    {
        // Object to store the current state, for passing to the caller.
        public class CurrentState
        {
            public int ImportPhase = -1;
            public int ContactsImported;
            public int ContactsAdded;
            public int ContactsTotal;
            public string sUName;
            public string dtLastSeen="";
        }

        public bool bModeImport = true;

        public string sGroupID;
        public long iPersUserID;
        public string sPersUsersIDs;
        public List<String> lstContHar;
        public List<String> lstFilterHar;
        public int iContHarCount;

        public int nudContacterCountMembers = 0;
        public int nudContacterCountAdministrators = 0;
        public int nudContacterCountAuthors = 0;
        public int nudContacterCountCommentators = 0;
        public int nudNeedContacterMultiplicator = 10;

        private int ContactsImported;
        private int ContactsTotal;
        private int ContactsAdded;
        public int maxContacterCount;
        public int nudDuplicatesAction = 0;
        public string groupSearchQuery = "";

        private FormEditContactsDB mFormEditContactsDB = null;
        private FormEditPersonenDB mFormEditPersonenDB = null;
        private FormEditGroupsDB mFormEditGroupsDB = null;
        private FormInitDialogsWithContactersFromGroups mFormInitDialogsWithContactersFromGroups = null;

        private FormMain mFormMain = null;
        public Dictionary<long, HashSet<long>> usersGroupIDs = new Dictionary<long, HashSet<long>>();

        private string sUnknownAge;

        public ImportContactsFromGroup(FormEditContactsDB formEditContactsDB, FormMain _formmain, bool _bModeImport)
        {
            bModeImport = _bModeImport;
            mFormEditContactsDB = formEditContactsDB;
            mFormMain = _formmain;

            sUnknownAge = mFormMain.sDBDataItemsStrings_AgeUnknown;
        }

        public ImportContactsFromGroup(FormEditGroupsDB formEditGroupsDB, FormMain _formmain, bool _bModeImport)
        {
            bModeImport = _bModeImport;
            mFormEditGroupsDB = formEditGroupsDB;
            mFormMain = _formmain;

            sUnknownAge = mFormMain.sDBDataItemsStrings_AgeUnknown;
        }


        public ImportContactsFromGroup(FormInitDialogsWithContactersFromGroups formInitDialogsWithContactersFromGroups, FormMain _formmain, bool _bModeImport)
        {
            bModeImport = _bModeImport;
            mFormInitDialogsWithContactersFromGroups = formInitDialogsWithContactersFromGroups;
            mFormMain = _formmain;

            sUnknownAge = mFormMain.sDBDataItemsStrings_AgeUnknown;
        }

        public ImportContactsFromGroup(FormEditPersonenDB formEditPersonenDB, FormMain _formmain)
        {
            mFormEditPersonenDB = formEditPersonenDB;
            mFormMain = _formmain;

            sUnknownAge = mFormMain.sDBDataItemsStrings_AgeUnknown;
            bModeImport = false;
        }

        public void ImportContacts(System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
        {
            // Initialize the variables.
            CurrentState state = new CurrentState();
            state.ImportPhase = 0;
            try
            {
                int totalCount = 0;
                var usrToImportIDs = new List<long>();
                HashSet<long> usrToImportIDsAdministrators = new HashSet<long>();
                HashSet<long> usrToImportIDsAuthors = new HashSet<long>();
                HashSet<long> usrToImportIDsCommentators = new HashSet<long>();
                HashSet<long> usrToImportIDsMembers = new HashSet<long>();

                /*
                if (sGroupID.StartsWith("GroupID="))
                {
                    sGroupID = sGroupID.Substring(8);
                    int iOffs = 0;
                    do
                    {
                        try
                        {
                            long lCurGroupID = Convert.ToInt64(sGroupID);
                            var ids = FormMain.api.Groups.GetMembers(lCurGroupID, out totalCount, 1000, iOffs, VkNet.Enums.SafetyEnums.GroupsSort.IdAsc);
                            if (ids.Count > 0)
                            {
                                iOffs += 1000;
                                foreach (long id in ids)
                                {
                                    usrToImportIDs.Add(id);
                                    if (!usersGroupIDs.ContainsKey(id))
                                        usersGroupIDs.Add(id, new HashSet<long>());
                                    usersGroupIDs[id].Add(lCurGroupID);
                                }
                                state.ContactsImported = 0;
                                state.ContactsAdded = 0;
                                state.ContactsTotal = usrToImportIDs.Count;
                                state.dtLastSeen = null;
                                state.sUName = "";
                                worker.ReportProgress(0, state);

                                if (worker.CancellationPending)
                                {
                                    e.Cancel = true;
                                    break;
                                }

                                if (usrToImportIDs.Count >= maxContacterCount * 100)
                                    break;
                            }
                            else
                                break;
                        }
                        catch (Exception exp)
                        {
                            mFormMain.ExceptionToLogList("ImportContactsFromGroup", "ImportContacts", exp);
                            break;
                        }
                    }
                    while (true);

                    totalCount = usrToImportIDs.Count;
                }
                //--- GroupAACM --- Start ---
                else if (sGroupID.StartsWith("GroupAACM="))
                {
                    sGroupID = sGroupID.Substring(10);
                    String sImportedGroupsIDs = sGroupID;
                    bool bBreakAddGroups = false;
                    state.ContactsAdded = 0;

                    // Administrators
                    if (nudContacterCountAdministrators > 0)
                    {
                        while (sGroupID.Length > 0)
                        {
                            long iIUID = Convert.ToInt64(sGroupID.Substring(0, sGroupID.IndexOf("|")));
                            sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                            //--
                            do
                            {
                                try
                                {
                                    VkNet.Model.Group group = FormMain.api.Groups.GetById(Convert.ToInt64(iIUID), GroupsFields.Contacts);

                                    if (group.Contacts != null)
                                    {
                                        foreach (VkNet.Model.Contact contact in group.Contacts)
                                        {
                                            if (contact.UserId.HasValue)
                                            {
                                                long id = contact.UserId.Value;

                                                if (!usrToImportIDs.Contains(id))
                                                {
                                                    usrToImportIDs.Add(id);

                                                    if (!usrToImportIDsAdministrators.Contains(id))
                                                        usrToImportIDsAdministrators.Add(id);
                                                }

                                                if (!usersGroupIDs.ContainsKey(id))
                                                    usersGroupIDs.Add(id, new HashSet<long>());
                                                usersGroupIDs[id].Add(iIUID);

                                                if (usrToImportIDsAdministrators.Count >= nudContacterCountAdministrators * nudNeedContacterMultiplicator)
                                                    break;
                                            }
                                        }

                                        state.ContactsImported = usrToImportIDsAdministrators.Count;
                                        state.ContactsTotal = nudContacterCountAdministrators * nudNeedContacterMultiplicator;
                                        state.dtLastSeen = null;
                                        state.sUName = "Найдено " + usrToImportIDsAdministrators.Count.ToString() + " контактов (Администраторы групп)";
                                        worker.ReportProgress(0, state);

                                        if (worker.CancellationPending)
                                        {
                                            bBreakAddGroups = true;
                                            e.Cancel = true;
                                            break;
                                        }

                                        if (usrToImportIDsAdministrators.Count >= nudContacterCountAdministrators * nudNeedContacterMultiplicator)
                                        {
                                            bBreakAddGroups = true;
                                            break;
                                        }

                                    }

                                    break;

                                }
                                catch (Exception exp)
                                {
                                    mFormMain.ExceptionToLogList("ImportContactsFromGroup", "ImportContacts", exp);
                                    bBreakAddGroups = true;
                                    break;
                                }
                            }
                            while (true);
                            //--

                            if (bBreakAddGroups)
                                break;
                        }
                    }

                    // Authors
                    if (nudContacterCountAuthors > 0)
                    {
                        sGroupID = sImportedGroupsIDs;
                        bBreakAddGroups = false;

                        while (sGroupID.Length > 0)
                        {
                            long iIUID = Convert.ToInt64(sGroupID.Substring(0, sGroupID.IndexOf("|")));
                            sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                            //--
                            try
                            {
                                var posts = FormMain.api.Wall.Get(-iIUID, out totalCount, 100);
                                if (posts.Count > 0)
                                {
                                    foreach (VkNet.Model.Post post in posts)
                                    {
                                        if (post.FromId.HasValue)
                                        {
                                            long id = post.FromId.Value;
                                            if (!usrToImportIDs.Contains(id))
                                            {
                                                usrToImportIDs.Add(id);

                                                if (!usrToImportIDsAuthors.Contains(id))
                                                    usrToImportIDsAuthors.Add(id);
                                            }

                                            if (!usersGroupIDs.ContainsKey(id))
                                                usersGroupIDs.Add(id, new HashSet<long>());
                                            usersGroupIDs[id].Add(iIUID);

                                            if (usrToImportIDsAuthors.Count >= nudContacterCountAuthors * nudNeedContacterMultiplicator)
                                                break;
                                        }
                                    }

                                    state.ContactsImported = usrToImportIDsAuthors.Count;
                                    state.ContactsTotal = nudContacterCountAuthors * nudNeedContacterMultiplicator;
                                    state.dtLastSeen = null;
                                    state.sUName = "Найдено " + usrToImportIDsAuthors.Count.ToString() + " контактов (Авторы записей)";
                                    worker.ReportProgress(0, state);

                                    if (worker.CancellationPending)
                                    {
                                        e.Cancel = true;
                                        break;
                                    }

                                    if (usrToImportIDsAuthors.Count >= nudContacterCountAuthors * nudNeedContacterMultiplicator)
                                    {
                                        break;
                                    }
                                }
                            }
                            catch (Exception exp)
                            {
                                mFormMain.ExceptionToLogList("ImportContactsFromGroup", "ImportContacts", exp);
                                break;
                            }

                        }

                    }

                    // Commentators
                    if (nudContacterCountCommentators > 0)
                    {
                        sGroupID = sImportedGroupsIDs;
                        bBreakAddGroups = false;

                        while (sGroupID.Length > 0)
                        {
                            long iIUID = Convert.ToInt64(sGroupID.Substring(0, sGroupID.IndexOf("|")));
                            sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                            //--
                            try
                            {
                                var posts = FormMain.api.Wall.Get(-iIUID, out totalCount, 100);
                                if (posts.Count > 0)
                                {
                                    foreach (VkNet.Model.Post post in posts)
                                    {
                                        var msgsReceived = FormMain.api.Wall.GetComments(-iIUID, post.Id, out totalCount, VkNet.Enums.SafetyEnums.CommentsSort.Desc, false, 100);
                                        foreach (VkNet.Model.Comment msg in msgsReceived)
                                        {
                                            long id = msg.FromId;
                                            if (!usrToImportIDs.Contains(id))
                                            {
                                                usrToImportIDs.Add(id);

                                                if (!usrToImportIDsCommentators.Contains(id))
                                                    usrToImportIDsCommentators.Add(id);
                                            }
                                            if (!usersGroupIDs.ContainsKey(id))
                                                usersGroupIDs.Add(id, new HashSet<long>());
                                            usersGroupIDs[id].Add(iIUID);

                                            if (usrToImportIDsCommentators.Count >= nudContacterCountCommentators * nudNeedContacterMultiplicator)
                                                break;

                                        }

                                        state.ContactsImported = usrToImportIDsCommentators.Count;
                                        state.ContactsTotal = nudContacterCountCommentators * nudNeedContacterMultiplicator;
                                        state.dtLastSeen = null;
                                        state.sUName = "Найдено " + usrToImportIDsCommentators.Count.ToString() + " контактов (Комментаторы записей)";
                                        worker.ReportProgress(0, state);

                                        if (usrToImportIDsCommentators.Count >= nudContacterCountCommentators * nudNeedContacterMultiplicator)
                                            break;
                                    }

                                    if (worker.CancellationPending)
                                    {
                                        e.Cancel = true;
                                        break;
                                    }

                                    if (usrToImportIDsCommentators.Count >= nudContacterCountCommentators * nudNeedContacterMultiplicator)
                                    {
                                        break;
                                    }
                                }
                            }
                            catch (Exception exp)
                            {
                                mFormMain.ExceptionToLogList("ImportContactsFromGroup", "ImportContacts", exp);
                                break;
                            }

                        }

                    }

                    // Members
                    if (nudContacterCountMembers > 0)
                    {
                        sGroupID = sImportedGroupsIDs;
                        bBreakAddGroups = false;

                        while (sGroupID.Length > 0)
                        {
                            long iIUID = Convert.ToInt64(sGroupID.Substring(0, sGroupID.IndexOf("|")));
                            sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                            //--
                            int iOffs = 0;
                            do
                            {
                                try
                                {
                                    var ids = FormMain.api.Groups.GetMembers(iIUID, out totalCount, 1000, iOffs, VkNet.Enums.SafetyEnums.GroupsSort.IdAsc);
                                    if (ids.Count > 0)
                                    {
                                        iOffs += 1000;
                                        foreach (long id in ids)
                                        {
                                            if (!usrToImportIDs.Contains(id))
                                            {
                                                usrToImportIDs.Add(id);

                                                if (!usrToImportIDsMembers.Contains(id))
                                                    usrToImportIDsMembers.Add(id);
                                            }

                                            if (!usersGroupIDs.ContainsKey(id))
                                                usersGroupIDs.Add(id, new HashSet<long>());
                                            usersGroupIDs[id].Add(iIUID);

                                            if (usrToImportIDsMembers.Count >= nudContacterCountMembers * nudNeedContacterMultiplicator)
                                                break;
                                        }

                                        state.ContactsImported = usrToImportIDsMembers.Count;
                                        state.ContactsTotal = nudContacterCountMembers * nudNeedContacterMultiplicator;
                                        state.dtLastSeen = null;
                                        state.sUName = "Найдено " + usrToImportIDsMembers.Count.ToString() + " контактов (Участники групп)";
                                        worker.ReportProgress(0, state);

                                        if (worker.CancellationPending)
                                        {
                                            bBreakAddGroups = true;
                                            e.Cancel = true;
                                            break;
                                        }

                                        if (usrToImportIDsMembers.Count >= nudContacterCountMembers * nudNeedContacterMultiplicator)
                                        {
                                            bBreakAddGroups = true;
                                            break;
                                        }
                                    }
                                    else
                                        break;
                                }
                                catch (Exception exp)
                                {
                                    mFormMain.ExceptionToLogList("ImportContactsFromGroup", "ImportContacts", exp);
                                    bBreakAddGroups = true;
                                    break;
                                }
                            }
                            while (true);
                            //--

                            if (bBreakAddGroups)
                                break;
                        }
                    }
                    totalCount = usrToImportIDs.Count;
                }
                //--- GroupAACM --- End ---
                else if (sGroupID.StartsWith("GroupsIDs="))
                {
                    sGroupID = sGroupID.Substring(10);
                    bool bBreakAddGroups = false;
                    while (sGroupID.Length > 0)
                    {
                        long iIUID = Convert.ToInt64(sGroupID.Substring(0, sGroupID.IndexOf("|")));
                        sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                        //--
                        int iOffs = 0;
                        do
                        {
                            try
                            {
                                var ids = FormMain.api.Groups.GetMembers(iIUID, out totalCount, 1000, iOffs, VkNet.Enums.SafetyEnums.GroupsSort.IdAsc);
                                if (ids.Count > 0)
                                {
                                    iOffs += 1000;
                                    foreach (long id in ids)
                                    {
                                        if (!usrToImportIDs.Contains(id))
                                            usrToImportIDs.Add(id);

                                        if (!usersGroupIDs.ContainsKey(id))
                                            usersGroupIDs.Add(id, new HashSet<long>());
                                        usersGroupIDs[id].Add(iIUID);
                                    }

                                    state.ContactsImported = 0;
                                    state.ContactsAdded = 0;
                                    state.ContactsTotal = usrToImportIDs.Count;
                                    state.dtLastSeen = null;
                                    state.sUName = "";
                                    worker.ReportProgress(0, state);

                                    if (worker.CancellationPending)
                                    {
                                        bBreakAddGroups = true;
                                        e.Cancel = true;
                                        break;
                                    }

                                    if (usrToImportIDs.Count >= maxContacterCount * 100)
                                    {
                                        bBreakAddGroups = true;
                                        break;
                                    }
                                }
                                else
                                    break;
                            }
                            catch (Exception exp)
                            {
                                mFormMain.ExceptionToLogList("ImportContactsFromGroup", "ImportContacts", exp);
                                bBreakAddGroups = true;
                                break;
                            }
                        }
                        while (true);
                        //--

                        if (bBreakAddGroups)
                            break;
                    }
                    totalCount = usrToImportIDs.Count;
                }
                else if (sGroupID.StartsWith("GrWallIDs="))
                {
                    sGroupID = sGroupID.Substring(10);
                    while (sGroupID.Length > 0)
                    {
                        long iIUID = Convert.ToInt64(sGroupID.Substring(0, sGroupID.IndexOf("|")));
                        sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                        //--
                        try
                        {
                            var posts = FormMain.api.Wall.Get(-iIUID, out totalCount, 100);
                            if (posts.Count > 0)
                            {
                                foreach (VkNet.Model.Post post in posts)
                                {
                                    if (post.FromId.HasValue)
                                    {
                                        long id = post.FromId.Value;
                                        if (!usrToImportIDs.Contains(id))
                                            usrToImportIDs.Add(id);

                                        if (!usersGroupIDs.ContainsKey(id))
                                            usersGroupIDs.Add(id, new HashSet<long>());
                                        usersGroupIDs[id].Add(iIUID);
                                    }
                                }
                                state.ContactsImported = 0;
                                state.ContactsAdded = 0;
                                state.ContactsTotal = usrToImportIDs.Count;
                                state.dtLastSeen = null;
                                state.sUName = "";
                                worker.ReportProgress(0, state);

                                if (worker.CancellationPending)
                                {
                                    e.Cancel = true;
                                    break;
                                }

                                if (usrToImportIDs.Count >= maxContacterCount * 100)
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            mFormMain.ExceptionToLogList("ImportContactsFromGroup", "ImportContacts", exp);
                            break;
                        }

                    }
                    totalCount = usrToImportIDs.Count;
                }
                else if (sGroupID.StartsWith("GrWCmtIDs="))
                {
                    sGroupID = sGroupID.Substring(10);
                    while (sGroupID.Length > 0)
                    {
                        long iIUID = Convert.ToInt64(sGroupID.Substring(0, sGroupID.IndexOf("|")));
                        sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                        //--
                        try
                        {
                            var posts = FormMain.api.Wall.Get(-iIUID, out totalCount, 100);
                            if (posts.Count > 0)
                            {
                                foreach (VkNet.Model.Post post in posts)
                                {
                                    if (post.FromId.HasValue)
                                    {
                                        long id = post.FromId.Value;
                                        if (!usrToImportIDs.Contains(id))
                                            usrToImportIDs.Add(id);

                                        if (!usersGroupIDs.ContainsKey(id))
                                            usersGroupIDs.Add(id, new HashSet<long>());
                                        usersGroupIDs[id].Add(iIUID);
                                    }

                                    var msgsReceived = FormMain.api.Wall.GetComments(-iIUID, post.Id, out totalCount, VkNet.Enums.SafetyEnums.CommentsSort.Desc, false, 100);
                                    foreach (VkNet.Model.Comment msg in msgsReceived)
                                    {
                                        long id = msg.FromId;
                                        if (!usrToImportIDs.Contains(id))
                                            usrToImportIDs.Add(id);

                                        if (!usersGroupIDs.ContainsKey(id))
                                            usersGroupIDs.Add(id, new HashSet<long>());
                                        usersGroupIDs[id].Add(iIUID);

                                    }

                                    state.ContactsImported = 0;
                                    state.ContactsAdded = 0;
                                    state.ContactsTotal = usrToImportIDs.Count;
                                    state.dtLastSeen = null;
                                    state.sUName = "";
                                    worker.ReportProgress(0, state);
                                }

                                if (worker.CancellationPending)
                                {
                                    e.Cancel = true;
                                    break;
                                }

                                if (usrToImportIDs.Count >= maxContacterCount * 100)
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            mFormMain.ExceptionToLogList("ImportContactsFromGroup", "ImportContacts", exp);
                            break;
                        }

                    }
                    totalCount = usrToImportIDs.Count;
                }
                else if (sGroupID.StartsWith("GrCmtsIDs="))
                {
                    sGroupID = sGroupID.Substring(10);
                    while (sGroupID.Length > 0)
                    {
                        long iIUID = Convert.ToInt64(sGroupID.Substring(0, sGroupID.IndexOf("|")));
                        sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                        //--
                        try
                        {
                            var posts = FormMain.api.Wall.Get(-iIUID, out totalCount, 100);
                            if (posts.Count > 0)
                            {
                                foreach (VkNet.Model.Post post in posts)
                                {
                                    var msgsReceived = FormMain.api.Wall.GetComments(-iIUID, post.Id, out totalCount, VkNet.Enums.SafetyEnums.CommentsSort.Desc, false, 100);
                                    foreach (VkNet.Model.Comment msg in msgsReceived)
                                    {
                                        long id = msg.FromId;
                                        if (!usrToImportIDs.Contains(id))
                                            usrToImportIDs.Add(id);

                                        if (!usersGroupIDs.ContainsKey(id))
                                            usersGroupIDs.Add(id, new HashSet<long>());
                                        usersGroupIDs[id].Add(iIUID);
                                    }

                                    state.ContactsImported = 0;
                                    state.ContactsAdded = 0;
                                    state.ContactsTotal = usrToImportIDs.Count;
                                    state.dtLastSeen = null;
                                    state.sUName = "";
                                    worker.ReportProgress(0, state);
                                }

                                if (worker.CancellationPending)
                                {
                                    e.Cancel = true;
                                    break;
                                }

                                if (usrToImportIDs.Count >= maxContacterCount * 100)
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            mFormMain.ExceptionToLogList("ImportContactsFromGroup", "ImportContacts", exp);
                            break;
                        }

                    }
                    totalCount = usrToImportIDs.Count;
                }
                else if (sGroupID.StartsWith("UserID="))
                {
                    sGroupID = sGroupID.Substring(7);
                    long iIUID = Convert.ToInt64(sGroupID);
                    do
                    {
                        try
                        {
                            var users = FormMain.api.Friends.Get(iIUID);
                            foreach (VkNet.Model.User usrAdr in users)
                                usrToImportIDs.Add(usrAdr.Id);
                            totalCount = users.Count;
                            break;
                        }
                        catch (Exception exp)
                        {
                            mFormMain.ExceptionToLogList("ImportContactsFromGroup", "ImportContacts", exp);
                            break;
                        }
                    }
                    while (true);
                }
                else 
                */
                if (sGroupID.StartsWith("UIDs="))
                {
                    sGroupID = sGroupID.Substring(5);
                    while (sGroupID.Length > 0)
                    {
                        long iIUID = Convert.ToInt64(sGroupID.Substring(0, sGroupID.IndexOf("|")));
                        sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                        usrToImportIDs.Add(iIUID);
                        totalCount++;
                    }
                }
                /*
                else if (sGroupID.StartsWith("Search="))
                {
                    sGroupID = sGroupID.Substring(7);
                    String ssQuery = sGroupID.Substring(0, sGroupID.IndexOf("|"));
                    sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                    String ssSex = sGroupID.Substring(0, sGroupID.IndexOf("|"));
                    sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                    String ssOnline = sGroupID.Substring(0, sGroupID.IndexOf("|"));
                    sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                    String ssHasPhoto = sGroupID.Substring(0, sGroupID.IndexOf("|"));
                    sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                    String ssCountry = sGroupID.Substring(0, sGroupID.IndexOf("|"));
                    sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                    String ssCity = sGroupID.Substring(0, sGroupID.IndexOf("|"));
                    sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                    String ssStatus = sGroupID.Substring(0, sGroupID.IndexOf("|"));
                    sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                    String ssAgeFrom = sGroupID.Substring(0, sGroupID.IndexOf("|"));
                    sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                    String ssAgeTo = sGroupID.Substring(0, sGroupID.IndexOf("|"));
                    sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);

                    String ssGroupID = sGroupID.Substring(0, sGroupID.IndexOf("|"));
                    sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                    String ssReligion = sGroupID.Substring(0, sGroupID.IndexOf("|"));
                    sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                    String ssInterests = sGroupID.Substring(0, sGroupID.IndexOf("|"));
                    sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                    String ssCompany = sGroupID.Substring(0, sGroupID.IndexOf("|"));
                    sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);
                    String ssPosition = sGroupID.Substring(0, sGroupID.IndexOf("|"));
                    sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);

                    int? isCountry = null;
                    if (!ssCountry.Equals("null"))
                        isCountry = Convert.ToInt32(ssCountry);

                    int? isCity = null;
                    if (!ssCity.Equals("null"))
                        isCity = Convert.ToInt32(ssCity);

                    int? isAgeFrom = null;
                    if (ssAgeFrom.Length > 0)
                        isAgeFrom = Convert.ToInt32(ssAgeFrom);

                    int? isAgeTo = null;
                    if (ssAgeTo.Length > 0)
                        isAgeTo = Convert.ToInt32(ssAgeTo);

                    int? isStatus = null;
                    if (!ssStatus.Equals("0"))
                        isStatus = Convert.ToInt32(ssStatus);

                    int? isGroupID = null;
                    if (!ssGroupID.Equals("-1"))
                        isGroupID = Convert.ToInt32(ssGroupID);

                    int iOffs = 0;
                    do
                    {
                        try
                        {
                            var users = FormMain.api.Users.Search(ssQuery, out totalCount, null, 1000, iOffs, Convert.ToInt32(ssSex), Convert.ToInt32(ssOnline), Convert.ToInt32(ssHasPhoto), isCountry, isCity, isStatus, isAgeFrom, isAgeTo, isGroupID, ssReligion, ssInterests, ssCompany, ssPosition);
                            if (users.Count > 0)
                            {
                                iOffs += 1000;
                                foreach (VkNet.Model.User usrAdr in users)
                                    usrToImportIDs.Add(usrAdr.Id);
                            }
                            else
                                break;
                        }
                        catch (Exception exp)
                        {
                            mFormMain.ExceptionToLogList("ImportContactsFromGroup", "ImportContacts", exp);
                            break;
                        }
                    }
                    while (true);
                    totalCount = usrToImportIDs.Count;
                }
                */

                ContactsImported = 0;
                ContactsAdded = 0;
                ContactsTotal = totalCount;

                String[] RQV = new String[iContHarCount];
                String[] EQV = new String[iContHarCount];
                foreach (String str in lstFilterHar)
                {
                    if (str == null)
                        continue;

                    if (str.Length == 0)
                        continue;

                    RQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1).ToLower();
                }

                Boolean bRQVEmpty = true;
                for (int iv = 0; iv < RQV.Length; iv++)
                {
                    if (RQV[iv].Length > 0)
                    {
                        bRQVEmpty = false;
                        break;
                    }
                }

                bool bAcceptToAll = false;
                bool bAcceptExisted = false;

                if (nudDuplicatesAction > 0)
                {
                    bAcceptToAll = true;
                    bAcceptExisted = nudDuplicatesAction == 1;
                }

                int iCycleGroupsContacterImport = -1;
                do
                {
                    if (nudContacterCountAdministrators > 0 || nudContacterCountAuthors > 0 || nudContacterCountCommentators > 0 || nudContacterCountMembers > 0)
                    {
                        iCycleGroupsContacterImport++;
                        usrToImportIDs.Clear();
                        if (iCycleGroupsContacterImport == 0)
                        {
                            state.ImportPhase = 1;
                            usrToImportIDs.AddRange(usrToImportIDsAdministrators);
                            ContactsAdded = 0;
                            ContactsImported = 0;
                            ContactsTotal = usrToImportIDs.Count;
                            maxContacterCount = nudContacterCountAdministrators;
                        }
                        else if (iCycleGroupsContacterImport == 1)
                        {
                            state.ImportPhase = 2;
                            usrToImportIDs.AddRange(usrToImportIDsAuthors);
                            ContactsAdded = 0;
                            ContactsImported = 0;
                            ContactsTotal = usrToImportIDs.Count;
                            maxContacterCount = nudContacterCountAuthors;
                        }
                        else if (iCycleGroupsContacterImport == 2)
                        {
                            state.ImportPhase = 3;
                            usrToImportIDs.AddRange(usrToImportIDsCommentators);
                            ContactsImported = 0;
                            ContactsAdded = 0;
                            ContactsTotal = usrToImportIDs.Count;
                            maxContacterCount = nudContacterCountCommentators;
                        }
                        else if (iCycleGroupsContacterImport == 3)
                        {
                            state.ImportPhase = 4;
                            usrToImportIDs.AddRange(usrToImportIDsMembers);
                            ContactsAdded = 0;
                            ContactsImported = 0;
                            ContactsTotal = usrToImportIDs.Count;
                            maxContacterCount = nudContacterCountMembers;
                        }
                        else
                            break;
                    }

                    state.ContactsImported = ContactsImported;
                    state.ContactsAdded = ContactsAdded;
                    state.ContactsTotal = ContactsTotal;
                    state.dtLastSeen = "";
                    state.sUName = "";
                    worker.ReportProgress(0, state);

                    if (iCycleGroupsContacterImport >= 0)
                    {
                        if (ContactsTotal == 0)
                            continue;
                    }

                    foreach (long usrID in usrToImportIDs)
                    {
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }
                        else
                        {
                            if (ContactsAdded >= maxContacterCount)
                                break;

                            ContactsImported++;
                            state.ContactsImported = ContactsImported;

                            String sUID = usrID.ToString();

                            FormWebBrowser.Persone usrAdr = null;
                            /*
                            do
                            {
                                try
                                {
                                    usrAdr = FormMain.api.Users.Get(Convert.ToInt64(sUID), ProfileFields.FirstName | ProfileFields.LastName | ProfileFields.City | ProfileFields.Sex | ProfileFields.BirthDate | ProfileFields.Country | ProfileFields.Relation | ProfileFields.Online | ProfileFields.Counters | ProfileFields.LastSeen);
                                    break;
                                }
                                catch (Exception exp)
                                {
                                    mFormMain.ExceptionToLogList("ImportContactsFromGroup", "ImportContacts", exp);
                                    break;
                                }
                            }
                            while (true);
                            */

                            if (mFormEditPersonenDB != null)
                            {
                                String sULogin = mFormEditPersonenDB.PersonenList_GetUserField(0, sUID, FormEditPersonenDB.USERFIELD_LOGIN);
                                String sUPwd = mFormEditPersonenDB.PersonenList_GetUserField(0, sUID, FormEditPersonenDB.USERFIELD_PASSWORD);
                                long uid = AutorizeVK(sULogin, sUPwd);
                                if (uid >= 0)
                                {
                                    mFormMain.fwbVKontakte.Setup(sULogin, sUPwd, WebBrowserCommand.GetPersoneAttributes, uid);
                                    mFormMain.fwbVKontakte.WaitResult();
                                    usrAdr = mFormMain.fwbVKontakte.personeAtrributes;
                                }
                            }
                            else if (mFormEditContactsDB != null)
                            {
                                mFormMain.fwbVKontakte.Setup(mFormMain.userLogin, mFormMain.userPassword, WebBrowserCommand.GetContactAttributes, usrID);
                                mFormMain.fwbVKontakte.WaitResult();
                                usrAdr = mFormMain.fwbVKontakte.contactAtrributes;
                            }

                            if (usrAdr != null)
                            {
                                if (usrAdr.FirstName.Length == 0 && usrAdr.LastName.Length == 0)
                                    usrAdr = null;
                            }

                            if (usrAdr != null)
                            {
                                String sUName = usrAdr.FirstName + " " + usrAdr.LastName;
                                state.dtLastSeen = usrAdr.LastSeen;
                                state.sUName = sUName;

                                if (iCycleGroupsContacterImport >= 0)
                                {
                                    String _role = "Неизвестно";
                                    if (iCycleGroupsContacterImport == 0)
                                        _role = "Администратор группы";
                                    else if (iCycleGroupsContacterImport == 1)
                                        _role = "Автор записей";
                                    else if (iCycleGroupsContacterImport == 2)
                                        _role = "Комментатор записей";
                                    else if (iCycleGroupsContacterImport == 3)
                                        _role = "Участник группы";

                                    state.sUName += " (" + _role + ")";
                                }
                                worker.ReportProgress(0, state);

                                List<String> lstContHarValues = new List<String>();
                                int nContHarCount = 0;
                                if (mFormEditContactsDB != null)
                                    nContHarCount = mFormEditContactsDB.iContHarCount;
                                else if (mFormEditGroupsDB != null || mFormInitDialogsWithContactersFromGroups != null)
                                    nContHarCount = mFormMain.iContHarCount;
                                else if (mFormEditPersonenDB != null)
                                    nContHarCount = mFormEditPersonenDB.iContHarCount;

                                String sAttrFileName = "";
                                String scpuID = "";
                                if (mFormEditContactsDB != null || mFormEditGroupsDB != null || mFormInitDialogsWithContactersFromGroups != null)
                                {
                                    if (sPersUsersIDs.Length > 0)
                                    {
                                        scpuID = sPersUsersIDs.Substring(0, sPersUsersIDs.IndexOf("|"));
                                        sAttrFileName = "cont_" + FormMain.getSocialNetworkPrefix() + scpuID + "_" + sUID + ".txt";
                                    }
                                    else
                                        sAttrFileName = "cont_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt";
                                }
                                else if (mFormEditPersonenDB != null)
                                    sAttrFileName = "pers_" + FormMain.getSocialNetworkPrefix(0) + sUID + ".txt";

                                bool bDataFileExists = false;
                                if (File.Exists(Path.Combine(FormMain.sDataPath, sAttrFileName)))
                                {
                                    bDataFileExists = true;
                                    var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, sAttrFileName));
                                    lstContHarValues = new List<String>(srcFile);
                                    for (int i = 0; i < nContHarCount; i++)
                                    {
                                        String sCV = lstContHar[i].Substring(lstContHar[i].IndexOf("|") + 1);
                                        if (sCV.Length > 0)
                                        {
                                            if (sCV.Equals("#sex#"))
                                            {
                                                String sexValue = lstContHarValues[i].Substring(lstContHarValues[i].IndexOf("|") + 1);
                                                if (sexValue.Length==0 || sexValue.ToLower().Equals("не указан"))
                                                    lstContHarValues[i] = lstContHar[i];
                                            }
                                            else
                                                lstContHarValues[i] = lstContHar[i];
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < nContHarCount; i++)
                                        lstContHarValues.Add(lstContHar[i]);
                                }

                                int iAgeField = -1;
                                int iOnlineField = -1;
                                string lastSeen = usrAdr.LastSeen.Length > 0 ? usrAdr.LastSeen : "";
                                bool lastSeenOnline = false;
                                bool lastSeenHasValue = usrAdr.LastSeen.Length > 0;

                                for (int i = 0; i < nContHarCount; i++)
                                {
                                    if (lstContHarValues[i].IndexOf("#sex#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#sex#", usrAdr.Sex);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#city#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#city#", usrAdr.City);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#country#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#country#", usrAdr.Country);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#relation#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#relation#", usrAdr.Relation);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#online#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#online#", usrAdr.Online);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#birthdate#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#birthdate#", usrAdr.BirthDate != null ? usrAdr.BirthDate : "");
                                    }
                                    else if (lstContHarValues[i].IndexOf("#counters_friends#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#counters_friends#", usrAdr.CountersFriends);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#search_query#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#search_query#", groupSearchQuery);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#group_role#") > 0)
                                    {
                                        String _role = "Неизвестно";
                                        if (iCycleGroupsContacterImport == 0)
                                            _role = "Администратор группы";
                                        else if (iCycleGroupsContacterImport == 1)
                                            _role = "Автор записей";
                                        else if (iCycleGroupsContacterImport == 2)
                                            _role = "Комментатор записей";
                                        else if (iCycleGroupsContacterImport == 3)
                                            _role = "Участник группы";
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#group_role#", _role);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#source_groups#") > 0)
                                    {
                                        String _usrGrIDs = "";
                                        if (usersGroupIDs.ContainsKey(usrID))
                                        {
                                            bool bFirst = false;
                                            foreach (long _ugrud in usersGroupIDs[usrID])
                                            {
                                                if (bFirst)
                                                    _usrGrIDs += ",";
                                                _usrGrIDs += "club" + _ugrud.ToString();
                                                bFirst = true;
                                            }
                                        }
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#source_groups#", _usrGrIDs);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#age#") > 0)
                                    {
                                        String birthdate = usrAdr.BirthDate != null ? usrAdr.BirthDate : "";
                                        String sAge = sUnknownAge;
                                        if (birthdate.Length > 0)
                                        {
                                            String sDD = birthdate.Substring(0, birthdate.IndexOf("."));
                                            birthdate = birthdate.Substring(birthdate.IndexOf(".") + 1);
                                            if (birthdate.IndexOf(".") > 0)
                                            {
                                                String sMM = birthdate.Substring(0, birthdate.IndexOf("."));
                                                birthdate = birthdate.Substring(birthdate.IndexOf(".") + 1);
                                                DateTime bday = new DateTime(Convert.ToInt32(birthdate), Convert.ToInt32(sMM), Convert.ToInt32(sDD));
                                                DateTime today = DateTime.Today;
                                                int age = today.Year - bday.Year;
                                                if (bday > today.AddYears(-age)) age--;
                                                sAge = Convert.ToString(age);
                                            }
                                        }

                                        lstContHarValues[i] = lstContHarValues[i].Replace("#age#", sAge);
                                    }
                                    else if (lstContHarValues[i].IndexOf("#clear#") > 0)
                                    {
                                        lstContHarValues[i] = lstContHarValues[i].Replace("#clear#", "");
                                    }

                                    if (lstContHarValues[i].Substring(lstContHarValues[i].IndexOf("|") + 1).Contains("|"))
                                    {
                                        String sRID = lstContHarValues[i].Substring(0, lstContHarValues[i].IndexOf("|"));
                                        String sRVAL = lstContHarValues[i].Substring(lstContHarValues[i].IndexOf("|") + 1);
                                        lstContHarValues[i] = sRID + "|" + sRVAL.Replace('|', 'I');
                                    }

                                }

                                bool bAcceptEmptyAge = false;
                                int iAcceptAgeMin = -1;
                                int iAcceptAgeMax = -1;

                                if (iAgeField >= 0)
                                {
                                    if (RQV[iAgeField].Length > 0)
                                    {
                                        String sAgeFilter = RQV[iAgeField].Trim();
                                        if (sAgeFilter[0] == '!')
                                        {
                                            bAcceptEmptyAge = true;
                                            sAgeFilter = sAgeFilter.Substring(1).Trim();
                                        }
                                        if (sAgeFilter.Length > 0)
                                        {
                                            if (sAgeFilter.IndexOf("-") >= 0)
                                            {
                                                String sAgeMin = sAgeFilter.Substring(0, sAgeFilter.IndexOf("-")).Trim();
                                                String sAgeMax = sAgeFilter.Substring(sAgeFilter.IndexOf("-") + 1).Trim();
                                                iAcceptAgeMin = 0;
                                                if (sAgeMin.Length > 0)
                                                    iAcceptAgeMin = Convert.ToInt32(sAgeMin);
                                                iAcceptAgeMax = 1000;
                                                if (sAgeMax.Length > 0)
                                                    iAcceptAgeMax = Convert.ToInt32(sAgeMax);
                                            }
                                            else
                                            {
                                                if (sUnknownAge.Equals(sAgeFilter))
                                                    bAcceptEmptyAge = true;
                                                else
                                                {
                                                    iAcceptAgeMin = Convert.ToInt32(sAgeFilter);
                                                    iAcceptAgeMax = iAcceptAgeMin;
                                                }
                                            }
                                        }
                                    }
                                }

                                bool bNeedOnlineCompare = false;
                                if (iOnlineField >= 0)
                                {
                                    if (RQV[iOnlineField].Length > 0)
                                    {
                                        bNeedOnlineCompare = !(RQV[iOnlineField].ToLower().Equals("on line") || RQV[iOnlineField].ToLower().Equals("off line"));
                                    }
                                }

                                bool bEquals = bRQVEmpty;

                                if (!bEquals)
                                {

                                    foreach (String str in lstContHarValues)
                                    {
                                        if (str == null)
                                            continue;

                                        if (str.Length == 0)
                                            continue;

                                        EQV[Convert.ToInt32(str.Substring(0, str.IndexOf("|"))) - 1] = str.Substring(str.IndexOf("|") + 1).ToLower();
                                    }

                                    bEquals = true;
                                    for (int iv = 0; iv < RQV.Length; iv++)
                                    {
                                        if (RQV[iv].Length == 0)
                                            continue;

                                        if (!(RQV[iv].ToLower().Equals(EQV[iv]) || EQV[iv].StartsWith(RQV[iv].ToLower())))
                                        {
                                            //---
                                            //if (iOnlineField==iv)
                                            //{
                                            //    bEquals = false;
                                            //    if (bNeedOnlineCompare)
                                            //    {
                                            //        if (lastSeenHasValue)
                                            //        {
                                            //            if (RQV[iv].ToLower().Equals("hour"))
                                            //                bEquals = lastSeenOnline || DateTime.Now.Subtract(lastSeen).TotalHours <= 1;
                                            //            else if (RQV[iv].ToLower().Equals("day"))
                                            //                bEquals = lastSeenOnline || DateTime.Now.Subtract(lastSeen).TotalDays <= 1;
                                            //            else if (RQV[iv].ToLower().Equals("week"))
                                            //                bEquals = lastSeenOnline || DateTime.Now.Subtract(lastSeen).TotalDays <= 7;
                                            //            else if (RQV[iv].ToLower().Equals("month"))
                                            //                bEquals = lastSeenOnline || DateTime.Now.Subtract(lastSeen).TotalDays <= 30;
                                            //        }
                                            //    }

                                            //    if (!bEquals)
                                            //        break;

                                            //}
                                            //else 
                                            if (iAgeField == iv)
                                            {
                                                String sAge = EQV[iv];
                                                if (sUnknownAge.Equals(sAge))
                                                {
                                                    bEquals = bAcceptEmptyAge;
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        int iAge = Convert.ToInt32(sAge);
                                                        if (iAge < iAcceptAgeMin || iAge > iAcceptAgeMax)
                                                            bEquals = false;
                                                    }
                                                    catch
                                                    {
                                                        bEquals = false;
                                                    }
                                                }

                                                if (!bEquals)
                                                    break;
                                            }
                                            else
                                            {
                                                //+---
                                                if (RQV[iv].IndexOf("~") >= 0 || RQV[iv].IndexOf("|") >= 0)
                                                {
                                                    bEquals = false;
                                                    String sFilter = RQV[iv];
                                                    bool bInverse = false;
                                                    String sValue = EQV[iv].Trim();
                                                    if (sFilter[0] == '~')
                                                    {
                                                        bInverse = true;
                                                        bEquals = true;
                                                        sFilter = sFilter.Substring(1);
                                                    }
                                                    if (sFilter.Length > 0)
                                                    {
                                                        if (sFilter[sFilter.Length - 1] != '|')
                                                            sFilter += "|";

                                                        while (sFilter.Length > 0)
                                                        {
                                                            String sPart = sFilter.Substring(0, sFilter.IndexOf("|")).Trim();
                                                            sFilter = sFilter.Substring(sFilter.IndexOf("|") + 1).Trim();
                                                            if (!bInverse)
                                                            {
                                                                if (sPart.Equals(sValue))
                                                                {
                                                                    bEquals = true;
                                                                    break;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (sPart.Equals(sValue))
                                                                {
                                                                    bEquals = false;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }

                                                }
                                                else if (RQV[iv].IndexOf("-") >= 0)
                                                {
                                                    String sval = RQV[iv].Trim();
                                                    String svalMin = sval.Substring(0, sval.IndexOf("-")).Trim();
                                                    String svalMax = sval.Substring(sval.IndexOf("-") + 1).Trim();
                                                    long ivalmin = 0;
                                                    long ivalmax = 9999999999;
                                                    try
                                                    {
                                                        if (svalMin.Length > 0)
                                                            ivalmin = Convert.ToInt32(svalMin);
                                                        if (svalMax.Length > 0)
                                                            ivalmax = Convert.ToInt32(svalMax);

                                                        sval = EQV[iv].Trim();
                                                        try
                                                        {
                                                            int ival = Convert.ToInt32(sval);
                                                            if (ival < ivalmin || ival > ivalmax)
                                                                bEquals = false;
                                                        }
                                                        catch
                                                        {
                                                            bEquals = false;
                                                        }
                                                    }
                                                    catch
                                                    {
                                                        bEquals = false;
                                                    }
                                                }
                                                else
                                                    bEquals = false;
                                                //---
                                                if (!bEquals)
                                                    break;
                                                //+---
                                            }
                                        }
                                    }
                                }

                                if (bEquals)
                                {
                                    if (bDataFileExists && bModeImport)
                                    {
                                        if (!bAcceptToAll)
                                        {
                                            bAcceptExisted = false;
                                            FormPersoneExists fpe = new FormPersoneExists();
                                            fpe.Setup(mFormMain, scpuID.Length > 0 ? scpuID : iPersUserID.ToString(), sUID, sUName);
                                            System.Windows.Forms.DialogResult dr = fpe.ShowDialog();
                                            if (dr == System.Windows.Forms.DialogResult.OK)
                                                bAcceptExisted = true;
                                            else if (dr == System.Windows.Forms.DialogResult.Cancel)
                                                bAcceptExisted = false;
                                            else if (dr == System.Windows.Forms.DialogResult.Abort)
                                            {
                                                bAcceptExisted = false;
                                                e.Cancel = true;
                                                break;
                                            }

                                            bAcceptToAll = fpe.checkBoxToAll.Checked;
                                        }

                                        if (!bAcceptExisted)
                                            bEquals = false;
                                    }
                                }

                                if (scpuID.Equals(sUID))
                                    bEquals = false;

                                if (bEquals)
                                {

                                    File.WriteAllLines(Path.Combine(FormMain.sDataPath, sAttrFileName), lstContHarValues, Encoding.UTF8);

                                    if (mFormEditContactsDB != null)
                                    {
                                        bool bShowAddToList = true;

                                        if (scpuID.Length > 0)
                                        {
                                            if (!iPersUserID.ToString().Equals(scpuID))
                                            {
                                                bShowAddToList = false;
                                                mFormEditContactsDB.ExternalContactsList_AddUser(scpuID, sUID, sUName);
                                            }
                                        }

                                        if (bShowAddToList)
                                        {
                                            mFormEditContactsDB.UpdateProgress_AddUserToVisualList(sUID, sUName, mFormEditContactsDB.UpdateProgress_GetVisualListIdx(sUID));
                                            mFormEditContactsDB.ContactsList_AddUser(sUID, sUName, "");
                                        }
                                    }
                                    else if (mFormEditGroupsDB != null)
                                    {
                                        mFormEditGroupsDB.ExternalContactsList_AddUser(scpuID, sUID, sUName);
                                    }
                                    else if (mFormInitDialogsWithContactersFromGroups != null)
                                    {
                                        mFormInitDialogsWithContactersFromGroups.ExternalContactsList_AddUser(scpuID, sUID, sUName);
                                    }
                                    else if (mFormEditPersonenDB != null)
                                    {
                                        String sULogin = mFormEditPersonenDB.PersonenList_GetUserField(0, sUID, FormEditPersonenDB.USERFIELD_LOGIN);
                                        String sUPwd = mFormEditPersonenDB.PersonenList_GetUserField(0, sUID, FormEditPersonenDB.USERFIELD_PASSWORD);
                                        mFormEditPersonenDB.UpdateProgress_AddUserToVisualList(mFormEditPersonenDB.PersonenList_AddUser(0, sUID, sUName, sULogin, sUPwd), mFormEditPersonenDB.UpdateProgress_GetVisualListIdx(0, sUID));
                                    }
                                    ContactsAdded++;
                                    state.ContactsAdded = ContactsAdded;
                                    worker.ReportProgress(0, state);
                                    if (sPersUsersIDs.Length > 0)
                                    {
                                        string sidcscpuID = sPersUsersIDs.Substring(0, sPersUsersIDs.IndexOf("|"));
                                        sPersUsersIDs = sPersUsersIDs.Substring(sPersUsersIDs.IndexOf("|") + 1);
                                        sPersUsersIDs = sPersUsersIDs + sidcscpuID + "|";
                                    }
                                }
                                //}
                            }//if (usrAdr != null)
                        }//else ... (worker.CancellationPending)
                    }//foreach (long usrID in usrToImportIDs)

                    if (iCycleGroupsContacterImport < 0)
                        break;
                }
                while (true);
            }
            catch (Exception exp)
            {
                if (mFormEditContactsDB != null)
                    mFormEditContactsDB.ExceptionToLogList("importcontacts", iPersUserID.ToString(), exp);
            }
            finally
            {

            }
        }

        private long AutorizeVK(String sUsrSelLogin, String sUsrSelPwd)
        {
            if (mFormMain.fwbVKontakte == null)
            {
                mFormMain.fwbVKontakte = new FormWebBrowser(mFormMain, true);
                mFormMain.fwbVKontakte.Init();
            }

            //stopTimers();

            mFormMain.fwbVKontakte.Setup(sUsrSelLogin, sUsrSelPwd, WebBrowserCommand.LoginPersone);
            if (!mFormMain.fwbVKontakteFirstShow)
            {
                mFormMain.fwbVKontakteFirstShow = true;
                mFormMain.fwbVKontakte.Show();
            }
            mFormMain.fwbVKontakte.WaitResult();
            long persID = mFormMain.fwbVKontakte.loggedPersoneID;
            return persID;
        }

    }
}

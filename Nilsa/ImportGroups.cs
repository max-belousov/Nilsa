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
    public class ImportGroups
    {
        // Object to store the current state, for passing to the caller.
        public class CurrentState
        {
            public int GroupsImported;
            public int GroupsAdded;
            public int GroupsTotal;
            public string sUName;
        }

        public string sGroupID;
        public long iPersUserID;
        public string sPersUsersIDs;
        public List<String> lstContHar;
        public List<String> lstFilterHar;
        public int iGroupHarCount;

        private int GroupsImported;
        private int GroupsTotal;
        private int GroupsAdded;
        public int maxGroupsCount;

        private FormEditGroupsDB mFormEditGroupsDB = null;
        private FormInitDialogsWithContactersFromGroups mFormInitDialogsWithContactersFromGroups = null;
        private FormMain mFormMain = null;

        private string sUnknownAge;
        private string groupSearchQuery = "";
        public int nudDuplicatesAction = 0;

        public ImportGroups(FormEditGroupsDB formEditGroupsDB, FormMain _formmain)
        {
            mFormEditGroupsDB = formEditGroupsDB;
            mFormMain = _formmain;

        }

        public ImportGroups(FormInitDialogsWithContactersFromGroups formInitDialogsWithContactersFromGroups, FormMain _formmain)
        {
            mFormInitDialogsWithContactersFromGroups = formInitDialogsWithContactersFromGroups;
            mFormMain = _formmain;

        }

        public void ImportContacts(System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
        {
            // Initialize the variables.
            CurrentState state = new CurrentState();
            bool bSearchGroup = false;
            try
            {
                int totalCount = 0;
                var usrToImportIDs = new List<long>();

                /*
                if (sGroupID.StartsWith("UserID="))
                {
                    sGroupID = sGroupID.Substring(7);
                    long iIUID = Convert.ToInt64(sGroupID);
                    do
                    {
                        try
                        {
                            var users = FormMain.api.Groups.Get(iIUID);
                            foreach (VkNet.Model.Group usrAdr in users)
                                usrToImportIDs.Add(usrAdr.Id);
                            totalCount = users.Count;
                            break;
                        }
                        catch (Exception exp)
                        {
                            mFormMain.ExceptionToLogList("ImportGroups", "ImportGroups", exp);
                            break;
                        }
                    }
                    while (true);
                }
                else if (sGroupID.StartsWith("UserIDs="))
                {
                    sGroupID = sGroupID.Substring(8);
                    while (sGroupID.Length > 0)
                    {
                        long iIUID = Convert.ToInt64(sGroupID.Substring(0, sGroupID.IndexOf("|")));
                        sGroupID = sGroupID.Substring(sGroupID.IndexOf("|") + 1);

                        do
                        {
                            try
                            {
                                var users = FormMain.api.Groups.Get(iIUID);
                                foreach (VkNet.Model.Group usrAdr in users)
                                {
                                    if (!usrToImportIDs.Contains(usrAdr.Id))
                                    {
                                        usrToImportIDs.Add(usrAdr.Id);
                                        totalCount++;
                                    }
                                }
                                break;
                            }
                            catch (Exception exp)
                            {
                                mFormMain.ExceptionToLogList("ImportGroups", "ImportGroups", exp);
                                break;
                            }
                        }
                        while (true);
                    }
                }
                else if (sGroupID.StartsWith("UIDs="))
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
                else if (sGroupID.StartsWith("Search="))
                {
                    bSearchGroup = true;
                    sGroupID = sGroupID.Substring(7);
                    groupSearchQuery = sGroupID;
                    String ssQuery = sGroupID;
                    do
                    {
                        try
                        {
                            var users = FormMain.api.Groups.Search(ssQuery, out totalCount, null, 1000);
                            foreach (VkNet.Model.Group usrAdr in users)
                                usrToImportIDs.Add(usrAdr.Id);
                            totalCount = users.Count;
                            break;
                        }
                        catch (Exception exp)
                        {
                            mFormMain.ExceptionToLogList("ImportGroups", "ImportGroups", exp);
                            break;
                        }
                    }
                    while (true);
                }
                */

                GroupsImported = 0;
                GroupsAdded = 0;
                GroupsTotal = totalCount;

                String[] RQV = new String[iGroupHarCount];
                String[] EQV = new String[iGroupHarCount];
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

                state.GroupsImported = GroupsImported;
                state.GroupsAdded = GroupsAdded;
                state.GroupsTotal = GroupsTotal;
                state.sUName = "";
                worker.ReportProgress(0, state);

                foreach (long usrID in usrToImportIDs)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        if (GroupsAdded >= maxGroupsCount)
                            break;

                        GroupsImported++;
                        state.GroupsImported = GroupsImported;

                        String sUID = usrID.ToString();

                        //if (mFormEditGroupsDB.ContactsList_GetUserIdx(sUID) < 0)
                        //{
                        VkNet.Model.Group usrAdr = null;

                        /*
                        do
                        {
                            try
                            {
                                usrAdr = FormMain.api.Groups.GetById(Convert.ToInt64(sUID), GroupsFields.CanCreateTopic | GroupsFields.CanPost | GroupsFields.CanSeelAllPosts | GroupsFields.Counters | GroupsFields.Description | GroupsFields.MembersCount);
                                break;
                            }
                            catch (Exception exp)
                            {
                                mFormMain.ExceptionToLogList("ImportGroups", "ImportGroups", exp);
                                break;
                            }
                        }
                        while (true);
                        */

                        if (usrAdr != null)
                        {
                            String sUName = usrAdr.Name;

                            state.sUName = sUName;
                            worker.ReportProgress(0, state);

                            List<String> lstContHarValues = new List<String>();

                            int nContHarCount = 0;
                            if (mFormEditGroupsDB != null)
                                nContHarCount = mFormEditGroupsDB.iGroupHarCount;
                            else if (mFormInitDialogsWithContactersFromGroups != null)
                                nContHarCount = mFormInitDialogsWithContactersFromGroups.iGroupHarCount;

                            String sAttrFileName = "";
                            String scpuID = "";
                            if (mFormEditGroupsDB != null)
                            {

                                if (sPersUsersIDs.Length > 0)
                                {
                                    scpuID = sPersUsersIDs.Substring(0, sPersUsersIDs.IndexOf("|"));
                                    //if (bRotate)
                                    //{
                                    //    bRotate = false;
                                    //    sPersUsersIDs = sPersUsersIDs.Substring(sPersUsersIDs.IndexOf("|") + 1);
                                    //    sPersUsersIDs = sPersUsersIDs + scpuID + "|";
                                    //}
                                    sAttrFileName = "grp_" + FormMain.getSocialNetworkPrefix() + scpuID + "_" + sUID + ".txt";
                                }
                                else
                                    sAttrFileName = "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt";
                            }
                            else if (mFormInitDialogsWithContactersFromGroups != null)
                            {

                                if (sPersUsersIDs.Length > 0)
                                {
                                    scpuID = sPersUsersIDs.Substring(0, sPersUsersIDs.IndexOf("|"));
                                    sAttrFileName = "grp_" + FormMain.getSocialNetworkPrefix() + scpuID + "_" + sUID + ".txt";
                                }
                                else
                                    sAttrFileName = "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt";
                            }


                            // Check exist - start
                            bool bGroupExist = false;
                            if (bSearchGroup && nudDuplicatesAction == 1)
                            {
                                if (sPersUsersIDs.Length > 0)
                                {
                                    string sPersUserPoolIDs = sPersUsersIDs;
                                    while (sPersUserPoolIDs.Length > 0)
                                    {
                                        string sidcscpuID = sPersUserPoolIDs.Substring(0, sPersUserPoolIDs.IndexOf("|"));
                                        sPersUserPoolIDs = sPersUserPoolIDs.Substring(sPersUserPoolIDs.IndexOf("|") + 1);
                                        if (File.Exists(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + sidcscpuID + "_" + sUID + ".txt")))
                                        {
                                            bGroupExist = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (File.Exists(Path.Combine(FormMain.sDataPath, "grp_" + FormMain.getSocialNetworkPrefix() + iPersUserID.ToString() + "_" + sUID + ".txt")))
                                        bGroupExist = true;
                                }

                                if (bGroupExist)
                                    continue;
                            }
                            // Chack exist - end

                            if (File.Exists(Path.Combine(FormMain.sDataPath, sAttrFileName)))
                            {
                                var srcFile = File.ReadAllLines(Path.Combine(FormMain.sDataPath, sAttrFileName));
                                lstContHarValues = new List<String>(srcFile);
                                for (int i = 0; i < nContHarCount; i++)
                                {
                                    String sCV = lstContHar[i].Substring(lstContHar[i].IndexOf("|") + 1);
                                    if (sCV.Length > 0)
                                        lstContHarValues[i] = lstContHar[i];
                                }
                            }
                            else
                            {
                                for (int i = 0; i < nContHarCount; i++)
                                    lstContHarValues.Add(lstContHar[i]);
                            }

                            for (int i = 0; i < nContHarCount; i++)
                            {
                                if (lstContHarValues[i].IndexOf("#can_post#") > 0)
                                {
                                    String svkv = usrAdr.CanPost ? "Yes" : "No";
                                    lstContHarValues[i] = lstContHarValues[i].Replace("#can_post#", svkv);
                                }
                                else if (lstContHarValues[i].IndexOf("#members_count#") > 0)
                                {
                                    lstContHarValues[i] = lstContHarValues[i].Replace("#members_count#", usrAdr.MembersCount.HasValue ? usrAdr.MembersCount.ToString() : "");
                                }
                                else if (lstContHarValues[i].IndexOf("#description#") > 0)
                                {
                                    lstContHarValues[i] = lstContHarValues[i].Replace("#description#", usrAdr.Description);
                                }
                                else if (lstContHarValues[i].IndexOf("#can_see_all_posts#") > 0)
                                {
                                    String svkv = usrAdr.CanSeelAllPosts ? "Yes" : "No";
                                    lstContHarValues[i] = lstContHarValues[i].Replace("#can_see_all_posts#", svkv);
                                }
                                else if (lstContHarValues[i].IndexOf("#can_create_topic#") > 0)
                                {
                                    String svkv = usrAdr.CanCreateTopic ? "Yes" : "No";
                                    lstContHarValues[i] = lstContHarValues[i].Replace("#can_create_topic#", svkv);
                                }
                                else if (lstContHarValues[i].IndexOf("#is_member#") > 0)
                                {
                                    String svkv = usrAdr.IsMember.HasValue ? (usrAdr.IsMember.Value ? "Yes" : "No"):"Unknown";
                                    lstContHarValues[i] = lstContHarValues[i].Replace("#is_member#", svkv);
                                }
                                else if (lstContHarValues[i].IndexOf("#search_query#") > 0)
                                {
                                    lstContHarValues[i] = lstContHarValues[i].Replace("#search_query#", groupSearchQuery);
                                }
                                else if (lstContHarValues[i].IndexOf("#is_closed#") > 0)
                                {
                                    String svkv = usrAdr.IsClosed.HasValue ? (usrAdr.IsClosed==GroupPublicity.Closed ? "Closed" :(usrAdr.IsClosed == GroupPublicity.Private ? "Private":"Public")) :"Unknown";
                                    lstContHarValues[i] = lstContHarValues[i].Replace("#is_closed#", svkv);
                                }
                                else if (lstContHarValues[i].IndexOf("#is_admin#") > 0)
                                {
                                    String svkv = usrAdr.IsAdmin ? "Yes" : "No";
                                    lstContHarValues[i] = lstContHarValues[i].Replace("#is_admin#", svkv);
                                }
                                else if (lstContHarValues[i].IndexOf("#type#") > 0)
                                {
                                    String svkv = "Unknown";
                                    if (usrAdr.Type == VkNet.Enums.SafetyEnums.GroupType.Group)
                                        svkv = "Group";
                                    else if (usrAdr.Type == VkNet.Enums.SafetyEnums.GroupType.Event)
                                        svkv = "Event";
                                    else if (usrAdr.Type == VkNet.Enums.SafetyEnums.GroupType.Page)
                                        svkv = "Page";
                                    lstContHarValues[i] = lstContHarValues[i].Replace("#type#", svkv);
                                }
                                else if (lstContHarValues[i].IndexOf("#clear#") > 0)
                                {
                                    lstContHarValues[i] = lstContHarValues[i].Replace("#clear#", "");
                                }
                                if (lstContHarValues[i].Substring(lstContHarValues[i].IndexOf("|") + 1).Contains("|"))
                                {
                                    String sRID = lstContHarValues[i].Substring(0, lstContHarValues[i].IndexOf("|"));
                                    String sRVAL = lstContHarValues[i].Substring(lstContHarValues[i].IndexOf("|") + 1);
                                    lstContHarValues[i] = sRID+"|"+sRVAL.Replace('|', 'I');
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

                                    if (!RQV[iv].Equals(EQV[iv]))
                                    {
                                        //---
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

                            if (bEquals)
                            {
                                File.WriteAllLines(Path.Combine(FormMain.sDataPath, sAttrFileName), lstContHarValues, Encoding.UTF8);

                                if (mFormEditGroupsDB != null)
                                {
                                    bool bShowAddToList = true;

                                    if (scpuID.Length > 0)
                                    {
                                        if (!iPersUserID.ToString().Equals(scpuID))
                                        {
                                            bShowAddToList = false;
                                            mFormEditGroupsDB.ExternalGroupsList_Add(scpuID, sUID, sUName);
                                        }
                                    }

                                    if (bShowAddToList)
                                    {
                                        mFormEditGroupsDB.UpdateProgress_AddToVisualList(sUID, sUName, mFormEditGroupsDB.UpdateProgress_GetVisualListIdx(sUID));
                                        mFormEditGroupsDB.GroupsList_Add(sUID, sUName);
                                    }
                                }
                                else if (mFormInitDialogsWithContactersFromGroups != null)
                                {
                                    bool bShowAddToList = true;

                                    if (scpuID.Length > 0)
                                    {
                                        if (!iPersUserID.ToString().Equals(scpuID))
                                        {
                                            bShowAddToList = false;
                                            mFormInitDialogsWithContactersFromGroups.ExternalGroupsList_Add(scpuID, sUID, sUName);
                                        }
                                    }

                                    if (bShowAddToList)
                                    {
                                        mFormInitDialogsWithContactersFromGroups.ExternalGroupsList_Add(iPersUserID.ToString(), sUID, sUName);
                                    }
                                }
                                
                                GroupsAdded++;
                                state.GroupsAdded = GroupsAdded;
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
            }
            catch (Exception exp)
            {
                if (mFormEditGroupsDB != null)
                    mFormEditGroupsDB.ExceptionToLogList("importgroups", iPersUserID.ToString(), exp);
            }
            finally
            {

            }



        }


    }
}

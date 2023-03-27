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
    public class WorkGroups
    {
        // Object to store the current state, for passing to the caller.
        public class CurrentState
        {
            public int Worked;
            public int Total;
            public int Deleted;
        }

        public string sGroupID;

        private int Worked;
        private int Total;
        private int Deleted;

        private FormEditGroupsDB mFormEditGroupsDB = null;
        private FormMain mFormMain = null;

        public WorkGroups(FormEditGroupsDB formEditGroupsDB, FormMain _formmain)
        {
            mFormEditGroupsDB = formEditGroupsDB;
            mFormMain = _formmain;

        }

        public void doWork(System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
        {
            // Initialize the variables.
            CurrentState state = new CurrentState();
            try
            {
                int totalCount = 0;
                var usrGroupMembersIDs = new List<long>();
                int iOffs = 0;

                /*
                do
                {
                    try
                    {

                        var ids = FormMain.api.Groups.GetMembers(Convert.ToUInt32(sGroupID), out totalCount, 1000, iOffs, VkNet.Enums.SafetyEnums.GroupsSort.IdAsc);
                        if (ids.Count > 0)
                        {
                            iOffs += 1000;
                            foreach (long id in ids)
                                usrGroupMembersIDs.Add(id);
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
                */

                Worked = 0;
                Total = usrGroupMembersIDs.Count;
                Deleted = 0;

                state.Worked = Worked;
                state.Total = Total;
                worker.ReportProgress(0, state);

                foreach (long usrID in usrGroupMembersIDs)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        Worked++;
                        state.Worked = Worked;
                        worker.ReportProgress(0, state);

                        VkNet.Model.User usrAdr = null;

                        /*
                        do
                        {
                            try
                            {
                                usrAdr = FormMain.api.Users.Get(usrID, ProfileFields.FirstName | ProfileFields.LastName);
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

                        if (usrAdr != null)
                        {
                            if (usrAdr.IsDeactivated)
                            {
                                try
                                {
                                    //FormMain.api.Groups.removeUser(Convert.ToInt64(sGroupID), usrID);
                                    Deleted++;
                                    state.Deleted = Deleted;
                                    worker.ReportProgress(0, state);
                                }
                                catch (Exception exp)
                                {
                                    mFormMain.ExceptionToLogList("removeUser", "removeUser", exp);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                mFormEditGroupsDB.ExceptionToLogList("workgroups", sGroupID, exp);
            }
            finally
            {

            }



        }


    }
}

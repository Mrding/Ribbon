using System;
using System.Collections.Generic;
using Luna.Data.Event;
using Luna.Shifts.Domain;
using NHibernate;
using NHibernate.Event;

namespace Luna.Shifts.Data.Listeners
{
    public class TermBackupListener : IPreDeleteEventListener, IPostInsertEventListener
    {
        private static bool IsEnabled;

        private static ISession DeleteSession;

        private static IDictionary<string, string> SqlCaches = new Dictionary<string, string>();

        public static void Enable()
        {
            IsEnabled = true;
            SqlCaches.Clear();
            DeleteSession = null;
        }

        public static void Disable()
        {
            IsEnabled = false;
            SqlCaches.Clear();
            DeleteSession = null;
        }


        #region IPreDeleteEventListener Members

        public bool OnPreDelete(PreDeleteEvent @event)
        {
            if (IsEnabled)
            {
                var entity = default(Term);
                if (@event.TryCatchEntity(ref entity))
                {
                    if (DeleteSession == null)
                        DeleteSession = @event.Persister.Factory.GetCurrentSession();

                    var employeeId = entity.GetSnapshotValue<Guid>("EmployeeId");
                    var key = string.Format("{0}{1}", employeeId, entity.Start.Date);

                    if (!SqlCaches.ContainsKey(key))
                    {
                        var sql = string.Format("delete BackupTerm where BackupTerm.EmployeeId ='{0}' and datediff(d,starttime,'{1:yyyy/MM/dd HH:mm}')=0", employeeId, entity.Start.Date);
                        SqlCaches[key] = sql;
                        DeleteSession.CreateSQLQuery(sql).ExecuteUpdate();
                    }
                }
            }
            return false;
        }

        #endregion

        #region IPreInsertEventListener Members

        public void OnPostInsert(PostInsertEvent @event)
        {
            if (IsEnabled)
            {
                var entity = default(Term);
                if (@event.TryCatchEntity(ref entity))
                {
                    var statelessSession = @event.Persister.Factory.OpenStatelessSession();
                    using (var tx = statelessSession.BeginTransaction())
                    {
                        var employeeId = entity.GetSnapshotValue<Guid>("EmployeeId");

                        var backup = new BackupTerm(entity.Id, employeeId, entity.Start, entity.End, entity.Text, entity.Background, entity.Level);

                        if (entity.Bottom != null)
                            backup.ParentTermId = entity.Bottom.Id;

                        statelessSession.Insert(backup);
                        tx.Commit();
                    }
                }
            }
        }

        #endregion
    }
}

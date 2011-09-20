using System;
using Luna.Common;
using Luna.Common.Constants;
using Luna.Core;
using Luna.Data.Event;
using Luna.Shifts.Domain;
using NHibernate;
using NHibernate.Event;
using NHibernate.Persister.Entity;

namespace Luna.Shifts.Data.Listeners
{
    public class TermAlteringListener : IPostUpdateEventListener, IPostDeleteEventListener, IPostInsertEventListener, IPreUpdateEventListener
    {
        private static string _alterWay = "unspecified";
        private static string _batchAlteringComments;

        public static void Batching(string comments)
        {
            _alterWay = "batch";
            _batchAlteringComments = comments;
        }

        public static void Manually()
        {
            _alterWay = "manually";
            _batchAlteringComments = string.Empty;
        }

        //private object[] GetOldEntity(IStatelessSession statelessSession, Int64 entityId)
        //{
        //    var oldentity = statelessSession.CreateSQLQuery(@"select StartTime,FinishTime,Locked,TimeBoxId from Term where TermId=:id")
        //        .SetInt64("id", entityId)
        //        .UniqueResult<object[]>();
        //    return oldentity;
        //}

        private static string FormatTime(DateTime start, DateTime end)
        {
            return string.Format("{0:yy/MM/dd HH:mm}~{1:yy/MM/dd HH:mm}", start, end);
        }

        private void SaveLog(IStatelessSession session, TermLog entity)
        {
            if (session == null) return;

            using (var tx = session.BeginTransaction())
            {
                var currentEmployee = ApplicationCache.Get<Entity>(Global.LoginEmployee);
                if (currentEmployee != null)
                    entity.AlterEmployeeId = currentEmployee.Id;
                session.Insert(entity);
                
                tx.Commit();
            }
        }
        public void OnPostDelete(PostDeleteEvent @event)
        {
            var entity = default(Term);

            if (@event.TryCatchEntity(ref entity))
            {
                var statelessSession = @event.Persister.Factory.OpenStatelessSession();

                var log = new TermLog
                              {
                                  EmployeeId = entity.GetSnapshotValue<Guid>("EmployeeId"),
                                  Action = "D",
                                  Type = entity.GetType().Name,
                                  Name = entity.Text,
                                  Category = _alterWay,
                                  OldTime = FormatTime(entity.Start, entity.End),
                                  NewTime = null,
                                  Remark = _batchAlteringComments
                              };
                SaveLog(statelessSession, log);
                entity.EndEdit();
            }
        }



        public void OnPostUpdate(PostUpdateEvent @event)
        {
            var entity = default(Term);

            if (@event.TryCatchEntity(ref entity))
            {
                var statelessSession = @event.Persister.Factory.OpenStatelessSession();

                var start = entity.GetSnapshotValue<DateTime>("Start");
                var end = entity.GetSnapshotValue<DateTime>("End");
                var locked = entity.GetSnapshotValue<bool>("Locked");

                if (start == entity.Start && end == entity.End && locked == entity.Locked) // no changed
                    return;

                //var length = (entity.End - entity.Start).TotalMinutes;
                //var oldtime = (end - start).TotalMinutes;

                var log = new TermLog
                              {
                                  SourceId = entity.Id,
                                  EmployeeId = entity.GetSnapshotValue<Guid>("EmployeeId"),
                                  Type = entity.GetType().Name,
                                  Name = entity.Text,
                                  Category = _alterWay,
                                  OldTime = FormatTime(start, end),
                                  NewTime = FormatTime(entity.Start, entity.End),
                                  Remark = _batchAlteringComments
                              };
                log.Action = entity.GetAction();

                SaveLog(statelessSession, log);
                entity.EndEdit();
            }
        }

        #region IPostInsertEventListener Members

        public void OnPostInsert(PostInsertEvent @event)
        {
            var entity = default(Term);

            if (@event.TryCatchEntity(ref entity))
            {
                var statelessSession = @event.Persister.Factory.OpenStatelessSession();

                var log = new TermLog
                              {
                                  SourceId = entity.Id,
                                  EmployeeId = entity.GetSnapshotValue<Guid>("EmployeeId"),
                                  Action = "I",
                                  Type = entity.GetType().Name,
                                  Name = entity.Text,
                                  Category = _alterWay,
                                  OldTime = null,
                                  NewTime = FormatTime(entity.Start, entity.End),
                                  Remark = _batchAlteringComments
                              };

                SaveLog(statelessSession, log);
            }
        }

        #endregion

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            var entity = default(Term);
            if (_alterWay == "batch" && @event.TryCatchEntity(ref entity) && entity.GetAction() != string.Empty)
            {

                var tag = string.Format("{0}{1}", entity.Tag, _batchAlteringComments);
                Set(@event.Persister, @event.State, "Tag", tag);
                entity.Tag = tag;
            }
            return false;
        }

        private static void Set(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }

    }
}
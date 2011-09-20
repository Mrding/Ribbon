using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using NHibernate.Engine;

namespace Luna.Data
{
    public static class NHibernateManager
    {
        public static ISessionFactory Factory
        {
            get { return ServiceLocator.Current.GetInstance<ISessionFactory>(); }
        }

        public static ISessionFactoryImplementor FactoryImplementor
        {
            get { return Factory as ISessionFactoryImplementor; }
        }

        public static ISession CurrentSession
        {
            get { return Factory.GetCurrentSession(); }
        }

        public static ISessionImplementor CurrentSessionImplementor
        {
            get { return CurrentSession.GetSessionImplementation(); }
        }

        public static IDbConnection GetConnection()
        {
            return FactoryImplementor.ConnectionProvider.GetConnection();
        }

        public static void CloseConnection(IDbConnection conn)
        {
            FactoryImplementor.ConnectionProvider.CloseConnection(conn);
        }

        public static IDataReader ExecuteReader(string sql)
        {
            return CurrentSession.GetSessionImplementation().Batcher.ExecuteReader(new SqlCommand(sql));
        }

        public static void CloseReader(IDataReader reader)
        {
            CurrentSession.GetSessionImplementation().Batcher.CloseReader(reader);
        }
    }
}

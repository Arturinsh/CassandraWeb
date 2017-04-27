using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cassandra;
using CassandraWebTest.Models;

namespace CassandraWebTest.DAO
{
    public interface ICassandraDAO
    {
        ISession GetSession();
    }

    public class CassandraDAO : ICassandraDAO
    {
        private static Cluster Cluster;
        private static ISession Session;

        public CassandraDAO()
        {
            SetCluster();
        }

        private void SetCluster()
        {
            if (Cluster == null)
            {
                Cluster = Connect();
            }
        }

        public ISession GetSession()
        {
            if (Cluster == null)
            {
                SetCluster();
                Session = Cluster.Connect("test_keyspace");
                Session.UserDefinedTypes.Define(
                  UdtMap.For<Vote>(),  UdtMap.For<Comment>());
            }
            else if (Session == null)
            {
                Session = Cluster.Connect("test_keyspace");
                Session.UserDefinedTypes.Define(
                   UdtMap.For<Vote>(), UdtMap.For<Comment>());
            }

            return Session;
        }

        private Cluster Connect()
        {
            string user = getAppSetting("cassandraUser");
            string pwd = getAppSetting("cassandraPassword");
            string node = getAppSetting("cassandraNode");
            

            QueryOptions queryOptions = new QueryOptions()
                .SetConsistencyLevel(ConsistencyLevel.One);

            Cluster cluster = Cluster.Builder()
                .AddContactPoint(node)
               // .WithCredentials(user, pwd)
                .WithQueryOptions(queryOptions)
                .Build();

            return cluster;
        }

        private string getAppSetting(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }
    }
}
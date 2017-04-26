using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cassandra.Mapping.Attributes;

namespace CassandraWebTest.Models
{
    [Table("test_keyspace.users")]
    public class UsersModels
    {
        public Guid id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string fbkey { get; set; }
        public string gkey { get; set; }
    }
}
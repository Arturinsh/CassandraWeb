using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cassandra.Mapping.Attributes;
using Cassandra;

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
        public bool notification { get; set; }
        public int employeeid { get; set; }
    }

    [Table("test_keyspace.articles")]
    public class ArticlesModels
    {
        private DateTimeOffset utcTimestamp;
        public Guid id { get; set; }
        public DateTimeOffset timestamp
        {
            get { return utcTimestamp.ToLocalTime(); }
            set { this.utcTimestamp = value; }
        }
        public int view_count { get; set; }
        public string author { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public List<Comment> comments { get; set; }
    }

    public class Comment
    {
        public Guid id { get; set; }
        public string author { get; set; }
        public string commentary { get; set; }
        public IEnumerable<Vote> votes { get; set; }

        public Comment()
        {
            id = Guid.NewGuid();
        }
    }

    public class Vote
    {
        public string username { get; set; }
        public int vote { get; set; }
    }

    public class CommentModel
    {
        public string commentary { get; set; }
        public int vote { get; set; }
    }
}
using CassandraWebTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cassandra;
using Cassandra.Mapping;
using System.Threading.Tasks;
using Cassandra.Data.Linq;

namespace CassandraWebTest.DAO
{
    public interface IUsersDAO
    {
        Task<IEnumerable<UsersModels>> getUsers();

        Task addUser(UsersModels user);

        void deleteUser(UsersModels user);

        bool checkUserValidation(string username, string password);

        bool checkUserNameAvailability(string username);

        UsersModels GetUserByName(string id);

        UsersModels updateUser(UsersModels user);

        UsersModels FindById(string id);

        UsersModels FindByFacebookKey(string key);

        UsersModels FindByGoogleKey(string key);
    }

    public class UsersDAO : IUsersDAO
    {
        protected readonly ISession session;
        protected readonly IMapper mapper;

        public UsersDAO()
        {
            ICassandraDAO cassandraDAO = new CassandraDAO();
            session = cassandraDAO.GetSession();
            mapper = new Mapper(session);
        }

        public async Task<IEnumerable<UsersModels>> getUsers()
        {
            return await mapper.FetchAsync<UsersModels>();
        }

        public async Task addUser(UsersModels user)
        {
            user.id = Guid.NewGuid();
            await mapper.InsertAsync(user);
        }

        public void deleteUser(UsersModels user)
        {
            mapper.Delete<UsersModels>("WHERE id = ?", user.id);
        }

        public bool checkUserValidation(string username, string password)
        {
            UsersModels model = mapper.FirstOrDefault<UsersModels>("WHERE username = ?", username);
            if (model != null && model.password == password)
                return true;
            //UsersModels model = mapper.Fetch<UsersModels>().Where(x=> x.password == password && x.username == username).First();
            //TODO change to return bool or check if list has any member
            return false;
        }

        public bool checkUserNameAvailability(string username)
        {
            UsersModels model = mapper.FirstOrDefault<UsersModels>("WHERE username = ?", username);
            if (model == null)
                return true;
            return false;
        }

        public UsersModels GetUserByName(string name)
        {
            return mapper.FirstOrDefault<UsersModels>("WHERE username = ?", name);
        }

        public UsersModels updateUser(UsersModels user)
        {
            //TODO if list emptu create new
            mapper.Update<UsersModels>("SET username = ?, password = ?, fbkey = ?, gkey = ? WHERE id = ?",
                user.username, user.password, user.fbkey, user.gkey, user.id);
            return user;
        }

        public UsersModels FindById(string id)
        {
            return mapper.FirstOrDefault<UsersModels>("WHERE id = ?", new Guid(id));
        }

        public UsersModels FindByFacebookKey(string key)
        {
            return mapper.FirstOrDefault<UsersModels>("WHERE fbkey = ?", key);
        }

        public UsersModels FindByGoogleKey(string key)
        {
            return mapper.FirstOrDefault<UsersModels>("WHERE gkey = ?", key);
        }
    }
}
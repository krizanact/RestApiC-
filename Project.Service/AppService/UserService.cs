using Project.Model.DatabaseConnector;
using Project.Model.Core;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Project.Service.Extension;
using System;
using System.Collections.Generic;

namespace Project.Service.AppService
{
    public interface IUserService
    {

        /// <summary>
        /// Returns list of users
        /// </summary>
        /// <returns></returns>
        Task<List<User>> GetUsers();

        /// <summary>
        /// Adds new user to database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task CreateUser(User user);

        /// <summary>
        /// Returns true if useranme already exists in database
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<bool> CheckUsername(string username);

        /// <summary>
        /// Returns user by his unique username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<User> GetUser(string username);

        /// <summary>
        /// Returns true if username and password are correct
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> CheckLogin(string username, string password);

    }

    public class UserService : RepositoryBase<User>, IRepository<User>, IUserService
    {

        private readonly IUnitOfWork _unitOfWork;

        public UserService(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<User>> GetUsers()
        {
            return await DbContext.Users.Include(r => r.Role).ToListAsync();
        }

        public async Task CreateUser(User user)
        {
            // Hash user password
            user.Password = user.Password.SHA512Hash();
            user.CreateDate = DateTime.Now;
            await Add(user);
            await _unitOfWork.Commit();
        }

        public async Task<bool> CheckUsername(string username)
        {
            return await DbContext.Users.Where(x => x.Username.Equals(username)).AnyAsync();
        }

        public async Task<User> GetUser(string username)
        {
            return await DbContext.Users.Include(r => r.Role).Where(x => x.Username.Equals(username)).SingleOrDefaultAsync();
        }

        public async Task<bool> CheckLogin(string username, string password)
        {
            return await DbContext.Users.Where(x => x.Username.Equals(username) && x.Password.Equals(password.SHA512Hash())).AnyAsync();
        }
    }
}

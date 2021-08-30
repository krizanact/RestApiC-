using Project.Model.DatabaseConnector;
using Project.Model.Core;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

namespace Project.Service.AppService
{
    public interface INewsService
    {


        /// <summary>
        /// Removes selected news from database
        /// </summary>
        /// <param name="news"></param>
        /// <returns></returns>
        Task DeleteNews(News news);

        /// <summary>
        /// Retruns true if selected id exists in database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> CheckIfIdExist(int id);


        /// <summary>
        /// Updates selected news data and returns edited object
        /// </summary>
        /// <param name="news"></param>
        /// <returns></returns>
        Task<News> EditNews(News news);

        /// <summary>
        /// Adds news to database and returns created object
        /// </summary>
        /// <param name="news"></param>
        /// <returns></returns>
        Task<News> CreateNews(News news);

        /// <summary>
        /// Returns single news details by unique id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<News> GetNews(int id);

        /// <summary>
        /// Returns list of news
        /// </summary>
        /// <returns></returns>
        Task<List<News>> GetNews();
      
    }

    public class NewsService : RepositoryBase<News>, IRepository<News>, INewsService
    {

        private readonly IUnitOfWork _unitOfWork;

        public NewsService(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task DeleteNews(News news)
        {
            Delete(news);
            await _unitOfWork.Commit();
        }

        public async Task<bool> CheckIfIdExist(int id)
        {
            return await DbContext.News.Where(x => x.Id == id).AnyAsync();
        }

        public async Task<News> EditNews(News news)
        {
            news.UpdateDate = DateTime.Now;
            Update(news);
            await _unitOfWork.Commit();

            return await GetNews(news.Id);
        }

        public async Task<News> CreateNews(News news)
        {
            news.CreateDate = DateTime.Now;
            await Add(news);
            await _unitOfWork.Commit();

            return await GetNews(news.Id);
        }

        public async Task<News> GetNews(int id)
        {
            return await DbContext.News.Include(u => u.User).Where(x => x.Id == id).SingleOrDefaultAsync();
        }

        public async Task<List<News>> GetNews()
        {
            return await DbContext.News.Include(u => u.User).ToListAsync();
        }
    }
}

using System.Threading.Tasks;
using Project.Model.Core;

namespace Project.Model.DatabaseConnector
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory _dbFactory;
        private DataBaseConnection _dbContext;

        public UnitOfWork(IDbFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        private DataBaseConnection DbContext => _dbContext ?? (_dbContext = _dbFactory.Init());

        public async Task Commit()
        {
            await DbContext.SaveChangesAsync();
        }
    }
}

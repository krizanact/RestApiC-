using Project.Model.Core;
using Microsoft.EntityFrameworkCore;
using Project.Model.DatabaseConnector;

namespace Project.Model.DatabaseConnector
{
    // Abstracting database entity so using can be made only dbContext object
    public class DbFactory : Disposable, IDbFactory
    {
        DataBaseConnection _dbContext;
        private readonly DbContextOptions<DataBaseConnection> _dbContextOptions;

        public DbFactory(DataBaseConnection context, DbContextOptions<DataBaseConnection> dbContextOptions)
        {
            _dbContext = context;
            _dbContextOptions = dbContextOptions;
        }

        public DataBaseConnection Init()
        {
            return _dbContext ?? (_dbContext = new DataBaseConnection());
        }

        public DataBaseConnection InitThreadSafe()
        {
            return new DataBaseConnection(_dbContextOptions);
        }

        protected override void DisposeCore()
        {
            if (_dbContext != null)
                _dbContext.Dispose();
        }
    }
}
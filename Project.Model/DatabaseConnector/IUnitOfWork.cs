using System.Threading.Tasks;

namespace Project.Model.DatabaseConnector
{
    public interface IUnitOfWork
    {
        Task Commit();
    }
}

using System;
using Project.Model.Core;

namespace Project.Model.DatabaseConnector
{
   public interface IDbFactory : IDisposable
    {
        DataBaseConnection Init();
        DataBaseConnection InitThreadSafe();
    }
}

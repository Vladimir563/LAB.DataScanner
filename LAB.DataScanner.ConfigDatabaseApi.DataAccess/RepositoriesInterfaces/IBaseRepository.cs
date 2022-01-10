using Microsoft.AspNet.OData;
using System.Collections.Generic;

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.RepositoriesInterfaces
{
    public interface IBaseRepository <T> where T : class
    {
        SingleResult<T> Get(string key);

        IEnumerable<T> GetAll();

        void Create(T entity);

        T Update(int id, T newEntity);

        bool Delete(int id);
    }
}

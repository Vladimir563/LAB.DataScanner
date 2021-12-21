using Microsoft.AspNet.OData;
using System.Collections.Generic;

namespace LAB.DataScanner.ConfigDatabaseApi.Contracts.Services
{
    public interface IBaseService <T> where T : class
    {
        SingleResult<T> GetEntity(string key);

        IEnumerable<T> GetAllEntities();

        void CreateEntity(T entity);

        T UpdateEntity(int id, T newEntity);

        bool DeleteEntity(int id);
    }
}

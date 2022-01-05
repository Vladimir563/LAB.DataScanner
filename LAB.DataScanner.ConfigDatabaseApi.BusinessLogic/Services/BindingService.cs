using LAB.DataScanner.ConfigDatabaseApi.Contracts.Repositories;
using LAB.DataScanner.ConfigDatabaseApi.Contracts.Services;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.AspNet.OData;
using System.Collections.Generic;

namespace LAB.DataScanner.ConfigDatabaseApi.BusinessLogic.Services
{
    internal class BindingService : IBaseService<Binding>
    {
        private readonly IBaseRepository<Binding> _appBindingResitory;

        public BindingService(IBaseRepository<Binding> appBindingResitory)
        {
            _appBindingResitory = appBindingResitory;
        }

        public void CreateEntity(Binding entity) =>
            _appBindingResitory.Create(entity);

        public bool DeleteEntity(int id)
        {
            var appType = _appBindingResitory.Get(id.ToString());

            if (appType is null)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<Binding> GetAllEntities() =>
            _appBindingResitory.GetAll();

        public SingleResult<Binding> GetEntity(string key) =>
            _appBindingResitory.Get(key);

        public Binding UpdateEntity(int id, Binding newEntity) =>
            _appBindingResitory.Update(id, newEntity);
    }
}

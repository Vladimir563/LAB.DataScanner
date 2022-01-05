using LAB.DataScanner.ConfigDatabaseApi.Contracts.Repositories;
using LAB.DataScanner.ConfigDatabaseApi.Contracts.Services;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.AspNet.OData;
using System.Collections.Generic;

namespace LAB.DataScanner.ConfigDatabaseApi.BusinessLogic.Services
{
    internal class ApplicationInstanceService : IBaseService<ApplicationInstance>
    {
        private readonly IBaseRepository<ApplicationInstance> _appInstanceResitory;

        public ApplicationInstanceService(IBaseRepository<ApplicationInstance> appInstanceResitory)
        {
            _appInstanceResitory = appInstanceResitory;
        }

        public void CreateEntity(ApplicationInstance entity) =>
            _appInstanceResitory.Create(entity);

        public bool DeleteEntity(int id)
        {
            var appType = _appInstanceResitory.Get(id.ToString());

            if (appType is null)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<ApplicationInstance> GetAllEntities() =>
            _appInstanceResitory.GetAll();

        public SingleResult<ApplicationInstance> GetEntity(string key) =>
            _appInstanceResitory.Get(key);

        public ApplicationInstance UpdateEntity(int id, ApplicationInstance newEntity) =>
            _appInstanceResitory.Update(id, newEntity);
    }
}

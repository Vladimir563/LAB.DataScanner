using LAB.DataScanner.ConfigDatabaseApi.Contracts.Repositories;
using LAB.DataScanner.ConfigDatabaseApi.Contracts.Services;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.AspNet.OData;
using System.Collections.Generic;

namespace LAB.DataScanner.ConfigDatabaseApi.BusinessLogic.Services
{
    internal class ApplicationTypeService : IBaseService<ApplicationType>
    {
        private readonly IBaseRepository<ApplicationType> _appTypeResitory;

        public ApplicationTypeService(IBaseRepository<ApplicationType> appTypeResitory)
        {
            _appTypeResitory = appTypeResitory;
        }

        public void CreateEntity(ApplicationType entity) => 
            _appTypeResitory.Create(entity);

        public bool DeleteEntity(int id) 
        {
            var appType = _appTypeResitory.Get(id.ToString());

            if (appType is null) 
            {
                return false;
            }

            return true;
        }

        public IEnumerable<ApplicationType> GetAllEntities() => 
            _appTypeResitory.GetAll();

        public SingleResult<ApplicationType> GetEntity(string key) =>
            _appTypeResitory.Get(key);

        public ApplicationType UpdateEntity(int id, ApplicationType newEntity) => 
            _appTypeResitory.Update(id, newEntity);
    }
}

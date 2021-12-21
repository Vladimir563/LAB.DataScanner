using LAB.DataScanner.ConfigDatabaseApi.Contracts.Repositories;
using LAB.DataScanner.ConfigDatabaseApi.Contracts.Services;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.AspNet.OData;
using System;
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

        public void CreateEntity(ApplicationType entity)
        {
            throw new NotImplementedException();
        }

        public bool DeleteEntity(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ApplicationType> GetAllEntities()
        {
            throw new NotImplementedException();
        }

        public SingleResult<ApplicationType> GetEntity(string key) =>
            _appTypeResitory.Get(key);

        public ApplicationType UpdateEntity(int id, ApplicationType newEntity)
        {
            throw new NotImplementedException();
        }
    }
}

using LAB.DataScanner.ConfigDatabaseApi.Contracts.Repositories;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.Repositories
{
    internal class ApplicationInstancesRepository : IBaseRepository<ApplicationInstance>
    {
        public void Create(ApplicationInstance entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public SingleResult<ApplicationInstance> Get(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ApplicationInstance> GetAll()
        {
            throw new NotImplementedException();
        }

        public ApplicationInstance Update(int id, ApplicationInstance newEntity)
        {
            throw new NotImplementedException();
        }
    }
}

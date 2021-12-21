using LAB.DataScanner.ConfigDatabaseApi.Contracts.Repositories;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.EF;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.Repositories
{
    internal class ApplicationTypesRepository : IBaseRepository<ApplicationType>
    {
        private readonly IConfigDatabaseContext _db;
        public ApplicationTypesRepository(IConfigDatabaseContext dbContext)
        {
            _db = dbContext;
        }

        public void Create(ApplicationType entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public SingleResult<ApplicationType> Get(string key) =>
            SingleResult.Create(_db.ApplicationTypes.Where(s => s.TypeId.Equals(int.Parse(key))));

        public IEnumerable<ApplicationType> GetAll()
        {
            throw new NotImplementedException();
        }

        public ApplicationType Update(int id, ApplicationType newEntity)
        {
            throw new NotImplementedException();
        }
    }
}

using LAB.DataScanner.ConfigDatabaseApi.Contracts.Repositories;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.Repositories
{
    internal class BindingsRepository : IBaseRepository<Binding>
    {
        public void Create(Binding entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public SingleResult<Binding> Get(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Binding> GetAll()
        {
            throw new NotImplementedException();
        }

        public Binding Update(int id, Binding newEntity)
        {
            throw new NotImplementedException();
        }
    }
}

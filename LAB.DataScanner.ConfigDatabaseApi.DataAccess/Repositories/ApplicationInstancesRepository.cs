using LAB.DataScanner.ConfigDatabaseApi.Contracts.Repositories;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.EF;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.AspNet.OData;
using System.Collections.Generic;
using System.Linq;

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.Repositories
{
    internal class ApplicationInstancesRepository : IBaseRepository<ApplicationInstance>
    {
        private readonly IConfigDatabaseContext _db;
        public ApplicationInstancesRepository(IConfigDatabaseContext dbContext)
        {
            _db = dbContext;
        }

        public void Create(ApplicationInstance entity)
        {
            _db.ApplicationInstances.Add(entity);

            _db.SaveChanges();
        }


        public bool Delete(int id)
        {
            var entity = _db.ApplicationInstances.Find(id);

            _db.ApplicationInstances.Remove(entity);

            _db.SaveChanges();

            return true;
        }

        public SingleResult<ApplicationInstance> Get(string key) =>
            SingleResult.Create(_db.ApplicationInstances.Where(s => s.InstanceId.Equals(int.Parse(key))));

        public IEnumerable<ApplicationInstance> GetAll() =>
            _db.ApplicationInstances;

        public ApplicationInstance Update(int id, ApplicationInstance newEntity)
        {
            var entity = _db.ApplicationInstances.Find(id);

            entity = newEntity;

            _db.SaveChanges();

            return entity;
        }
    }
}

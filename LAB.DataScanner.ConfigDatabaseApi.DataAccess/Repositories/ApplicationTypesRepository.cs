using LAB.DataScanner.ConfigDatabaseApi.DataAccess.EF;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.RepositoriesInterfaces;
using Microsoft.AspNet.OData;
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
            _db.ApplicationTypes.Add(entity);

            _db.SaveChanges();
        }
            

        public bool Delete(int id) 
        {
            var entity = _db.ApplicationTypes.Find(id);

            _db.ApplicationTypes.Remove(entity);

            _db.SaveChanges();

            return true;
        }
            

        public SingleResult<ApplicationType> Get(string key) =>
            SingleResult.Create(_db.ApplicationTypes.Where(s => s.TypeId.Equals(int.Parse(key))));

        public IEnumerable<ApplicationType> GetAll() =>
            _db.ApplicationTypes;

        public ApplicationType Update(int id, ApplicationType newEntity)
        {
            var entity = _db.ApplicationTypes.Find(id);

            entity = newEntity;

            _db.SaveChanges();

            return entity;
        }
    }
}

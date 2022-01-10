using LAB.DataScanner.ConfigDatabaseApi.DataAccess.EF;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.RepositoriesInterfaces;
using Microsoft.AspNet.OData;
using System.Collections.Generic;
using System.Linq;

namespace LAB.DataScanner.ConfigDatabaseApi.DataAccess.Repositories
{
    internal class BindingsRepository : IBaseRepository<Binding>
    {
        private readonly IConfigDatabaseContext _db;
        public BindingsRepository(IConfigDatabaseContext dbContext)
        {
            _db = dbContext;
        }

        public void Create(Binding entity)
        {
            _db.Bindings.Add(entity);

            _db.SaveChanges();
        }


        public bool Delete(int id)
        {
            var entity = _db.Bindings.Find(id);

            _db.Bindings.Remove(entity);

            _db.SaveChanges();

            return true;
        }


        public SingleResult<Binding> Get(string key) =>
            SingleResult.Create(_db.Bindings.Where(s => s.BindingId.Equals(int.Parse(key))));

        public IEnumerable<Binding> GetAll() =>
            _db.Bindings;

        public Binding Update(int id, Binding newEntity)
        {
            var entity = _db.Bindings.Find(id);

            entity = newEntity;

            _db.SaveChanges();

            return entity;
        }
    }
}

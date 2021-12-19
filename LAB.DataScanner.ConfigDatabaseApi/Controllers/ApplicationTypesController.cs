using LAB.DataScanner.ConfigDatabaseApi.DataAccess.EF;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LAB.DataScanner.ConfigDatabaseApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class ApplicationTypesController : BaseController<ApplicationType>
    {
        private readonly DataScannerDbContext _context;

        public ApplicationTypesController(DataScannerDbContext context)
        {
            _context = context;
        }

        public override SingleResult<ApplicationType> Get([FromODataUri] string key) 
        {
            return SingleResult.Create(_context.ApplicationTypes.Where(s => s.TypeId.Equals(key)));
        }

    }
}


using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LAB.DataScanner.ConfigDatabaseApi.Controllers
{
    [ODataFormatting]
    [ODataRouting]
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class BaseController<T> : ODataController where T : class
    {
        [HttpGet]
        [EnableQuery]
        public virtual SingleResult<T> Get([FromODataUri] string key)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [EnableQuery]
        public virtual IActionResult Post(T entity)
        {
            throw new NotImplementedException();
        }

        [HttpPatch]
        [EnableQuery]
        public virtual IActionResult Patch([FromODataUri] string key, Delta<T> entityDelta)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [EnableQuery]
        public virtual IActionResult Delete([FromODataUri] string key)
        {
            throw new NotImplementedException();
        }
    }
}

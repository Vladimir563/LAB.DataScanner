using FluentValidation;
using LAB.DataScanner.ConfigDatabaseApi.Contracts.Services;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.EF;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LAB.DataScanner.ConfigDatabaseApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class ApplicationTypesController : BaseController<ApplicationType>
    {
        private readonly IBaseService<ApplicationType> _appTypeService;

        private readonly IValidator<ApplicationType> _validator;

        public ApplicationTypesController(IBaseService<ApplicationType> appTypeService,
            IValidator<ApplicationType> validator)
        {
            _appTypeService = appTypeService;
            _validator = validator;
        }

        public override SingleResult<ApplicationType> Get([FromODataUri] string key) =>
            _appTypeService.GetEntity(key);
    }
}


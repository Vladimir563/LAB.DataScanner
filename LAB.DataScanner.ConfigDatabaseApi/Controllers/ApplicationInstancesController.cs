using FluentValidation;
using LAB.DataScanner.ConfigDatabaseApi.Contracts.Services;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LAB.DataScanner.ConfigDatabaseApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class ApplicationInstancesController : BaseController<ApplicationInstance>
    {
        private readonly IBaseService<ApplicationInstance> _appInstanceService;

        private readonly IValidator<ApplicationInstance> _validator;

        public ApplicationInstancesController(IBaseService<ApplicationInstance> appInstanceService,
            IValidator<ApplicationInstance> validator)
        {
            _appInstanceService = appInstanceService;
            _validator = validator;
        }
    }
}

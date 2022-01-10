using FluentValidation;
using LAB.DataScanner.ConfigDatabaseApi.BusinessLogic.Services;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LAB.DataScanner.ConfigDatabaseApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class BindingsController : BaseController<Binding>
    {
        private readonly IBaseService<Binding> _appBindingService;

        private readonly IValidator<Binding> _validator;

        public BindingsController(IBaseService<Binding> appBindingService,
            IValidator<Binding> validator)
        {
            _appBindingService = appBindingService;
            _validator = validator;
        }
    }
}

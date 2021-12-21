using FluentValidation;
using LAB.DataScanner.ConfigDatabaseApi.DataAccess.Entities;

namespace LAB.DataScanner.ConfigDatabaseApi.Validation
{
    public class BindingValidator : AbstractValidator<Binding>
    {
        public BindingValidator()
        {
            RuleFor(x => x.BindingId).NotEmpty().NotNull().WithMessage("BindingId cannot be null or empty");
        }
    }
}

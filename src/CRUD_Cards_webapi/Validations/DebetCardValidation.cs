using CRUD_Cards_webapi.Models;
using FluentValidation;

namespace CRUD_Cards_webapi.Validations;

internal sealed class DebetCardValidation : AbstractValidator<DebetCardBase>
{
    public DebetCardValidation()
    {
        var date = DateTime.UtcNow;
        RuleFor(x => x.ExpireMonth)
            .GreaterThanOrEqualTo(date.Month).WithMessage("Month cannot be expired")
            .LessThanOrEqualTo(12).WithMessage("In year for now only 12 month");

        RuleFor(x => x.ExpireYear).GreaterThanOrEqualTo(date.Year).WithMessage("You cannot add expired card");

        RuleFor(x => x.Number).CreditCard().WithMessage("Not valid card number");

        // Not checked
        RuleFor(x => x.Holder).Matches("[a-zA-Z ]+").WithMessage("Not a valid character in holder name");
    }
}
using CRUD_Cards_webapi.Models;
using FluentValidation;

namespace CRUD_Cards_webapi.Validations;

internal sealed class DebetCardValidation : AbstractValidator<DebetCardBase>
{
    public DebetCardValidation()
    {
        var date = DateTime.UtcNow;
        var month = date.Month;
        var year = date.Year / 100;


        RuleFor(x => x.ExpireMonth).InclusiveBetween(month, 12).WithMessage("Month cannot be expired");

        RuleFor(x => x.ExpireYear).GreaterThanOrEqualTo(year).WithMessage("You cannot add expired card");

        RuleFor(x => x.Number)
            //.NotNull().WithMessage("It hard to generate card number so it just must exist")
            .CreditCard().WithMessage("Not valid card number")
            ;

        // Not checked
        RuleFor(x => x.Holder).Matches("[a-zA-Z ]+").WithMessage("Not a valid character in holder name");
    }
}
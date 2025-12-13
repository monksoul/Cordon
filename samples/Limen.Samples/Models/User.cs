namespace Limen.Samples.Models;

public class User : IValidatableObject
{
    [Min(1)] public int Id { get; set; }

    [Required] public string? Name { get; set; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // return validationContext.ValidateWith(new UserValidator());

        return validationContext.ValidateUsing<User>(validator =>
        {
            validator.RuleFor(u => u.Name).NotBlank().MinLength(3).UserName().WithMessage("不是有效的互联网用户名")
                .RuleFor(u => u.Id).Max(int.MaxValue);
        });
    }
}

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Name).NotBlank().MinLength(3).UserName().WithMessage("不是有效的互联网用户名");

        RuleFor(u => u.Id).Max(int.MaxValue);
    }
}
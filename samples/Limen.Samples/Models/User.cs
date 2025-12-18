namespace Limen.Samples.Models;

public class User : IValidatableObject
{
    [Min(1)] public int Id { get; set; }

    [Required] [MinLength(2)] public string? Name { get; set; }

    [GreaterThanOrEqualTo(18)] public int Age { get; set; }

    public List<Address>? Addresses { get; set; }

    [Compare(nameof(ConfirmPassword))] public string? Password { get; set; }

    public string? ConfirmPassword { get; set; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return validationContext.ContinueWith<User>()
            .RuleFor(u => u.Name).NotBlank().MinLength(3).UserName().WithMessage("不是有效的互联网用户名")
            .RuleFor(u => u.Id).Max(int.MaxValue)
            .RuleForEach(u => u.Addresses).ChildRules(u =>
            {
                u.RuleFor(c => c.Country).Required()
                    .RuleFor(c => c.Province).Required()
                    .RuleFor(c => c.City).Required();
            }).ToResults();

        // return validationContext.ValidateWith(new UserValidator());

        // return validationContext.ValidateUsing<User>(validator =>
        // {
        //     validator.RuleFor(u => u.Name).NotBlank().MinLength(3).UserName().WithMessage("不是有效的互联网用户名")
        //         .RuleFor(u => u.Id).Max(int.MaxValue)
        //         .RuleForEach(u => u.Addresses).ChildRules(u =>
        //         {
        //             u.RuleFor(c => c.Country).Required()
        //                 .RuleFor(c => c.Province).Required()
        //                 .RuleFor(c => c.City).Required();
        //         });
        // });
    }
}

public class Address
{
    public string? Country { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
}

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Name).NotBlank().MinLength(3).UserName().WithMessage("不是有效的互联网用户名");

        RuleFor(u => u.Id).Max(int.MaxValue);

        // 可使用 RuleForEach(u => u.Addresses).ChildRules() 替换
        RuleFor(u => u.Addresses).Each<Address>(u =>
        {
            u.RuleFor(c => c.Country).Required()
                .RuleFor(c => c.Province).Required()
                .RuleFor(c => c.City).Required();
        });
    }
}
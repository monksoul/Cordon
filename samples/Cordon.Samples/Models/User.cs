namespace Cordon.Samples.Models;

public class User : IValidatableObject
{
    [Required(ErrorMessage = "名字不能为空")]
    [MinLength(2, ErrorMessage = "名字不能少于 2 个字符")]
    public string? Name { get; set; }

    [Min(18, ErrorMessage = "年龄不能小于 18 岁")]
    public int Age { get; set; }

    [Required(ErrorMessage = "密码不能为空")]
    [Compare(nameof(ConfirmPassword), ErrorMessage = "两次输入的密码不一致")]
    public string? Password { get; set; }

    public string? ConfirmPassword { get; set; }

    [Required]
    [EmailAddress]
    // [AllowedEmailDomains]
    public string? EmailAddress { get; set; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // if (EmailAddress is not null &&
        //     !_allowedDomains.Any(domain => EmailAddress.EndsWith(domain, StringComparison.OrdinalIgnoreCase)))
        //     yield return new ValidationResult("仅支持 outlook、qq 和 163 邮箱格式。", [nameof(EmailAddress)]);

        return validationContext.ContinueWith<User>()
            .RuleFor(u => u.EmailAddress)
            .MustAny(["@outlook.com", "@qq.com", "@163.com"],
                (email, domain) => email.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
            .WithMessage("仅支持 outlook、qq 和 163 邮箱格式。")
            .ToResults();
    }
}

public class AllowedEmailDomainsAttribute : ValidationAttribute
{
    private readonly string[] _allowedDomains = ["@outlook.com", "@qq.com", "@163.com"];

    public string GetErrorMessage()
    {
        return "仅支持 outlook、qq 和 163 邮箱格式。";
    }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string email) return ValidationResult.Success;

        if (!_allowedDomains.Any(domain => email.EndsWith(domain, StringComparison.OrdinalIgnoreCase)))
            return new ValidationResult(GetErrorMessage());

        return base.IsValid(value, validationContext);
    }
}
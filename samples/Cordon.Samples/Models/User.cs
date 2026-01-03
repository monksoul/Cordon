namespace Cordon.Samples.Models;

public class Address : IValidatableObject
{
    public string? City { get; set; }
    public string? Street { get; set; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return validationContext.With<Address>()
            .RuleFor(u => u.City).Required()
            .RuleFor(u => u.Street).Required()
            .ToResults();
    }
}

public class WorkExperience : IValidatableObject
{
    public string? Company { get; set; }
    public string? Position { get; set; }
    public int Years { get; set; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return validationContext.With<WorkExperience>()
            .RuleFor(u => u.Company).Required()
            .RuleFor(u => u.Position).Required()
            .RuleFor(u => u.Years).Range(1900, 2026)
            .ToResults();
    }
}

public class User : IValidatableObject
{
    public string? Name { get; set; }
    public int Age { get; set; }

    public Address? Address { get; set; }

    public List<WorkExperience>? WorkExperiences { get; set; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return validationContext.With<User>()
            .RuleFor(u => u.Name).Required().MinLength(3)
            .RuleFor(u => u.Age).Age(true)
            .RuleFor(u => u.Address).Required()
            .RuleForCollection(u => u.WorkExperiences).MaxLength(5)
            .ToResults();
    }
}
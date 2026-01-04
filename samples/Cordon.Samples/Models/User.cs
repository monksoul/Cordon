namespace Cordon.Samples.Models;

public class Address
{
    public string? City { get; set; }
    public string? Street { get; set; }
}

public class WorkExperience
{
    public string? Company { get; set; }
    public string? Position { get; set; }
    public int Years { get; set; }
}

public class User : IValidatableObject
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public Address? Address { get; set; }
    public List<WorkExperience>? WorkExperiences { get; set; }
    public List<string>? Hobbies { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return validationContext.With<User>()
            .RuleFor(u => u.Name).Required().MinLength(3)
            .RuleFor(u => u.Age).Age(true)
            .RuleFor(u => u.Address).Required().ChildRules(addr =>
            {
                addr.RuleFor(a => a.City).Required()
                    .RuleFor(a => a.Street).Required();
            })
            .RuleForCollection(u => u.WorkExperiences).MaxLength(5).ChildRules(exp =>
            {
                exp.RuleFor(e => e.Company).Required()
                    .RuleFor(e => e.Position).Required()
                    .RuleFor(e => e.Years).Range(1900, 2026);
            })
            .RuleForCollection(u => u.Hobbies).EachRules(h => h.Required().MinLength(2))
            .ToResults();
    }
}
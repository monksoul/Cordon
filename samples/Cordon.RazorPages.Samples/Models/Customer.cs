namespace Cordon.RazorPages.Samples.Models;

public class Customer : IValidatableObject
{
    public int Id { get; set; }

    [Required] [StringLength(10)] public string? Name { get; set; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return validationContext.ContinueWith<Customer>()
            .RuleFor(u => u.Name).MinLength(3)
            .ToResults();
    }
}
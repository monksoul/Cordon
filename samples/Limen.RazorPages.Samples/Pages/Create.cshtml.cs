namespace Limen.RazorPages.Samples.Pages;

public class CreateModel : PageModel
{
    [BindProperty] public Customer? Customer { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    // [ValidationOptions(["create"])]
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        ;

        await Task.Delay(100);

        return RedirectToPage("./Index");
    }
}
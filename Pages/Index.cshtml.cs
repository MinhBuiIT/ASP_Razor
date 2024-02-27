using ASP_RazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_RazorWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly BlogContext _blogContext;
    public IndexModel(ILogger<IndexModel> logger,BlogContext blogContext)
    {
        _logger = logger;
        _blogContext = blogContext;
    }

    public void OnGet()
    {
        var blogs = (from a in _blogContext.articles
                    orderby a.Title descending
                    select a).ToList();
        ViewData["blogs"] = blogs;

    }
}

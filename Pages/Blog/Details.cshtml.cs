using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ASP_RazorWeb.Models;

namespace ASP_RazorWeb.Pages_Blog
{
    public class DetailsModel : PageModel
    {
        private readonly ASP_RazorWeb.Models.BlogContext _context;

        public DetailsModel(ASP_RazorWeb.Models.BlogContext context)
        {
            _context = context;
        }

        public Article Article { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync([FromRoute(Name = "idblog")]int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.articles.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            else
            {
                Article = article;
            }
            return Page();
        }
    }
}

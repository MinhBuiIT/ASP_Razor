using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ASP_RazorWeb.Models;

namespace ASP_RazorWeb.Pages_Blog
{
    public class CreateModel : PageModel
    {
        private readonly ASP_RazorWeb.Models.BlogContext _context;

        public CreateModel(ASP_RazorWeb.Models.BlogContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Article Article { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.articles.Add(Article);
            await _context.SaveChangesAsync();//lưu dữ liệu mới vào database

            return RedirectToPage("./Index");
        }
    }
}

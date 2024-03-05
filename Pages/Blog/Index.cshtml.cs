using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ASP_RazorWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace ASP_RazorWeb.Pages_Blog
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ASP_RazorWeb.Models.BlogContext _context;

        public IndexModel(ASP_RazorWeb.Models.BlogContext context)
        {
            _context = context;
        }
        public readonly int ELE_PER_PAGE = 10;
        public IList<Article> Article { get;set; } = default!;
        public int currentPage{get;set;}
        public int countPages{get;set;}

        public async Task OnGetAsync(string? searchString,int? pages)
        {
            if(pages == null) pages = 1;
            currentPage = (int)pages;
            countPages = (int)Math.Ceiling((double)_context.articles.Count()/ELE_PER_PAGE);
            // Article = await _context.articles.ToListAsync();
            var articles = (from a in _context.articles
                            orderby a.Created descending
                            select a).Skip((int)(pages -1)*ELE_PER_PAGE).Take(ELE_PER_PAGE);
            if(!string.IsNullOrEmpty(searchString)) {
                var articleSearch = articles.Where(a => a.Title.Contains(searchString));
                if(articleSearch != null) {
                    Article = await articleSearch.ToListAsync();
                }
                ViewData["searchString"] = searchString;
            }else {
                Article = await articles.ToListAsync();
            }
            
        }
    }
}

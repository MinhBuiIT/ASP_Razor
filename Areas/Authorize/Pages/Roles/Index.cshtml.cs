using ASP_RazorWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_RazorWeb.Pages.Roles
{
    [Authorize(Roles="Administrator")]
    public class IndexModel : RoleModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, BlogContext blogContext) : base(roleManager, blogContext)
        {
        }
        public List<IdentityRole> roleList{get;set;}
        public async Task<IActionResult> OnGet()
        {
            roleList = _roleManager.Roles.OrderBy(r => r.Name).ToList();
            return Page();
        }
    }
}

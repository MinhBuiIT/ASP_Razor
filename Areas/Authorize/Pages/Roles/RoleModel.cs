using ASP_RazorWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_RazorWeb.Pages.Roles {
    public class RoleModel: PageModel {
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly BlogContext _blogContext;

        [TempData]
        public string StatusMessage { get;set;}
        public RoleModel(RoleManager<IdentityRole> roleManager,BlogContext blogContext) {
            _roleManager = roleManager;
            _blogContext = blogContext;
        }
    }
}
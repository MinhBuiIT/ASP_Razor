using ASP_RazorWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ASP_RazorWeb.Pages.Roles.Users
{
    public class IndexModel : PageModel
    {
        public UserManager<AppUser> _userManager{get;set;}
        public RoleManager<IdentityRole> _roleManager{get;set;}

        [TempData]
        public string StatusMessage{get;set;}
        public class UserAndRole:AppUser {
            public string? RoleName{get;set;}
        }
        public IndexModel(UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager) {
            _userManager = userManager;
            _roleManager = roleManager;
         }
         public List<UserAndRole> Users{get;set;} = new List<UserAndRole>();
         public int totalUser{get;set;}
         public int currentPage{get;set;}
         public int countPage{get;set;}
         private readonly int ELE_PER_PAGE = 10;
        public async Task<IActionResult> OnGetAsync(int? pages)
        {
            if(pages == null) pages = 1;
            currentPage = (int)pages;
            List<AppUser> UsersAll = await _userManager.Users.OrderBy(user => user.UserName).ToListAsync();
            totalUser = UsersAll.Count();
            countPage = ((int)Math.Ceiling((double)totalUser / ELE_PER_PAGE));

            var userOnly = UsersAll.Skip((currentPage - 1) * ELE_PER_PAGE).Take(ELE_PER_PAGE).ToList();
            foreach(var user in userOnly) {
                var roleUsers = (await _userManager.GetRolesAsync(user)).ToList();
                string roleNames = String.Join(",",roleUsers);
                Users.Add(new UserAndRole(){Id = user.Id,UserName = user.UserName,RoleName = roleNames});
            }
            return Page();
        }
    }
}

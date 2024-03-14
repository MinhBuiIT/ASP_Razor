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

        public class RoleClaimModel: IdentityRole {
            public string[] Claims { get; set; }
        }
        public List<RoleClaimModel> roleList{get;set;} = new List<RoleClaimModel>();
        public async Task<IActionResult> OnGet()
        {
            var roles = _roleManager.Roles.OrderBy(r => r.Name).ToList();
            foreach (var r in roles)
            {
                var claims = (await _roleManager.GetClaimsAsync(r)).Select(c => c.Type + " - " + c.Value).ToArray();
                RoleClaimModel roleClaimModel = new RoleClaimModel(){
                    Id = r.Id,
                    Name = r.Name,
                    Claims = claims
                };
                roleList.Add(roleClaimModel);
            }
            return Page();
        }
    }
}

using ASP_RazorWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_RazorWeb.Pages.Roles
{
    [Authorize(Roles="Administrator")]
    public class DeleteModel : RoleModel
    {
        public DeleteModel(RoleManager<IdentityRole> roleManager, BlogContext blogContext) : base(roleManager, blogContext)
        {
        }

        public IdentityRole? role{get; set; }
        public async Task<ActionResult> OnGet(string roleId)
        {
            if(roleId == null) return NotFound("Không tìm thấy Role");
            role = await _roleManager.FindByIdAsync(roleId);
            if(role == null) return NotFound("Không tìm thấy Role");
            return Page();
        }
        public async Task<ActionResult> OnPostAsync(string roleId) {
            if(roleId == null) return NotFound("Không tìm thấy Role");
            role = await _roleManager.FindByIdAsync(roleId);
            if(role == null) return NotFound("Không tìm thấy Role");
            var result = await _roleManager.DeleteAsync(role);
            if(result.Succeeded) {
                StatusMessage = $"Xóa thành công role: {role.Name}";
                return RedirectToPage("./Index");
            }else {
                result.Errors.ToList().ForEach(err => {
                    ModelState.AddModelError(String.Empty,err.Description);
                });
                return Page();
            }
        }
    }
}

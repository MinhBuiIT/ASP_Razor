using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ASP_RazorWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_RazorWeb.Pages.Roles
{
    [Authorize(Roles="Administrator")]
    public class EditModel : RoleModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, BlogContext blogContext) : base(roleManager, blogContext)
        {
        }
         public class InputModel {
            [Required(ErrorMessage = "Vui lòng nhập {0}")]
            [StringLength(256,MinimumLength = 3,ErrorMessage = "{0} có độ dài từ {2} đến {1} ký tự")]
            [DisplayName("Role")]
            public string Name { get; set; }
        }

        [BindProperty]
        public InputModel Input{get; set; }
        public async Task<IActionResult>  OnGet(string roleid)
        {
            if(roleid == null) {
                return NotFound("Không tìm thấy Role");
            }
           IdentityRole role = await _roleManager.FindByIdAsync(roleid);
           if(role == null) {
                return NotFound("Không tìm thấy Role");
           }
           Input = new InputModel() {Name = role.Name};
           return Page();
        }
        public async Task<IActionResult> OnPostAsync(string roleid) {
            if(roleid == null) {
                return NotFound("Không tìm thấy Role");
            }
            IdentityRole role = await _roleManager.FindByIdAsync(roleid);
           if(role == null) {
                return NotFound("Không tìm thấy Role");
           }
           role.Name = Input.Name;
           var result = await _roleManager.UpdateAsync(role);
            if(result.Succeeded) {
                StatusMessage = "Cập nhật thành công";
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

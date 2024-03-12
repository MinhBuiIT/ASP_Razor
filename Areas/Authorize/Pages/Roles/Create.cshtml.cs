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
    public class CreateModel : RoleModel
    {
        public CreateModel(RoleManager<IdentityRole> roleManager, BlogContext blogContext) : base(roleManager, blogContext)
        {
        }

        public void OnGet()
        {
        }
        public class InputModel {
            [Required(ErrorMessage = "Vui lòng nhập {0}")]
            [StringLength(256,MinimumLength = 3,ErrorMessage = "{0} có độ dài từ {2} đến {1} ký tự")]
            [DisplayName("Role mới")]
            public string Name { get; set; }
        }

        [BindProperty]
        public InputModel Input{get; set; }
        public async Task<IActionResult> OnPostAsync() {
            if(!ModelState.IsValid) {
                return Page();
            }
            var identityRole = new IdentityRole(Input.Name);
            var result = await _roleManager.CreateAsync(identityRole);
            if(result.Succeeded) {
                StatusMessage = $"Thêm thành công role mới: {Input.Name}";
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

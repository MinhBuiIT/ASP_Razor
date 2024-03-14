using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ASP_RazorWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_RazorWeb.Pages.Roles
{
    [Authorize(Roles="Administrator")]
    public class AddClaimModel : RoleModel
    {
        public AddClaimModel(RoleManager<IdentityRole> roleManager, BlogContext blogContext) : base(roleManager, blogContext)
        {
        }
        public class InputModel {
            [Required(ErrorMessage = "Vui lòng nhập {0}")]
            [StringLength(256,MinimumLength = 3,ErrorMessage = "{0} có độ dài từ {2} đến {1} ký tự")]
            [DisplayName("Kiểu (tên) Claim")]
            public string ClaimType { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập {0}")]
            [StringLength(256,MinimumLength = 3,ErrorMessage = "{0} có độ dài từ {2} đến {1} ký tự")]
            [DisplayName("Giá trị")]
            public string ClaimValue { get; set; }
        }

        [BindProperty]
        public InputModel Input{get; set; }
        public IdentityRole? role{get; set; }

        public async Task<IActionResult> OnGetAsync(string roleid) {
            if(roleid == null) {
                return NotFound("Không tìm thấy role");
            }
            role = await _roleManager.FindByIdAsync(roleid);
            if(role == null) {
                return NotFound("Không tìm thấy role");
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string roleid) {
            if(roleid == null) {
                return NotFound("Không tìm thấy role");
            }
            role = await _roleManager.FindByIdAsync(roleid);
            if(role == null) {
                return NotFound("Không tìm thấy role");
            }
            bool checkSame = (await _roleManager.GetClaimsAsync(role)).Any(c => c.Value == Input.ClaimValue && c.Type == Input.ClaimType);
            if(checkSame) {
                ModelState.AddModelError("","Claim đã tồn tại trong role");
                return Page();
            }
            if(ModelState.IsValid) {
                Claim claim = new Claim(Input.ClaimType,Input.ClaimValue);
                var result = await _roleManager.AddClaimAsync(role,claim);
                if(!result.Succeeded) {
                    result.Errors.ToList().ForEach(e => {
                        ModelState.AddModelError("",e.Description);
                    });
                    return Page();
                }
                return RedirectToPage("./Edit",new {roleid = role.Id});
            }
            return Page();
        }
    }
}

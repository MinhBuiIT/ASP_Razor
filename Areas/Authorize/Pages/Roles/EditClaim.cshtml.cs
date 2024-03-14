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
    public class EditClaimModel : RoleModel
    {
        public EditClaimModel(RoleManager<IdentityRole> roleManager, BlogContext blogContext) : base(roleManager, blogContext)
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
        public IdentityRoleClaim<string> roleClaim{get; set; }

        public async Task<IActionResult> OnGetAsync(int? claimid) {
            if(claimid == null) {
                return NotFound("Không tìm thấy role");
            }
            roleClaim = _blogContext.RoleClaims.Where(rc => rc.Id == claimid).FirstOrDefault();
            if(roleClaim == null) {
                return NotFound("Không tìm thấy role");
            }
            role = _blogContext.Roles.Where(r => r.Id == roleClaim.RoleId).FirstOrDefault();
            Input = new InputModel() {ClaimType = roleClaim.ClaimType,ClaimValue = roleClaim.ClaimValue};
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? claimid) {
            if(claimid == null) {
                return NotFound("Không tìm thấy role");
            }
            roleClaim = _blogContext.RoleClaims.Where(rc => rc.Id == claimid).FirstOrDefault();
            if(roleClaim == null) {
                return NotFound("Không tìm thấy role");
            }
            role = _blogContext.Roles.Where(r => r.Id == roleClaim.RoleId).FirstOrDefault();

            bool checkSame = _blogContext.RoleClaims.Any(rc => rc.RoleId == role.Id && rc.Id != claimid && rc.ClaimValue == Input.ClaimValue && rc.ClaimType == Input.ClaimType);
            if(checkSame) {
                ModelState.AddModelError("","Claim đã tồn tại trong role");
                return Page();
            }
            if(ModelState.IsValid) {
                roleClaim.ClaimValue = Input.ClaimValue;
                roleClaim.ClaimType = Input.ClaimType;
                await _blogContext.SaveChangesAsync();
                StatusMessage = "Đã cập nhật Claim";
                return RedirectToPage("./Edit",new {roleid = role.Id});
            }
            return Page();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int? claimid) {
            if(claimid == null) {
                return NotFound("Không tìm thấy role");
            }
            roleClaim = _blogContext.RoleClaims.Where(rc => rc.Id == claimid).FirstOrDefault();
            if(roleClaim == null) {
                return NotFound("Không tìm thấy role");
            }
            role = _blogContext.Roles.Where(r => r.Id == roleClaim.RoleId).FirstOrDefault();

            var result = await _roleManager.RemoveClaimAsync(role,new Claim(roleClaim.ClaimType,roleClaim.ClaimValue));
            if(!result.Succeeded) {
                ModelState.AddModelError("","Lỗi xóa claim. Thử lại");
                return Page();
            }
            StatusMessage = "Đã xóa thành công claim";
            return RedirectToPage("./Edit",new {roleid = role.Id});
        }
    }
}

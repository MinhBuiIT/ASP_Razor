using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ASP_RazorWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ASP_RazorWeb.Pages.Roles.Users
{
    public class SetPasswordModel : PageModel
    {
        UserManager<AppUser> _userManager;

        [TempData]
        public string StatusMessage{get;set;}
        public SetPasswordModel(UserManager<AppUser> userManager) { 
            _userManager = userManager;
        }
        public AppUser? User { get; set; }
        public class InputModel {
            [Required(ErrorMessage = "Vui lòng nhập {0}")]
            [DisplayName("Mật khẩu mới")]
            [StringLength(100, MinimumLength = 3,ErrorMessage = "Độ dài {0} từ {2} đến {1} ký tự")]
            public string NewPassword { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập {0}")]
            [DisplayName("Xác nhận mật khẩu")]
            [Compare("NewPassword",ErrorMessage = "Mật khẩu không khớp")]
            public string ConfirmPassword { get; set; }
        }
        [BindProperty]
        public InputModel Input{get;set;}
        public async Task<IActionResult> OnGetAsync(string userid)
        {
            if(userid == null) return NotFound("Không tìm thấy User");
            User = await _userManager.FindByIdAsync(userid);
            if(User == null) return NotFound("Không tìm thấy User");

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string userid) {
            if(userid == null) return NotFound("Không tìm thấy User");
            User = await _userManager.FindByIdAsync(userid);
            if(User == null) return NotFound("Không tìm thấy User");
            if(!ModelState.IsValid) {
                return Page();
            }
            await _userManager.RemovePasswordAsync(User);
            var result = await _userManager.AddPasswordAsync(User,Input.NewPassword);
            if(result.Succeeded) {
                StatusMessage = $"Đã đặt mật khẩu cho {User.UserName} thành công";
                return RedirectToPage("./Index");
            }else {
                ModelState.AddModelError(String.Empty,"Cập nhật không thành công.Thử lại");
                return Page();
            }
        }
    }
}

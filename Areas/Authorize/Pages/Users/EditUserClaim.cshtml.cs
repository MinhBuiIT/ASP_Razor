using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ASP_RazorWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_RazorWeb.Pages.Roles.Users
{
    public class EditUserClaimModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly BlogContext _blogContext;

        [TempData]
        public string StatusMessage { get; set; }
        public EditUserClaimModel(UserManager<AppUser> userManager,BlogContext blogContext) { 
            _userManager = userManager;
            _blogContext = blogContext;
        }
        public NotFoundObjectResult OnGet() => NotFound("Không tìm thấy trang");
        
        public AppUser User{get;set;}
        public class InputModel {
            [Required(ErrorMessage = "Vui lòng nhập {0}")]
            [DisplayName("Loại đặc tính")]
            [StringLength(100,MinimumLength=3,ErrorMessage = "Độ dài {0} từ {2} đến {1} ký tự")]
            public string ClaimType{get; set; }

            [Required(ErrorMessage = "Vui lòng nhập {0}")]
            [DisplayName("Giá trị đặc tính")]
            [StringLength(100,MinimumLength=3,ErrorMessage = "Độ dài {0} từ {2} đến {1} ký tự")]
            public string ClaimValue{get; set; }
        }
        [BindProperty]
        public InputModel Input{get; set; }
        public async Task<IActionResult> OnGetAdd(string userid) {
            if(String.IsNullOrEmpty(userid)) return NotFound("Không tìm thấy user");
            User = await _userManager.FindByIdAsync(userid);
            if(User == null) return NotFound("Không tìm thấy user");
            return Page();
        }
        public async Task<IActionResult> OnPostAddAsync(string userid) {
            if(String.IsNullOrEmpty(userid)) return NotFound("Không tìm thấy user");
            User = await _userManager.FindByIdAsync(userid);
            if(User == null) return NotFound("Không tìm thấy user");
            if(!ModelState.IsValid) return Page();
            if(_blogContext.UserClaims.Any(uc => uc.ClaimValue == Input.ClaimValue && uc.ClaimType == Input.ClaimType)) {
                ModelState.AddModelError("","Đã tồn tại claim này");
                return Page();
            }
            var result = await _userManager.AddClaimAsync(User,new Claim(Input.ClaimType,Input.ClaimValue));
            if(!result.Succeeded) {
                result.Errors.ToList().ForEach(e => {
                    ModelState.AddModelError("",e.Description);
                    
                });
                return Page();
            }else {
                return RedirectToPage("./AddRole",new {userid = User.Id});
            }
        }
        public IdentityUserClaim<string>? userClaim{get;set;} = null;
        public async Task<IActionResult> OnGetEdit(int? claimid) {
            if(claimid == null) return NotFound("Không tìm thấy user");
            userClaim = _blogContext.UserClaims.Where(uc => uc.Id == claimid).FirstOrDefault();
            User = await _userManager.FindByIdAsync(userClaim.UserId);
            Input = new InputModel(){
                ClaimType = userClaim.ClaimType,
                ClaimValue = userClaim.ClaimValue
            };
            return Page();
        }
        public async Task<IActionResult> OnPostEdit(int? claimid) {
            if(claimid == null) return NotFound("Không tìm thấy user");
            userClaim = _blogContext.UserClaims.Where(uc => uc.Id == claimid).FirstOrDefault();
            User = await _userManager.FindByIdAsync(userClaim.UserId);
            if(!ModelState.IsValid) return Page();
            if(_blogContext.UserClaims.Any(uc => uc.ClaimValue == Input.ClaimValue && uc.ClaimType == Input.ClaimType && uc.Id != claimid)) {
                ModelState.AddModelError("","Đã tồn tại claim này");
                return Page();
            }

            userClaim.ClaimType = Input.ClaimType;
            userClaim.ClaimValue = Input.ClaimValue;
            _blogContext.SaveChanges();
            return RedirectToPage("./AddRole",new {userid = User.Id});
        }
        public async Task<IActionResult> OnPostDelete(int? claimid) {
            if(claimid == null) return NotFound("Không tìm thấy user");
            userClaim = _blogContext.UserClaims.Where(uc => uc.Id == claimid).FirstOrDefault();
            if(userClaim == null) {
                ModelState.AddModelError("","Lỗi Claim không tồn tại");
                return Page();
            }
            User = await _userManager.FindByIdAsync(userClaim.UserId);
            if(User == null) {
                ModelState.AddModelError("","Lỗi User không tồn tại");
                return Page();
            }

            var result = await _userManager.RemoveClaimAsync(User,new Claim(userClaim.ClaimType,userClaim.ClaimValue));
            if(!result.Succeeded) {
                result.Errors.ToList().ForEach(e => {
                    ModelState.AddModelError("",e.Description);
                    
                });
                return Page();
            }else {
                return RedirectToPage("./AddRole",new {userid = User.Id});
            }
        }
    }
}

using ASP_RazorWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ASP_RazorWeb.Pages.Roles.Users
{
    public class AddRoleModel : PageModel
    {
        UserManager<AppUser> _userManager;
        RoleManager<IdentityRole> _roleManager;

        [TempData]
        public string StatusMessage{get;set;}
        public AddRoleModel(UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager) { 
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public AppUser? User { get; set; }

        [BindProperty]
        public string[] RoleNames { get; set; }
        public SelectList RoleAll{get; set; }

        public async Task<IActionResult> OnGet(string userid)
        {
            if(userid == null) return NotFound("Không tìm thấy User");
            User = await _userManager.FindByIdAsync(userid);
            if(User == null) return NotFound("Không tìm thấy User");

            RoleNames = (await _userManager.GetRolesAsync(User)).ToArray();
            string[] roleNameList =  _roleManager.Roles.Select(r => r.Name).ToArray();
            RoleAll = new SelectList(roleNameList);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string userid)  {
            if(userid == null) return NotFound("Không tìm thấy User");
            User = await _userManager.FindByIdAsync(userid);
            if(User == null) return NotFound("Không tìm thấy User");
            
            string[] roleNameList =  _roleManager.Roles.Select(r => r.Name).ToArray();
            RoleAll = new SelectList(roleNameList);

            if(User.SecurityStamp == null) {
                ModelState.AddModelError("","Vui lòng đặt mật khẩu cho user này trước");
                return Page();
            }
            //roleNames
            var OldRoleNames = (await _userManager.GetRolesAsync(User)).ToList();

            var deleteRoleNames = OldRoleNames.Where(o => !RoleNames.Contains(o)).ToList();
            var addRoleName = RoleNames.Where(n => !OldRoleNames.Contains(n)).ToList();
            if(deleteRoleNames.Count() > 0) {
                var resultDelete = await _userManager.RemoveFromRolesAsync(User,deleteRoleNames);
                if(!resultDelete.Succeeded) {
                    foreach(var err in resultDelete.Errors.ToList()) {
                        ModelState.AddModelError(String.Empty,err.Description);    
                    }
                    return Page();
                }
            }
            if(addRoleName.Count() > 0) {
                var resultAdd = await _userManager.AddToRolesAsync(User,addRoleName);
                // return Content(resultAdd.Succeeded.ToString());
                if(!resultAdd.Succeeded) {
                    foreach(var err in resultAdd.Errors.ToList()) {
                        ModelState.AddModelError(String.Empty,err.Description);    
                    }
                    return Page();
                }
            }
            StatusMessage = "Cập nhật Role thành công";
            return RedirectToPage("./Index");
        }
    }
}

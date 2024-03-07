// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using ASP_RazorWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace ASP_RazorWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IUserStore<AppUser> userStore,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ProviderDisplayName { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// TempData như là session
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
        
        public IActionResult OnGet() => RedirectToPage("./Login");

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Lỗi cung cấp dịch vụ ngoài: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Lỗi không thể lấy được thông tin dịch vụ ngoài";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Khi user đã có tài khoản rùi
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
            _logger.LogInformation(result.Succeeded.ToString());
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Lỗi không thể lấy thông tin dịch vụ ngoài";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                //nếu nhập email trùng với email của liên kết ngoài và có trong db => xử lý liên kết
                //nếu nhập email trùng với email của liên kết ngoài không có db => tạo tkhoan, dky, lien ket
                //neu nhập email khác với email của liên kết ngoài mà 2 email đều có trong db => lỗi
                //nếu nhập email khác với email của liên kết ngoài mà email lket ngoài có trong db, email nhập không có trong db => lỗi
                var registerEmailUser = await _userManager.FindByEmailAsync(Input.Email);
                string externalEmail = null;
                if(info.Principal.HasClaim(c => c.Type == ClaimTypes.Email)) {
                    externalEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
                }
                AppUser externalEmailUser = null;
                if(externalEmail != null) {
                    externalEmailUser = await _userManager.FindByEmailAsync(externalEmail);
                }
                if(externalEmailUser != null && registerEmailUser != null) {
                    if(externalEmailUser.Id == registerEmailUser.Id) {
                        var resultLink = await _userManager.AddLoginAsync(registerEmailUser,info);
                        if(resultLink.Succeeded) {
                            await _signInManager.SignInAsync(registerEmailUser,isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }else {
                            ModelState.AddModelError(String.Empty,"Liên kết tài khoản không thành công");
                            return Page();
                        }
                    }else {
                        //neu nhập email khác với email của liên kết ngoài mà 2 email đều có trong db
                        ModelState.AddModelError(String.Empty,"Vui lòng chọn email liên kết khác");
                        return Page();
                    }
                }


                if(externalEmailUser != null && registerEmailUser == null) {
                    ModelState.AddModelError(String.Empty,"Email đăng ký không tồn tại");
                    return Page();
                }
                if(externalEmailUser == null && registerEmailUser != null) { 
                    ModelState.AddModelError(String.Empty,"Email liên kết không tồn tại ");
                    return Page();
                }
                if(externalEmailUser == null && registerEmailUser == null) {
                    if(externalEmail == Input.Email) {
                        AppUser newUser = CreateUser();
                        await _userStore.SetUserNameAsync(newUser,Input.Email,CancellationToken.None);
                        await _emailStore.SetEmailAsync(newUser,Input.Email,CancellationToken.None);
                        var resultNew = await _userManager.CreateAsync(newUser);
                        if(resultNew.Succeeded) {
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                            await _userManager.ConfirmEmailAsync(newUser,code);
                            var resultLinkNew = await _userManager.AddLoginAsync(newUser,info);
                            if(resultLinkNew.Succeeded) {
                                await _signInManager.SignInAsync(newUser,isPersistent:false);
                                return LocalRedirect(returnUrl);
                            }else {
                                ModelState.AddModelError(String.Empty,"Liên kết không thành công");
                                return Page();
                            }
                        }else {
                            ModelState.AddModelError(String.Empty,"Tạo tài khoản không thành công");
                                return Page();
                        }
                    }else {
                        ModelState.AddModelError(String.Empty,"Email không trùng khớp");
                        return Page();
                    }
                }
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }

        private AppUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
                    $"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml");
            }
        }

        private IUserEmailStore<AppUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppUser>)_userStore;
        }
    }
}

using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DataImporter.Common.Utilities;
using DataImporter.Membership.Entities;
using DataImporter.Web.Filters;
using DataImporter.Web.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace DataImporter.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailService _emailService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<IActionResult> Register(string returnUrl = null)
        {
            var model = new RegisterModel();
            model.ReturnUrl = returnUrl;
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ReCaptcha(0.5)]
        public async Task<IActionResult> Register([FromForm] RegisterModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {Name = model.Name, UserName = model.Email, Email = model.Email};
                var result = await _userManager.CreateAsync(user, model.Password);
                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("view_permission", "true"));

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");


                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        await GenerateTokenAndSendConFirmationEmailAsync(user, returnUrl);
                        return RedirectToAction(nameof(EmailConfiramtion), "Account", new {email = model.Email});
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateUserName")
                    {
                        ModelState.AddModelError(string.Empty, "An user is already registred with the email");
                    }
                    else
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [TempData] public string ErrorMessage { get; set; }

        [TempData] public string EmailConfirmSuccess { get; set; }
        [TempData] public string EmailConfirmError { get; set; }

        public async Task<IActionResult> Login(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            if (!string.IsNullOrEmpty(EmailConfirmSuccess))
            {
                ViewBag.EmailConfirmSuccess = EmailConfirmSuccess;
            }
            else if (!string.IsNullOrEmpty(EmailConfirmError))
            {
                ViewBag.EmailConfirmError = EmailConfirmError;
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var model = new LoginModel();
            model.ReturnUrl = returnUrl;
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            var r = Request;
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new {ReturnUrl = returnUrl, RememberMe = model.RememberMe});
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        var appUser = await _userManager.FindByEmailAsync(model.Email);
                        if (appUser != null)
                        {
                            bool emailStatus = await _userManager.IsEmailConfirmedAsync(appUser);
                            if (emailStatus == false)
                            {
                                ViewBag.EmailConfirmation = "Email is unconfirmed, please confirm it first";
                                return View(model);
                            }
                        }
                    }

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EmailConfiramtion(string email)
        {
            if (email == null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Email = email;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailConfirmation(string email, string returnUrl = null)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            await GenerateTokenAndSendConFirmationEmailAsync(user, returnUrl);

            return RedirectToAction(nameof(EmailConfiramtion), new {email});
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code, string returnUrl)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                //return NotFound($"Unable to load user with ID '{userId}'.");
                ViewBag.UserNotFound = $"Unable to load user with ID '{userId}'.";
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                EmailConfirmSuccess = "Thank you for confirming your email. You may now Login.";
            else
                EmailConfirmError = "Error confirming your email.";

            return RedirectToAction(nameof(Login), "Account", new {returnUrl});
        }

        [HttpGet]
        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

        private async Task GenerateTokenAndSendConFirmationEmailAsync(ApplicationUser user, string returnUrl = null)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.ActionLink(
                "ConfirmEmail", "Account",
                values: new {userId = user.Id, code = code, returnUrl = returnUrl},
                protocol: Request.Scheme);

            await _emailService.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }
    }
}
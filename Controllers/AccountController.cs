using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hermes.Models.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace OrdersManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if ((HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Order");
            }
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        /// <summary>
        /// Login Method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                IdentityUser user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email not confirmed yet");
                    return View(model);
                }

                if (await userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);
                }

                SignInResult result = await signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    await userManager.AddClaimAsync(user, new Claim("UserRole", "Admin"));
                    return RedirectToAction("Index","Order");
                }
                else if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public IActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            return View(model);
        }

        /// <summary>
        /// Register new user Method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(RegisterViewModel request)
        {
            if (request.Password != request.ConfirmPassword)
            {
                ModelState.AddModelError("message", "Password not matching");
                return View(request);
            }
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                IdentityUser userCheck = await userManager.FindByEmailAsync(request.Email);
                if (userCheck == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = request.Username,
                        NormalizedUserName = request.Username,
                        Email = request.Email,
                        PhoneNumber = request.PhoneNumber,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                    };
                    IdentityResult result = await userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Order");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (IdentityError error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(request);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists.");
                    return View(request);
                }
            }
            else
            {
                ModelState.Clear();
            }
            return View(request);
        }

        /// <summary>
        /// Sign out method
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}

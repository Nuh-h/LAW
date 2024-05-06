using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArabicLearning.Repositories.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using ArabicLearning.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Okta.AspNetCore;

namespace ArabicLearning.Controllers
{

    [Route("auth/[Action]")]
    public class AuthController : Controller
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly SignInManager<AppIdentityUser> _signInManager;
        public AuthController(UserManager<AppIdentityUser> userManager, SignInManager<AppIdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(SignupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppIdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ModelState.AddModelError(string.Empty, "Invalid Registeration Attempt");

            }

            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(SigninViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }


            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            string redirectUrl = Url.Action("ExternalAuthResponse", "Auth");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("GoogleOpenID", redirectUrl);
            return new ChallengeResult("GoogleOpenID", properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult> OktaLogin()
        {
            string redirectUrl = Url.Action("ExternalAuthResponse", "Auth");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("OktaOpenID", redirectUrl);
            return new ChallengeResult("OktaOpenID", properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ExternalAuthResponse()
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            Console.WriteLine(info);
            if (info == null)
                return RedirectToAction(nameof(Login));

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            //string[] userInfo = { info.Principal.FindFirst(ClaimTypes.Name).Value, info.Principal.FindFirst(ClaimTypes.Email).Value };
            if (result.Succeeded)
            {
                return RedirectToAction("Secured", "Home");
            }
            else
            {
                AppIdentityUser user = new AppIdentityUser
                {
                    Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                    UserName = info.Principal.FindFirst(ClaimTypes.Email).Value
                };

                //Check for duplicate user/already registered, but non-externally
                var reg = await _userManager.FindByEmailAsync(user.Email);
                if (reg != null)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Secured", "Home");
                }

                IdentityResult identResult = await _userManager.CreateAsync(user);
                if (identResult.Succeeded)
                {
                    identResult = await _userManager.AddLoginAsync(user, info);
                    if (identResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return RedirectToAction("Secured", "Home");
                    }
                }
                return RedirectToAction("Denied", "Home");
            }
        }

    }
}

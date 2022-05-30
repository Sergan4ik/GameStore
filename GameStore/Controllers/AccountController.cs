using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Models;
using GameStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly GamesDBContext _context;
        private readonly UserManager<AuthUser> _userManager;
        private readonly SignInManager<AuthUser> _signInManager;

        public AccountController(UserManager<AuthUser> userManager, SignInManager<AuthUser> signInManager, GamesDBContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                AuthUser user = new AuthUser() { Email = model.Email, UserName = model.Email, BirthDate = model.BirthDate };
                // додаємо користувача
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var newUser = await _context.Users.AddAsync(new User()
                    {
                        Email = model.Email,
                        Username = model.UserName,
                        BirthDate = model.BirthDate,
                        Balance = 0,
                    });
                    await _context.SaveChangesAsync(); 
                    await _signInManager.SignInAsync(user, false);
                
                    return RedirectToAction("Index", "GamesCopy",
                        new { userId = newUser.Entity.Id, username = newUser.Entity.Username });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl(new { controller = "GamesCopy", action = "Index" });
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // перевіряємо, чи належить URL додатку
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        User targetUser = _context.Users.First(u => u.Email == model.Email);
                        return RedirectToAction("Index", "GamesCopy",
                            new { userId = targetUser.Id, username = targetUser.Username });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильний логін чи (та) пароль");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult AccountFunding(string email)
        {
            return View(new AccountFunding(){Amount = 0 , Email = email});
        }

        [HttpPost]
        public async Task<IActionResult> AccountFunding(decimal Amount)
        {
            User targetUser = _context.GetUserByEmail(User.Identity.Name);
            targetUser.Balance += Amount;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "GamesCopy" , new{username = targetUser.Username , userId = targetUser.Id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // видаляємо аутентифікаційні куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
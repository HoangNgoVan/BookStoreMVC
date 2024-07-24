using BookStoreMVC.Models;
using BookStoreMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IWebHostEnvironment environment;


        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IWebHostEnvironment environment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.environment = environment;

        }
        public IActionResult Register()
        {
            try
            {
                if (signInManager.IsSignedIn(User))
                {
                    return RedirectToAction("Index", "Home");
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi tải trang đăng ký";
                LogEvents.LogToFile("View Register Account", ex.ToString(), environment);
                return View();
            }

        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                if (signInManager.IsSignedIn(User))
                {
                    return RedirectToAction("Index", "Home");
                }

                if (!ModelState.IsValid)
                {
                    return View(registerDto);
                }

                // create a new account and authenticate the user
                var user = new ApplicationUser()
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    UserName = registerDto.Email, // UserName will be used to autheticate the user 
                    Email = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber,
                    Address = registerDto.Address,
                    CreatedAt = DateTime.Now,
                };

                var result = await userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    // successful user registration
                    await userManager.AddToRoleAsync(user, "client");

                    // sign in the new user
                    await signInManager.SignInAsync(user, false);

                    TempData["success"] = "Tạo thành công tài khoản";

                    return RedirectToAction("Index", "Home");
                }


                // registration failed => show registration errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(registerDto);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi tạo Tài khoản";
                LogEvents.LogToFile("Register Account Post", ex.ToString(), environment);
                return View();
            }

        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                if (signInManager.IsSignedIn(User))
                {
                    await signInManager.SignOutAsync();
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi đăng xuất tài khoản";
                LogEvents.LogToFile("View Logout Account", ex.ToString(), environment);
                return View();
            }
        }


        public IActionResult Login()
        {
            try
            {
                if (signInManager.IsSignedIn(User))
                {
                    return RedirectToAction("Index", "Home");
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi tải trang đăng nhập tài khoản";
                LogEvents.LogToFile("View Login Account", ex.ToString(), environment);
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                if (signInManager.IsSignedIn(User))
                {
                    return RedirectToAction("Index", "Home");
                }

                if (!ModelState.IsValid)
                {
                    return View(loginDto);
                }

                var result = await signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password,
                    loginDto.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Đăng nhập thất bại, sai tài khoản hoặc mật khẩu";
                }

                return View(loginDto);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi đăng nhập tài khoản";
                LogEvents.LogToFile("Login Account Post", ex.ToString(), environment);
                return View();
            }

        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var appUer = await userManager.GetUserAsync(User);
                if (appUer == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var profileDto = new ProfileDto()
                {
                    FirstName = appUer.FirstName,
                    LastName = appUer.LastName,
                    Email = appUer.Email ?? "",
                    PhoneNumber = appUer.PhoneNumber,
                    Address = appUer.Address,
                };

                return View(profileDto);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi tải thông tin cá nhân";
                LogEvents.LogToFile("Get Profile User", ex.ToString(), environment);
                return View();
            }

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileDto profileDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ErrorMessage = "Vui lòng điền tất cả các trường bắt buộc";
                    return View(profileDto);
                }

                // Get the current user
                var appUer = await userManager.GetUserAsync(User);
                if (appUer == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Update the user  profile
                appUer.FirstName = profileDto.FirstName;
                appUer.LastName = profileDto.LastName;
                appUer.UserName = profileDto.Email;
                appUer.Email = profileDto.Email;
                appUer.PhoneNumber = profileDto.PhoneNumber;
                appUer.Address = profileDto.Address;

                var result = await userManager.UpdateAsync(appUer);

                if (result.Succeeded)
                {
                    ViewBag.SuccessMessage = "Cập nhật thành công thông tin";
                }
                else
                {
                    ViewBag.ErrorMessage = "Không thể cập nhật thông tin cá nhân: " + result.Errors.First().Description;
                }

                return View(profileDto);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi sửa thông tin cá nhân";
                LogEvents.LogToFile("Edit Profile User Post", ex.ToString(), environment);
                return View();
            }
        }

        [Authorize]
        public IActionResult Password()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Password(PasswordDto passwordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                // Get the current user
                var appUer = await userManager.GetUserAsync(User);
                if (appUer == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Check password exist
                var passwordExist = await userManager.CheckPasswordAsync(appUer, passwordDto.NewPassword);
                if (passwordExist)
                {
                    ViewBag.ErrorMessage = "Mật khẩu đã tồn tại";
                }

                //Update the password
                var result = await userManager.ChangePasswordAsync(appUer, passwordDto.CurrentPassword, passwordDto.NewPassword);
                if (result.Succeeded)
                {
                    ViewBag.SuccessMessage = "Cập nhật thành công mật khẩu";
                }
                else
                {
                    ViewBag.ErrorMessage = "Error: " + result.Errors.First().Description;
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi thay đổi mật khẩu";
                LogEvents.LogToFile("Edit Password Post", ex.ToString(), environment);
                return View();
            }
        }


        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([Required, EmailAddress] string email)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Email = email;

            if (!ModelState.IsValid)
            {
                ViewBag.EmailError = ModelState["email"]?.Errors.First().ErrorMessage ?? "Địa chỉ email không hợp lệ";
                return View();
            }

            var user = await userManager.FindByEmailAsync(email);

            if (user != null) 
            {
                // generate password reset token
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                string resetUrl = Url.ActionLink("ResetPasswork", "Account", new {token }) ?? "URL Error";

                Console.WriteLine("Password reset link: " + resetUrl);
            }

            ViewBag.SuccessMessage = "Vui lòng check Email của bạn và nhấp vào link để cài đặt lại mật khẩu!";

            return View();
        }
    }
}

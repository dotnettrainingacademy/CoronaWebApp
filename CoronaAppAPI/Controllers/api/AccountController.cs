using CoronaAppAPI.Models;
using CoronaAppAPI.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CoronaAppAPI.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationSettings applicationSettings;
        private readonly IEmailSender _emailSender;


        public AccountsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOptions<ApplicationSettings> applicationSettings, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            this.applicationSettings = applicationSettings.Value;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<Object> SignUpUser(SignUpViewModel model)
        {
            AppUser appUser = new AppUser()
            {
                UserName = model.FullName,
                FullName = model.FullName,
                Email = model.Email,
            };

            var applicationUser = appUser;
            try
            {
                var result = await _userManager.CreateAsync(applicationUser, model.Password);
                if (result.Succeeded)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(new { message = $"Already registered!" });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            AppUser user = new AppUser();
            if (loginViewModel.Email.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(loginViewModel.Email);
            }
            else
            {
                user = await _userManager.FindByNameAsync(loginViewModel.Email);
            }
            var status = $"{loginViewModel.Email}";
            var login = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
            if (user != null && login)
            {
                status = $"{user.UserName} is correct";
                try
                {
                    var tokenDescripter = new SecurityTokenDescriptor
                    {
                        Subject = new System.Security.Claims.ClaimsIdentity(new Claim[] {
                        new Claim("UserID", user.Id.ToString())
                    }),
                        Expires = DateTime.UtcNow.AddSeconds(20),
                        SigningCredentials = new SigningCredentials(new
                                SymmetricSecurityKey(Encoding.UTF8.GetBytes(applicationSettings.JWT_Secret)),
                                SecurityAlgorithms.HmacSha256Signature)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescripter);
                    var token = tokenHandler.WriteToken(securityToken);
                    return Ok(new { token, user.Id });
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                return BadRequest(new { message = $"Invalid username or password {login}" });
            }
        }

        [HttpPost]
        [Route("ForgetPassword")]
        public async Task<IActionResult> PasswordReset(PasswordResetViewModel passwordResetViewModel)
        {
            var user = await _userManager.FindByEmailAsync(passwordResetViewModel.Email);
            if (user != null) //  && await _userManager.IsEmailConfirmedAsync(user)
            {
                string token;
                try
                {
                    token = await _userManager.GeneratePasswordResetTokenAsync(user);
                }
                catch (Exception)
                {

                    throw;
                }
                var callbackUrl = Url.Page(
                      "/Account/ResetPassword",
                      pageHandler: null,
                      values: new { token },
                      protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    passwordResetViewModel.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return Ok();
            }
            else
            {
                return Ok(new { message = "something bad happended" });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetUserData")]
        public async Task<Object> GetUserData()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }
            return new
            {
                user.Id,
                user.FullName,
                user.Email,
            };
        }

        [HttpGet]
        [Authorize]
        [Route("GetAllUsers")]
        public async Task<Object> GetAllUsers()
        {
            var allusers =await _userManager.Users.ToListAsync();
            if (allusers != null)
            {
                return Ok(allusers);
            }
            else
            {
                return BadRequest(new { Message = "No Data Found :(" });
            }
        }
    }
}
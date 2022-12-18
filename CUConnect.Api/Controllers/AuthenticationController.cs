using CUConnect.Database;
using CUConnect.Database.Entities;
using CUConnect.Models;
using CUConnect.Models.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CUConnect.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        //Identity
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        //======================================================================================================================================
        public AuthenticationController(IConfiguration configuration, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        [HttpPost,Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserView request)
        {
            var IsExist = await _userManager.FindByEmailAsync(request.Email);
            if (IsExist != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });

            var appUser = new AppUser()
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender,
            };

            var userResult = await _userManager.CreateAsync(appUser, request.Password);
            if (!userResult.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            if (!await _roleManager.RoleExistsAsync(Roles.User.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
            if (await _roleManager.RoleExistsAsync(Roles.User.ToString()))
            {
                await _userManager.AddToRoleAsync(appUser, Roles.User.ToString());
                await _userManager.AddClaimAsync(appUser, new Claim(ClaimTypes.Role, Roles.User.ToString()));
            }

            return Ok(new { Status = "Success", Message = "User created successfully!" });
        }


        //---------------------------------------------------------------------------------------------------------------------------------------

        [HttpPost,Route("Login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginUserView request)
        {
            var userResult = await _userManager.FindByEmailAsync(request.Email);
            if (userResult != null && await _userManager.CheckPasswordAsync(userResult, request.Password))
            {
               IList<Claim> claim=await _userManager.GetClaimsAsync(userResult);
                string token = CreatToken(userResult,claim);
                return Ok(token);
            }
            return Unauthorized();
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //JWT Token
        private string CreatToken(AppUser user, IList<Claim> claim)
        {
            var profileID = FindProfile(user.Id);
            
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Email, user.UserName));
            claims.Add(new Claim(ClaimTypes.Name, user.FirstName));
            claims.Add(new Claim(ClaimTypes.UserData, profileID.ToString()));
            foreach(var claimItem in claim)
                claims.Add(new Claim(ClaimTypes.Role, claimItem.Value));

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("JsonWebTokenKeys:IssuerSigningKey").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("JsonWebTokenKeys:ValidIssuer").Value,
                audience: _configuration.GetSection("JsonWebTokenKeys:ValidAudience").Value,
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: cred
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        //--------------------------------------------------------------------------------------------------------------------------------------
        private int FindProfile(string userID)
        {
            using(var _dbContext=new CUConnectDBContext())
            {
                var profile=_dbContext.Profiles.Where(x=>x.UserId.Equals(userID)).FirstOrDefault();
                return profile != null ? profile.ProfileId : -1;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------

        [HttpPost,Route("changePassword")]
        [Authorize(Roles=nameof(Roles.User))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordView changePassword)
        {
            var userResult=await _userManager.FindByEmailAsync(changePassword.Email);
            if (userResult != null)
            {
                var status=await _userManager.ChangePasswordAsync(userResult,changePassword.OldPassword,changePassword.NewPassword);
                if(status.Succeeded)
                    return Ok(new { Status = "Success", Message = "Password Changed!" });
            }
            return BadRequest(new { Status = "Faild", Message = "Try Again!" });
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //[HttpPost,Route("forgotPassword")]
        //public async Task<IActionResult> ForgotPassword()
    }
}

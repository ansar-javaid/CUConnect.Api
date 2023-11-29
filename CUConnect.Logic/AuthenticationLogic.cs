using CUConnect.Database;
using CUConnect.Database.Entities;
using CUConnect.Models;
using CUConnect.Models.Repository;
using CUConnect.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CUConnect.Logic
{
    public class AuthenticationLogic : ControllerBase, IAuthenticationREPO
    {
        private readonly IConfiguration _configuration;
        //Identity
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        //Constructor
        public AuthenticationLogic(IConfiguration configuration, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Register([FromBody] RegisterUserView request)
        {
            // Check if a user with the same email already exists in the system.
            var IsExist = await _userManager.FindByEmailAsync(request.Email);
            if (IsExist != null)
                // If a user with the same email already exists, return a 500 Internal Server Error with an error message.
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });

            // If a user with the same email does not exist, create a new AppUser object with the input request properties.
            var appUser = new AppUser()
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender,
            };

            // Create the new user in the system using the injected UserManager object.
            var userResult = await _userManager.CreateAsync(appUser, request.Password);
            if (!userResult.Succeeded)
                // If user creation fails, return a 500 Internal Server Error with an error message.
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            // Check if the "User" role exists in the system using the injected RoleManager object.
            if (!await _roleManager.RoleExistsAsync(Roles.User.ToString()))
                // If the "User" role does not exist, create a new role with the name "User".
                await _roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));

            // If the "User" role exists or was just created, add it to the newly created user.
            if (await _roleManager.RoleExistsAsync(Roles.User.ToString()))
            {
                await _userManager.AddToRoleAsync(appUser, Roles.User.ToString());
                // Add a claim to the user indicating that they belong to the "User" role.
                await _userManager.AddClaimAsync(appUser, new Claim(ClaimTypes.Role, Roles.User.ToString()));
            }

            return Ok(new { Status = "Success", Message = "User created successfully!" });
        }



        public async Task<ActionResult<string>> Login([FromBody] LoginUserView request)
        {
            var userResult = await _userManager.FindByEmailAsync(request.Email);
            if (userResult != null && await _userManager.CheckPasswordAsync(userResult, request.Password))
            {
                IList<Claim> claim = await _userManager.GetClaimsAsync(userResult);
                string token = CreatToken(userResult, claim);
                return StatusCode(StatusCodes.Status200OK, new { Token = token });
            }
            return StatusCode(StatusCodes.Status401Unauthorized, new { Data = "Check User Name or Password again!" });
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //JWT Token
        private string CreatToken(AppUser user, IList<Claim> claim)
        {
            var profileID = FindProfile(user.Id);

            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim("Email", user.UserName));
            claims.Add(new Claim("Name", user.FirstName));
            claims.Add(new Claim("Profile", profileID.ToString()));
            foreach (var claimItem in claim)
                claims.Add(new Claim(ClaimTypes.Role, claimItem.Value));

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("JsonWebTokenKeys:IssuerSigningKey").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("JsonWebTokenKeys:ValidIssuer").Value,
                audience: _configuration.GetSection("JsonWebTokenKeys:ValidAudience").Value,
                claims: claims,
                expires: DateTime.Now.AddDays(14),
                signingCredentials: cred
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        //--------------------------------------------------------------------------------------------------------------------------------------
        private int FindProfile(string userID)
        {
            using (var _dbContext = new CUConnectDBContext())
            {
                var profile = _dbContext.Profiles.Where(x => x.UserId.Equals(userID)).FirstOrDefault();
                return profile != null ? profile.ProfileId : -1;
            }
        }


        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordView changePassword)
        {
            var userResult = await _userManager.FindByEmailAsync(changePassword.Email);
            if (userResult != null)
            {
                var status = await _userManager.ChangePasswordAsync(userResult, changePassword.OldPassword, changePassword.NewPassword);
                if (status.Succeeded)
                    return Ok(new { Status = "Success", Message = "Password Changed!" });
            }
            return BadRequest(new { Status = "Faild", Message = "Try Again!" });
        }

    }
}

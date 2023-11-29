using CUConnect.Models.Repository;
using CUConnect.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace CUConnect.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationREPO _authentication;

        public AuthenticationController(IAuthenticationREPO authenticationREPO)
        {
            _authentication = authenticationREPO;
        }


        /// <summary>
        /// Register a new user in Database
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserView request)
        {
            return Ok(await _authentication.Register(request));
        }


        /// <summary>
        /// Return JWT Token, with Email, Name, ProfileID=ProfileID (if -1 then No Profile is created yet), and with User Role
        /// 
        /// Decode it inside the code, Help jwt.io
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("Login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginUserView request)
        {
            var result = await _authentication.Login(request);
            return Ok(result.Result);
        }


        /// <summary>
        /// Use to change password, accepts valid user's email, with old and new password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost, Route("changePassword")]

        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordView request)
        {
            return Ok(await _authentication.ChangePassword(request));
        }
    }
}

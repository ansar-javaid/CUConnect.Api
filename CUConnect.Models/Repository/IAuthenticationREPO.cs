using CUConnect.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace CUConnect.Models.Repository
{
    public interface IAuthenticationREPO
    {
        public Task<IActionResult> Register(RegisterUserView request);
        public Task<ActionResult<string>> Login(LoginUserView request);
        public Task<IActionResult> ChangePassword(ChangePasswordView changePassword);
    }
}

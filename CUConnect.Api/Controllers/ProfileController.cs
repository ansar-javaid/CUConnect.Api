using CUConnect.Database;
using CUConnect.Logic;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CUConnect.Api.Controllers
{


    [Route("api/profile")]
    [ApiController]
    // [Authorize(Roles = nameof(Roles.User))]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ProfileLogic _logic;

        public ProfileController(UserManager<AppUser> userManager, IHostEnvironment environment)
        {
            _userManager = userManager;
            _logic = new ProfileLogic(_userManager, environment);
        }

        [HttpGet, Route("GetAllDepartment")]
        public IActionResult GetDepartments()
        {
            return Ok(new { Database = _logic.GetAllDepartment() });
        }

        /// <summary>
        /// Returns a single Profile associated with its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("GetProfileOnly")]
        public async Task<ActionResult<ProfileOnlyViewRES>> GetProfileById(int id)
        {
            var result = await _logic.GetProfileOnly(id);
            return Ok(new { Database = result != null ? result : null });
        }


        /// <summary>
        /// First Create Departments i.e (Computer Science, Electical Engeniering, Adminstration, Accounts)
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>

        [HttpPost, Route("CreatDepartment")]
        public IActionResult CreatDepartments([FromBody] DepartmentView department)
        {
            return Ok(new { Database = _logic.CreatDepartment(department) });
        }


        /// <summary>
        /// After Creating Departments, create Profile under that Department by selecting the department name using a registered user
        /// email. i.e (department of Computer Science, IEEE, Accounts Office, Clasees BCS-1A, BCS-8A)
        /// </summary>
        /// <param name="profileView"></param>
        /// <returns></returns>

        [HttpPost, Route("CreatProfile")]
        public async Task<IActionResult> CreatProfile([FromForm] ProfileView profileView)
        {
            var result = await _logic.CreatProfile(profileView);
            return Ok(result);
        }
        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// This return the list of all users, available for profile registration/association.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("AvailableUsers")]
        public async Task<ActionResult<RegisteredUsersViewRES>> GetRegisteredUsers()
        {
            var result = await _logic.GetRegisteredUsers();
            return Ok(result);
        }


        /// <summary>
        /// Retuen the list off all Profiles, registered in System.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetAllProfiles")]
        public async Task<ActionResult<RegisteredUsersViewRES>> GetAllProfiles()
        {
            var result = await _logic.GetAllProfiles();
            return Ok(result);
        }



    }
}

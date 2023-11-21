using CUConnect.Database;
using CUConnect.Models;
using CUConnect.Models.Repository;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CUConnect.Api.Controllers
{


    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {


        private readonly IProfileREPO _profile;

        public ProfileController(UserManager<AppUser> userManager, IHostEnvironment environment, IProfileREPO profile)
        {
            _profile = profile;
        }


        /// <summary>
        /// Returns the list of All departments from Database
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetAllDepartment")]
        //[Authorize(Roles = nameof(Roles.Supper))]
        public async Task<ActionResult<DepartmentViewRES>> GetDepartments()
        {
            return Ok(await _profile.GetAllDepartment());
        }

        /// <summary>
        /// Returns a single Profile associated with its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("GetProfileOnly")]
        public async Task<ActionResult<ProfileOnlyViewRES>> GetProfileById(int id)
        {
            var result = await _profile.GetProfileOnly(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// This return the list of all users, available for profile registration/association.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("AvailableUsers")]
       // [Authorize(Roles = nameof(Roles.Supper))]
        public async Task<ActionResult<RegisteredUsersViewRES>> GetRegisteredUsers()
        {
            return Ok(await _profile.GetRegisteredUsers());
        }


        /// <summary>
        /// Retuen the list off all Profiles, registered in System.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetAllProfiles")]
       // [Authorize(Roles = nameof(Roles.User))]
        public async Task<ActionResult<RegisteredUsersViewRES>> GetAllProfiles()
        {
            return Ok(await _profile.GetAllProfiles());
        }


        /// <summary>
        /// First Create Departments i.e (Computer Science, Electical Engeniering, Adminstration, Accounts)
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        //[Authorize(Roles = nameof(Roles.Supper))]
        [HttpPost, Route("CreatDepartment")]
        public async Task<ActionResult> CreatDepartments([FromBody] DepartmentView department)
        {
            return Ok(await _profile.CreatDepartment(department));
        }


        /// <summary>
        /// After Creating Departments, create Profile under that Department by selecting the department name using a registered user
        /// email. i.e (department of Computer Science, IEEE, Accounts Office, Clasees BCS-1A, BCS-8A)
        /// </summary>
        /// <param name="profileView"></param>
        /// <returns></returns>
        //[Authorize(Roles = nameof(Roles.Supper))]
        [HttpPost, Route("CreatProfile")]
        public async Task<ActionResult> CreatProfile([FromForm] ProfileView profileView)
        {
            return Ok(await _profile.CreatProfile(profileView));
        }



    }
}

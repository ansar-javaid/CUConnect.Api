using AutoMapper;
using CUConnect.Database;
using CUConnect.Database.Entities;
using CUConnect.Logic;
using CUConnect.Models;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CUConnectDBContext = CUConnect.Database.Entities.CUConnectDBContext;

namespace CUConnect.Api.Controllers
{


    [Route("api/profile")]
    [ApiController]
   // [Authorize(Roles = nameof(Roles.User))]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ProfileLogic _logic;

        public ProfileController(UserManager<AppUser> userManager,IHostEnvironment environment)
        {
            _userManager = userManager;
            _logic=new ProfileLogic(_userManager,environment);
        }

        [HttpGet,Route("GetAllDepartment")]
        public IActionResult GetDepartments()
        {
           return Ok(new { Database= _logic.GetAllDepartment()});
        }


        [HttpGet, Route("GetProfileOnly")]
        public async Task<IActionResult> GetProfileById(int id)
        {
            var result = await _logic.GetProfileOnly(id);
            return Ok(new { Database=result != null ? result : null });
        }


        /// <summary>
        /// First Create Departments i.e (Computer Science, Electical Engeniering, Adminstration, Accounts)
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>

        [HttpPost,Route("CreatDepartment")]
        public IActionResult CreatDepartments([FromBody] DepartmentView department)
        {
            return Ok(new { Database = _logic.CreatDepartment(department)});
        }


        /// <summary>
        /// After Creating Departments, create Profile under that Department by selecting the department name using a registered user
        /// email. i.e (department of Computer Science, IEEE, Accounts Office, Clasees BCS-1A, BCS-8A)
        /// </summary>
        /// <param name="profileView"></param>
        /// <returns></returns>

        [HttpPost,Route("CreatProfile")]
        public async Task<IActionResult> CreatProfile([FromForm] ProfileView profileView)
        {
            var result=await _logic.CreatProfile(profileView);
            return Ok(result);
        }
        //-----------------------------------------------------------------------------------------


    }
}

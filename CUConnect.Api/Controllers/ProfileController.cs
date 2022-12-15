using AutoMapper;
using CUConnect.Database;
using CUConnect.Database.Entities;
using CUConnect.Logic;
using CUConnect.Models;
using CUConnect.Models.RequestModels;
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
    [Authorize(Roles = nameof(Roles.User))]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ProfileLogic _logic;

        public ProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _logic=new ProfileLogic(_userManager);
        }

        [HttpGet,Route("GetAllDepartment")]
        public IActionResult GetDepartments()
        {
           return Ok(new { Database= _logic.GetAllDepartment()});
        }

        [HttpPost,Route("CreatDepartment")]
        public IActionResult CreatDepartments([FromBody] DepartmentView department)
        {
            return Ok(new { Database = _logic.CreatDepartment(department)});
        }

        [HttpPost,Route("CreatProfile")]
        public async Task<IActionResult> CreatProfile([FromBody] ProfileView profileView)
        {
            var result=await _logic.CreatProfile(profileView);
            return Ok(result);
        }

    }
}

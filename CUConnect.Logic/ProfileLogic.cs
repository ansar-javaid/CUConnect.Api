using AutoMapper;
using CUConnect.Database;
using CUConnect.Database.Entities;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CUConnectDBContext = CUConnect.Database.Entities.CUConnectDBContext;
using Profile = CUConnect.Database.Entities.Profile;

namespace CUConnect.Logic
{
    public class ProfileLogic : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        public ProfileLogic(UserManager<AppUser> userManager)
        {
            _userManager=userManager;
        }

    
        public async Task<IEnumerable<Department>> GetAllDepartment()
        {
           
            using (var _dbContext =new  CUConnectDBContext())
            {               
                    var result = await _dbContext.Departments.ToListAsync();
                return result;              
            }
        }

        public async Task<IActionResult> CreatDepartment(DepartmentView view)
        {
            using (var _dbContext = new CUConnectDBContext())
            {
                var department = new Department()
                {
                    Name = view.Name
                };
                 _dbContext.Departments.Add(department);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
        }


        public async Task<IActionResult> CreatProfile(ProfileView profileView)
        {
            var user = await _userManager.FindByEmailAsync(profileView.Email);
            if (user != null)
            {
                using (var _dbContext = new CUConnectDBContext())
                {
                    if (profileView.Department == 0 && profileView.Class == 0 && profileView.Email!=null)
                    {
                        var profile = new Profile()
                        {
                            UserId = user.Id,
                            Title = profileView.Title,
                            Description = profileView.Description,
                            CreatedOn = DateTime.Now,
                        };
                        _dbContext.Profiles.Add(profile);
                        await _dbContext.SaveChangesAsync();
                        return StatusCode(StatusCodes.Status201Created, new {Status="Success", For=profileView.Email, Level="Admin"});
                    }

                    else if (profileView.Department != 0 && profileView.Email != null)
                    {
                        var profile = new Profile()
                        {
                            UserId = user.Id,
                            Title = profileView.Title,
                            Description = profileView.Description,
                            DepartmentId = profileView.Department,
                            CreatedOn = DateTime.Now,
                        };
                        _dbContext.Profiles.Add(profile);
                        await _dbContext.SaveChangesAsync();
                        return StatusCode(StatusCodes.Status201Created, new { Status = "Success", For = profileView.Email, Level = "Department" });
                    }

                    else if (profileView.Department != 0 && profileView.Class != 0 && profileView.Email != null)
                    {
                        var profile = new Profile()
                        {
                            UserId = user.Id,
                            Title = profileView.Title,
                            Description = profileView.Description,
                            DepartmentId = profileView.Department,
                            ClassId=profileView.Class,
                            CreatedOn = DateTime.Now,
                        };
                        _dbContext.Profiles.Add(profile);
                        await _dbContext.SaveChangesAsync();
                        return StatusCode(StatusCodes.Status201Created, new { Status = "Success", For = profileView.Email, Level = "Cr" });
                    }

                }
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }
    }
}

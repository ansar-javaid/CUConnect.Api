using AutoMapper;
using CUConnect.Database;
using CUConnect.Database.Entities;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using CUConnectDBContext = CUConnect.Database.Entities.CUConnectDBContext;
using Profile = CUConnect.Database.Entities.Profile;

namespace CUConnect.Logic
{
    public class ProfileLogic : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly FileUploadLogic _fileUploadLogic;
        public ProfileLogic(UserManager<AppUser> userManager, IHostEnvironment environment)
        {
            _userManager = userManager;
            _fileUploadLogic = new FileUploadLogic(environment);
        }


        public async Task<IEnumerable<Department>> GetAllDepartment()
        {

            using (var _dbContext = new CUConnectDBContext())
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
            if (user != null )
            {
                if (profileView.File.Length>0)
                {
                    using (var _dbContext = new CUConnectDBContext())
                    {
                        //If DepartementID and ClassID both are 0 then its an Adminstrative Profile
                        if (profileView.DepartmentId == 0 && profileView.ClassId == 0 && profileView.Email != null)
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
                            var result = await _fileUploadLogic.Upload(profileView.File, profile);
                            return StatusCode(StatusCodes.Status201Created, new { Status = "Success", For = profileView.Email, Level = "Admin", Uploaded=result.status });
                        }

                        else if (profileView.DepartmentId != 0 && profileView.Email != null)
                        {
                            var profile = new Profile()
                            {
                                UserId = user.Id,
                                Title = profileView.Title,
                                Description = profileView.Description,
                                DepartmentId = profileView.DepartmentId,
                                CreatedOn = DateTime.Now,
                            };
                            _dbContext.Profiles.Add(profile);
                            await _dbContext.SaveChangesAsync();
                            var result = await _fileUploadLogic.Upload(profileView.File, profile);
                            return StatusCode(StatusCodes.Status201Created, new { Status = "Success", For = profileView.Email, Level = "Department", Uploaded = result.status });
                        }

                        else if (profileView.DepartmentId != 0 && profileView.ClassId != 0 && profileView.Email != null)
                        {
                            var profile = new Profile()
                            {
                                UserId = user.Id,
                                Title = profileView.Title,
                                Description = profileView.Description,
                                DepartmentId = profileView.DepartmentId,
                                ClassId = profileView.ClassId,
                                CreatedOn = DateTime.Now,
                            };
                            _dbContext.Profiles.Add(profile);
                            await _dbContext.SaveChangesAsync();
                            return StatusCode(StatusCodes.Status201Created, new { Status = "Success", For = profileView.Email, Level = "Cr" });
                        }

                    }
                }
                return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Please Select Profile Image!" });
            }
            return StatusCode(StatusCodes.Status404NotFound, new { Status="Registered User Email Not Found!" });
        }

        public async Task<List<PostViewRES>> GetProfileWithPosts(int profileId)
        {
            using (var _dbContext = new CUConnectDBContext())
            {
                //var profile = await _dbContext.Profiles.Where(x => x.ProfileId == profileId).Include(x => x.Posts).ToListAsync();
                return await (from x in _dbContext.Profiles
                              where (x.ProfileId == profileId)
                              join y in _dbContext.Posts
                              on x.ProfileId equals y.ProfileId
                              select new PostViewRES
                              {
                                  ProfileID = x.ProfileId,
                                  ProfileTitle = x.Title,


                                  PostID = y.PostId,
                                  PostDescription = y.Description,
                                  PostsCreatedOn = y.PostedOn,

                              }).ToListAsync();
            }
        }//------------------------------------------------------------------------------------------------------------------------

        public async Task<ProfileOnlyViewRES> GetProfileOnly(int profileId)
        {
            using (var _dbContext = new CUConnectDBContext())
            {
                return await (from x in _dbContext.Profiles
                              where (x.ProfileId == profileId)

                              select new ProfileOnlyViewRES
                              {
                                  ProfileID = x.ProfileId,
                                  ProfileTitle = x.Title,
                                  ProfileDescription = x.Description,
                              }).FirstOrDefaultAsync();

            }
        }
        //---------------------------------------------------------------------------------------------------------------------------
        
    }
}

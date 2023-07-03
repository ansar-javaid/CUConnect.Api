using CUConnect.Database;
using CUConnect.Database.Entities;
using CUConnect.Models;
using CUConnect.Models.Repository;
using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using CUConnectDBContext = CUConnect.Database.Entities.CUConnectDBContext;
using Profile = CUConnect.Database.Entities.Profile;

namespace CUConnect.Logic
{
    public class ProfileLogic : ControllerBase, IProfileREPO
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly FileUploadLogic _fileUploadLogic;
        public ProfileLogic(UserManager<AppUser> userManager, IHostEnvironment environment)
        {
            _userManager = userManager;
            _fileUploadLogic = new FileUploadLogic(environment);
        }






        #region All Departments
        public async Task<List<DepartmentViewRES>> GetAllDepartment()
        {
            using (var _dbContext = new CUConnectDBContext())
            {
                var result = await _dbContext.Departments
                    .Select(x => new DepartmentViewRES
                    {
                        DepartmentId = x.DepartmentId,
                        Name = x.Name
                    }).ToListAsync();
                return result;
            }
        }
        #endregion



        #region Create Department
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
                return Ok("Department Created");
            }
        }
        #endregion


        #region Create Profile
        public async Task<IActionResult> CreatProfile(ProfileView profileView)
        {
            var user = await _userManager.FindByEmailAsync(profileView.Email);
            if (user != null)
            {
                if (profileView.File.Length > 0)
                {
                    using (var _dbContext = new CUConnectDBContext())
                    {
                        var profieResult = _dbContext.Profiles.Where(x => x.UserId.Equals(user.Id)).FirstOrDefault();
                        if (profieResult != null)
                            return StatusCode(StatusCodes.Status400BadRequest, new { Status = "One user can have only one profile", Profile = profieResult.Title });
                        //If DepartementID and ClassID both are null then its an Adminstrative Profile
                        if (profileView.DepartmentId == null && profileView.ClassId == null && profileView.Email != null)
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
                            await MakeAdmin(user);
                            var result = await _fileUploadLogic.Upload(profileView.File, profile);
                            return StatusCode(StatusCodes.Status201Created, new { Status = "Success", For = profileView.Email, Level = "Admin", Uploaded = result.status });
                        }

                        //If DepartementID is not null and ClassID is null then its an Departmental Profile
                        else if (profileView.DepartmentId != null && profileView.ClassId == null && profileView.Email != null)
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
                            await MakeAdmin(user);
                            var result = await _fileUploadLogic.Upload(profileView.File, profile);
                            return StatusCode(StatusCodes.Status201Created, new { Status = "Success", For = profileView.Email, Level = "Department", Uploaded = result.status });
                        }

                        //If DepartementID is  null and ClassID is not null then its an Cr. Profile
                        else if (profileView.DepartmentId == null && profileView.ClassId != null && profileView.Email != null)
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
                            await MakeAdmin(user);
                            var result = await _fileUploadLogic.Upload(profileView.File, profile);
                            return StatusCode(StatusCodes.Status201Created, new { Status = "Success", For = profileView.Email, Level = "Cr", Uploaded = result.status });
                        }

                    }
                }
                return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Please Select Profile Image!" });
            }
            return StatusCode(StatusCodes.Status404NotFound, new { Status = "Registered User Email Not Found!" });
        }
        #endregion

        public async Task<List<RegisteredUsersViewRES>> GetRegisteredUsers()
        {
            using (var _dbContext = new CUConnectDBContext())
            {

                return await _dbContext.AspNetUsers
                                        .Include(u => u.AspNetUserClaims)
                                        .Where(u => u.AspNetUserClaims.Any(c => c.ClaimValue.Equals(nameof(Roles.User)))) //for User
                                        .Select(u => new RegisteredUsersViewRES
                                        {
                                            Email = u.Email,
                                            FirstName = u.FirstName,
                                            LastName = u.LastName,
                                            JoinedOn = u.UserJoined,
                                            UserRole = u.AspNetUserClaims.FirstOrDefault(c => c.ClaimValue.Equals(nameof(Roles.User))).ClaimValue //for User
                                        })
                                        .ToListAsync();
            }


            //return await (from user in _dbContext.AspNetUsers
            //             join claim in _dbContext.AspNetUserClaims
            //             on user.Id equals claim.UserId
            //             on user.Id equals claim.UserId
            //             where claim.ClaimValue.Equals(nameof(Roles.User))
            //             select new RegisteredUsersViewRES
            //             {
            //                 Email = user.Email,
            //                 FirstName = user.FirstName,
            //                 LastName = user.LastName,
            //                 JoinedOn = user.UserJoined,
            //                 UserRole = claim.ClaimValue
            //             }).ToListAsync();

        }



        public async Task<ProfileOnlyViewRES> GetProfileOnly(int profileId)
        {
            IHttpContextAccessor httpContext = new HttpContextAccessor();
            var request = httpContext.HttpContext.Request;
            var host = $"{request.Scheme}://{request.Host}/files/";
            using (var _dbContext = new CUConnectDBContext())
            {
                return await (from x in _dbContext.Profiles
                              where (x.ProfileId == profileId)

                              select new ProfileOnlyViewRES
                              {
                                  ProfileID = x.ProfileId,
                                  ProfileTitle = x.Title,
                                  ProfileDescription = x.Description,
                                  Path = host + x.Documents.FirstOrDefault().Name // Include the related Document entity
                              }).FirstOrDefaultAsync();

            }
        }
        //---------------------------------------------------------------------------------------------------------------------------

        public async Task<List<ProfileOnlyViewRES>> GetAllProfiles()
        {
            IHttpContextAccessor httpContext = new HttpContextAccessor();
            var request = httpContext.HttpContext.Request;
            var host = $"{request.Scheme}://{request.Host}/files/";
            using (var _dbContext = new CUConnectDBContext())
            {
                return await _dbContext.Profiles
                    .Include(x => x.Documents)
                    .Where(p => p.ProfileId.Equals(p.ProfileId))
                    .Select(x => new ProfileOnlyViewRES
                    {
                        ProfileID = x.ProfileId,
                        ProfileTitle = x.Title,
                        ProfileDescription = x.Description,
                        Path = host + x.Documents.FirstOrDefault().Name // Include the related Document entity
                    }).ToListAsync();

            }
        }




        private async Task MakeAdmin(AppUser user)
        {
            // Remove the existing claim from the user
            var existingClaims = await _userManager.GetClaimsAsync(user);
            if (existingClaims != null && existingClaims.Count > 0)
            {
                foreach (var claim in existingClaims)
                {
                    if (claim.Type == ClaimTypes.Role && claim.Value == Roles.User.ToString())
                    {
                        await _userManager.RemoveClaimAsync(user, claim);
                    }
                }
            }

            // Add a new claim to the user
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, Roles.Admin.ToString()));
        }

    }
}

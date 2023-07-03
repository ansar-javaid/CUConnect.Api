using CUConnect.Models.RequestModels;
using CUConnect.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace CUConnect.Models.Repository
{
    public interface IProfileREPO
    {
        #region GET Methods
        public Task<List<DepartmentViewRES>> GetAllDepartment();
        public Task<List<RegisteredUsersViewRES>> GetRegisteredUsers();
        public Task<ProfileOnlyViewRES> GetProfileOnly(int profileId);
        public Task<List<ProfileOnlyViewRES>> GetAllProfiles();
        #endregion

        #region POST Methods
        public Task<IActionResult> CreatDepartment(DepartmentView view);
        public Task<IActionResult> CreatProfile(ProfileView profileView);
        #endregion

    }
}

using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUConnect.Models.ResponseModels
{
    public class ProfileAdminViewRES
    {
       
        public ProfileDetail ProfileDetail { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string UserRole { get; set; }


    }
    public class ProfileDetail
    {
        public int ProfileId { get; set; }
        public string ProfileTitle { get; set; }
    }
}

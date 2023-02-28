using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUConnect.Models.ResponseModels
{
    public class ProfileOnlyViewRES
    {
        public int ProfileID { get; set; }

        public string? ProfileTitle { get; set; }

        public string? ProfileDescription { get; set; }
    }
}

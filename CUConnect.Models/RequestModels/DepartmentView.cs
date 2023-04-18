using System.ComponentModel.DataAnnotations;

namespace CUConnect.Models.RequestModels
{
    public class DepartmentView
    {
        [Required]
        public string Name { get; set; }
    }
}

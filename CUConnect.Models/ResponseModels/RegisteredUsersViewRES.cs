namespace CUConnect.Models.ResponseModels
{
    public class RegisteredUsersViewRES
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime JoinedOn { get; set; }
        public string UserRole { get; set; }

    }
}

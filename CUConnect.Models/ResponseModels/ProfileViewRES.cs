namespace CUConnect.Models.ResponseModels
{
    public class ProfileViewRES
    {
        public int ProfileID { get; set; }

        public string? ProfileTitle { get; set; }

        public string? ProfileDescription { get; set; }

        public DateTime ProfileCreatedOn { get; set; }


        public int PostID { get; set; }

        public string? PostDescription { get; set; }

        public DateTime? PostsCreatedOn { get; set; }
    }
}

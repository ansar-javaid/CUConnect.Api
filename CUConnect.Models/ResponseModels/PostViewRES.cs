namespace CUConnect.Models.ResponseModels
{
    public class PostViewRES
    {
        public int ProfileID { get; set; }

        public string? ProfileTitle { get; set; }

        public Cover? CoverPicture { get; set; }

        public class Cover
        {
            public string? ProfileImage { get; set; }
        }




        public int PostID { get; set; }

        public string? PostDescription { get; set; }

        public DateTime? PostsCreatedOn { get; set; }
        public bool Reaction { get; set; }

        public List<Files>? FilePath { get; set; }



        public class Files
        {
            public string? Path { get; set; }
        }


    }
}

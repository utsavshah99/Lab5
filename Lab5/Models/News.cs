namespace Lab5.Models
{
    public class News
    {
        public int Id { get; set; }
        public string SportClubId { get; set; }
        public string Title {  get; set; }
        public string Description {  get; set; }
        public string ImageUrl { get; set; }
        public SportClub SportClub { get; set; }  // Navigation property
    }

}

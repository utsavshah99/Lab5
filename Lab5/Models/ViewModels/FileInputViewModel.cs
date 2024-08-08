using System.ComponentModel.DataAnnotations;

namespace Lab5.Models.ViewModels
{
    public class FileInputViewModel
    {
        
        public string SportClubId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public IFormFile File { get; set; }

    }
}

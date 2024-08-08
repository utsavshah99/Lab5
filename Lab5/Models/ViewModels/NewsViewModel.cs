using System.Collections.Generic;

namespace Lab5.Models.ViewModels
{
    public class NewsViewModel
    {
        public SportClub SportClub { get; set; }
        public IEnumerable<News> News { get; set; }
    }

}

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Project.Model.Model.News
{
    public class NewsInput
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Short description is required")]
        [MinLength(3, ErrorMessage = "Short description requires at least 3 characters")]
        public string ShortDescription { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [MinLength(5, ErrorMessage = "Description requires at least 5 characters")]
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }

    public class NewsInputEdit
    {
        [Required(ErrorMessage = "Id is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Id is required")]
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }
}

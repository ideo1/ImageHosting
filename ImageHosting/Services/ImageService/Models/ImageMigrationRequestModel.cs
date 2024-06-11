using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ImageHosting.Services.ImageService.Models
{
    public class ImageMigrationRequestModel
    {
        [FromQuery(Name = "startPageNumber")]
        [Required]
        [Range(0, 10000, ErrorMessage = "Please enter a value bigger than {1}")]
        public int StartPageNumber { get; set; }
        [FromQuery(Name = "numberOfPagesToProcess")]
        [Range(1, 500, ErrorMessage = "Please enter a value bigger than {1}")]
        public int NumberOfPagesToProcess { get; set; } = 1;
    }
}

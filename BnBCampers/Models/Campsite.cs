using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BnBCampers.Models
{
    public class CampsiteCreateModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Type { get; set; }
        public bool Available { get; set; }
        [Required]
        public IFormFile Image { get; set; }
    }

    public class Campsite
    {
        [Key]
        public int CampsiteID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public bool Available { get; set; }
        public Uri ImageUrl { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

        public Campsite() {
            Reviews = new List<Review>();
        }
    }
}

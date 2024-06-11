using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BnBCampers.Models
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public User? User { get; set; }

        [ForeignKey("CampsiteID")]
        public int CampsiteID { get; set; }

        [ForeignKey("UserID")]
        public int UserID { get; set; }
    }
}

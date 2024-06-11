using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BnBCampers.Models
{
    public class BookingDetails
    {
        public int BookingID { get; set; }
        public int CampsiteID { get; set; }
        public int UserID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public UserDetails UserDetails { get; set; }
        public Campsite Campsite { get; set; }
    }

    public class Booking
    {
        [Key]
        public int BookingID { get; set; }
        
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public virtual Campsite? Campsite { get; set; }
        public virtual User? User { get; set; }

        [ForeignKey("CampsiteID")]
        public int CampsiteID { get; set; }
        [ForeignKey("UserID")]
        public int UserID { get; set; }

    }
}

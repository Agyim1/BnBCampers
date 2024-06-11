using System.ComponentModel.DataAnnotations;

namespace BnBCampers.Models
{
    public class Login
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class UserDetails
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class UserProfile
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<Booking> Bookings { get; set; }
    }

    public class UserPasswordReset
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }

    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public bool isAdmin { get; set; }

        public User()
        {
            Bookings = new List<Booking>();
            isAdmin = false;
        }
    }
}

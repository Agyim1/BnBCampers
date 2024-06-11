using BnBCampers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices.JavaScript;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/User/register
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(User user)
    {
        var tempUser = new User //create a temp user 
        {
            Username = user.Username,
            Email = user.Email,
            Password = user.Password 
        };
        _context.Users.Add(tempUser); //add user to the context
        await _context.SaveChangesAsync(); //save to the database
        return CreatedAtAction(nameof(GetUser), new { id = user.UserID }, new {User = user}); //return the newly created user
    }

    [HttpPatch("resetPassword/{id}")]
    public async Task<ActionResult<User>> ResetPassword([FromRoute]int id,[FromBody] UserPasswordReset userPasswords)
    {
        var user = await _context.Users
            .Where(u => u.UserID == id) //find the user with the ID
            .FirstOrDefaultAsync();

        if (user == null) //if user is not found
        {
            return NotFound();
        }

        if (userPasswords.oldPassword != user.Password) //if old password is not equal to user current password
        {
            return Unauthorized();
        }

        user.Password = userPasswords.newPassword; //change the password

        _context.SaveChanges(); 

        return Ok(new { Message = "Password Change successfully.", User = user }); //return the newly saved user
    }


    // POST: api/User/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]Login data)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == data.email && u.Password == data.password); //find the user with the email and password

        if (user == null) //if no user found 
        {
            return Unauthorized();
        }

        return Ok(new { Message = "Login successful", User = user }); //send the found user back to the frontend
    }

    //POST: api/user
    [HttpPost]
    public async Task<IActionResult> EditDetails(User user)
    {
        var userDetails = await _context.Users.Where(u => u.UserID == user.UserID).FirstOrDefaultAsync();

        if(userDetails == null)
        {
            return NotFound();
        }

        if(userDetails.Password != user.Password)
        {
            return Unauthorized();
        }

        userDetails.Username = user.Username;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Login successful", User = userDetails });
    }

    // GET: api/User/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserProfile>> GetUser(int id)
    {
        var user = await _context.Users
            .Include(u => u.Bookings) //include bookings
            .ThenInclude(b => b.Campsite) //include campsites of that bookings
            .Where(u => u.UserID == id) //find the user with id
            .Select(u => new UserProfile //select particular data to return
            {
                UserID = u.UserID,
                Username = u.Username,
                Email = u.Email,
                Bookings = u.Bookings.Select(b => new Booking
                {
                    BookingID = b.BookingID,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    Campsite = b.Campsite
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }

        return user; //return the data
    }
}

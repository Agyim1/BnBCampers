using BnBCampers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly ApplicationDbContext _context; //databse context for the ORM

    public BookingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/Bookings
    [HttpPost]
    public async Task<ActionResult<Booking>> CreateBooking(Booking bookingDetails)
    {
        var campsite = await _context.Campsites
            .Where(x => x.CampsiteID == bookingDetails.CampsiteID) //Find the campsite with particular id
            .FirstOrDefaultAsync();

        if (campsite == null)
        {
            return NotFound();
        }

        campsite.Available = false; //make the campsite availability to false

        _context.Bookings.Add(bookingDetails); //add new booking to the db context
        await _context.SaveChangesAsync(); //save the changes to database
        return CreatedAtAction("GetBooking", new { id = bookingDetails.BookingID }, bookingDetails); //return the newly created booking back to frontend
    }

    // GET: api/Bookings/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Booking>> GetBooking(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Campsite) // Include campsite details
            .Include(b => b.User)     // Include user details
            .FirstOrDefaultAsync(b => b.BookingID == id); // find the booking with specific id

        if (booking == null) //if booking is not found
        {
            return NotFound();
        }

        return booking; 
    }

    // DELETE: api/Bookings/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Campsite) //include campsites details
            .Where(b => b.BookingID == id) // find the specific booking with ID
            .FirstOrDefaultAsync();
    
        if (booking == null) //if booking is not found
        {
            return NotFound();
        }

        if (booking.Campsite != null) //if the campsite in the booking is not null
        {
            booking.Campsite.Available = true; //set the availability of the campsite to true
        }

        _context.Bookings.Remove(booking); //remove the booking from the db
        await _context.SaveChangesAsync();

        return NoContent(); //do not return anything just 200 Status code
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDetails>>> GetBookings()
    {
        return await _context.Bookings
            .Include(b => b.User) //includes users details
            .Include(b => b.Campsite) //includes campsite details
            .Select(b => new BookingDetails //select the booking details which want to add to the response
            {
                BookingID = b.BookingID,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                UserDetails = new UserDetails //select user details which want to add to the response
                {
                    Username = b.User.Username,
                    Email = b.User.Email
                },
                Campsite = new Campsite //select campsite details which wants to add to the response
                {
                    CampsiteID = b.Campsite.CampsiteID,
                    Name = b.Campsite.Name,
                    Description = b.Campsite.Description,
                    Location = b.Campsite.Location,
                    Price = b.Campsite.Price,
                    Type = b.Campsite.Type,
                    Available = b.Campsite.Available,
                    ImageUrl = b.Campsite.ImageUrl
                }
            })
            .ToListAsync(); //return the list to frontend.
    }

    
}

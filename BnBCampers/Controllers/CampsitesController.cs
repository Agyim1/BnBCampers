using BnBCampers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class CampsitesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public CampsitesController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment; //environment of the API service
    }

    // GET: api/Campsites
    // GET: api/Campsites
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Campsite>>> GetCampsites([FromQuery] string search = null)
    {
        IQueryable<Campsite> query = _context.Campsites.Include(c => c.Reviews);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c => c.Description.ToLower().Contains(search.ToLower()));
        }

        return await query.ToListAsync();
    }

    [HttpPost]
    public async Task<IActionResult> AddCampsite([FromForm] CampsiteCreateModel model)
    {
        if (!ModelState.IsValid) //if there is a missing data
        {
            return BadRequest(ModelState);
        }

        var filePath = Path.Combine(_environment.WebRootPath, "uploads", model.Image.FileName); //upload the file to the public folder with orignal filename
        using (var stream = new FileStream(filePath, FileMode.Create)) //open the file stream to save the file
        {
            await model.Image.CopyToAsync(stream); //copy the file from model (request) to stream
        }

        var campsite = new Campsite
        {
            Name = model.Name,
            Description = model.Description,
            Location = model.Location,
            Price = model.Price,
            Type = model.Type,
            Available = model.Available,
            ImageUrl = new Uri($"uploads/{model.Image.FileName}", UriKind.Relative) //change the path of the uploaded file
        };

        _context.Campsites.Add(campsite); //save to the database
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCampsite), new { id = campsite.CampsiteID }, campsite);
    }

    // GET: api/Campsites/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Campsite>> GetCampsite(int id)
    {
        var campsite = await _context.Campsites
        .Include(c => c.Reviews) //include reviews
        .ThenInclude(r => r.User) //include users from the reviews
        .FirstOrDefaultAsync(c => c.CampsiteID == id); //filter all campsite with the campsite ID

        if (campsite == null) //if campsite not found return null
        {
            return NotFound();
        }
        return campsite; //return campsite
    }

    // DELETE: api/Campsites/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCampsite(int id)
    {

        var campsite = _context.Campsites 
            .Include(c => c.Reviews) //include reviews
            .Where(c=>c.CampsiteID ==id) //get the campsite
            .FirstOrDefault();

        if(campsite == null) //if campsite not found
        {
            return NotFound();
        }

        var bookings = _context.Bookings
            .Where(b => b.CampsiteID == id) //get all bookings of the perticular campsite
            .ToList();

        if (bookings.Any()) //if booking found, delete its all bookings
        {
            _context.Bookings.RemoveRange(bookings);
        }

        if (campsite.Reviews.Any())
        {
            _context.Reviews.RemoveRange(campsite.Reviews); //remove all the reviews of the campsite
        }

        _context.Campsites.Remove(campsite); //remove campsite

        _context.SaveChanges(); //save the database to reflect the changes
        
        return NoContent(); //return 200 status code
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using Microsoft.AspNetCore.Authorization;
namespace RaceDay.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private object eventItem;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .AsNoTracking()
                .Select(e => new
                {
                    eventId = e.EventId,
                    name = e.Title,
                    date = e.EventDate,
                    location = e.Location,
                    description = e.Description
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/1
        [HttpGet("{eventId:int}")]
        public async Task<IActionResult> GetEvent(int eventId)
        {
            var eventItem = await _context.Events
                .AsNoTracking()
                .Where(e => e.EventId == eventId)
                .Select(e => new
                {
                    eventId = e.EventId,
                    name = e.Title,
                    date = e.EventDate,
                    location = e.Location,
                    description = e.Description
                })
                .FirstOrDefaultAsync();

            if (eventItem == null)
            {
                return NotFound();
            }

            return Ok(eventItem);
        }

        // POST: api/events

        [HttpPost]
        public async Task<IActionResult> CreateEvent(
            [FromBody] CreateEventRequest request)
        {
            var organiserExists = await _context.Organisers
      .AnyAsync(o => o.OrganiserID == request.OrganiserId);

            if (!organiserExists)
            {
                return BadRequest("The specified organiser does not exist.");
            }

            var eventItem = new RaceDay.API.Models.Event
            {
                OrganiserId = request.OrganiserId,
                Title = request.Name,
                EventDate = request.Date,
                Location = request.Location,
                Description = request.Description
            };

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { eventId = eventItem.EventId },
                new
                {
                    eventId = eventItem.EventId,
                    message = "Event created successfully."
                });
        }


        // PUT: api/events/1
        [HttpPut("{eventId:int}")]
        public async Task<IActionResult> UpdateEvent(
            int eventId,
            [FromBody] CreateEventRequest request)
        {
            // load the event from the database
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventItem == null)
            {
                return NotFound("Event not found.");
            }

            var organiserIdClaim = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(organiserIdClaim, out int loggedInOrganiserId))
            {
                return Unauthorized("Organiser ID is missing or invalid.");
            }

            if (eventItem.Organiserid != loggedInOrganiserId)
            {
                return Forbid();
            }

            eventItem.OrganiserId = request.OrganiserId;
            eventItem.Title = request.Name;
            eventItem.EventDate = request.Date;
            eventItem.Location = request.Location;
            eventItem.Description = request.Description;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                eventId = eventItem.EventId,
                message = "Event updated successfully."
            });
        }

        // DELETE: api/events/1

        [HttpDelete("{eventId:int}")]
        public async Task<IActionResult> DeleteEvent(int eventId)
        {
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventItem == null)
            {
                return NotFound("Event not found.");
            }

            var organiserIdClaim = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(organiserIdClaim, out int loggedInOrganiserId))
            {
                return Unauthorized("Organiser ID is missing or invalid.");
            }

            if (eventItem.OrganiserId != loggedInOrganiserId)
            {
                return Forbid();
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                eventId = eventId,
                message = "Event deleted successfully."
            });
        }
    }
}
    

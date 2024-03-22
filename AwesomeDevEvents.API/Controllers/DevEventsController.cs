using AwesomeDevEvents.API.Context;
using AwesomeDevEvents.API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AwesomeDevEvents.API.Controllers
{
    [Route("api/dev-events")]
    [ApiController]
    public class DevEventsController : ControllerBase
    {
        private readonly DevEventsDBContext _dbContext;

        public DevEventsController(DevEventsDBContext dbContext) 
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var devEvent = _dbContext.DevEvents.Include(e => e.Speakers).Where(d => d.IsDeleted == false).ToList();
            return Ok(devEvent);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var devEvent = _dbContext.DevEvents.Include(e => e.Speakers).SingleOrDefault(d => d.Id == id);

            if (devEvent == null) return NotFound();

            return Ok(devEvent);
        }

        [HttpPost]
        public IActionResult Insert(DevEvent devEvent)
        {
            _dbContext.DevEvents.Add(devEvent);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = devEvent.Id }, devEvent);
        }

        [HttpPut("{id}")]
        public IActionResult Updade(Guid id, DevEvent devEventImput)
        {
            var devEvent = _dbContext.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null) return NotFound();

            devEvent.Update(devEventImput.Title, devEventImput.Description, devEventImput.StartDate, devEventImput.EndDate);

            _dbContext.DevEvents.Update(devEvent);
            _dbContext.SaveChanges();
            return NoContent();


        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var devEvent = _dbContext.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null) return NotFound();

            devEvent.Delete();
            _dbContext.SaveChanges();
            return NoContent();
        }

        [HttpPost("{id}/speakers")]
        public IActionResult InsertSpeaker(Guid id, DevEventSpeaker devEventSpeaker)
        {
            devEventSpeaker.DevEventId = id;

            var devEvent = _dbContext.DevEvents.Any(d => d.Id == id);

            if (!devEvent) return NotFound();


            _dbContext.devEventSpeakers.Add(devEventSpeaker);
            _dbContext.SaveChanges();

            return NoContent();
        }


    }
}

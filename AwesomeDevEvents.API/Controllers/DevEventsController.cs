using AutoMapper;
using AwesomeDevEvents.API.Context;
using AwesomeDevEvents.API.Entities;
using AwesomeDevEvents.API.Models;
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
        private readonly IMapper _mapper;

        public DevEventsController(
            DevEventsDBContext dbContext,
            IMapper mapper) 
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Obter todos os eventos
        /// </summary>
        /// <returns>Coleção de eventos </returns>
        /// <response code="200">Sucesso</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var devEvents = _dbContext.DevEvents.Include(e => e.Speakers).Where(d => d.IsDeleted == false).ToList();

            var viewModel = _mapper.Map<List<DevEventViewModel>>(devEvents);
            return Ok(viewModel);
        }

        /// <summary>
        /// Obter um evento
        /// </summary>
        /// <param name="id">Identificados do evento</param>
        /// <returns>Coleção de eventos </returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Não Encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var devEvent = _dbContext.DevEvents.Include(e => e.Speakers).SingleOrDefault(d => d.Id == id);

            if (devEvent == null) return NotFound();

            var viewModel = _mapper.Map<DevEventViewModel>(devEvent);

            return Ok(viewModel);
        }

        /// <summary>
        /// Cadastrar um evento
        /// </summary>
        /// <remarks>
        /// {"title":"string","description":"string","startDate":"2023-02-27T17:59:14.141Z","endDate":"2023-02-27T17:59:14.141Z"}
        /// </remarks>
        /// <param name="devEventInput">dados do evento</param>
        /// <returns>Objeto recém-criado</returns>
        /// <response code="201">Sucesso</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Insert(DevEventInputModel devEventInput)
        {
            var devEvent = _mapper.Map<DevEvent>(devEventInput);

            _dbContext.DevEvents.Add(devEvent);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = devEvent.Id }, devEvent);
        }

        /// <summary>
        /// Atualizar um evento
        /// </summary>
        /// <remarks>
        /// {"title":"string","description":"string","startDate":"2023-02-27T17:59:14.141Z","endDate":"2023-02-27T17:59:14.141Z"}
        /// </remarks>
        /// <param name="id">Identificador do evento</param>
        /// <param name="devEventImput">dados do evento</param>
        /// <returns>Nada.</returns>
        /// <response Code="404">Não Encontrado.</response>
        /// <response Code="204">Sucesso</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Updade(Guid id, DevEventInputModel devEventImput)
        {
            var devEvent = _dbContext.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null) return NotFound();

            devEvent.Update(devEventImput.Title, devEventImput.Description, devEventImput.StartDate, devEventImput.EndDate);

            _dbContext.DevEvents.Update(devEvent);
            _dbContext.SaveChanges();
            return NoContent();


        }
        /// <summary>
        /// Deletar um evento
        /// </summary>
        /// <param name="id">identificador do evento</param>
        /// <returns>Nada</returns>
        /// <response code="404">Não encontrado</response>
        /// <response code="204">Sucesso</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Delete(Guid id)
        {
            var devEvent = _dbContext.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null) return NotFound();

            devEvent.Delete();
            _dbContext.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// Cadastrar um palestrante
        /// </summary>
        /// <remarks>
        /// {"name":"string","talkTitle":"string","talkDescription":"string","linkedInProfile":"string"}
        /// </remarks>
        /// <param name="id">Id do Evento</param>
        /// <param name="devEventSpeakerInput">Dados do palestrante</param>
        /// <returns>Nada</returns>
        /// <response code="404">Não Encontrado.</response>
        /// <response code="204">Sucesso</response>   
        [HttpPost("{id}/speakers")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult InsertSpeaker(Guid id, DevEventSpeakerInputModel devEventSpeakerInput)
        {
            var devEventSpeaker = _mapper.Map<DevEventSpeaker>(devEventSpeakerInput);

            devEventSpeaker.DevEventId = id;

            var devEvent = _dbContext.DevEvents.Any(d => d.Id == id);

            if (!devEvent) return NotFound();


            _dbContext.devEventSpeakers.Add(devEventSpeaker);
            _dbContext.SaveChanges();

            return NoContent();
        }


    }
}

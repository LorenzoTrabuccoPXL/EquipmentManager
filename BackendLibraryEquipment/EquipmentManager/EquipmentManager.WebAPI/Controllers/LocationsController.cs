using EquipmentManager.Application.Services;
using EquipmentManager.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManager.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly LocationsService _locationsService;

        public LocationsController(LocationsService locationsService)
        {
            _locationsService = locationsService;
        }

        // GET: api/<LocationsController>
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _locationsService.GetAllAsync());
        }

        // GET api/<LocationsController>/5
        [HttpGet("GetById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Toegevoegd: voor als het ID ongeldig is (bijv. een negatief getal)
        [ProducesResponseType(StatusCodes.Status404NotFound)]   // Toegevoegd: voor als het ID niet bestaat in de database
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _locationsService.GetByIdAsync(id));
        }

        // POST api/<LocationsController>
        [HttpPost("Add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add(Locations location)
        {
            await _locationsService.AddAsync(location);

            return Ok();
        }

        // PUT api/<LocationsController>/5
        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Locations location)
        {
            await _locationsService.UpdateAsync(location);

            return Ok();
        }

        // DELETE api/<LocationsController>/5
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            await _locationsService.DeleteAsync(id);

            return Ok();
        }
    }
}

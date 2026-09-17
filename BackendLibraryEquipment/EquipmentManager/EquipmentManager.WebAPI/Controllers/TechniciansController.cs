using EquipmentManager.Application.Services;
using EquipmentManager.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManager.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TechniciansController : ControllerBase
    {
        private readonly TechniciansService _techniciansService;

        public TechniciansController(TechniciansService techniciansService)
        {
            _techniciansService = techniciansService;
        }

        // GET: api/<TechniciansController>
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _techniciansService.GetAllAsync());
        }

        // GET api/<TechniciansController>/5
        [HttpGet("GetById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Toegevoegd: voor als het ID ongeldig is (bijv. een negatief getal)
        [ProducesResponseType(StatusCodes.Status404NotFound)]   // Toegevoegd: voor als het ID niet bestaat in de database
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _techniciansService.GetByIdAsync(id));
        }

        // POST api/<TechniciansController>
        [HttpPost("Add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add(Technicians technicians)
        {
            await _techniciansService.AddAsync(technicians);

            return Ok();
        }

        // PUT api/<TechniciansController>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Technicians technicians)
        {
            await _techniciansService.UpdateAsync(technicians);

            return Ok();
        }

        // DELETE api/<TechniciansController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            await _techniciansService.DeleteAsync(id);

            return Ok();
        }
    }
}

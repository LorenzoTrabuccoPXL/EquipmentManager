using EquipmentManager.Application.Services;
using EquipmentManager.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentManager.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceRecordsController : ControllerBase
        {
            private readonly MaintenanceRecordsService _maintenanceRecordsService;

            public MaintenanceRecordsController(MaintenanceRecordsService maintenanceRecordsService)
            {
                _maintenanceRecordsService = maintenanceRecordsService;
            }

            // GET: api/<EquipmentController>
            [HttpGet("GetAll")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> GetAll()
            {
                return Ok(await _maintenanceRecordsService.GetAllAsync());
            }

            // GET api/<MaintenanceRecordsController>/5
            [HttpGet("GetById/{id}")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)] // Toegevoegd: voor als het ID ongeldig is (bijv. een negatief getal)
            [ProducesResponseType(StatusCodes.Status404NotFound)]   // Toegevoegd: voor als het ID niet bestaat in de database
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> GetById(int id)
            {
                return Ok(await _maintenanceRecordsService.GetByIdAsync(id));
            }

            // POST api/<MaintenanceRecordsController>
            [HttpPost("Add")]
            [ProducesResponseType(StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status409Conflict)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> Add(MaintenanceRecords maintenanceRecord)
            {
                await _maintenanceRecordsService.AddAsync(maintenanceRecord);

                return Ok();
            }

            // PUT api/<MaintenanceRecordsController>/5
            [HttpPut("Update")]
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> Update(MaintenanceRecords maintenanceRecord)
            {
                await _maintenanceRecordsService.UpdateAsync(maintenanceRecord);

                return Ok();
            }

            // DELETE api/<MaintenanceRecordsController>/5
            [HttpDelete("Delete/{id}")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> Delete(int id)
            {
                await _maintenanceRecordsService.DeleteAsync(id);

                return Ok();
            }
    }
}

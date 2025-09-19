using ApexGym.Application.Dtos;
using ApexGym.Application.Interfaces.Repositories;
using ApexGym.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApexGym.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TrainersController : ControllerBase
    {
        private readonly ITrainerRepository _trainerRepository;
        private readonly IMapper _mapper;

        public TrainersController(ITrainerRepository trainerRepository, IMapper mapper)
        {
            _trainerRepository = trainerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Trainer>>> GetTrainers()
        {
            var trainers = await _trainerRepository.GetAllAsync();
            return Ok(trainers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Trainer>> GetTrainer(int id)
        {
            var trainer = await _trainerRepository.GetByIdAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }
            return Ok(trainer);
        }

        // GET: api/trainers/5/details  <-- Different route
        [HttpGet("{id}/details")] // This creates api/trainers/5/details
        public async Task<ActionResult<GetTrainerDto>> GetTrainerWithDetails(int id) // Renamed to avoid confusion
        {
            var trainer = await _trainerRepository.GetTrainerWithDetailsAsync(id);
            var target= _mapper.Map<GetTrainerDto>(trainer);
            if (target == null)
            {
                return NotFound();
            }
            return Ok(target);
        }
        // POST: api/trainers
        [HttpPost] // [16] Maps HTTP POST requests to this method
        [Authorize(Roles = "Admin")] // [17] EXTRA PROTECTION: Only users with "Admin" role can access this
        public async Task<ActionResult<Trainer>> PostTrainer(TrainerCreateDto trainerCreateDto)
        {
            // [18] BUSINESS LOGIC: Use AutoMapper to convert the DTO to a Trainer entity
            // This automatically copies FirstName, LastName, etc. from the DTO to the new Trainer object
            var trainer = _mapper.Map<Trainer>(trainerCreateDto);

            // [19] DATA ACCESS: Add the new trainer to the database
            var createdTrainer = await _trainerRepository.AddAsync(trainer);

            // [20] RETURN: HTTP 201 Created status with:
            // - A Location header pointing to the new resource (https://localhost:7164/api/trainers/10)
            // - The complete created trainer object in the response body
            return CreatedAtAction(nameof(GetTrainer), new { id = createdTrainer.Id }, createdTrainer);
        }


        // PUT: api/trainers/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutTrainer(int id, TrainerUpdateDto trainerUpdateDto)
        {
            // [21] VALIDATION: First, check if the trainer we're trying to update exists
            var existingTrainer = await _trainerRepository.GetByIdAsync(id);
            if (existingTrainer == null)
            {
                return NotFound();
            }

            // [22] BUSINESS LOGIC: Use AutoMapper to apply updates from the DTO to the existing entity
            // This only updates properties that are provided (not null) in the DTO
            _mapper.Map(trainerUpdateDto, existingTrainer);

            // [23] DATA ACCESS: Save the updated entity back to the database
            await _trainerRepository.UpdateAsync(existingTrainer);

            // [24] RETURN: HTTP 204 No Content (standard response for successful PUT requests)
            return NoContent();
        }

        // DELETE: api/trainers/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTrainer(int id)
        {
            // [25] VALIDATION: Check if the trainer exists
            var trainer = await _trainerRepository.GetByIdAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            // [26] DATA ACCESS: Delete the trainer from the database
            await _trainerRepository.DeleteAsync(trainer);

            // [27] RETURN: HTTP 204 No Content (standard response for successful DELETE requests)
            return NoContent();
        }
    }
}

using ApexGym.Application.Dtos;
using ApexGym.Application.Interfaces;
using ApexGym.Application.Interfaces.Repositories;
using ApexGym.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApexGym.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkoutClassesController : ControllerBase
    {
        private readonly IWorkoutClassRepository _workoutClassRepository;
        private readonly IMapper _mapper;

        public WorkoutClassesController(IWorkoutClassRepository workoutClassRepository, IMapper mapper)
        {
            _workoutClassRepository = workoutClassRepository;
            _mapper = mapper;
        }

        // GET: api/workoutclasses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkoutClassDto>>> GetWorkoutClasses()
        {
            var classes = await _workoutClassRepository.GetAllAsync();
            var target = _mapper.Map<List<WorkoutClassDto>>(classes);
            return Ok(target);
        }

        // GET: api/workoutclasses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkoutClass>> GetWorkoutClass(int id)
        {
            // Use the special method to get class with trainer details
            var workoutClass = await _workoutClassRepository.GetByIdWithDetailsAsync(id);

            if (workoutClass == null)
            {
                return NotFound();
            }

            return Ok(workoutClass);
        }

        // POST: api/workoutclasses
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<WorkoutClass>> PostWorkoutClass(WorkoutClassCreateDto workoutClassCreateDto)
        {
            // Validate that end time is after start time
            if (workoutClassCreateDto.EndTime <= workoutClassCreateDto.StartTime)
            {
                return BadRequest("End time must be after start time.");
            }

            var workoutClass = _mapper.Map<WorkoutClass>(workoutClassCreateDto);
            var createdClass = await _workoutClassRepository.AddAsync(workoutClass);

            return CreatedAtAction(nameof(GetWorkoutClass), new { id = createdClass.Id }, createdClass);
        }

        // PUT: api/workoutclasses/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutWorkoutClass(int id, WorkoutClassUpdateDto workoutClassUpdateDto)
        {
            var existingClass = await _workoutClassRepository.GetByIdAsync(id);
            if (existingClass == null)
            {
                return NotFound();
            }

            // Validate that end time is after start time if both are provided
            if (workoutClassUpdateDto.EndTime.HasValue && workoutClassUpdateDto.StartTime.HasValue &&
                workoutClassUpdateDto.EndTime <= workoutClassUpdateDto.StartTime)
            {
                return BadRequest("End time must be after start time.");
            }

            _mapper.Map(workoutClassUpdateDto, existingClass);
            await _workoutClassRepository.UpdateAsync(existingClass);

            return NoContent();
        }

        // DELETE: api/workoutclasses/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteWorkoutClass(int id)
        {
            var workoutClass = await _workoutClassRepository.GetByIdAsync(id);
            if (workoutClass == null)
            {
                return NotFound();
            }

            await _workoutClassRepository.DeleteAsync(workoutClass);

            return NoContent();
        }
    }
}
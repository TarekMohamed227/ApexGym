using ApexGym.Application.Dtos;
using ApexGym.Application.Interfaces;
using ApexGym.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace ApexGym.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkoutClassesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        // SIMPLIFIED: Only 2 dependencies!
        public WorkoutClassesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkoutClassDto>>> GetWorkoutClasses()
        {
            var classes = await _unitOfWork.WorkoutClasses.GetAllAsync();
            var target = _mapper.Map<List<WorkoutClassDto>>(classes);
            return Ok(target);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkoutClassDto>> GetWorkoutClass(int id)
        {
            var workoutClass = await _unitOfWork.WorkoutClasses.GetByIdAsync(id);
            if (workoutClass == null)
            {
                return NotFound();
            }

            var target = _mapper.Map<WorkoutClassDto>(workoutClass);
            return Ok(target);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<WorkoutClassDto>> GetWorkoutClassWithDetails(int id)
        {
            // Use SPECIFIC repository for custom method with includes
            var workoutClass = await _unitOfWork.WorkoutClassRepository.GetByIdWithDetailsAsync(id);
            if (workoutClass == null)
            {
                return NotFound();
            }

            var target = _mapper.Map<WorkoutClassDto>(workoutClass);
            return Ok(target);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<WorkoutClassDto>> PostWorkoutClass(WorkoutClassCreateDto workoutClassCreateDto, CancellationToken cancellationToken)
        {
            if (workoutClassCreateDto.EndTime <= workoutClassCreateDto.StartTime)
            {
                return BadRequest("End time must be after start time.");
            }

            var workoutClass = _mapper.Map<WorkoutClass>(workoutClassCreateDto);
            var createdClass = await _unitOfWork.WorkoutClasses.AddAsync(workoutClass, cancellationToken);

            // SAVE THE CHANGES
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0)
            {
                return BadRequest("Failed to create workout class.");
            }

            var createdClassDto = _mapper.Map<WorkoutClassDto>(createdClass);
            return CreatedAtAction(nameof(GetWorkoutClass), new { id = createdClassDto.Id }, createdClassDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutWorkoutClass(int id, WorkoutClassUpdateDto workoutClassUpdateDto)
        {
            var existingClass = await _unitOfWork.WorkoutClasses.GetByIdAsync(id);
            if (existingClass == null)
            {
                return NotFound();
            }

            if (workoutClassUpdateDto.EndTime.HasValue && workoutClassUpdateDto.StartTime.HasValue &&
                workoutClassUpdateDto.EndTime <= workoutClassUpdateDto.StartTime)
            {
                return BadRequest("End time must be after start time.");
            }

            _mapper.Map(workoutClassUpdateDto, existingClass);
            await _unitOfWork.WorkoutClasses.UpdateAsync(existingClass);

            // SAVE THE CHANGES
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0)
            {
                return BadRequest("Failed to update workout class.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteWorkoutClass(int id)
        {
            var workoutClass = await _unitOfWork.WorkoutClasses.GetByIdAsync(id);
            if (workoutClass == null)
            {
                return NotFound();
            }

            await _unitOfWork.WorkoutClasses.DeleteAsync(workoutClass);

            // SAVE THE CHANGES
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0)
            {
                return BadRequest("Failed to delete workout class.");
            }

            return NoContent();
        }
    }
}
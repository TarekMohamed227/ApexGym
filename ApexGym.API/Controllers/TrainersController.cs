using ApexGym.Application.Dtos;
using ApexGym.Application.Interfaces;
using ApexGym.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApexGym.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TrainersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        // SIMPLIFIED: Only 2 dependencies!
        public TrainersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrainerGetDto>>> GetTrainers() // Changed to return DTO
        {
            var trainers = await _unitOfWork.Trainers.GetAllAsync();
            var trainerDtos = _mapper.Map<List<Trainer>>(trainers); // Map to DTO
            return Ok(trainerDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Trainer>> GetTrainer(int id) // Changed to return DTO
        {
            var trainer = await _unitOfWork.Trainers.GetByIdAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            var trainerDto = _mapper.Map<Trainer>(trainer); // Map to DTO
            return Ok(trainerDto);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<GetTrainerDto>> GetTrainerWithDetails(int id)
        {
            // Use SPECIFIC repository for custom method with includes
            var trainer = await _unitOfWork.TrainerRepository.GetTrainerWithDetailsAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            var trainerDto = _mapper.Map<GetTrainerDto>(trainer);
            return Ok(trainerDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Trainer>> PostTrainer(TrainerCreateDto trainerCreateDto) // Changed to return DTO
        {
            var trainer = _mapper.Map<Trainer>(trainerCreateDto);
            var createdTrainer = await _unitOfWork.Trainers.AddAsync(trainer);

            // SAVE THE CHANGES
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0)
            {
                return BadRequest("Failed to create trainer.");
            }

            var trainerDto = _mapper.Map<Trainer>(createdTrainer); // Map to DTO
            return CreatedAtAction(nameof(GetTrainer), new { id = trainerDto.Id }, trainerDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutTrainer(int id, TrainerUpdateDto trainerUpdateDto)
        {
            var existingTrainer = await _unitOfWork.Trainers.GetByIdAsync(id);
            if (existingTrainer == null)
            {
                return NotFound();
            }

            _mapper.Map(trainerUpdateDto, existingTrainer);
            await _unitOfWork.Trainers.UpdateAsync(existingTrainer);

            // SAVE THE CHANGES
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0)
            {
                return BadRequest("Failed to update trainer.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTrainer(int id)
        {
            var trainer = await _unitOfWork.Trainers.GetByIdAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            await _unitOfWork.Trainers.DeleteAsync(trainer);

            // SAVE THE CHANGES
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0)
            {
                return BadRequest("Failed to delete trainer.");
            }

            return NoContent();
        }
    }
}
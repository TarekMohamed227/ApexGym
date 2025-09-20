using ApexGym.Application.Dtos;
using ApexGym.Application.Dtos.Validators;
using ApexGym.Application.Interfaces;
using ApexGym.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApexGym.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MembersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembers()
        {
            // Get user info from claims for business logic
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var isAdmin = User.IsInRole("Admin");

            // Use Unit of Work to access Member repository
            var members = await _unitOfWork.Members.GetAllAsync();
            var memberDtos = _mapper.Map<List<MemberDto>>(members);
            return Ok(memberDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MemberDto>> GetMember(int id)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            var memberDto = _mapper.Map<MemberDto>(member);
            return Ok(memberDto);
        }

        [HttpPost]
        public async Task<ActionResult<MemberDto>> PostMember(MemberCreateDto memberCreateDto)
        {
            // Use the specific repository for custom methods - NO CASTING NEEDED!
            if (!await _unitOfWork.MemberRepository.IsEmailUniqueAsync(memberCreateDto.Email))
            {
                return BadRequest("Email is already in use.");
            }

            var member = _mapper.Map<Member>(memberCreateDto);
            var createdMember = await _unitOfWork.Members.AddAsync(member);

            // SAVE THE CHANGES
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0)
            {
                return BadRequest("Failed to create member.");
            }

            var createdMemberDto = _mapper.Map<MemberDto>(createdMember);
            return CreatedAtAction(nameof(GetMember), new { id = createdMemberDto.Id }, createdMemberDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, MemberUpdateDto memberUpdateDto)
        {
            // Validation with FluentValidation
            var validator = new MemberUpdateDtoValidator();
            var validationResult = await validator.ValidateAsync(memberUpdateDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
            }

            var existingMember = await _unitOfWork.Members.GetByIdAsync(id);
            if (existingMember == null)
            {
                return NotFound();
            }

            // Use the specific repository for custom methods - NO CASTING NEEDED!
            if (memberUpdateDto.Email != null &&
                memberUpdateDto.Email != existingMember.Email &&
                !await _unitOfWork.MemberRepository.IsEmailUniqueAsync(memberUpdateDto.Email))
            {
                return BadRequest("Email is already in use.");
            }

            _mapper.Map(memberUpdateDto, existingMember);
            await _unitOfWork.Members.UpdateAsync(existingMember);

            // SAVE THE CHANGES
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0)
            {
                return BadRequest("Failed to update member.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            await _unitOfWork.Members.DeleteAsync(member);

            // SAVE THE CHANGES
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0)
            {
                return BadRequest("Failed to delete member.");
            }

            return NoContent();
        }
    }
}
using ApexGym.Application.Dtos;
using ApexGym.Application.Dtos.Validators;
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
    public class MembersController : ControllerBase
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IMapper _mapper;
        public MembersController(IMemberRepository memberRepository, IMapper mapper)
        {
            _memberRepository = memberRepository;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            var members = await _memberRepository.GetAllAsync();
            return Ok(members);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            return Ok(member);
        }


        // POST: api/members
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
            // Check if the email is already taken (Business Rule)
            if (!await _memberRepository.IsEmailUniqueAsync(member.Email))
            {
                return BadRequest("Email is already in use.");
            }

            // The RegistrationDate should be set to UTC Now, not provided by the user.
            member.RegistrationDate = DateTime.UtcNow;

            var createdMember = await _memberRepository.AddAsync(member);

            // Returns a 201 Created response, with a Location header pointing to the new resource
            return CreatedAtAction(nameof(GetMember), new { id = createdMember.Id }, createdMember);
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, MemberUpdateDto memberUpdateDto)
        {
            // === NEW VALIDATION CODE ===
            // Create an instance of the validator
            var validator = new MemberUpdateDtoValidator();
            // Perform the validation
            var validationResult = await validator.ValidateAsync(memberUpdateDto);

            // If validation fails, return a 400 Bad Request with the error messages
            if (!validationResult.IsValid)
            {
                // This returns a 400 status code and a list of errors
                return BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
            }

            var existingMember = await _memberRepository.GetByIdAsync(id);
            if (existingMember == null)
            {
                return NotFound();
            }

            // Check email uniqueness only if a new email is provided AND it's different from the old one
            if (memberUpdateDto.Email != null &&
                memberUpdateDto.Email != existingMember.Email &&
                !await _memberRepository.IsEmailUniqueAsync(memberUpdateDto.Email))
            {
                return BadRequest("Email is already in use.");
            }

            // Use AutoMapper to update the tracked entity from the DTO
            _mapper.Map(memberUpdateDto, existingMember);

            await _memberRepository.UpdateAsync(existingMember);

            return NoContent();
        }


        // DELETE: api/members/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            await _memberRepository.DeleteAsync(member);

            return NoContent(); // Standard response for a successful DELETE request
        }
    }
}

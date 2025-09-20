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
    public class AttendanceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        // SIMPLIFIED: Only 2 dependencies instead of 3!
        public AttendanceController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/attendance
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetAttendances()
        {
            // Use Unit of Work to access Attendance repository
            var attendances = await _unitOfWork.Attendances.GetAllAsync();
            var attendanceDtos = _mapper.Map<List<AttendanceDto>>(attendances);
            return Ok(attendanceDtos);
        }

        // GET: api/attendance/5/10
        [HttpGet("{memberId}/{classId}")]
        public async Task<ActionResult<AttendanceDto>> GetAttendance(int memberId, int classId)
        {
            // Use Unit of Work to access Attendance repository
            var attendance = await _unitOfWork.Attendances.GetByIdAsync(memberId, classId);
            if (attendance == null)
            {
                return NotFound();
            }

            var attendanceDto = _mapper.Map<AttendanceDto>(attendance);
            return Ok(attendanceDto);
        }

        // GET: api/attendance/member/5
        [HttpGet("member/{memberId}")]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetMemberAttendances(int memberId)
        {
            // Use Unit of Work to access Attendance repository
            var attendances = await _unitOfWork.Attendances.GetAttendancesByMemberAsync(memberId);
            var attendanceDtos = _mapper.Map<List<AttendanceDto>>(attendances);
            return Ok(attendanceDtos);
        }

        // GET: api/attendance/class/5
        [HttpGet("class/{classId}")]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetClassAttendances(int classId)
        {
            // Use Unit of Work to access Attendance repository
            var attendances = await _unitOfWork.Attendances.GetAttendancesByClassAsync(classId);
            var attendanceDtos = _mapper.Map<List<AttendanceDto>>(attendances);
            return Ok(attendanceDtos);
        }

        // POST: api/attendance
        [HttpPost]
        public async Task<ActionResult<AttendanceDto>> PostAttendance(AttendanceCreateDto attendanceCreateDto)
        {
            // Check if member is already registered for this class
            if (await _unitOfWork.Attendances.IsMemberRegisteredAsync(
                attendanceCreateDto.MemberId, attendanceCreateDto.WorkoutClassId))
            {
                return BadRequest("Member is already registered for this class.");
            }

            // Check if class has reached maximum capacity
            var attendanceCount = await _unitOfWork.Attendances.GetAttendanceCountForClassAsync(
                attendanceCreateDto.WorkoutClassId);

            // Use Unit of Work to access WorkoutClass repository
            var workoutClass = await _unitOfWork.WorkoutClasses.GetByIdAsync(attendanceCreateDto.WorkoutClassId);

            if (workoutClass == null)
            {
                return NotFound("Workout class not found.");
            }

            if (attendanceCount >= workoutClass.MaxCapacity)
            {
                return BadRequest("This class has reached maximum capacity.");
            }

            var attendance = _mapper.Map<Attendance>(attendanceCreateDto);

            // Use Unit of Work to access Attendance repository
            var createdAttendance = await _unitOfWork.Attendances.AddAsync(attendance);

            // SAVE ALL CHANGES TOGETHER
            var result = await _unitOfWork.CompleteAsync();

            if (result == 0)
            {
                return BadRequest("Failed to save attendance.");
            }

            var attendanceDto = _mapper.Map<AttendanceDto>(createdAttendance);

            return CreatedAtAction(nameof(GetAttendance), new
            {
                memberId = attendanceDto.MemberId,
                classId = attendanceDto.WorkoutClassId
            }, attendanceDto);
        }

        // PUT: api/attendance/5/10
        [HttpPut("{memberId}/{classId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutAttendance(int memberId, int classId, AttendanceUpdateDto attendanceUpdateDto)
        {
            // Use Unit of Work to access Attendance repository
            var attendance = await _unitOfWork.Attendances.GetByIdAsync(memberId, classId);
            if (attendance == null)
            {
                return NotFound();
            }

            _mapper.Map(attendanceUpdateDto, attendance);

            // Use Unit of Work to access Attendance repository
            await _unitOfWork.Attendances.UpdateAsync(attendance);

            // SAVE THE CHANGE
            var result = await _unitOfWork.CompleteAsync();

            if (result == 0)
            {
                return BadRequest("Failed to update attendance.");
            }

            return NoContent();
        }

        // DELETE: api/attendance/5/10
        [HttpDelete("{memberId}/{classId}")]
        public async Task<IActionResult> DeleteAttendance(int memberId, int classId)
        {
            // Use Unit of Work to access Attendance repository
            var attendance = await _unitOfWork.Attendances.GetByIdAsync(memberId, classId);
            if (attendance == null)
            {
                return NotFound();
            }

            // Use Unit of Work to access Attendance repository
            await _unitOfWork.Attendances.DeleteAsync(attendance);

            // SAVE THE CHANGE
            var result = await _unitOfWork.CompleteAsync();

            if (result == 0)
            {
                return BadRequest("Failed to delete attendance.");
            }

            return NoContent();
        }
    }
}
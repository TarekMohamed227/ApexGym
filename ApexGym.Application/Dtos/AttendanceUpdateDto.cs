using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Application.Dtos
{
    public class AttendanceUpdateDto
    {
        public bool Attended { get; set; } // Only allow updating attendance status
    }
}

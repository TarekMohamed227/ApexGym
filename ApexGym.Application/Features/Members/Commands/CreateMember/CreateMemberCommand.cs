using ApexGym.Application.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Application.Features.Members.Commands.CreateMember
{
    public record CreateMemberCommand(MemberCreateDto MemberCreateDto):IRequest<MemberCreateDto>
    {
       
    }
}

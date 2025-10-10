using ApexGym.Application.Dtos;
using ApexGym.Application.Interfaces;
using ApexGym.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexGym.Application.Features.Members.Commands.CreateMember
{
    public class CreateMemberCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateMemberCommand, MemberCreateDto>
    {
        public async Task<MemberCreateDto> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
        {
            var memberEntity = mapper.Map<Member>(request.MemberCreateDto);
            var member = await unitOfWork.Members.AddAsync(memberEntity, cancellationToken);
            var memberDto = mapper.Map<MemberCreateDto>(member);
            return memberDto;
        }
    }
}

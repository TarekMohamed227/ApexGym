using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexGym.Application.Dtos;
using ApexGym.Domain.Entities;
using AutoMapper;

namespace ApexGym.Application.Mappings
{
    public class MemberProfile : Profile
    {
        public MemberProfile()
        {
            // This is the simplest mapping configuration.
            // It tells AutoMapper: "You can map from a Member object to another Member object."
            // It will automatically copy all properties with matching names and types.
            CreateMap<Member, Member>();

            // We will add more mappings here in the future, for example:
            // CreateMap<CreateMemberDto, Member>();
            // CreateMap<Member, MemberResponseDto>();

            CreateMap<MemberUpdateDto, Member>()
           .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}

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

            CreateMap<WorkoutClass, WorkoutClassDto>();
            CreateMap<Trainer, GetTrainerDto>();

            // We will add more mappings here in the future, for example:
            // CreateMap<CreateMemberDto, Member>();
            // CreateMap<Member, MemberResponseDto>();

            CreateMap<MemberUpdateDto, Member>()
           .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<MemberCreateDto, Member>()
           .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => DateTime.UtcNow)) // Custom rule
           .ForMember(dest => dest.MembershipPlan, opt => opt.Ignore()); // Ignore what we don't need



            // Inside MemberProfile.cs, add these new mappings to the constructor
            CreateMap<TrainerCreateDto, Trainer>();
            CreateMap<TrainerUpdateDto, Trainer>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<WorkoutClassCreateDto, WorkoutClass>();
            CreateMap<WorkoutClassUpdateDto, WorkoutClass>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}

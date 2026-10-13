using AutoMapper;
using Partners.Application;
using Partners.Domain;

namespace Partners.Aplication.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            SourceMemberNamingConvention = PascalCaseNamingConvention.Instance;
            DestinationMemberNamingConvention = PascalCaseNamingConvention.Instance;

            ConfigurePartnerMappings();
        }

        private void ConfigurePartnerMappings()
        {
            CreateMap<PartnerDetailsDto, Partner>().ReverseMap()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            CreateMap<Partner, PartnerDetailsDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<PartnerType>(src.Type, true)));

            CreateMap<CreatePartnerDto, Partner>().ReverseMap()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            CreateMap<Partner, CreatePartnerDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<PartnerType>(src.Type, true)));

            CreateMap<UpdatePartnerDto, Partner>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            CreateMap<Partner, UpdatePartnerDto>()
             .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<PartnerType>(src.Type, true)));
        }
    }
}

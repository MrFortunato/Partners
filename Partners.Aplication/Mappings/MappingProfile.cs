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
            CreateMap<PartnerDetailsDto, Partner>().ReverseMap();
            CreateMap<CreatePartnerDto, Partner>().ReverseMap();
            CreateMap<UpdatePartnerDto, Partner>().ReverseMap();
        }
    }
}

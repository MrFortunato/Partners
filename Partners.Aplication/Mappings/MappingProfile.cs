using AutoMapper;
using Partners.Application;
using Partners.Domain;

namespace Partners.Aplication.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ConfigureNamingConventions();
            CreatePartnerMappingg();
        }
        private void ConfigureNamingConventions()
        {
            SourceMemberNamingConvention = LowerUnderscoreNamingConvention.Instance;
            DestinationMemberNamingConvention = PascalCaseNamingConvention.Instance;
        }

        private void CreatePartnerMappingg()
        {
            CreateMap<PartnerDetailsDto, Partner>().ReverseMap();
            CreateMap<CreatePartnerDto, Partner>().ReverseMap();
            CreateMap<UpdatePartnerDto, Partner>().ReverseMap();
        }
    }
}

using AutoMapper;
using MyCRM.DAL.DataModel;
using MyCRM.Model;

namespace MyCRM.Repository.Automapper
{
    public class RepositoryMappingService : IRepositoryMappingService
    {
        public IMapper mapper { get; private set; }

        public RepositoryMappingService()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PisUsersDResetar, UsersDomain>()
                    .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.uId))
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.firstName))
                    .ForMember(dest => dest.UserSurname, opt => opt.MapFrom(src => src.lastName))
                    .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.email))
                    .ForMember(dest => dest.UserApproved, opt => opt.MapFrom(src => src.admincheck))
                    .ForMember(dest => dest.TariffId, opt => opt.MapFrom(src => src.tariffId));

                cfg.CreateMap<UsersDomain, PisUsersDResetar>()
                    .ForMember(dest => dest.uId, opt => opt.MapFrom(src => src.UserId))
                    .ForMember(dest => dest.firstName, opt => opt.MapFrom(src => src.UserName))
                    .ForMember(dest => dest.lastName, opt => opt.MapFrom(src => src.UserSurname))
                    .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.UserEmail))
                    .ForMember(dest => dest.admincheck, opt => opt.MapFrom(src => src.UserApproved))
                    .ForMember(dest => dest.tariffId, opt => opt.MapFrom(src => src.TariffId));
                    

                cfg.CreateMap<UserDevice, UserDeviceModel>()
                    .ForMember(dest => dest.udId, opt => opt.MapFrom(src => src.udId))
                    .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.name))
                    .ForMember(dest => dest.from_date, opt => opt.MapFrom(src => src.from_date))
                    .ForMember(dest => dest.to_date, opt => opt.MapFrom(src => src.to_date));
                cfg.CreateMap<UserDeviceModel, UserDevice>()
                    .ForMember(dest => dest.udId, opt => opt.MapFrom(src => src.udId))
                    .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.name))
                    .ForMember(dest => dest.from_date, opt => opt.MapFrom(src => src.from_date))
                    .ForMember(dest => dest.to_date, opt => opt.MapFrom(src => src.to_date));
                cfg.CreateMap<UserDeviceModel, PisUsersDResetar>()
                    .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.email));
                cfg.CreateMap<PisUsersDResetar, UserDeviceModel>()
                    .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.email));

            });

            mapper = config.CreateMapper();
        }

        public TDestination Map<TDestination>(object source)
        {
            return mapper.Map<TDestination>(source);
        }
    }
}

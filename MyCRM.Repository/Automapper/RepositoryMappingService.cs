using AutoMapper;
using MyCRM.Repository.Automapper;
using MyCRM.DAL.DataModel;
using MyCRM.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCRM.Repository.Automapper
{
    public class RepositoryMappingService : IRepositoryMappingService
    {

        public Mapper mapper;

        public RepositoryMappingService()
        {
            var config = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<PisUsersDResetar, UsersDomain>(); //ruta baza - GUI
                    cfg.CreateMap<UsersDomain, PisUsersDResetar>(); //ruta GUI - baza

                });
            mapper = new Mapper(config);
        }
        public TDestination Map<TDestination>(object source)
        {
            return mapper.Map<TDestination>(source);
        }
    }
}
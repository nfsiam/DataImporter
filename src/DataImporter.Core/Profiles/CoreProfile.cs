using EO = DataImporter.Core.Entities;
using BO = DataImporter.Core.BusinessObjects;
using AutoMapper;

namespace DataImporter.Core.Profiles
{
    public class CoreProfile : Profile
    {
        public CoreProfile()
        {
            CreateMap<EO.Group, BO.Group>().ReverseMap();
            CreateMap<EO.Header, BO.Header>().ReverseMap();
            CreateMap<EO.Row, BO.Row>().ReverseMap();
            CreateMap<EO.Cell, BO.Cell>().ReverseMap();
            CreateMap<EO.Import, BO.Import>().ReverseMap();
            CreateMap<EO.Export, BO.Export>().ReverseMap();
            CreateMap<EO.Contact, BO.Contact>().ReverseMap();
        }
    }
}
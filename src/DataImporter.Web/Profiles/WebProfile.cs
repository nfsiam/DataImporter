using AutoMapper;
using DataImporter.Core.BusinessObjects;
using DataImporter.Web.Models.Dashboard;
using DataImporter.Web.Models.Group;
using DataImporter.Web.Models.Import;

namespace DataImporter.Web.Profiles
{
    public class ModuleOneProfile : Profile
    {
        public ModuleOneProfile()
        {
            CreateMap<Group, GroupListModel>().ReverseMap();
            CreateMap<CreateGroupModel, Group>().ReverseMap();
            CreateMap<EditGroupModel, Group>().ReverseMap();

            CreateMap<NewTaskModel, Import>().ReverseMap();
            CreateMap<Models.Export.NewTaskModel, Export>().ReverseMap();

            CreateMap<DashboardModel, Dashboard>().ReverseMap();
        }
    }
}
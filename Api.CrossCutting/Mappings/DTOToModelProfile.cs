using Api.Domain.DTO.User;
using Api.Domain.Models.User;
using AutoMapper;

namespace Api.CrossCutting.Mappings
{
    public class DTOToModelProfile : Profile
    {
        public DTOToModelProfile()
        {
            CreateMap<UserModel, UserDTO>()
                .ReverseMap();

            CreateMap<UserModel, UserDTOCreate>()
                .ReverseMap();

            CreateMap<UserModel, UserDTOUpdate>()
                .ReverseMap();
        }
    }
}

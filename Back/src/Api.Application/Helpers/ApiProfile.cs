using AutoMapper;
using Api.Aplication.Dtos;
using Api.Domain;
using Api.Domain.Identity;
using Api.Application.Dtos;

namespace Api.Application.Helpers
{
    public class ApiProfile : Profile
    {
        public ApiProfile()
        {
            CreateMap<Documento, DocumentoDto>().ReverseMap();

            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserLoginDto>().ReverseMap();
            CreateMap<User, UserUpdateDto>().ReverseMap();
        }

    }
}
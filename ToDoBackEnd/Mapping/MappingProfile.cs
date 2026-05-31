using AutoMapper;
using ToDoBackEnd.Models;
using ToDoBackEnd.Dtos;

namespace ToDoBackEnd.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TaskItem, TaskDto>();
    }
}
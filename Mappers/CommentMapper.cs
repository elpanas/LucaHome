using AutoMapper;
using LucaHome.DTO;
using LucaHome.Models;

namespace LucaHome.Mappers
{
    public class CommentMapper : Profile
    {
        public CommentMapper()
        {
            CreateMap<CommentDTOIn, Comment>();
            CreateMap<Comment, CommentDTOOut>();
        }
    }
}

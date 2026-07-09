using AutoMapper;
using Domain.Entities;
using LibrarySystem.web.ViewModels.Author;
using LibrarySystem.web.ViewModels.Publisher;
using LibrarySystem.Web.ViewModels.Book;
using LibrarySystem.Web.ViewModels.Category;

namespace LibrarySystem.web.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryViewModel, Category>();

            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorViewModel, Author>();

            CreateMap<Publisher, PublisherViewModel>();
            CreateMap<PublisherViewModel, Publisher>();

            CreateMap<Book, BookViewModel>()
    .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? src.Author.FullName : ""))
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : ""))
    .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher != null ? src.Publisher.Name : ""));

            CreateMap<BookViewModel, Book>();
        }
    }
}

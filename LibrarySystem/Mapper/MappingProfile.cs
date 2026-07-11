using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using LibrarySystem.web.ViewModels.Author;
using LibrarySystem.web.ViewModels.Book;
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
            CreateMap<CategoryDto, CategoryViewModel>();
            CreateMap<CategoryViewModel, CategoryDto>();

            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorViewModel, Author>();
            CreateMap<AuthorDto, AuthorViewModel>();
            CreateMap<AuthorViewModel, AuthorDto>();

            CreateMap<Publisher, PublisherViewModel>();
            CreateMap<PublisherViewModel, Publisher>();
            CreateMap<PublisherDto, PublisherViewModel>();
            CreateMap<PublisherViewModel, PublisherDto>();

            CreateMap<Book, BookViewModel>()
    .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? src.Author.FullName : ""))
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : ""))
    .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher != null ? src.Publisher.Name : ""));

            CreateMap<BookViewModel, Book>();
            CreateMap<BookDto, BookViewModel>();
            CreateMap<BookViewModel, BookDto>();

            CreateMap<Domain.Entities.BookCopy, BookCopyViewModel>();
            CreateMap<BookCopyViewModel, Domain.Entities.BookCopy>();


        }
    }
}

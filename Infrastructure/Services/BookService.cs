using Application.Common;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class BookService : IBookService
{
    private readonly IGenericService<Book> _generic;

    public BookService(IGenericService<Book> generic)
    {
        _generic = generic;
    }

    public async Task<List<BookDto>> GetAllAsync()
    {
        var items = await _generic.GetAllAsync();
        return items.Select(i => new BookDto
        {
            Id = i.Id,
            Title = i.Title,
            AuthorId = i.AuthorId,
            CategoryId = i.CategoryId,
            PublisherId = i.PublisherId,
            CoverImageUrl = i.CoverImageUrl,
            Description = i.Description,
            PublishedYear=i.PublishedYear,
            ISBN=i.ISBN,
            PublisherName=i.Publisher?.Name??string.Empty,
            CategoryName=i.Category?.Name??string.Empty,
            AuthorName=i.Author?.FullName??string.Empty

        }).ToList();
    }

    public async Task<BookDto?> GetByIdAsync(Guid id)
    {
        var item = await _generic.GetByIdAsync(id);
        return item == null ? null : new BookDto
        {
            Id = item.Id,
            Title = item.Title,
            AuthorId = item.AuthorId,
            CategoryId = item.CategoryId,
            PublisherId = item.PublisherId,
            CoverImageUrl = item.CoverImageUrl,
            Description = item.Description,
            ISBN=item.ISBN,
            PublishedYear=item.PublishedYear,
            PublisherName=item.Publisher?.Name??string.Empty,
            CategoryName=item.Category?.Name??string.Empty,
            AuthorName=item.Author?.FullName??string.Empty
        };
    }

    public async Task<Result> CreateAsync(BookDto dto)
    {
        var entity = new Book
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            Title = dto.Title,
            AuthorId = dto.AuthorId,
            CategoryId = dto.CategoryId,
            PublisherId = dto.PublisherId,
            CoverImageUrl = dto.CoverImageUrl,
            Description = dto.Description,
            PublishedYear=dto.PublishedYear,
            ISBN=dto.ISBN
        };

        return await _generic.CreateAsync(entity);
    }

    public async Task<Result> UpdateAsync(BookDto dto)
    {
        var entity = new Book
        {
            Id = dto.Id,
            Title = dto.Title,
            AuthorId = dto.AuthorId,
            CategoryId = dto.CategoryId,
            PublisherId = dto.PublisherId,
            CoverImageUrl = dto.CoverImageUrl,
            Description = dto.Description,
            PublishedYear=dto.PublishedYear,
            ISBN=dto.ISBN
        };

        return await _generic.UpdateAsync(entity);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        return await _generic.DeleteAsync(id);
    }
}

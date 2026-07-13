using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

        // Application service registrations (implementations live in Infrastructure)
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IPublisherService, PublisherService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IFineService, FineService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        return services;
    }
}
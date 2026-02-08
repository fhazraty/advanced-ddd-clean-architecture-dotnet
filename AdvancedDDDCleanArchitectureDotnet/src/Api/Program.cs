using Api.Model;
using Application.Abstractions;
using Application.Orders.Commands;
using Application.Orders.Queries;
using Infrastructure.Outbox;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Db")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));
builder.Services.AddHostedService<OutboxProcessor>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapPost("/orders", async (CreateOrderCommand cmd, ISender sender) =>
{
    var result = await sender.Send(cmd);
    return result.IsSuccess ? Results.Ok(new { id = result.Value }) : Results.BadRequest(result.Error);
});

app.MapPost("/orders/{id:guid}/confirm", async (Guid id, ISender sender) =>
{
    var result = await sender.Send(new ConfirmOrderCommand(id));
    return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
});

app.MapPost("/orders/{id:guid}/pay", async (Guid id, string paymentRef, ISender sender) =>
{
    var result = await sender.Send(new PayOrderCommand(id, paymentRef));
    return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
});

app.MapGet("/orders/confirmed", async (ISender sender) =>
{
    var orders = await sender.Send(new GetConfirmedOrdersQuery());
    return Results.Ok(orders.Select(o => new { o.Id, o.Status, Total = o.Total().Amount }));
});

app.MapPost("/orders/{id:guid}/items", async (Guid id, AddOrderItemRequest req, ISender sender) =>
{
    var cmd = new AddOrderItemCommand(id, req.ProductId, req.UnitPrice, req.Currency, req.Quantity);
    var result = await sender.Send(cmd);
    return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error);
});

app.Run();
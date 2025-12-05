using Banking.Application.Accounts;
using Banking.Application.Abstractions;
using Banking.Application.Transactions;
using Banking.Infrastructure.Persistence;
using Banking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// In-memory DB just for example
builder.Services.AddDbContext<BankingDbContext>(options =>
    options.UseInMemoryDatabase("BankingDB"));

// DI registrations
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<CreateAccountHandler>();
builder.Services.AddScoped<GetAccountBalanceHandler>();
builder.Services.AddScoped<PostTransactionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoints
app.MapPost("/api/accounts", async (CreateAccountCommand cmd, CreateAccountHandler handler) =>
{
    var result = await handler.Handle(cmd);
    return Results.Created($"/api/accounts/{result.Id}", result);
});

app.MapGet("/api/accounts/{accountNumber}", async (string accountNumber, GetAccountBalanceHandler handler) =>
{
    var result = await handler.Handle(new GetAccountBalanceQuery { AccountNumber = accountNumber });
    return Results.Ok(result);
});

app.MapPost("/api/transactions", async (PostTransactionCommand cmd, PostTransactionHandler handler) =>
{
    var result = await handler.Handle(cmd);
    return Results.Ok(result);
});

app.Run();
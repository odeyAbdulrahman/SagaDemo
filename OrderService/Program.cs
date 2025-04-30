
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SagaOrchestrator;
using static SharedContracts.ISharedContracts;

namespace OrderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();

            builder.Services.AddOpenApi();

            builder.Services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/orders/{orderId}", (Guid orderId, [FromServices] IOrderStateRepository repository) =>
            {
                var state = repository.GetState(orderId);
                return state is null
                    ? Results.NotFound($"Order {orderId} not found")
                    : Results.Ok(new OrderStatusResponse
                    {
                        OrderId = state.OrderId,
                        CurrentStatus = state.CurrentState,
                        Steps = state.Steps,
                        LastUpdated = state.LastUpdated,
                        FailureReason = state.FailureReason!
                    });
            })
            .WithName("GetOrderStatus")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get order processing status";
                operation.Description = "Returns the current state and processing history of a specific order";
                return operation;
            });

            // Get all orders
            app.MapGet("/orders/getAll", ([FromServices] IOrderStateRepository repository) =>
            {
                return Results.Ok(repository.GetAllStates()
                    .Select(s => new OrderStatusResponse
                    {
                        OrderId = s.OrderId,
                        CurrentStatus = s.CurrentState,
                        LastUpdated = s.LastUpdated
                    }));
            })
            .WithName("GetAllOrders")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get all orders";
                operation.Description = "Returns a list of all orders with their current status";
                return operation;
            });

            app.MapPost("/orders", handler: async (SubmitOrder order, IPublishEndpoint publishEndpoint) =>
            {
                await publishEndpoint.Publish(order);
                return Results.Accepted();
            });

            app.Run();
        }
    }
}

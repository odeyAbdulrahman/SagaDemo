using MassTransit;
using Microsoft.AspNetCore.Builder;

namespace SagaOrchestrator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<OrderSagaStateMachine, OrderSagaState>()
                    .InMemoryRepository();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("order-saga", e =>
                    {
                        e.ConfigureSaga<OrderSagaState>(context);
                    });

                    cfg.ConfigureEndpoints(context);

                    cfg.UseMessageRetry(r => r.Interval(5, 1000));
                });
            });

            var app = builder.Build();
            app.Run();
        }
    }
}

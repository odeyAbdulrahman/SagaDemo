using MassTransit;
using static SharedContracts.ISharedContracts;

namespace PaymentService;

public class PaymentConsumer : IConsumer<ProcessPayment>, IConsumer<RefundPayment>
{
    private static readonly Random _random = new();

    public async Task Consume(ConsumeContext<ProcessPayment> context)
    {
        await Task.Delay(1000);

        if (_random.Next(0, 5) == 0)
        {
            await context.Publish(new PaymentFailed(context.Message.OrderId, "Payment declined"));
            return;
        }

        var transactionId = $"TXN-{Guid.NewGuid()}";
        await context.Publish(new PaymentCompleted(context.Message.OrderId, transactionId));
    }

    public async Task Consume(ConsumeContext<RefundPayment> context)
    {
        // Simulate refund processing
        await Task.Delay(500);
        Console.WriteLine($"Refund processed for transaction {context.Message.TransactionId}");
    }
}
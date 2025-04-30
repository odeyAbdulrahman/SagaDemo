// InventoryConsumer.cs
using MassTransit;
using static SharedContracts.ISharedContracts;

namespace InventoryService;

public class InventoryConsumer : IConsumer<AllocateInventory>, IConsumer<DeallocateInventory>
{
    public async Task Consume(ConsumeContext<AllocateInventory> context)
    {
        await Task.Delay(800);

        if (new Random().Next(0, 5) == 0)
        {
            await context.Publish(new InventoryAllocationFailed(
                context.Message.OrderId,
                "Insufficient inventory"));
            return;
        }

        await context.Publish(new InventoryAllocated(context.Message.OrderId));
    }

    public async Task Consume(ConsumeContext<DeallocateInventory> context)
    {
        // Simulate inventory deallocation
        await Task.Delay(300);
        Console.WriteLine($"Inventory deallocated for order {context.Message.OrderId}");
    }
}
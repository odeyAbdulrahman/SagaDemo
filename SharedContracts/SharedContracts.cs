namespace SharedContracts
{
    public interface ISharedContracts
    {
        public record SubmitOrder(Guid OrderId, string CustomerId, List<OrderItem> Items, decimal Amount);

        public record OrderItem(Guid ProductId, int Quantity, decimal Price);

        public record OrderSubmitted(Guid OrderId, DateTime Timestamp);

        public record OrderSubmissionFailed(Guid OrderId, string Reason);

        public record CheckOrderStatus(Guid OrderId);

        public record OrderStatusResult(Guid OrderId, string Status);


        public record ProcessPayment(Guid OrderId, string CustomerId, decimal Amount);

        public record PaymentCompleted(Guid OrderId, string TransactionId);

        public record PaymentFailed(Guid OrderId, string Reason);

        public record RefundPayment(Guid OrderId, string TransactionId);


        public record AllocateInventory(Guid OrderId, List<OrderItem> Items);

        public record InventoryAllocated(Guid OrderId);

        public record InventoryAllocationFailed(Guid OrderId, string Reason);

        public record DeallocateInventory(Guid OrderId);
    }
}

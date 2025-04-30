using MassTransit;
using static SharedContracts.ISharedContracts;

namespace SagaOrchestrator
{
    public class OrderSagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = null!;
        public Guid OrderId { get; set; }
        public string CustomerId { get; set; } = null!;
        public decimal Amount { get; set; }
        public List<OrderItem> Items { get; set; } = [];
        public string? PaymentTransactionId { get; set; }
        public string? FailureReason { get; set; }
        public DateTime SubmittedDate { get; set; }

        public List<OrderProcessingStep> Steps { get; set; } = new();
        public DateTime? LastUpdated { get; set; }
    }

    public record OrderProcessingStep(
    string StepName,
    string Status,
    DateTime Timestamp,
    string? Details = null);
}

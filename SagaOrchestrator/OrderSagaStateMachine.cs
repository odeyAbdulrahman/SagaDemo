using MassTransit;
using Microsoft.Extensions.Logging;
using static SharedContracts.ISharedContracts;

namespace SagaOrchestrator
{
    public class OrderSagaStateMachine : MassTransitStateMachine<OrderSagaState>
    {
        // States
        public State Submitted { get; private set; } = default!;
        public State Processing { get; private set; } = default!;
        public State Completed { get; private set; } = default!;
        public State Failed { get; private set; } = default!;

        public Event<SubmitOrder> OrderSubmitted { get; private set; } = default!;
        public Event<PaymentCompleted> PaymentCompleted { get; private set; } = default!;
        public Event<PaymentFailed> PaymentFailed { get; private set; } = default!;
        public Event<InventoryAllocated> InventoryAllocated { get; private set; } = default!;
        public Event<InventoryAllocationFailed> InventoryAllocationFailed { get; private set; } = default!;

        public OrderSagaStateMachine(ILogger<OrderSagaStateMachine> logger)
        {
            // Initialize states
            InstanceState(x => x.CurrentState);

            // Initialize events with correlation
            Event(() => OrderSubmitted, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => PaymentCompleted, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => PaymentFailed, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => InventoryAllocated, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => InventoryAllocationFailed, x => x.CorrelateById(context => context.Message.OrderId));

            // State machine definition
            Initially(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.CustomerId = context.Message.CustomerId;
                        context.Saga.Amount = context.Message.Amount;
                        context.Saga.Items = context.Message.Items;
                        context.Saga.SubmittedDate = DateTime.UtcNow;
                        logger.LogInformation("Order submitted: {OrderId}", context.Saga.OrderId);
                    })
                    .Publish(context => new ProcessPayment(
                        context.Saga.OrderId,
                        context.Saga.CustomerId,
                        context.Saga.Amount))
                    .TransitionTo(Submitted)
                    .Catch<Exception>(ex => ex
                        .Then(context => logger.LogError(context.Exception, "Error processing OrderSubmitted"))
                    ));

            During(Submitted,
                When(PaymentCompleted)
                    .Then(context =>
                    {
                        context.Saga.PaymentTransactionId = context.Message.TransactionId;
                        logger.LogInformation("Payment completed for {OrderId}", context.Saga.OrderId);
                    })
                    .Publish(context => new AllocateInventory(
                        context.Saga.OrderId,
                        context.Saga.Items))
                    .TransitionTo(Processing),

                When(PaymentFailed)
                    .Then(context =>
                    {
                        context.Saga.FailureReason = context.Message.Reason;
                        logger.LogError("Payment failed for {OrderId}: {Reason}",
                            context.Saga.OrderId, context.Message.Reason);
                    })
                    .Publish(context => new OrderSubmissionFailed(
                        context.Saga.OrderId,
                        $"Payment failed: {context.Saga.FailureReason}"))
                    .TransitionTo(Failed));

            During(Processing,
                When(InventoryAllocated)
                    .Then(context =>
                        logger.LogInformation("Inventory allocated for {OrderId}", context.Saga.OrderId))
                    .Publish(context => new OrderSubmitted(
                        context.Saga.OrderId,
                        DateTime.UtcNow))
                    .TransitionTo(Completed),

                When(InventoryAllocationFailed)
                    .Then(context =>
                    {
                        context.Saga.FailureReason = context.Message.Reason;
                        logger.LogError("Inventory allocation failed for {OrderId}: {Reason}",
                            context.Saga.OrderId, context.Message.Reason);
                    })
                    .Publish(context => new RefundPayment(
                        context.Saga.OrderId,
                        context.Saga.PaymentTransactionId!))
                    .Publish(context => new OrderSubmissionFailed(
                        context.Saga.OrderId,
                        $"Inventory allocation failed: {context.Saga.FailureReason}"))
                    .TransitionTo(Failed));

            DuringAny(
                When(OrderSubmitted)
                    .Then(context =>
                        logger.LogDebug("Order submitted event received: {OrderId}", context.Message.OrderId)),

                When(PaymentCompleted)
                    .Then(context =>
                        logger.LogDebug("Payment completed event received: {OrderId}", context.Message.OrderId)),

                When(PaymentFailed)
                    .Then(context =>
                        logger.LogDebug("Payment failed event received: {OrderId}", context.Message.OrderId)),

                When(InventoryAllocated)
                    .Then(context =>
                        logger.LogDebug("Inventory allocated event received: {OrderId}", context.Message.OrderId)),

                When(InventoryAllocationFailed)
                    .Then(context =>
                        logger.LogDebug("Inventory allocation failed event received: {OrderId}", context.Message.OrderId)));
        }
    }
}
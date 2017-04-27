using System;
using System.Threading.Tasks;
using Rebus.Handlers;
using Rebus.Sagas;
using RebusTest.Messages;
using RebusTest.Messages.Commands;
using RebusTest.Messages.Events;
using Serilog;

namespace RebusTest.Handlers.DeliverySaga
{
    public class MessageDeliverySaga : 
        Saga<MessageDeliverySagaData>, 
        IAmInitiatedBy<DeliverMessage>, 
        IHandleMessages<ConfirmMessageIsDelivered>, 
        IHandleMessages<MessageDelivered>, 
        IHandleMessages<CheckMessageDeliveryResponse>
    {
        private static readonly TimeSpan MessageDeliveryVerifyTimeout = TimeSpan.FromSeconds(10);

        private static readonly int MaxAttemptsToManuallyCheckThatMessageWasDelivered = 3;

        private static readonly ILogger Log = Serilog.Log.Logger.ForContext<MessageDeliverySaga>();

        protected override void CorrelateMessages(ICorrelationConfig<MessageDeliverySagaData> config)
        {
            config.Correlate<DeliverMessage>(m => m.MessageId, s => s.Id);
            config.Correlate<ConfirmMessageIsDelivered>(m => m.MessageId, s => s.Id);
            config.Correlate<MessageDelivered>(m => m.MessageId, s => s.Id);
            config.Correlate<CheckMessageDeliveryResponse>(m => m.MessageId, s => s.Id);
        }

        public async Task Handle(DeliverMessage message)
        {
            Log.Information($"Initiating delivery of message: '{message.MessageBody}'");

            await Bus.Current.Send(new SendMessage {MessageId = message.MessageId, MessageBody = message.MessageBody});
            await ScheduleMessageDeliveryFerification(message.MessageId);
            
            Data.MessageDeliveryStatus = DeliveryStatus.DeliveryPending;
            Data.MessageBody = message.MessageBody;
        }

        public async Task Handle(ConfirmMessageIsDelivered message)
        {
            if (Data.MessageDeliveryStatus == DeliveryStatus.Delivered)
            {
                MarkAsComplete();
                Log.Information("All good. Saga completed.");
                return;
            }

            Data.DeliveryCheckAttempts += 1;

            Log.Information($"Attempt {Data.DeliveryCheckAttempts} to check whether message '{Data.MessageBody}' is delivered: Mssage delivery status is {Data.MessageDeliveryStatus}");

            await Bus.Current.Send(new CheckMessageDeliveryRequest {MessageId = message.MessageId});

            Log.Information("Sent [CheckMessageDeliveryRequest]");

            await ScheduleToRecheckDeliveryOrGiveup(message.MessageId);
        }

        public Task Handle(MessageDelivered message)
        {
            Data.MessageDeliveryStatus = DeliveryStatus.Delivered;
            MarkAsComplete();
            Log.Information("[MessageDelivered] event received. All done.");
            return Task.CompletedTask;
        }

        public Task Handle(CheckMessageDeliveryResponse message)
        {
            Data.MessageDeliveryStatus = message.IsSuccessfull
                ? DeliveryStatus.Delivered
                : DeliveryStatus.DeliveryFailed;

            Log.Information($"Received [CheckMessageDeliveryResponse]: Mesasge '{Data.MessageBody}' status is {Data.MessageDeliveryStatus}. All done.");

            MarkAsComplete();

            return Task.CompletedTask;
        }

        private async Task ScheduleToRecheckDeliveryOrGiveup(Guid messageId)
        {
            if (Data.DeliveryCheckAttempts <= MaxAttemptsToManuallyCheckThatMessageWasDelivered)
            {
                await ScheduleMessageDeliveryFerification(messageId);
            }
            else
            {
                Log.Information("Attempts exhausted. All done");
                MarkAsComplete();
            }
        }

        private static async Task ScheduleMessageDeliveryFerification(Guid messageId)
        {
            await Bus.Current.Defer(MessageDeliveryVerifyTimeout, new ConfirmMessageIsDelivered {MessageId = messageId});
        }
    }
}

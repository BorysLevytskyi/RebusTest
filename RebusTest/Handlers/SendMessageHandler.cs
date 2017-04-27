using System;
using System.Threading.Tasks;
using Rebus.Handlers;
using RebusTest.Messages.Commands;
using RebusTest.Messages.Events;
using Serilog;

namespace RebusTest.Handlers
{
    public class SendMessageHandler : IHandleMessages<SendMessage>
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<SendMessageHandler>();

        private static readonly Random Random = new Random();

        public async Task Handle(SendMessage message)
        {
            // Emulate unereliable system that 1 in 3 times doesn't send confirmation event
            Log.Information($"Sending message '{message.MessageBody}'");

            if (Random.Next(1, 4) == 3)
            {
                Log.Warning("Event wasn't published");
                return;
            }

            // in 50% cases it will arrive after 10 seconds
            if (Random.Next(1, 3) == 2)
            {
                Log.Warning("[MessageDelivered] event will be deferred by 5 seconds");
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            await Bus.Current.Publish(new MessageDelivered() {MessageId = message.MessageId});
        }
    }
}

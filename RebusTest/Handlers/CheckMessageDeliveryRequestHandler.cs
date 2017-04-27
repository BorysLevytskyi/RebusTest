using System.Threading.Tasks;
using Rebus.Handlers;
using RebusTest.Messages;
using RebusTest.Messages.Commands;

namespace RebusTest.Handlers
{
    public class CheckMessageDeliveryRequestHandler : IHandleMessages<CheckMessageDeliveryRequest>
    {
        public async Task Handle(CheckMessageDeliveryRequest message)
        {
            // Always respond as if message was successfully delivered
            await Bus.Current.Reply(new CheckMessageDeliveryResponse {MessageId = message.MessageId, IsSuccessfull = true});
        }
    }
}

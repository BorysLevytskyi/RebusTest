using System;

namespace RebusTest.Messages.Commands
{
    public class CheckMessageDeliveryRequest
    {
        public Guid MessageId { get; set; }
    }
}

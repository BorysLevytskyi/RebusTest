using System;

namespace RebusTest.Messages.Commands
{
    public class DeliverMessage
    {
        public Guid MessageId { get; set; }

        public string MessageBody { get; set; }
    }

    public class ConfirmMessageIsDelivered
    {
        public Guid MessageId { get; set; }
    }
}

using System;

namespace RebusTest.Messages
{
    public class CheckMessageDeliveryResponse
    {
        public Guid MessageId { get; set; }

        public bool IsSuccessfull { get; set; }
    }
}

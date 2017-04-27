using System;
using Rebus.Sagas;

namespace RebusTest.Handlers.DeliverySaga
{
    public class MessageDeliverySagaData : ISagaData
    {
        public Guid Id { get; set; }

        public int Revision { get; set; }

        public DeliveryStatus MessageDeliveryStatus { get; set; }

        public int DeliveryCheckAttempts { get; set; }

        public string MessageBody { get; set; }
    }
}
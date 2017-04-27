using System;
using System.Collections.Generic;
using System.Text;

namespace RebusTest.Messages.Events
{
    public class MessageDelivered
    {
        public Guid MessageId { get; set; }
    }
}

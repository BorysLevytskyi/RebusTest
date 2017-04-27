using System;
using System.Collections.Generic;
using System.Text;

namespace RebusTest.Messages.Commands
{
    public class SendMessage
    {
        public Guid MessageId { get; set; }

        public string MessageBody { get; set; }
    }
}

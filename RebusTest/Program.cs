using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Autofac;
using Rebus.Autofac;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Logging;
using Rebus.Routing.TypeBased;
using Rebus.Sagas;
using Rebus.SqlServer.Transport;
using RebusTest.Messages;
using RebusTest.Messages.Commands;
using RebusTest.Messages.Events;
using Serilog;
using Serilog.Core;

namespace RebusTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AppBootstrapper.StartTheBus();
            RunApp().Wait();
        }

        private static async Task RunApp()
        {
            string cmd;
            while ((cmd = ReadCommand()) != "exit")
            {
                if (string.IsNullOrEmpty(cmd))
                {
                    Console.WriteLine("Nothing to send");
                    continue;
                }

                Console.WriteLine($"Sending '{cmd}' to bus");
                await Bus.Current.Send(new DeliverMessage {MessageBody = cmd, MessageId = Guid.NewGuid()});
            }
        }

        private static string ReadCommand()
        {
            Console.Write("Type message to send: ");
            return Console.ReadLine();
        }
    }
}
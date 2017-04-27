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
            while ((cmd = Console.ReadLine()) != "exit")
            {
                if (string.IsNullOrEmpty(cmd))
                {
                    Log.Logger.Information("Nothing to send");
                    continue;
                }

                Log.Logger.Information($"Sending: {cmd}");
                await Bus.Current.Send(new DeliverMessage {MessageBody = cmd, MessageId = Guid.NewGuid()});
            }
        }
    }
}
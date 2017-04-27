using System.Reflection;
using Autofac;
using Rebus.Autofac;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;
using Rebus.Sagas;
using Rebus.SqlServer.Transport;
using RebusTest.Messages.Commands;
using RebusTest.Messages.Events;
using Serilog;

namespace RebusTest
{
    internal static class AppBootstrapper
    {
        public static void StartTheBus()
        {
            ConfigureLogger();
            var container = BuildContainer();
            StartBus(container);
        }

        private static void StartBus(IContainer container)
        {
            var endpointQueueName = "rebus.test";
            var connectionString = "server=.;database=RebusTest;trusted_connection=true";

            IBus bus = Configure.With(new AutofacContainerAdapter(container))
                .Transport(t => t.UseSqlServer(connectionString, "RebusQueue", endpointQueueName))
                .Routing(r => r.TypeBased().MapAssemblyOf<DeliverMessage>(endpointQueueName))
                .Logging(l => l.Serilog(Log.Logger))
                .Sagas(s => s.StoreInSqlServer(connectionString, "RebusSaga", "ix_RebusSga"))
                .Start();

            bus.Subscribe<MessageDelivered>();

            Bus.Current = bus;

            Log.Logger.Information("Bus Started");
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(Assembly.GetEntryAssembly())
                .Where(t => t.IsClosedTypeOf(typeof(IHandleMessages<>)) || t.IsClosedTypeOf(typeof(Saga<>)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            return builder.Build();
        }

        private static void ConfigureLogger()
        {
            Log.Logger = new Serilog.LoggerConfiguration()
                .WriteTo.ColoredConsole(outputTemplate: "[{Level}] {SourceContext} {Message}{NewLine}")
                .CreateLogger()
                .ForContext("{Source}", "App");
        }
    }
}
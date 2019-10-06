using Convey.MessageBrokers.Outbox.Outbox;
using Convey.MessageBrokers.Outbox.Processors;
using Microsoft.Extensions.DependencyInjection;

namespace Convey.MessageBrokers.Outbox
{
    public static class Extensions
    {
        public static IConveyBuilder AddMessageOutbox(this IConveyBuilder builder)
        {
            builder.Services.AddTransient<IMessageOutbox, MongoMessageOutbox>();
            builder.Services.AddHostedService<OutboxProcessor>();

            return builder;
        }
    }
}
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Convey.MessageBrokers.Outbox.Outbox;
using Convey.Persistence.MongoDB;
using Microsoft.Extensions.Hosting;

namespace Convey.MessageBrokers.Outbox.Processors
{
    internal sealed class OutboxProcessor : IHostedService
    {
        private readonly IMessageOutboxAccessor _outbox;
        private readonly IMongoSessionFactory _sessionFactory;
        private readonly IPublisher _publisher;

        public OutboxProcessor(IMessageOutbox outbox, IPublisher publisher, IMongoSessionFactory sessionFactory)
        {
            _outbox = outbox as IMessageOutboxAccessor;
            _publisher = publisher;
            _sessionFactory = sessionFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                using var session = _sessionFactory.CreateAsync();
                var messages = await _outbox.GetUnsentAsync();
                var publishTasks = messages.Select(om => _publisher.PublishAsync(om.Message, om.MessageId.ToString()));
                await Task.WhenAll(publishTasks);
                await _outbox.ProcessAsync(messages);

                await Task.Delay(2000, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
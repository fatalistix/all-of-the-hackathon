using HrDirectorRabbitService.Services;
using MassTransit;
using RabbitCommon.Messages;

namespace HrDirectorRabbitService.Consumers;

public class WishlistsConsumer(CollectingService service) : IConsumer<WishlistsMessage>
{
    public Task Consume(ConsumeContext<WishlistsMessage> context)
    {
        service.AddWishlists(context.Message.TeamLeadsWishlists, context.Message.JuniorsWishlists,
            context.Message.HackathonId);
        return Task.CompletedTask;
    }
}
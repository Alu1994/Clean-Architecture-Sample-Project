using CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Sells;
using CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Messaging;
using System.Text.Json;

namespace CleanArchitectureSampleProject.Core.Domain.AggregateRoots.Events;

public sealed class CreateSellEvent : DomainEventBase
{
    public CreateSellEvent(Sell sell)
    {
        Message = new SellEventData(sell);
    }
}

public sealed class SellEventData : IMessage
{
    public SellEventData()
    {

    }

    public SellEventData(Sell sell)
    {
        Body = sell;
    }

    public Sell? Body { get; private set; }

    public IMessage WithBody(string body)
    {
        Body = JsonSerializer.Deserialize<Sell>(body)!;
        return this;
    }
}
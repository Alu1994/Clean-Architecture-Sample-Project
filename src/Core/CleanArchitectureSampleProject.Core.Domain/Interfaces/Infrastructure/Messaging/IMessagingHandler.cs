namespace CleanArchitectureSampleProject.Core.Domain.Interfaces.Infrastructure.Messaging;

public interface IMessagingHandler
{
    public Type[] Event { get; }
    Task<CreateResult> SendMessage(IMessage message, CancellationToken cancellationToken);
    Task<IMessage> GetMessage(CancellationToken cancellationToken);
    Task<TMessage?> GetMessage<TMessage>(CancellationToken cancellationToken);
    Task<RemoveResult> RemoveMessage(CreateResult message, CancellationToken cancellationToken);
}

public interface IMessage;

public readonly record struct CreateResult(string MessageId, string PopReceipt, DateTimeOffset DispatchedDate);

public readonly record struct RemoveResult(bool IsRemoved, int StatusCode);



























//public interface IMessage<TMessageBody>
//{
//    string MessageId { get; init; }
//    string PopReceipt { get; init; }
//    TMessageBody Body { get; }
//    DateTimeOffset DispatchedDate { get; init; }

//    IMessage<TMessageBody> WithBody(TMessageBody body);
//    IMessage<TMessageBody> WithBody(string body);
//}

//public interface IPrimitive
//{

//}

//public interface IMessaging<TMessageBody> : IPrimitive
//{
//    Task<IMessage<TMessageBody>> SendMessage(IMessage<TMessageBody> message, CancellationToken cancellationToken);
//    Task<IMessage<TMessageBody>> GetMessage(CancellationToken cancellationToken);
//    Task<RemoveResult> RemoveMessage(IMessage<TMessageBody> message, CancellationToken cancellationToken);
//}





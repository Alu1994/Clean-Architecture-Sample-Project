namespace CleanArchitectureSampleProject.Domain.Interfaces.Infrastructure.Messaging;

public interface IMessagingHandler
{
    public Type[] Event { get; }
    Task<CreateResult> SendMessage(IMessage message, CancellationToken cancellationToken);
    Task<IMessage> GetMessage(CancellationToken cancellationToken);
    Task<TMessage?> GetMessage<TMessage>(CancellationToken cancellationToken);
    Task<RemoveResult> RemoveMessage(CreateResult message, CancellationToken cancellationToken);
}

public interface IMessage
{

}

public readonly struct CreateResult(string messageId, string popReceipt, DateTimeOffset dispatchedDate)
{
    public string MessageId { get; init; } = messageId;
    public string PopReceipt { get; init; } = popReceipt;
    public DateTimeOffset DispatchedDate { get; init; }
}


public readonly struct RemoveResult(bool isRemoved, int statusCode)
{
    public bool IsRemoved { get; init; } = isRemoved;
    public int StatusCode { get; init; } = statusCode;
}



























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





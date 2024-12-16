using CleanArchitectureSampleProject.CrossCuttingConcerns;

namespace CleanArchitectureSampleProject.Infrastructure.Messaging;

public record SerializeMessage<T>(string Type, string Content)
{
    public SerializeMessage(T Content) : this(typeof(T).Name, Json.SerializeWithoutReferenceLoop(Content)) { }
}

public record DeserializeMessage(string Type, string Content);
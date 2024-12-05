namespace CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;

public record CreateUserResourceResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ResourceId { get; set; }
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    public bool CanDelete { get; set; }
}

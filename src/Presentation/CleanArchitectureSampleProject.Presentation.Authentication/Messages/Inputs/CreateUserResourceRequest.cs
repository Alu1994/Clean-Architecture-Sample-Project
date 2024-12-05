namespace CleanArchitectureSampleProject.Presentation.Authentication.Messages.Inputs;

public record CreateUserResourceRequest
{
    public int UserId { get; set; }
    public int ResourceId { get; set; }
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    public bool CanDelete { get; set; }
}

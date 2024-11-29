namespace CleanArchitectureSampleProject.Presentation.Authentication.Messages.Inputs;

public record CreateUserRequest
{
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateTime CreationDate { get; set; }
}

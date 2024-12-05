namespace CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;

public record CreateUserResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateTime CreationDate { get; set; }
}

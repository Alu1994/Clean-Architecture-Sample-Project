namespace CleanArchitectureSampleProject.Presentation.Authentication.Messages.Outputs;

public record CreateResourceResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
}

using CleanArchitectureSampleProject.Core.Application.Inputs.Products;
using CleanArchitectureSampleProject.Core.Application.UseCases;

namespace CleanArchitectureSampleProject.Presentation.ControllerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController(ILogger<CategoryController> logger, ICategoryUseCases categoryUseCases) : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger = logger;
        private readonly ICategoryUseCases _categoryUseCases = categoryUseCases;

        [HttpGet(Name = "GetAllCategories")]
        public async Task<IResult> GetAll(CancellationToken cancellation)
        {
            const string errorTitleMessage = "Error while getting all categories.";

            var result = await _categoryUseCases.GetCategories(cancellation);
            
            return result.Match(success =>
                Results.Ok(success),
                error => Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: errorTitleMessage,
                    detail: error.Message,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }

        [HttpGet("{categoryId:Guid}", Name = "GetCategoryById")]
        public async Task<IResult> Get(Guid categoryId, CancellationToken cancellation)
        {
            const string errorTitleMessage = "Error while getting category by id.";

            var result = await _categoryUseCases.GetCategoryById(categoryId, cancellation);

            return result.Match(success => 
                Results.Ok(success),
                error => Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: errorTitleMessage,
                    detail: error.Message,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }

        [HttpGet("by-name/{categoryName}", Name = "GetCategoryByName")]
        public async Task<IResult> Get(string categoryName, CancellationToken cancellation)
        {
            const string errorTitleMessage = "Error while getting category by name.";

            var result = await _categoryUseCases.GetCategoryByName(categoryName, cancellation);

            return result.Match(success =>
                Results.Ok(success),
                error => Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: errorTitleMessage,
                    detail: error.Message,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }

        [HttpPost(Name = "CreateCategory")]
        public async Task<IResult> Post(CreateCategoryInput category, CancellationToken cancellation)
        {
            const string errorTitleMessage = "Error while creating new category.";

            var result = await _categoryUseCases.CreateCategory(category, cancellation);

            return result.Match(success =>
                Results.Ok(success),
                error => Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: errorTitleMessage,
                    detail: error.Message,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }
    }
}

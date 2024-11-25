using CleanArchitectureSampleProject.Core.Application.Inputs;
using CleanArchitectureSampleProject.Core.Application.UseCases;
using CleanArchitectureSampleProject.CrossCuttingConcerns;

namespace CleanArchitectureSampleProject.Presentation.ControllerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductUseCases _productUseCases;

        public ProductController(ILogger<ProductController> logger, IProductUseCases productUseCases)
        {
            _logger = logger;
            _productUseCases = productUseCases;
        }

        [HttpGet(Name = "GetAllProducts")]
        public async Task<IResult> GetAll(CancellationToken cancellation)
        {
            const string errorTitleMessage = "Error while getting all products.";

            var result = await _productUseCases.GetProducts(cancellation);

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

        [HttpGet("{productId:Guid}", Name = "GetProductById")]
        public async Task<IResult> Get(Guid productId, CancellationToken cancellation)
        {
            const string errorTitleMessage = "Error while getting product by id.";

            var result = await _productUseCases.GetProductById(productId, cancellation);

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

        [HttpGet("by-names/{productName}", Name = "GetProductByName")]
        public async Task<IResult> Get(string productName, CancellationToken cancellation)
        {
            const string errorTitleMessage = "Error while getting product by name.";

            var result = await _productUseCases.GetProductByName(productName, cancellation);

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

        [HttpPost(Name = "CreateProduct")]
        public async Task<IResult> Post(CreateProductInput product, CancellationToken cancellation)
        {
            const string errorTitleMessage = "Error while creating new product.";

            var result = await _productUseCases.CreateProduct(product, cancellation);

            return result.Match(success =>
                Results.Ok(success),
                error => Results.BadRequest(new
                {
                    @Type = HttpStatusCode.BadRequest.ToString(),
                    Title = errorTitleMessage,
                    Detail = errorTitleMessage,
                    Errors = error.Errors.Select(x => x.Message),
                    StatusCode = StatusCodes.Status400BadRequest
                })
            );
        }
    }
}

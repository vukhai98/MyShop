using eShopSolution.Application.Catalog.Products;
using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {

        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }


        // https://localhost:5001/api/product/?pageIndex=1&pageSize=10&CategoryId=
        [HttpGet("{languageId}")]
        public async Task<IActionResult> GetAllPaging(string languageId, [FromQuery] GetPublicProductPagingRequest request)
        {
            var products = await _productService.GetAllByCategoryId(languageId, request);

            return Ok(products);
        }


        // https://localhost:5001/api/product/{id}
        [HttpGet("{productId}/{languageId}")]
        public async Task<IActionResult> GetById(int productId, string languageId)
        {
            var product = await _productService.GetProductById(productId, languageId);

            if (product == null)
                return BadRequest($"Cann't find product with id :{productId}");

            return Ok(product);
        }

        // https://localhost:5001/api/product
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productId = await _productService.Create(request);

            if (productId == 0)
                return BadRequest();

            return Ok(request);

        }

        // https://localhost:5001/api/product
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ProductUpdateRequest request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var affectedResult = await _productService.Update(request);

            if (affectedResult == 0)
                return BadRequest();

            return Ok();

        }


        // https://localhost:5001/api/product/{id}
        [HttpDelete("productId")]
        public async Task<IActionResult> Delete(int productId)
        {
            var result = await _productService.Delete(productId);

            if (result == 0)
                return BadRequest();

            return Ok(result);
        }


        // https://localhost:5001/api/product
        [HttpPatch("price/{productId}/newPrice")]
        public async Task<IActionResult> UpdatePrice(int id, decimal newPrice)
        {
            var isSuccessful = await _productService.UpdatePrice(id, newPrice);

            if (isSuccessful)
                return Ok();

            return BadRequest();

        }

        // Images 

        [HttpPost("{productId}/images")]
        public async Task<IActionResult> CreateImage(int productId, [FromForm] ProductImageCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var imageId = await _productService.AddImage(productId, request);

            if (imageId == 0)
            {
                return BadRequest();
            }

            var image = await _productService.GetImageById(imageId);

            return CreatedAtAction(nameof(GetImageById), new { id = imageId }, image);

        }


        [HttpPut("{productId}/images/{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromForm] ProductImageUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _productService.UpdateImage(imageId, request);

            if (result == 0)
            {
                return BadRequest();
            }

            return Ok(result);

        }


        [HttpDelete("{productId}/images/{imageId}")]
        public async Task<IActionResult> RemoveImage(int imageId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _productService.RemoveImage(imageId);

            if (result == 0)
            {
                return BadRequest();
            }

            return Ok(result);

        }


        [HttpGet("{productId}/images/{imageId}")]
        public async Task<IActionResult> GetImageById(int imageId)
        {
            var image = await _productService.GetImageById(imageId);

            if (image == null)
            {
                return BadRequest($"Cannot find image witd : {imageId}");
            }

            return Ok(image);

        }

    }
}

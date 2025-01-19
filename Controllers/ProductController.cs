using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebCoffee.Models;
using WebCoffee.Repository;

namespace WebCoffee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")] // Áp dụng CORS cho controller này

    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Lấy tất cả sản phẩm
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return Ok(products); // Trả về danh sách sản phẩm
        }



        [HttpGet("find/{name}")]
        public async Task<ActionResult<sanpham?>> GetProductByName(string name)
        {
            var product = await _productRepository.GetProductByNameAsync(name);
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }

            return Ok(product);
        }


        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductModel productModel)
        {
            var result = await _productRepository.AddProductAsync(productModel);
            return result; // Trả về kết quả từ repository
        }

        // Phương thức chỉnh sửa sản phẩm
        [HttpPut("edit products/{name}")]
        public async Task<IActionResult> EditProduct(string name, [FromForm] ProductModel productModel)
        {
            var result = await _productRepository.EditProductByNameAsync(name, productModel);

            if (result.StatusCode == StatusCodes.Status404NotFound)
            {
                return NotFound(result.Value);
            }

            return Ok(result.Value);
        }


        // Xóa sản phẩm
        [HttpDelete("delete/{name}")]
        public async Task<string> DeleteProduct(string name)
        {
            return await _productRepository.DeleteProductAsync(name);
        }

    }
}

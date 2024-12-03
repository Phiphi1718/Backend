using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using WebCoffee.Data;
using WebCoffee.Models;

namespace WebCoffee.Repository
{
    public interface IProductRepository
    {
        Task<List<products>> GetAllProductsAsync(); // Lấy tất cả sản phẩm
        Task<JsonResult> AddProductAsync(ProductModel productModel);
        Task<JsonResult> EditProductByNameAsync(string name, ProductModel productModel);
        Task<string> DeleteProductAsync(string Name);
        Task<sanpham> GetProductByNameAsync(string name);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly CoffeeHouseDbContext _context;
        private readonly IHinhAnhRepository _hinhAnhRepository;

        public ProductRepository(CoffeeHouseDbContext context, IHinhAnhRepository hinhAnhRepository)
        {
            _context = context;
            _hinhAnhRepository = hinhAnhRepository;
        }

        public async Task<List<products>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category) // Bao gồm thông tin Category để lấy tên danh mục
                .Select(product => new products
                {
                    Id = product.Id,
                    ImageUrl = product.ImageUrl,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    CategoryName = product.Category.CategoryName // Lấy tên danh mục thay vì ID
                })
                .ToListAsync();
        }

        public async Task<sanpham> GetProductByNameAsync(string name)
        {
            var product = await _context.Products
                .Include(p => p.Category) // Bao gồm thông tin Category
                .Where(p => p.Name == name)
                .Select(p => new sanpham
                {
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CategoryName = p.Category.CategoryName, // Lấy tên danh mục thay vì ID
                    ImageUrls = new List<string> { p.ImageUrl }
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                throw new Exception("Không tìm thấy sản phẩm");
            }

            return product;
        }

        public async Task<JsonResult> AddProductAsync(ProductModel productModel)
        {
            // Kiểm tra nếu sản phẩm đã tồn tại
            var existingProduct = await _context.Products.SingleOrDefaultAsync(p => p.Name == productModel.Name);
            if (existingProduct != null)
            {
                return new JsonResult("Sản phẩm đã tồn tại")
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            // Kiểm tra nếu Category tồn tại
            var category = await _context.ProductCategories.SingleOrDefaultAsync(c => c.Id.ToString() == productModel.CategoryId);
            if (category == null)
            {
                return new JsonResult("Danh mục không tồn tại")
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            // Tạo sản phẩm mới
            var product = new Product
            {
                Name = productModel.Name,
                Price = productModel.Price ?? 0,
                Description = productModel.Description,
                CategoryId = category.Id
            };

            // Nếu có hình ảnh, lưu ảnh và lấy URL
            if (productModel.Images != null && productModel.Images.Count > 0)
            {
                var imageUrls = await _hinhAnhRepository.WriteFileAsync(productModel.Images, "Products");
                if (imageUrls.Count > 0)
                {
                    product.ImageUrl = imageUrls[0]; // Gán URL của ảnh đầu tiên
                }
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return new JsonResult("Sản phẩm và hình ảnh đã được thêm thành công")
            {
                StatusCode = StatusCodes.Status201Created
            };
        }


        public async Task<JsonResult> EditProductByNameAsync(string name, ProductModel productModel)
        {
            var existingProduct = await _context.Products.SingleOrDefaultAsync(p => p.Name == name);

            if (existingProduct == null)
            {
                return new JsonResult("Không tìm thấy sản phẩm cần chỉnh sửa")
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            // Kiểm tra nếu CategoryId hợp lệ
            var category = await _context.ProductCategories.SingleOrDefaultAsync(c => c.Id.ToString() == productModel.CategoryId);
            if (category == null)
            {
                return new JsonResult("Danh mục không tồn tại")
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            // Cập nhật thông tin sản phẩm
            existingProduct.Name = productModel.Name;
            existingProduct.Price = productModel.Price ?? existingProduct.Price;
            existingProduct.Description = productModel.Description ?? existingProduct.Description;
            existingProduct.CategoryId = category.Id;

            // Cập nhật hình ảnh nếu có
            if (productModel.Images != null && productModel.Images.Count > 0)
            {
                var imageUrls = await _hinhAnhRepository.WriteFileAsync(productModel.Images, "Products");
                if (imageUrls.Count > 0)
                {
                    existingProduct.ImageUrl = imageUrls[0];
                }
            }

            await _context.SaveChangesAsync();
            return new JsonResult("Đã chỉnh sửa thông tin sản phẩm")
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        public async Task<string> DeleteProductAsync(string name)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
            if (product == null)
            {
                return "Xóa sản phẩm không thành công: Sản phẩm không tồn tại";
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return "Xóa sản phẩm thành công";
        }

    }


}

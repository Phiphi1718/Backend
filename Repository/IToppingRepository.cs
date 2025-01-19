using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using WebCoffee.Data;
using WebCoffee.Models;
using WebCoffee.Repository;

public interface IToppingRepository
{
    Task<IEnumerable<topping>> GetAllToppingsAsync();
    Task<topping> GetToppingByNameAsync(string name); // Sửa phương thức này để lấy topping theo tên
    Task<JsonResult> AddTopingAsync(Toping topingModel);
    Task<JsonResult> EditTopingAsync(string name, Toping topingModel); // Sửa phương thức này để chỉnh sửa theo tên
    Task<JsonResult> DeleteToppingAsync(string name); // Sửa phương thức này để xóa topping theo tên
}

public class ToppingRepository : IToppingRepository
{
    private readonly CoffeeHouseDbContext _context;
    private readonly IHinhAnhRepository _hinhAnhRepository;

    public ToppingRepository(CoffeeHouseDbContext context, IHinhAnhRepository hinhAnhRepository)
    {
        _context = context;
        _hinhAnhRepository = hinhAnhRepository;
    }

    public async Task<IEnumerable<topping>> GetAllToppingsAsync()
    {
        return await _context.Toppings
            .Select(product => new topping
            {
                Name = product.Name,
                Price = product.Price ?? 0, // Xử lý null bằng cách thay giá trị mặc định là 0
                Description = product.Description,
                Img = product.Img
            })
            .ToListAsync();
    }



    public async Task<topping> GetToppingByNameAsync(string name)
    {
        var toppings = await _context.Toppings
            .Where(p => p.Name == name)
            .Select(p => new topping
            {
                Name = p.Name,
                Price = p.Price ?? 0, // Sử dụng null-coalescing để xử lý null
                Description = p.Description,
                Img = p.Img
            })
            .FirstOrDefaultAsync();

        if (toppings == null)
        {
            throw new Exception("Không tìm thấy topping");
        }
        return toppings;
    }

    public async Task<JsonResult> AddTopingAsync(Toping topingModel)
    {
        var existingToping = await _context.Toppings.SingleOrDefaultAsync(t => t.Name == topingModel.Name);

        if (existingToping == null)
        {
            var toping = new Topping
            {
                Name = topingModel.Name,
                Price = topingModel.Price,
                Description = topingModel.Description
            };

            if (topingModel.Img != null && topingModel.Img.Count > 0)
            {
                var imageUrls = await _hinhAnhRepository.WriteFileAsync(topingModel.Img, "Topings");
                if (imageUrls.Count > 0)
                {
                    toping.Img = imageUrls[0];
                }
            }

            await _context.Toppings.AddAsync(toping);
            await _context.SaveChangesAsync();

            return new JsonResult("Topping và hình ảnh đã được thêm thành công")
            {
                StatusCode = StatusCodes.Status201Created
            };
        }
        else
        {
            return new JsonResult("Topping đã tồn tại")
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }

    public async Task<JsonResult> EditTopingAsync(string name, Toping topingModel)
    {
        var existingToping = await _context.Toppings.SingleOrDefaultAsync(t => t.Name == name);
        if (existingToping == null)
        {
            return new JsonResult("Không tìm thấy topping cần chỉnh sửa")
            {
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        existingToping.Name = topingModel.Name;
        existingToping.Price = topingModel.Price;
        existingToping.Description = topingModel.Description;

        if (topingModel.Img != null && topingModel.Img.Count > 0)
        {
            var imageUrls = await _hinhAnhRepository.WriteFileAsync(topingModel.Img, "Topings");

            if (imageUrls.Count > 0)
            {
                existingToping.Img = imageUrls[0];
            }
        }

        await _context.SaveChangesAsync();
        return new JsonResult("Đã chỉnh sửa thông tin topping")
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    public async Task<JsonResult> DeleteToppingAsync(string name)
    {
        var topping = await _context.Toppings.SingleOrDefaultAsync(t => t.Name == name);
        if (topping == null)
        {
            return new JsonResult("Không tìm thấy topping cần xóa")
            {
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        _context.Toppings.Remove(topping);
        await _context.SaveChangesAsync();

        return new JsonResult("Xóa thành công")
        {
            StatusCode = StatusCodes.Status200OK
        };
    }
}

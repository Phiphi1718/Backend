using WebCoffee.Data;

namespace WebCoffee.Models
{
    public class ProductVM
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; } // Sử dụng CategoryName thay vì CategoryId
    }
    public class products : ProductVM
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

        public virtual ProductCategory? Category { get; set; }


        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
    public class ProductModel
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? CategoryId { get; set; }
        public List<IFormFile>? Images { get; set; }
    }

    public class sanpham
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; } // Sử dụng CategoryName thay vì CategoryId
        public List<string>? ImageUrls { get; set; }
    }
    public class Image
    {
        public string Url { get; set; }
    }


}

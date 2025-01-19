using WebCoffee.Data;

namespace WebCoffee.Models
{
    public class ReviewMD
    {
        public string UserEmail { get; set; } // Email của người dùng

        public string ProductName { get; set; } // Tên sản phẩm

        public int? Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime? ReviewDate { get; set; }

    }
    public class ReviewMD2
    {
        public int Id { get; set; }
        public virtual Product Product { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}

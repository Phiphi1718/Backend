using WebCoffee.Data;

namespace WebCoffee.Models
{
    public class OrderMD
    {
       
        public int? UserId { get; set; }

        public DateTime? OrderDate { get; set; }

        public int? TotalItems { get; set; }

        public decimal? TotalAmount { get; set; }

        public int? PaymentId { get; set; }

       
    }
    public class OrderVM : OrderMD
    {
        public int Id { get; set; }

        public virtual ICollection<OrderDetaildsVM> OrderDetails { get; set; } = new List<OrderDetaildsVM>(); // Thay đổi kiểu thành OrderDetaildsVM

        public virtual Payment? Payment { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public virtual User? User { get; set; }
    }

}

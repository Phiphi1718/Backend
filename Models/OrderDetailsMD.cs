using WebCoffee.Data;

namespace WebCoffee.Models
{
    public class OrderDetailsMD
    {

        public int? OrderId { get; set; }

        public int? ProductId { get; set; }

        public int? ToppingId { get; set; }

        public int? Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? ToppingPrice { get; set; }

        public decimal? TotalPrice { get; set; }

    }
    public class OrderDetaildsVM : OrderDetailsMD
    {
        public int Id { get; set; }
        public virtual Order? Order { get; set; }

        public virtual Product? Product { get; set; }

        public virtual Topping? Topping { get; set; }

    }
}

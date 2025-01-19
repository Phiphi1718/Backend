using WebCoffee.Data;

namespace WebCoffee.Models
{
    public class Toping
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public List<IFormFile>? Img { get; set; }
    }
    public class topping
    {

        public string? Name { get; set; }

        public decimal? Price { get; set; }

        public string? Description { get; set; }

        public string? Img { get; set; }
    }
    public class topingss : topping
    {
        public int Id { get; set; }
    }

}

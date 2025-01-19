namespace WebCoffee.Models
{
    public class JobDTO
    {
        public string? FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? CitizenId { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Gender { get; set; }

        public string? EducationLevel { get; set; }

        public string? Address { get; set; }

        public string? Position { get; set; }
        public List<IFormFile>? Images { get; set; }
    }

}

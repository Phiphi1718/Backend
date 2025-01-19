using Microsoft.EntityFrameworkCore;
using WebCoffee.Data;
using WebCoffee.Models;

namespace WebCoffee.Repository
{
    public interface IReviewRepository
    {
        Task<IEnumerable<ReviewMD2>> GetAllReviewsAsync();
        Task<ReviewMD2?> GetReviewByIdAsync(int id);

        Task<ReviewMD2> AddReviewAsync(ReviewMD review);

    }

    public class ReviewRepository : IReviewRepository
    {
        private readonly CoffeeHouseDbContext _context;

        public ReviewRepository(CoffeeHouseDbContext context)
        {
            _context = context;
        }

        // Thêm mới review với email và tên sản phẩm
        // Thêm mới review với email và tên sản phẩm
        public async Task<ReviewMD2> AddReviewAsync(ReviewMD review)
        {
            // Tìm User theo email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == review.UserEmail);

            if (user == null)
                throw new InvalidOperationException("User not found with the provided email.");

            // Tìm Product theo tên
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Name == review.ProductName);

            if (product == null)
                throw new InvalidOperationException("Product not found with the provided name.");

            // Tạo đối tượng review mới
            var newReview = new Review
            {
                UserId = user.Id, // Lấy UserId từ kết quả tìm kiếm user
                ProductId = product.Id, // Lấy ProductId từ kết quả tìm kiếm product
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.ReviewDate ?? DateTime.UtcNow
            };

            // Thêm review vào database
            _context.Reviews.Add(newReview);
            await _context.SaveChangesAsync();

            // Trả về đối tượng ReviewMD2 để trả về cho client
            var createdReview = await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == newReview.Id);

            if (createdReview == null)
                throw new InvalidOperationException("Error occurred while adding the review.");

            return new ReviewMD2
            {
                Id = createdReview.Id,
                Product = createdReview.Product!,
                User = createdReview.User!
            };
        }
        // Lấy tất cả review với thông tin User và Product
        public async Task<IEnumerable<ReviewMD2>> GetAllReviewsAsync()
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .Select(r => new ReviewMD2
                {
                    Id = r.Id,
                    Product = r.Product!,
                    User = r.User!
                })
                .ToListAsync();
        }

        // Lấy review theo ID
        public async Task<ReviewMD2?> GetReviewByIdAsync(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return null;

            return new ReviewMD2
            {
                Id = review.Id,
                Product = review.Product!,
                User = review.User!
            };
        }
    }
}

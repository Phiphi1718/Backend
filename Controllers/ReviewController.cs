using Microsoft.AspNetCore.Mvc;
using WebCoffee.Models;
using WebCoffee.Repository;

namespace WebCoffee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        // GET: api/Review
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewMD2>>> GetAllReviews()
        {
            try
            {
                var reviews = await _reviewRepository.GetAllReviewsAsync();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Review/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewMD2>> GetReviewById(int id)
        {
            try
            {
                var review = await _reviewRepository.GetReviewByIdAsync(id);

                if (review == null)
                {
                    return NotFound();
                }

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Review
        [HttpPost]
        public async Task<ActionResult<ReviewMD2>> AddReview(ReviewMD review)
        {
            try
            {
                var createdReview = await _reviewRepository.AddReviewAsync(review);

                // Return the created review
                return CreatedAtAction(nameof(GetReviewById), new { id = createdReview.Id }, new
                {
                    message = "Review added successfully!",
                    review = createdReview
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);  // Invalid user or product
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

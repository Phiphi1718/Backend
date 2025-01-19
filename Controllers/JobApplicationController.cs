using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCoffee.Data;
using WebCoffee.Models;
using WebCoffee.Repository;

namespace WebCoffee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplicationsRepository _repository;

        public JobApplicationController(IJobApplicationsRepository repository)
        {
            _repository = repository;
        }


        [HttpPost("AddJobApplication")]
        public async Task<JsonResult> AddJobApplicationAsync([FromForm] JobDTO jobDTO)
        {
            // Gọi phương thức AddJobApplicationAsync từ Repository để xử lý việc thêm ứng viên
            return await _repository.AddJobApplicationAsync(jobDTO);
        }

        // Lấy đơn xin việc theo tên
        [HttpGet("ByName/{name}")]
        public async Task<ActionResult<JobApplication>> GetJobApplicationByName(string name)
        {
            var jobApplication = await _repository.GetJobApplicationByNameAsync(name);

            if (jobApplication == null)
            {
                return NotFound();
            }

            return Ok(jobApplication);
        }

        // Lấy tất cả các đơn xin việc
        [HttpGet("Getall")]
        public async Task<ActionResult<IEnumerable<JobApplication>>> GetAllJobApplications()
        {
            var jobApplications = await _repository.GetAllJobApplicationsAsync();

            if (jobApplications == null || !jobApplications.Any())
            {
                return NotFound("No job applications found.");
            }

            return Ok(jobApplications);
        }
    }
}



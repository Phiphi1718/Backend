using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCoffee.Data;
using WebCoffee.Models;

namespace WebCoffee.Repository
{
    public interface IJobApplicationsRepository
    {
        Task<JobApplication> GetJobApplicationByNameAsync(string name);
        Task<IEnumerable<JobApplication>> GetAllJobApplicationsAsync();
        Task<JsonResult> AddJobApplicationAsync(JobDTO jobDTO);  // Thay vì Job, sử dụng JobDTO và trả về JsonResult
    }

    public class JobApplicationRepository : IJobApplicationsRepository
    {
        private readonly CoffeeHouseDbContext _context;
        private readonly IHinhAnhRepository _imageRepository;

        public JobApplicationRepository(CoffeeHouseDbContext context, IHinhAnhRepository hinhAnhRepository)
        {
            _context = context;
            _imageRepository = hinhAnhRepository;
        }

        // Lấy JobApplication theo tên
        public async Task<JobApplication> GetJobApplicationByNameAsync(string name)
        {
            // Sử dụng ToLower() để so sánh không phân biệt chữ hoa chữ thường
            return await _context.JobApplications
                .FirstOrDefaultAsync(j => j.FullName.ToLower() == name.ToLower());
        }

        // Lấy tất cả JobApplications
        public async Task<IEnumerable<JobApplication>> GetAllJobApplicationsAsync()
        {
            return await _context.JobApplications.ToListAsync();
        }
        public async Task<JsonResult> AddJobApplicationAsync(JobDTO jobDTO)
        {
            // Kiểm tra nếu đơn ứng tuyển đã tồn tại
            var existingJobApplication = await _context.JobApplications
                .SingleOrDefaultAsync(j => j.FullName == jobDTO.FullName && j.CitizenId == jobDTO.CitizenId);
            if (existingJobApplication != null)
            {
                return new JsonResult("Ứng viên đã tồn tại")
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            // Tạo JobApplication mới
            var jobApplication = new JobApplication
            {
                FullName = jobDTO.FullName,
                BirthDate = jobDTO.BirthDate,
                CitizenId = jobDTO.CitizenId,
                Phone = jobDTO.Phone,
                Email = jobDTO.Email,
                Gender = jobDTO.Gender,
                EducationLevel = jobDTO.EducationLevel,
                Address = jobDTO.Address,
                Position = jobDTO.Position,
            };

            // Nếu có hình ảnh, lưu ảnh và lấy URL
            if (jobDTO.Images != null && jobDTO.Images.Count > 0)
            {
                try
                {
                    var imageUrls = await _imageRepository.WriteFileAsync(jobDTO.Images, "JobApplications"); // Gọi repository để lưu ảnh
                    if (imageUrls.Count > 0)
                    {
                        jobApplication.Image = imageUrls[0]; // Gán URL của ảnh đầu tiên
                    }
                }
                catch (Exception ex)
                {
                    return new JsonResult($"Lỗi khi tải ảnh: {ex.Message}")
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
            }

            // Thêm JobApplication vào DbContext và lưu thay đổi
            _context.JobApplications.Add(jobApplication);
            await _context.SaveChangesAsync();

            return new JsonResult("Đơn ứng tuyển đã được thêm thành công")
            {
                StatusCode = StatusCodes.Status201Created
            };
        }



    }



}

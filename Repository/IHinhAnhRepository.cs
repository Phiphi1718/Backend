namespace WebCoffee.Repository
{
    public interface IHinhAnhRepository
    {
        Task<List<string>> WriteFileAsync(List<IFormFile> files, string folder);

    }
    public class HinhAnhSPRepository : IHinhAnhRepository
    {
        public Task<string> SaveImageAsync(string imgBase64, string folderName)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> WriteFileAsync(List<IFormFile> files, string folder)
        {
            string local;
            var imageUrls = new List<string>();
            var errorMessages = new List<string>(); // Danh sách để lưu trữ thông báo lỗi

            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    continue;
                }

                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];

                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                {
                    local = "Images";
                }
                else if (extension == ".pdf" || extension == ".doc" || extension == ".docx" || extension == ".xls" || extension == ".xlsx")
                {
                    local = "Files";
                }
                else
                {
                    // Nếu extension không hợp lệ, thêm thông báo lỗi vào danh sách và chuyển sang file tiếp theo
                    errorMessages.Add($"File không hợp lệ '{file.FileName}'.");
                    continue;
                }

                try
                {
                    var exactPath = Path.Combine(Directory.GetCurrentDirectory(), "UpLoad", local, folder);

                    // Tạo thư mục nếu nó chưa tồn tại
                    if (!Directory.Exists(exactPath))
                    {
                        Directory.CreateDirectory(exactPath);
                    }

                    var filePath = Path.Combine(exactPath, file.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    string result = $"Upload/{local}/{folder}/{file.FileName}";
                    imageUrls.Add(result);
                }
                catch (Exception ex)
                {
                    errorMessages.Add($"Lỗi khi upload file '{file.FileName}': {ex.Message}");
                }
            }

            // Kiểm tra nếu có lỗi, trả về danh sách thông báo lỗi
            if (errorMessages.Count > 0)
            {
                throw new Exception(string.Join(Environment.NewLine, errorMessages));
            }

            return imageUrls;
        }

    }
}

 using System;
using System.IO;
using System.Threading.Tasks;
using BlazorInputFile;
using Bookstore_UI.Contracts;
using Microsoft.AspNetCore.Hosting;

namespace Bookstore_UI.Services
{
    public class FileUpload : IFileUpload
    {
        private readonly IWebHostEnvironment _env;
        public FileUpload(IWebHostEnvironment env)
        {
            _env = env;
        }
        public void RemoveFile(string imageName)
        {
            var path = $"{_env.WebRootPath}\\uploads\\{imageName}";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        public async Task UploadFile(IFileListEntry file, string imageName)
        {
            try
            {
                var ms = new MemoryStream();
                await file.Data.CopyToAsync(ms);

                var path = $"{_env.WebRootPath}\\uploads\\{imageName}";

                using(FileStream fs = new FileStream(path, FileMode.Create))
                {
                    ms.WriteTo(fs);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void UploadFile(IFileListEntry file, MemoryStream msFile, string imageName)
        {
            try
            {
                var path = $"{_env.WebRootPath}\\uploads\\{imageName}";

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    msFile.WriteTo(fs);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

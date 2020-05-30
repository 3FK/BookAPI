using System;
using System.IO;
using System.Threading.Tasks;
using BlazorInputFile;

namespace Bookstore_UI.Contracts
{
    public interface IFileUpload
    {
        public Task UploadFile(IFileListEntry file, string imageName);
        public void UploadFile(IFileListEntry file, MemoryStream msFile, string imageName);
        public void RemoveFile(string imageName);
    }
}

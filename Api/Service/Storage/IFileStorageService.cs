namespace Api.Service.Storage
{
    public interface IFileStorageService
    {
        Task<bool> RemoveFileAsync(string fileName);
        Task<string> UploadFileAsync(IFormFile file);
    }
}
using Microsoft.AspNetCore.Http;

namespace FitnessProjecttt.Application.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file);

    Task<string> UploadVideoAsync(IFormFile file);
}
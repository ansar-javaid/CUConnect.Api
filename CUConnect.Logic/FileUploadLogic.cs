using CUConnect.Database.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;


namespace CUConnect.Logic
{
    public class FileUploadLogic : ControllerBase
    {
        private readonly IHostEnvironment _environment;
        public FileUploadLogic(IHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<(bool status,int totalfiles,double size)> Upload(List<IFormFile> files, Post post)
        {                  
                double size = 0;
                var rootPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "Resourses", "Document");
            if (!Directory.Exists(rootPath))
                    Directory.CreateDirectory(rootPath);
                using (var _dbContext = new CUConnectDBContext())
                {

                    foreach (var file in files)
                    {
                        string fileExtension = Path.GetExtension(file.FileName);
                        string[] split = file.ContentType.Split('/');
                        var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                        var filePath = Path.Combine(rootPath, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            var document = new Document
                            {
                                PostsId = post.PostId,
                                Name = uniqueFileName,
                                FamilyType = fileExtension,
                                MimeType = fileExtension,
                                Path = $"/Resourses/Document/{uniqueFileName}"
                            };
                            await file.CopyToAsync(stream);
                            _dbContext.Documents.Add(document);
                            await _dbContext.SaveChangesAsync();
                        }
                        size = +file.Length;
                    }
                    return (true, files.Count,size);
                }
            }


        public async Task<(bool status, double size)> Upload(IFormFile file, Profile profile)
        {
            double size = 0;
            var rootPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "Resourses", "Document");
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);
            using (var _dbContext = new CUConnectDBContext())
            {
                    string fileExtension = Path.GetExtension(file.FileName);
                    string[] split = file.ContentType.Split('/');
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                    var filePath = Path.Combine(rootPath, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        var document = new Document
                        {
                            ProfileId = profile.ProfileId,
                            Name = uniqueFileName,
                            FamilyType = fileExtension,
                            MimeType = fileExtension,
                            Path = $"/Resourses/Document/{uniqueFileName}"
                        };
                        await file.CopyToAsync(stream);
                        _dbContext.Documents.Add(document);
                        await _dbContext.SaveChangesAsync();
                    }
                    size = +file.Length;
                }
                return (true, size);
        }
    }
}

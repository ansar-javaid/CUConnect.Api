using CUConnect.Database.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace CUConnect.Logic
{
    public class FileUploadLogic : ControllerBase
    {
        private readonly IHostEnvironment _environment;
        public FileUploadLogic(IHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<(bool status, int totalFiles, double size)> Upload(List<IFormFile> files, Post post)
        {
            double size = 0;

            // Get the path to the root directory for storing documents
            var rootPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "Resources", "Document");

            // Create the directory if it does not exist
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            using (var _dbContext = new CUConnectDBContext())
            {
                foreach (var file in files)
                {
                    // Get the file extension and generate a unique file name
                    string fileExtension = Path.GetExtension(file.FileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                    // Construct the file path
                    var filePath = Path.Combine(rootPath, uniqueFileName);

                    if (IsImageFile(fileExtension))
                    {
                        // Load the image and resize it while preserving aspect ratio
                        using (var image = Image.Load(file.OpenReadStream()))
                        {
                            int targetWidth = 800;
                            int targetHeight = 800;

                            if (image.Width > image.Height)
                            {
                                targetHeight = (int)Math.Round((double)image.Height / image.Width * targetWidth);
                            }
                            else
                            {
                                targetWidth = (int)Math.Round((double)image.Width / image.Height * targetHeight);
                            }

                            // Resize the image and save it with a JPEG encoder
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Size = new Size(targetWidth, targetHeight),
                                Mode = ResizeMode.Crop
                            }));

                            using (var memoryStream = new MemoryStream())
                            {
                                image.Save(memoryStream, new JpegEncoder
                                {
                                    Quality = 80 // adjust the quality level here
                                });

                                // Save the compressed image to the file system
                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    memoryStream.Seek(0, SeekOrigin.Begin);
                                    await memoryStream.CopyToAsync(fileStream);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Copy the file to the file system without any modification
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }

                    // Create a new document object and save it to the database
                    var document = new Document
                    {
                        PostsId = post.PostId,
                        Name = uniqueFileName,
                        FamilyType = fileExtension,
                        MimeType = fileExtension,
                        Path = $"/Resources/Document/{uniqueFileName}"
                    };

                    _dbContext.Documents.Add(document);
                    await _dbContext.SaveChangesAsync();

                    // Update the total size of the uploaded files
                    size += new FileInfo(filePath).Length;
                }

                // Return the upload status, total number of files, and total size of the uploaded files
                return (true, files.Count, size);
            }
        }

        // Checks whether the given file extension corresponds to an image file
        private bool IsImageFile(string fileExtension)
        {
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            return imageExtensions.Contains(fileExtension.ToLowerInvariant());
        }







        public async Task<(bool status, double size)> Upload(IFormFile file, Profile profile)
        {
            double size = 0;
            var rootPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "Resources", "Document");
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

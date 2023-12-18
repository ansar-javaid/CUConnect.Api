using CUConnect.Database.Entities;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace CUConnect.Logic
{
    public class FileUploadLogic : ControllerBase
    {
        private readonly IHostEnvironment _environment;
        //FireBase Storage Variables
        private static readonly string _ApiKey = "AIzaSyA9t7uK2c4TeqPGDOPM1_bz8OH9V7xI5Yw";
        private static readonly string _Bucket = "ai-connect-c7217.appspot.com";
        private static readonly string _AuthEmail = "eno.j5566@gmail.com";
        private static readonly string _AuthPassword = "Cortana$5566";
        private static readonly string _FileBaseURL = "https://firebasestorage.googleapis.com/v0/b/ai-connect-c7217.appspot.com/o/";

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


            //Distributing the tasks
            var sizeReducerTasks = new List<Task>();
            Parallel.ForEach(files, file =>
            {
                var task = Task.Run(() => SizeReducer(file, rootPath, post, null));
                sizeReducerTasks.Add(task);
                size = +file.Length;
            });

            await Task.WhenAll(sizeReducerTasks);

            // Return the upload status, total number of files, and total size of the uploaded files
            return (true, files.Count, size);
        }

        // Checks whether the given file extension corresponds to an image file
        private static bool IsImageFile(string fileExtension)
        {
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            return imageExtensions.Contains(fileExtension.ToLowerInvariant());
        }


        #region FireBase Cloud Upload & Image Size Reducer
        private async Task SizeReducer(IFormFile file, string rootPath, Post? post, Profile? profile)
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
                            Quality = 90 // adjust the quality level here
                        });

                        // Save the compressed image to the file system
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            await memoryStream.CopyToAsync(fileStream);

                            //Fire Base Cloud Upload Code
                            {
                                // Reset the stream position
                                memoryStream.Seek(0, SeekOrigin.Begin);

                                var auth = new FirebaseAuthProvider(new FirebaseConfig(_ApiKey));
                                var a = await auth.SignInWithEmailAndPasswordAsync(_AuthEmail, _AuthPassword);

                                var cancellation = new CancellationTokenSource();

                                var task = new FirebaseStorage(
                                    _Bucket,
                                    new FirebaseStorageOptions
                                    {
                                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                                        ThrowOnCancel = true
                                    })
                                    .Child(uniqueFileName)
                                    .PutAsync(memoryStream, cancellation.Token);

                                task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");


                                try
                                {
                                    await task;
                                    // Delete the file from the local disk after a successful upload
                                    // Dispose of the FileStream when it's no longer needed
                                    memoryStream.Dispose();
                                    fileStream.Dispose();

                                    // Delete the file from the local disk after a successful upload
                                    System.IO.File.Delete(filePath);

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }

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
                    // Reset the stream position
                    fileStream.Seek(0, SeekOrigin.Begin);

                    var auth = new FirebaseAuthProvider(new FirebaseConfig(_ApiKey));
                    var a = await auth.SignInWithEmailAndPasswordAsync(_AuthEmail, _AuthPassword);

                    var cancellation = new CancellationTokenSource();

                    var task = new FirebaseStorage(
                        _Bucket,
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                            ThrowOnCancel = true
                        })
                        .Child(uniqueFileName)
                        .PutAsync(fileStream, cancellation.Token);

                    task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");


                    try
                    {
                        await task;
                        // Delete the file from the local disk after a successful upload
                        // Dispose of the FileStream when it's no longer needed
                        fileStream.Dispose();

                        // Delete the file from the local disk after a successful upload
                        System.IO.File.Delete(filePath);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }


            using (var _dbContext = new CUConnectDBContext())
            {
                if (post != null)
                {
                    // Create a new document object and save it to the database for Post
                    var document = new Document
                    {
                        PostsId = post.PostId,
                        Name = uniqueFileName,
                        FamilyType = fileExtension,
                        MimeType = fileExtension,
                        Path = _FileBaseURL + uniqueFileName + "?alt=media"
                    };

                    _dbContext.Documents.Add(document);
                    await _dbContext.SaveChangesAsync();
                }
                if (profile != null)
                {
                    // Create a new document object and save it to the database for Profile
                    var document = new Document
                    {
                        ProfileId = profile.ProfileId,
                        Name = uniqueFileName,
                        FamilyType = fileExtension,
                        MimeType = fileExtension,
                        Path = _FileBaseURL + uniqueFileName + "?alt=media"
                    };
                    _dbContext.Documents.Add(document);
                    await _dbContext.SaveChangesAsync();
                }
            }

            // Update the total size of the uploaded files
            //size += new FileInfo(filePath).Length;
        }
        #endregion

        #region Delete Files from Fire Storage
        public async Task<bool> DeleteFiles(List<string> fileNames)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(_ApiKey));
                var a = await auth.SignInWithEmailAndPasswordAsync(_AuthEmail, _AuthPassword);

                var storage = new FirebaseStorage(
                    _Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    });

                foreach (var fileName in fileNames)
                {
                    // Specify the path to the file you want to delete in the Firebase Storage bucket
                    var path = $"{fileName}";

                    // Delete the file
                    await storage.Child(path).DeleteAsync();
                }

                return true; // Deletion successful
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        #endregion



        public Task<(bool status, double size)> Upload(IFormFile file, Profile profile)
        {
            double size = 0;
            var rootPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "Resources", "Document");
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);
            // Calling for Profile
            SizeReducer(file, rootPath, null, profile).Wait();
            return Task.FromResult((true, size));
        }
    }

}

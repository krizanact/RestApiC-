using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Project.API.FileConfig
{
    public static class FileMethods
    {

        /// <summary>
        /// Method uploads file to our targeted directory
        /// </summary>
        /// <param name="_webHost">WebHost instance injected as a param</param>
        /// <param name="image"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static async Task<string> UploadImage(IWebHostEnvironment _webHost, IFormFile image, string folderName)
        {
            // Upload image file into "folderName" directory
            string uniqueFileName = FileMethods.GetUniqueFileName(image.FileName);
            string fullPath = Path.Combine(_webHost.WebRootPath, folderName, uniqueFileName);
            await image.CopyToAsync(new FileStream(fullPath, FileMode.Create));

            // Return unique saved filename
            return uniqueFileName;
        }

        /// <summary>
        /// Method deletes file from folder
        /// </summary>
        /// <param name="_webHost">IWebHostEnvironment _webHost param</param>
        /// <param name="fileName">Name of the file we want to delete</param>
        public static void DeleteFromFolder(IWebHostEnvironment _webHost, string folderName, string fileName)
        {
            string path = Path.Combine(_webHost.WebRootPath, folderName, fileName);
            if (File.Exists(path))
            {
                // Try to delete this file even if it fails continue running
                try
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    File.Delete(path);
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// Generates unique name for each file that is uploaded
        /// </summary>
        /// <returns>The unique file name.</returns>
        /// <param name="fileName">File name.</param>
        private static string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);

            var uniqueFileName = Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);

            // Replace all problematic characters
           

            byte[] tempBytes;
            tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(uniqueFileName);
            uniqueFileName = System.Text.Encoding.UTF8.GetString(tempBytes).Replace("?","-");

            return uniqueFileName;
        }

       

    }
}

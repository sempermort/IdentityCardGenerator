using IdentityCardGenerator.Interfaces;
using System;
using System.IO;

namespace IdentityCardGenerator.Services
{
    public class PhotoService: IPhotoService
    {
        public string GetPhotoPathForId(string idNumber, string photoFolderPath)
        {
            try
            {
                // Check if the photo folder exists
                if (!Directory.Exists(photoFolderPath))
                {
                    throw new DirectoryNotFoundException($"Photo folder not found: {photoFolderPath}");
                }

                // Look for photo files that match the ID number
                // This assumes photos are named with the ID number (e.g., "12345.jpg")
                var photoExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp" };
                
                foreach (var extension in photoExtensions)
                {
                    var photoPath = Path.Combine(photoFolderPath, idNumber + extension);
                    if (File.Exists(photoPath))
                    {
                        return photoPath;
                    }
                }

                // If not found by ID, try to find by name
                // This is a fallback approach if photos are named by person's name
                return FindPhotoByName(idNumber, photoFolderPath);
            }
            catch (Exception ex)
            {
                // Log exception or handle as appropriate for your application
                throw new Exception($"Error finding photo for ID {idNumber}: {ex.Message}", ex);
            }
        }


        public string FindPhotoByName(string idNumber, string photoFolderPath)
        {
            // This is a simplified implementation
            // In a real application, you might need more sophisticated name matching
            var photoFiles = Directory.GetFiles(photoFolderPath, $"{idNumber}*");
            
            if (photoFiles.Length > 0)
            {
                return photoFiles[0]; // Return the first match
            }

            return string.Empty; // No photo found
        }

        
    }
}
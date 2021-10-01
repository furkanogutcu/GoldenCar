using System;
using System.IO;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Http;
using MimeDetective;

namespace Core.Utilities.Helpers.FileHelper
{
    public class FileHelper
    {
        //Configuration
        private static readonly string RootDirectory = Environment.CurrentDirectory + "\\wwwroot";
        private static readonly FileCategory[] AcceptedFileCategories =
        {
            new FileCategory
            {
                FolderName = "images",
                ExtensionMimeType = RecognizedFileTypes.Images,
                MaximumUploadSizeInByte = ConvertMbToByte("5")
            }
        };

        public static IResult Upload(IFormFile file)
        {
            var requestShield = Shield(file);   //Shield method returns the ExtensionInformation object.
            if (!requestShield.Success)
            {
                return new ErrorResult(requestShield.Message);
            }

            var fileExtension = Path.GetExtension(file.FileName);
            var randomName = Guid.NewGuid().ToString();

            var extensionFolder = $"{requestShield.Data.FolderName}\\";

            var uploadFolder = $"{RootDirectory}\\{extensionFolder}";
            var fileName = randomName + fileExtension;
            var fileFullPath = uploadFolder + fileName;

            CheckAndCreateDirectory(uploadFolder);
            CreateFile(fileFullPath, file);
            var dbImagePath = extensionFolder + fileName;
            return new SuccessResult(dbImagePath.Replace("\\", "/"));
        }

        public static IResult Update(IFormFile file, string imagePath)
        {
            var requestShield = Shield(file);
            if (!requestShield.Success)
            {
                return new ErrorResult(requestShield.Message);
            }

            var fileExtension = Path.GetExtension(file.FileName);
            var randomName = Guid.NewGuid().ToString();

            var extensionFolder = $"{requestShield.Data.FolderName}\\";

            var uploadFolder = $"{RootDirectory}\\{extensionFolder}";
            var fileName = randomName + fileExtension;
            var fileFullPath = uploadFolder + fileName;

            Delete((RootDirectory + imagePath).Replace("/", "\\"));
            CheckAndCreateDirectory(uploadFolder);
            CreateFile(fileFullPath, file);
            var dbImagePath = extensionFolder + fileName;
            return new SuccessResult(dbImagePath.Replace("\\", "/"));
        }

        public static IResult Delete(string directory)
        {
            var newDirectory = $"{RootDirectory}\\{directory.Replace("/", "\\")}";
            if (File.Exists(newDirectory))
            {
                File.Delete(newDirectory);
                return new SuccessResult();
            }
            return new ErrorResult("Dosya bulunamadi");
        }

        //Private methods

        private static IDataResult<ExtensionInformation> Shield(IFormFile file)
        {
            var extensionInRequest = Path.GetExtension(file.FileName);
            var mimeTypeInRequest = file.ContentType;

            var extensionInformationResult = GetExtensionInformation(extensionInRequest);
            if (!extensionInformationResult.Success)
            {
                return new ErrorDataResult<ExtensionInformation>(extensionInformationResult.Message);
            }

            var fileSizeLimit = extensionInformationResult.Data.MaximumUploadSizeInByte;

            var sizeLimitResult = CheckIfFileSizeIsWithinLimits(file, fileSizeLimit);
            if (!sizeLimitResult.Success)
            {
                return new ErrorDataResult<ExtensionInformation>(sizeLimitResult.Message);
            }

            var detectMimeTypeResult = GetDetectedMimeType(file);
            if (!detectMimeTypeResult.Success)
            {
                return new ErrorDataResult<ExtensionInformation>(detectMimeTypeResult.Message);
            }

            var expectedMimeType = extensionInformationResult.Data.MimeType;
            var detectedMimeType = detectMimeTypeResult.Data;

            var isDetectedMimeTypeValid = CheckIfFileMimeTypeValid(detectedMimeType);
            if (!isDetectedMimeTypeValid.Success)
            {
                return new ErrorDataResult<ExtensionInformation>(isDetectedMimeTypeValid.Message);
            }

            var maliciousRequestCheck = MaliciousRequestDetection(mimeTypeInRequest, expectedMimeType, detectedMimeType);

            if (!maliciousRequestCheck.Success)
            {
                return new ErrorDataResult<ExtensionInformation>(maliciousRequestCheck.Message);
            }

            return new SuccessDataResult<ExtensionInformation>(extensionInformationResult.Data);

        }

        private static IResult MaliciousRequestDetection(string mimeTypeInRequest, string expectedMimeType, string detectedMimeType)
        {
            if (expectedMimeType != mimeTypeInRequest)
            {
                return new ErrorResult("Istekteki dosya uzantisi icin beklenen mime tipi ile istekteki mime tipi uyusmuyor");
            }
            if (detectedMimeType != mimeTypeInRequest)
            {
                return new ErrorResult("Dosyanin gercek mime tipi ile istekteki mime tipi uyusmuyor");
            }
            return new SuccessResult();
        }

        private static IResult CheckIfFileMimeTypeValid(string mimeType)
        {
            foreach (var acceptedFileCategory in AcceptedFileCategories)
            {
                foreach (var acceptedMimeType in acceptedFileCategory.ExtensionMimeType.Values)
                {
                    if (acceptedMimeType == mimeType)
                    {
                        return new SuccessResult();
                    }
                }
            }

            return new ErrorResult("Mime tipi gecersiz");
        }

        private static IResult CheckIfFileSizeIsWithinLimits(IFormFile file, double maximumSizeLimit)
        {
            if (file.Length > 0 && file.Length <= maximumSizeLimit)
            {
                return new SuccessResult();
            }
            var requstFileSize = Math.Round(ConvertByteToMb(file.Length.ToString()), 2);
            var maximumFileSize = Math.Round(ConvertByteToMb(maximumSizeLimit.ToString()), 2);
            return new ErrorResult($"Dosya boyutu cok fazla. Yuklenen dosyanin boyutu: {requstFileSize} MB - Kabul edilen en fazla dosya boyutu: {maximumFileSize} MB");
        }

        private static IDataResult<string> GetDetectedMimeType(IFormFile file)
        {
            var inspector = new ContentInspectorBuilder()
            {
                Definitions = MimeDetective.Definitions.Default.All()
            }.Build();

            byte[] content;

            using (FileStream fileStream = new FileStream(Path.GetTempFileName(), FileMode.Open, FileAccess.Write, FileShare.Read, 1024, FileOptions.None))
            {
                file.CopyTo(fileStream);
                var tempFilePath = fileStream.Name;
                fileStream.Close();
                content = ContentReader.Default.ReadFromFile(tempFilePath);
                File.Delete(tempFilePath);
            }

            var results = inspector.Inspect(content);
            var resultsByMimeType = results.ByMimeType();

            if (resultsByMimeType.IsEmpty)
            {
                return new ErrorDataResult<string>(null, "Mime tipi tespit edilemedi");
            }

            return new SuccessDataResult<string>(resultsByMimeType[0].MimeType, "Mime tipi tespit edildi");
        }

        private static IDataResult<ExtensionInformation> GetExtensionInformation(string extension)
        {
            var newExtension = extension.ToLower();

            foreach (var acceptedFileCategory in AcceptedFileCategories)
            {
                foreach (var acceptedExtensionMimeType in acceptedFileCategory.ExtensionMimeType)
                {
                    if (($".{acceptedExtensionMimeType.Key}") == extension)
                    {
                        ExtensionInformation extensionInformation = new ExtensionInformation
                        {
                            Extension = acceptedExtensionMimeType.Key,
                            MimeType = acceptedExtensionMimeType.Value,
                            FolderName = acceptedFileCategory.FolderName,
                            MaximumUploadSizeInByte = acceptedFileCategory.MaximumUploadSizeInByte
                        };
                        return new SuccessDataResult<ExtensionInformation>(extensionInformation);
                    }
                }
            }

            return new ErrorDataResult<ExtensionInformation>("Dosya türü desteklenmiyor");
        }

        private static double ConvertByteToMb(string byteSize)
        {
            var newByteSize = byteSize.Replace(".", ",");
            var size = Convert.ToDouble(newByteSize);
            return size / 1048576;
        }

        private static double ConvertMbToByte(string mbSize)
        {
            var newByteSize = mbSize.Replace(".", ",");
            var size = Convert.ToDouble(newByteSize);
            return size * 1048576;
        }

        private static void CheckAndCreateDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static void CreateFile(string directory, IFormFile file)
        {
            using (FileStream fs = File.Create(directory))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
        }
    }
}
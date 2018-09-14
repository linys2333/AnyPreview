using NETCore.Encrypt.Extensions;
using System;
using System.Text.RegularExpressions;

namespace AnyPreview.Service.Dtos
{
    public class OSSObjectDto
    {
        public OSSObjectDto(string bucket, string filePath)
        {
            OSSPath = $"oss://{bucket}/{filePath}";
            HashPath = OSSPath.ToLower().HMACSHA1("");
            Bucket = bucket;
            FilePath = filePath;
        }

        public OSSObjectDto(string ossPath)
        {
            OSSPath = ossPath;
            HashPath = OSSPath.ToLower().HMACSHA1("salt");
            Bucket = Regex.Match(OSSPath, "(?<=^oss://)[^/]+?(?=/)").Value;
            FilePath = Regex.Match(OSSPath, "(?<=^oss://.+?/).+$").Value;
        }
        
        public string OSSPath { get; set; }

        public string FileType { get; set; }

        public string Bucket { get; set; }

        public string FilePath { get; set; }

        public string HashPath { get; }

        public string IMMKey
        {
            get
            {
                if (string.IsNullOrEmpty(FileType))
                {
                    throw new ArgumentNullException(nameof(FileType));
                }
                return $"{HashPath}_{FileType.ToLower()}";
            }
        }

        public bool IsHtml => FileType?.IndexOf("htm") == 0;
    }
}

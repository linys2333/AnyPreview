using System.ComponentModel.DataAnnotations;

namespace AnyPreview.Portal.Dtos
{
    public class GenerateRequest
    {
        private string m_HashOSSPath;

        /// <summary>
        /// 原始文档的OSS路径
        /// </summary>
        [Required, RegularExpression("^oss://[^/]+/.+$")]
        public string OSSPath { get; set; }

        /// <summary>
        /// 是否重新生成，默认false
        /// </summary>
        public bool IsRegenerate { get; set; }
    }
}

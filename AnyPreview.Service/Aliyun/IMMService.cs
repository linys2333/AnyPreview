using Aliyun.Acs.Core.Http;
using AnyPreview.Core.Aliyun;
using AnyPreview.Service.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;
using Newtonsoft.Json;
using System;

namespace AnyPreview.Service.Aliyun
{
    public class IMMService
    {
        private readonly IMMClient m_IMMClient;
        private readonly ILogger m_Logger;
        private readonly IMMSetting m_IMMSetting;

        public IMMService(IMMClient immClient, ILogger logger, IMMSetting immSetting)
        {
            m_IMMClient = immClient;
            m_Logger = logger;
            m_IMMSetting = immSetting;
        }

        public DocumentConvertResultDto Convert(OSSObjectDto ossObjectDto)
        {
            Requires.NotNull(ossObjectDto, nameof(ossObjectDto));

            var request = new CreateOfficeConversionTaskRequest
            {
                AcceptFormat = FormatType.JSON,
                Project = m_IMMSetting.Project,
                SrcUri = ossObjectDto.OSSPath,
                SrcType = ossObjectDto.FileType,
                TgtType = m_IMMSetting.ConvertType,
                TgtUri = m_IMMSetting.GetTgtUri(ossObjectDto.IMMKey)
            };

            try
            {
                var task = m_IMMClient.GetAcsResponse(request);
                if (task.IsFail)
                {
                    m_Logger.LogError($"文档转换失败：{JsonConvert.SerializeObject(task)}");
                }
                return new DocumentConvertResultDto(task);
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "文档转换错误");
                return null;
            }
        }

        public DocumentConvertResultDto Query(string taskId)
        {
            Requires.NotNullOrEmpty(taskId, nameof(taskId));

            var request = new GetOfficeConversionTaskRequest
            {
                AcceptFormat = FormatType.JSON,
                Project = m_IMMSetting.Project,
                TaskId = taskId,
            };

            try
            {
                var task = m_IMMClient.GetAcsResponse(request);
                if (task.IsFail)
                {
                    m_Logger.LogError($"文档转换失败：{JsonConvert.SerializeObject(task)}");
                }
                return new DocumentConvertResultDto(task);
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, "查询文档转换任务错误");
                return null;
            }
        }
    }
}

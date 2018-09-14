using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.imm.Model.V20170906;
using AnyPreview.Core.Aliyun;
using AnyPreview.Core.Settings;
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

        public DocConvertResultDto Convert(OSSObjectDto ossObjectDto)
        {
            Requires.NotNull(ossObjectDto, nameof(ossObjectDto));

            var request = new ConvertOfficeFormatRequest
            {
                AcceptFormat = FormatType.JSON,
                Project = m_IMMSetting.Project,
                SrcUri = ossObjectDto.OSSPath,
                SrcType = ossObjectDto.FileType,
                TgtType = m_IMMSetting.ConvertType,
                TgtUri = m_IMMSetting.GetTgtUri(ossObjectDto.IMMKey)
            };

            var task = Request(request);
            if (task == null)
            {
                return null;
            }

            var result = new DocConvertResultDto(task);
            if (result.Status == DocConvertStatus.Failed)
            {
                m_Logger.LogWarning($"文档转换失败：{JsonConvert.SerializeObject(task)}");
            }

            return result;
        }

        public DocConvertResultDto CreateConvertTask(OSSObjectDto ossObjectDto)
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

            var task = Request(request);
            if (task == null)
            {
                return null;
            }

            var result = new DocConvertResultDto(task);
            if (result.Status == DocConvertStatus.Failed)
            {
                m_Logger.LogWarning($"创建转换任务失败：{JsonConvert.SerializeObject(task)}");
            }

            return result;
        }

        public DocConvertResultDto QueryConvertTask(string taskId)
        {
            Requires.NotNullOrEmpty(taskId, nameof(taskId));

            var request = new GetOfficeConversionTaskRequest
            {
                AcceptFormat = FormatType.JSON,
                Project = m_IMMSetting.Project,
                TaskId = taskId
            };
            
            var task = Request(request);
            if (task == null)
            {
                return null;
            }

            var result = new DocConvertResultDto(task);
            if (result.Status == DocConvertStatus.Failed)
            {
                m_Logger.LogWarning($"文档转换失败：{JsonConvert.SerializeObject(task)}");
            }

            return result;
        }

        private T Request<T>(AcsRequest<T> request) where T : AcsResponse
        {
            try
            {
                return m_IMMClient.GetAcsResponse(request);
            }
            catch (Exception ex)
            {
                m_Logger.LogError(ex, $"{request.ActionName}异常：{JsonConvert.SerializeObject(request)}");
                return null;
            }
        }
    }
}

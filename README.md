## 一些说明

> 对接阿里云智能媒体管理（IMM）服务，实现文档在线预览

> 依赖阿里云OSS服务

> 简单来说，就是将要预览的文档（OSS对象），通过IMM服务先进行一道转换，生成vector类型文件（一系列OSS对象），然后通过官方js渲染引擎实现预览

****

## 入坑

## &sect; 智能媒体管理

#### 1、官方文档

 * [文档转换/预览功能介绍](https://yq.aliyun.com/articles/581576?spm=a2c4e.11153940.blogcont589902.28.5e907f2e4JsJnY)
 * [快速入门及API](https://help.aliyun.com/product/62354.html?spm=a2c4g.11186623.3.1.Z1wO8H)
 * [.Net SDK](https://develop.aliyun.com/tools/sdk#/dotnet)

#### 2、要点说明

 * 文档转换及预览

    1、

    * [CreateOfficeConversionTask](https://help.aliyun.com/document_detail/72044.html?spm=a2c4g.11186623.6.584.UQN3Ey)

 * STS Token

    1、

    * [AssumeRole](https://help.aliyun.com/document_detail/28763.html?spm=a2c4g.11186623.6.682.CGHj83)
    * [使用STS报错](https://help.aliyun.com/knowledge_detail/39744.html?spm=a2c4g.11186623.6.739.t8kZIQ)

 * SDK
    
    1、


#### 3、参考资料

 * 


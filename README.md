## 一些说明

> 对接阿里云智能媒体管理（IMM）服务，实现文档在线预览

> 依赖阿里云OSS服务

> 简单来说，就是将要预览的文档（OSS对象），通过IMM服务先进行一道转换，生成vector类型文件（一系列OSS对象），然后通过官方js渲染引擎实现预览

> 话说，阿里云的产品还真是多到爆啊。。。

****

## 入坑

## &sect; 智能媒体管理

#### 1、官方文档

 * [文档转换/预览功能介绍](https://yq.aliyun.com/articles/581576?spm=a2c4e.11153940.blogcont589902.28.5e907f2e4JsJnY)
 * [快速入门及API](https://help.aliyun.com/product/62354.html?spm=a2c4g.11186623.3.1.Z1wO8H)
 * [.Net SDK](https://develop.aliyun.com/tools/sdk#/dotnet)
 * [SDK源码](https://github.com/aliyun/aliyun-openapi-net-sdk)

#### 2、要点说明

 * 文档转换及预览

    1、SrcUri参数格式：oss://{bucket}/{prefix}/{key}，指向源OSS对象

    2、CreateOfficeConversionTask接口是异步的，每次调用返回状态都是Running或Failed，并没有文档中的Finished状态，只有在GetOfficeConversionTask接口中才能获取到完整的3个状态

    3、~~前端预览引擎直接放到目标桶中，其文件权限均设为公共读，目录结构：~~
    ```
    {bucket}/
    [├── {prefix}/]
        ├── preview/
            ├── index.html
            ├── latest/
                ├── bundle/
                ├── extends/
                ...
    ```

    4、最终预览Url格式：https://preview.imm.aliyun.com/index.html?url=oss://{bucket}/[{prefix}/]{TgtUri}&accessKeyId=xxx&accessKeySecret=xxx&stsToken=UrlEncode(xxx)&region=oss-{region}&bucket={bucket}&cope=[0|1]&...

    5、注：已新增同步接口（ConvertOfficeFormat），但只支持转换时间在5s的任务。另外阿里云对预览引擎做了统一升级，详见：[https://help.aliyun.com/document_detail/74947.html?spm=a2c4g.11186623.2.3.31412902XQKVju](https://help.aliyun.com/document_detail/74947.html?spm=a2c4g.11186623.2.3.31412902XQKVju)

    * [CreateOfficeConversionTask](https://help.aliyun.com/document_detail/86147.html?spm=a2c4g.11186623.6.587.366711a346Lp6e)
    
    * [ConvertOfficeFormat](https://help.aliyun.com/document_detail/72044.html?spm=a2c4g.11186623.6.584.UQN3Ey)

    * [前端预览引擎](https://imm-demo.oss-cn-shanghai.aliyuncs.com/formatconvert/preview/V2.0.0_20180427.zip?spm=a2c4e.11153940.blogcont589902.24.15407f2eE7kJHt&file=V2.0.0_20180427.zip)

 * STS Token

    1、主要用到AssumeRole接口，该接口作用是临时（900~3600s）获取指定角色的指定权限

    2、在RAM权限管理那配置文档预览角色的策略，赋予该角色文档预览目录的读权限（不然最后一步预览会报403的）。然后在实际调用时，可通过Policy参数获取文档预览目录下的指定文档的读权限
    
    3、记得赋予子账号AliyunSTSAssumeRoleAccess权限，否则无法调用AssumeRole接口

    4、最后临时凭证获得的权限是角色的权限和传入的Policy的交集

    * [STS实践](https://help.aliyun.com/document_detail/31935.html?spm=a2c4g.11186623.2.6.StbfA6)
    * [AssumeRole](https://help.aliyun.com/document_detail/28763.html?spm=a2c4g.11186623.6.682.CGHj83)
    * [使用STS报错](https://help.aliyun.com/knowledge_detail/39744.html?spm=a2c4g.11186623.6.739.t8kZIQ)
    * [RAM Policy Editor](http://gosspublic.alicdn.com/ram-policy-editor/index.html?spm=a2c4g.11186623.2.17.Q3lh2B)

 * SDK
    
    1、官方源码中的文件夹名字就是nuget包名，~~可惜OSS和IMM目前都还不支持~~

    2、不过翻了github上的源码可以发现，调用阿里云API的核心类库（aliyun-net-sdk-core）其实都有了，公共类参数基本不用自定义，缺的就是部分产品API请求相关的对象。于是要实现IMM API请求，只需要基于官方基类稍稍扩展一下就OK了~

    3、OSS的API比较特殊，不能像上面那样扩展，就找了个第三方包（NetCorePal.Aliyun.OSS.SDK），用着还不错

    4、注：OSS和IMM SDK官方已更新
    
    * [https://github.com/aliyun/aliyun-oss-csharp-sdk](https://github.com/aliyun/aliyun-oss-csharp-sdk)，坑爹的nuget包还是只到2.8版，不支持 .Net Core2，所以需要手动编译引用dll

    * [https://github.com/aliyun/aliyun-openapi-net-sdk/tree/master/aliyun-net-sdk-imm](https://github.com/aliyun/aliyun-openapi-net-sdk/tree/master/aliyun-net-sdk-imm)

 * 缺陷

    1、IMM目前还是个新产品，对html文档的转换支持很差。由于预览引擎不允许访问外部链接，所以导致hmtl中外部js、css、图片等均无法加载，样式失真严重

    2、所以html文档，不建议走IMM服务，可以考虑直接交给浏览器去加载

    3、另外对mht文件也不支持，如果有mht转换成word文档的，也是容易预览失败的

 * 遇到的问题

    1、最主要的就是这个问题，预览提示：“服务器异常，请联系管理员”

     ![服务器异常](./Doc/服务器异常.jpg)
     
    可以从以下几个方面入手排查

    * 转换失败：检查下转换后的oss文档，看是否完整；检查转成Task是否失败
    * 路径错误：检查下预览url中的路径是否正确
    * STSToken策略缺失：看下预览的响应有没有拒绝访问之类的错误，再检查token对应角色的策略设置

#### 3、参考资料

 * [文档预览功能使用技巧](https://yq.aliyun.com/articles/609835?spm=a2c4e.11153940.blogrightarea610103.14.4c5c6c05pHSmIn)
 * [OSS访问控制](https://help.aliyun.com/document_detail/31867.html?spm=a2c4g.11186623.4.6.H90iTA)
 * [OSS权限问题排查](https://www.alibabacloud.com/help/zh/doc-detail/42777.htm)
 * [Policy语法结构](https://help.aliyun.com/document_detail/28664.html?spm=a2c4g.11186623.2.1.ncUFzL)


﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AnyPreview.Core.Common
{
    public static class CommonConstants
    {
        public static class Resources
        {
            public static Dictionary<string, string> ContentTypeDict = new Dictionary<string, string>();
            public static Dictionary<string, string> IMMFileTypeDict = new Dictionary<string, string>();
        }

        public static class DateTimeFormatter
        {
            public static readonly string HyphenLongDateTime = "yyyy-MM-dd HH:mm:ss";
        }

        public static class Regexs
        {
            public static readonly Regex ContentDispositionFileNameRegex =
                new Regex(@"(?<=\bfilename=""?)[^""]+?(?=""|;|$)", RegexOptions.Compiled);
        }
    }
}

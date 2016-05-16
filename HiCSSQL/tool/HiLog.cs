using System;

namespace HiCSSQL
{
    /// <summary>
    /// 日志
    /// </summary>
    public class HiLog
    {
        public delegate void OnLOG(string script);
        private static OnLOG onlog = null;
        public static void SetLogFun(OnLOG logfun)
        {
            onlog = logfun;
        }

        public static void Write(string script)
        {
            if (onlog != null)
            {
                onlog(script);
            }
        }
        public static void Write(string format, params string[] lst)
        {
            if (onlog != null)
            {
                onlog(string.Format(format, lst));
            }
        }
        public static void Write(string format, params object[] lst)
        {
            if (onlog != null)
            {
                onlog(string.Format(format, lst));
            }
        }
        public static void Write(string format, params int[] lst)
        {
            if (onlog != null)
            {
                onlog(string.Format(format, lst));
            }
        }
    }
}

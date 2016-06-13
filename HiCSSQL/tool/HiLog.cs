using System;

namespace HiCSSQL
{
    /// <summary>
    /// 日志
    /// </summary>
    public class HiLog
    {
        private static Action<string> onlog = null;
        public static void SetLogFun(Action<string> logfun)
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
                if (lst == null)
                {
                    onlog(format);
                }
                onlog(string.Format(format, lst));
            }
        }
        public static void Write(string format, params object[] lst)
        {
            if (onlog != null)
            {
                if (lst == null)
                {
                    onlog(format);
                }
                onlog(string.Format(format, lst));
            }
        }
        public static void Write(string format, params int[] lst)
        {
            if (onlog != null)
            {
                if (lst == null)
                {
                    onlog(format);
                }
                onlog(string.Format(format, lst));
            }
        }
    }
}

using System;

namespace HiCSSQL
{
    internal class HiLog
    {
        private static SQLProxy.OnLOG onlog = null;
        public static void SetLogFun(SQLProxy.OnLOG logfun)
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

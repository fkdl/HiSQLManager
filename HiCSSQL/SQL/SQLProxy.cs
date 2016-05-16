using System;
using System.Collections.Generic;

namespace HiCSSQL
{
    /// <summary>
    /// SQL访问代理
    /// </summary>
    public sealed class SQLProxy
    {
        static CachMng<SQLData> mng = new CachMng<SQLData>();

        /// <summary>
        /// 加载存储SQL XML文件的文件夹。
        /// </summary>
        /// <param name="folder"></param>
        public static void LoadXMLs(string folder)
        {
            mng.ParseEvt = SQLData.Parse;
            mng.LoadXMLsByFolder(folder);
        }

        /// <summary>
        /// 根据主键取得SQL的相关信息
        /// </summary>
        /// <param name="key">SQL主键</param>
        /// <param name="handler">获得SQL的函数</param>
        /// <returns></returns>
        public static SQLData GetValue(string key)
        {
            return mng.GetValue(key);
        }
    }
}

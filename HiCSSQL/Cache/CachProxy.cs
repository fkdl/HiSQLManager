using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiCSSQL
{
    public sealed class CachProxy<T> where T: class, ICachItem, new()
    {
        /// <summary>
        /// 加载存储SQL XML文件的文件夹。
        /// </summary>
        /// <param name="folder"></param>
        public void LoadXMLs(string folder)
        {
            mng.LoadXMLsByFolder(folder);
        }

        /// <summary>
        /// 根据主键取得SQL的相关信息
        /// </summary>
        /// <param name="key">SQL主键</param>
        /// <param name="handler">获得SQL的函数</param>
        /// <returns></returns>
        public T GetValue(string key)
        {
            return mng.GetValue(key);
        }

        CachMng<T> mng = new CachMng<T>();
    }
}

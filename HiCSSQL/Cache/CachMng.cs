using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace HiCSSQL
{
    /// <summary>
    /// SQL信息的管理类。
    /// 用户可以通过本类中的方法取得与SQL ID相对应的SqlInfo对象。
    /// 
    /// 
    /// 1：当系统启动的时候，或存储SQL信息的配置文件改动的时候，系统会读取配置文件中的SQL信息并存储到内存中。
    /// 2：调用的时候，从内存中根据SQL信息的ID读取适合的SQL对象。
    /// 3：SQL对象根据调用时候提供的数值，组合成可以执行的SQLInfo对象。
    /// 4：存储SQL信息的对象为T对象，T对象包括SQL的ID，SQL语句，以及执行SQL语句需要的多个参数对象。
    /// </summary>
    internal class CachMng<T> where T : ICachItem, new()
    {
        /// <summary>
        /// 最后更新时间。
        /// </summary>
        private DateTime lastUpdateTime;

        /// <summary>
        /// 存储SQL对象的集合。
        /// </summary>
        private Dictionary<string, CachData<T>> sqlDct = new Dictionary<string, CachData<T>>();
        string folder = "";

        public void LoadXMLsByFolder(string path)
        {
            this.folder = path;
            if (!Directory.Exists(path))
            {
                HiLog.Write("\"{0}\" folder is not exist,please check you xml folder.", path);
                throw new Exception(string.Format("\"{0}\" folder is not exist,please check you xml folder.", path));
            }
            ReadXMLFiles(path);
            lastUpdateTime = Directory.GetLastWriteTime(path);
        }

        private bool IsFolderChanged(string path)
        {
            DateTime dt = Directory.GetLastWriteTime(path);
            if (dt <= lastUpdateTime)
            {
                return false;
            }
            else
            {
                lastUpdateTime = dt;
                return true;
            }
        }

        private void ReadXMLFiles(string path)
        {
            sqlDct.Clear();
            string[] files = Directory.GetFiles(path);
            int index = 0;
            foreach (string it in files)
            {
                if (!it.ToLower().EndsWith(".xml"))
                {
                    continue;
                }
                index++;
                ReadXMLFile(it, sqlDct);
            }

            if (index < 1)
            {
                HiLog.Write("folder ({0}) not include xml files", path);
                throw new Exception(string.Format("folder ({0}) not include xml files", path));
            }
        }

        /// <summary>
        /// 从配置文件中读取SQL信息
        /// </summary>
        private void ReadXMLFile(string file, IDictionary<string, CachData<T>> dic)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            
            XmlNode node = doc.DocumentElement;
            if (node == null)
            {
                return;
            }

            foreach (XmlNode child in node.ChildNodes)
            {
                // 如果是注释
                if (XmlNodeType.Comment == child.NodeType)
                {
                    continue;
                }
                foreach (XmlNode child2 in child.ChildNodes)
                {
                    // 如果是注释
                    if (XmlNodeType.Comment == child2.NodeType)
                    {
                        continue;
                    }

                    XmlAttributeCollection ndAtt = child2.Attributes;

                    if (ndAtt["id"] == null || ndAtt["id"].Value == null || ndAtt["id"].Value.Trim().Length < 1)
                    {
                        continue;
                    }

                    CachData<T> data = new CachData<T>();
                    if (!data.Parse(child2))
                    {
                        continue;
                    }
                    CachData<T> item = null;
                    bool isFind = dic.TryGetValue(data.ID, out item);
                    if (isFind)
                    {
                        HiLog.Write("key:{0} is alread in file({1}), so  in file({2}) sencond time is error", data.ID, item.File, file);
                        throw new Exception(string.Format("key:{0} is alread in file({1}), so  in file({2}) sencond time is error", data.ID, data.File, file));
                    }
                    data.File = file;
                    dic.Add(data.ID, data);
                }
            }
        }

        /// <summary>
        /// 存储SQL对象的集合。
        /// </summary>
        public Dictionary<string, CachData<T>> SQLDct
        {
            get
            {
                if (IsFolderChanged(folder))
                {
                    ReadXMLFiles(folder);
                }

                return sqlDct;
            }
        }
    }
}

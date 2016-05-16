using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace HiCSSQL
{
    public  delegate T ParseCallBack<T>(XmlNode node) where T: class;
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
    internal class CachMng<T> where T : class
    {
        /// <summary>
        /// 最后更新时间。
        /// </summary>
        private DateTime lastUpdateTime = DateTime.Now.AddYears(-1);

        /// <summary>
        /// 存储SQL对象的集合。
        /// </summary>
        private Dictionary<string, CachData<T>> sqlDct = new Dictionary<string, CachData<T>>();
        string folder = "";
        private List<string> files = new List<string>();
        public void LoadXMLsByFolder(string path)
        {
            if (ParseEvt == null)
            {
                HiLog.Write("not set parse callback, so can't load xml");
                throw new Exception("not set parse callback, so can't load xml");
            }
            this.folder = path;
            if (!Directory.Exists(path))
            {
                HiLog.Write("\"{0}\" folder is not exist,please check you xml folder.", path);
                throw new Exception(string.Format("\"{0}\" folder is not exist,please check you xml folder.", path));
            }
            ReadXMLFiles(path);
            lastUpdateTime = GetLastTime();
        }

        public ParseCallBack<T> ParseEvt { set; get; }

        private bool IsFolderChanged(string path)
        {
            DateTime dt = GetLastTime();
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

        private DateTime GetLastTime()
        {
            string[] files = Directory.GetFiles(folder);

            DateTime dt = lastUpdateTime;
            foreach (string it in files)
            {
                DateTime t = File.GetLastWriteTime(it);
                if (t > dt)
                {
                    dt = t;
                }
            }
            return dt;
        }

        private void ReadXMLFiles(string path)
        {
            files.Clear();
            sqlDct.Clear();
            string[] fls = Directory.GetFiles(path);
            foreach (string it in fls)
            {
                if (!it.ToLower().EndsWith(".xml"))
                {
                    continue;
                }

                files.Add(it);
            }

            if (files.Count < 1)
            {
                HiLog.Write("folder ({0}) not include xml files", path);
                throw new Exception(string.Format("folder ({0}) not include xml files", path));
            }

            foreach(string it in files)
            {
                ReadXMLFile(it, sqlDct);
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

                    string id = ndAtt["id"].Value;
                    CachData<T> item = null;
                    bool isFind = dic.TryGetValue(id, out item);
                    if (isFind)
                    {
                        HiLog.Write("key:{0} is alread in file({1}), so  in file({2}) sencond time is error", id, item.File, file);
                        throw new Exception(string.Format("key:{0} is alread in file({1}), so  in file({2}) sencond time is error", id, item.File, file));
                    }

                    if (ParseEvt == null)
                    {
                        break;
                    }
                    T data = ParseEvt(child2);
                    if (data == null)
                    {
                        continue;
                    }
                    item = new CachData<T>();
                    item.File = file;
                    item.Data = data;
                    dic.Add(id, item);
                }
            }
        }

        /// <summary>
        /// 取得存储的值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T  GetValue(string id)
        {
            CachData<T> data = null;
            if (!SQLDct.TryGetValue(id, out data))
            {
                return default(T);
            }

            return data.Data;
        }

        /// <summary>
        /// 存储SQL对象的集合。
        /// </summary>
        private Dictionary<string, CachData<T>> SQLDct
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

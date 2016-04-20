using System;
using System.Xml;

namespace HiCSSQL
{
    /// <summary>
    /// 缓存的XML信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class CachData<T> where T : ICachItem, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CachData()
        {
            Data = new T();
        }

        /// <summary>
        /// 存储所在文件
        /// </summary>
        public string File { set; get; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { set; get; }

        /// <summary>
        /// 存储的对象
        /// </summary>
        public T Data { set; get; }

        public bool Parse(XmlNode node)
        {
            // 如果是注释
            if (XmlNodeType.Comment == node.NodeType)
            {
                return false;
            }

            XmlAttributeCollection ndAtt = node.Attributes;

            this.ID = ndAtt["id"].Value;
            Data = new T();
            if (!Data.Parse(node))
            {
                return false;
            }
            return true;
        }
    }
}

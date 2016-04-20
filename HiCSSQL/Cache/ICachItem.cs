using System;
using System.Xml;

namespace HiCSSQL
{
    /// <summary>
    /// 缓存对象(外部可见)
    /// </summary>
    public interface ICachItem
    {
        /// <summary>
        /// 根据XML节点解析对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool Parse(XmlNode node);
    }
}

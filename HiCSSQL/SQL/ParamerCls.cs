using System;
using System.Xml;

namespace HiCSSQL
{
    internal class ParamerCls
    {
        public string ParamerName;
        public string ParamerText;
        public bool IsOutParamer = false;

        /// <summary>
        /// 构造函数。（读取配置文件）
        /// </summary>
        /// <param name="node">配置文件节点</param>
        public ParamerCls(XmlNode node)
        {
            XmlAttributeCollection ndAtt = node.Attributes;
            this.ParamerName = ndAtt["name"].Value;
            this.ParamerText = ndAtt["value"].Value;

            if (ndAtt["isOut"] != null && ndAtt["isOut"].Value.ToLower() == "true")
            {
                this.IsOutParamer = true;
            }
        }
    }
}

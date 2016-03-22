using System;
using System.Collections.Generic;
using System.Xml;

namespace HiCSSQL
{
    /// <summary>
        /// SQL配置文件中，某个SQL ID对应的类。本类中存储着某个SQL ID对应对象的结构。
        /// 包括SQL语句，数据库类型，参数信息等。
        /// </summary>
    internal class SQLData
    {
        private const string SQL_SERVER = "sqlserver";
        private const string ORACLE = "oracle";
        private const string OTHER = "other";

        /// <summary>
        /// 存储参数信息的集合。
        /// </summary>
        public Dictionary<string, ParamerCls> paramDict = new Dictionary<string, ParamerCls>();

        public SQLData(XmlNode node)
        {
            // 如果是注释
            if (XmlNodeType.Comment == node.NodeType)
            {
                return;
            }

            XmlAttributeCollection ndAtt = node.Attributes;

            this.id = ndAtt["id"].Value;
            if (ndAtt["type"] != null && ndAtt["type"].Value != null)
            {
                this.sqlType = ndAtt["type"].Value.ToLower();
            }

            ParamerCls paramerCls;
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name.ToUpper() == "TEXT")
                {
                    this.sql = child.InnerText.Replace("\r\n", "").Replace("\t", " ").Replace("  ", " ").Trim();
                    continue;
                }

                if (child.Name.ToUpper() == "PARAMERS")
                {
                    foreach (XmlNode childPar in child)
                    {
                        paramerCls = new ParamerCls(childPar);
                        if (paramerCls.ParamerName != null)
                        {
                            paramDict.Add(paramerCls.ParamerText, paramerCls);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// SQL语句的ID。
        /// </summary>
        private string id;

        /// <summary>
        /// SQL语句
        /// </summary>
        private string sql;

        /// <summary>
        /// 数据库类型
        /// </summary>
        private string sqlType;

        public string File { get; set; }

        public string ID
        {
            get
            {
                return this.id;
            }
        }

        public string SQL
        {
            get
            {
                return this.sql;
            }
        }

        /// <summary>
        /// 数据库类型。
        /// </summary>
        public string SqlType
        {
            get
            {
                if (sqlType == null)
                {
                    return SQLData.SQL_SERVER;
                }
                else
                {
                    return this.sqlType;
                }
            }
        }
    }
}

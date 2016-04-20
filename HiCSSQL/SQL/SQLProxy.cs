﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.OracleClient;
using MySql.Data.MySqlClient;

namespace HiCSSQL
{
    /// <summary>
    /// SQL访问代理
    /// </summary>
    public sealed class SQLProxy
    {
        public delegate DbParameter OnCreateParamHandler();
        public delegate void OnLOG(string script);
        static Dictionary<string, OnCreateParamHandler> handlers = new Dictionary<string, OnCreateParamHandler>();
        static CachMng<SQLData> mng = new CachMng<SQLData>();

        /// <summary>
        /// 加载存储SQL XML文件的文件夹。
        /// </summary>
        /// <param name="folder"></param>
        public static void LoadXMLs(string folder)
        {
            mng.LoadXMLsByFolder(folder);
        }

        /// <summary>
        /// 添加特殊类型数据库创建SQL参数的函数。
        /// SQLServer，Oracle，MySQL默认支持，不需要额外提供
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="createHandler">创建参数的函数</param>
        public static void AddParamFun(string dbType, OnCreateParamHandler createHandler)
        {
            dbType = dbType.ToLower();
            handlers[dbType] = createHandler;
        }

        /// <summary>
        /// 根据属性名称，取得对应的值。
        /// </summary>
        /// <param name="objVal">赋值对象</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>true:属性存在；false:属性不存在</returns>
        public delegate bool OnGetObjectHandler(string propertyName, ref object objVal);

        /// <summary>
        /// 根据主键取得SQL的相关信息
        /// </summary>
        /// <param name="key">SQL主键</param>
        /// <param name="handler">获得SQL的函数</param>
        /// <returns></returns>
        public static SqlInfo GetSqlInfo(string key, OnGetObjectHandler handler = null)
        {
            CachData<SQLData> data = null;
            if (!mng.SQLDct.TryGetValue(key, out data))
            {
                return null;
            }

            SqlInfo info = new SqlInfo(data.Data.SQL);
            if (data.Data.paramDict.Count < 1)
            {
                return info;
            }

            if (handler == null)
            {
                throw new Exception(string.Format("sql where id({0}) need paramers,but not set OnGetObjectHandler object", key));
            }

            info.Parameters = new DbParameter[data.Data.paramDict.Count];
            int index = 0;
            foreach (var it in data.Data.paramDict)
            {
                object val = null;
                if (!handler(it.Value.ParamerText, ref val))
                {
                    throw new Exception(string.Format("sql where id({0}) get param({1}) value failed", key, it.Value.ParamerText));
                }

                if (val == null)
                {
                    val = DBNull.Value;
                }
                info.Parameters[index] = CreateParamer(data.Data.SqlType, it.Value.ParamerName, val, it.Value.IsOutParamer);
                if (it.Value.IsOutParamer)
                {
                    info.Parameters[index].Direction = ParameterDirection.InputOutput;
                }

                index++;
            }
            return info;
        }

        private static DbParameter CreateParamer(string type, string key, object val, bool isOut)
        {
            DbParameter param = null;
            if (type == "")
            {
                param = new SqlParameter();
            }
            if (type == "sqlserver")
            {
                param = new SqlParameter();
            }
            if (type == "oracle")
            {
                param = new OracleParameter();
            }
            if (type == "mysql")
            {
                param = new MySqlParameter();
            }
            if (type == "oledb")
            {
                param = new OleDbParameter();
            }
            OnCreateParamHandler handler = null;
            if (handlers.TryGetValue(type, out handler))
            {
                param = handler();
            }
            else
            {
                throw new Exception(string.Format("database type({0}) not support", type));
            }

            param.ParameterName = key;

            if (!isOut)
            {
                param.Value = val;
            }
            if (isOut)
            {
                param.Direction = ParameterDirection.Output;
            }

            return param;
        }

        public static void SetLog(OnLOG evt)
        {
            HiLog.SetLogFun(evt);
        }
    }
}
using System;
using System.Data;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HiCSSQL;

namespace HiCSSQLTest
{
    [TestClass]
    public class UnitTestSQLProxy
    {
        string folder = "";
        public UnitTestSQLProxy()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo topDir = System.IO.Directory.GetParent(path);
            folder = topDir.FullName + "\\xml";
            SQLProxy.SetLog(script =>
            {
                Trace.WriteLine(script);
            });
        }

        [TestMethod]
        public void LoadXMLs_Normal()
        {
            string path = folder + "\\sql";
            SQLProxy.LoadXMLs(path);
            Assert.IsTrue(SQLProxy.GetSqlInfo("DATA.COLLATIONS.GETDATASOURCE") != null);
        }
        [TestMethod]
        public void LoadXMLs_NoXMLFile()
        {
            string path = folder + "\\sqlNoXMLFile";
            try
            {
                SQLProxy.LoadXMLs(path);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        [TestMethod]
        public void LoadXMLs_NoFolder()
        {
            string path = folder + "\\sqlNotExist";
            try
            {
                SQLProxy.LoadXMLs(path);
                Assert.IsTrue(false);
            }
            catch(Exception ex)
            {
                ex.ToString();
            }
        }

        [TestMethod]
        public void LoadXMLs_XMLError()
        {
            string path = folder + "\\sqlXmlError";
            try
            {
                SQLProxy.LoadXMLs(path);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        [TestMethod]
        public void LoadXMLs_NodeRepeate()
        {
            string path = folder + "\\sqlNodeRepeate";
            try
            {
                SQLProxy.LoadXMLs(path);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
    }
}

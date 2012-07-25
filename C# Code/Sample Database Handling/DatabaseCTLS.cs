using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace PrintShop.Data
{
    /// <summary>
    /// Geneneric Database Class
    /// </summary>
    [Serializable]
    public class DatabaseCTLS
    {
        // Collection Of Contents Of Table Results
        public Dictionary<String, Object> contents = new Dictionary<String, Object>();

        public String _tableName = "";              // Database Table
        public String ColumnList { get; set; }      // Column List
        public String _PrimaryKey = "";             // Primary Key

        /// <summary>
        /// Unique ID of the Table of Which This Class Represents
        /// </summary>
        public String UNIQUE_ID
        {
            get
            {
                return (String)contents[getName()] ?? "";
            }
            set
            {
                contents[getName()] = value;
            }
        }

        public string ClassName = "";       // Name Of Class ( Often Matched To Database Table )

        /// <summary>
        /// Initialize Self
        /// </summary>
        public void Initialize()
        {

            Initialize(_tableName, _PrimaryKey, ClassName);
        }

        /// <summary>
        /// Setup Class
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKey"></param>
        /// <param name="className"></param>
        public void Initialize(string tableName, string primaryKey, string className)
        {
            _tableName = tableName;
            _PrimaryKey = primaryKey;
            ClassName = className;

            Database db = DatabaseFactory.CreateDatabase("db");

            // Get Empty Result
            DataSet ds = db.ExecuteDataSet(CommandType.Text, "SELECT TOP 1 * FROM " + _tableName + " WHERE 1>2");
            DataTable dt = ds.Tables[0];

            ColumnList = "";

            // Build List Of Columns
            foreach (DataColumn column in dt.Columns)
            {
                if (ColumnList.Length > 0) ColumnList += ",";
                ColumnList += column.ColumnName;
                contents.Add(column.ColumnName, null);
            }
        }

        /// <summary>
        /// Update Record
        /// </summary>
        /// <param name="WHERE"></param>
        /// <returns></returns>
        public Boolean Update(String WHERE)
        {
            Boolean successFlag = true;
            String[] columns = ColumnList.Split(new[] { ',' });

            try
            {
                Database db = DatabaseFactory.CreateDatabase("db");
                String sql = "UPDATE " + _tableName + " SET ";

                Boolean isFirst = true;
                foreach (String key in columns)
                {
                    String s = (String)key;
                    if (s.Equals(_PrimaryKey)) continue;
                    if(contents[key] is System.Byte[]) continue;
                    if (!isFirst)
                    {
                        sql += ", ";
                    }
                    else
                    {
                        isFirst = false;
                    }

                    if (contents[key] == null || contents[key].ToString().Length==0)
                    {
                        sql += key + "=null";
                    }
                    else if (AllNumeric(contents[key].ToString()))
                    {
                        sql += key + "=" + contents[key] + "";
                    }
                    else if(contents[key] is System.Byte[])
                    {
                        // For Today-Do Nothing
                    }
                    else
                    {
                        sql += key + "='" + SafeSQL(contents[key].ToString()) + "'";
                    }
                }

                if (String.IsNullOrEmpty(WHERE))
                {
                    sql += " WHERE " + _PrimaryKey + "=" + contents[_PrimaryKey];
                }
                else
                {
                    sql += " WHERE " + WHERE;
                }

                db.ExecuteNonQuery(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                successFlag = false;
            }

            return successFlag;
        }

        /// <summary>
        /// Fix Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public String SafeSQL(String value)
        {
            value = value.Replace("'", "''");
            return value;
        }

        /// <summary>
        /// Delete Function
        /// </summary>
        /// <param name="WHERE"></param>
        /// <returns></returns>
        public Boolean Delete(String WHERE)
        {
            Boolean successFlag = true;

            try
            {
                Database db = DatabaseFactory.CreateDatabase("db");
                String sql = "DELETE FROM " + _tableName;

                if (String.IsNullOrEmpty(WHERE))
                {
                    sql += " WHERE " + _PrimaryKey + "='" + contents[" + _PrimaryKey + "] + "'";
                }
                else
                {
                    sql += " WHERE " + WHERE;
                }

                db.ExecuteNonQuery(CommandType.Text, sql);

                String[] columns = ColumnList.Split(new[] { ',' });
                foreach (String key in columns)
                {
                    contents[key] = null;
                }

            }
            catch (Exception ex)
            {
                successFlag = false;
            }

            return successFlag;
        }

        /// <summary>
        /// Custom Select Clause
        /// </summary>
        /// <param name="WHERE"></param>
        /// <param name="ORDER"></param>
        /// <returns></returns>
        public ArrayList Select(String WHERE, String ORDER)
        {
            ArrayList collection = new ArrayList();
            try
            {
                Database db = DatabaseFactory.CreateDatabase("db");
                String sql = "SELECT " + ColumnList;

                sql += " FROM " + _tableName;

                if (!String.IsNullOrEmpty(WHERE))
                {
                    sql += " WHERE " + WHERE;
                }

                if (!String.IsNullOrEmpty(ORDER))
                {
                    sql += " ORDER BY " + ORDER;
                }

                DataSet ds = db.ExecuteDataSet(CommandType.Text, sql);

                Type t = Type.GetType(ClassName);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    object clss = Activator.CreateInstance(t);

                    String[] columns = ColumnList.Split(new []{','});
                    foreach(String column in columns)
                    {
                        ((DatabaseCTLS)clss).contents[column] = row[column].ToString();
                    }

                    collection.Add(clss);
                }

                return collection;
            }
            catch (Exception ex)
            {
                //
            }

            return collection;

        }

        /// <summary>
        /// Select Top 1 Row
        /// </summary>
        /// <param name="WHERE"></param>
        /// <returns></returns>
        public Boolean SelectTop1(String WHERE)
        {
            Boolean successFlag = true;
            try
            {
                Database db = DatabaseFactory.CreateDatabase("db");
                String sql = "SELECT TOP 1 " + ColumnList;

                sql += " FROM " + _tableName;

                if (!String.IsNullOrEmpty(WHERE))
                {
                    sql += " WHERE " + WHERE;
                }

                DataSet ds = db.ExecuteDataSet(CommandType.Text, sql);
                
                if (ds.Tables[0].Rows.Count > 0)
                {
                    LoadRow(ds.Tables[0].Rows[0]);
                }
                else
                {
                    successFlag = false;
                }
            }
            catch (Exception ex)
            {
                successFlag = false;
            }

            return successFlag;

        }

        /// <summary>
        /// Load Row Into Self
        /// </summary>
        /// <param name="dr"></param>
        private void LoadRow(DataRow dr)
        {
            String[] columns = ColumnList.Split(new[] { ',' });

            foreach (String key in columns)
            {
                if (dr.Table.Columns[key].DataType.Equals(System.Type.GetType("System.Byte[]")))
                {
                    byte[] image = new byte[0];
                    if (!(dr[key] is DBNull))
                    {
                        image = (byte[])dr[key];
                    }

                    contents[key] = image;
                }
                else
                {
                    contents[key] = dr[key].ToString();
                }
            }
        }

        /// <summary>
        /// Create New Record
        /// </summary>
        /// <returns>Success Flag</returns>
        public Boolean CreateNew()
        {
            Boolean successFlag = true;
            try
            {
                Database db = DatabaseFactory.CreateDatabase("db");
                String sql = "INSERT INTO " + _tableName + " (";
                Boolean isFirst = true;
                foreach (object key in contents.Keys)
                {
                    String s = (String)key;
                    if (s.Equals(_PrimaryKey)) continue;

                    if (isFirst)
                    {
                        isFirst = false;
                        sql += s;
                    }
                    else
                    {
                        sql += ", " + s;
                    }
                }
                sql += ") OUTPUT INSERTED.Unique_ID VALUES (";

                isFirst = true;
                foreach (object key in contents.Keys)
                {
                    String s = (String)key;
                    if (s.Equals(_PrimaryKey)) continue;

                    if (isFirst)
                    {
                        isFirst = false;
                        if (contents[s] == null)
                        {
                            sql += "null";
                        }
                        else if (AllNumeric(contents[s].ToString()))
                        {
                            sql += "" + contents[s] + "";
                        }
                        else
                        {
                            sql += "'" + SafeSQL(contents[s].ToString()) + "'";
                        }
                    }
                    else
                    {
                        if (contents[s] == null)
                        {
                            sql += ",null";
                        }
                        else if (AllNumeric(contents[s].ToString()))
                        {
                            sql += "," + contents[s] + "";
                        }
                        else
                        {
                            sql += ",'" + contents[s] + "'";
                        }
                    }
                }
                sql += ")";
                Object obj = db.ExecuteScalar(CommandType.Text, sql);
                if (obj != null) UNIQUE_ID = Convert.ToString(obj);
            }
            catch (Exception ex)
            {
                successFlag = false;
            }
            finally
            {
            }

            return successFlag;
        }

        const string ALL_NUMERIC_PATTERN = "^(?:(?:[+\\-]?\\$?)|(?:\\$?[+\\-]?))?(?:(?:\\d{1,3}(?:(?:,\\d{3})|(?:\\d))*(?:\\.(?:\\d*|\\d+[eE][+\\-]\\d+))?)|(?:\\.\\d+(?:[eE][+\\-]\\d+)?))$";

        static readonly Regex All_Numeric_Regex = new Regex(ALL_NUMERIC_PATTERN);

        /// <summary>
        /// Check For All Numeric
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        static bool AllNumeric(string inputString)
        {
            if (All_Numeric_Regex.IsMatch(inputString))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get Method Name
        /// </summary>
        /// <returns></returns>
        public String getName()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            return methodBase.Name.Replace("set_", "").Replace("get_", "");
        }
    }
}

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Data.Common;
using System.Collections.Generic;


//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;

//using System.Data.Common;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

namespace Printer
{
    /// <summary>
    /// SQLSERVER助手类
    /// </summary>
    public class DBHelper : IDisposable
    {
        public DBHelper()
        {
            this.connectionString = "Data Source=192.168.0.136;Initial Catalog=IPMS;User ID=sa;Password=123456";
            // string newPassword= CryptClass.DecryptDES("W3xXWf2Gx24=","shanxijc");
            // this.connectionString = "Data Source=192.168.0.136;Initial Catalog=IPMS;User ID=sa;Password=" + newPassword;
        }
        public DBHelper(string connString)
        {
            this.connectionString = connString;
        }
        private string connectionString;
        private SqlConnection connection;
        /// <summary>
        /// 连接字符串
        /// </summary> 
        public string ConnectionString
        {
            get { return this.connectionString; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.connection != null && this.connection.State != ConnectionState.Closed)
                {
                    this.connection.Close();
                }
                this.connection.Dispose();
            }
        }
        /// <summary>
        /// 释放连接
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private SqlConnection getConnection()
        {
            try
            {
                if (this.connection == null)
                {
                    this.connection = new SqlConnection(this.connectionString);
                    this.connection.Open();
                }
                else if (this.connection.State != ConnectionState.Open)
                {
                    this.connection.Open();
                }
                return this.connection;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 得到一个SqlDataReader
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public SqlDataReader GetDataReader(string sql)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, this.getConnection()))
                {
                    cmd.Prepare();
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 得到一个SqlDataReader
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public SqlDataReader GetDataReader(string sql, SqlParameter[] parameter)
        {
            using (SqlCommand cmd = new SqlCommand(sql, this.getConnection()))
            {
                if (parameter != null && parameter.Length > 0)
                    cmd.Parameters.AddRange(parameter);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                cmd.Prepare();
                return dr;
            }
        }

        /// <summary>
        /// 得到一个DataTable
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {

            using (SqlCommand cmd = new SqlCommand(sql, this.getConnection()))
            {
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(dr);
                    cmd.Prepare();
                    return dt;
                }
            }

        }

        /// <summary>
        /// 得到一个DataTable
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, SqlParameter[] parameter)
        {
            using (SqlCommand cmd = new SqlCommand(sql, this.getConnection()))
            {
                if (parameter != null && parameter.Length > 0)
                    cmd.Parameters.AddRange(parameter);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(dr);
                    cmd.Parameters.Clear();
                    cmd.Prepare();
                    return dt;
                }
            }
        }

        /// <summary>
        /// 得到数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string sql)
        {
            using (SqlDataAdapter dap = new SqlDataAdapter(sql, this.getConnection()))
            {
                DataSet ds = new DataSet();
                dap.Fill(ds);
                return ds;
            }
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Execute(string sql)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, this.getConnection()))
                {
                    cmd.CommandTimeout = 2;
                    cmd.Prepare();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        /// <summary>
        /// 执行带参数的SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int Execute(string sql, SqlParameter[] parameter)
        {
            using (SqlCommand cmd = new SqlCommand(sql, this.getConnection()))
            {
                try
                {
                    if (parameter != null && parameter.Length > 0)
                        cmd.Parameters.AddRange(parameter);
                    int i = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    cmd.Prepare();
                    return i;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 得到一个字段的值
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string ExecuteScalar(string sql)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, this.getConnection()))
                {
                    object obj = cmd.ExecuteScalar();
                    cmd.Prepare();
                    return obj != null ? obj.ToString() : string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 得到一个字段的值
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string ExecuteScalar(string sql, SqlParameter[] parameter)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, this.getConnection()))
                {
                    if (parameter != null && parameter.Length > 0)
                        cmd.Parameters.AddRange(parameter);
                    object obj = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    cmd.Prepare();
                    return obj != null ? obj.ToString() : string.Empty;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// 获取一个sql的字段名称
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public string GetFields(string sql, SqlParameter[] param)
        {
            System.Text.StringBuilder names = new System.Text.StringBuilder(500);
            using (SqlCommand cmd = new SqlCommand(sql, this.getConnection()))
            {
                if (param != null && param.Length > 0)
                    cmd.Parameters.AddRange(param);
                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        names.Append("[" + dr.GetName(i) + "]" + (i < dr.FieldCount - 1 ? "," : string.Empty));
                    }
                    cmd.Parameters.Clear();
                    cmd.Prepare();
                    return names.ToString();
                }
            }
        }

        public string GetFieldValue(string sql)
        {
            string strTemp = string.Empty;
            return strTemp;
        }

        /// <summary>
        /// 获取一个sql的字段名称
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="tableName">表名 </param>
        /// <returns></returns>
        public string GetFields(string sql, SqlParameter[] param, out string tableName)
        {
            System.Text.StringBuilder names = new System.Text.StringBuilder(500);
            using (SqlCommand cmd = new SqlCommand(sql, this.getConnection()))
            {
                if (param != null && param.Length > 0)
                    cmd.Parameters.AddRange(param);
                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    tableName = dr.GetSchemaTable().TableName;
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        names.Append("[" + dr.GetName(i) + "]" + (i < dr.FieldCount - 1 ? "," : string.Empty));
                    }
                    cmd.Parameters.Clear();
                    cmd.Prepare();
                    return names.ToString();
                }
            }
        }

        /// <summary>
        /// 得到一个字段的备注说明
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filedName"></param>
        /// <returns></returns>
        public string GetFieldNote(string tableName, string fieldName)
        {

            System.Text.StringBuilder names = new System.Text.StringBuilder(500);
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = this.getConnection();
                cmd.CommandText = @"select value from sys.extended_properties a 
                        left join sys.syscolumns b on a.major_id=b.id and a.minor_id=b.colid 
                        where a.name='MS_Description' and object_id('" + tableName + "')=a.major_id and b.name='" + fieldName + "'";
                object obj = cmd.ExecuteScalar();
                return obj == null ? string.Empty : obj.ToString();
            }
        }

        /// <summary>
        /// 设置一个字段的备注说明
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filedName"></param>
        ///  <param name="filedNote"></param>
        /// <returns></returns>

        public void SetFieldNote(string tableName, string fieldName, string fieldNote)
        {
            System.Text.StringBuilder names = new System.Text.StringBuilder(500);
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = this.getConnection();
                cmd.CommandText = "EXEC sys.sp_addextendedproperty " +
                    "@name=N'MS_Description', @value=N'" + fieldNote + "' ," +
                    "@level0type=N'SCHEMA',@level0name=N'dbo', " +
                    "@level1type=N'TABLE',@level1name=N'" + tableName + "'," +
                    "@level2type=N'COLUMN',@level2name=N'" + fieldName + "'";
                cmd.ExecuteNonQuery();
                //    sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_testTable', N'COLUMN', N'createTime'

            }
        }
        /// <summary>
        /// 更新一个字段的说明
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldNote"></param>
        public void UpdateFieldNote(string tableName, string fieldName, string fieldNote)
        {
            System.Text.StringBuilder names = new System.Text.StringBuilder(500);
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = this.getConnection();
                cmd.CommandText = "EXEC sys.sp_updateextendedproperty " +
                    "@name=N'MS_Description', @value=N'" + fieldNote + "' ," +
                    "@level0type=N'SCHEMA',@level0name=N'dbo', " +
                    "@level1type=N'TABLE',@level1name=N'" + tableName + "'," +
                    "@level2type=N'COLUMN',@level2name=N'" + fieldName + "'";
                cmd.ExecuteNonQuery();
                //    sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_testTable', N'COLUMN', N'createTime'

            }
        }


        /// <summary>
        /// 判断一个表的某列是否为自增列
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filedName"></param>
        /// <returns></returns>
        public bool IsIdentity(string tableName, string fieldName)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = this.getConnection();
                cmd.CommandText = "select COLUMNPROPERTY(object_id('" + tableName + "'),'" + fieldName + "','IsIdentity')";
                return "1" == cmd.ExecuteScalar().ToString();
            }
        }


        /// <summary>
        /// 判断一个表的某列是否为主键
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filedName"></param>
        /// <returns></returns>
        public bool IsPrimaryKey(string tableName, string fieldName)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = this.getConnection();
                cmd.CommandText = "sp_pkeys";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@table_name", tableName));
                using (SqlDataAdapter dap = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    dap.Fill(dt);
                    return dt.Select("COLUMN_NAME='" + fieldName + "'").Length > 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="param"></param>
        /// <param name="paramlength"></param>
        /// <returns></returns>
        public string ProcedureReturn(string procName, SqlParameter[] param, int paramlength)
        {
            using (SqlCommand cmd = this.getConnection().CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure; //指定执行存储过程操作
                cmd.CommandText = procName; //存储过程名称 
                if (param != null && param.Length > 0)
                { cmd.Parameters.AddRange(param); }
                cmd.ExecuteNonQuery();
                string result = param[paramlength - 1].Value.ToString();
                return result;

            }
        }

        /// <summary>
        /// 对象参数转换DbParameter
        /// </summary>
        /// <returns></returns>
        public static SqlParameter[] GetParameter<T>(T entity)
        {
            IList<SqlParameter> parameter = new List<SqlParameter>();
            DbType dbtype = new DbType();
            Type type = entity.GetType();
            PropertyInfo[] props = type.GetProperties().Where(p => p.GetGetMethod().IsVirtual == false).ToArray();
            foreach (PropertyInfo pi in props)
            {
                if (pi.GetValue(entity, null) != null)
                {
                    switch (pi.PropertyType.ToString())
                    {
                        case "System.Nullable`1[System.Int32]":
                            dbtype = DbType.Int32;
                            break;
                        case "System.Nullable`1[System.Decimal]":
                            dbtype = DbType.Decimal;
                            break;
                        case "System.Nullable`1[System.DateTime]":
                            dbtype = DbType.DateTime;
                            break;
                        default:
                            dbtype = DbType.String;
                            break;
                    }
                    parameter.Add(new SqlParameter("@" + pi.Name, dbtype) { Value = pi.GetValue(entity, null) });
                }
            }
            return parameter.ToArray();
        }
        

        public List<T> ConvertToModelList<T>(DataTable dt) where T : new()
        {
            // 定义集合
            List<T> ts = new List<T>();
            if (dt == null || dt.Rows.Count <= 0) { return ts; }
            // 获得此模型的类型
            Type type = typeof(T);
            string tempName = "";
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;
                    // 检查DataTable是否包含此列
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter
                        if (!pi.CanWrite) continue;
                        object value = dr[tempName];
                        try
                        {
                            if (value != DBNull.Value)
                            {
                                if (pi.PropertyType == typeof(string)) { pi.SetValue(t, value.ToString(), null); }
                                else if (pi.PropertyType == typeof(int)) { pi.SetValue(t, Convert.ToInt32(value), null); }
                                else if (pi.PropertyType == typeof(decimal)) { pi.SetValue(t, Convert.ToDecimal(value), null); }
                                else if (pi.PropertyType == typeof(DateTime)) { pi.SetValue(t, Convert.ToDateTime(value), null); }
                                else { pi.SetValue(t, value, null); }
                            }
                        }
                        catch
                        {
                            //pi.SetValue(t, value.ToString(), null);
                        }

                    }

                }
                ts.Add(t);
            }
            return ts;
        }

        public T ConvertToModel<T>(DataTable dt) where T : new()
        {

            T t = new T();
            if (dt == null || dt.Rows.Count <= 0) { return t; }

            Type type = typeof(T);
            string tempName = "";
            // 获得此模型的公共属性
            PropertyInfo[] propertys = t.GetType().GetProperties();
            foreach (PropertyInfo pi in propertys)
            {
                tempName = pi.Name;
                // 检查DataTable是否包含此列
                if (dt.Columns.Contains(tempName))
                {
                    // 判断此属性是否有Setter
                    if (!pi.CanWrite) continue;
                    object value = dt.Rows[0][tempName];
                    try
                    {
                        if (value != DBNull.Value)
                        {
                            if (pi.PropertyType == typeof(string)) { pi.SetValue(t, value.ToString(), null); }
                            else if (pi.PropertyType == typeof(int)) { pi.SetValue(t, Convert.ToInt32(value), null); }
                            else if (pi.PropertyType == typeof(decimal)) { pi.SetValue(t, Convert.ToDecimal(value), null); }
                            else if (pi.PropertyType == typeof(DateTime)) { pi.SetValue(t, Convert.ToDateTime(value), null); }
                            else { pi.SetValue(t, value, null); }
                        }
                    }
                    catch
                    {
                        //pi.SetValue(t, value.ToString(), null); 
                    }
                }
            }
            return t;
        }


    }


    public class CryptClass
    {
        //默认密钥向量
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        public static string EncryptKey = "12345678";

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch (Exception ex)
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            if (decryptString == "")
                return "";
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return "";
            }
        }


    }




}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 设备属性基类，可以继承该类实现设备属性（device.Propertys）的扩展。使用 override 重写 SetInt、SetLong、SetDecimal、SetString、SetText、SetBytes 方法，在属性对象实例化后会调用相应的方法将数据库中的值传给该属性。
    /// </summary>
    public abstract class DeviceProperty
    {
        internal void SetDevice(Device device)
        {
            this.Device = device;
        }

        /// <summary>
        /// 该属性所关联的设备。
        /// </summary>
        public Device Device { get; private set; }

        internal void SetIntValue(int serialNumber, int dataValue)
        {
            this.SetInt(serialNumber, dataValue);
        }

        internal void SetLongValue(int serialNumber, long dataValue)
        {
            this.SetLong(serialNumber, dataValue);
        }

        internal void SetDecimalValue(int serialNumber, decimal dataValue)
        {
            this.SetDecimal(serialNumber, dataValue);
        }

        internal void SetStringValue(int serialNumber, string dataValue)
        {
            this.SetString(serialNumber, dataValue);
        }

        internal void SetTextValue(int serialNumber, string dataValue)
        {
            this.SetText(serialNumber, dataValue);
        }

        internal void SetBytesValue(int serialNumber, byte[] dataValue)
        {
            this.SetBytes(serialNumber, dataValue);
        }

        internal void SaveValue()
        {
            this.Save();
        }

        /// <summary>
        /// 设置整数数值
        /// </summary>
        /// <param name="serialNumber">序号</param>
        /// <param name="dataValue">整数数值</param>
        protected virtual void SetInt(int serialNumber, int dataValue)
        {
        }

        /// <summary>
        /// 设置长整数数值
        /// </summary>
        /// <param name="serialNumber">序号</param>
        /// <param name="dataValue">长整数数值</param>
        protected virtual void SetLong(int serialNumber, long dataValue)
        {
        }

        /// <summary>
        /// 设置十进制数值
        /// </summary>
        /// <param name="serialNumber">序号</param>
        /// <param name="dataValue">十进制数值</param>
        protected virtual void SetDecimal(int serialNumber, decimal dataValue)
        {
        }

        /// <summary>
        /// 设置字符串数值
        /// </summary>
        /// <param name="serialNumber">序号</param>
        /// <param name="dataValue">字符串数值</param>
        protected virtual void SetString(int serialNumber, string dataValue)
        {
        }

        /// <summary>
        /// 设置长文本数值
        /// </summary>
        /// <param name="serialNumber">序号</param>
        /// <param name="dataValue">长文本数值</param>
        protected virtual void SetText(int serialNumber, string dataValue)
        {
        }

        /// <summary>
        /// 设置二进制数值
        /// </summary>
        /// <param name="serialNumber">序号</param>
        /// <param name="dataValue">二进制数值</param>
        protected virtual void SetBytes(int serialNumber, byte[] dataValue)
        {
        }

        /// <summary>
        /// 比较两个设备属性是否完全相同（应首先比较类型是否一致，再进行内部属性值的比较）。
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public abstract bool Equals(DeviceProperty property);

        /// <summary>
        /// 保存属性数据。
        /// </summary>
        protected abstract void Save();

        internal List<string> SaveSQL;

        internal void SaveSqlToDb()
        {
            if (SaveSQL != null)
            {
                AC.Base.Database.DbConnection dbConn = this.Device.Application.GetDbConnection();
                if (dbConn != null)
                {
                    try
                    {
                        foreach (string strSql in this.SaveSQL)
                        {
                            dbConn.ExecuteNonQuery(strSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        dbConn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 保存整数数值。继承的类通过调用 base.SaveInt()、base.SaveLong()、base.SaveDecimal()、base.SaveString()、base.SaveText() 或 base.SaveBytes() 方法保存各类型数据。
        /// </summary>
        /// <param name="serialNumber">序号</param>
        /// <param name="dataValue">整数数值</param>
        protected void SaveInt(int serialNumber, int dataValue)
        {
            string strSql = this.Device.DeviceId + "," + Function.SqlStr(this.GetType().FullName) + "," + serialNumber + "," + dataValue.ToString();
            strSql = "INSERT INTO " + Tables.DevicePropertyInt.TableName + " (" + Tables.DevicePropertyInt.DeviceId + "," + Tables.DevicePropertyInt.PropertyType + "," + Tables.DevicePropertyInt.SerialNumber + "," + Tables.DevicePropertyInt.DataValue + ") VALUES (" + strSql + ")";
            this.SaveSQL.Add(strSql);
        }

        /// <summary>
        /// 保存长整数数值
        /// </summary>
        /// <param name="serialNumber">序号</param>
        /// <param name="dataValue">长整数数值</param>
        protected void SaveLong(int serialNumber, long dataValue)
        {
            string strSql = this.Device.DeviceId + "," + Function.SqlStr(this.GetType().FullName) + "," + serialNumber + "," + dataValue.ToString();
            strSql = "INSERT INTO " + Tables.DevicePropertyLong.TableName + " (" + Tables.DevicePropertyLong.DeviceId + "," + Tables.DevicePropertyLong.PropertyType + "," + Tables.DevicePropertyLong.SerialNumber + "," + Tables.DevicePropertyLong.DataValue + ") VALUES (" + strSql + ")";
            this.SaveSQL.Add(strSql);
        }

        /// <summary>
        /// 保存十进制数值
        /// </summary>
        /// <param name="serialNumber">序号</param>
        /// <param name="dataValue">十进制数值</param>
        protected void SaveDecimal(int serialNumber, decimal dataValue)
        {
            string strSql = this.Device.DeviceId + "," + Function.SqlStr(this.GetType().FullName) + "," + serialNumber + "," + dataValue.ToString();
            strSql = "INSERT INTO " + Tables.DevicePropertyDecimal.TableName + " (" + Tables.DevicePropertyDecimal.DeviceId + "," + Tables.DevicePropertyDecimal.PropertyType + "," + Tables.DevicePropertyDecimal.SerialNumber + "," + Tables.DevicePropertyDecimal.DataValue + ") VALUES (" + strSql + ")";
            this.SaveSQL.Add(strSql);
        }

        /// <summary>
        /// 保存字符串数值
        /// </summary>
        /// <param name="serialNumber">序号</param>
        /// <param name="dataValue">字符串数值</param>
        protected void SaveString(int serialNumber, string dataValue)
        {
            string strSql = this.Device.DeviceId + "," + Function.SqlStr(this.GetType().FullName) + "," + serialNumber + "," + Function.SqlStr(dataValue, 250);
            strSql = "INSERT INTO " + Tables.DevicePropertyString.TableName + " (" + Tables.DevicePropertyString.DeviceId + "," + Tables.DevicePropertyString.PropertyType + "," + Tables.DevicePropertyString.SerialNumber + "," + Tables.DevicePropertyString.DataValue + ") VALUES (" + strSql + ")";
            this.SaveSQL.Add(strSql);
        }

        /// <summary>
        /// 保存长文本数值
        /// </summary>
        /// <param name="serialNumber">序号</param>
        /// <param name="dataValue">长文本数值</param>
        protected void SaveText(int serialNumber, string dataValue)
        {
            AC.Base.Database.DbConnection dbConn = this.Device.Application.GetDbConnection();
            if (dbConn != null)
            {
                try
                {
                    string strSql = this.Device.DeviceId + "," + Function.SqlStr(this.GetType().FullName) + "," + serialNumber + "," + Function.SqlStr(dataValue);
                    strSql = "INSERT INTO " + Tables.DevicePropertyText.TableName + " (" + Tables.DevicePropertyText.DeviceId + "," + Tables.DevicePropertyText.PropertyType + "," + Tables.DevicePropertyText.SerialNumber + "," + Tables.DevicePropertyText.DataValue + ") VALUES (" + strSql + ")";
                    dbConn.ExecuteNonQuery(strSql);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    dbConn.Close();
                }
            }
        }

        /// <summary>
        /// 保存二进制数值
        /// </summary>
        /// <param name="serialNumber">序号</param>
        /// <param name="dataValue">二进制数值</param>
        protected void SaveBytes(int serialNumber, byte[] dataValue)
        {
            AC.Base.Database.DbConnection dbConn = this.Device.Application.GetDbConnection();
            if (dbConn != null)
            {
                try
                {
                    System.Data.IDbDataParameter parDeviceId = (System.Data.IDbDataParameter)dbConn.GetNewDbDataParameter();
                    parDeviceId.ParameterName = "@" + Tables.DevicePropertyBytes.DeviceId;
                    parDeviceId.Value = this.Device.DeviceId;

                    System.Data.IDbDataParameter parPropertyType = (System.Data.IDbDataParameter)dbConn.GetNewDbDataParameter();
                    parPropertyType.ParameterName = "@" + Tables.DevicePropertyBytes.PropertyType;
                    parPropertyType.Value = Function.SqlStr(this.GetType().FullName);

                    System.Data.IDbDataParameter parSerialNumber = (System.Data.IDbDataParameter)dbConn.GetNewDbDataParameter();
                    parSerialNumber.ParameterName = "@" + Tables.DevicePropertyBytes.SerialNumber;
                    parSerialNumber.Value = serialNumber;

                    System.Data.IDbDataParameter parDataValue = (System.Data.IDbDataParameter)dbConn.GetNewDbDataParameter();
                    parDataValue.ParameterName = "@" + Tables.DevicePropertyBytes.DataValue;
                    parDataValue.Value = dataValue;

                    string strSql = "@" + Tables.DevicePropertyBytes.DeviceId + "," + "@" + Tables.DevicePropertyBytes.PropertyType + "," + "@" + Tables.DevicePropertyBytes.SerialNumber + "," + "@" + Tables.DevicePropertyBytes.DataValue;
                    strSql = "INSERT INTO " + Tables.DevicePropertyBytes.TableName + " (" + Tables.DevicePropertyBytes.DeviceId + "," + Tables.DevicePropertyBytes.PropertyType + "," + Tables.DevicePropertyBytes.SerialNumber + "," + Tables.DevicePropertyBytes.DataValue + ") VALUES (" + strSql + ")";
                    dbConn.ExecuteNonQuery(strSql, -1, System.Data.CommandType.Text, parDeviceId, parPropertyType, parSerialNumber, parDataValue);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    dbConn.Close();
                }
            }
        }

        /// <summary>
        /// 删除该属性数据。
        /// </summary>
        internal void Delete()
        {
            AC.Base.Database.DbConnection dbConn = this.Device.Application.GetDbConnection();
            if (dbConn != null)
            {
                try
                {
                    string strSql;

                    strSql = "DELETE FROM " + Tables.DevicePropertyInt.TableName + " WHERE " + Tables.DevicePropertyInt.DeviceId + "=" + this.Device.DeviceId + " AND " + Tables.DevicePropertyInt.PropertyType + "=" + Function.SqlStr(this.GetType().FullName);
                    dbConn.ExecuteNonQuery(strSql);

                    strSql = "DELETE FROM " + Tables.DevicePropertyLong.TableName + " WHERE " + Tables.DevicePropertyLong.DeviceId + "=" + this.Device.DeviceId + " AND " + Tables.DevicePropertyLong.PropertyType + "=" + Function.SqlStr(this.GetType().FullName);
                    dbConn.ExecuteNonQuery(strSql);

                    strSql = "DELETE FROM " + Tables.DevicePropertyDecimal.TableName + " WHERE " + Tables.DevicePropertyDecimal.DeviceId + "=" + this.Device.DeviceId + " AND " + Tables.DevicePropertyDecimal.PropertyType + "=" + Function.SqlStr(this.GetType().FullName);
                    dbConn.ExecuteNonQuery(strSql);

                    strSql = "DELETE FROM " + Tables.DevicePropertyString.TableName + " WHERE " + Tables.DevicePropertyString.DeviceId + "=" + this.Device.DeviceId + " AND " + Tables.DevicePropertyString.PropertyType + "=" + Function.SqlStr(this.GetType().FullName);
                    dbConn.ExecuteNonQuery(strSql);

                    strSql = "DELETE FROM " + Tables.DevicePropertyText.TableName + " WHERE " + Tables.DevicePropertyText.DeviceId + "=" + this.Device.DeviceId + " AND " + Tables.DevicePropertyText.PropertyType + "=" + Function.SqlStr(this.GetType().FullName);
                    dbConn.ExecuteNonQuery(strSql);

                    strSql = "DELETE FROM " + Tables.DevicePropertyBytes.TableName + " WHERE " + Tables.DevicePropertyBytes.DeviceId + "=" + this.Device.DeviceId + " AND " + Tables.DevicePropertyBytes.PropertyType + "=" + Function.SqlStr(this.GetType().FullName);
                    dbConn.ExecuteNonQuery(strSql);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    dbConn.Close();
                }
            }
        }
    }
}

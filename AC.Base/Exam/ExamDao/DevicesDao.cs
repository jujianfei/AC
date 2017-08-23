using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base;
using System.Data.SqlClient;
using System.Data;
using AC.Base.Exam.Exam;


namespace AC.Base.Exam.ExamDao
{
    class DevicesDao
    {
        private AC.Base.Database.DbConnection dbConnection;
          
        public DevicesDao(ApplicationClass application)
        {
            dbConnection = application.GetDbConnection();
        }


        #region <<新增>>

        public int insert(Devices devices)
        {
            devices.BaseKey = this.getMaxKeyValue();
            String sql = "insert into devices(BaseKey,TaskConfigId,Suite,Factory,Address,DeviceType) \n"
                + "values(" + devices.BaseKey + "," + devices.TaskConfigId + "," + (devices.Suite == "" ? "null" : devices.Suite) + "," + (devices.Factory == "" ? "null" : devices.Factory) + "," + devices.Address + "," + devices.DeviceType + ")";
            try
            {
                return dbConnection.ExecuteNonQuery(sql);
            }
            catch (Exception)
            {
                return 0;
            }
            finally
            {
                dbConnection.Close();
            }

        }

        #endregion

        #region <<删除>>

        public void delete(String querySql)
        {
            String sql = "delete from devices where 1>0 " + querySql;
            try
            {
                dbConnection.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
               // Console.WriteLine(ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }
        }

        public void delete(int baseKey)
        {
            String querySql = " and baseKey=" + baseKey;
            this.delete(querySql);
        }

        public void delete(Devices devices)
        {
            this.delete(devices.BaseKey);
        }

        #endregion

        #region <<修改>>

        public void update(Devices devices)
        {
            String sql = "update devices set TaskConfigId=" + devices.TaskConfigId + ",Suite=" + devices.Suite + ",Factory=" + devices.Factory + " ,Address=" + devices.Address + " , DeviceType=" + devices.DeviceType + " where  BaseKey=" + devices.BaseKey + "";

            try
            {
                dbConnection.ExecuteNonQuery(sql);
            }
            catch (Exception ex)
            {
               // Console.WriteLine(ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }
        }

        #endregion

        #region <<查询>>

        public List<Devices> select()
        {
            return this.select("");
        }

        public List<Devices> select(int baseKey)
        {
            return this.select(" and baseKey=" + baseKey);
        }

        public List<Devices> select(string searchSql)
        {
            String sql = " select BaseKey,TaskConfigId,Suite,Factory,Address,DeviceType from Devices where 1>0 ";
            sql += searchSql;
            return DevicesRowMapper(dbConnection.ExecuteReader(sql));
        }

        #endregion

        #region 获取最大关键字

        private int getMaxKeyValue()
        {
            String sql = "select max(baseKey) from devices ";
            IDataReader dr = dbConnection.ExecuteReader(sql);
            int maxKeyValue=1;
            while (dr.Read())
            {
                maxKeyValue = dr[0].ToString() == "" ? 1 : Convert.ToInt32(dr[0]) + 1;
            }
            dr.Close();
            return maxKeyValue;
        }

        #endregion

        #region 处理DataReader对象，返回List

        public List<Devices> DevicesRowMapper(IDataReader dr)
        {
            List<Devices> DevicesList = new List<Devices>();
            try
            {
                while (dr.Read())
                {
                    Devices devices = new Devices();
                    devices.BaseKey = Convert.ToInt32(dr[0]);
                    devices.TaskConfigId = Convert.ToInt32(dr[1]);
                    devices.Suite = dr[2].ToString();
                    devices.Factory = dr[3].ToString();
                    devices.Address = dr[4].ToString();
                    devices.DeviceType = Convert.ToInt32(dr[5]);

                    DevicesList.Add(devices);
                }
            }
            catch (Exception ex)
            {
               // Console.WriteLine("---------------------------------------->" + ex.Message);
            }
            finally
            {
                dr.Close();
            }
            return DevicesList;
        }
        #endregion
    }
}

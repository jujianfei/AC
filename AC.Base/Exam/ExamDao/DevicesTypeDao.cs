using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base;
using System.Data;
using AC.Base.Exam.Exam;

namespace AC.Base.Exam.ExamDao
{
    class DevicesTypeDao
    {
        private AC.Base.Database.DbConnection dbConnection;

        public DevicesTypeDao(ApplicationClass application)
        {
            dbConnection = application.GetDbConnection();
        }


        public List<DevicesType> select()
        {
            return this.select("");
        }

        public List<DevicesType> select(int baseKey)
        {
            return this.select(" and baseKey=" + baseKey);
        }

        public List<DevicesType> select(string searchSql)
        {
            String sql = " select BaseKey,DeviceTypeName from DeviceType where 1>0 ";
            sql += searchSql;
            return DeviceTypeRowMapper(dbConnection.ExecuteReader(sql));
        }

        public List<DevicesType> DeviceTypeRowMapper(IDataReader dr)
        {
            List<DevicesType> deviceTypeList = new List<DevicesType>();
            try
            {
                while (dr.Read())
                {
                    DevicesType devicesType = new DevicesType();
                    devicesType.BaseKey = Convert.ToInt32(dr[0]);
                    devicesType.DevicesTypeName = dr[1] + "";
                    deviceTypeList.Add(devicesType);
                }
            }
            catch (Exception ex)
            {
               // Console.WriteLine("---------------------------------------->" + ex.Message);
            }
            finally
            {
                dr.Close();
                dbConnection.Close();
            }
            return deviceTypeList;
        }

    }
}

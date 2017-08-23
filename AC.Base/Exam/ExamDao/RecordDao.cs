using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base;
using AC.Base.Exam.Exam;
using System.Data.SqlClient;
using System.Data;

namespace AC.Base.Exam.ExamDao
{
    class RecordDao
    {

        private AC.Base.Database.DbConnection dbConnection;

        public RecordDao(ApplicationClass application)
        {
            dbConnection = application.GetDbConnection();
        }


        #region <<新增>>

        public void insert(Record record)
        {
            record.BaseKey = this.getMaxKeyValue();
            String sql = "insert into record(BaseKey,TaskConfigId,XmlConfig) \n"
                        + "values("+record.BaseKey+","+record.TaskConfigId+",'"+record.XmlConfig+"')";
            try
            {
                dbConnection.ExecuteNonQuery(sql);
            }
            catch (Exception)
            {

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
            String sql = "delete from record where 1>0 " + querySql;
            try
            {
                dbConnection.ExecuteNonQuery(sql);
            }
            catch (Exception)
            {

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

        public void delete(Record record)
        {
            this.delete(record.BaseKey);
        }

        #endregion

        #region <<修改>>

        public int update(Record record)
        {
            String sql = "update record set TaskConfigId="+ record.TaskConfigId+",XmlConfig='"+record.XmlConfig+"' where  BaseKey="+record.BaseKey;
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

        #region <<查询>>

        public List<Record> select()
        {
            return this.select("");
        }

        public List<Record> select(int baseKey)
        {
            return this.select(" and baseKey=" + baseKey);
        }

        public List<Record> select(string searchSql)
        {
            String sql = " select BaseKey,TaskConfigId,XmlConfig from Record where 1>0 ";
            sql += searchSql;
            return RecordRowMapper(dbConnection.ExecuteReader(sql));
        }

        #endregion

        #region 获取最大关键字

        private int getMaxKeyValue()
        {
            String sql = "select max(baseKey) from record ";
            IDataReader dr = dbConnection.ExecuteReader(sql);
            int maxKeyValue =1;
            while (dr.Read())
            {
                maxKeyValue = dr[0].ToString() == "" ? 1 : Convert.ToInt32(dr[0]) + 1;
            }
           
            dr.Close();
            return maxKeyValue;
        }

        #endregion

        #region 处理DataReader对象，返回List

        public List<Record> RecordRowMapper(IDataReader dr)
        {
            List<Record> RecordList = new List<Record>();
            try
            {
                while (dr.Read())
                {
                    Record record = new Record();
                    record.BaseKey = Convert.ToInt32(dr[0]);
                    record.TaskConfigId = Convert.ToInt32(dr[1]);
                    record.XmlConfig = dr[2].ToString();
                    RecordList.Add(record);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                dr.Close();
            }
            return RecordList;
        }
        #endregion
    }
}

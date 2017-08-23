using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base;
using AC.Base.Exam.Exam;
using System.Data;

namespace AC.Base.Exam.ExamDao
{
    class ExamRecordDao
    {
         private AC.Base.Database.DbConnection dbConnection;

         public ExamRecordDao(ApplicationClass application)
        {
            dbConnection = application.GetDbConnection();
        }


        #region <<新增>>

        public void insert(ExamRecord examRecord)
        {
            examRecord.BaseKey = this.getMaxKeyValue();
            String sql = "insert into examRecord(BaseKey,TaskConfigId,XmlConfig) \n"
                        + "values("+examRecord.BaseKey+","+examRecord.TaskConfigId+",'"+examRecord.XmlConfig+"')";
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
            String sql = "delete from examRecord where 1>0 " + querySql;
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

        public void delete(Record examRecord)
        {
            this.delete(examRecord.BaseKey);
        }

        #endregion

        #region <<修改>>

        public int update(ExamRecord examRecord)
        {
            String sql = "update examRecord set TaskConfigId="+ examRecord.TaskConfigId+",XmlConfig='"+examRecord.XmlConfig+"' where  BaseKey="+examRecord.BaseKey;
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

        public List<ExamRecord> select()
        {
            return this.select("");
        }

        public List<ExamRecord> select(int baseKey)
        {
            return this.select(" and baseKey=" + baseKey);
        }

        public List<ExamRecord> select(string searchSql)
        {
            String sql = " select BaseKey,TaskConfigId,XmlConfig from ExamRecord where 1>0 ";
            sql += searchSql;
            return ExamRecordRowMapper(dbConnection.ExecuteReader(sql));
        }

        #endregion

        #region 获取最大关键字

        private int getMaxKeyValue()
        {
            String sql = "select max(baseKey) from examRecord ";
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

        public List<ExamRecord> ExamRecordRowMapper(IDataReader dr)
        {
            List<ExamRecord> ExamRecordList = new List<ExamRecord>();
            try
            {
                while (dr.Read())
                {
                    ExamRecord examRecord = new ExamRecord();
                    examRecord.BaseKey = Convert.ToInt32(dr[0]);
                    examRecord.TaskConfigId = Convert.ToInt32(dr[1]);
                    examRecord.XmlConfig = dr[2].ToString();
                    ExamRecordList.Add(examRecord);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                dr.Close();
            }
            return ExamRecordList;
        }
        #endregion


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base;
using System.Data;
using AC.Base.Exam.Exam;
using System.Data.SqlClient;

namespace AC.Base.Exam.ExamDao
{
    class ExamTypeDao
    {
        private AC.Base.Database.DbConnection dbConnection;
        public ExamTypeDao(ApplicationClass application)
        {
            dbConnection = application.GetDbConnection();
        }

        #region 获取最大关键字

        private int getMaxKeyValue()
        {
            String sql = "select max(baseKey) from examType ";
            IDataReader dr = dbConnection.ExecuteReader(sql);
            int maxKeyValue = dr[0].ToString() == "" ? 1 : Convert.ToInt32(dr[0]) + 1;
            dr.Close();
            return maxKeyValue;
        }

        #endregion

        #region 处理DataReader对象，返回List

        public List<ExamType> ExamTypeRowMapper(IDataReader dr)
        {
            List<ExamType> examTypeList = new List<ExamType>();
            try
            {
                while (dr.Read())
                {
                    ExamType examType = new ExamType();
                    examType.BaseKey = Convert.ToInt32(dr[0]);
                    examType.TestType = Convert.ToInt32(dr[1]);
                    examType.Item = Convert.ToInt32(dr[2]);
                    examTypeList.Add(examType);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                dr.Close();
            }
            return examTypeList;
        }
        #endregion

        #region <<新增>>

        public void insert(ExamType examType)
        {
            examType.BaseKey = this.getMaxKeyValue();
            String sql = "insert into examType(BaseKey,TestType,Item) \n"
                        + "values(@BaseKey,@TestType,@Item)";
            SqlParameter[] parameters = {
                new SqlParameter("@BaseKey", examType.BaseKey),
                new SqlParameter("@TestType", examType.TestType),
                new SqlParameter("@Item",examType.Item)
            };

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
            String sql = "delete from examType where 1>0 " + querySql;
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

        public void delete(ExamType examType)
        {
            this.delete(examType.BaseKey);
        }

        #endregion

        #region <<修改>>

        public void update(ExamType examType)
        {
            String sql = "update examType set TestType=@TestType,Item=@Item  where  BaseKey=@BaseKey";
            SqlParameter[] parameters = {
                new SqlParameter("@TestType", examType.TestType),
                new SqlParameter("@Item",examType.Item),
                new SqlParameter("@BaseKey", examType.BaseKey)
            };
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

        #region <<查询>>

        public List<ExamType> select()
        {
            return this.select("");
        }

        public List<ExamType> select(int baseKey)
        {
            return this.select(" and baseKey=" + baseKey);
        }

        public List<ExamType> select(string searchSql)
        {
            String sql = " select BaseKey,TestType,Item from ExamType where 1>0 ";
            sql += searchSql;
            return ExamTypeRowMapper(dbConnection.ExecuteReader(sql));
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base;
using AC.Base.Exam.Exam;
using System.Data;
using System.Data.SqlClient;


namespace AC.Base.Exam.ExamDao
{
    class ExamItemDao
    {
        private AC.Base.Database.DbConnection dbConnection;
        public ExamItemDao(ApplicationClass application)
        {
            dbConnection = application.GetDbConnection();
        }

        #region <<新增>>

        public void insert(ExamItem examItem)
        {
            examItem.BaseKey = this.getMaxKeyValue();
            String sql = "insert into examItem(BaseKey,Item,FunCode,ParentId) \n"
                        + "values(@BaseKey,@Item,@FunCode,@ParentId)";
            SqlParameter[] parameters = {
                new SqlParameter("@BaseKey", examItem.BaseKey),
                new SqlParameter("@Item", examItem.Item),
                new SqlParameter("@FunCode", examItem.FunCode),
                new SqlParameter("@ParentId",examItem.ParentId)
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

        public void delete(String  querySql)
        {
            String sql = "delete from examItem where 1>0 " + querySql;
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

        public void delete(ExamItem examItem)
        {
            this.delete(examItem.BaseKey);
        }

        #endregion

        #region <<修改>>

        public void update(ExamItem examItem)
        {
            String sql = "update examItem set Item=@Item,FunCode=@FunCode,ParentId=@ParentId where  BaseKey=@BaseKey";
            SqlParameter[] parameters = {
                new SqlParameter("@Item", examItem.Item),
                new SqlParameter("@FunCode", examItem.FunCode),
                new SqlParameter("@ParentId",examItem.ParentId),
                new SqlParameter("@BaseKey", examItem.BaseKey)
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

        public List<ExamItem> select()
        {
            return this.select("");
        }

        public List<ExamItem> select(int baseKey)
        {
            return this.select(" and baseKey="+baseKey );
        }

        public List<ExamItem> select(string searchSql)
        {
            String sql = " select BaseKey,Item,FunCode,ParentId from ExamItem where 1>0 ";
            sql += searchSql;
            return ExamItemRowMapper(dbConnection.ExecuteReader(sql));
        }

        #endregion

        #region 获取最大关键字

        private  int getMaxKeyValue()
        {
            String sql = "select max(baseKey) from examItem ";
            IDataReader dr=dbConnection.ExecuteReader(sql);
            int maxKeyValue = dr[0].ToString() == "" ? 1 : Convert.ToInt32(dr[0]) + 1;
            dr.Close();
            dbConnection.Close();
            return maxKeyValue;
        }

        #endregion
   
        #region 处理DataReader对象，返回List

        public List<ExamItem> ExamItemRowMapper(IDataReader dr)
        {
            List<ExamItem> ExamItemList = new List<ExamItem>();
            try
            {
                while (dr.Read())
                {
                    ExamItem examItem = new ExamItem();
                    examItem.BaseKey =Convert.ToInt32(dr[0]);
                    examItem.Item = dr[1].ToString();
                    examItem.FunCode = dr[2].ToString();
                    examItem.ParentId =Convert.ToInt32(dr[3]);
                   

                    ExamItemList.Add(examItem);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                dr.Close();
                dbConnection.Close();
            }
            return ExamItemList;
        }

        #endregion
    }
}

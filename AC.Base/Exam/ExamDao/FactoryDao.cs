using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base;
using AC.Base.Exam.Exam;
using System.Data;

namespace AC.Base.Exam.ExamDao
{
    public class FactoryDao
    {

        private AC.Base.Database.DbConnection dbConnection;

        public FactoryDao(ApplicationClass application)
        {
            dbConnection = application.GetDbConnection();
        }


        #region <<新增>>

        public int insert(Factory factory)
        {
            factory.BaseKey = this.getMaxKeyValue();
            String sql = "insert into factory(BaseKey,FactoryName) \n"
                + "values(" + factory.BaseKey + ",'" + factory.FactoryName + "')";
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
            String sql = "delete from factory where 1>0 " + querySql;
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

        public void delete(Factory factory)
        {
            this.delete(factory.BaseKey);
        }

        #endregion

        #region <<修改>>

        public void update(Factory factory)
        {
            String sql = "update Factory set FactoryName='" + factory.FactoryName + "' where  BaseKey=" + factory.BaseKey + "";

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

        public List<Factory> select()
        {
            return this.select("");
        }

        public List<Factory> select(int baseKey)
        {
            return this.select(" and baseKey=" + baseKey);
        }

        public List<Factory> select(string searchSql)
        {
            String sql = " select BaseKey,FactoryName from Factory where 1>0 ";
            sql += searchSql;
            return FactoryRowMapper(dbConnection.ExecuteReader(sql));
        }

        #endregion

        #region 获取最大关键字

        private int getMaxKeyValue()
        {
            String sql = "select max(baseKey) from Factory ";
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

        public List<Factory> FactoryRowMapper(IDataReader dr)
        {
            List<Factory> factoryList = new List<Factory>();
            try
            {
                while (dr.Read())
                {
                    Factory factory = new Factory();
                    factory.BaseKey = Convert.ToInt32(dr[0]);
                    factory.FactoryName = dr[1]+"";
                    factoryList.Add(factory);
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
            return factoryList;
        }
        #endregion
    }
}


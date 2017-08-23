using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 任务组。
    /// </summary>
    public class TaskGroup
    {
        /// <summary>
        /// 任务组。
        /// </summary>
        /// <param name="application"></param>
        public TaskGroup(ApplicationClass application)
        {
            this.Application = application;
        }

        /// <summary>
        /// 应用程序框架。
        /// </summary>
        public ApplicationClass Application { get; private set; }

        /// <summary>
        /// 任务组编号。
        /// </summary>
        public int TaskGroupId { get; private set; }

        /// <summary>
        /// 任务组名称。
        /// </summary>
        public string Name { get; set; }

        internal virtual void SetDataReader(System.Data.IDataReader dr)
        {
            this.TaskGroupId = Function.ToInt(dr[Tables.TaskGroup.TaskGroupId]);
            this.Name = Function.ToString(dr[Tables.TaskGroup.Name]);
        }

        private TaskConfigCollection m_TaskConfigs;
        /// <summary>
        /// 当前任务组中已配置的任务。
        /// </summary>
        public TaskConfigCollection TaskConfigs
        {
            get
            {
                if (this.m_TaskConfigs == null)
                {
                    this.m_TaskConfigs = new TaskConfigCollection();
                }
                return this.m_TaskConfigs;
            }
        }

        /// <summary>
        /// 保存任务组。
        /// </summary>
        public void Save()
        {
            if (this.Name == null || this.Name.Length == 0)
            {
                throw new Exception("任务组名称必须输入。");
            }

            AC.Base.Database.DbConnection dbConn = this.Application.GetDbConnection();
            if (dbConn != null)
            {
                try
                {
                    if (this.TaskGroupId == 0)
                    {
                        string strSql = "SELECT MAX(" + Tables.TaskGroup.TaskGroupId + ") FROM " + Tables.TaskGroup.TableName;
                        this.TaskGroupId = Function.ToInt(dbConn.ExecuteScalar(strSql)) + 1;

                        strSql = this.TaskGroupId + "," + Function.SqlStr(this.Name, 250);
                        strSql = "INSERT INTO " + Tables.TaskGroup.TableName + " (" + Tables.TaskGroup.TaskGroupId + "," + Tables.TaskGroup.Name + ") VALUES (" + strSql + ")";
                        dbConn.ExecuteNonQuery(strSql);
                    }
                    else
                    {
                        string strSql = "UPDATE " + Tables.TaskGroup.TableName + " Set " + Tables.TaskGroup.Name + "=" + Function.SqlStr(this.Name, 250) + " Where " + Tables.TaskGroup.TaskGroupId + "=" + this.TaskGroupId;
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

        /// <summary>
        /// 删除当前任务组。
        /// </summary>
        public void Delete()
        {
            AC.Base.Database.DbConnection dbConn = this.Application.GetDbConnection();
            if (dbConn != null)
            {
                try
                {
                    //还需添加删除任务日志的代码

                    string strSql;
                    strSql = "Delete From " + Tables.TaskConfig.TableName + " Where " + Tables.TaskConfig.TaskGroupId + "=" + this.TaskGroupId;
                    dbConn.ExecuteNonQuery(strSql);

                    strSql = "Delete From " + Tables.TaskGroup.TableName + " Where " + Tables.TaskGroup.TaskGroupId + "=" + this.TaskGroupId;
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
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// 任务组集合。
    /// </summary>
    public class TaskGroupCollection : ReadOnlyCollection<TaskGroup>
    {
        private ApplicationClass m_Application;

        internal TaskGroupCollection(ApplicationClass application)
        {
            this.m_Application = application;
            this.Load();
        }

        private void Load()
        {
            AC.Base.Database.DbConnection dbConn = this.m_Application.GetDbConnection();
            if (dbConn != null)
            {
                try
                {
                    string strSql = "SELECT * FROM " + Tables.TaskGroup.TableName;
                    System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                    while (dr.Read())
                    {
                        TaskGroup _TaskGroup = new TaskGroup(this.m_Application);
                        _TaskGroup.SetDataReader(dr);
                        this.Add(_TaskGroup);
                    }
                    dr.Close();

                    strSql = "SELECT * FROM " + Tables.TaskConfig.TableName;
                    dr = dbConn.ExecuteReader(strSql);
                    while (dr.Read())
                    {
                        TaskGroup _TaskGroup = this.GetById(Function.ToInt(dr[Tables.TaskConfig.TaskGroupId]));
                        if (_TaskGroup != null)
                        {
                            TaskType _TaskType = this.m_Application.TaskTypes.GetTaskType(Function.ToString(dr[Tables.TaskConfig.TaskType]));
                            if (_TaskType != null)
                            {
                                TaskConfig _TaskConfig = _TaskType.CreateTaskConfig(_TaskGroup);
                                _TaskConfig.SetDataReader(dr);
                                _TaskGroup.TaskConfigs.Add(_TaskConfig);
                            }
                        }
                    }
                    dr.Close();
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

        internal new void Add(TaskGroup taskGroup)
        {
            base.Items.Add(taskGroup);
        }

        /// <summary>
        /// 获取指定编号的任务组。
        /// </summary>
        /// <param name="taskGroupId"></param>
        /// <returns></returns>
        public TaskGroup GetById(int taskGroupId)
        {
            foreach (TaskGroup taskGroup in this)
            {
                if (taskGroup.TaskGroupId == taskGroupId)
                {
                    return taskGroup;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定编号的任务配置。
        /// </summary>
        /// <param name="taskConfigId">任务配置编号。</param>
        /// <returns></returns>
        public TaskConfig GetTaskConfig(int taskConfigId)
        {
            foreach (TaskGroup taskGroup in this)
            {
                foreach (TaskConfig taskConfig in taskGroup.TaskConfigs)
                {
                    if (taskConfig.TaskConfigId == taskConfigId)
                    {
                        return taskConfig;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "共 " + this.Count + " 项任务";
        }
    }
}

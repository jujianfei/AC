using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 任务被创建后产生的事件所调用的委托。
    /// </summary>
    /// <param name="task">被创建并且还未开始运行的任务。</param>
    public delegate void TaskCreatedEventHandler(Task task);

    /// <summary>
    /// 任务配置。
    /// </summary>
    public abstract class TaskConfig : ICloneable
    {
        /// <summary>
        /// 获取应用程序框架。
        /// </summary>
        public ApplicationClass Application { get; internal set; }

        /// <summary>
        /// 获取任务类型。
        /// </summary>
        public TaskType TaskType { get; internal set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryTimes { get;  set; }


        /// <summary>
        /// 获取该任务所属的任务组。
        /// </summary>
        public TaskGroup Group { get; internal set; }

        /// <summary>
        /// 获取任务编号。
        /// </summary>
        public int TaskConfigId { get; private set; }


        public int currentIndex { get;  set; }

        private string m_Name;
        /// <summary>
        /// 获取或设置任务名称。
        /// </summary>
        public string Name
        {
            get
            {
                if (this.m_Name == null)
                {
                    this.m_Name = "";
                }
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }

        /// <summary>
        /// 该任务是否允许自动运行。
        /// </summary>
        public bool Enable { get; set; }

        private TaskPeriod m_Period;
        /// <summary>
        /// 获取或设置任务的运行周期。
        /// </summary>
        public TaskPeriod Period
        {
            get
            {
                if (this.m_Period == null)
                {
                    this.m_Period = this.TaskType.PeriodTypes[0].CreateTaskPeriod();
                }
                return this.m_Period;
            }
            set
            {
                if (this.m_Period != null)
                {
                    this.m_Period.Config = null;
                }

                this.m_Period = value;

                if (this.m_Period != null)
                {
                    this.m_Period.Config = this;
                }
            }
        }

        /// <summary>
        /// 任务最大运行时间（秒）。表示一个任务最长可运行的时间，超过该指定时间还未运行结束将被强行终止，0 表示不限制任务的运行时间。
        /// </summary>
        public int MaxRunTime { get; set; }


        public System.Windows.Forms.TreeListView tlvDevice;

        private decimal m_RetryProgress = 80;
        /// <summary>
        /// 任务重试进度指标，当任务运行进度未达此指定值后进行重试。
        /// </summary>
        public decimal RetryProgress
        {
            get
            {
                return this.m_RetryProgress;
            }
            set
            {
                this.m_RetryProgress = value;
            }
        }

        /// <summary>
        /// 是否不合格重试
        /// </summary>
        public bool IsUnqualifiedRetry { get; set; }


        private int[] m_RetryInterval;
        /// <summary>
        /// 任务重试的时间间隔(秒)。
        /// </summary>
        public int[] RetryInterval
        {
            get
            {
                if (this.m_RetryInterval == null)
                {
                    this.m_RetryInterval = new int[this.TaskType.MaxRetryTimes];
                    for (int intIndex = 0; intIndex < this.m_RetryInterval.Length; intIndex++)
                    {
                        this.m_RetryInterval[intIndex] = 4 + 3 * intIndex;
                    }
                }
                return this.m_RetryInterval;
            }
            set { this.m_RetryInterval = value; }
        }

        /// <summary>
        /// 从保存此任务数据的 XML 文档节点初始化当前任务。
        /// </summary>
        /// <param name="taskConfig">该对象节点的数据</param>
        public abstract void SetTaskConfig(System.Xml.XmlNode taskConfig);

        /// <summary>
        /// 获取当前任务的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        /// <returns></returns>
        public abstract System.Xml.XmlNode GetTaskConfig(System.Xml.XmlDocument xmlDoc);

        internal void SetDataReader(System.Data.IDataReader dr)
        {
            this.TaskConfigId = Function.ToInt(dr[Tables.TaskConfig.TaskConfigId]);
            this.Name = Function.ToString(dr[Tables.TaskConfig.Name]);
            this.Enable = Function.ToBool(dr[Tables.TaskConfig.EnableAuto]);

            object objConfig = dr[Tables.TaskConfig.XMLConfig];
            if (objConfig != null && !(objConfig is System.DBNull))
            {
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                try
                {
                    xmlDoc.LoadXml(objConfig.ToString());

                    if (xmlDoc.ChildNodes.Count > 0)
                    {
                        foreach (System.Xml.XmlNode xnConfig in xmlDoc.ChildNodes[0].ChildNodes)
                        {
                            switch (xnConfig.Name)
                            {
                                case "PeriodType":
                                    TaskPeriodType _TaskPeriodType = this.Application.TaskPeriodTypes.GetPeriodType(xnConfig.InnerText);
                                    if (_TaskPeriodType != null)
                                    {
                                        this.Period = _TaskPeriodType.CreateTaskPeriod();
                                    }
                                    break;

                                case "Period":
                                    if (xnConfig.ChildNodes.Count > 0 && this.Period != null)
                                    {
                                        this.Period.SetTaskPeriodConfig(xnConfig.ChildNodes[0]);
                                    }
                                    break;

                                case "MaxRunTime":
                                    this.MaxRunTime = Function.ToInt(xnConfig.InnerText);
                                    break;

                                case "RetryProgress":
                                    this.RetryProgress = Function.ToDecimal(xnConfig.InnerText);
                                    break;

                                case "IsUnqualifiedRetry":
                                    this.IsUnqualifiedRetry = Function.ToBool(xnConfig.InnerText);
                                    break;

                                case "RetryInterval":
                                    for (int intIndex = 0; intIndex < xnConfig.ChildNodes.Count; intIndex++)
                                        if (intIndex < this.RetryInterval.Length)
                                            this.RetryInterval[intIndex] = Function.ToInt(xnConfig.ChildNodes[intIndex].InnerText);
                                    break;

                                case "TaskConfig":
                                    if (xnConfig.ChildNodes.Count > 0)
                                    {
                                        this.SetTaskConfig(xnConfig.ChildNodes[0]);
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// 保存当前任务信息。
        /// </summary>
        public void Save()
        {
            if (this.Name == null || this.Name.Length == 0)
            {
                throw new Exception("任务名称必须输入。");
            }
            else if (this.Group == null)
            {
                throw new Exception("必须设置任务所属的任务组。");
            }
            else if (this.Period == null)
            {
                throw new Exception("必须配置任务的运行周期。");
            }

            AC.Base.Database.DbConnection dbConn = this.Application.GetDbConnection();
            if (dbConn != null)
            {
                try
                {
                    string strConfig = null;
                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);
                    xmlDoc.AppendChild(xnConfig);

                    System.Xml.XmlNode xnPeriodType = xmlDoc.CreateElement("PeriodType");
                    xnPeriodType.InnerText = this.Period.GetType().FullName;
                    xnConfig.AppendChild(xnPeriodType);

                    System.Xml.XmlNode xnPeriodConfig = this.Period.GetTaskPeriodConfig(xmlDoc);
                    if (xnPeriodConfig != null)
                    {
                        System.Xml.XmlNode xnPeriod = xmlDoc.CreateElement("Period");
                        xnPeriod.AppendChild(xnPeriodConfig);
                        xnConfig.AppendChild(xnPeriod);
                    }

                    System.Xml.XmlNode xnMaxRunTime = xmlDoc.CreateElement("MaxRunTime");
                    xnMaxRunTime.InnerText = this.MaxRunTime.ToString();
                    xnConfig.AppendChild(xnMaxRunTime);

                    System.Xml.XmlNode xnRetryProgress = xmlDoc.CreateElement("RetryProgress");
                    xnRetryProgress.InnerText = this.RetryProgress.ToString();
                    xnConfig.AppendChild(xnRetryProgress);

                    System.Xml.XmlNode xnIsUnqualifiedRetry = xmlDoc.CreateElement("IsUnqualifiedRetry");
                    xnIsUnqualifiedRetry.InnerText = this.IsUnqualifiedRetry.ToString();
                    xnConfig.AppendChild(xnIsUnqualifiedRetry);

                    System.Xml.XmlNode xnRetryInterval = xmlDoc.CreateElement("RetryInterval");
                    for (int i = 0; i < this.RetryInterval.Length; i++)
                    {
                        System.Xml.XmlNode xnRetryIntervalChild = xmlDoc.CreateElement(string.Format("Interval{0}", i + 1));
                        xnRetryIntervalChild.InnerText = this.RetryInterval[i].ToString();
                        xnRetryInterval.AppendChild(xnRetryIntervalChild);
                    }
                    xnConfig.AppendChild(xnRetryInterval);

                    System.Xml.XmlNode xnTaskConfig = this.GetTaskConfig(xmlDoc);
                    if (xnTaskConfig != null)
                    {
                        System.Xml.XmlNode xn = xmlDoc.CreateElement("TaskConfig");
                        xn.AppendChild(xnTaskConfig);
                        xnConfig.AppendChild(xn);
                    }

                    strConfig = xmlDoc.OuterXml;

                    if (this.TaskConfigId == 0)
                    {
                        string strSql = "SELECT MAX(" + Tables.TaskConfig.TaskConfigId + ") FROM " + Tables.TaskConfig.TableName;
                        this.TaskConfigId = Function.ToInt(dbConn.ExecuteScalar(strSql)) + 1;

                        strSql = this.TaskConfigId + "," + Function.SqlStr(this.TaskType.Code, 250) + "," + Function.SqlStr(this.Name, 250) + "," + this.Group.TaskGroupId + "," + Function.BoolToByte(this.Enable) + "," + Function.SqlStr(strConfig);
                        strSql = "INSERT INTO " + Tables.TaskConfig.TableName + " (" + Tables.TaskConfig.TaskConfigId + "," + Tables.TaskConfig.TaskType + "," + Tables.TaskConfig.Name + "," + Tables.TaskConfig.TaskGroupId + "," + Tables.TaskConfig.EnableAuto + "," + Tables.TaskConfig.XMLConfig + ") VALUES (" + strSql + ")";
                        dbConn.ExecuteNonQuery(strSql);

                        this.Group.TaskConfigs.Add(this);
                    }
                    else
                    {
                        string strSql = "UPDATE " + Tables.TaskConfig.TableName + " Set " + Tables.TaskConfig.Name + "=" + Function.SqlStr(this.Name, 250) + "," + Tables.TaskConfig.EnableAuto + "=" + Function.BoolToByte(this.Enable) + "," + Tables.Device.XMLConfig + "=" + Function.SqlStr(strConfig) + " Where " + Tables.TaskConfig.TaskConfigId + "=" + this.TaskConfigId;
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
        /// 删除当前任务。
        /// </summary>
        public void Delete()
        {
            AC.Base.Database.DbConnection dbConn = this.Application.GetDbConnection();
            if (dbConn != null)
            {
                try
                {
                    //删除任务
                    string strSql;
                    strSql = "Delete From " + Tables.TaskConfig.TableName + " Where " + Tables.TaskConfig.TaskConfigId + "=" + this.TaskConfigId;
                    dbConn.ExecuteNonQuery(strSql);

                    //删除任务日志(当年)
                    strSql = "Delete From " + Tables.TaskLog.GetTableName(DateTime.Today) + " Where " + Tables.TaskLog.TaskConfigId + "=" + this.TaskConfigId;
                    dbConn.ExecuteNonQuery(strSql);

                    //删除任务运行日志
                    //strSql = "Delete From " + Tables.TaskRunLog.GetTableName(DateTime.Today) + " Where " + Tables.TaskRunLog.TaskConfigId + "=" + this.TaskConfigId;
                    //dbConn.ExecuteNonQuery(strSql);
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
        /// 任务被创建后产生的事件。
        /// </summary>
        public event TaskCreatedEventHandler TaskCreated;

        /// <summary>
        /// 获取一个首次运行的任务对象的新实例。
        /// </summary>
        /// <param name="isAuto">当前任务是否是自动运行。</param>
        /// <param name="taskPlanTime">任务计划运行时间。当 IsAuto=true 时此属性表示任务自动运行的计划时间，当 IsAuto=false 时此属性表示任务手动启动的时间。</param>
        /// <param name="TestedDeviceSN">被测设备编号</param>
        /// <param name="TestedDeviceType"></param>
        /// <param name="FactoryClassifyID"></param>
        /// <param name="ModelClassifyID"></param>
        /// <returns></returns>
        public Task CreateTask(bool isAuto, DateTime taskPlanTime, string TestedDeviceSN, string TestedDeviceType, int FactoryClassifyID, int ModelClassifyID)
        {
            System.Reflection.ConstructorInfo ci = this.TaskType.Type.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            Task task = objInstance as Task;
            task.Config = this;

            TaskLog _TaskLog = new TaskLog(this, isAuto, taskPlanTime,"");
            task.Log = _TaskLog;
            if (this.TaskCreated != null)
            {
                this.TaskCreated(task);
            }

            return task;
        }

        public Task CreateTask()
        {
            System.Reflection.ConstructorInfo ci = this.TaskType.Type.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            Task task = objInstance as Task;
            task.Config = this;

            TaskLog _TaskLog = new TaskLog(this, true, DateTime.Now, string.Format("测试{0}{1}{2}", DateTime.Now.ToString(), DateTime.Now.Hour, DateTime.Now.Minute));
            task.Log = _TaskLog;
            if (this.TaskCreated != null)
            {
                this.TaskCreated(task);
            }
            return task;
        }

        /// <summary>
        /// 获取一个首次运行的任务对象的新实例。
        /// </summary>
        /// <param name="isAuto">当前任务是否是自动运行</param>
        /// <param name="taskPlanTime">任务计划运行时间。当 IsAuto=true 时此属性表示任务自动运行的计划时间，当 IsAuto=false 时此属性表示任务手动启动的时间。</param>
        /// <param name="taskName">任务名称</param>
        /// <returns></returns>
        public Task CreateTask(bool isAuto, DateTime taskPlanTime, string taskName,System.Windows.Forms.TreeListView tlvDevice)
        {
            System.Reflection.ConstructorInfo ci = this.TaskType.Type.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            Task task = objInstance as Task;
            task.Config = this;
            this.tlvDevice = tlvDevice;
            TaskLog _TaskLog = new TaskLog(this, isAuto, taskPlanTime, taskName);
            task.Log = _TaskLog;
            
            if (this.TaskCreated != null)
            {
                this.TaskCreated(task);
            }

            return task;
        }

        internal Task CreateTask(TaskLog taskLog, bool IsTriggerTaskCreated)
        {
            System.Reflection.ConstructorInfo ci = this.TaskType.Type.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            Task task = objInstance as Task;
            task.Config = this;
            task.Log = taskLog;
            if (IsTriggerTaskCreated && this.TaskCreated != null)
            {
                this.TaskCreated(task);
            }

            return task;
        }

        /// <summary>
        /// 确定指定的对象是否等于当前的对象。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is TaskConfig && this.TaskConfigId > 0)
            {
                TaskConfig task = obj as TaskConfig;
                if (task.TaskConfigId > 0 && task.TaskConfigId == this.TaskConfigId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 用作特定类型的哈希函数。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        #region ICloneable 成员

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            System.Reflection.ConstructorInfo ci = this.GetType().GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            TaskConfig taskConfig = objInstance as TaskConfig;
            taskConfig.Application = this.Application;
            taskConfig.TaskType = this.TaskType;
            taskConfig.Group = this.Group;
            taskConfig.TaskConfigId = this.TaskConfigId;
            taskConfig.m_Name = this.m_Name;
            taskConfig.Enable = this.Enable;
            taskConfig.m_Period = this.m_Period;
            taskConfig.MaxRunTime = this.MaxRunTime;
            taskConfig.m_RetryProgress = this.m_RetryProgress;
            taskConfig.m_RetryInterval = this.m_RetryInterval;
            return taskConfig;
        }

        #endregion
    }

    /// <summary>
    /// 任务配置集合。
    /// </summary>
    public class TaskConfigCollection : ReadOnlyCollection<TaskConfig>
    {
        internal TaskConfigCollection()
        {
        }

        internal new void Add(TaskConfig item)
        {
            base.Items.Add(item);
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "共 " + this.Count + " 个任务配置";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Exam.Exam;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 任务运行日志。
    /// </summary>
    public class TaskLog
    {
        /// <summary>
        /// 首次运行的任务日志。
        /// </summary>
        /// <param name="taskConfig"></param>
        /// <param name="isAuto"></param>
        /// <param name="taskPlanTime"></param>
        internal TaskLog(TaskConfig taskConfig, bool isAuto, DateTime taskPlanTime, string taskName)
        {
            this.Config = taskConfig;
            this.IsAuto = isAuto;
            this.PlanTime = taskPlanTime;
            this.TaskName = taskName;
            this.Save(0);
        }

        internal TaskLog(TaskConfig taskConfig, System.Data.IDataReader dr)
        {
            this.Config = taskConfig;
            this.TaskName = Function.ToString(dr[Tables.TaskLog.TaskName]);
            this.LogId = Function.ToLong(dr[Tables.TaskLog.TaskLogId]);
            this.IsAuto = Function.ToBool(dr[Tables.TaskLog.IsAuto]);
            this.PlanTime = new DateTime(Function.ToLong(dr[Tables.TaskLog.PlanTime]));
        }

        private void Save(int tryCount)
        {
            if (this.LogId > 0)
            {
                throw new Exception("任务运行日志 " + this.LogId + " 已保存过，不能再次保存");
            }

            DateTime dtmLodId = DateTime.Now;

            string strTableName = Tables.TaskLog.GetTableName(dtmLodId);
            Database.DbConnection dbConnection = this.Config.Application.GetDbConnection();

            if (dbConnection.TableIsExist(strTableName) == false)
            {
                dbConnection.CreateTable(typeof(Tables.TaskLog), strTableName);
            }

            try
            {
                string strSql = Function.SqlStr(dtmLodId.Ticks.ToString()) + "," + this.Config.TaskConfigId.ToString() + "," + Function.BoolToByte(this.IsAuto) + "," + this.PlanTime.Ticks.ToString() + ",0,0,"+Function.SqlStr(this.TaskName);
                strSql = "INSERT INTO " + strTableName + " (" + Tables.TaskLog.TaskLogId + "," + Tables.TaskLog.TaskConfigId + "," + Tables.TaskLog.IsAuto + "," + Tables.TaskLog.PlanTime + "," + Tables.TaskLog.RetryTimes + "," + Tables.TaskLog.LastProgress +","+Tables.TaskLog.TaskName+ ") VALUES (" + strSql + ")";
                dbConnection.ExecuteNonQuery(strSql);

                this.LogId = dtmLodId.Ticks;
            }
            catch
            {
                if (tryCount > 3)
                {
                    throw new Exception("保存任务运行日志 " + this.GetType().FullName + " 时发生错误，已重试3次。");
                }
                else
                {
                    this.Save(tryCount + 1);
                }
            }
            finally
            {
                dbConnection.Close();
            }
        }

        private bool IsRun = false;     //是否有重试任务正在运行。

        /// <summary>
        /// 获取一个重试运行的任务对象的新实例，该任务将继续运行前置任务未完成的工作。
        /// </summary>
        /// <param name="TestedDeviceSN">被测设备编号</param>
        /// <param name="TestedDeviceType">被测设备类型</param>
        /// <param name="FactoryClassifyID">厂商分类ID</param>
        /// <param name="ModelClassifyID">型号分类ID</param>
        /// <returns></returns>
        public Task CreateTask(string TestedDeviceSN, string TestedDeviceType, int FactoryClassifyID, int ModelClassifyID)
        {
            //if (this.Tasks.Count == 0)
            //{
            //    throw new Exception("未发现该任务的首次运行信息，必须等待首次运行结束后方可进行重试操作。");
            //}

            try
            {
                this.IsRun = true;
                Task task = this.Config.CreateTask(this, true);
                task.Stopped += new TaskStoppedEventHandler(task_Stopped);
                task.RetryTimes = this.Tasks.Count;
                return task;
            }
            catch (Exception ex)
            {
                this.IsRun = false;
                throw ex;
            }
        }

        
        public Task CreateTask(System.Windows.Forms.TreeListView tlvTask,int deviceType,int testType,string address,string suite,int factory)
        {
            if (IsRun)
                throw new Exception("重试任务正在运行，必须等待该任务结束后方可再次启动。");

            try
            {
                this.IsRun = true;
                Task task = this.Config.CreateTask(this, true);
                task.Stopped += new TaskStoppedEventHandler(task_Stopped);
                task.RetryTimes = this.Tasks.Count;
                task.TreeListview = tlvTask;
                task.Address = address;
                return task;
            }
            catch (Exception ex)
            {
                this.IsRun = false;
                throw ex;
            }
        }

        public Task CreateTask(System.Windows.Forms.TreeListView tlvTask, int index)
        {
            if (IsRun)
            {
                throw new Exception("重试任务正在运行，必须等待该任务结束后方可再次启动。");
            }

            try
            {
                this.IsRun = true;
                Task task = this.Config.CreateTask(this, true);
                task.Stopped += new TaskStoppedEventHandler(task_Stopped);
                task.RetryTimes = this.Tasks.Count;
                task.TreeListview = tlvTask;
                task.index = index;
                task.taskName = "";
                return task;
            }
            catch (Exception ex)
            {
                this.IsRun = false;
                throw ex;
            }
        }

        
        internal void AddTask(System.Data.IDataReader dr)
        {
            Task task = this.Config.CreateTask(this, false);
            task.SetDataReader(dr);
            this.Tasks.Add(task);
        }

        private void task_Stopped(Task task)
        {
            task.Stopped -= new TaskStoppedEventHandler(task_Stopped);
            this.IsRun = false;
        }

        /// <summary>
        /// 该任务日志的配置信息。
        /// </summary>
        public TaskConfig Config { get; private set; }

        private long m_LogId;
        /// <summary>
        /// 任务日志 ID。该 ID 同时表示此日志的日期和时间的计时周期数，表示自 0001 年 1 月 1 日午夜 12:00:00 以来经过的以 100 纳秒为间隔的间隔数。每个计时周期表示一百纳秒，即一千万分之一秒。1 毫秒内有 10,000 个计时周期。
        /// </summary>
        public long LogId
        {
            get
            {
                return this.m_LogId;
            }
            private set
            {
                this.m_LogId = value;
                this.m_LogTime = new DateTime(this.m_LogId);
            }
        }

        private DateTime m_LogTime;
        /// <summary>
        /// 任务日志时间，该时间是 TaskLogId 属性的时间表现形式。
        /// </summary>
        public DateTime LogTime
        {
            get
            {
                return m_LogTime;
            }
        }

        /// <summary>
        /// 该任务首次是否为自动运行。
        /// </summary>
        public bool IsAuto { get; private set; }

        /// <summary>
        /// 计划执行时间。
        /// </summary>
        public DateTime PlanTime { get; private set; }

        


        /// <summary>
        /// 任务名称。
        /// </summary>
        public String TaskName { get; set; }

        private TaskCollection m_Tasks;
        /// <summary>
        /// 该任务详细的执行情况。
        /// </summary>
        public TaskCollection Tasks
        {
            get
            {
                if (this.m_Tasks == null)
                {
                    this.m_Tasks = new TaskCollection();
                }
                return this.m_Tasks;
            }
        }
    }

    /// <summary>
    /// 任务日志集合。
    /// </summary>
    public class TaskLogCollection : System.Collections.Generic.List<TaskLog>
    {
    }
}

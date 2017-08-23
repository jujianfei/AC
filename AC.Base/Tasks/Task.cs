using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Exam.Exam;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 任务开始后产生的事件所调用的委托。
    /// </summary>
    /// <param name="task">产生事件的任务。</param>
    public delegate void TaskStartedEventHandler(Task task);

    /// <summary>
    /// 任务结束后产生的事件所调用的委托。
    /// </summary>
    /// <param name="task">产生事件的任务。</param>
    public delegate void TaskStoppedEventHandler(Task task);

    /// <summary>
    /// 任务执行进度发生变化产生的事件所调用的委托。
    /// </summary>
    /// <param name="type">类型，false代表处理中，true代表结束</param>
    /// <param name="result">true代表成功，false代表失败</param>
    /// <param name="index">Tree第一层行号</param>
    /// <param name="row">行号</param>
    /// <param name="message">返回打印信息</param>
    public delegate void TaskChangedEventHandler(bool type, bool result, int index, int row, string message);

   /// <summary>
    ///  任务执行进度发生变化产生的事件所调用的委托。
   /// </summary>
   /// <param name="progress">进度百分比</param>
   /// <param name="sn">类型码</param>
   /// <param name="message">信息</param>
    public delegate void TaskProgressChangedEventHandler(decimal progress , int sn , string message);

    /// <summary>
    /// 任务状态改变后产生的事件所调用的委托。
    /// </summary>
    /// <param name="task">状态发生改变的任务。</param>
    /// <param name="state">改变后的状态。</param>
    public delegate void TaskStateChangedEventHandler(Task task, TaskStateOptions state);

    /// <summary>
    /// 任务产生的异常事件所调用的委托。
    /// </summary>
    /// <param name="task">产生异常的任务。</param>
    /// <param name="exception">异常信息。</param>
    public delegate void TaskExceptionEventHandler(Task task, Exception exception);

    /// <summary>
    /// 自动任务基类，表示一个可以按周期定时运行或由操作员手动启动运行的完成一系列操作的任务。继承该基类的实体类必须提供一个无参数的构造函数，且必须添加 TaskTypeAttribute 特性；另外还需新建一个继承自 TaskConfig 的任务配置类，并在 TaskTypeAttribute 特性的 configType 参数中指明该配置的类型声明。
    /// </summary>
    public abstract class Task
    {
        /// <summary>
        /// 获取该任务的配置对象。
        /// </summary>
        public TaskConfig Config { get; internal set; }

        /// <summary>
        /// 获取该任务的运行日志。
        /// </summary>
        public TaskLog Log { get; internal set; }

        /// <summary>
        /// 获取该任务是第几次运行。0 表示该任务首次运行；1 表示该任务已经运行过一次，本次是第一次重试。
        /// </summary>
        public int RetryTimes { get; internal set; }

        /// <summary>
        /// 测试项
        /// </summary>
        public System.Windows.Forms.TreeListView TreeListview { get; set; }

        /// <summary>
        /// 任务执行进度发生变化产生的事件。
        /// </summary>
        public event TaskChangedEventHandler TaskChanged;

        /// <summary>
        /// 开始运行时间
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// 结束运行时间
        /// </summary>
        public DateTime StopTime { get; protected set; }

        /// <summary>
        /// 测试名称
        /// </summary>
        public string taskName { get; set; }

        /// <summary>
        /// 设备地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 测试设备列表id
        /// </summary>
        public int index { get; set; }

        private TaskStateOptions m_State = TaskStateOptions.Start;
        /// <summary>
        /// 获取任务当前状态。
        /// </summary>
        public TaskStateOptions State
        {
            get { return this.m_State; }
            private set
            {
                this.m_State = value;
                if (this.StateChanged != null)
                {
                    this.StateChanged(this, this.State);
                }
            }
        }

        /// <summary>
        /// 任务状态改变后产生的事件。
        /// </summary>
        public event TaskStateChangedEventHandler StateChanged;

        /// <summary>
        /// 任务开始后产生的事件。
        /// </summary>
        public event TaskStartedEventHandler Started;

        /// <summary>
        /// 任务结束后产生的事件。
        /// </summary>
        public event TaskStoppedEventHandler Stopped;

        /// <summary>
        /// 获取当前任务执行进度。
        /// </summary>
        public decimal Progress { get; private set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public String DeviceName { get;  set; }

        /// <summary>
        /// 文件索引
        /// </summary>
        public String TimeStr { get;  set; }
        

        /// <summary>
        /// 任务执行进度发生变化产生的事件。
        /// </summary>
        public event TaskProgressChangedEventHandler ProgressChanged;

        private TaskExceptionInfoCollection m_ExceptionInfos;
        /// <summary>
        /// 任务运行时产生的异常信息。
        /// </summary>
        public TaskExceptionInfoCollection ExceptionInfos
        {
            get
            {
                if (this.m_ExceptionInfos == null)
                {
                    this.m_ExceptionInfos = new TaskExceptionInfoCollection();
                }
                return this.m_ExceptionInfos;
            }
        }

        /// <summary>
        /// 任务运行时产生的异常事件。
        /// </summary>
        public event TaskExceptionEventHandler TaskException;

        private System.Threading.Thread thrStart;   //启动任务的线程。
        private System.Threading.Thread thrAbort;   //调用 Abort 方法强行停止任务的线程。

        /// <summary>
        /// 启动并执行任务。
        /// </summary>
        public void Start()
        {
            try
            {
                if (this.State != TaskStateOptions.Start)
                {
                    switch (this.State)
                    {
                        case TaskStateOptions.Running:
                            throw new NotImplementedException("任务正在运行中，不得再次开始。");

                        case TaskStateOptions.Done:
                            throw new NotImplementedException("任务已运行完毕，不得再次开始。");

                        case TaskStateOptions.Stopping:
                            throw new NotImplementedException("任务正在停止中，不得再次开始。");

                        case TaskStateOptions.Stop:
                            throw new NotImplementedException("任务已手动停止，不得再次开始。");

                        case TaskStateOptions.Exception:
                            throw new NotImplementedException("任务已异常终止，不得再次开始。");

                        default:
                            throw new NotImplementedException("尚未实现该枚举。");
                    }
                }

                this.thrStart = System.Threading.Thread.CurrentThread;
                this.StartTime = DateTime.Now;
                this.StopTime = this.StartTime;
                this.Progress = 0;

                if (this.Started != null)
                {
                    this.Started(this);
                }

                this.State = TaskStateOptions.Running;

                try
                {

                    this.Run();

                    if (this.State != TaskStateOptions.Running)
                    {
                        return;
                    }

                    this.State = TaskStateOptions.Done;
                }
                catch (Exception ex)
                {
                    this.OnException(ex.Message);

                    if (this.State != TaskStateOptions.Running)
                    {
                        return;
                    }
                    this.State = TaskStateOptions.Exception;
                }
                this.Save();
            }
            catch (Exception exx)
            {
                this.OnException(exx.ToString() + "\r\n" + this.State.GetDescription());
            }
        }

        /// <summary>
        /// 强行停止任务。
        /// </summary>
        /// <param name="timeout">延迟强行停止的时间(毫秒)。调用该方法后会首先调用继承任务的 Abort 方法让任务安全终止，如果在该指定时间内任务任然未能终止，则将强行终止该任务的线程。</param>
        public void Stop(int timeout)
        {
            if (this.State == TaskStateOptions.Start)
            {
                //throw new NotImplementedException("必须首先运行任务后方可调用 Stop 方法强行停止任务。");
                return;
            }

            if (this.State == TaskStateOptions.Running)
            {
                DateTime dtmStop = DateTime.Now;

                this.State = TaskStateOptions.Stopping;

                this.thrAbort = new System.Threading.Thread(new System.Threading.ThreadStart(this.AbortThread));
                this.thrAbort.IsBackground = true;
                this.thrAbort.Start();

                //延迟多少毫秒后任务任然没有停止，则强行终止线程

                while (dtmStop.AddMilliseconds(timeout) > DateTime.Now)
                {
                    if (this.thrStart.ThreadState == System.Threading.ThreadState.Stopped)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(10);
                }

                while (this.thrStart.ThreadState != System.Threading.ThreadState.Stopped && this.thrStart.ThreadState != System.Threading.ThreadState.Aborted)
                {
                    this.thrStart.Abort();
                    this.thrStart.Join(100);
                }

                while (this.thrAbort.ThreadState != System.Threading.ThreadState.Stopped && this.thrAbort.ThreadState != System.Threading.ThreadState.Aborted)
                {
                    this.thrAbort.Abort();
                    this.thrAbort.Join(100);
                }

                this.State = TaskStateOptions.Stop;

                this.Save();
            }
        }

        private void AbortThread()
        {
            try
            {
                this.Abort();
            }
            catch (Exception ex)
            {
                this.OnException(ex);
            }
        }

        internal void SetDataReader(System.Data.IDataReader dr)
        {
            this.RetryTimes = Function.ToInt(dr[Tables.TaskRunLog.RetryTimes]);
            this.taskName = dr[Tables.TaskRunLog.DeviceName]+"";
            this.StartTime = new DateTime(Function.ToLong(dr[Tables.TaskRunLog.StartTime]));
            this.StopTime = new DateTime(Function.ToLong(dr[Tables.TaskRunLog.StopTime]));
            this.m_State = (TaskStateOptions)Function.ToInt(dr[Tables.TaskRunLog.TaskState]);
            this.Progress = Function.ToDecimal(dr[Tables.TaskRunLog.TaskProgress]);
            this.DeviceName = Function.ToString(dr[Tables.TaskRunLog.DeviceName]);
            this.TimeStr = Function.ToString(dr[Tables.TaskRunLog.TimeStr]);
            object objConfig = dr[Tables.TaskConfig.XMLConfig];
            if (objConfig != null && !(objConfig is System.DBNull))
            {
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                try
                {
                    xmlDoc.LoadXml(objConfig.ToString());

                    if (xmlDoc.ChildNodes.Count > 0)
                    {
                        this.SetTaskRunConfig(xmlDoc.ChildNodes[0]);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        //任务运行完成后必须调用该方法
        private void Save()
        {
            this.StopTime = DateTime.Now;
            AC.Base.Database.DbConnection dbConn = this.Config.Application.GetDbConnection();
            if (dbConn != null)
            {
                try
                {
                    string strTableName = Tables.TaskRunLog.GetTableName(this.Log.LogTime);
                    if (dbConn.TableIsExist(strTableName) == false)
                        dbConn.CreateTable(typeof(Tables.TaskRunLog), strTableName);
                    string strConfig = null;
                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    System.Xml.XmlNode xnConfig = this.GetTaskRunConfig(xmlDoc);
                    if (xnConfig != null)
                    {
                        xmlDoc.AppendChild(xnConfig);
                        strConfig = xmlDoc.OuterXml;
                    }
                    this.TimeStr = this.Log.LogId.ToString() + "_" + this.RetryTimes;

                    string strSql = Function.SqlStr(this.Log.LogId.ToString()) + "," + 
                                    this.RetryTimes + "," +
                                     this.StartTime.Ticks + "," +
                                    this.StopTime.Ticks + "," +
                                    (this.StopTime - this.StartTime).TotalMilliseconds + "," +
                                    ((int)this.State) + "," +
                                    this.Progress + "," +
                                     Function.SqlStr( this.taskName) + "," +
                                     Function.SqlStr(this.TimeStr) + "," +
                                    Function.SqlStr(strConfig);

                    strSql = "INSERT INTO " + strTableName + " (" + Tables.TaskRunLog.TaskLogId + "," + Tables.TaskRunLog.RetryTimes + "," + Tables.TaskRunLog.StartTime + "," + Tables.TaskRunLog.StopTime + "," + Tables.TaskRunLog.TimeSpan + "," + Tables.TaskRunLog.TaskState + "," + Tables.TaskRunLog.TaskProgress + "," + Tables.TaskRunLog.DeviceName + "," +Tables.TaskRunLog.TimeStr+","+ Tables.TaskRunLog.XMLConfig + ") VALUES (" + strSql + ")";
                    dbConn.ExecuteNonQuery(strSql);

                    strTableName = Tables.TaskLog.GetTableName(this.Log.LogTime);
                    strSql = "UPDATE " + strTableName + " Set " + Tables.TaskLog.RetryTimes + "=" + this.RetryTimes + "," + Tables.TaskLog.LastProgress + "=" + this.Progress + " Where " + Tables.TaskLog.TaskLogId + "=" + Function.SqlStr(this.Log.LogId.ToString());
                    dbConn.ExecuteNonQuery(strSql);

                    if (this.ExceptionInfos.Count > 0)
                    {
                        strTableName = Tables.TaskExceptionLog.GetTableName(this.Log.LogTime);
                        if (dbConn.TableIsExist(strTableName) == false)
                            dbConn.CreateTable(typeof(Tables.TaskExceptionLog), strTableName);

                        for (int intIndex = 0; intIndex < this.ExceptionInfos.Count; intIndex++)
                        {
                            strSql = this.Log.LogId + "," + this.RetryTimes + "," + (intIndex + 1) + "," + this.ExceptionInfos[intIndex].ExceptionTime.Ticks + "," + Function.SqlStr(this.ExceptionInfos[intIndex].ExceptionInfo);
                            strSql = "INSERT INTO " + strTableName + " (" + Tables.TaskExceptionLog.TaskLogId + "," + Tables.TaskExceptionLog.RetryTimes + "," + Tables.TaskExceptionLog.ExceptionId + "," + Tables.TaskExceptionLog.ExceptionTime + "," + Tables.TaskExceptionLog.XMLConfig + ") VALUES (" + strSql + ")";
                            dbConn.ExecuteNonQuery(strSql);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.OnException(ex);
                }
                finally
                {
                    dbConn.Close();
                }
            }

            this.Log.Tasks.Add(this);

            if (this.Stopped != null)
            {
                try
                {
                    this.Stopped(this);
                }
                catch (Exception ex)
                {
                    this.OnException(ex.Message);
                }
            }
        }

        protected void OnTaskChanged(bool type, bool result, int index, int row, string message)
        {
            if (this.TaskChanged != null)
            {
                this.TaskChanged(type, result, index, row, DateTime.Now.ToString("hh:mm:ss.fff") + ":" + message);
            }
        }
       

        /// <summary>
        /// 任务运行中进度发生改变后调用的方法。
        /// </summary>
        /// <param name="maxValue">该任务预计执行的总工作量，该值从 1 开始计算。</param>
        /// <param name="currentValue">该任务当前完成的工作量，该值应大于等于 1 并且小于等于 maxValue，当任务执行到最后如果 currentValue 不等于 maxValue 则视为该任务未全部完成。</param>
        /// <param name="message">任务当前的运行消息，例如正在召测的设备或处理的数据项等。</param>
        protected void OnProgressChanged(int maxValue, int currentValue, int sn ,string message)
        {
            if (this.ProgressChanged != null)
            {
                decimal decProgress = 0;
                if (maxValue > 0 && currentValue >= 0)
                {
                    if (currentValue > maxValue)
                    {
                        decProgress = 100;
                    }
                    else
                    {
                        decProgress = (decimal)currentValue / (decimal)maxValue;
                    }
                }
                this.Progress = decProgress;
                this.ProgressChanged(decProgress, sn , message);
            }
        }

        /// <summary>
        /// 任务运行中产生的异常消息。
        /// </summary>
        /// <param name="exceptionMessage"></param>
        protected void OnException(string exceptionMessage)
        {
            this.OnException(new Exception(exceptionMessage));
        }

        /// <summary>
        /// 任务运行中产生的异常。
        /// </summary>
        /// <param name="exception"></param>
        protected void OnException(Exception exception)
        {
            this.ExceptionInfos.Add(exception);

            if (this.TaskException != null)
            {
                this.TaskException(this, exception);
            }
        }

        /// <summary>
        /// 开始运行任务。
        /// </summary>
        protected abstract void Run();

        /// <summary>
        /// 终止任务。
        /// </summary>
        protected abstract void Abort();

        /// <summary>
        /// 获取任务当前运行状态配置信息。通常为任务运行完毕保存任务运行日志时调用该方法，以便日后读取该任务运行日志时还原任务的运行情况。
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        protected abstract System.Xml.XmlNode GetTaskRunConfig(System.Xml.XmlDocument xmlDoc);

        /// <summary>
        /// 设置任务当前运行状态配置信息。
        /// </summary>
        /// <param name="runConfig"></param>
        protected abstract void SetTaskRunConfig(System.Xml.XmlNode runConfig);

        /// <summary>
        /// 向页面输出 HTML 格式的任务运行情况报表，使用 UTF-8 编码格式。
        /// </summary>
        /// <param name="output">字符输出对象，调用 WriteLine 方法向界面输出 HTML 内容。</param>
        public abstract void WriterHtml(System.IO.TextWriter output);
    }

    /// <summary>
    /// 自动任务集合。
    /// </summary>
    public class TaskCollection : ReadOnlyCollection<Task>
    {
        internal TaskCollection()
        {
        }

        internal new void Add(Task item)
        {
            base.Items.Add(item);
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "共 " + this.Count + " 个自动任务";
        }
    }
}

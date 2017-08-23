using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Tasks;
using AC.Base.Database;

namespace AC.Base.Exam.Exam
{

    public struct TaskObject 
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public const string TableName = "TaskRecord";

        /// <summary>
        /// 任务配置编号
        /// </summary>
        
        public const string TaskConfigId = "TaskConfigId";

        /// <summary>
        /// 任务类型
        /// </summary>
       
        public const string TaskType = "TaskType";

        /// <summary>
        /// 任务名称
        /// </summary>
        
        public const string Name = "TaskName";

        /// <summary>
        /// 所属任务组
        /// </summary>
        
        public const string TaskGroupId = "TaskGroup";

        /// <summary>
        /// 允许自动运行
        /// </summary>
      
        public const string EnableAuto = "EnableAuto";

        /// <summary>
        /// 配置信息
        /// </summary>
        
        public const string XMLConfig = "XMLConfig";
    }
}

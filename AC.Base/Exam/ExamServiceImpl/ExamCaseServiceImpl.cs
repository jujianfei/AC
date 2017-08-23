using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Exam.ExamService;
using System.Threading;

namespace AC.Base.Exam.ExamServiceImpl
{
    public class ExamCaseServiceImpl : ExamCaseService
    {
        /// <summary>
        /// 测试项执行任务接口
        /// </summary>
        /// <param name="examItem">0代表（通讯单元），1代表（通讯设备）</param>
        /// <param name="examDevice">0代表（集中器），1代表（表）</param>
        /// <param name="protocol">0代表（本地通信协议），1代表（远程通信协议），如果examdevice为1时，本值为null</param>
        /// <param name="funCode">测试项功能代码</param>
        /// <param name="address">设备地址</param>
        /// <param name="record">打印日志队列，这边只需要往队列里面塞值即可：</param>
        /// <returns>true代表（成功），false代表（失败）</returns>
        public bool exam(byte examItem, byte examDevice, byte protocol, byte funCode, string address,Queue<String> record)
        {
            Thread.Sleep(1000);
            return true;
        }


        public bool exam(string funCode)
        {
            Thread.Sleep(1000);
            return true;
        }
    }
}

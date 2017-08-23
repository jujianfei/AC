using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Exam.Exam
{
    public class ExamRecordValue
    {
        private int device;

        public int Device
        {
            get { return device; }
            set { device = value; }
        }

        private List<ExamRecordValueDevice> examRecordValueDeviceList = new List<ExamRecordValueDevice>();

        public List<ExamRecordValueDevice> ExamRecordValueDeviceList
        {
            get { return examRecordValueDeviceList; }
            set { examRecordValueDeviceList = value; }
        }

       
    }
}

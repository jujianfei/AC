using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Exam.Exam
{
    public class ExamRecordValueDevice
    {
        private int item;

        public int Item
        {
            get { return item; }
            set { item = value; }
        }
        private int result;

        public int Result
        {
            get { return result; }
            set { result = value; }
        }
        private int recordId;

        public int RecordId
        {
            get { return recordId; }
            set { recordId = value; }
        }
    }
}

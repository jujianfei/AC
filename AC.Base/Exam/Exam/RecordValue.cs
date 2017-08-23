using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Exam.Exam
{
    public class RecordValue
    {
        private string recordId;

        public string RecordId
        {
            get { return recordId; }
            set { recordId = value; }
        }

        private string value;

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}

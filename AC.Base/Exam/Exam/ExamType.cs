using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Exam.Exam
{
    public class ExamType
    {
        private int baseKey;

        public int BaseKey
        {
            get { return baseKey; }
            set { baseKey = value; }
        }

       
        private int testType;

        public int TestType
        {
            get { return testType; }
            set { testType = value; }
        }

        private int item;

        public int Item
        {
            get { return item; }
            set { item = value; }
        }


    }
}

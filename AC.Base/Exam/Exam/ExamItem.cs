using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base;

namespace AC.Base.Exam.Exam
{
    public class ExamItem
    {
        private ApplicationClass application;

        public ApplicationClass Application
        {
            get { return application; }
            set { application = value; }
        }

        private int baseKey;

        public int BaseKey
        {
            get { return baseKey; }
            set { baseKey = value; }
        }
        private string item;

        public string Item
        {
            get { return item; }
            set { item = value; }
        }
        private string funCode;

        public string FunCode
        {
            get { return funCode; }
            set { funCode = value; }
        }
        private int parentId;

        public int ParentId
        {
            get { return parentId; }
            set { parentId = value; }
        }
    }
}

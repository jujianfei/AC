using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Exam.Exam;
using AC.Base.Exam.ExamService;
using AC.Base;
using AC.Base.Exam.ExamDao;

namespace AC.Base.Exam.ExamServiceImpl
{
    public class ExamTypeServiceImpl : ExamTypeService
    {
        private ExamTypeDao examTypeDao;
        public ExamTypeServiceImpl(ApplicationClass application)
        {
            if (examTypeDao == null)
            {
                examTypeDao = new ExamTypeDao(application); 
            }
        }

        public List<ExamType> getExamType(int testType)
        {
            String querysql = " and 1>0 ";
            querysql += " and TestType=" + testType;
            return this.examTypeDao.select(querysql);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Exam.ExamDao;
using AC.Base;
using AC.Base.Exam.Exam;

namespace AC.Base.Exam.ExamServiceImpl
{
    public class ExamRecordServiceImpl
    {
        private ExamRecordDao examRecordDao;
         public ExamRecordServiceImpl(ApplicationClass application)
        {
            if (examRecordDao == null)
            {
                examRecordDao = new ExamRecordDao(application); 
            }
        }


         public void insertExamRecord(ExamRecord examRecord)
        {
            examRecordDao.insert(examRecord);
        }

         public int updateExamRecord(ExamRecord examRecord)
        {
            return examRecordDao.update(examRecord);
        }

         public ExamRecord queryExamRecord(string taskConfigId)
        {
            List<ExamRecord>  examRecordList=examRecordDao.select(" and taskConfigId=" + taskConfigId);
            return examRecordList.Count > 0 ? examRecordList[0] : null;
        }

         public void deleteExamRecord(string taskConfigId)
         {
             examRecordDao.delete(" and taskConfigId=" + taskConfigId);
         }

        
    }
}

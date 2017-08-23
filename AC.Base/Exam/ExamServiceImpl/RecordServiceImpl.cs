using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Exam.ExamDao;
using AC.Base;
using AC.Base.Exam.Exam;

namespace AC.Base.Exam.ExamServiceImpl
{
    public class RecordServiceImpl
    {
         private RecordDao recordDao;
         public RecordServiceImpl(ApplicationClass application)
        {
            if (recordDao == null)
            {
                recordDao = new RecordDao(application); 
            }
        }


         public void insertRecord(Record record)
        {
            recordDao.insert(record);
        }

         public int updateRecord(Record record)
        {
            return recordDao.update(record);
        }

         public Record queryRecord(String TaskConfigId)
         {
             List<Record> records = recordDao.select(" and TaskConfigId = " + TaskConfigId);
            if (records.Count > 0)
            {
                return records[0];
            }
            else
            {
                return null;
            }
         }

       

    }
}

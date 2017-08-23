using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Exam.ExamService;
using AC.Base.Exam.Exam;
using AC.Base;
using AC.Base.Exam.ExamDao;


namespace AC.Base.Exam.ExamServiceImpl
{
    public class ExamItemServiceImpl : ExamItemService
    {
        private ExamItemDao examItemDao;
        public ExamItemServiceImpl(ApplicationClass application)
        {
            if (examItemDao == null)
            {
                examItemDao = new ExamItemDao(application); 
            }
        }
        public List<ExamItem> getExamItem(int baseKey)
        {
            return examItemDao.select(baseKey);
        }

        public List<ExamItem> getChildExamItem(int baseKey)
        {
            return examItemDao.select(" and parentId=" + baseKey);
        }

        public List<ExamItem> getExamItem()
        {
            return examItemDao.select();
        }

        public ExamItem queryExamItem(int baseKey)
        {
            List<ExamItem> examItemList = examItemDao.select(baseKey);
            return examItemList.Count > 0 ? examItemList[0] : null;
        }

        public ExamItem getExamItem(string funCode)
        {
            List<ExamItem> examItemList = examItemDao.select(" and FunCode='" + funCode + "'");
            return examItemList.Count > 0 ? examItemList[0] : null;
        }
    }
}

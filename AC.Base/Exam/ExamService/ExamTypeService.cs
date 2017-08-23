using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Exam.ExamServiceImpl;
using AC.Base.Exam.Exam;

namespace AC.Base.Exam.ExamService
{
    interface ExamTypeService
    {
        #region 根据选择内容获取测试类别
        List<ExamType> getExamType(int testType);
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Exam.Exam;



namespace AC.Base.Exam.ExamService
{
    interface ExamItemService
    {
        #region <<获取测试项>>

        List<ExamItem> getExamItem(int baseKey);

        #endregion

        #region <<通过父类找到对应的子类>>

        List<ExamItem> getChildExamItem(int baseKey);

        #endregion

    }
}

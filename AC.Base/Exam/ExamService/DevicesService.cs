using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Exam.Exam;

namespace AC.Base.Exam.ExamService
{
    public interface DevicesService
    {
        int insertDevices(Devices devices);

        void updateDevcies(Devices devices);

        void deleteDevices(String taskConfigId);
    }
}

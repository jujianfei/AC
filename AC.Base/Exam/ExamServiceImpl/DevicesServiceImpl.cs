using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Exam.ExamService;
using AC.Base.Exam.Exam;
using AC.Base.Exam.ExamDao;
using AC.Base;


namespace AC.Base.Exam.ExamServiceImpl
{
    public class DevicesServiceImpl:DevicesService
    {
        private DevicesDao devicesDao;
        public DevicesServiceImpl(ApplicationClass application)
        {
            if (devicesDao == null)
            {
                devicesDao = new DevicesDao(application); 
            }
        }


        public int insertDevices(Devices devices)
        {
            return devicesDao.insert(devices);
            
        }

        public void updateDevcies(Devices devices)
        {
            devicesDao.update(devices);
        }

        public List<Devices> getDevices(string taskConfigId)
        {
            return devicesDao.select(" and TaskConfigId=" + taskConfigId);
        }

        public void deleteDevices(String taskConfigId)
        {
            devicesDao.delete(" and taskConfigId=" + taskConfigId);
        }
    }
}

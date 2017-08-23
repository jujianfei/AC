using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Exam.ExamDao;
using AC.Base;
using AC.Base.Exam.Exam;

namespace AC.Base.Exam.ExamServiceImpl
{
    public class DevicesTypeServiceImpl
    {

        private DevicesTypeDao devicesTypeDao;
        public DevicesTypeServiceImpl(ApplicationClass application)
        {
            if (devicesTypeDao == null)
            {
                devicesTypeDao = new DevicesTypeDao(application); 
            }
        }

        public List<DevicesType> getDevicesType()
        {
            return devicesTypeDao.select("");
        }

        public DevicesType getDevicesType(int baseKey)
        {
            List<DevicesType> devicesTypes = devicesTypeDao.select(" and baseKey=" + baseKey);
            return devicesTypes.Count > 0 ? devicesTypes[0] : null;
        }

       
    }
}

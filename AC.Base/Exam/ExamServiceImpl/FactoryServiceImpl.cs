using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Exam.ExamDao;
using AC.Base;
using AC.Base.Exam.Exam;

namespace AC.Base.Exam.ExamServiceImpl
{
    public class FactoryServiceImpl
    {
        private FactoryDao factoryDao;
        public FactoryServiceImpl(ApplicationClass application)
        {
            if (factoryDao == null)
            {
                factoryDao = new FactoryDao(application); 
            }
        }

        public List<Factory> getFactorys()
        {
            return factoryDao.select();
        }

        public void insertFactory(Factory factory)
        {
            factoryDao.insert(factory);
        }

        public void updateFactory(Factory factory)
        {
            factoryDao.update(factory);
        }

        public void delFactory(Factory factory)
        {
            factoryDao.delete(factory);
        }

        public Factory getFactorys(string basekey)
        {
            List<Factory> factoryList= factoryDao.select(" and basekey=" + basekey==""?"null":basekey);
            return factoryList.Count > 0 ? factoryList[0] : null;
        }

    }
}

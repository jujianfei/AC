using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Exam.Exam
{
    public class Factory
    {
        /// <summary>
        /// 主键
        /// </summary>
        private int baseKey;

        public int BaseKey
        {
            get { return baseKey; }
            set { baseKey = value; }
        }

        /// <summary>
        /// 厂家型号
        /// </summary>
        private string factoryName;

        public string FactoryName
        {
            get { return factoryName; }
            set { factoryName = value; }
        }

    }
}

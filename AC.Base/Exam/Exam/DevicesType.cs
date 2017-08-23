using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Exam.Exam
{
    public class DevicesType
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
        /// 设备类型
        /// </summary>
        private string devicesTypeName;

        public string DevicesTypeName
        {
            get { return devicesTypeName; }
            set { devicesTypeName = value; }
        }

        


    }
}

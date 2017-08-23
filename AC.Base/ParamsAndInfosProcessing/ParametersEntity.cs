using System;
using System.Collections.Generic;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// ParametersEntity实体类()
    /// </summary>
    public class ParametersEntity
    {
        #region "Private Variables"
        /// <summary>
        /// 参数字段名
        /// </summary>
        private String _PP_ParaName = string.Empty; // 参数字段名
        /// <summary>
        /// 参数值
        /// </summary>
        private String _PP_ParaValue = string.Empty; // 参数值
        /// <summary>
        /// 参数值的数据类型
        /// </summary>
        private String _PP_ParaValueType = string.Empty; // 参数值的数据类型
        #endregion

        #region "Public Variables"
        /// <summary>
        /// 参数字段名
        /// </summary>
        public String PP_ParaName
        {
            set { this._PP_ParaName = value; }
            get { return this._PP_ParaName; }
        }
        /// <summary>
        /// 参数值
        /// </summary>
        public String PP_ParaValue
        {
            set { this._PP_ParaValue = value; }
            get { return this._PP_ParaValue; }
        }
        /// <summary>
        /// 参数值的数据类型
        /// </summary>
        public String PP_ParaValueType
        {
            set { this._PP_ParaValueType = value; }
            get { return this._PP_ParaValueType; }
        }
        #endregion
    }
}
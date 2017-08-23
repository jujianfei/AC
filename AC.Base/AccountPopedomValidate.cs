using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 帐号权限验证。
    /// </summary>
    public class AccountPopedomValidate
    {
        /// <summary>
        /// 初始化帐号权限验证的新实例。
        /// </summary>
        public AccountPopedomValidate()
        {
        }

        /// <summary>
        /// 初始化帐号权限验证的新实例。
        /// </summary>
        /// <param name="validate">权限验证结果。如果验证结果为 Succeed 或 Loser，则 Popedom 属性为 null 并且无需判断 Popedom 属性；如果验证结果为 Setting 则需继续检查 Popedom 属性的设置信息。</param>
        /// <param name="popedom">该权限的详细配置</param>
        public AccountPopedomValidate(AccountPopedomValidateOptions validate, IPopedom popedom)
        {
            this.m_Validate = validate;
            this.m_Popedom = popedom;
        }

        private AccountPopedomValidateOptions m_Validate = AccountPopedomValidateOptions.Loser;
        /// <summary>
        /// 权限验证结果。
        /// </summary>
        public AccountPopedomValidateOptions Validate
        {
            get { return m_Validate; }
            set { m_Validate = value; }
        }

        private IPopedom m_Popedom;
        /// <summary>
        /// 该权限的详细配置。如无配置则返回 null，表示该项权限无任何限制。
        /// 为使权限验证的性能提高，对于无任何限制的权限应该返回 null。
        /// </summary>
        public IPopedom Popedom
        {
            get { return m_Popedom; }
            set { m_Popedom = value; }
        }
    }
}

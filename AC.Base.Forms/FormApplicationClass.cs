using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base;
using AC.Base.Database;

namespace AC.Base.Forms
{
    /// <summary>
    /// 窗体应用程序框架。
    /// </summary>
    public abstract class FormApplicationClass : ApplicationClass
    {
        /// <summary>
        /// 初始化窗体应用程序框架。
        /// </summary>
        protected FormApplicationClass()
        {
        }

        /// <summary>
        /// 向应用程序框架内添加对象类型。
        /// </summary>
        /// <param name="type">欲添加的类型的声明。</param>
        public override void AddType(Type type)
        {
            if (IsDevicePlugin(type))
            {
                //向设备插件类型信息中添加新的插件类型
                if (this.AddDevicePluginType(type, true))
                {
                    //如果成功添加则检查 m_DevicePluginTypes 中是否有可以继续添加的插件。
                    this.DoDevicePluginType();
                }
            }
            else if (IsClassifyPlugin(type))
            {
                //向分类插件类型信息中添加新的插件类型
                if (this.AddClassifyPluginType(type, true))
                {
                    //如果成功添加则检查 m_ClassifyPluginTypes 中是否有可以继续添加的插件。
                    this.DoClassifyPluginType();
                }
            }
            else if (IsGlobalPlugin(type))
            {
                //向全局插件类型信息中添加新的插件类型
                if (this.AddGlobalPluginType(type, true))
                {
                    //如果成功添加则检查 m_GlobalPluginTypes 中是否有可以继续添加的插件。
                    this.DoGlobalPluginType();
                }
            }
            else if (IsDeviceArchiveItem(type))
            {
                //向设备档案项类型信息中添加新的档案类型
                if (this.AddDeviceArchiveItemType(type, true))
                {
                    //如果成功添加则检查 m_DeviceArchiveItemTypes 中是否有可以继续添加的档案项。
                    this.DoDeviceArchiveItemType();
                }
            }
            else if (IsDeviceArchiveUpdateItem(type))
            {
                //向设备档案更新项类型信息中添加新的档案类型
                if (this.AddDeviceArchiveUpdateItemType(type, true))
                {
                    //如果成功添加则检查是否有可以继续添加的档案更新项。
                    this.DoDeviceArchiveUpdateItemType();
                }
            }
            else if (IsDeviceArchiveDeleteItem(type))
            {
                //向设备档案删除项类型信息中添加新的档案类型
                if (this.AddDeviceArchiveDeleteItemType(type, true))
                {
                    //如果成功添加则检查是否有可以继续添加的档案删除项。
                    this.DoDeviceArchiveDeleteItemType();
                }
            }
            else if (type.IsInterface && type.GetInterface(typeof(IParameter).FullName) != null)
            {
                this.m_ParameterTypes.Add(type);
            }
            else
            {
                base.AddType(type);
            }
        }

        //查找插件信息集合及继承的插件信息中指定类型的插件信息
        private PluginTypeInfo GetPluginTypeInfoBase(Type baseType, System.Collections.Generic.IEnumerable<PluginTypeInfo> pluginTypeInfos)
        {
            foreach (PluginTypeInfo pluginTypeInfo in pluginTypeInfos)
            {
                if (pluginTypeInfo.Type.Equals(baseType))
                {
                    return pluginTypeInfo;
                }
                else if (pluginTypeInfo.Inherits.Count > 0)
                {
                    this.GetPluginTypeInfoBase(baseType, pluginTypeInfo.Inherits);
                }
            }
            return null;
        }

        #region << 全局插件 >>

        private System.Collections.Generic.List<Type> m_GlobalPluginTypes = new List<Type>();
        /// <summary>
        /// 全局插件类型信息。
        /// </summary>
        protected List<PluginTypeInfo> GlobalPluginTypeInfos = new List<PluginTypeInfo>();

        /// <summary>
        /// 判断传入的类型是否是全局插件。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsGlobalPlugin(Type type)
        {
            //实现了 IGlobalPlugin 接口
            if (type.GetInterface(typeof(IGlobalPlugin).FullName) != null)
            {
                //不是抽象类
                if (type.IsAbstract == false)
                {
                    //提供无参数的构造函数
                    System.Reflection.ConstructorInfo ci = type.GetConstructor(new System.Type[] { });
                    if (ci != null)
                    {
                        //添加了 PluginTypeAttribute 特性。
                        object[] objAttr = type.GetCustomAttributes(typeof(GlobalPluginTypeAttribute), false);
                        if (objAttr != null && objAttr.Length > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //添加全局插件类型。
        private bool AddGlobalPluginType(Type pluginType, bool isAddTemp)
        {
            bool isInherit = false;
            if (pluginType.BaseType != null && pluginType.BaseType.Equals(typeof(object)) == false)
            {
                if (IsGlobalPlugin(pluginType.BaseType))
                {
                    //该插件继承并重写某一插件
                    PluginTypeInfo inheritInfo = this.GetPluginTypeInfoBase(pluginType.BaseType, this.GlobalPluginTypeInfos);
                    if (inheritInfo != null)
                    {
                        inheritInfo.Inherits.Add(new PluginTypeInfo(pluginType));
                        return true;
                    }
                    else
                    {
                        if (isAddTemp)
                        {
                            this.m_GlobalPluginTypes.Add(pluginType);
                        }
                    }
                    isInherit = true;
                }
            }

            if (isInherit == false)
            {
                this.GlobalPluginTypeInfos.Add(new PluginTypeInfo(pluginType));
                return true;
            }

            return false;
        }

        private void DoGlobalPluginType()
        {
            int intTypeCount = this.m_GlobalPluginTypes.Count;

            for (int intIndex = 0; intIndex < this.m_GlobalPluginTypes.Count; intIndex++)
            {
                if (this.AddGlobalPluginType(this.m_GlobalPluginTypes[intIndex], false))
                {
                    this.m_GlobalPluginTypes.RemoveAt(intIndex);
                    intIndex--;
                }
            }

            if (intTypeCount != this.m_GlobalPluginTypes.Count && this.m_GlobalPluginTypes.Count > 0)
            {
                this.DoGlobalPluginType();
            }
        }

        /// <summary>
        /// 获取适合指定操作员使用的并继承自指定基类的全局插件。
        /// </summary>
        /// <param name="account">操作员账号</param>
        /// <param name="baseTypes">全局插件需继承的基类，可指定多个基类，插件只要继承自其中一个或实现 IGlobalHtmlPlugin 接口即可。</param>
        /// <returns></returns>
        protected PluginTypeCollection GetGlobalPluginTypes(IAccount account, Type[] baseTypes)
        {
            PluginTypeCollection pluginTypes = new PluginTypeCollection(null);
            List<PluginType> lstPluginType = new List<PluginType>();

            foreach (PluginTypeInfo _PluginTypeInfo in this.GlobalPluginTypeInfos)
            {
                //是否继承自指定的基类
                if (this.GlobalPluginInheritableCheck(_PluginTypeInfo.Type, baseTypes))
                {
                    PluginType _PluginType = this.GetGlobalPluginType(account, baseTypes, _PluginTypeInfo);
                    if (_PluginType != null && _PluginType.IsEnd == false)
                    {
                        if (_PluginType.ParentType == null)
                        {
                            pluginTypes.Add(_PluginType);
                        }
                        else
                        {
                            lstPluginType.Add(_PluginType);
                        }
                    }
                }
            }

            int intLastNum = 0;
            while (intLastNum != lstPluginType.Count)
            {
                intLastNum = lstPluginType.Count;
                for (int intIndex = 0; intIndex < lstPluginType.Count; intIndex++)
                {
                    PluginType _PluginType = pluginTypes.FindParent(lstPluginType[intIndex].ParentType);
                    if (_PluginType != null && _PluginType.IsEnd == false)
                    {
                        _PluginType.Children.Add(lstPluginType[intIndex]);
                        lstPluginType.RemoveAt(intIndex);
                        intIndex--;
                    }
                }
            }

            return pluginTypes;
        }

        //根据插件信息获取最匹配设备的插件类型
        private GlobalPluginType GetGlobalPluginType(IAccount account, Type[] baseTypes, PluginTypeInfo pluginTypeInfo)
        {
            //首先检查继承的类是否满足要求
            foreach (PluginTypeInfo inheritInfo in pluginTypeInfo.Inherits)
            {
                GlobalPluginType _PluginType = this.GetGlobalPluginType(account, baseTypes, inheritInfo);
                if (_PluginType != null)
                {
                    return _PluginType;
                }
            }

            //操作员是否有权限
            AccountPopedomValidate apv = account.PopedomValidate(pluginTypeInfo.Type);
            if (apv.Validate == AccountPopedomValidateOptions.Loser)
            {
                return null;
            }

            //该插件在当前系统中是否适用。
            if (pluginTypeInfo.Type.GetCustomAttributes(typeof(GlobalPluginTypeAttribute), false).Length > 0)
            {
                GlobalPluginTypeAttribute attr = pluginTypeInfo.Type.GetCustomAttributes(typeof(GlobalPluginTypeAttribute), false)[0] as GlobalPluginTypeAttribute;

                //通过该属性指向的类检查该插件能否用于指定的设备
                if (attr.ForCheckType != null)
                {
                    if (attr.ForCheckType.GetInterface(typeof(IGlobalCheck).FullName) != null)
                    {
                        System.Reflection.ConstructorInfo ci = attr.ForCheckType.GetConstructor(new System.Type[] { });
                        object objInstance = ci.Invoke(new object[] { });
                        IGlobalCheck globalCheck = objInstance as IGlobalCheck;
                        if (globalCheck.Check(this) == false)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        throw new Exception(pluginTypeInfo.Type.FullName + " 添加的全局插件特性中 ForCheckType 所指向的检查类 " + attr.ForCheckType.FullName + " 未实现 IGlobalCheck 接口。");
                    }
                }
            }

            return new GlobalPluginType(this, pluginTypeInfo.Type);
        }

        private bool GlobalPluginInheritableCheck(Type pluginType, Type[] baseTypes)
        {
            if (pluginType.GetInterface(typeof(IGlobalHtmlPlugin).FullName) != null)
            {
                return true;                                                //HTML 插件
            }
            else
            {
                foreach (Type baseType in baseTypes)
                {
                    if (Function.IsInheritableBaseType(pluginType, baseType))
                    {
                        return true;                                        //继承自指定的基类
                    }
                }

                return pluginType.BaseType.Equals(typeof(object));          //如果未继承任何基类，则可能是分割线或分类
            }
        }

        /// <summary>
        /// 获取指定的全局插件类型。
        /// </summary>
        /// <param name="account">用于验证插件权限的操作员账号。(必须)</param>
        /// <param name="globalPluginType">实现 IGlobalPlugin 接口的插件类。</param>
        /// <param name="baseTypes">全局插件需继承的基类，可指定多个基类，插件只要继承自其中一个或实现 IGlobalHtmlPlugin 接口即可。</param>
        /// <returns>适合的插件类型，如无合适的类型则返回 null。</returns>
        protected GlobalPluginType GetGlobalPluginType(IAccount account, Type globalPluginType, Type[] baseTypes)
        {
            PluginTypeInfo _PluginTypeInfo = this.GetPluginTypeInfoBase(globalPluginType, this.GlobalPluginTypeInfos);
            if (_PluginTypeInfo != null)
            {
                //是否继承自指定的基类
                if (this.GlobalPluginInheritableCheck(_PluginTypeInfo.Type, baseTypes))
                {
                    return this.GetGlobalPluginType(account, baseTypes, _PluginTypeInfo);
                }
            }
            return null;
        }
        #endregion
        
        #region << 分类插件 >>

        private System.Collections.Generic.List<Type> m_ClassifyPluginTypes = new List<Type>();
        /// <summary>
        /// 分类插件类型信息。
        /// </summary>
        protected List<PluginTypeInfo> ClassifyPluginTypeInfos = new List<PluginTypeInfo>();

        /// <summary>
        /// 判断传入的类型是否是分类插件。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsClassifyPlugin(Type type)
        {
            //实现了 IClassifyPlugin 接口
            if (type.GetInterface(typeof(IClassifyPlugin).FullName) != null)
            {
                //不是抽象类
                if (type.IsAbstract == false)
                {
                    //提供无参数的构造函数
                    System.Reflection.ConstructorInfo ci = type.GetConstructor(new System.Type[] { });
                    if (ci != null)
                    {
                        //添加了 PluginTypeAttribute 特性。
                        object[] objAttr = type.GetCustomAttributes(typeof(ClassifyPluginTypeAttribute), false);
                        if (objAttr != null && objAttr.Length > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //添加分类插件类型。
        private bool AddClassifyPluginType(Type pluginType, bool isAddTemp)
        {
            bool isInherit = false;
            if (pluginType.BaseType != null && pluginType.BaseType.Equals(typeof(object)) == false)
            {
                if (IsClassifyPlugin(pluginType.BaseType))
                {
                    //该插件继承并重写某一插件
                    PluginTypeInfo inheritInfo = this.GetPluginTypeInfoBase(pluginType.BaseType, this.ClassifyPluginTypeInfos);
                    if (inheritInfo != null)
                    {
                        inheritInfo.Inherits.Add(new PluginTypeInfo(pluginType));
                        return true;
                    }
                    else
                    {
                        if (isAddTemp)
                        {
                            this.m_ClassifyPluginTypes.Add(pluginType);
                        }
                    }
                    isInherit = true;
                }
            }

            if (isInherit == false)
            {
                this.ClassifyPluginTypeInfos.Add(new PluginTypeInfo(pluginType));
                return true;
            }

            return false;
        }

        private void DoClassifyPluginType()
        {
            int intTypeCount = this.m_ClassifyPluginTypes.Count;

            for (int intIndex = 0; intIndex < this.m_ClassifyPluginTypes.Count; intIndex++)
            {
                if (this.AddClassifyPluginType(this.m_ClassifyPluginTypes[intIndex], false))
                {
                    this.m_ClassifyPluginTypes.RemoveAt(intIndex);
                    intIndex--;
                }
            }

            if (intTypeCount != this.m_ClassifyPluginTypes.Count && this.m_ClassifyPluginTypes.Count > 0)
            {
                this.DoClassifyPluginType();
            }
        }

        /// <summary>
        /// 获取适合指定操作员使用的并继承自指定基类的分类插件。
        /// </summary>
        /// <param name="account">用于验证插件权限的操作员账号。(必须)</param>
        /// <param name="baseTypes">分类插件需继承的基类，可指定多个基类，插件只要继承自其中一个或实现 IClassifyHtmlPlugin 接口即可。</param>
        /// <param name="classifys">获取指定分类可用的插件。</param>
        /// <returns></returns>
        protected PluginTypeCollection GetClassifyPluginTypes(IAccount account, Type[] baseTypes, Classify[] classifys)
        {
            PluginTypeCollection pluginTypes = new PluginTypeCollection(null);
            List<PluginType> lstPluginType = new List<PluginType>();

            foreach (PluginTypeInfo _PluginTypeInfo in this.ClassifyPluginTypeInfos)
            {
                //是否继承自指定的基类
                if (this.ClassifyPluginInheritableCheck(_PluginTypeInfo.Type, baseTypes))
                {
                    PluginType _PluginType = this.GetClassifyPluginType(account, baseTypes, classifys, _PluginTypeInfo);
                    if (_PluginType != null && _PluginType.IsEnd == false)
                    {
                        if (_PluginType.ParentType == null)
                        {
                            pluginTypes.Add(_PluginType);
                        }
                        else
                        {
                            lstPluginType.Add(_PluginType);
                        }
                    }
                }
            }

            int intLastNum = 0;
            while (intLastNum != lstPluginType.Count)
            {
                intLastNum = lstPluginType.Count;
                for (int intIndex = 0; intIndex < lstPluginType.Count; intIndex++)
                {
                    PluginType _PluginType = pluginTypes.FindParent(lstPluginType[intIndex].ParentType);
                    if (_PluginType != null && _PluginType.IsEnd == false)
                    {
                        _PluginType.Children.Add(lstPluginType[intIndex]);
                        lstPluginType.RemoveAt(intIndex);
                        intIndex--;
                    }
                }
            }

            return pluginTypes;
        }

        //根据插件信息获取最匹配分类的插件类型
        private ClassifyPluginType GetClassifyPluginType(IAccount account, Type[] baseTypes, Classify[] classifys, PluginTypeInfo pluginTypeInfo)
        {
            //首先检查继承的类是否满足要求
            foreach (PluginTypeInfo inheritInfo in pluginTypeInfo.Inherits)
            {
                ClassifyPluginType _PluginType = this.GetClassifyPluginType(account, baseTypes, classifys, inheritInfo);
                if (_PluginType != null)
                {
                    return _PluginType;
                }
            }

            //操作员是否有权限
            AccountPopedomValidate apv = account.PopedomValidate(pluginTypeInfo.Type);
            if (apv.Validate == AccountPopedomValidateOptions.Loser)
            {
                return null;
            }

            //该插件是否适用所选分类，并检查继承的插件，比较是否所选分类使用相同的插件。
            if (pluginTypeInfo.Type.GetCustomAttributes(typeof(ClassifyPluginTypeAttribute), false).Length > 0)
            {
                ClassifyPluginTypeAttribute attr = pluginTypeInfo.Type.GetCustomAttributes(typeof(ClassifyPluginTypeAttribute), false)[0] as ClassifyPluginTypeAttribute;

                //该插件有分类类型要求，分别检查分类类型、驱动类型、分类功能实现、驱动功能实现是否满足该类型要求
                if (attr.ForClassifyType != null)
                {
                    foreach (Classify classify in classifys)
                    {
                        bool bolTrue = false;
                        if (attr.ForClassifyType.IsClass)
                        {
                            if (Function.IsInheritableBaseType(classify.GetType(), attr.ForClassifyType))
                            {
                                bolTrue = true;
                            }
                        }
                        else if (attr.ForClassifyType.IsInterface)
                        {
                            if (classify.GetType().GetInterface(attr.ForClassifyType.FullName) != null)
                            {
                                bolTrue = true;
                            }
                        }

                        if (bolTrue == false)
                        {
                            return null;
                        }
                    }
                }

                //通过该属性指向的类检查该插件能否用于指定的分类
                if (attr.ForCheckType != null)
                {
                    foreach (Classify classify in classifys)
                    {
                        if (attr.ForCheckType.GetInterface(typeof(IClassifyCheck).FullName) != null)
                        {
                            System.Reflection.ConstructorInfo ci = attr.ForCheckType.GetConstructor(new System.Type[] { });
                            object objInstance = ci.Invoke(new object[] { });
                            IClassifyCheck classifyCheck = objInstance as IClassifyCheck;
                            if (classifyCheck.Check(classify) == false)
                            {
                                return null;
                            }
                        }
                        else
                        {
                            throw new Exception(pluginTypeInfo.Type.FullName + " 添加的分类插件特性中 ForCheckType 所指向的检查类 " + attr.ForCheckType.FullName + " 未实现 IClassifyCheck 接口。");
                        }
                    }
                }

                //插件支持的分类数量
                if (attr.ClassifyMaximum > 0 && classifys.Length > 1)
                {
                    if (attr.ClassifyMaximum < classifys.Length)
                    {
                        return null;
                    }
                }
            }

            return new ClassifyPluginType(this, pluginTypeInfo.Type);
        }

        //检查插件继承关系。如果插件无任何继承，则返回true表示可用，否则查看是否继承自指定类型。
        private bool ClassifyPluginInheritableCheck(Type pluginType, Type[] baseTypes)
        {
            if (pluginType.GetInterface(typeof(IClassifyHtmlPlugin).FullName) != null)
            {
                return true;                                                //HTML 插件
            }
            else
            {
                foreach (Type baseType in baseTypes)
                {
                    if (Function.IsInheritableBaseType(pluginType, baseType))
                    {
                        return true;                                        //继承自指定的基类
                    }
                }

                return pluginType.BaseType.Equals(typeof(object));          //如果未继承任何基类，则可能是分割线或分类
            }
        }

        /// <summary>
        /// 获取指定的分类插件类型。
        /// </summary>
        /// <param name="account">用于验证插件权限的操作员账号。(必须)</param>
        /// <param name="classifyPluginType">实现 IClassifyPlugin 接口的插件类。</param>
        /// <param name="baseTypes">分类插件需继承的基类，可指定多个基类，插件只要继承自其中一个或实现 IClassifyHtmlPlugin 接口即可。</param>
        /// <param name="classifys">分类集合。</param>
        /// <returns>适用于指定分类的插件类型，如无合适的类型则返回 null。</returns>
        protected ClassifyPluginType GetClassifyPluginType(IAccount account, Type classifyPluginType, Type[] baseTypes, Classify[] classifys)
        {
            PluginTypeInfo _PluginTypeInfo = this.GetPluginTypeInfoBase(classifyPluginType, this.ClassifyPluginTypeInfos);
            if (_PluginTypeInfo != null)
            {
                //是否继承自指定的基类
                if (this.ClassifyPluginInheritableCheck(_PluginTypeInfo.Type, baseTypes))
                {
                    return this.GetClassifyPluginType(account, baseTypes, classifys, _PluginTypeInfo);
                }
            }
            return null;
        }

        #endregion

        #region << 设备插件 >>

        private System.Collections.Generic.List<Type> m_DevicePluginTypes = new List<Type>();
        /// <summary>
        /// 设备插件类型信息。
        /// </summary>
        protected List<PluginTypeInfo> DevicePluginTypeInfos = new List<PluginTypeInfo>();

        /// <summary>
        /// 判断传入的类型是否是设备插件。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsDevicePlugin(Type type)
        {
            //实现了 IDevicePlugin 接口
            if (type.GetInterface(typeof(IDevicePlugin).FullName) != null)
            {
                //不是抽象类
                if (type.IsAbstract == false)
                {
                    //提供无参数的构造函数
                    System.Reflection.ConstructorInfo ci = type.GetConstructor(new System.Type[] { });
                    if (ci != null)
                    {
                        //添加了 PluginTypeAttribute 特性。
                        object[] objAttr = type.GetCustomAttributes(typeof(DevicePluginTypeAttribute), false);
                        if (objAttr != null && objAttr.Length > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //添加设备插件类型。
        private bool AddDevicePluginType(Type pluginType, bool isAddTemp)
        {
            bool isInherit = false;
            if (pluginType.BaseType != null && pluginType.BaseType.Equals(typeof(object)) == false)
            {
                if (IsDevicePlugin(pluginType.BaseType))
                {
                    //该插件继承并重写某一插件
                    PluginTypeInfo inheritInfo = this.GetPluginTypeInfoBase(pluginType.BaseType, this.DevicePluginTypeInfos);
                    if (inheritInfo != null)
                    {
                        inheritInfo.Inherits.Add(new PluginTypeInfo(pluginType));
                        return true;
                    }
                    else
                    {
                        if (isAddTemp)
                        {
                            this.m_DevicePluginTypes.Add(pluginType);
                        }
                    }
                    isInherit = true;
                }
            }

            if (isInherit == false)
            {
                this.DevicePluginTypeInfos.Add(new PluginTypeInfo(pluginType));
                return true;
            }

            return false;
        }

        private void DoDevicePluginType()
        {
            int intTypeCount = this.m_DevicePluginTypes.Count;

            for (int intIndex = 0; intIndex < this.m_DevicePluginTypes.Count; intIndex++)
            {
                if (this.AddDevicePluginType(this.m_DevicePluginTypes[intIndex], false))
                {
                    this.m_DevicePluginTypes.RemoveAt(intIndex);
                    intIndex--;
                }
            }

            if (intTypeCount != this.m_DevicePluginTypes.Count && this.m_DevicePluginTypes.Count > 0)
            {
                this.DoDevicePluginType();
            }
        }

        /// <summary>
        /// 获取适合指定操作员使用的并继承自指定基类的设备插件。
        /// </summary>
        /// <param name="account">用于验证插件权限的操作员账号。(必须)</param>
        /// <param name="baseTypes">设备插件需继承的基类，可指定多个基类，插件只要继承自其中一个或实现 IDeviceHtmlPlugin 接口即可。</param>
        /// <param name="devices">获取指定设备可用的插件。</param>
        /// <returns></returns>
        protected PluginTypeCollection GetDevicePluginTypes(IAccount account, Type[] baseTypes, Device[] devices)
        {
            PluginTypeCollection pluginTypes = new PluginTypeCollection(null);
            List<PluginType> lstPluginType = new List<PluginType>();

            foreach (PluginTypeInfo _PluginTypeInfo in this.DevicePluginTypeInfos)
            {
                //是否继承自指定的基类
                if (this.DevicePluginInheritableCheck(_PluginTypeInfo.Type, baseTypes))
                {
                    PluginType _PluginType = this.GetDevicePluginType(account, baseTypes, devices, _PluginTypeInfo);
                    if (_PluginType != null && _PluginType.IsEnd == false)
                    {
                        if (_PluginType.ParentType == null)
                        {
                            pluginTypes.Add(_PluginType);
                        }
                        else
                        {
                            lstPluginType.Add(_PluginType);
                        }
                    }
                }
            }

            int intLastNum = 0;
            while (intLastNum != lstPluginType.Count)
            {
                intLastNum = lstPluginType.Count;
                for (int intIndex = 0; intIndex < lstPluginType.Count; intIndex++)
                {
                    PluginType _PluginType = pluginTypes.FindParent(lstPluginType[intIndex].ParentType);
                    if (_PluginType != null && _PluginType.IsEnd == false)
                    {
                        _PluginType.Children.Add(lstPluginType[intIndex]);
                        lstPluginType.RemoveAt(intIndex);
                        intIndex--;
                    }
                }
            }

            return pluginTypes;
        }


        /// <summary>
        /// 根据插件信息获取最匹配设备的插件类型。
        /// </summary>
        /// <param name="account"></param>
        /// <param name="baseTypes"></param>
        /// <param name="devices"></param>
        /// <param name="pluginTypeInfo"></param>
        /// <returns></returns>
        private DevicePluginType GetDevicePluginType(IAccount account, Type[] baseTypes, Device[] devices, PluginTypeInfo pluginTypeInfo)
        {
            //首先检查继承的类是否满足要求
            foreach (PluginTypeInfo inheritInfo in pluginTypeInfo.Inherits)
            {
                DevicePluginType _PluginType = this.GetDevicePluginType(account, baseTypes, devices, inheritInfo);
                if (_PluginType != null)
                {
                    return _PluginType;
                }
            }

            //操作员是否有权限
            AccountPopedomValidate apv = account.PopedomValidate(pluginTypeInfo.Type);
            if (apv.Validate == AccountPopedomValidateOptions.Loser)
            {
                return null;
            }

            //该插件是否适用所选设备，并检查继承的插件，比较是否所选设备使用相同的插件。
            if (pluginTypeInfo.Type.GetCustomAttributes(typeof(DevicePluginTypeAttribute), false).Length > 0)
            {
                DevicePluginTypeAttribute attr = pluginTypeInfo.Type.GetCustomAttributes(typeof(DevicePluginTypeAttribute), false)[0] as DevicePluginTypeAttribute;

                //该插件有设备类型要求，分别检查设备类型、驱动类型、设备功能实现、驱动功能实现是否满足该类型要求
                if (attr.ForDeviceType != null)
                {
                    foreach (Device device in devices)
                    {
                        bool bolTrue = false;
                        if (attr.ForDeviceType.IsClass)
                        {
                            if (Function.IsInheritableBaseType(device.DeviceType.Type, attr.ForDeviceType))
                            {
                                bolTrue = true;
                            }
                            else if (device.DeviceType.DriveType != null && Function.IsInheritableBaseType(device.DeviceType.DriveType, attr.ForDeviceType))
                            {
                                bolTrue = true;
                            }
                        }
                        else if (attr.ForDeviceType.IsInterface)
                        {
                            if (device.DeviceType.Type.GetInterface(attr.ForDeviceType.FullName) != null)
                            {
                                bolTrue = true;
                            }
                            else if (device.DeviceType.DriveType != null && device.DeviceType.DriveType.GetInterface(attr.ForDeviceType.FullName) != null)
                            {
                                bolTrue = true;
                            }
                        }

                        if (bolTrue == false)
                        {
                            return null;
                        }
                    }
                }

                //通过该属性指向的类检查该插件能否用于指定的设备
                if (attr.ForCheckType != null)
                {
                    foreach (Device device in devices)
                    {
                        if (attr.ForCheckType.GetInterface(typeof(IDeviceCheck).FullName) != null)
                        {
                            System.Reflection.ConstructorInfo ci = attr.ForCheckType.GetConstructor(new System.Type[] { });
                            object objInstance = ci.Invoke(new object[] { });
                            IDeviceCheck deviceCheck = objInstance as IDeviceCheck;
                            if (deviceCheck.Check(device) == false)
                            {
                                return null;
                            }
                        }
                        else
                        {
                            throw new Exception(pluginTypeInfo.Type.FullName + " 添加的设备插件特性中 ForCheckType 所指向的检查类 " + attr.ForCheckType.FullName + " 未实现 IDeviceCheck 接口。");
                        }
                    }
                }

                //插件支持的设备数量
                if (attr.DeviceMaximum > 0 && devices.Length > 1)
                {
                    if (attr.DeviceMaximum < devices.Length)
                    {
                        return null;
                    }
                }
            }

            return new DevicePluginType(this, pluginTypeInfo.Type);
        }


        #region<< edited by xc >>

        /// <summary>
        /// 获取全部适合指定操作员使用的并继承自指定基类的设备插件。WEB popmenu 
        /// </summary>
        /// <param name="account">用于验证插件权限的操作员账号。(必须)</param>
        /// <param name="baseTypes">设备插件需继承的基类，可指定多个基类，插件只要继承自其中一个或实现 IDeviceHtmlPlugin 接口即可。</param>
        /// <returns></returns>
        protected PluginTypeCollection GetAllDevicePluginTypes(IAccount account, Type[] baseTypes)
        {
            PluginTypeCollection pluginTypes = new PluginTypeCollection(null);
            List<PluginType> lstPluginType = new List<PluginType>();

            foreach (PluginTypeInfo _PluginTypeInfo in this.DevicePluginTypeInfos)
            {
                //是否继承自指定的基类
                if (this.DevicePluginInheritableCheck(_PluginTypeInfo.Type, baseTypes))
                {
                    PluginType _PluginType = this.GetAllDevicePluginType(account, baseTypes, _PluginTypeInfo);
                    if (_PluginType != null && _PluginType.IsEnd == false)
                    {
                        if (_PluginType.ParentType == null)
                        {
                            pluginTypes.Add(_PluginType);
                        }
                        else
                        {
                            lstPluginType.Add(_PluginType);
                        }
                    }
                }
            }

            int intLastNum = 0;
            while (intLastNum != lstPluginType.Count)
            {
                intLastNum = lstPluginType.Count;
                for (int intIndex = 0; intIndex < lstPluginType.Count; intIndex++)
                {
                    PluginType _PluginType = pluginTypes.FindParent(lstPluginType[intIndex].ParentType);
                    if (_PluginType != null && _PluginType.IsEnd == false)
                    {
                        _PluginType.Children.Add(lstPluginType[intIndex]);
                        lstPluginType.RemoveAt(intIndex);
                        intIndex--;
                    }
                }
            }

            return pluginTypes;
        }


        /// <summary>
        /// 根据插件信息获取全部设备的插件类型。WEB popmenu 
        /// </summary>

        private DevicePluginType GetAllDevicePluginType(IAccount account, Type[] baseTypes, PluginTypeInfo pluginTypeInfo)
        {
            //首先检查继承的类是否满足要求
            foreach (PluginTypeInfo inheritInfo in pluginTypeInfo.Inherits)
            {
                DevicePluginType _PluginType = this.GetAllDevicePluginType(account, baseTypes, inheritInfo);
                if (_PluginType != null)
                {
                    return _PluginType;
                }
            }

            //操作员是否有权限
            AccountPopedomValidate apv = account.PopedomValidate(pluginTypeInfo.Type);
            if (apv.Validate == AccountPopedomValidateOptions.Loser)
            {
                return null;
            }

            return new DevicePluginType(this, pluginTypeInfo.Type);
        }


        #endregion




        //检查插件继承关系。如果插件无任何继承，则返回true表示可用，否则查看是否继承自指定类型。
        private bool DevicePluginInheritableCheck(Type pluginType, Type[] baseTypes)
        {
            if (pluginType.GetInterface(typeof(IDeviceHtmlPlugin).FullName) != null)
            {
                return true;                                                //HTML 插件
            }
            else
            {
                foreach (Type baseType in baseTypes)
                {
                    if (Function.IsInheritableBaseType(pluginType, baseType))
                    {
                        return true;                                        //继承自指定的基类
                    }
                }

                return pluginType.BaseType.Equals(typeof(object));          //如果未继承任何基类，则可能是分割线或分类
            }
        }

        /// <summary>
        /// 获取指定的设备插件类型。
        /// </summary>
        /// <param name="account">用于验证插件权限的操作员账号。(必须)</param>
        /// <param name="devicePluginType">实现 IDevicePlugin 接口的插件类。</param>
        /// <param name="baseTypes">设备插件需继承的基类，可指定多个基类，插件只要继承自其中一个或实现 IDeviceHtmlPlugin 接口即可。</param>
        /// <param name="devices">设备集合。</param>
        /// <returns>适用于指定设备的插件类型，如无合适的类型则返回 null。</returns>
        protected DevicePluginType GetDevicePluginType(IAccount account, Type devicePluginType, Type[] baseTypes, Device[] devices)
        {
            PluginTypeInfo _PluginTypeInfo = this.GetPluginTypeInfoBase(devicePluginType, this.DevicePluginTypeInfos);
            if (_PluginTypeInfo != null)
            {
                //是否继承自指定的基类
                if (this.DevicePluginInheritableCheck(_PluginTypeInfo.Type, baseTypes))
                {
                    return this.GetDevicePluginType(account, baseTypes, devices, _PluginTypeInfo);
                }
            }
            return null;
        }

        #endregion

        #region << 设备档案项 >>

        #region << 档案查看项 >>

        private System.Collections.Generic.List<Type> m_DeviceArchiveItemTypes = new List<Type>();
        private List<DeviceArchiveItemTypeInfo> m_DeviceArchiveItemTypeInfos = new List<DeviceArchiveItemTypeInfo>();

        //判断传入的类型是否是设备档案项
        private bool IsDeviceArchiveItem(Type type)
        {
            //实现了 IDeviceArchiveItem 接口
            if (type.GetInterface(typeof(IDeviceArchiveItem).FullName) != null)
            {
                //不是抽象类
                if (type.IsAbstract == false)
                {
                    //提供无参数的构造函数
                    System.Reflection.ConstructorInfo ci = type.GetConstructor(new System.Type[] { });
                    if (ci != null)
                    {
                        //添加了 DeviceArchiveItemTypeAttribute 特性。
                        object[] objAttr = type.GetCustomAttributes(typeof(DeviceArchiveItemTypeAttribute), false);
                        if (objAttr != null && objAttr.Length > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //添加设备档案项类型。
        private bool AddDeviceArchiveItemType(Type archiveType, bool isAddTemp)
        {
            if (archiveType.BaseType != null && archiveType.BaseType.Equals(typeof(object)) == false)
            {
                if (IsDeviceArchiveItem(archiveType.BaseType))
                {
                    //该档案项继承并重写某一插件
                    DeviceArchiveItemTypeInfo inheritInfo = this.FindDeviceArchiveItemInherit(this.m_DeviceArchiveItemTypeInfos, archiveType.BaseType);
                    if (inheritInfo != null)
                    {
                        inheritInfo.Inherits.Add(new DeviceArchiveItemTypeInfo(archiveType));
                        return true;
                    }
                    else
                    {
                        if (isAddTemp)
                        {
                            this.m_DeviceArchiveItemTypes.Add(archiveType);
                        }
                        return false;
                    }
                }
            }

            bool bolIsAdd = true;
            DeviceArchiveItemTypeAttribute attr = archiveType.GetCustomAttributes(typeof(DeviceArchiveItemTypeAttribute), false)[0] as DeviceArchiveItemTypeAttribute;
            if (attr.OrdinalNumber > 0)
            {
                for (int intIndex = 0; intIndex < this.m_DeviceArchiveItemTypeInfos.Count; intIndex++)
                {
                    if (attr.OrdinalNumber < this.m_DeviceArchiveItemTypeInfos[intIndex].OrdinalNumber)
                    {
                        this.m_DeviceArchiveItemTypeInfos.Insert(intIndex, new DeviceArchiveItemTypeInfo(archiveType));
                        bolIsAdd = false;
                        break;
                    }
                }
            }

            if (bolIsAdd)
            {
                this.m_DeviceArchiveItemTypeInfos.Add(new DeviceArchiveItemTypeInfo(archiveType));
            }
            return true;
        }

        private void DoDeviceArchiveItemType()
        {
            int intTypeCount = this.m_DeviceArchiveItemTypes.Count;

            for (int intIndex = 0; intIndex < this.m_DeviceArchiveItemTypes.Count; intIndex++)
            {
                if (this.AddDeviceArchiveItemType(this.m_DeviceArchiveItemTypes[intIndex], false))
                {
                    this.m_DeviceArchiveItemTypes.RemoveAt(intIndex);
                    intIndex--;
                }
            }

            if (intTypeCount != this.m_DeviceArchiveItemTypes.Count && this.m_DeviceArchiveItemTypes.Count > 0)
            {
                this.DoDeviceArchiveItemType();
            }
        }

        /// <summary>
        /// 获取继承自指定基类的设备档案项。
        /// </summary>
        /// <param name="device">获取指定设备可用的档案项。</param>
        /// <returns></returns>
        public DeviceArchiveItemTypeCollection GetDeviceArchiveItemTypes(Device device)
        {
            DeviceArchiveItemTypeCollection archiveTypes = new DeviceArchiveItemTypeCollection();

            foreach (DeviceArchiveItemTypeInfo info in this.m_DeviceArchiveItemTypeInfos)
            {
                //该档案项是否适用所选设备，并检查继承的档案项查找最适合的类型。
                DeviceArchiveItemTypeInfo archiveInfo = this.GetDeviceArchiveItemTypeInfo(device, info);

                if (archiveInfo != null)
                {
                    DeviceArchiveItemType archiveType = new DeviceArchiveItemType(archiveInfo.Type);
                    archiveTypes.Add(archiveType);
                }
            }

            return archiveTypes;
        }


        #endregion

        #region << 档案更新项 >>

        private System.Collections.Generic.List<Type> m_DeviceArchiveUpdateItemTypes = new List<Type>();
        private List<DeviceArchiveItemTypeInfo> m_DeviceArchiveUpdateItemTypeInfos = new List<DeviceArchiveItemTypeInfo>();

        //判断传入的类型是否是设备档案项
        private bool IsDeviceArchiveUpdateItem(Type type)
        {
            //实现了 IDeviceArchiveUpdateItem 接口
            if (type.GetInterface(typeof(IDeviceArchiveUpdateItem).FullName) != null)
            {
                //不是抽象类
                if (type.IsAbstract == false)
                {
                    //提供无参数的构造函数
                    System.Reflection.ConstructorInfo ci = type.GetConstructor(new System.Type[] { });
                    if (ci != null)
                    {
                        //添加了 DeviceArchiveItemTypeAttribute 特性。
                        object[] objAttr = type.GetCustomAttributes(typeof(DeviceArchiveItemTypeAttribute), false);
                        if (objAttr != null && objAttr.Length > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //添加设备档案项类型。
        private bool AddDeviceArchiveUpdateItemType(Type archiveType, bool isAddTemp)
        {
            if (archiveType.BaseType != null && archiveType.BaseType.Equals(typeof(object)) == false)
            {
                if (IsDeviceArchiveUpdateItem(archiveType.BaseType))
                {
                    //该档案项继承并重写某一插件
                    DeviceArchiveItemTypeInfo inheritInfo = this.FindDeviceArchiveItemInherit(this.m_DeviceArchiveUpdateItemTypeInfos, archiveType.BaseType);
                    if (inheritInfo != null)
                    {
                        inheritInfo.Inherits.Add(new DeviceArchiveItemTypeInfo(archiveType));
                        return true;
                    }
                    else
                    {
                        if (isAddTemp)
                        {
                            this.m_DeviceArchiveUpdateItemTypes.Add(archiveType);
                        }
                        return false;
                    }
                }
            }

            bool bolIsAdd = true;
            DeviceArchiveItemTypeAttribute attr = archiveType.GetCustomAttributes(typeof(DeviceArchiveItemTypeAttribute), false)[0] as DeviceArchiveItemTypeAttribute;
            if (attr.OrdinalNumber > 0)
            {
                for (int intIndex = 0; intIndex < this.m_DeviceArchiveUpdateItemTypeInfos.Count; intIndex++)
                {
                    if (attr.OrdinalNumber < this.m_DeviceArchiveUpdateItemTypeInfos[intIndex].OrdinalNumber)
                    {
                        this.m_DeviceArchiveUpdateItemTypeInfos.Insert(intIndex, new DeviceArchiveItemTypeInfo(archiveType));
                        bolIsAdd = false;
                        break;
                    }
                }
            }

            if (bolIsAdd)
            {
                this.m_DeviceArchiveUpdateItemTypeInfos.Add(new DeviceArchiveItemTypeInfo(archiveType));
            }
            return true;
        }

        private void DoDeviceArchiveUpdateItemType()
        {
            int intTypeCount = this.m_DeviceArchiveUpdateItemTypes.Count;

            for (int intIndex = 0; intIndex < this.m_DeviceArchiveUpdateItemTypes.Count; intIndex++)
            {
                if (this.AddDeviceArchiveUpdateItemType(this.m_DeviceArchiveUpdateItemTypes[intIndex], false))
                {
                    this.m_DeviceArchiveUpdateItemTypes.RemoveAt(intIndex);
                    intIndex--;
                }
            }

            if (intTypeCount != this.m_DeviceArchiveUpdateItemTypes.Count && this.m_DeviceArchiveUpdateItemTypes.Count > 0)
            {
                this.DoDeviceArchiveUpdateItemType();
            }
        }

        /// <summary>
        /// 获取继承自指定基类的设备档案项。
        /// </summary>
        /// <param name="baseType">设备档案项需继承的基类。</param>
        /// <param name="device">获取指定设备可用的档案项。</param>
        /// <returns></returns>
        protected DeviceArchiveItemTypeCollection GetDeviceArchiveUpdateItemTypes(Type baseType, Device device)
        {
            DeviceArchiveItemTypeCollection archiveTypes = new DeviceArchiveItemTypeCollection();

            foreach (DeviceArchiveItemTypeInfo info in this.m_DeviceArchiveUpdateItemTypeInfos)
            {
                //是否继承自指定的基类
                if (this.DeviceArchiveItemInheritableCheck(info.Type, baseType) == false)
                {
                    continue;
                }

                //该档案项是否适用所选设备，并检查继承的档案项查找最适合的类型。
                DeviceArchiveItemTypeInfo archiveInfo = this.GetDeviceArchiveItemTypeInfo(device, info);

                if (archiveInfo != null)
                {
                    DeviceArchiveItemType archiveType = new DeviceArchiveItemType(archiveInfo.Type);
                    archiveTypes.Add(archiveType);
                }
            }

            return archiveTypes;
        }

        #endregion

        #region << 档案删除项 >>

        private System.Collections.Generic.List<Type> m_DeviceArchiveDeleteItemTypes = new List<Type>();
        private List<DeviceArchiveItemTypeInfo> m_DeviceArchiveDeleteItemTypeInfos = new List<DeviceArchiveItemTypeInfo>();

        //判断传入的类型是否是设备档案项
        private bool IsDeviceArchiveDeleteItem(Type type)
        {
            //实现了 IDeviceArchiveDeleteItem 接口
            if (type.GetInterface(typeof(IDeviceArchiveDeleteItem).FullName) != null)
            {
                //不是抽象类
                if (type.IsAbstract == false)
                {
                    //提供无参数的构造函数
                    System.Reflection.ConstructorInfo ci = type.GetConstructor(new System.Type[] { });
                    if (ci != null)
                    {
                        //添加了 DeviceArchiveItemTypeAttribute 特性。
                        object[] objAttr = type.GetCustomAttributes(typeof(DeviceArchiveItemTypeAttribute), false);
                        if (objAttr != null && objAttr.Length > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //添加设备档案项类型。
        private bool AddDeviceArchiveDeleteItemType(Type archiveType, bool isAddTemp)
        {
            if (archiveType.BaseType != null && archiveType.BaseType.Equals(typeof(object)) == false)
            {
                if (IsDeviceArchiveDeleteItem(archiveType.BaseType))
                {
                    //该档案项继承并重写某一插件
                    DeviceArchiveItemTypeInfo inheritInfo = this.FindDeviceArchiveItemInherit(this.m_DeviceArchiveDeleteItemTypeInfos, archiveType.BaseType);
                    if (inheritInfo != null)
                    {
                        inheritInfo.Inherits.Add(new DeviceArchiveItemTypeInfo(archiveType));
                        return true;
                    }
                    else
                    {
                        if (isAddTemp)
                        {
                            this.m_DeviceArchiveDeleteItemTypes.Add(archiveType);
                        }
                        return false;
                    }
                }
            }

            bool bolIsAdd = true;
            DeviceArchiveItemTypeAttribute attr = archiveType.GetCustomAttributes(typeof(DeviceArchiveItemTypeAttribute), false)[0] as DeviceArchiveItemTypeAttribute;
            if (attr.OrdinalNumber > 0)
            {
                for (int intIndex = 0; intIndex < this.m_DeviceArchiveDeleteItemTypeInfos.Count; intIndex++)
                {
                    if (attr.OrdinalNumber < this.m_DeviceArchiveDeleteItemTypeInfos[intIndex].OrdinalNumber)
                    {
                        this.m_DeviceArchiveDeleteItemTypeInfos.Insert(intIndex, new DeviceArchiveItemTypeInfo(archiveType));
                        bolIsAdd = false;
                        break;
                    }
                }
            }

            if (bolIsAdd)
            {
                this.m_DeviceArchiveDeleteItemTypeInfos.Add(new DeviceArchiveItemTypeInfo(archiveType));
            }
            return true;
        }

        private void DoDeviceArchiveDeleteItemType()
        {
            int intTypeCount = this.m_DeviceArchiveDeleteItemTypes.Count;

            for (int intIndex = 0; intIndex < this.m_DeviceArchiveDeleteItemTypes.Count; intIndex++)
            {
                if (this.AddDeviceArchiveDeleteItemType(this.m_DeviceArchiveDeleteItemTypes[intIndex], false))
                {
                    this.m_DeviceArchiveDeleteItemTypes.RemoveAt(intIndex);
                    intIndex--;
                }
            }

            if (intTypeCount != this.m_DeviceArchiveDeleteItemTypes.Count && this.m_DeviceArchiveDeleteItemTypes.Count > 0)
            {
                this.DoDeviceArchiveDeleteItemType();
            }
        }

        /// <summary>
        /// 获取继承自指定基类的设备档案项。
        /// </summary>
        /// <param name="baseType">设备档案项需继承的基类。</param>
        /// <param name="device">获取指定设备可用的档案项。</param>
        /// <returns></returns>
        protected DeviceArchiveItemTypeCollection GetDeviceArchiveDeleteItemTypes(Type baseType, Device device)
        {
            DeviceArchiveItemTypeCollection archiveTypes = new DeviceArchiveItemTypeCollection();

            foreach (DeviceArchiveItemTypeInfo info in this.m_DeviceArchiveDeleteItemTypeInfos)
            {
                //是否继承自指定的基类
                if (this.DeviceArchiveItemInheritableCheck(info.Type, baseType) == false)
                {
                    continue;
                }

                //该档案项是否适用所选设备，并检查继承的档案项查找最适合的类型。
                DeviceArchiveItemTypeInfo archiveInfo = this.GetDeviceArchiveItemTypeInfo(device, info);

                if (archiveInfo != null)
                {
                    DeviceArchiveItemType archiveType = new DeviceArchiveItemType(archiveInfo.Type);
                    archiveTypes.Add(archiveType);
                }
            }

            return archiveTypes;
        }

        #endregion

        private DeviceArchiveItemTypeInfo FindDeviceArchiveItemInherit(List<DeviceArchiveItemTypeInfo> infos, Type baseType)
        {
            foreach (DeviceArchiveItemTypeInfo info in infos)
            {
                if (info.Type.Equals(baseType))
                {
                    return info;
                }
                else if (info.Inherits.Count > 0)
                {
                    DeviceArchiveItemTypeInfo infoChildren = FindDeviceArchiveItemInherit(info.Inherits, baseType);
                    if (infoChildren != null)
                    {
                        return infoChildren;
                    }
                }
            }
            return null;
        }

        //检查档案项继承关系。如果档案项无任何继承，则返回true表示可用，否则查看是否继承自指定类型。
        private bool DeviceArchiveItemInheritableCheck(Type archiveType, Type baseType)
        {
            Type pluginBaseType = archiveType.BaseType;

            while (pluginBaseType.Equals(typeof(Object)) == false)
            {
                //如果继承的类没有实现IDeviceArchiveItem接口，则检查该基类是否继承指定的类型。
                if (pluginBaseType.GetInterface(typeof(IDeviceArchiveItem).FullName) == null)
                {
                    return Function.IsInheritableBaseType(pluginBaseType, baseType);
                }

                pluginBaseType = pluginBaseType.BaseType;
            }
            return true;
        }

        //检查档案项是否适用于某设备。首先检查继承的档案项是否适用，然后检查父档案项，并返回最适用于该设备的档案项，如无适用档案项则返回null。
        private DeviceArchiveItemTypeInfo GetDeviceArchiveItemTypeInfo(Device device, DeviceArchiveItemTypeInfo info)
        {
            if (info.Inherits.Count > 0)
            {
                foreach (DeviceArchiveItemTypeInfo inherit in info.Inherits)
                {
                    DeviceArchiveItemTypeInfo infoInherit = GetDeviceArchiveItemTypeInfo(device, inherit);
                    if (infoInherit != null)
                    {
                        return infoInherit;
                    }
                }
            }

            if (info.Type.GetCustomAttributes(typeof(DeviceArchiveItemTypeAttribute), false).Length > 0)
            {
                DeviceArchiveItemTypeAttribute attr = info.Type.GetCustomAttributes(typeof(DeviceArchiveItemTypeAttribute), false)[0] as DeviceArchiveItemTypeAttribute;

                if (attr.ForDeviceType != null)
                {
                    //该档案项有设备类型要求，分别检查设备类型、驱动类型、设备功能实现、驱动功能实现是否满足该类型要求
                    bool bolTrue = false;
                    if (attr.ForDeviceType.IsClass)
                    {
                        if (Function.IsInheritableBaseType(device.GetType(), attr.ForDeviceType))
                        {
                            bolTrue = true;
                        }
                        else if (device.DeviceType.DriveType != null && Function.IsInheritableBaseType(device.DeviceType.DriveType, attr.ForDeviceType))
                        {
                            bolTrue = true;
                        }
                    }
                    else if (attr.ForDeviceType.IsInterface)
                    {
                        if (device.DeviceType.Type.GetInterface(attr.ForDeviceType.FullName) != null)
                        {
                            bolTrue = true;
                        }
                        else if (device.DeviceType.DriveType != null && device.DeviceType.DriveType.GetInterface(attr.ForDeviceType.FullName) != null)
                        {
                            bolTrue = true;
                        }
                    }

                    if (bolTrue == false)
                    {
                        return null;
                    }
                }

                //通过该属性指向的类检查该档案项能否用于指定的设备
                if (attr.ForCheckType != null)
                {
                    if (attr.ForCheckType.GetInterface(typeof(IDeviceCheck).FullName) != null)
                    {
                        System.Reflection.ConstructorInfo ci = attr.ForCheckType.GetConstructor(new System.Type[] { });
                        object objInstance = ci.Invoke(new object[] { });
                        IDeviceCheck deviceCheck = objInstance as IDeviceCheck;
                        if (deviceCheck.Check(device) == false)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        throw new Exception(info.Type.FullName + " 添加的设备档案项特性中 ForCheckType 所指向的检查类 " + attr.ForCheckType.FullName + " 未实现 IDeviceCheck 接口。");
                    }
                }
            }

            return info;
        }

        #endregion

        #region << 参数 >>

        private List<Type> m_ParameterTypes = new List<Type>();
        /// <summary>
        /// 系统内所有的参数类型。
        /// </summary>
        protected IEnumerable<Type> ParameterTypes
        {
            get { return this.m_ParameterTypes; }
        }

        #endregion
    }
}

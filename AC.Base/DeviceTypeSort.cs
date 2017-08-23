using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 设备类型分类。
    /// </summary>
    public class DeviceTypeSort
    {
        /// <summary>
        /// 设备类型键值集合。
        /// </summary>
        static internal SortedList<string, DeviceType> AllDeviceTypes = new SortedList<string, DeviceType>();

        private ApplicationClass m_Application;

        internal DeviceTypeSort(ApplicationClass application, string name, DeviceTypeSort parent)
        {
            this.m_Application = application;
            this.Name = name;
            this.Parent = parent;
            this.Children = new DeviceTypeSortCollection();
            this.DeviceTypes = new DeviceTypeCollection();
        }

        /// <summary>
        /// 设备类型分类名。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 上层设备类型分类。
        /// </summary>
        public DeviceTypeSort Parent { get; private set; }

        /// <summary>
        /// 下级设备类型分类。
        /// </summary>
        public DeviceTypeSortCollection Children { get; private set; }

        /// <summary>
        /// 所属该分类的设备类型。
        /// </summary>
        public DeviceTypeCollection DeviceTypes { get; private set; }

        /// <summary>
        /// 获取指定分类路径的设备类型分类。
        /// </summary>
        /// <param name="sortPath">设备类型分类路径。例如“公司名/系列名”，分类之间使用“/”符号分隔。</param>
        /// <returns>设备类型分类。如未查找到该路径的分类则返回 null。</returns>
        public DeviceTypeSort GetSort(string sortPath)
        {
            string[] strSortNames = sortPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (strSortNames.Length == 0)
            {
                return this;
            }
            else
            {
                foreach (DeviceTypeSort children in this.Children)
                {
                    if (children.Name.Equals(strSortNames[0]))
                    {
                        if (strSortNames.Length == 1)
                        {
                            return children;
                        }
                        else
                        {
                            string strSortName = "";
                            for (int intIndex = 1; intIndex < strSortNames.Length; intIndex++)
                            {
                                strSortName += '/' + strSortNames[intIndex];
                            }
                            return children.GetSort(strSortName.Substring(1));
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// 获取指定类型名的设备类型。
        /// </summary>
        /// <param name="typeName">类型名称，如“命名空间1.命名空间2.设备类名”</param>
        /// <returns>设备类型。如无对应的设备类型则返回 null。</returns>
        public DeviceType GetDeviceType(string typeName)
        {
            if (AllDeviceTypes.ContainsKey(typeName))
            {
                return AllDeviceTypes[typeName];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取指定类型声明的设备类型。
        /// </summary>
        /// <param name="type">设备类型声明</param>
        /// <returns>设备类型。如无对应的设备类型则返回 null。</returns>
        public DeviceType GetDeviceType(Type type)
        {
            if (AllDeviceTypes.ContainsKey(type.FullName))
            {
                return AllDeviceTypes[type.FullName];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取所有继承自指定类型名的设备类型。
        /// </summary>
        /// <param name="typeName">类型名称，如“命名空间1.命名空间2.设备类名”</param>
        /// <returns>继承自指定类型名的设备类型集合。</returns>
        public DeviceTypeCollection GetDeviceTypes(string typeName)
        {
            Type t = Function.GetType(typeName);
            if (t != null)
            {
                return this.GetDeviceTypes(t);
            }
            else
            {
                return new DeviceTypeCollection();
            }
        }

        /// <summary>
        /// 获取所有继承自指定类型声明的设备类型。
        /// </summary>
        /// <param name="type">设备类型声明</param>
        /// <returns>继承自指定类型声明的设备类型集合。</returns>
        public DeviceTypeCollection GetDeviceTypes(Type type)
        {
            DeviceTypeCollection deviceTypes = new DeviceTypeCollection();
            this.FindDeviceTypes(type, deviceTypes);
            return deviceTypes;
        }

        private void FindDeviceTypes(Type type, DeviceTypeCollection deviceTypes)
        {
            foreach (DeviceType t in this.DeviceTypes)
            {
                if (type.IsInterface)
                {
                    if (t.Type.GetInterface(type.Name) != null)
                    {
                        deviceTypes.Add(t);
                    }
                }
                else
                {
                    if (Function.IsInheritableBaseType(t.Type, type) || t.Type.Equals(type))
                    {
                        deviceTypes.Add(t);
                    }
                }
            }

            foreach (DeviceTypeSort s in this.Children)
            {
                s.FindDeviceTypes(type, deviceTypes);
            }
        }

        internal DeviceTypeSort FindSort(string sortPath)
        {
            string[] strSortNames = sortPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (strSortNames.Length == 0)
            {
                return this;
            }
            else
            {
                foreach (DeviceTypeSort children in this.Children)
                {
                    if (children.Name.Equals(strSortNames[0]))
                    {
                        if (strSortNames.Length == 1)
                        {
                            return children;
                        }
                        else
                        {
                            string strSortName = "";
                            for (int intIndex = 1; intIndex < strSortNames.Length; intIndex++)
                            {
                                strSortName += '/' + strSortNames[intIndex];
                            }
                            return children.FindSort(strSortName.Substring(1));
                        }
                    }
                }

                DeviceTypeSort sort = new DeviceTypeSort(this.m_Application, strSortNames[0], this);
                this.Children.Add(sort);

                if (strSortNames.Length == 1)
                {
                    return sort;
                }
                else
                {
                    string strSortName = "";
                    for (int intIndex = 1; intIndex < strSortNames.Length; intIndex++)
                    {
                        strSortName += '/' + strSortNames[intIndex];
                    }
                    return sort.FindSort(strSortName.Substring(1));
                }
            }
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Name.Length == 0)
            {
                return "[设备分类]";
            }
            else
            {
                return this.Name;
            }
        }

        /// <summary>
        /// 设备类型分类集合。
        /// </summary>
        public class DeviceTypeSortCollection : ReadOnlyCollection<DeviceTypeSort>
        {
            internal DeviceTypeSortCollection()
            {
            }

            internal new void Add(DeviceTypeSort item)
            {
                base.Add(item);
            }

            /// <summary>
            /// 获取当前对象的字符串表示形式。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "共 " + this.Count + " 个设备类型分类";
            }
        }
    }
}

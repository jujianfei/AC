using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Drives;

namespace AC.Base
{
    /// <summary>
    /// 描述继承 Device 的设备类型。如果 ParentTypes 属性为 null 则表明该设备是一个通道，必须作为根节点设备；如果 ChildrenTypes 属性为 null 则表明该设备是一个末端节点的设备，不允许再连接下级设备。
    /// </summary>
    public class DeviceType
    {
        private ApplicationClass m_Application;

        internal DeviceType(ApplicationClass application, Type type)
        {
            this.m_Application = application;
            this.Type = type;
        }

        /// <summary>
        /// 获取当前设备的类型声明。
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 设备类型名。
        /// </summary>
        public string Name
        {
            get
            {
                return ((DeviceTypeAttribute)this.Type.GetCustomAttributes(typeof(DeviceTypeAttribute), false)[0]).Name;
            }
        }

        /// <summary>
        /// 设备类型代码。
        /// </summary>
        public string Code
        {
            get
            {
                return this.Type.FullName;
            }
        }

        /// <summary>
        /// 该设备类型所属分类。
        /// </summary>
        public DeviceTypeSort Sort { get; internal set; }

        /// <summary>
        /// 设备描述。有关该设备详细的说明。
        /// </summary>
        public string Description
        {
            get
            {
                return ((DeviceTypeAttribute)this.Type.GetCustomAttributes(typeof(DeviceTypeAttribute), false)[0]).Description;
            }
        }

        /// <summary>
        /// 该设备所使用的驱动类型声明。如该设备无驱动，则此属性返回 null。
        /// </summary>
        public Type DriveType
        {
            get
            {
                return ((DeviceTypeAttribute)this.Type.GetCustomAttributes(typeof(DeviceTypeAttribute), false)[0]).DriveType;
            }
        }

        /// <summary>
        /// 该设备必须挂接的上级设备类型。如果该设备是通道则返回 null，如果该设备对上级设备类型无任何限制则返回 new Type[] { }，否则返回上级设备类型。
        /// </summary>
        public Type[] ParentTypes
        {
            get
            {
                return ((DeviceTypeAttribute)this.Type.GetCustomAttributes(typeof(DeviceTypeAttribute), false)[0]).ParentTypes;
            }
        }

        /// <summary>
        /// 该设备允许连接的下级设备类型。如果该设备是末端设备不允许再连接子设备则返回 null，如果该设备对下级设备类型无任何限制则返回 new Type[] { }，否则返回下级设备类型。
        /// </summary>
        public Type[] ChildrenTypes
        {
            get
            {
                return ((DeviceTypeAttribute)this.Type.GetCustomAttributes(typeof(DeviceTypeAttribute), false)[0]).ChildrenTypes;
            }
        }

        /// <summary>
        /// 该类设备使用的 16 * 16 图标。如果设备未提供图标则此属性提供默认的设备图标。
        /// </summary>
        public System.Drawing.Image Icon16
        {
            get
            {
                IDeviceTypeImage img = this.GetImages(this.Type);
                if (img != null && img.Icon16 != null)
                {
                    return img.Icon16;
                }
                else
                {
                    return Properties.Resources.Device16;
                }
            }
        }

        /// <summary>
        /// 该类设备使用的 32 * 32 图标。如果设备未提供图标则此属性提供默认的设备图标。
        /// </summary>
        public System.Drawing.Image Icon32
        {
            get
            {
                IDeviceTypeImage img = this.GetImages(this.Type);
                if (img != null && img.Icon32 != null)
                {
                    return img.Icon32;
                }
                else
                {
                    return Properties.Resources.Device32;
                }
            }
        }

        private System.Collections.Generic.Dictionary<DeviceStateOptions, System.Drawing.Image> dicIcon16;
        internal System.Drawing.Image GetIcon16(DeviceStateOptions state)
        {
            if (this.dicIcon16 == null)
            {
                IDeviceTypeImage img = this.GetImages(this.Type);
                this.dicIcon16 = new Dictionary<DeviceStateOptions, System.Drawing.Image>();

                foreach (DeviceStateOptions deviceState in Enum.GetValues(typeof(DeviceStateOptions)))
                {
                    if (img != null)
                    {
                        if (img.stateIcon16 != null && img.stateIcon16[deviceState] != null)
                            this.dicIcon16.Add(deviceState, img.stateIcon16[deviceState]);
                        else
                            this.dicIcon16.Add(deviceState, this.Icon16);
                    }
                    else
                        this.dicIcon16.Add(deviceState, this.Icon16);
                }
            }
            return this.dicIcon16[state];
        }

        private System.Collections.Generic.Dictionary<DeviceStateOptions, System.Drawing.Image> dicIcon32;
        internal System.Drawing.Image GetIcon32(DeviceStateOptions state)
        {
            if (this.dicIcon32 == null)
            {
                this.dicIcon32 = new Dictionary<DeviceStateOptions, System.Drawing.Image>();

                foreach (DeviceStateOptions deviceState in Enum.GetValues(typeof(DeviceStateOptions)))
                {
                    this.dicIcon32.Add(deviceState, this.Icon32);
                }
            }
            return this.dicIcon32[state];
        }

        /// <summary>
        /// 该类设备的外观照片，如果未提供设备照片则返回 null。图片长宽应控制在 640*480 以内。
        /// </summary>
        public System.Drawing.Image Photo
        {
            get
            {
                IDeviceTypeImage img = this.GetImages(this.Type);
                if (img != null && img.Photo != null)
                {
                    return img.Photo;
                }
                else
                {
                    return null;
                }
            }
        }

        //该设备的图标和照片。
        private IDeviceTypeImage GetImages(Type deviceType)
        {
            if (deviceType.GetCustomAttributes(typeof(DeviceTypeAttribute), false).Length > 0)
            {
                DeviceTypeAttribute attr = (DeviceTypeAttribute)deviceType.GetCustomAttributes(typeof(DeviceTypeAttribute), false)[0];
                if (attr.ImageType != null && attr.ImageType.GetInterface(typeof(IDeviceTypeImage).FullName) != null)
                {
                    System.Reflection.ConstructorInfo ci = attr.ImageType.GetConstructor(new System.Type[] { });
                    object objInstance = ci.Invoke(new object[] { });

                    return objInstance as IDeviceTypeImage;
                }
                else
                {
                    if (deviceType.BaseType != typeof(Device) && deviceType.BaseType.IsAbstract == false)
                    {
                        return GetImages(deviceType.BaseType);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 当前设备类型能否作为根节点通道。
        /// </summary>
        /// <returns></returns>
        public bool CanRoot()
        {
            return this.ParentTypes == null;
        }

        /// <summary>
        /// 当前设备类型能否作为某种设备的子设备添加到该设备下。
        /// </summary>
        /// <param name="parentType">上级设备的类型。</param>
        /// <returns></returns>
        public bool CanChildren(DeviceType parentType)
        {
            if (this.ParentTypes == null)
            {
                //当前设备只能作为根节点通道
                return false;
            }
            else if (parentType.ChildrenTypes == null)
            {
                //上级设备不允许连接子设备。
                return false;
            }
            else
            {
                if (parentType.ChildrenTypes.Length == 1 && parentType.ChildrenTypes[0].Equals(typeof(Object)))
                {
                    //上级设备指明任何类型设备均可以作为其子设备
                    return true;
                }

                if (parentType.ChildrenTypes.Length > 0)
                {
                    //上级设备对下级设备有类型要求
                    bool bolTrue = false;
                    foreach (Type typChildren in parentType.ChildrenTypes)
                    {
                        //只要下级设备满足上级设备的一项要求，便认为适合做下级设备。
                        if (typChildren.IsClass)
                        {
                            if (Function.IsInheritableBaseType(this.Type, typChildren))
                            {
                                bolTrue = true;
                                break;
                            }
                            else if (this.DriveType != null && Function.IsInheritableBaseType(this.DriveType, typChildren))
                            {
                                bolTrue = true;
                                break;
                            }
                        }
                        else if (typChildren.IsInterface)
                        {
                            if (this.Type.GetInterface(typChildren.FullName) != null)
                            {
                                bolTrue = true;
                                break;
                            }
                            else if (this.DriveType != null && this.DriveType.GetInterface(typChildren.FullName) != null)
                            {
                                bolTrue = true;
                                break;
                            }
                        }
                    }

                    if (bolTrue == false)
                    {
                        return false;       //当前设备没有一条满足上级设备的要求
                    }
                }

                if (this.ParentTypes.Length > 0)
                {
                    //当前设备对上级设备有类型要求
                    bool bolTrue = false;
                    foreach (Type typParent in this.ParentTypes)
                    {
                        //只要上级设备满足当前设备的一项要求，便认为当前设备适合做下级设备。
                        if (typParent.IsClass)
                        {
                            if (Function.IsInheritableBaseType(parentType.Type, typParent))
                            {
                                bolTrue = true;
                                break;
                            }
                            else if (parentType.DriveType != null && Function.IsInheritableBaseType(parentType.DriveType, typParent))
                            {
                                bolTrue = true;
                                break;
                            }
                        }
                        else if (typParent.IsInterface)
                        {
                            if (parentType.Type.GetInterface(typParent.FullName) != null)
                            {
                                bolTrue = true;
                                break;
                            }
                            else if (parentType.DriveType != null && parentType.DriveType.GetInterface(typParent.FullName) != null)
                            {
                                bolTrue = true;
                                break;
                            }
                        }

                    }

                    if (bolTrue == false)
                    {
                        return false;       //上级设备没有一条满足当前设备的要求
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// 获取当前类型设备的新实例。
        /// </summary>
        /// <returns>当前类型设备的新实例。</returns>
        public Device CreateDevice()
        {
            System.Reflection.ConstructorInfo ci = this.Type.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            Device device = objInstance as Device;
  
            device.SetApplication(this.m_Application);
            device.DeviceType = this;
            return device;
        }

        /// <summary>
        /// 获取当前类型设备的新实例。
        /// </summary>
        /// <param name="parent">上级设备。</param>
        /// <returns>当前类型设备的新实例。</returns>
        public Device CreateDevice(Device parent)
        {
            System.Reflection.ConstructorInfo ci = this.Type.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            Device device = objInstance as Device;
            device.SetApplication(this.m_Application);
            device.DeviceType = this;
            device.SetParent(parent);
            return device;
        }

        /// <summary>
        /// 创建当前设备类型的驱动对象，如该设备无驱动则返回 null。
        /// </summary>
        /// <returns></returns>
        internal IDrive CreateDrive()
        {
            System.Reflection.ConstructorInfo ci = this.DriveType.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            return objInstance as IDrive;
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
    
    /// <summary>
    /// 设备类型集合。
    /// </summary>
    public class DeviceTypeCollection : ReadOnlyCollection<DeviceType>
    {
        internal DeviceTypeCollection()
        {
        }

        internal new void Add(DeviceType item)
        {
            if (DeviceTypeSort.AllDeviceTypes.ContainsKey(item.Code) == false)
            {
                DeviceTypeSort.AllDeviceTypes.Add(item.Code, item);
                base.Items.Add(item);
            }
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "共 " + this.Count + " 个设备类型";
        }
    }
}

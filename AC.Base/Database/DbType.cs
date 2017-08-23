using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Database
{
    /// <summary>
    /// 描述某种数据库类型。
    /// </summary>
    public class DbType
    {
        private ApplicationClass m_Application;

        internal DbType(ApplicationClass application, Type type)
        {
            this.m_Application = application;
            this.Type = type;
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 数据库类型名。
        /// </summary>
        public string Name
        {
            get
            {
                return ((DbTypeAttribute)this.Type.GetCustomAttributes(typeof(DbTypeAttribute), false)[0]).Name;
            }
        }

        /// <summary>
        /// 数据库类型代码。
        /// </summary>
        public string Code
        {
            get
            {
                return this.Type.FullName;
            }
        }

        /// <summary>
        /// 数据库描述。有关该数据库详细的说明。
        /// </summary>
        public string Description
        {
            get
            {
                return ((DbTypeAttribute)this.Type.GetCustomAttributes(typeof(DbTypeAttribute), false)[0]).Description;
            }
        }

        /// <summary>
        /// 该类数据库使用的 16 * 16 图标。
        /// </summary>
        public System.Drawing.Image Icon16
        {
            get
            {
                IIcon img = this.GetImages(this.Type);
                if (img != null && img.Icon16 != null)
                {
                    return img.Icon16;
                }
                else
                {
                    return Properties.Resources.SQLServer16;
                }
            }
        }

        /// <summary>
        /// 该类数据库使用的 32 * 32 图标。
        /// </summary>
        public System.Drawing.Image Icon32
        {
            get
            {
                IIcon img = this.GetImages(this.Type);
                if (img != null && img.Icon32 != null)
                {
                    return img.Icon32;
                }
                else
                {
                    return Properties.Resources.SQLServer32;
                }
            }
        }

        /// <summary>
        /// 该数据库的图标和照片。
        /// </summary>
        /// <returns></returns>
        private IIcon GetImages(Type dbType)
        {
            if (dbType.GetCustomAttributes(typeof(DbTypeAttribute), false).Length > 0)
            {
                DbTypeAttribute attr = (DbTypeAttribute)dbType.GetCustomAttributes(typeof(DbTypeAttribute), false)[0];
                if (attr.ImageType != null && attr.ImageType.GetInterface(typeof(IIcon).Name) != null)
                {
                    System.Reflection.ConstructorInfo ci = attr.ImageType.GetConstructor(new System.Type[] { });
                    object objInstance = ci.Invoke(new object[] { });

                    return objInstance as IIcon;
                }
                else
                {
                    if (dbType.BaseType != typeof(IDb) && dbType.BaseType.IsAbstract == false)
                    {
                        return GetImages(dbType.BaseType);
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
        /// 获取数据库连接对象的新实例。
        /// </summary>
        /// <returns></returns>
        public IDb CreateDb()
        {
            System.Reflection.ConstructorInfo ci = this.Type.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            return objInstance as IDb;
        }

        /// <summary>
        /// 获取当前数据库的连接配置界面。
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public IDbConfigControl CreateConfigControl(Type baseType)
        {
            Type type = this.m_Application.GetControlType(this.Type, baseType);
            if (type != null)
            {
                if (type.GetInterface(typeof(IDbConfigControl).FullName) != null && Function.IsInheritableBaseType(type, baseType))
                {
                    System.Reflection.ConstructorInfo ci = type.GetConstructor(new System.Type[] { });
                    object objInstance = ci.Invoke(new object[] { });
                    return objInstance as IDbConfigControl;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Type.Name;
        }
    }

    /// <summary>
    /// 数据库类型集合。
    /// </summary>
    public class DbTypeCollection : ReadOnlyCollection<DbType>
    {
        internal DbTypeCollection()
        {
        }

        internal new void Add(DbType item)
        {
            base.Items.Add(item);
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "共 " + this.Count + " 个数据库类型";
        }
    }
}

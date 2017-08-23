using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 描述继承 Classify 的分类类型。
    /// </summary>
    public class ClassifyType
    {
        private ApplicationClass m_Application;

        internal ClassifyType(ApplicationClass application, Type type)
        {
            this.m_Application = application;
            this.Type = type;
            this.m_NeedCache = ((ClassifyTypeAttribute)this.Type.GetCustomAttributes(typeof(ClassifyTypeAttribute), false)[0]).NeedCache;
        }

        /// <summary>
        /// 获取当前分类的类型声明。
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 分类类型名。
        /// </summary>
        public string Name
        {
            get
            {
                return ((ClassifyTypeAttribute)this.Type.GetCustomAttributes(typeof(ClassifyTypeAttribute), false)[0]).Name;
            }
        }

        /// <summary>
        /// 分类类型代码。
        /// </summary>
        public string Code
        {
            get
            {
                return this.Type.FullName;
            }
        }

        private bool m_NeedCache;
        /// <summary>
        /// 该类型分类是否需要进行缓存。通常对部门、行业等数量不多但是层级较多的分类进行缓存，对用户等数量较多层级简单的分类不进行缓存。
        /// </summary>
        public bool NeedCache
        {
            get
            {
                return this.m_NeedCache;
            }
        }


        /// <summary>
        /// 获取当前类型分类的新实例。
        /// </summary>
        /// <returns>当前类型分类的新实例。</returns>
        public Classify CreateClassify()
        {
            System.Reflection.ConstructorInfo ci = this.Type.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            Classify classify = objInstance as Classify;
            classify.SetApplication(this.m_Application);
            classify.ClassifyType = this;
            return classify;
        }

        /// <summary>
        /// 获取当前类型分类的新实例。
        /// </summary>
        /// <param name="parent">上级分类。</param>
        /// <returns>当前类型分类的新实例。</returns>
        public Classify CreateClassify(Classify parent)
        {
            System.Reflection.ConstructorInfo ci = this.Type.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            Classify classify = objInstance as Classify;
            classify.SetApplication(this.m_Application);
            classify.ClassifyType = this;
            classify.SetParent(parent);
            return classify;
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
    /// 分类类型集合。
    /// </summary>
    public class ClassifyTypeCollection : ReadOnlyCollection<ClassifyType>
    {
        internal ClassifyTypeCollection()
        {
        }

        /// <summary>
        /// 获取指定类型名的分类类型。
        /// </summary>
        /// <param name="typeName">类型名称，如“命名空间1.命名空间2.分类类名”</param>
        /// <returns>分类类型。如无对应的分类类型则返回 null。</returns>
        public ClassifyType GetClassifyType(string typeName)
        {
            foreach (ClassifyType _ClassifyType in this)
            {
                if (_ClassifyType.Code.Equals(typeName))
                {
                    return _ClassifyType;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定类型声明的分类类型。
        /// </summary>
        /// <param name="type">分类类型声明</param>
        /// <returns>分类类型。如无对应的分类类型则返回 null。</returns>
        public ClassifyType GetClassifyType(Type type)
        {
            foreach (ClassifyType _ClassifyType in this)
            {
                if (_ClassifyType.Type.Equals(type))
                {
                    return _ClassifyType;
                }
            }
            return null;
        }

        internal new void Add(ClassifyType item)
        {
            base.Items.Add(item);
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "共 " + this.Count + " 个分类类型";
        }
    }
}

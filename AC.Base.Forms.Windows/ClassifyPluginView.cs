using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Forms;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 分类插件视图。
    /// </summary>
    public class ClassifyPluginView : PluginViewBase
    {
        /// <summary>
        /// 分类插件视图。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="pluginType">插件类型。</param>
        /// <param name="classifys">该视图需要呈现的分类对象。</param>
        public ClassifyPluginView(WindowsFormApplicationClass application, PluginType pluginType, Classify[] classifys)
            : base(application, pluginType)
        {
            this.Classifys = classifys;
        }

        /// <summary>
        /// 该视图所使用的分类。
        /// </summary>
        public Classify[] Classifys { get; private set; }

        /// <summary>
        /// 设置插件参数。PluginViewBase将插件初始化后调用该方法，由继承的类对插件进行一些设置后再进行后续的加载步骤。
        /// </summary>
        /// <param name="plugin">需要被设置参数的插件。</param>
        protected override void SetPlugin(IPlugin plugin)
        {
            IClassifyPlugin classifyPlugin = plugin as IClassifyPlugin;
            classifyPlugin.SetClassifys(this.Classifys);
        }

        /// <summary>
        /// 获取显示在视图标签页上的文字。
        /// </summary>
        /// <returns></returns>
        public override string GetViewTitle()
        {
            string strClassifyName = "";
            for (int intIndex = 0; intIndex < this.Classifys.Length; intIndex++)
            {
                if (intIndex < 2)
                {
                    strClassifyName += "," + this.Classifys[intIndex].Name;
                }
            }
            if (this.Classifys.Length > 2)
            {
                strClassifyName += "...";
            }
            return this.PluginType.Name + " - " + strClassifyName.Substring(1);
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string strClassifyName = "";
            foreach (Classify classify in this.Classifys)
            {
                strClassifyName += "," + classify.Name;
            }
            return this.PluginType.Name + " - " + strClassifyName.Substring(1);
        }
    }
}

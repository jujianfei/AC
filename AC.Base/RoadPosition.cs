using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 路灯处各部门。
    /// </summary>
    [ClassifyType("路段灯杆", false)]
    public class RoadPosition : Classify
    {
        /// <summary>
        /// 获取当前分类 16*16 像素图标。
        /// </summary>
        /// <returns></returns>
        public override System.Drawing.Image GetIcon16()
        {
            return Properties.Resources.ChannelService16;
        }

        /// <summary>
        /// 获取当前分类 32*32 图标。
        /// </summary>
        /// <returns></returns>
        public override System.Drawing.Image GetIcon32()
        {
            return Properties.Resources.ChannelService32;
        }

        private static bool IconIsSaved = false;
        private void IconSave()
        {
            if (IconIsSaved == false)
            {
                this.GetIcon16().Save(this.Application.TemporaryDirectory + this.GetType().Name + "16.gif");
                this.GetIcon32().Save(this.Application.TemporaryDirectory + this.GetType().Name + "32.gif");
            }
        }

        /// <summary>
        /// 获取当前分类 16*16 像素的图标路径。
        /// </summary>
        /// <returns></returns>
        public override string GetIcon16Url()
        {
            this.IconSave();
            return this.Application.TemporaryDirectoryRelativePath + this.GetType().Name + "16.gif";
        }

        /// <summary>
        /// 获取当前分类 32*32像素的图标路径。
        /// </summary>
        /// <returns></returns>
        public override string GetIcon32Url()
        {
            this.IconSave();
            return this.Application.TemporaryDirectoryRelativePath + this.GetType().Name + "32.gif";
        }
    }
}

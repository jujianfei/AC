using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 公共分类。
    /// </summary>
    [ClassifyType("公共分类", true)]
    public class CommonCustomClassify : Classify
    {
        /// <summary>
        /// 获取当前分类 16*16 像素图标。
        /// </summary>
        /// <returns></returns>
        public override System.Drawing.Image GetIcon16()
        {
            if (base.GetLevel() == 0)
            {
                return Properties.Resources.CommonClassifyRoot16;
            }
            else if (base.GetLevel() == 1)
            {
                return Properties.Resources.CommonClassify16;
            }
            else
            {
                return Properties.Resources.CommonClassifyItem16;
            }
        }

        /// <summary>
        /// 获取当前分类 32*32 图标。
        /// </summary>
        /// <returns></returns>
        public override System.Drawing.Image GetIcon32()
        {
            if (base.GetLevel() == 0)
            {
                return Properties.Resources.CommonClassifyRoot32;
            }
            else if (base.GetLevel() == 1)
            {
                return Properties.Resources.CommonClassify32;
            }
            else
            {
                return Properties.Resources.CommonClassifyItem32;
            }
        }

        private static bool IconUrlIsSave = false;
        private static void IconUrlSave(ApplicationClass application)
        {
            if (IconUrlIsSave == false)
            {
                Properties.Resources.CommonClassifyRoot16.Save(application.TemporaryDirectory + "CommonClassifyRoot16.gif");
                Properties.Resources.CommonClassifyRoot32.Save(application.TemporaryDirectory + "CommonClassifyRoot32.gif");
                Properties.Resources.CommonClassify16.Save(application.TemporaryDirectory + "CommonClassify16.gif");
                Properties.Resources.CommonClassify32.Save(application.TemporaryDirectory + "CommonClassify32.gif");
                Properties.Resources.CommonClassifyItem16.Save(application.TemporaryDirectory + "CommonClassifyItem16.gif");
                Properties.Resources.CommonClassifyItem32.Save(application.TemporaryDirectory + "CommonClassifyItem32.gif");

                IconUrlIsSave = true;
            }
        }

        /// <summary>
        /// 获取当前设备16*16像素含设备状态的图标路径。
        /// </summary>
        /// <returns></returns>
        public override string GetIcon16Url()
        {
            IconUrlSave(base.Application);

            if (base.GetLevel() == 0)
            {
                return base.Application.TemporaryDirectoryRelativePath + "CommonClassifyRoot16.gif";
            }
            else if (base.GetLevel() == 1)
            {
                return base.Application.TemporaryDirectoryRelativePath + "CommonClassify16.gif";
            }
            else
            {
                return base.Application.TemporaryDirectoryRelativePath + "CommonClassifyItem16.gif";
            }
        }

        /// <summary>
        /// 获取当前设备32*32像素含设备状态的图标路径。
        /// </summary>
        /// <returns></returns>
        public override string GetIcon32Url()
        {
            IconUrlSave(base.Application);

            if (base.GetLevel() == 0)
            {
                return base.Application.TemporaryDirectoryRelativePath + "CommonClassifyRoot32.gif";
            }
            else if (base.GetLevel() == 1)
            {
                return base.Application.TemporaryDirectoryRelativePath + "CommonClassify32.gif";
            }
            else
            {
                return base.Application.TemporaryDirectoryRelativePath + "CommonClassifyItem32.gif";
            }
        }
    }
}

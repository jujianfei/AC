using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// HTML 插件。不能直接实现该接口，而应实现具体的如 IGlobalHtmlPlugin、IDeviceHtmlPlugin 等接口。该接口为需要使用 HTML 方式呈现页面提供接口支持。
    /// </summary>
    public interface IHtmlPlugin : IPlugin
    {
        /// <summary>
        /// 向页面输出 HTML，使用 UTF-8 编码格式。
        /// </summary>
        /// <param name="output">字符输出对象，调用 WriteLine 方法向界面输出 HTML 内容。</param>
        void WriterHtml(System.IO.TextWriter output);
    }
}

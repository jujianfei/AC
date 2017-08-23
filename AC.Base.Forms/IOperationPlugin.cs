using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOperationPlugin : IGlobalPlugin, IHtmlPlugin
    {
       /// <summary>
       /// 
       /// </summary>
       /// <param name="ID"></param>
        void SetParameter(string  ID);
    }
}

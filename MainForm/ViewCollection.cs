using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Forms.Windows;

namespace MainForm
{
    internal class ViewCollection : AC.Base.Forms.Windows.IViewCollection
    {
        private frmMain m_MainForm;
        public ViewCollection(frmMain mainForm)
        {
            this.m_MainForm = mainForm;
        }

        #region IEnumerable<ViewBase> 成员

        public IEnumerator<AC.Base.Forms.Windows.ViewBase> GetEnumerator()
        {
            return new ViewEnumerator(this.m_MainForm);
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new ViewEnumerator(this.m_MainForm);
        }

        #endregion

        #region IViewCollection 成员

        public void Load(ViewBase view, ViewDockOptions viewDock)
        {
            //this.m_MainForm.DeivceLoadView(view);
        }

        public PluginViewBase GetView(AC.Base.Forms.IPlugin plugin)
        {
            foreach (ViewBase view in this)
            {
                if (view is PluginViewBase)
                {
                    if (view.Equals(plugin))
                    {
                        return view as PluginViewBase;
                    }
                }
            }

            return null;
        }

        public ControlView GetView(System.Windows.Forms.Control control)
        {
            foreach (ViewBase view in this)
            {
                if (view is ControlView)
                {
                    if (view.Equals(control))
                    {
                        return view as ControlView;
                    }
                }
            }

            return null;
        }

        #endregion
    }

    public class ViewEnumerator : IEnumerator<ViewBase>
    {
        private List<ViewBase> _vb;
        private frmMain m_MainForm;
        private int m_intIndex = -1;
        public ViewEnumerator(frmMain _mainForm)
        { 
            this.m_MainForm = _mainForm;
            _vb = new List<ViewBase>();
        }

        #region IEnumerator<ViewBase> 成员

        public ViewBase Current
        {
            get { return this._vb[this.m_intIndex] ; }
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            this._vb = null;
        }

        #endregion

        #region IEnumerator 成员

        object System.Collections.IEnumerator.Current
        {
            get { return this._vb[this.m_intIndex]; }
        }

        public bool MoveNext()
        {
            this.m_intIndex++;
            if (this.m_intIndex < this._vb.Count)
                return true;
            else
                return false;
        }

        public void Reset()
        {
            this.m_intIndex = -1;
        }

        #endregion
    }
}

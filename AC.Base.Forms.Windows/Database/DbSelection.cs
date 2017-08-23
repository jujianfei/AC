using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.Database;

namespace AC.Base.Forms.Windows.Database
{
    /// <summary>
    /// 数据库连接选择控件。可以通过 Db 属性获取或设置操作员当前选择的数据库连接信息。
    /// </summary>
    public class DbSelection : Control
    {
        private FormApplicationClass Application;
        private GroupBox gbDbConfig;
        private ComboBox cmbDbType;
        private IEnumerable<IDb> m_Dbs;

        /// <summary>
        /// 数据库连接选择控件。可以通过 Db 属性获取或设置操作员当前选择的数据库连接信息。
        /// </summary>
        /// <param name="application"></param>
        /// <param name="dbs">已配置的数据库连接信息。如果操作员选择某种数据库类型且该类型存在该数组中，则使用该已配置的数据库类型加载界面，否则界面显示默认空白值。</param>
        public DbSelection(FormApplicationClass application, IEnumerable<IDb> dbs)
        {
            this.Application = application;
            this.m_Dbs = dbs;

            this.gbDbConfig = new GroupBox();
            this.gbDbConfig.Dock = DockStyle.Fill;
            this.Controls.Add(this.gbDbConfig);

            Panel panDbType = new Panel();
            panDbType.Dock = DockStyle.Top;
            panDbType.Size = new System.Drawing.Size(0, 20);
            this.Controls.Add(panDbType);

            cmbDbType = new ComboBox();
            cmbDbType.Dock = DockStyle.Left;
            cmbDbType.Size = new System.Drawing.Size(140, 20);
            cmbDbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDbType.SelectedIndexChanged += new EventHandler(cmbDbType_SelectedIndexChanged);
            foreach (DbType dbType in application.DbTypes)
            {
                if (this.Application.GetControlTypes(dbType.Type).Count > 0)
                {
                    cmbDbType.Items.Add(dbType);
                }
            }
            if (cmbDbType.Items.Count > 0)
            {
                cmbDbType.SelectedIndex = 0;
            }
            panDbType.Controls.Add(cmbDbType);

            Label labDbType = new Label();
            labDbType.Dock = System.Windows.Forms.DockStyle.Left;
            labDbType.Size = new System.Drawing.Size(80, 20);
            labDbType.Text = "数据库类型:";
            labDbType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            panDbType.Controls.Add(labDbType);
        }

        private void cmbDbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.gbDbConfig.Controls.Count > 0)
            {
                for (int intIndex = 0; intIndex < this.gbDbConfig.Controls.Count; intIndex++)
                {
                    this.gbDbConfig.Controls[0].Dispose();
                }
            }

            DbType dbType = ((ComboBox)sender).SelectedItem as DbType;
            IDb db = null;

            if (this.m_Dbs != null)
            {
                foreach (IDb db1 in this.m_Dbs)
                {
                    if (dbType.Type.Equals(db1.GetType()))
                    {
                        db = db1;
                        break;
                    }
                }
            }

            if (db == null)
            {
                db = dbType.CreateDb();
            }

            IDbConfigControl dbConfig = dbType.CreateConfigControl(typeof(System.Windows.Forms.Control));
            dbConfig.SetDb(db);

            Control ctl = dbConfig as Control;
            ctl.Dock = DockStyle.Fill;
            this.gbDbConfig.Controls.Add(ctl);
        }

        /// <summary>
        /// 获取或设置当前配置界面所使用的数据库连接配置信息。
        /// </summary>
        public IDb Db
        {
            get
            {
                if (this.gbDbConfig.Controls.Count > 0)
                {
                    IDbConfigControl dbConfig = this.gbDbConfig.Controls[0] as IDbConfigControl;
                    if (dbConfig != null)
                    {
                        return dbConfig.GetDb();
                    }
                }

                throw new Exception("未选择数据库类型。");
            }
            set
            {
                for (int intIndex = 0; intIndex < this.cmbDbType.Items.Count; intIndex++)
                {
                    DbType dbType = this.cmbDbType.Items[intIndex] as DbType;
                    if (dbType.Type.Equals(value.GetType()))
                    {
                        if (this.cmbDbType.SelectedIndex != intIndex)
                        {
                            this.cmbDbType.SelectedIndex = intIndex;
                        }

                        IDbConfigControl dbConfig = this.gbDbConfig.Controls[0] as IDbConfigControl;
                        dbConfig.SetDb(value);
                        break;
                    }
                }
            }
        }
    }
}

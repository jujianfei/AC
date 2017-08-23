using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.Database;

namespace AC.Base.Forms.Windows.Database
{
    /// <summary>
    /// SQL Server 数据库连接配置。
    /// </summary>
    [Control(typeof(SQLServerDb))]
    public partial class SQLServerDbConfig : UserControl, IDbConfigControl
    {
        private SQLServerDb m_Db;

        /// <summary>
        /// SQL Server 数据库连接配置。
        /// </summary>
        public SQLServerDbConfig()
        {
            InitializeComponent();
        }

        private void SQLServerDbConfig_Load(object sender, EventArgs e)
        {
            this.cmbInstance.Items.Add("<正在搜索服务器实例，请稍候...>");

            System.Threading.Thread thrSqlServerInstance = new System.Threading.Thread(FindSqlServerInstanceNames);
            thrSqlServerInstance.IsBackground = true;
            thrSqlServerInstance.Start();
        }

        //搜索 SQL Server 服务器实例
        private void FindSqlServerInstanceNames()
        {
            try
            {
                cmbInstance_AddInstanceNames_BeginInvoke(SQLServerDb.GetSqlServerInstances());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbInstance_AddInstanceNames_BeginInvoke(IEnumerable<string> instanceNames)
        {
            try
            {
                this.cmbInstance.BeginInvoke(new cmbInstance_AddInstanceNames_Invoke(cmbInstance_AddInstanceNames), instanceNames);
            }
            catch { }
        }

        private delegate void cmbInstance_AddInstanceNames_Invoke(IEnumerable<string> instanceNames);
        private void cmbInstance_AddInstanceNames(IEnumerable<string> instanceNames)
        {
            this.cmbInstance.Items.Clear();

            int intIndex = 0;
            foreach (string strServerInstance in instanceNames)
            {
                intIndex++;
                this.cmbInstance.Items.Add(strServerInstance);
            }

            this.labInstanceMsg.Text = "(搜索到 " + intIndex + " 个服务器实例)";
        }

        #region IDbConfigControl 成员

        /// <summary>
        /// 设置数据库对象，并将该连接内容显示在界面控件上。
        /// </summary>
        /// <param name="db"></param>
        public void SetDb(IDb db)
        {
            this.m_Db = db as SQLServerDb;

            this.cmbInstance.Text = this.m_Db.InstanceName;
            if (this.m_Db.LoginSecure)
            {
                this.radLoginWindows.Checked = true;
            }
            else
            {
                this.radLoginSQLServer.Checked = true;
                this.txtLoginUsername.Text = this.m_Db.UserName;
                this.txtLoginPassword.Text = this.m_Db.Password;
            }
            this.cmbDatabase.Text = this.m_Db.DatabaseName;
        }

        /// <summary>
        /// 获取该数据库连接配置信息。
        /// </summary>
        /// <returns></returns>
        public IDb GetDb()
        {
            this.m_Db.InstanceName = this.cmbInstance.Text;
            this.m_Db.LoginSecure = this.radLoginWindows.Checked;
            this.m_Db.UserName = this.txtLoginUsername.Text;
            this.m_Db.Password = this.txtLoginPassword.Text;
            this.m_Db.DatabaseName = this.cmbDatabase.Text;
            return this.m_Db;
        }

        #endregion

        private void cmbInstance_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.cmbDatabase.Text = "";
            this.cmbDatabase.Items.Clear();
        }

        private void radLoginWindows_CheckedChanged(object sender, EventArgs e)
        {
            LoginCheckedChanged();
        }

        private void radLoginSQLServer_CheckedChanged(object sender, EventArgs e)
        {
            LoginCheckedChanged();
        }

        private void LoginCheckedChanged()
        {
            if (this.radLoginWindows.Checked)
            {
                this.txtLoginUsername.Enabled = false;
                this.txtLoginPassword.Enabled = false;
            }
            else
            {
                this.txtLoginUsername.Enabled = true;
                this.txtLoginPassword.Enabled = true;
            }
        }

        //查看 SQL Server 某实例的所有数据库
        private bool m_IsReadDatabase = false;                          //是否已经读取过该实例的所有数据库了
        private void cmbDatabase_DropDown(object sender, EventArgs e)
        {
            if (m_IsReadDatabase == false)
            {
                this.cmbDatabase.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                LoadDatabases();
            }

            this.cmbDatabase.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void LoadDatabases()
        {
            this.cmbDatabase.Items.Clear();

            try
            {
                IEnumerable<string> databases = SQLServerDb.GetSqlServerDatabases(this.cmbInstance.Text, this.radLoginWindows.Checked, this.txtLoginUsername.Text, this.txtLoginPassword.Text);
                foreach (string strName in databases)
                {
                    this.cmbDatabase.Items.Add(strName);
                }

                this.cmbDatabase.Items.Add("<新建...>");

                this.m_IsReadDatabase = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SQL Server 数据库", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbDatabase_Leave(object sender, EventArgs e)
        {
            //失去焦点后，用户下次再点击下拉列表时将重新读取数据库集合
            m_IsReadDatabase = false;
        }

        private void cmbDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbDatabase.SelectedIndex == this.cmbDatabase.Items.Count - 1)
            {
                try
                {
                    frmCreateSQLServerOptions _frmCreateSQLServerOptions = new frmCreateSQLServerOptions();
                    if (_frmCreateSQLServerOptions.ShowDialog() == DialogResult.OK)
                    {
                        string strDatabaseName = _frmCreateSQLServerOptions.DatabaseName;
                        SQLServerDb.CreateDatabase(this.cmbInstance.Text, this.radLoginWindows.Checked, this.txtLoginUsername.Text, this.txtLoginPassword.Text, strDatabaseName, _frmCreateSQLServerOptions.DatabasePath, _frmCreateSQLServerOptions.DatabaseSize);

                        LoadDatabases();
                        this.cmbDatabase.SelectedItem = strDatabaseName;

                        MessageBox.Show("数据库“" + strDatabaseName + "”已成功创建。", "SQL Server 数据库", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        this.cmbDatabase.SelectedText = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "SQL Server 数据库", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.cmbDatabase.SelectedText = "";
                }
            }
        }

    }
}

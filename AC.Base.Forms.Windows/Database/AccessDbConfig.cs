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
    /// Access 数据库连接配置。
    /// </summary>
    [Control(typeof(AccessDb))]
    public partial class AccessDbConfig : UserControl, IDbConfigControl
    {
        private AccessDb m_Db;

        /// <summary>
        /// Access 数据库连接配置。
        /// </summary>
        public AccessDbConfig()
        {
            InitializeComponent();
        }

        #region IDbConfigControl 成员

        /// <summary>
        /// 设置数据库对象，并将该连接内容显示在界面控件上。
        /// </summary>
        /// <param name="db"></param>
        public void SetDb(IDb db)
        {
            this.m_Db = db as AccessDb;

            this.txtFileName.Text = this.m_Db.FileName;
            this.txtPassword.Text = this.m_Db.Password;
        }

        /// <summary>
        /// 获取该数据库连接配置信息。
        /// </summary>
        /// <returns></returns>
        public IDb GetDb()
        {
            this.m_Db.FileName = this.txtFileName.Text;
            this.m_Db.Password = this.txtPassword.Text;
            return this.m_Db;
        }

        #endregion

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "mdb";
            ofd.Filter = "Access 数据库文件(*.mdb)|*.mdb|所有文件|*.*";
            ofd.Multiselect = false;
            ofd.Title = "选择 Access 数据库文件";
            ofd.FileName = this.m_Db.FileName;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.txtFileName.Text = ofd.FileName;
            }
        }

        private void btnCreateMDB_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "mdb";
            sfd.Filter = "Access 数据库文件(*.mdb)|*.mdb|所有文件|*.*";
            sfd.Title = "设置 Access 数据库的文件名";
            sfd.FileName = this.m_Db.FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    AccessDb _AccessDb = AccessDb.CreateDbInstance(sfd.FileName);
                    this.txtFileName.Text = _AccessDb.FileName;

                    MessageBox.Show("数据库已创建。", "Access 数据库", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Access 数据库", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

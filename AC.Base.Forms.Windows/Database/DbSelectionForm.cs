using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using AC.Base.Database;

namespace AC.Base.Forms.Windows.Database
{
    /// <summary>
    /// 数据库类型选择窗体。可以通过 Db 属性获取或设置操作员当前选择的数据库连接信息。
    /// </summary>
    public class DbSelectionForm : Form
    {
        private DbSelection m_DbSelection;
        private List<IDb> lstDbs;

        /// <summary>
        /// 数据库类型选择窗体。可以通过 Db 属性获取或设置操作员当前选择的数据库连接信息。
        /// </summary>
        /// <param name="application"></param>
        public DbSelectionForm(FormApplicationClass application)
        {
            this.lstDbs = new List<IDb>();
            IDb dbDefault = null;

            //删除了属性(XML) DbSelection
            //if (Properties.Settings.Default.DbSelection != null)
            //{
            //    System.Xml.XmlDocument xmlDoc = Properties.Settings.Default.DbSelection;
            //    if (xmlDoc.ChildNodes.Count > 0)
            //    {
            //        foreach (System.Xml.XmlNode xnDb in xmlDoc.ChildNodes[0].ChildNodes)
            //        {
            //            System.Xml.XmlNode xaDbType = xnDb.Attributes.GetNamedItem("DbType");
            //            string strDbType = xaDbType.InnerText;

            //            foreach (DbType dbType in application.DbTypes)
            //            {
            //                if (dbType.Type.FullName.Equals(strDbType))
            //                {
            //                    IDb db = dbType.CreateDb();
            //                    db.SetConfig(xnDb);

            //                    this.lstDbs.Add(db);


            //                    if (xnDb.Attributes.GetNamedItem("Default") != null)
            //                    {
            //                        dbDefault = db;
            //                    }
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}

            this.Padding = new System.Windows.Forms.Padding(8);
            this.Size = new Size(480, 320);
            this.ShowIcon = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.Text = "选择数据库类型";

            this.m_DbSelection = new DbSelection(application, this.lstDbs);
            this.m_DbSelection.Dock = DockStyle.Fill;
            this.Controls.Add(this.m_DbSelection);

            if (dbDefault != null)
            {
                this.m_DbSelection.Db = dbDefault;
            }

            Panel panButton = new System.Windows.Forms.Panel();
            Panel panButton2 = new System.Windows.Forms.Panel();
            Button btnCancel = new System.Windows.Forms.Button();
            Panel panButton3 = new System.Windows.Forms.Panel();
            Button btnAccept = new System.Windows.Forms.Button();

            panButton.Controls.Add(panButton2);
            panButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            panButton.Location = new System.Drawing.Point(8, 237);
            panButton.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            panButton.Size = new System.Drawing.Size(389, 28);

            panButton2.Controls.Add(btnCancel);
            panButton2.Controls.Add(panButton3);
            panButton2.Controls.Add(btnAccept);
            panButton2.Dock = System.Windows.Forms.DockStyle.Right;
            panButton2.Location = new System.Drawing.Point(226, 5);
            panButton2.Size = new System.Drawing.Size(163, 23);

            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Dock = System.Windows.Forms.DockStyle.Left;
            btnCancel.Location = new System.Drawing.Point(80, 0);
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.Text = "取消";
            this.CancelButton = btnCancel;

            panButton3.Dock = System.Windows.Forms.DockStyle.Left;
            panButton3.Location = new System.Drawing.Point(75, 0);
            panButton3.Size = new System.Drawing.Size(5, 23);

            btnAccept.Dock = System.Windows.Forms.DockStyle.Left;
            btnAccept.Location = new System.Drawing.Point(0, 0);
            btnAccept.Size = new System.Drawing.Size(75, 23);
            btnAccept.Text = "确定";
            btnAccept.Click += new EventHandler(btnAccept_Click);
            this.AcceptButton = btnAccept;

            this.Controls.Add(panButton);
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认使用该数据库连接吗？", "数据库连接", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
            {
                IDb db = this.Db;
                bool bolIsAdd = true;
                for (int intIndex = 0; intIndex < this.lstDbs.Count; intIndex++)
                {
                    if (this.lstDbs[intIndex].GetType().Equals(db.GetType()))
                    {
                        this.lstDbs[intIndex] = db;
                        bolIsAdd = false;
                        break;
                    }
                }
                if (bolIsAdd)
                {
                    this.lstDbs.Add(db);
                }

                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                System.Xml.XmlNode xnDbs = xmlDoc.CreateElement("Dbs");
                xmlDoc.AppendChild(xnDbs);

                foreach (IDb db1 in this.lstDbs)
                {
                    System.Xml.XmlNode xnDb = db1.GetConfig(xmlDoc);

                    System.Xml.XmlAttribute xaDbType = xmlDoc.CreateAttribute("DbType");
                    xaDbType.InnerText = db1.GetType().FullName;
                    xnDb.Attributes.Append(xaDbType);

                    if (db1.Equals(db))
                    {
                        System.Xml.XmlAttribute xaDefault = xmlDoc.CreateAttribute("Default");
                        xnDb.Attributes.Append(xaDefault);
                    }

                    xnDbs.AppendChild(xnDb);
                }

                //Properties.Settings.Default.DbSelection = xmlDoc;
                //Properties.Settings.Default.Save();

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        /// <summary>
        /// 获取或设置当前配置界面所使用的数据库连接配置信息。
        /// </summary>
        public IDb Db
        {
            get
            {
                return this.m_DbSelection.Db;
            }
            set
            {
                this.m_DbSelection.Db = value;
            }
        }
    }
}

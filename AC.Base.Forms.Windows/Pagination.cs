using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 用户在翻页控件上点击翻页按钮后产生的事件所用到的委托。
    /// </summary>
    /// <param name="pageNumber">需要转到的页数。</param>
    public delegate void PagingEventHandler(int pageNumber);

    /// <summary>
    /// 为实现 IPagination 分页接口的插件提供翻页操作的界面。
    /// </summary>
    public class Pagination : System.Windows.Forms.TableLayoutPanel
    {
        private Label labPageInfo;
        private Button btnBack;
        private Button btnNext;
        private ComboBox cmbPageNum;
        private Button btnPage;

        /// <summary>
        /// 为实现 IPagination 分页接口的插件提供翻页操作的界面。
        /// </summary>
        public Pagination()
        {
            this.AutoSize = true;
            this.ColumnCount = 6;
            this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());

            this.labPageInfo = new Label();
            this.labPageInfo.AutoEllipsis = true;
            this.labPageInfo.Dock = DockStyle.Fill;
            this.labPageInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labPageInfo.Text = "第0页 共0页 计0条";
            this.Controls.Add(labPageInfo);

            this.btnBack = new Button();
            this.btnBack.Size = new System.Drawing.Size(70, 21);
            this.btnBack.Text = "上一页";
            this.btnBack.Enabled = false;
            this.btnBack.Click += new EventHandler(btnBack_Click);
            this.Controls.Add(btnBack);

            this.btnNext = new Button();
            this.btnNext.Size = new System.Drawing.Size(70, 21);
            this.btnNext.Text = "下一页";
            this.btnNext.Enabled = false;
            this.btnNext.Click += new EventHandler(btnNext_Click);
            this.Controls.Add(btnNext);

            Label labGoto = new Label();
            labGoto.AutoSize = true;
            labGoto.Dock = DockStyle.Fill;
            labGoto.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labGoto.Text = "转至第";
            this.Controls.Add(labGoto);

            this.cmbPageNum = new ComboBox();
            this.cmbPageNum.Size = new System.Drawing.Size(70, 21);
            this.cmbPageNum.Enabled = false;
            this.cmbPageNum.SelectedValueChanged += new EventHandler(cmbPageNum_SelectedValueChanged);
            this.Controls.Add(cmbPageNum);

            this.btnPage = new Button();
            this.btnPage.Size = new System.Drawing.Size(21, 21);
            this.btnPage.Text = "页";
            this.btnPage.Enabled = false;
            this.btnPage.Click += new EventHandler(btnPage_Click);
            this.Controls.Add(btnPage);
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            this.cmbPageNum.Text = (Function.ToInt(this.cmbPageNum.Text) - 1).ToString();
        }

        void btnNext_Click(object sender, EventArgs e)
        {
            this.cmbPageNum.Text = (Function.ToInt(this.cmbPageNum.Text) + 1).ToString();
        }

        void cmbPageNum_SelectedValueChanged(object sender, EventArgs e)
        {
            this.OnPaging();
        }

        void btnPage_Click(object sender, EventArgs e)
        {
            this.OnPaging();
        }

        private void OnPaging()
        {
            if (this.Paging != null)
            {
                this.Paging(Function.ToInt(this.cmbPageNum.Text));
            }
        }

        /// <summary>
        /// 用户在翻页控件上点击翻页按钮后产生的事件。
        /// </summary>
        public event PagingEventHandler Paging;

        /// <summary>
        /// 设置数据总数及页大小。
        /// </summary>
        /// <param name="recordsetCount">所显示数据在不分页的情况下总的记录数。</param>
        /// <param name="pageSize">每页显示多少条数据，“0”表示不分页。</param>
        public void SetPagination(int recordsetCount, int pageSize)
        {
            int intPageNum = 0;
            int intPageCount = 0;

            if (recordsetCount > 0)
            {
                intPageNum = Function.ToInt(this.cmbPageNum.Text);

                if (pageSize > 0)
                {
                    intPageCount = recordsetCount / pageSize;
                    if ((recordsetCount % pageSize) > 0)
                    {
                        intPageCount++;
                    }

                    if (intPageNum < 1)
                    {
                        intPageNum = 1;
                    }
                    if (intPageNum > intPageCount)
                    {
                        intPageNum = intPageCount;
                    }
                }
                else
                {
                    intPageNum = 1;
                    intPageCount = 1;
                }
            }

            this.labPageInfo.Text = "第" + intPageNum + "页 共" + intPageCount + "页 计" + recordsetCount + "条";
            this.cmbPageNum.Text = intPageNum.ToString();

            if (recordsetCount > 0 && pageSize > 0)
            {
                if (intPageNum <= 1)
                {
                    this.btnBack.Enabled = false;
                }
                else
                {
                    this.btnBack.Enabled = true;
                }

                if (intPageNum >= intPageCount)
                {
                    this.btnNext.Enabled = false;
                }
                else
                {
                    this.btnNext.Enabled = true;
                }

                this.cmbPageNum.Enabled = true;
                this.btnPage.Enabled = true;

                if (this.cmbPageNum.Items.Count != intPageCount)
                {
                    //该控件的下拉菜单内容肯定是从1开始递增的
                    //移除多余的选项
                    int intPageNumItemsCount = this.cmbPageNum.Items.Count;
                    for (int intIndex = intPageCount; intIndex < intPageNumItemsCount; intIndex++)
                    {
                        this.cmbPageNum.Items.RemoveAt(this.cmbPageNum.Items.Count - 1);
                    }

                    //增补不够的选项
                    for (int intIndex = this.cmbPageNum.Items.Count + 1; intIndex <= intPageCount; intIndex++)
                    {
                        this.cmbPageNum.Items.Add(intIndex.ToString());
                    }
                }
            }
            else
            {
                this.btnBack.Enabled = false;
                this.btnNext.Enabled = false;
                this.cmbPageNum.Enabled = false;
                this.btnPage.Enabled = false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 由开始日期和结束日期组成的一段时间。
    /// </summary>
    public class DateRangeValue
    {
        /// <summary>
        /// 由开始日期和结束日期组成的一段时间。
        /// </summary>
        /// <param name="date">将开始日期和结束日期均设置为该日期。</param>
        public DateRangeValue(DateTime date)
        {
            this.StartDate = date;
        }

        /// <summary>
        /// 由开始日期和结束日期组成的一段时间。
        /// </summary>
        /// <param name="startDate">开始日期。</param>
        /// <param name="endDate">结束日期。</param>
        public DateRangeValue(DateTime startDate, DateTime endDate)
        {
            this.m_StartDate = startDate;
            this.EndDate = endDate;
        }

        private DateTime m_StartDate;
        /// <summary>
        /// 获取或设置开始日期。
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return this.m_StartDate;
            }
            set
            {
                this.m_StartDate = value;

                if (this.m_EndDate != null)
                {
                    if (this.m_StartDate > this.m_EndDate)
                    {
                        this.m_EndDate = this.m_StartDate;
                    }
                }
                else
                {
                    this.m_EndDate = this.m_StartDate;
                }
            }
        }

        private DateTime m_EndDate;
        /// <summary>
        /// 获取或设置结束日期。
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                return this.m_EndDate;
            }
            set
            {
                this.m_EndDate = value;

                if (this.m_StartDate != null)
                {
                    if (this.m_StartDate > this.m_EndDate)
                    {
                        this.m_StartDate = this.m_EndDate;
                    }
                }
                else
                {
                    this.m_StartDate = this.m_EndDate;
                }
            }
        }

        /// <summary>
        /// 获取起始时间至结束时间的时间间隔。
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetTimeSpan()
        {
            return this.EndDate - this.StartDate;
        }

        /// <summary>
        /// 确定当前日期范围是否包含指定的某一天。
        /// </summary>
        /// <param name="date">日期。</param>
        /// <returns></returns>
        public bool ContainsDay(DateTime date)
        {
            if (this.StartDate <= date && date <= this.EndDate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取当前日期范围内所涵盖的月份的集合。
        /// </summary>
        /// <returns></returns>
        public IList<DateTime> GetMonths()
        {
            int intStartMonth = this.StartDate.Year * 12 + (this.StartDate.Month - 1);
            int intEndMonth = this.EndDate.Year * 12 + (this.EndDate.Month - 1);
            DateTime dtmStart = new DateTime(this.StartDate.Year, this.StartDate.Month, 1);

            List<DateTime> lstMonths = new List<DateTime>();

            for (int intIndex = 0; intIndex < (intEndMonth - intStartMonth + 1); intIndex++)
            {
                lstMonths.Add(dtmStart.AddMonths(intIndex));
            }

            return lstMonths;
        }

        /// <summary>
        /// 确定当前日期范围是否包含指定的某一月。
        /// </summary>
        /// <param name="date">日期（仅判断该日期的年、月部分）。</param>
        /// <returns></returns>
        public bool ContainsMonth(DateTime date)
        {
            DateTime dtmStart = new DateTime(this.StartDate.Year, this.StartDate.Month, 1);
            DateTime dtmEnd = new DateTime(this.EndDate.Year, this.EndDate.Month, 1);
            DateTime dtmDate = new DateTime(date.Year, date.Month, 1);

            if (dtmStart <= dtmDate && dtmDate <= dtmEnd)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取当前日期范围内所涵盖的年份的集合。
        /// </summary>
        /// <returns></returns>
        public IList<DateTime> GetYears()
        {
            DateTime dtmStart = new DateTime(this.StartDate.Year, 1, 1);

            List<DateTime> lstMonths = new List<DateTime>();

            for (int intIndex = 0; intIndex < (this.EndDate.Year - this.StartDate.Year + 1); intIndex++)
            {
                lstMonths.Add(dtmStart.AddYears(intIndex));
            }

            return lstMonths;
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.StartDate.Year == this.EndDate.Year && this.StartDate.Month == this.EndDate.Month && this.StartDate.Day == this.EndDate.Day)
            {
                return this.StartDate.ToString("yyyy-MM-dd");
            }
            else
            {
                return this.StartDate.ToString("yyyy-MM-dd") + " " + this.EndDate.ToString("yyyy-MM-dd");
            }
        }
    }
}

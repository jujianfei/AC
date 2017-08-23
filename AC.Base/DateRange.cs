using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base
{
    /// <summary>
    /// 获取数据时用于指定日期范围。
    /// </summary>
    public class DateRange : List<DateRangeValue>
    {
        /// <summary>
        /// 初始化获取数据时的日期范围。
        /// </summary>
        public DateRange()
        {
            this.ResetUniqueId();
        }

        /// <summary>
        /// 初始化获取数据时的日期范围，并默认添加一天的日期数据。
        /// </summary>
        /// <param name="date"></param>
        public DateRange(DateTime date)
            : this()
        {
            this.Add(date);
        }

        /// <summary>
        /// 初始化获取数据时的日期范围，并默认添加一段范围的日期数据。
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public DateRange(DateTime startDate, DateTime endDate)
            : this()
        {
            this.Add(startDate, endDate);
        }

        private int m_UniqueId;
        /// <summary>
        ///  该日期范围随机代码。
        /// </summary>
        public int UniqueId
        {
            get
            {
                return this.m_UniqueId;
            }
        }

        private void ResetUniqueId()
        {
            this.m_UniqueId = Function.GetRnd();
        }

        /// <summary>
        /// 添加一个新的日期作为数据筛选的日期范围。
        /// </summary>
        /// <param name="date"></param>
        public void Add(DateTime date)
        {
            base.Add(new DateRangeValue(date));
        }

        /// <summary>
        /// 添加一个新的日期作为数据筛选的日期范围。
        /// </summary>
        /// <param name="dateNum">8位整型日期。yyyyMMdd</param>
        public void Add(int dateNum)
        {
            this.Add(Function.ToDateTime(dateNum));
        }

        /// <summary>
        /// 添加一个新的时段作为数据筛选的日期范围。
        /// </summary>
        /// <param name="dateNum1">起始日期，8位整型日期(yyyyMMdd)</param>
        /// <param name="dateNum2">结束日期，8位整型日期(yyyyMMdd)</param>
        public void Add(int dateNum1, int dateNum2)
        {
            this.Add(Function.ToDateTime(dateNum1), Function.ToDateTime(dateNum2));
        }

        /// <summary>
        /// 添加一个新的时段作为数据筛选的日期范围。
        /// </summary>
        /// <param name="dateNum1">起始日期，8位整型日期(yyyyMMdd)</param>
        /// <param name="timeNum1">起始时间，6位整型时间(hhmmss)</param>
        /// <param name="dateNum2">结束日期，8位整型日期(yyyyMMdd)</param>
        /// <param name="timeNum2">结束时间，6位整型时间(hhmmss)</param>
        public void Add(int dateNum1, int timeNum1, int dateNum2, int timeNum2)
        {
            this.Add(Function.ToDateTime(dateNum1, timeNum1), Function.ToDateTime(dateNum2, timeNum2));
        }

        /// <summary>
        /// 添加一个新的日期段作为数据筛选的日期范围。
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public void Add(DateTime startDate, DateTime endDate)
        {
            base.Add(new DateRangeValue(startDate, endDate));
        }

        /// <summary>
        /// 确定当前日期范围是否包含指定的某一天。
        /// </summary>
        /// <param name="date">日期。</param>
        /// <returns></returns>
        public bool ContainsDay(DateTime date)
        {
            foreach (DateRangeValue value in this)
            {
                if (value.ContainsDay(date))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 确定当前日期范围是否包含指定的一段时间。
        /// </summary>
        /// <param name="startDate">开始日期。</param>
        /// <param name="endDate">结束日期。</param>
        /// <returns></returns>
        public bool ContainsDay(DateTime startDate, DateTime endDate)
        {
            TimeSpan tsDiffDay = endDate - startDate;
            for (int intDays = 0; intDays <= tsDiffDay.Days; intDays++)
            {
                DateTime date = startDate.AddDays(intDays);
                bool bolContains = false;

                foreach (DateRangeValue value in this)
                {
                    if (value.ContainsDay(date))
                    {
                        bolContains = true;
                        break;
                    }
                }

                if (bolContains == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取当前日期范围内所有日的集合。
        /// </summary>
        /// <returns>按日期大小排序的日期集合。</returns>
        public IList<DateTime> GetDays()
        {
            List<DateTime> lst = new List<DateTime>();

            foreach (DateRangeValue value in this)
            {
                TimeSpan tsDiffDay = value.EndDate - value.StartDate;
                DateTime dtmStart = new DateTime(value.StartDate.Year, value.StartDate.Month, value.StartDate.Day);
                for (int intDays = 0; intDays <= tsDiffDay.Days; intDays++)
                {
                    DateTime dtm = dtmStart.AddDays(intDays);
                    if (lst.Contains(dtm) == false)
                    {
                        lst.Add(dtm);
                    }
                }
            }

            lst.Sort();
            return lst;
        }

        /// <summary>
        /// 获取当前日期范围内所涵盖的月份的集合。
        /// </summary>
        /// <returns></returns>
        public IList<MonthlyForDayRange> GetMonthRanges()
        {
            SortedList<int, MonthlyForDayRange> slRanges = new SortedList<int, MonthlyForDayRange>();

            foreach (DateRangeValue value in this)
            {
                TimeSpan tsDiffDay = value.EndDate - value.StartDate;
                for (int intDays = 0; intDays <= tsDiffDay.Days; intDays++)
                {
                    DateTime dtm = value.StartDate.AddDays(intDays);
                    int intMonth = dtm.Year * 100 + dtm.Month;

                    if (slRanges.ContainsKey(intMonth) == false)
                    {
                        slRanges.Add(intMonth, new MonthlyForDayRange(dtm.Year, dtm.Month));
                    }

                    slRanges[intMonth].DayCode += 1 << (dtm.Day - 1);
                }
            }

            return slRanges.Values;
        }

        /// <summary>
        /// 获取当前日期范围内所涵盖的月份的集合，并且该集合内的时间精确到毫秒。
        /// </summary>
        /// <returns></returns>
        public IList<MonthlyForTickRange> GetMonthTickRanges()
        {
            SortedList<int, MonthlyForTickRange> slRanges = new SortedList<int, MonthlyForTickRange>();

            foreach (DateRangeValue value in this)
            {
                int intStartMonth = value.StartDate.Year * 12 + value.StartDate.Month - 1;
                int intEndMonth = value.EndDate.Year * 12 + value.EndDate.Month - 1;
                DateTime dtmDateRangeEnd = new DateTime((value.EndDate.Ticks / 10000000) * 10000000 + 9999999);

                for (int intMonths = 0; intMonths <= (intEndMonth - intStartMonth); intMonths++)
                {
                    int intYear = (intStartMonth + intMonths) / 12;
                    int intMonth = (intStartMonth + intMonths) % 12 + 1;
                    int intYearMonth = intYear * 100 + intMonth;

                    if (slRanges.ContainsKey(intYearMonth) == false)
                    {
                        slRanges.Add(intYearMonth, new MonthlyForTickRange(intYear, intMonth));
                    }

                    DateTime dtmStart;
                    if (intMonth == value.StartDate.Month && intYear == value.StartDate.Year)
                    {
                        dtmStart = value.StartDate;
                    }
                    else
                    {
                        dtmStart = new DateTime(intYear, intMonth, 1);
                    }

                    DateTime dtmEnd;
                    if (intMonth == value.EndDate.Month && intYear == value.EndDate.Year)
                    {
                        dtmEnd = dtmDateRangeEnd;
                    }
                    else
                    {
                        dtmEnd = new DateTime(new DateTime(intYear, intMonth, DateTime.DaysInMonth(intYear, intMonth), 23, 59, 59).Ticks + 9999999);
                    }

                    slRanges[intYearMonth].lstStartTime.Add(dtmStart);
                    slRanges[intYearMonth].lstEndTime.Add(dtmEnd);
                }
            }

            foreach (MonthlyForTickRange tickRange in slRanges.Values)
            {
                tickRange.Join();
            }
            return slRanges.Values;
        }

        /// <summary>
        /// 获取当前日期范围内所涵盖的年份的集合，并且该集合内的时间精确到毫秒。
        /// </summary>
        /// <returns></returns>
        public IList<YearlyForTickRange> GetYearTickRanges()
        {
            SortedList<int, YearlyForTickRange> slRanges = new SortedList<int, YearlyForTickRange>();

            foreach (DateRangeValue value in this)
            {
                DateTime dtmDateRangeEnd = new DateTime((value.EndDate.Ticks / 10000000) * 10000000 + 9999999);

                for (int intYears = 0; intYears <= (value.EndDate.Year - value.StartDate.Year); intYears++)
                {
                    int intYear = value.StartDate.Year + intYears;

                    if (slRanges.ContainsKey(intYear) == false)
                    {
                        slRanges.Add(intYear, new YearlyForTickRange(intYear));
                    }

                    DateTime dtmStart;
                    if (intYears == 0)
                    {
                        dtmStart = value.StartDate;
                    }
                    else
                    {
                        dtmStart = new DateTime(intYear, 1, 1);
                    }

                    DateTime dtmEnd;
                    if (intYears == (value.EndDate.Year - value.StartDate.Year))
                    {
                        dtmEnd = dtmDateRangeEnd;
                    }
                    else
                    {
                        dtmEnd = new DateTime(new DateTime(intYear, 12, 31, 23, 59, 59).Ticks + 9999999);
                    }

                    slRanges[intYear].lstStartTime.Add(dtmStart);
                    slRanges[intYear].lstEndTime.Add(dtmEnd);
                }
            }

            foreach (YearlyForTickRange tickRange in slRanges.Values)
            {
                tickRange.Join();
            }
            return slRanges.Values;
        }

        /// <summary>
        /// 确定当前日期范围是否包含指定的某一月。
        /// </summary>
        /// <param name="date">日期（仅判断该日期的年、月部分）。</param>
        /// <returns></returns>
        public bool ContainsMonth(DateTime date)
        {
            foreach (DateRangeValue value in this)
            {
                if (value.ContainsMonth(date))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 确定当前月份范围是否包含指定的一段时间的月份。
        /// </summary>
        /// <param name="startDate">开始月份（仅使用年、月属性）。</param>
        /// <param name="endDate">结束月份（仅使用年、月属性）。</param>
        /// <returns></returns>
        public bool ContainsMonth(DateTime startDate, DateTime endDate)
        {
            if (startDate.Day > 1)
            {
                startDate = new DateTime(startDate.Year, startDate.Month, 1);
            }
            int intDiffMonth = (endDate.Year * 12 + endDate.Month - 1) - (startDate.Year * 12 + startDate.Month - 1);
            for (int intMonths = 0; intMonths <= intDiffMonth; intMonths++)
            {
                DateTime date = startDate.AddMonths(intMonths);
                bool bolContains = false;

                foreach (DateRangeValue value in this)
                {
                    if (value.ContainsMonth(date))
                    {
                        bolContains = true;
                        break;
                    }
                }

                if (bolContains == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 获取当前日期范围内所有月的集合。
        /// </summary>
        /// <returns>按日期大小排序的日期集合。</returns>
        public IList<DateTime> GetMonths()
        {
            List<DateTime> lst = new List<DateTime>();
            foreach (DateRangeValue value in this)
            {
                int intDiffMonth = (value.EndDate.Year * 12 + value.EndDate.Month - 1) - (value.StartDate.Year * 12 + value.StartDate.Month - 1);
                DateTime dtmStart = new DateTime(value.StartDate.Year, value.StartDate.Month, 1);
                for (int intMonths = 0; intMonths <= intDiffMonth; intMonths++)
                {
                    DateTime dtm = dtmStart.AddMonths(intMonths);
                    lst.Add(dtm);
                }
            }

            lst.Sort();
            return lst;
        }

        /// <summary>
        /// 获取当前日期范围内所涵盖的年份的集合。
        /// </summary>
        /// <returns></returns>
        public IList<YearlyForMonthRange> GetYearRanges()
        {
            SortedList<int, YearlyForMonthRange> slRanges = new SortedList<int, YearlyForMonthRange>();

            foreach (DateRangeValue value in this)
            {
                int intDiffMonth = (value.EndDate.Year * 12 + value.EndDate.Month - 1) - (value.StartDate.Year * 12 + value.StartDate.Month - 1);
                for (int intMonths = 0; intMonths <= intDiffMonth; intMonths++)
                {
                    DateTime dtm = value.StartDate.AddMonths(intMonths);

                    if (slRanges.ContainsKey(dtm.Year) == false)
                    {
                        slRanges.Add(dtm.Year, new YearlyForMonthRange(dtm.Year));
                    }

                    slRanges[dtm.Year].MonthCode += 1 << (dtm.Month - 1);
                }
            }

            return slRanges.Values;
        }

        /// <summary>
        /// 针对每月产生一张数据表并且使用 yyyyMMdd 日期格式的日期范围。
        /// </summary>
        public class MonthlyForDayRange
        {
            internal MonthlyForDayRange(int year, int month)
            {
                this.Date = new DateTime(year, month, 1);
            }

            /// <summary>
            /// 该日期范围所指的月份。该值的 Day 属性始终为 1。
            /// </summary>
            public DateTime Date { get; private set; }

            internal long DayCode;      //一个月中31天的位标志。

            /// <summary>
            /// 获取该日期范围的 SQL 查询条件。
            /// </summary>
            /// <param name="columnName">日期字段名称。</param>
            /// <returns></returns>
            public string GetSqlWhere(string columnName)
            {
                List<int> lst2 = new List<int>();   //1天或连续2天的日期
                SortedDictionary<int, int> sd3 = new SortedDictionary<int, int>();  //连续3天及连续3天以上的日期

                int intLastDay = 0;
                int intLastNum = 0;
                for (int intDayIndex = 0; intDayIndex < 32; intDayIndex++)
                {
                    if ((this.DayCode & (1 << intDayIndex)) == (1 << intDayIndex))
                    {
                        if (intLastDay == 0)
                        {
                            intLastDay = intDayIndex + 1;
                        }
                        intLastNum++;
                    }
                    else
                    {
                        if (intLastNum != 0)
                        {
                            if (intLastNum == 1)
                            {
                                lst2.Add(intLastDay);
                            }
                            else if (intLastNum == 2)
                            {
                                lst2.Add(intLastDay);
                                lst2.Add(intLastDay + 1);
                            }
                            else
                            {
                                sd3.Add(intLastDay, intLastDay + intLastNum - 1);
                            }
                            intLastNum = 0;
                            intLastDay = 0;
                        }
                    }
                }

                int intOrNum = 0;           //返回的SQL条件中 OR 条件的数量
                if (lst2.Count > 0)
                {
                    intOrNum++;
                }
                intOrNum += sd3.Count;

                string strSql = "";
                int intDateNum = this.Date.Year * 10000 + this.Date.Month * 100;

                if (lst2.Count == 1)
                {
                    strSql += " OR " + columnName + "=" + (intDateNum + lst2[0]);
                }
                else if (lst2.Count > 1)
                {
                    string strDay = "";
                    foreach (int intDay in lst2)
                    {
                        strDay += "," + (intDateNum + intDay);
                    }
                    strSql += " OR " + columnName + " IN (" + strDay.Substring(1) + ")";
                }

                foreach (System.Collections.Generic.KeyValuePair<int, int> kvp in sd3)
                {
                    if (intOrNum > 1)
                    {
                        strSql += " OR (" + columnName + ">=" + (intDateNum + kvp.Key) + " AND " + columnName + "<=" + (intDateNum + kvp.Value) + ")";
                    }
                    else
                    {
                        strSql += " OR " + columnName + ">=" + (intDateNum + kvp.Key) + " AND " + columnName + "<=" + (intDateNum + kvp.Value);
                    }
                }

                if (strSql.Length > 0)
                {
                    strSql = strSql.Substring(4);
                }
                if (sd3.Count > 0)
                {
                    strSql = "(" + strSql + ")";
                }

                return strSql;
            }

            /// <summary>
            /// 获取当前对象的字符串表示形式。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string strDay = "";

                for (int intDayIndex = 0; intDayIndex < 31; intDayIndex++)
                {
                    if ((this.DayCode & (1 << intDayIndex)) == (1 << intDayIndex))
                    {
                        strDay += "," + (intDayIndex + 1);
                    }
                }

                if (strDay.Length > 0)
                {
                    strDay = strDay.Substring(1);
                }
                return strDay;
            }
        }

        /// <summary>
        /// 针对每年产生一张数据表并且使用 yyyyMM00 日期格式的日期范围。
        /// </summary>
        public class YearlyForMonthRange
        {
            internal YearlyForMonthRange(int year)
            {
                this.Date = new DateTime(year, 1, 1);
            }

            /// <summary>
            /// 该日期范围所指的年份。该值的 Month 及 Day 属性始终为 1。
            /// </summary>
            public DateTime Date { get; private set; }

            internal int MonthCode;        //一年中12个月的位标志。

            /// <summary>
            /// 获取该月份范围的 SQL 查询条件。
            /// </summary>
            /// <param name="columnName">日期字段名称。该字段内储存的日期应该是 YYYYMM00 格式。</param>
            /// <returns></returns>
            public string GetSqlWhere(string columnName)
            {
                List<int> lst2 = new List<int>();   //1月或连续2月的月份
                SortedDictionary<int, int> sd3 = new SortedDictionary<int, int>();  //连续3月及连续3月以上的月份

                int intLastMonth = 0;
                int intLastNum = 0;
                for (int intMonthIndex = 0; intMonthIndex < 13; intMonthIndex++)
                {
                    if ((this.MonthCode & (1 << intMonthIndex)) == (1 << intMonthIndex))
                    {
                        if (intLastMonth == 0)
                        {
                            intLastMonth = intMonthIndex + 1;
                        }
                        intLastNum++;
                    }
                    else
                    {
                        if (intLastNum != 0)
                        {
                            if (intLastNum == 1)
                            {
                                lst2.Add(intLastMonth);
                            }
                            else if (intLastNum == 2)
                            {
                                lst2.Add(intLastMonth);
                                lst2.Add(intLastMonth + 1);
                            }
                            else
                            {
                                sd3.Add(intLastMonth, intLastMonth + intLastNum - 1);
                            }
                            intLastNum = 0;
                            intLastMonth = 0;
                        }
                    }
                }

                int intOrNum = 0;           //返回的SQL条件中 OR 条件的数量
                if (lst2.Count > 0)
                {
                    intOrNum++;
                }
                intOrNum += sd3.Count;

                string strSql = "";
                int intDateNum = this.Date.Year * 10000;

                if (lst2.Count == 1)
                {
                    strSql += " OR " + columnName + "=" + (intDateNum + lst2[0] * 100);
                }
                else if (lst2.Count > 1)
                {
                    string strMonth = "";
                    foreach (int intMonth in lst2)
                    {
                        strMonth += "," + (intDateNum + intMonth * 100);
                    }
                    strSql += " OR " + columnName + " IN (" + strMonth.Substring(1) + ")";
                }

                foreach (System.Collections.Generic.KeyValuePair<int, int> kvp in sd3)
                {
                    if (intOrNum > 1)
                    {
                        strSql += " OR (" + columnName + ">=" + (intDateNum + kvp.Key * 100) + " AND " + columnName + "<=" + (intDateNum + kvp.Value * 100) + ")";
                    }
                    else
                    {
                        strSql += " OR " + columnName + ">=" + (intDateNum + kvp.Key * 100) + " AND " + columnName + "<=" + (intDateNum + kvp.Value * 100);
                    }
                }

                if (strSql.Length > 0)
                {
                    strSql = strSql.Substring(4);
                }
                if (sd3.Count > 0)
                {
                    strSql = "(" + strSql + ")";
                }

                return strSql;
            }

            /// <summary>
            /// 获取当前对象的字符串表示形式。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string strMonth = "";

                for (int intMonthIndex = 0; intMonthIndex < 12; intMonthIndex++)
                {
                    if ((this.MonthCode & (1 << intMonthIndex)) == (1 << intMonthIndex))
                    {
                        strMonth += "," + (intMonthIndex + 1);
                    }
                }

                if (strMonth.Length > 0)
                {
                    strMonth = strMonth.Substring(1);
                }
                return strMonth;
            }
        }

        /// <summary>
        /// 针对每月产生一张数据表并且使用 DateTime.Now.Ticks 计时周期格式的日期范围。
        /// </summary>
        public class MonthlyForTickRange
        {
            internal MonthlyForTickRange(int year, int month)
            {
                this.Date = new DateTime(year, month, 1);
            }

            internal List<DateTime> lstStartTime = new List<DateTime>();
            internal List<DateTime> lstEndTime = new List<DateTime>();

            /// <summary>
            /// 该时间范围所指的月份。该值的 Day 属性始终为 1。
            /// </summary>
            public DateTime Date { get; private set; }

            internal void Join()
            {
                if (lstStartTime.Count > 1)
                {
                    for (int intX = 0; intX < lstStartTime.Count; intX++)
                    {
                        bool bolAgain = false;
                        for (int intY = 0; intY < lstStartTime.Count; intY++)
                        {
                            if (intX != intY)
                            {
                                if (lstStartTime[intX] <= lstStartTime[intY] && lstEndTime[intX] >= lstEndTime[intY])
                                {
                                    lstStartTime.RemoveAt(intY);
                                    lstEndTime.RemoveAt(intY);
                                    bolAgain = true;
                                    break;
                                }
                                else if (lstStartTime[intX] <= lstStartTime[intY] && lstStartTime[intY] <= lstEndTime[intX])
                                {
                                    lstEndTime[intX] = lstEndTime[intY];
                                    lstStartTime.RemoveAt(intY);
                                    lstEndTime.RemoveAt(intY);
                                    bolAgain = true;
                                    break;
                                }
                                else if (lstStartTime[intX] <= lstEndTime[intY] && lstEndTime[intY] <= lstEndTime[intX])
                                {
                                    lstStartTime[intX] = lstStartTime[intY];
                                    lstStartTime.RemoveAt(intY);
                                    lstEndTime.RemoveAt(intY);
                                    bolAgain = true;
                                    break;
                                }
                            }
                        }
                        if (bolAgain)
                        {
                            this.Join();
                            break;
                        }
                    }
                }
            }

            /// <summary>
            /// 获取该日期范围的 SQL 查询条件。
            /// </summary>
            /// <param name="columnName">日期字段名称。</param>
            /// <returns></returns>
            public string GetSqlWhere(string columnName)
            {
                if (this.lstStartTime.Count == 1)
                {
                    return columnName + ">=" + this.lstStartTime[0].Ticks + " AND " + columnName + "<=" + this.lstEndTime[0].Ticks;
                }
                else
                {
                    string strSql = "";

                    for (int intIndex = 0; intIndex < this.lstStartTime.Count; intIndex++)
                    {
                        strSql += " OR (" + columnName + ">=" + this.lstStartTime[intIndex].Ticks + " AND " + columnName + "<=" + this.lstEndTime[intIndex].Ticks + ")";
                    }

                    return strSql.Substring(4);
                }
            }

            /// <summary>
            /// 获取指定的时间是否在当前日期范围内。
            /// </summary>
            /// <param name="ticks">日期和时间的计时周期数。</param>
            /// <returns></returns>
            public bool Contains(long ticks)
            {
                for (int intIndex = 0; intIndex < this.lstStartTime.Count; intIndex++)
                {
                    if (this.lstStartTime[intIndex].Ticks <= ticks && ticks <= this.lstEndTime[intIndex].Ticks)
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// 获取当前对象的字符串表示形式。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string strTime = "";

                for (int intIndex = 0; intIndex < this.lstStartTime.Count; intIndex++)
                {
                    strTime += "、" + this.lstStartTime[intIndex].ToString("dd日HH:mm:ss") + "至" + this.lstEndTime[intIndex].ToString("dd日HH:mm:ss");
                }

                if (strTime.Length > 0)
                {
                    strTime = this.Date.ToString("yyyy年MM月") + "(" + strTime.Substring(1) + ")";
                }
                return strTime;
            }
        }

        /// <summary>
        /// 针对每年产生一张数据表并且使用 DateTime.Now.Ticks 计时周期格式的日期范围。
        /// </summary>
        public class YearlyForTickRange
        {
            internal YearlyForTickRange(int year)
            {
                this.Date = new DateTime(year, 1, 1);
            }

            internal List<DateTime> lstStartTime = new List<DateTime>();
            internal List<DateTime> lstEndTime = new List<DateTime>();

            /// <summary>
            /// 该时间范围所指的年份。该值的 Month、Day 属性始终为 1。
            /// </summary>
            public DateTime Date { get; private set; }

            internal void Join()
            {
                if (lstStartTime.Count > 1)
                {
                    for (int intX = 0; intX < lstStartTime.Count; intX++)
                    {
                        bool bolAgain = false;
                        for (int intY = 0; intY < lstStartTime.Count; intY++)
                        {
                            if (intX != intY)
                            {
                                if (lstStartTime[intX] <= lstStartTime[intY] && lstEndTime[intX] >= lstEndTime[intY])
                                {
                                    lstStartTime.RemoveAt(intY);
                                    lstEndTime.RemoveAt(intY);
                                    bolAgain = true;
                                    break;
                                }
                                else if (lstStartTime[intX] <= lstStartTime[intY] && lstStartTime[intY] <= lstEndTime[intX])
                                {
                                    lstEndTime[intX] = lstEndTime[intY];
                                    lstStartTime.RemoveAt(intY);
                                    lstEndTime.RemoveAt(intY);
                                    bolAgain = true;
                                    break;
                                }
                                else if (lstStartTime[intX] <= lstEndTime[intY] && lstEndTime[intY] <= lstEndTime[intX])
                                {
                                    lstStartTime[intX] = lstStartTime[intY];
                                    lstStartTime.RemoveAt(intY);
                                    lstEndTime.RemoveAt(intY);
                                    bolAgain = true;
                                    break;
                                }
                            }
                        }
                        if (bolAgain)
                        {
                            this.Join();
                            break;
                        }
                    }
                }
            }

            /// <summary>
            /// 获取该日期范围的 SQL 查询条件。
            /// </summary>
            /// <param name="columnName">日期字段名称。</param>
            /// <returns></returns>
            public string GetSqlWhere(string columnName)
            {
                if (this.lstStartTime.Count == 1)
                {
                    return columnName + ">=" + this.lstStartTime[0].Ticks + " AND " + columnName + "<=" + this.lstEndTime[0].Ticks;
                }
                else
                {
                    string strSql = "";

                    for (int intIndex = 0; intIndex < this.lstStartTime.Count; intIndex++)
                    {
                        strSql += " OR (" + columnName + ">=" + this.lstStartTime[intIndex].Ticks + " AND " + columnName + "<=" + this.lstEndTime[intIndex].Ticks + ")";
                    }

                    return strSql.Substring(4);
                }
            }

            /// <summary>
            /// 获取指定的时间是否在当前日期范围内。
            /// </summary>
            /// <param name="ticks">日期和时间的计时周期数。</param>
            /// <returns></returns>
            public bool Contains(long ticks)
            {
                for (int intIndex = 0; intIndex < this.lstStartTime.Count; intIndex++)
                {
                    if (this.lstStartTime[intIndex].Ticks <= ticks && ticks <= this.lstEndTime[intIndex].Ticks)
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// 获取当前对象的字符串表示形式。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                string strTime = "";

                for (int intIndex = 0; intIndex < this.lstStartTime.Count; intIndex++)
                {
                    strTime += "、" + this.lstStartTime[intIndex].ToString("MM-dd HH:mm:ss") + "至" + this.lstEndTime[intIndex].ToString("MM-dd HH:mm:ss");
                }

                if (strTime.Length > 0)
                {
                    strTime = this.Date.ToString("yyyy年") + "(" + strTime.Substring(1) + ")";
                }
                return strTime;
            }
        }
    }
}

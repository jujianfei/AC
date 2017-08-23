using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 曲线点数。
    /// </summary>
    public enum CurvePointOptions
    {
        /// <summary>
        /// 4小时周期，一天6个数据点。
        /// </summary>
        Point6 = 1 << 0,

        /// <summary>
        /// 2小时周期，一天12个数据点。
        /// </summary>
        Point12 = 1 << 1,

        /// <summary>
        /// 1小时周期，一天24个数据点。
        /// </summary>
        Point24 = 1 << 2,

        /// <summary>
        /// 30分钟周期，一天48个数据点。
        /// </summary>
        Point48 = 1 << 3,

        /// <summary>
        /// 15分钟周期，一天96个数据点。
        /// </summary>
        Point96 = 1 << 4,

        /// <summary>
        /// 10分钟周期，一天144个数据点。
        /// </summary>
        Point144 = 1 << 5,

        /// <summary>
        /// 5分钟周期，一天288个数据点。
        /// </summary>
        Point288 = 1 << 6,

        /// <summary>
        /// 1分钟周期，一天1440个数据点。
        /// </summary>
        Point1440 = 1 << 7,

        /// <summary>
        /// 30秒钟周期，一天2880个数据点。
        /// </summary>
        Point2880 = 1 << 8,

        /// <summary>
        /// 15秒钟周期，一天5760个数据点。
        /// </summary>
        Point5760 = 1 << 9,

        /// <summary>
        /// 5秒周期，一天17280个数据点。
        /// </summary>
        Point17280 = 1 << 10,

        /// <summary>
        /// 1秒周期，一天86400个数据点。
        /// </summary>
        Point86400 = 1 << 11,
    }

    /// <summary>
    /// 曲线点数扩展。
    /// </summary>
    public static class CurvePointExtensions
    {
        /// <summary>
        /// 获取曲线点数的文字说明。
        /// </summary>
        /// <param name="curvePoint">曲线点数。</param>
        /// <returns>曲线点数的文字说明</returns>
        public static string GetDescription(this CurvePointOptions curvePoint)
        {
            switch (curvePoint)
            {
                case CurvePointOptions.Point6:
                    return "4小时";

                case CurvePointOptions.Point12:
                    return "2小时";

                case CurvePointOptions.Point24:
                    return "1小时";

                case CurvePointOptions.Point48:
                    return "30分钟";

                case CurvePointOptions.Point96:
                    return "15分钟";

                case CurvePointOptions.Point144:
                    return "10分钟";

                case CurvePointOptions.Point288:
                    return "5分钟";

                case CurvePointOptions.Point1440:
                    return "1分钟";

                case CurvePointOptions.Point2880:
                    return "30秒钟";

                case CurvePointOptions.Point5760:
                    return "15秒钟";

                case CurvePointOptions.Point17280:
                    return "5秒钟";

                case CurvePointOptions.Point86400:
                    return "1秒钟";

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }

        /// <summary>
        /// 获取曲线的总点数。
        /// </summary>
        /// <param name="curvePoint">曲线点数。</param>
        /// <returns>曲线总点数。</returns>
        public static int GetPointCount(this CurvePointOptions curvePoint)
        {
            switch (curvePoint)
            {
                case CurvePointOptions.Point6:
                    return 6;

                case CurvePointOptions.Point12:
                    return 12;

                case CurvePointOptions.Point24:
                    return 24;

                case CurvePointOptions.Point48:
                    return 48;

                case CurvePointOptions.Point96:
                    return 96;

                case CurvePointOptions.Point144:
                    return 144;

                case CurvePointOptions.Point288:
                    return 288;

                case CurvePointOptions.Point1440:
                    return 1440;

                case CurvePointOptions.Point2880:
                    return 2880;

                case CurvePointOptions.Point5760:
                    return 5760;

                case CurvePointOptions.Point17280:
                    return 17280;

                case CurvePointOptions.Point86400:
                    return 86400;

                default:
                    throw new NotImplementedException("尚未实现该枚举。");
            }
        }

        /// <summary>
        /// 获取该曲线2个点之间间隔的时间（秒）。
        /// </summary>
        /// <param name="curvePoint">曲线点数。</param>
        /// <returns></returns>
        public static int GetTimeSpan(this CurvePointOptions curvePoint)
        {
            return 86400 / curvePoint.GetPointCount();
        }

        /// <summary>
        /// 获取指定的2个时间之间间隔的点数。
        /// </summary>
        /// <param name="curvePoint"></param>
        /// <param name="startTimeNum">起始时间。</param>
        /// <param name="endTimeNum">结束时间。</param>
        /// <returns>如果起始时间等于结束时间，则返回 0。</returns>
        public static int GetTimeSpanPoint(this CurvePointOptions curvePoint, int startTimeNum, int endTimeNum)
        {
            return curvePoint.GetPointIndex(endTimeNum) - curvePoint.GetPointIndex(startTimeNum);
        }

        /// <summary>
        /// 根据一个时间值判断该曲线最小的密度点是何种类型。例如 090000 返回 Point24；092500 返回 Point288；092530 返回 Point2880。
        /// </summary>
        /// <param name="timeNum"></param>
        /// <returns></returns>
        public static CurvePointOptions GetCurvePoint(int timeNum)
        {
            foreach (int intCurvePoint in Enum.GetValues(typeof(CurvePointOptions)))
            {
                CurvePointOptions _CurvePoint = (CurvePointOptions)intCurvePoint;
                int intTimeNum = FormatTimeNum(_CurvePoint, timeNum);
                if (intTimeNum == timeNum)
                {
                    return _CurvePoint;
                }
            }

            return CurvePointOptions.Point24;
        }

        /// <summary>
        /// 根据两个时间之间的点数计算该曲线的类型。
        /// </summary>
        /// <param name="startTimeNum">起始时间。</param>
        /// <param name="endTimeNum">结束时间。</param>
        /// <param name="pointCount">两个时间之间的点数，该点数至少需要 ≥2</param>
        /// <returns></returns>
        public static CurvePointOptions GetCurvePoint(int startTimeNum, int endTimeNum, int pointCount)
        {
            if (pointCount < 2)
            {
                throw new Exception("两个时间之间的点数至少需要 ≥2");
            }
            if (startTimeNum >= endTimeNum)
            {
                throw new Exception("开始时间不能大于等于结束时间。");
            }

            int intStartSecond = (startTimeNum / 10000) * 3600 + ((startTimeNum / 100) % 100) * 60 + (startTimeNum % 100);
            int intEndSecond = (endTimeNum / 10000) * 3600 + ((endTimeNum / 100) % 100) * 60 + (endTimeNum % 100);
            int intPointSecond = (intEndSecond - intStartSecond) / (pointCount - 1);    //每两个点之间的间隔（秒）

            foreach (int intEnumValue in Enum.GetValues(typeof(CurvePointOptions)))
            {
                CurvePointOptions _CurvePoint = (CurvePointOptions)intEnumValue;
                if (_CurvePoint.GetTimeSpan() == intPointSecond)
                {
                    return _CurvePoint;
                }
            }

            throw new Exception("未发现 " + ((startTimeNum / 10000) + ":" + ((startTimeNum / 100) % 100) + ":" + (startTimeNum % 100)) + " 至 " + ((endTimeNum / 10000) + ":" + ((endTimeNum / 100) % 100) + ":" + (endTimeNum % 100)) + " 之间有 " + pointCount + " 个点的曲线类型。");
        }

        /// <summary>
        /// 获取曲线指定时间段的点数。
        /// </summary>
        /// <param name="curvePoint">曲线点数。</param>
        /// <param name="startTimeNum">起始时间。</param>
        /// <param name="endTimeNum">结束时间。</param>
        /// <returns>两个时间之间的曲线点数，如果起始时间等于结束时间则返回1。</returns>
        public static int GetPointCount(this CurvePointOptions curvePoint, int startTimeNum, int endTimeNum)
        {
            if (startTimeNum > endTimeNum)
            {
                throw new Exception("开始时间不能大于结束时间。");
            }
            else
            {
                return curvePoint.GetPointIndex(endTimeNum) - curvePoint.GetPointIndex(startTimeNum) + 1;
            }
        }

        /// <summary>
        /// 获取指定索引位置的时间。
        /// </summary>
        /// <param name="curvePoint"></param>
        /// <param name="pointIndex">从 0 开始的曲线索引位置。</param>
        /// <returns>从 0:00:00 开始的格式为 hhmmss 6位整型时间。</returns>
        public static int GetTimeNum(this CurvePointOptions curvePoint, int pointIndex)
        {
            int intPointSecond = CurvePointOptions.Point86400.GetPointCount() / curvePoint.GetPointCount(); //曲线上的一个点，对应多少长度的时间(秒)。
            int intSecond = intPointSecond * pointIndex;    //该点所经历过的时间(秒)

            return (intSecond / 60 / 60) * 10000 + ((intSecond / 60) % 60) * 100 + (intSecond % 60);
        }

        /// <summary>
        /// 获取指定时间后移若干个点的时间。
        /// </summary>
        /// <param name="curvePoint"></param>
        /// <param name="timeNum">整型时间（hhmmss）。</param>
        /// <param name="addPoint">后移的点数。</param>
        /// <returns>整型时间（hhmmss）。</returns>
        public static int GetTimeNum(this CurvePointOptions curvePoint, int timeNum, int addPoint)
        {
            return GetTimeNum(curvePoint, GetPointIndex(curvePoint, timeNum) + addPoint);
        }

        /// <summary>
        /// 将时间转为对应数据点的准确时间，返回的时间年月日部分与参数 dateTime 一致。
        /// </summary>
        /// <param name="curvePoint"></param>
        /// <param name="dateTime">日期时间</param>
        /// <returns>返回的时间值一定小于或等于传入的时间值。</returns>
        public static DateTime FormatDateTime(this CurvePointOptions curvePoint, DateTime dateTime)
        {
            if (curvePoint < CurvePointOptions.Point48)
            {
                int intPointHour = 24 / curvePoint.GetPointCount();     //曲线上的一个点，对应多少长度的时间(小时)。

                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, (dateTime.Hour / intPointHour) * intPointHour, 0, 0);
            }
            else if (curvePoint < CurvePointOptions.Point2880)
            {
                int intPointMinute = 1440 / curvePoint.GetPointCount(); //曲线上的一个点，对应多少长度的时间(分钟)。

                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, (dateTime.Minute / intPointMinute) * intPointMinute, 0);
            }
            else
            {
                int intPointSecond = 86400 / curvePoint.GetPointCount(); //曲线上的一个点，对应多少长度的时间(秒)。

                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, (dateTime.Second / intPointSecond) * intPointSecond);
            }
        }

        /// <summary>
        /// 将时间转为对应数据点的准确时间。
        /// </summary>
        /// <param name="curvePoint"></param>
        /// <param name="time">时间，只使用时、分、秒部分。</param>
        /// <returns>整型时间（hhmmss），返回的时间值一定小于或等于传入的时间值。</returns>
        public static int FormatTimeNum(this CurvePointOptions curvePoint, DateTime time)
        {
            return FormatTimeNum(curvePoint, time.Hour * 10000 + time.Minute * 100 + time.Second);
        }

        /// <summary>
        /// 将时间转为对应数据点的准确时间。
        /// </summary>
        /// <param name="curvePoint"></param>
        /// <param name="timeNum">整型时间（hhmmss）。</param>
        /// <returns>整型时间（hhmmss），返回的时间值一定小于或等于传入的时间值。</returns>
        public static int FormatTimeNum(this CurvePointOptions curvePoint, int timeNum)
        {
            if (curvePoint < CurvePointOptions.Point48)
            {
                int intPointHour = 24 / curvePoint.GetPointCount();     //曲线上的一个点，对应多少长度的时间(小时)。

                return ((timeNum / 10000) / intPointHour) * intPointHour * 10000;
            }
            else if (curvePoint < CurvePointOptions.Point2880)
            {
                int intPointMinute = 1440 / curvePoint.GetPointCount(); //曲线上的一个点，对应多少长度的时间(分钟)。

                return (timeNum / 10000) * 10000 + (((timeNum / 100) % 100) / intPointMinute) * intPointMinute * 100;
            }
            else
            {
                int intPointSecond = 86400 / curvePoint.GetPointCount(); //曲线上的一个点，对应多少长度的时间(秒)。

                return (timeNum / 10000) * 10000 + ((timeNum / 100) % 100) * 100 + ((timeNum % 100) / intPointSecond) * intPointSecond;
            }
        }

        /// <summary>
        /// 获取指定时间的从 0 开始的曲线索引位置。
        /// </summary>
        /// <param name="curvePoint"></param>
        /// <param name="timeNum">从 0:00:00 开始的格式为 hhmmss 6位整型时间。</param>
        /// <returns></returns>
        public static int GetPointIndex(this CurvePointOptions curvePoint, int timeNum)
        {
            if (curvePoint < CurvePointOptions.Point48)
            {
                int intPointHour = 24 / curvePoint.GetPointCount();     //曲线上的一个点，对应多少长度的时间(小时)。

                return (timeNum / 10000) / intPointHour;
            }
            else if (curvePoint < CurvePointOptions.Point2880)
            {
                int intPointMinute = 1440 / curvePoint.GetPointCount(); //曲线上的一个点，对应多少长度的时间(分钟)。
                int intHourPoint = 60 / intPointMinute;                 //一个小时有几个点

                return (timeNum / 10000) * intHourPoint + ((timeNum / 100) % 100) / intPointMinute;
            }
            else
            {
                int intPointSecond = 86400 / curvePoint.GetPointCount(); //曲线上的一个点，对应多少长度的时间(秒)。
                int intMinutePoint = 60 / intPointSecond;
                int intHourPoint = 3600 / intPointSecond;

                return (timeNum / 10000) * intHourPoint + ((timeNum / 100) % 100) * intMinutePoint + (timeNum % 100) / intPointSecond;
            }
        }

        /// <summary>
        /// 获取指定时间的索引位置。
        /// </summary>
        /// <param name="curvePoint"></param>
        /// <param name="time">时间，只使用时间部分。</param>
        /// <returns>从 0 开始的曲线索引位置。</returns>
        public static int GetPointIndex(this CurvePointOptions curvePoint, DateTime time)
        {
            return GetPointIndex(curvePoint, time.Hour * 10000 + time.Minute * 100 + time.Second);
        }
    }
}

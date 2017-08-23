using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 表示一天曲线数据数值集合中的一部分数值。
    /// </summary>
    public class CurvePartValue
    {
        /// <summary>
        /// 一天曲线数据数值集合中的一部分数值。
        /// </summary>
        /// <param name="curvePoint">曲线点数，该参数为 0:00:00 至 23:59:59 完整的曲线数据点数。</param>
        /// <param name="startTimeNum">第一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="endTimeNum">最后一个点的数值所对应的时间（hhmmss）。</param>
        public CurvePartValue(CurvePointOptions curvePoint, int startTimeNum, int endTimeNum)
        {
            this.m_CurvePoint = curvePoint;
            this.m_StartTimeNum = this.CurvePoint.FormatTimeNum(startTimeNum);
            this.m_EndTimeNum = this.CurvePoint.FormatTimeNum(endTimeNum);
        }

        /// <summary>
        /// 一天曲线数据数值集合中的一部分数值。
        /// </summary>
        /// <param name="curvePoint">曲线点数，该参数为 0:00:00 至 23:59:59 完整的曲线数据点数。</param>
        /// <param name="startTimeNum">第一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="values">曲线段各点数据。</param>
        public CurvePartValue(CurvePointOptions curvePoint, int startTimeNum, decimal?[] values)
        {
            if (values == null && values.Length == 0)
            {
                throw new Exception("传入的 values 数据数组长度需要大于 0。");
            }

            this.m_CurvePoint = curvePoint;
            this.m_StartTimeNum = this.CurvePoint.FormatTimeNum(startTimeNum);

            int intEndIndex = this.CurvePoint.GetPointIndex(this.StartTimeNum) + values.Length - 1;
            if (intEndIndex >= curvePoint.GetPointCount())
            {
                throw new Exception("传入的 values 数据数组长度超出了一天的范围。");
            }

            this.m_Values = values;
            this.m_EndTimeNum = curvePoint.GetTimeNum(intEndIndex);
        }

        /// <summary>
        /// 一天曲线数据数值集合中的一部分数值。
        /// </summary>
        /// <param name="startTimeNum">第一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="endTimeNum">最后一个点的数值所对应的时间（hhmmss）。</param>
        /// <param name="values">曲线段各点数据。</param>
        public CurvePartValue(int startTimeNum, int endTimeNum, decimal?[] values)
        {
            this.m_CurvePoint = CurvePointExtensions.GetCurvePoint(startTimeNum, endTimeNum, values.Length);
            this.m_StartTimeNum = this.CurvePoint.FormatTimeNum(startTimeNum);
            this.m_EndTimeNum = this.CurvePoint.FormatTimeNum(endTimeNum);
            this.m_Values = values;
        }

        private CurvePointOptions m_CurvePoint;
        /// <summary>
        /// 获取曲线点数，该属性为 0:00:00 至 23:59:59 完整的曲线数据点数。
        /// </summary>
        public CurvePointOptions CurvePoint
        {
            get
            {
                return this.m_CurvePoint;
            }
        }

        private int m_StartTimeNum;
        /// <summary>
        /// 获取第一个点的数值所对应的时间（hhmmss）。
        /// </summary>
        public int StartTimeNum
        {
            get
            {
                return this.m_StartTimeNum;
            }
        }

        private int m_EndTimeNum;
        /// <summary>
        /// 获取最后一个点的数值所对应的时间（hhmmss）。
        /// </summary>
        public int EndTimeNum
        {
            get
            {
                return this.m_EndTimeNum;
            }
        }

        private decimal?[] m_Values;
        /// <summary>
        /// 获取曲线数据段各点数据。
        /// </summary>
        public decimal?[] Values
        {
            get
            {
                if (this.m_Values == null)
                {
                    this.m_Values = new decimal?[this.CurvePoint.GetPointCount(this.StartTimeNum, this.EndTimeNum)];
                }
                return this.m_Values;
            }
        }

        /// <summary>
        /// 将指定时间的数据设置在曲线点对应的索引上。
        /// </summary>
        /// <param name="timeNum">整型时间 hhmmss。</param>
        /// <param name="value">数值。</param>
        public void SetValue(int timeNum, decimal? value)
        {
            this.Values[this.CurvePoint.GetTimeSpanPoint(this.StartTimeNum, timeNum)] = value;
        }

        /// <summary>
        /// 获取指定时间的数据。
        /// </summary>
        /// <param name="timeNum">整型时间 hhmmss。</param>
        /// <returns></returns>
        public decimal? GetValue(int timeNum)
        {
            return this.Values[this.CurvePoint.GetTimeSpanPoint(this.StartTimeNum, timeNum)];
        }

        /// <summary>
        /// 根据曲线数据段数组中的索引获取该索引对应的时间。
        /// </summary>
        /// <param name="valueIndex">当前数据段数值数组中的索引，该值从 0 开始。</param>
        /// <returns>数值时间(hhmmss)。</returns>
        public int GetTimeNum(int valueIndex)
        {
            return this.CurvePoint.GetTimeNum(this.StartTimeNum, valueIndex);
        }

        /// <summary>
        /// 根据曲线数据段数组中的索引获取该索引对应的时间。
        /// </summary>
        /// <param name="valueIndex">当前数据段数值数组中的索引，该值从 0 开始。</param>
        /// <returns>在完整的曲线数据中对应的索引。</returns>
        public int GetPointIndex(int valueIndex)
        {
            return this.CurvePoint.GetPointIndex(this.StartTimeNum) + valueIndex;
        }

        /// <summary>
        /// 将当前的曲线数据转换为另一种点数的曲线数据。
        /// </summary>
        /// <param name="convertedInto">转换后的曲线点数。</param>
        /// <param name="mergeOption">当转换后的点数小于目前曲线点数时，如何合并目前的数据。</param>
        /// <param name="splitOption">当转换后的点数大于目前曲线点数时，如何拆分目前的数据。</param>
        public void Convert(CurvePointOptions convertedInto, CurveMergeOptions mergeOption, CurveSplitOptions splitOption)
        {
            this.m_Values = ConvertTo(this.CurvePoint, this.StartTimeNum, this.Values, convertedInto, mergeOption, splitOption);
            this.m_CurvePoint = convertedInto;
            this.m_StartTimeNum = this.CurvePoint.FormatTimeNum(this.StartTimeNum);
            this.m_EndTimeNum = this.CurvePoint.FormatTimeNum(this.EndTimeNum);
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.CurvePoint + "(" + this.CurvePoint.GetDescription() + ") " + this.StartTimeNum + " - " + this.EndTimeNum;
        }

        /// <summary>
        /// 将指定的曲线段数据转换为另一种点数的曲线段数据。
        /// </summary>
        /// <param name="sourceCurvePoint">源数据的曲线点数，该参数为 0:00:00 至 23:59:59 完整的曲线数据点数。</param>
        /// <param name="sourceStartTimeNum">源数据第一个点的数值对应的时间。</param>
        /// <param name="sourceValues">源数据数组。</param>
        /// <param name="convertedInto">转换后的曲线点数。</param>
        /// <param name="mergeOption">当转换后的点数小于目前曲线点数时，如何合并目前的数据。</param>
        /// <param name="splitOption">当转换后的点数大于目前曲线点数时，如何拆分目前的数据。</param>
        /// <returns>转换后的曲线数据，该数组的长度等于 convertedInto 参数指定的点数。</returns>
        public static decimal?[] ConvertTo(CurvePointOptions sourceCurvePoint, int sourceStartTimeNum, decimal?[] sourceValues, CurvePointOptions convertedInto, CurveMergeOptions mergeOption, CurveSplitOptions splitOption)
        {
            if (sourceCurvePoint == convertedInto)
            {
                return sourceValues;
            }
            else
            {
                sourceStartTimeNum = sourceCurvePoint.FormatTimeNum(sourceStartTimeNum);

                if (sourceValues == null && sourceValues.Length == 0)
                {
                    throw new Exception("传入的 sourceValues 数据数组长度需要大于 0。");
                }

                int intEndIndex = sourceCurvePoint.GetPointIndex(sourceStartTimeNum) + sourceValues.Length - 1;
                if (intEndIndex >= sourceCurvePoint.GetPointCount())
                {
                    throw new Exception("传入的 sourceValues 数据数组长度超出了一天的范围。");
                }

                int sourceEndTimeNum = sourceCurvePoint.GetTimeNum(intEndIndex);
                int intIntoStartTimeNum = convertedInto.FormatTimeNum(sourceStartTimeNum);    //转换后的开始时间

                if (sourceCurvePoint > convertedInto)
                {
                    if ((sourceCurvePoint.GetPointCount() % convertedInto.GetPointCount()) == 0)
                    {
                        int intIntoEndTimeNum = convertedInto.FormatTimeNum(sourceEndTimeNum);        //转换后的结束时间
                        int intIntoPointCount = convertedInto.GetTimeSpanPoint(intIntoStartTimeNum, intIntoEndTimeNum) + 1; //转换后的数据点数
                        decimal?[] decValue = new decimal?[intIntoPointCount];
                        int intSourceIntoDiffPoint = sourceCurvePoint.GetTimeSpanPoint(intIntoStartTimeNum, sourceStartTimeNum); //源数据起始点与目标数据起始点相差的点数。
                        int intCurveMergeNum = sourceCurvePoint.GetPointCount() / convertedInto.GetPointCount();    //转换后数据的一个点对应源数据的几个点

                        for (int intIndex = 0; intIndex < decValue.Length; intIndex++)
                        {
                            AC.Base.Drives.CurveValue.ICurveMerge _CurveMerge = null;
                            switch (mergeOption)
                            {
                                case CurveMergeOptions.Addition:
                                    _CurveMerge = new AC.Base.Drives.CurveValue.CurveMergeAddition();
                                    break;

                                case CurveMergeOptions.Average:
                                    _CurveMerge = new AC.Base.Drives.CurveValue.CurveMergeAverage();
                                    break;

                                case CurveMergeOptions.First:
                                    _CurveMerge = new AC.Base.Drives.CurveValue.CurveMergeFirst();
                                    break;

                                case CurveMergeOptions.Maximum:
                                    _CurveMerge = new AC.Base.Drives.CurveValue.CurveMergeMaximum();
                                    break;

                                case CurveMergeOptions.Minimum:
                                    _CurveMerge = new AC.Base.Drives.CurveValue.CurveMergeMinimum();
                                    break;

                                default:
                                    throw new Exception("无 " + mergeOption.ToString() + " 数据转换方法。");
                            }

                            int intIntoTimeNum = convertedInto.GetTimeNum(intIntoStartTimeNum, intIndex);
                            for (int intCurveMergeIndex = 0; intCurveMergeIndex < intCurveMergeNum; intCurveMergeIndex++)
                            {
                                int intSourceTimeNum = sourceCurvePoint.GetTimeNum(intIntoTimeNum, intCurveMergeIndex);
                                if (sourceStartTimeNum <= intSourceTimeNum && intSourceTimeNum <= sourceEndTimeNum)
                                {
                                    _CurveMerge.Value.Add(sourceValues[intIndex * intCurveMergeNum + intCurveMergeIndex - intSourceIntoDiffPoint]);
                                }
                            }
                            decValue[intIndex] = _CurveMerge.GetValue();
                        }
                        return decValue;
                    }
                    else if (sourceCurvePoint == CurvePointOptions.Point144 && convertedInto == CurvePointOptions.Point96)
                    {
                        //将144点转为96点。先将144点转为288点，然后将288点转为96点。
                        decimal?[] decValue288 = ConvertTo(CurvePointOptions.Point144, sourceStartTimeNum, sourceValues, CurvePointOptions.Point288, mergeOption, splitOption);
                        return ConvertTo(CurvePointOptions.Point288, sourceStartTimeNum, decValue288, CurvePointOptions.Point96, mergeOption, splitOption);
                    }
                    else
                    {
                        throw new Exception("尚不支持将 " + sourceCurvePoint.GetPointCount() + " 点数据转为 " + convertedInto.GetPointCount() + " 点数据。");
                    }
                }
                else
                {
                    //sourceCurvePoint < convertedInto
                    if ((convertedInto.GetPointCount() % sourceCurvePoint.GetPointCount()) == 0)
                    {
                        int intIntoEndTimeNum = convertedInto.GetTimeNum(sourceCurvePoint.GetTimeNum(sourceEndTimeNum, 1), -1);         //转换后的结束时间。源时间加一个点的时间转成目标时间后，再计算目标时间减一个点的时间
                        int intIntoPointCount = convertedInto.GetTimeSpanPoint(intIntoStartTimeNum, intIntoEndTimeNum) + 1; //转换后的数据点数
                        decimal?[] decValue = new decimal?[intIntoPointCount];
                        int intCurveSplitNum = convertedInto.GetPointCount() / sourceCurvePoint.GetPointCount();    //源数据的一个点对应转换后数据的几个点

                        for (int intIndex = 0; intIndex < sourceValues.Length; intIndex++)
                        {
                            if (sourceValues[intIndex] != null)
                            {
                                for (int intCurveSplitIndex = 0; intCurveSplitIndex < intCurveSplitNum; intCurveSplitIndex++)
                                {
                                    switch (splitOption)
                                    {
                                        case CurveSplitOptions.Division:
                                            decValue[intIndex * intCurveSplitNum + intCurveSplitIndex] = sourceValues[intIndex] / intCurveSplitNum;
                                            break;

                                        case CurveSplitOptions.Copy:
                                            decValue[intIndex * intCurveSplitNum + intCurveSplitIndex] = sourceValues[intIndex];
                                            break;

                                        default:
                                            throw new Exception("无 " + splitOption.ToString() + " 数据转换方法。");
                                    }
                                }
                            }
                        }
                        return decValue;
                    }
                    else if (sourceCurvePoint == CurvePointOptions.Point96 && convertedInto == CurvePointOptions.Point144)
                    {
                        //将96点转为144点。先将96点转为288点，然后将288点转为144点。
                        decimal?[] decValue288 = ConvertTo(CurvePointOptions.Point96, sourceStartTimeNum, sourceValues, CurvePointOptions.Point288, mergeOption, splitOption);
                        return ConvertTo(CurvePointOptions.Point288, sourceStartTimeNum, decValue288, CurvePointOptions.Point144, mergeOption, splitOption);
                    }
                    else
                    {
                        throw new Exception("尚不支持将 " + sourceCurvePoint.GetPointCount() + " 点数据转为 " + convertedInto.GetPointCount() + " 点数据。");
                    }
                }
            }
        }

    }
}

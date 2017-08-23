using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 表示一天从 0:00:00 至 23:59:59 之间多个数值组成的数值集合。
    /// 可以使用 CurveValue3 = CurveValue1 + CurveValue2 直接将两条曲线相加。
    /// </summary>
    public class CurveValue
    {
        /// <summary>
        /// 表示一天从 0:00:00 至 23:59:59 之间多个数值组成的数值集合。
        /// </summary>
        /// <param name="curvePoint">该曲线的点数。</param>
        public CurveValue(CurvePointOptions curvePoint)
        {
            this.m_CurvePoint = curvePoint;
        }

        /// <summary>
        /// 使用数据数组初始化一天从 0:00:00 至 23:59:59 之间多个数值组成的数值集合。
        /// </summary>
        /// <param name="values">数据数组。数据数组的长度必须符合 CurvePointOptions 中定义的长度。</param>
        public CurveValue(decimal?[] values)
        {
            string strPoint = "";

            foreach (int intEnumValue in Enum.GetValues(typeof(CurvePointOptions)))
            {
                CurvePointOptions curvePoint = (CurvePointOptions)intEnumValue;

                if (values.Length == curvePoint.GetPointCount())
                {
                    this.m_CurvePoint = curvePoint;
                    this.m_Values = values;
                    return;
                }

                strPoint += "、" + curvePoint.GetPointCount();
            }

            throw new Exception("数据数组的长度必须等于 " + strPoint.Substring(1) + "。");
        }

        private CurvePointOptions m_CurvePoint;
        /// <summary>
        /// 曲线点数。
        /// </summary>
        public CurvePointOptions CurvePoint
        {
            get
            {
                return this.m_CurvePoint;
            }
        }

        private decimal?[] m_Values;
        /// <summary>
        /// 曲线各点数据。
        /// </summary>
        public decimal?[] Values
        {
            get
            {
                if (this.m_Values == null)
                {
                    this.m_Values = new decimal?[this.CurvePoint.GetPointCount()];
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
            this.Values[this.CurvePoint.GetPointIndex(timeNum)] = value;
        }

        /// <summary>
        /// 获取指定时间的数据。
        /// </summary>
        /// <param name="timeNum">整型时间 hhmmss。</param>
        /// <returns></returns>
        public decimal? GetValue(int timeNum)
        {
            return this.Values[this.CurvePoint.GetPointIndex(timeNum)];
        }

        /// <summary>
        /// 获取该曲线数据中的最大值。
        /// </summary>
        /// <returns></returns>
        public decimal? GetMaxValue()
        {
            decimal? decValue = null;

            foreach (decimal? d in this.Values)
            {
                if (d != null)
                {
                    if (decValue == null || d > decValue)
                    {
                        decValue = d;
                    }
                }
            }

            return decValue;
        }

        /// <summary>
        /// 获取该曲线数据中最大值的索引位置。如果曲线数据全为 null 则返回 -1。
        /// </summary>
        /// <returns></returns>
        public int GetMaxValueIndex()
        {
            decimal? decValue = null;
            int index = -1;

            for (int intIndex = 0; intIndex < this.Values.Length; intIndex++)
            {
                if (this.Values[intIndex] != null)
                {
                    if (decValue == null || this.Values[intIndex] > decValue)
                    {
                        decValue = this.Values[intIndex];
                        index = intIndex;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// 获取该曲线数据中的最小值。
        /// </summary>
        /// <returns></returns>
        public decimal? GetMinValue()
        {
            decimal? decValue = null;

            foreach (decimal? d in this.Values)
            {
                if (d != null)
                {
                    if (decValue == null || d < decValue)
                    {
                        decValue = d;
                    }
                }
            }

            return decValue;
        }

        /// <summary>
        /// 获取该曲线数据中最小值的索引位置。如果曲线数据全为 null 则返回 -1。
        /// </summary>
        /// <returns></returns>
        public int GetMinValueIndex()
        {
            decimal? decValue = null;
            int index = -1;

            for (int intIndex = 0; intIndex < this.Values.Length; intIndex++)
            {
                if (this.Values[intIndex] != null)
                {
                    if (decValue == null || this.Values[intIndex] < decValue)
                    {
                        decValue = this.Values[intIndex];
                        index = intIndex;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// 获取该曲线数据各点相加的合计值。
        /// </summary>
        /// <returns></returns>
        public decimal? GetSumValue()
        {
            decimal? decValue = null;

            foreach (decimal? d in this.Values)
            {
                if (d != null)
                {
                    if (decValue == null)
                    {
                        decValue = d;
                    }
                    else
                    {
                        decValue += d;
                    }
                }
            }

            return decValue;
        }

        /// <summary>
        /// 获取该曲线数据各有效点的平均值。
        /// </summary>
        /// <returns></returns>
        public decimal? GetAvgValue()
        {
            decimal? decValue = null;
            decimal decCount = 0;

            foreach (decimal? d in this.Values)
            {
                if (d != null)
                {
                    if (decValue == null)
                    {
                        decValue = d;
                    }
                    else
                    {
                        decValue += d;
                    }
                    decCount = decCount + 1;
                }
            }

            if (decCount > 0)
            {
                return decValue / decCount;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取该曲线数据中最大值与最小值的差值。
        /// </summary>
        /// <returns></returns>
        public decimal? GetMaxMinDiff()
        {
            decimal? decMaxValue = null;
            decimal? decMinValue = null;

            foreach (decimal? d in this.Values)
            {
                if (d != null)
                {
                    if (decMaxValue == null)
                    {
                        decMaxValue = d;
                        decMinValue = d;
                    }
                    else
                    {
                        if (d > decMaxValue)
                        {
                            decMaxValue = d;
                        }
                        else if (d < decMaxValue)
                        {
                            decMinValue = d;
                        }
                    }
                }
            }

            if (decMaxValue != null && decMinValue != null)
            {
                return decMaxValue - decMinValue;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取该曲线数据中最大值与最小值的相差比率，又叫峰谷差率 %，(最大值 - 最小值) / 最大值
        /// </summary>
        /// <returns></returns>
        public decimal? GetMaxMinDiffRatio()
        {
            decimal? decMaxValue = null;
            decimal? decMinValue = null;

            foreach (decimal? d in this.Values)
            {
                if (d != null)
                {
                    if (decMaxValue == null)
                    {
                        decMaxValue = d;
                        decMinValue = d;
                    }
                    else
                    {
                        if (d > decMaxValue)
                        {
                            decMaxValue = d;
                        }
                        else if (d < decMaxValue)
                        {
                            decMinValue = d;
                        }
                    }
                }
            }

            if (decMaxValue != null && decMinValue != null)
            {
                if (decMaxValue != 0)
                {
                    return (decMaxValue - decMinValue) / decMaxValue;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 将当前的曲线数据转换为另一种点数的曲线数据。
        /// </summary>
        /// <param name="convertedInto">转换后的曲线点数。</param>
        /// <param name="mergeOption">当转换后的点数小于目前曲线点数时，如何合并目前的数据。</param>
        /// <param name="splitOption">当转换后的点数大于目前曲线点数时，如何拆分目前的数据。</param>
        public void Convert(CurvePointOptions convertedInto, CurveMergeOptions mergeOption, CurveSplitOptions splitOption)
        {
            this.m_CurvePoint = convertedInto;
            this.m_Values = ConvertTo(this.Values, convertedInto, mergeOption, splitOption);
        }

        /// <summary>
        /// 将指定的曲线数据转换为另一种点数的曲线数据。
        /// </summary>
        /// <param name="sourceValues">源数据数组。</param>
        /// <param name="convertedInto">转换后的曲线点数。</param>
        /// <param name="mergeOption">当转换后的点数小于目前曲线点数时，如何合并目前的数据。</param>
        /// <param name="splitOption">当转换后的点数大于目前曲线点数时，如何拆分目前的数据。</param>
        /// <returns>转换后的曲线数据，该数组的长度等于 convertedInto 参数指定的点数。</returns>
        public static decimal?[] ConvertTo(decimal?[] sourceValues, CurvePointOptions convertedInto, CurveMergeOptions mergeOption, CurveSplitOptions splitOption)
        {
            string strPoint = "";

            foreach (int intEnumValue in Enum.GetValues(typeof(CurvePointOptions)))
            {
                CurvePointOptions _SourceCurvePoint = (CurvePointOptions)intEnumValue;

                if (sourceValues.Length == _SourceCurvePoint.GetPointCount())
                {
                    if (_SourceCurvePoint > convertedInto)
                    {
                        if ((_SourceCurvePoint.GetPointCount() % convertedInto.GetPointCount()) == 0)
                        {
                            decimal?[] decValue = new decimal?[convertedInto.GetPointCount()];
                            int intCurveMergeNum = _SourceCurvePoint.GetPointCount() / convertedInto.GetPointCount();
                            for (int intIndex = 0; intIndex < decValue.Length; intIndex++)
                            {
                                ICurveMerge _CurveMerge = null;
                                switch (mergeOption)
                                {
                                    case CurveMergeOptions.Addition:
                                        _CurveMerge = new CurveMergeAddition();
                                        break;

                                    case CurveMergeOptions.Average:
                                        _CurveMerge = new CurveMergeAverage();
                                        break;

                                    case CurveMergeOptions.First:
                                        _CurveMerge = new CurveMergeFirst();
                                        break;

                                    case CurveMergeOptions.Maximum:
                                        _CurveMerge = new CurveMergeMaximum();
                                        break;

                                    case CurveMergeOptions.Minimum:
                                        _CurveMerge = new CurveMergeMinimum();
                                        break;

                                    default:
                                        throw new Exception("无 " + mergeOption.ToString() + " 数据转换方法。");
                                }

                                for (int intCurveMergeIndex = 0; intCurveMergeIndex < intCurveMergeNum; intCurveMergeIndex++)
                                {
                                    _CurveMerge.Value.Add(sourceValues[intIndex * intCurveMergeNum + intCurveMergeIndex]);
                                }
                                decValue[intIndex] = _CurveMerge.GetValue();
                            }
                            return decValue;
                        }
                        else if (_SourceCurvePoint == CurvePointOptions.Point144 && convertedInto == CurvePointOptions.Point96)
                        {
                            //将144点转为96点。先将144点转为288点，然后将288点转为96点。
                            decimal?[] decValue288 = ConvertTo(sourceValues, CurvePointOptions.Point288, mergeOption, splitOption);
                            return ConvertTo(decValue288, CurvePointOptions.Point96, mergeOption, splitOption);
                        }
                        else
                        {
                            throw new Exception("尚不支持将 " + _SourceCurvePoint.GetPointCount() + " 点数据转为 " + convertedInto.GetPointCount() + " 点数据。");
                        }
                    }
                    else if (_SourceCurvePoint < convertedInto)
                    {
                        if ((convertedInto.GetPointCount() % _SourceCurvePoint.GetPointCount()) == 0)
                        {
                            decimal?[] decValue = new decimal?[convertedInto.GetPointCount()];
                            int intCurveSplitNum = convertedInto.GetPointCount() / _SourceCurvePoint.GetPointCount();
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
                        else if (_SourceCurvePoint == CurvePointOptions.Point96 && convertedInto == CurvePointOptions.Point144)
                        {
                            //将96点转为144点。先将96点转为288点，然后将288点转为144点。
                            decimal?[] decValue288 = ConvertTo(sourceValues, CurvePointOptions.Point288, mergeOption, splitOption);
                            return ConvertTo(decValue288, CurvePointOptions.Point144, mergeOption, splitOption);
                        }
                        else
                        {
                            throw new Exception("尚不支持将 " + _SourceCurvePoint.GetPointCount() + " 点数据转为 " + convertedInto.GetPointCount() + " 点数据。");
                        }
                    }
                    else
                    {
                        return sourceValues;
                    }
                }

                strPoint += "、" + _SourceCurvePoint.GetPointCount();
            }

            throw new Exception("源数据数组的长度必须等于 " + strPoint.Substring(1) + "。");
        }

        internal interface ICurveMerge
        {
            List<decimal?> Value { get; }

            decimal? GetValue();
        }

        /// <summary>
        /// 将多个点的数值相加后合并为一个数值。
        /// </summary>
        internal class CurveMergeAddition : ICurveMerge
        {
            private List<decimal?> m_Value = new List<decimal?>();
            public List<decimal?> Value { get { return this.m_Value; } }

            public decimal? GetValue()
            {
                decimal? decValue = null;
                foreach (decimal? value in this.Value)
                {
                    if (value != null)
                    {
                        if (decValue == null)
                        {
                            decValue = value;
                        }
                        else
                        {
                            decValue += value;
                        }
                    }
                }
                return decValue;
            }
        }

        /// <summary>
        /// 将多个点的数值平均后合并为一个数值。
        /// </summary>
        internal class CurveMergeAverage : ICurveMerge
        {
            private List<decimal?> m_Value = new List<decimal?>();
            public List<decimal?> Value { get { return this.m_Value; } }

            public decimal? GetValue()
            {
                decimal? decValue = null;
                decimal decNum = 0;
                foreach (decimal? value in this.Value)
                {
                    if (value != null)
                    {
                        if (decValue == null)
                        {
                            decValue = value;
                        }
                        else
                        {
                            decValue += value;
                        }
                        decNum += 1;
                    }
                }

                if (decNum > 0)
                {
                    return decValue / decNum;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 使用多个点中的第一个有效数值。
        /// </summary>
        internal class CurveMergeFirst : ICurveMerge
        {
            private List<decimal?> m_Value = new List<decimal?>();
            public List<decimal?> Value { get { return this.m_Value; } }

            public decimal? GetValue()
            {
                foreach (decimal? value in this.Value)
                {
                    if (value != null)
                    {
                        return value;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// 使用多个数值中最大的一个作为合并后的数值。
        /// </summary>
        internal class CurveMergeMaximum : ICurveMerge
        {
            private List<decimal?> m_Value = new List<decimal?>();
            public List<decimal?> Value { get { return this.m_Value; } }

            public decimal? GetValue()
            {
                decimal? decValue = null;
                foreach (decimal? value in this.Value)
                {
                    if (value != null)
                    {
                        if (decValue == null)
                        {
                            decValue = value;
                        }
                        else if (value > decValue)
                        {
                            decValue = value;
                        }
                    }
                }
                return decValue;
            }
        }

        /// <summary>
        /// 使用多个数值中最小的一个作为合并后的数值。
        /// </summary>
        internal class CurveMergeMinimum : ICurveMerge
        {
            private List<decimal?> m_Value = new List<decimal?>();
            public List<decimal?> Value { get { return this.m_Value; } }

            public decimal? GetValue()
            {
                decimal? decValue = null;
                foreach (decimal? value in this.Value)
                {
                    if (value != null)
                    {
                        if (decValue == null)
                        {
                            decValue = value;
                        }
                        else if (value < decValue)
                        {
                            decValue = value;
                        }
                    }
                }
                return decValue;
            }
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.CurvePoint + "(" + this.CurvePoint.GetDescription() + ")";
        }

        /// <summary>
        /// 将两个曲线相加。
        /// </summary>
        /// <param name="value1">曲线1</param>
        /// <param name="value2">曲线2</param>
        /// <returns></returns>
        public static CurveValue operator +(CurveValue value1, CurveValue value2)
        {
            if (value1.CurvePoint != value2.CurvePoint)
            {
                throw new CurveException("曲线1与曲线2的点数不一致，无法进行加法运算");
            }
            else
            {
                CurveValue _CurveValue = new CurveValue(value1.CurvePoint);
                for (int intIndex = 0; intIndex < _CurveValue.CurvePoint.GetPointCount(); intIndex++)
                {
                    if (value1.Values[intIndex] != null || value2.Values[intIndex] != null)
                    {
                        if (value1.Values[intIndex] == null)
                        {
                            _CurveValue.Values[intIndex] = value2.Values[intIndex];
                        }
                        else if (value2.Values[intIndex] == null)
                        {
                            _CurveValue.Values[intIndex] = value1.Values[intIndex];
                        }
                        else
                        {
                            _CurveValue.Values[intIndex] = value1.Values[intIndex] + value2.Values[intIndex];
                        }
                    }
                }
                return _CurveValue;
            }
        }

    }
}

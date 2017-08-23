using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    /// <summary>
    /// 常用函数。
    /// </summary>
    public class Function
    {
        #region << 字节函数 >>

        /// <summary>
        /// 将一个字节数据转为 BCD 编码的字节数据。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte ByteToBCD(byte value)
        {
            return (byte)((value / 10) * 16 + (value % 10));
        }

        /// <summary>
        /// 将一个字节数据转为 BCD 编码的字节数据。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte ByteToBCD(int value)
        {
            return (byte)((value / 10) * 16 + (value % 10));
        }

        /// <summary>
        /// 验证传入的一个字节数据能否正确转为 BCD 编码的数据。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ByteToBCDVerification(byte value)
        {
            if ((value >> 4) > 9)
            {
                return false;
            }
            if ((value % 0x10) > 9)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将 BCD 编码的数据转为一个字节的十六进制数据。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte BCDToByte(byte value)
        {
            return (byte)(((value >> 4) % 10) * 10 + value % 16);
        }

        /// <summary>
        /// 将使用空格分隔的十六进制显示的字符串转换为字节数组。
        /// </summary>
        /// <param name="value">使用十六进制显示的字符串</param>
        /// <returns>字节数组</returns>
        public static byte[] ToBytes(string value)
        {
            return ToBytes(value, null);
        }

        /// <summary>
        /// 将使用十六进制显示的字符串转换为字节数组。
        /// </summary>
        /// <param name="value">使用十六进制显示的字符串</param>
        /// <param name="separator">字符串中每个字节的分隔符</param>
        /// <returns>字节数组</returns>
        public static byte[] ToBytes(string value, string separator)
        {
            //Convert.ToByte
            string strValue = value.Trim();
            if (separator != null)
            {
                strValue = strValue.Replace(separator, "");
            }
            else
            {
                strValue = strValue.Replace(" ", "");
            }

            int intLength = strValue.Length / 2;
            if ((strValue.Length % 2) > 0)
            {
                intLength++;
            }

            byte[] bytDatas = new byte[intLength];

            for (int intIndex = 0; intIndex < strValue.Length; intIndex++)
            {
                if ((intIndex + 2) <= strValue.Length)
                {
                    bytDatas[intIndex / 2] = Convert.ToByte(strValue.Substring(intIndex, 2), 16);
                }
                else
                {
                    bytDatas[intIndex / 2] = Convert.ToByte(strValue.Substring(intIndex, 1), 16);
                }

                intIndex++;
            }
            return bytDatas;
        }

        /// <summary>
        /// 颠倒字节数组内的数据索引顺序。例如将 byte[]{0x68, 0x81, 0x16} 转换为 byte[]{0x16, 0x81, 0x68}
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] BytesReversal(byte[] values)
        {
            byte[] bytValue = new byte[values.Length];

            for (int intIndex = 0; intIndex < values.Length; intIndex++)
            {
                bytValue[intIndex] = values[values.Length - intIndex - 1];
            }

            return bytValue;
        }

        /// <summary>
        /// 比较传入的 2 个字节数组内容是否完全一致，如果其中任何一个为 null 则返回 false，只有当数组长度及内容完全一致时方返回 true。
        /// </summary>
        /// <param name="value1">字节数组。</param>
        /// <param name="value2">字节数组。</param>
        /// <returns></returns>
        public static bool Equals(byte[] value1, byte[] value2)
        {
            if (value1 == null || value2 == null)
            {
                return false;
            }
            else if (value1.Length != value2.Length)
            {
                return false;
            }
            else
            {
                for (int intIndex = 0; intIndex < value1.Length; intIndex++)
                {
                    if (value1[intIndex] != value2[intIndex])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 将字节数组转为可输出显示的字符串。
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string OutBytes(byte[] values)
        {
            if (values != null)
            {
                return BitConverter.ToString(values).Replace("-", " ");
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 确保将输入的布尔型值转为 0 或 1 的数字。例如 True 将转为 1；False 将转为 0 。
        /// </summary>
        /// <param name="value">欲转换成数字的布尔值</param>
        /// <returns>0 或 1 的数字</returns>
        public static byte BoolToByte(object value)
        {
            try
            {
                if (value == System.DBNull.Value)
                {
                    return 0;
                }
                else if (value == null)
                {
                    return 0;
                }
                else if (Convert.ToBoolean(value) == true)
                {
                    return 1;
                }
            }
            catch
            { }
            return 0;
        }

        /// <summary>
        /// 确保将 0 或 1 的数字或者 True 或 False 的字符串、布尔值转为 Boolean 型值。例如 0 将转为 False；1 将转为 True 。
        /// </summary>
        /// <param name="value">0、1、"true"、"false"、True、False</param>
        /// <returns>True 或 False 的布尔值</returns>
        public static bool ToBool(object value)
        {
            try
            {
                if (value == System.DBNull.Value)
                {
                    return false;
                }
                else if (value == null)
                {
                    return false;
                }
                else if (Convert.ToBoolean(value))
                {
                    return true;
                }
            }
            catch
            {
                try
                {
                    if (Convert.ToByte(value) == 1)
                    {
                        return true;
                    }
                }
                catch
                { }
            }
            return false;
        }

        /// <summary>
        /// 确保将输入的参数转为 Byte 型数字
        /// </summary>
        /// <param name="value">0 到 255（无符号）；舍入小数部分。</param>
        /// <returns>0 - 255 之间的数字，如果遇到异常返回“0”</returns>
        public static byte ToByte(object value)
        {
            try
            {
                if (value == System.DBNull.Value)
                {
                    return 0;
                }
                else if (value == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToByte(value);
                }
            }
            catch
            {
                return 0;
            }
        }


        /// <summary>
        /// 16进制字符串转换byte[]
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        /// <summary>
        /// byte[]转换16进制字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().Trim().ToUpper();
        }
        #endregion

        #region << 数字类型转换 >>

        /// <summary>
        /// 确保将输入的参数转为 Integer 型数字
        /// </summary>
        /// <param name="value">-2,147,483,648 到 2,147,483,647；舍入小数部分。</param>
        /// <returns>-2,147,483,648 到 2,147,483,647 之间的数字，如果遇到异常返回“0”</returns>
        public static int ToInt(object value)
        {
            try
            {
                if (value == System.DBNull.Value)
                {
                    return 0;
                }
                else if (value == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(value);
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 将输入的参数转为 Integer 型可空数字
        /// </summary>
        /// <param name="value">-2,147,483,648 到 2,147,483,647；舍入小数部分。</param>
        /// <returns>-2,147,483,648 到 2,147,483,647 之间的数字，如果遇到异常返回“null”</returns>
        public static int? ToIntNull(object value)
        {
            try
            {
                if (value == System.DBNull.Value)
                {
                    return null;
                }
                else if (value == null)
                {
                    return null;
                }
                else
                {
                    return Convert.ToInt32(value);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 比较两个可空数字是否完全相等。如果两个值均为空返回 true；均不为空且值相等返回 true；其余情况均返回 false。
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool Equals(int? value1, int? value2)
        {
            if (value1 == null && value2 == null)
            {
                return true;
            }
            else if (value1 != null && value2 != null)
            {
                return value1 == value2;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 比较两个可空数字是否完全相等。如果两个值均为空返回 true；均不为空且值相等返回 true；其余情况均返回 false。
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool Equals(long? value1, long? value2)
        {
            if (value1 == null && value2 == null)
            {
                return true;
            }
            else if (value1 != null && value2 != null)
            {
                return value1 == value2;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 比较两个可空数字是否完全相等。如果两个值均为空返回 true；均不为空且值相等返回 true；其余情况均返回 false。
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool Equals(decimal? value1, decimal? value2)
        {
            if (value1 == null && value2 == null)
            {
                return true;
            }
            else if (value1 != null && value2 != null)
            {
                return value1 == value2;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 确保将输入的参数转为 Long 型数字
        /// </summary>
        /// <param name="value">-9,223,372,036,854,775,808 到 9,223,372,036,854,775,807；舍入小数部分。</param>
        /// <returns>-9,223,372,036,854,775,808 到 9,223,372,036,854,775,807 之间的数字，如果遇到异常返回“0”</returns>
        public static long ToLong(object value)
        {
            try
            {
                if (value == System.DBNull.Value)
                {
                    return 0;
                }
                else if (value == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt64(value);
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 将输入的参数转为 Long 型可空数字
        /// </summary>
        /// <param name="value">-9,223,372,036,854,775,808 到 9,223,372,036,854,775,807；舍入小数部分。</param>
        /// <returns>-9,223,372,036,854,775,808 到 9,223,372,036,854,775,807 之间的数字，如果遇到异常返回“null”</returns>
        public static long? ToLongNull(object value)
        {
            try
            {
                if (value == System.DBNull.Value)
                {
                    return null;
                }
                else if (value == null)
                {
                    return null;
                }
                else
                {
                    return Convert.ToInt64(value);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 确保将输入的参数转为 Decimal 型数字
        /// </summary>
        /// <param name="value">对于零变比数值，即无小数位数值，为 +/-79,228,162,514,264,337,593,543,950,335。对于具有 28 位小数位的数字，范围是 +/-7.9228162514264337593543950335。最小的可用非零数是 0.0000000000000000000000000001 (+/-1E-28)。</param>
        /// <returns>Decimal 型数字，如果遇到异常返回“0”</returns>
        public static decimal ToDecimal(object value)
        {
            try
            {
                if (value == System.DBNull.Value)
                {
                    return 0;
                }
                else if (value == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToDecimal(value);
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 将输入的参数转为 Decimal 型可空数字
        /// </summary>
        /// <param name="value">对于零变比数值，即无小数位数值，为 +/-79,228,162,514,264,337,593,543,950,335。对于具有 28 位小数位的数字，范围是 +/-7.9228162514264337593543950335。最小的可用非零数是 0.0000000000000000000000000001 (+/-1E-28)。</param>
        /// <returns>Decimal 型数字，如果遇到异常返回“null”</returns>
        public static decimal? ToDecimalNull(object value)
        {
            try
            {
                if (value == System.DBNull.Value)
                {
                    return null;
                }
                else if (value == null)
                {
                    return null;
                }
                else
                {
                    return Convert.ToDecimal(value);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将输入的可空日期转换为可空的整型日期(YYYYMMDD)。
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int? ToIntDateNull(DateTime? date)
        {
            if (date == null)
            {
                return null;
            }
            else
            {
                return date.Value.Year * 10000 + date.Value.Month * 100 + date.Value.Day;
            }
        }

        /// <summary>
        /// 将输入的可空时间转为可空的整型时间(hhmmss)。
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int? ToIntTimeNull(DateTime? time)
        {
            if (time == null)
            {
                return null;
            }
            else
            {
                return time.Value.Hour * 10000 + time.Value.Minute * 100 + time.Value.Second;
            }
        }

        /// <summary>
        /// 判断传入的对象是否可以转为数字
        /// </summary>
        /// <param name="value">数字、字符串或任一对象</param>
        /// <returns>如果是数字返回 True，否则返回 False</returns>
        public static bool IsNumeric(object value)
        {
            try
            {
                if (value == System.DBNull.Value)
                {
                    return false;
                }
                else if (value == null)
                {
                    return false;
                }
                else
                {
                    decimal decValue;
                    return decimal.TryParse(value.ToString(), out decValue);
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region << 随机数 >>

        private static Random ran;

        /// <summary>
        /// 获得一个非负随机数。
        /// </summary>
        /// <returns>产生的整型随机数</returns>
        public static int GetRnd()
        {
            if (ran == null)
            {
                ran = new Random();
            }
            return ran.Next();
        }

        /// <summary>
        /// 获得指定范围的随机数。
        /// </summary>
        /// <param name="minValue">随机数的下限（最小值）</param>
        /// <param name="maxValue">随机数的上限（最大值）</param>
        /// <returns>产生的整型随机数</returns>
        public static int GetRnd(int minValue, int maxValue)
        {
            if (ran == null)
            {
                ran = new Random();
            }
            return ran.Next(minValue, maxValue);
        }

        /// <summary>
        /// 获得随机字符串。
        /// </summary>
        /// <param name="enabledNumeric">产生的随机字符串中是否允许包含数字</param>
        /// <param name="enabledCapital">产生的随机字符串中是否允许包含大写字母</param>
        /// <param name="enabledLowercase">产生的随机字符串中是否允许包含小写字母</param>
        /// <param name="enabledInterpunction">产生的随机字符串中是否允许包含符号</param>
        /// <param name="length">需要产生的字符串的长度</param>
        /// <returns></returns>
        public static string GetRnd(bool enabledNumeric, bool enabledCapital, bool enabledLowercase, bool enabledInterpunction, int length)
        {
            if (enabledNumeric == false && enabledCapital == false && enabledLowercase == false && enabledInterpunction == false)
            {
                throw new Exception("输出的字符串至少需要包含一种格式的字符");
            }

            if (length < 1)
            {
                throw new Exception("输出的字符串长度不能小于 1");
            }

            List<char> lstChar = new List<char>();
            if (enabledNumeric)
            {
                lstChar.AddRange(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            }

            if (enabledCapital)
            {
                lstChar.AddRange(new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' });
            }

            if (enabledLowercase)
            {
                lstChar.AddRange(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' });
            }

            if (enabledInterpunction)
            {
                lstChar.AddRange(new char[] { '`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '=', '+', '[', ']', '{', '}', '\\', '|', ';', ':', '\'', '"', ',', '.', '<', '>', '/', '?' });
            }

            string strRnd = "";
            if (ran == null)
            {
                ran = new Random();
            }

            for (int intIndex = 0; intIndex < length; intIndex++)
            {
                strRnd += lstChar[ran.Next(0, lstChar.Count - 1)];
            }

            return strRnd;
        }

        #endregion

        #region << 数学运算 >>

        /// <summary>
        /// 将传入的各个数值相加并返回相加后的结果，如果全部参数均为 null 则返回 null。
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int? DoAdd(params int?[] values)
        {
            if (values != null && values.Length > 0)
            {
                int? value = null;
                for (int intIndex = 0; intIndex < values.Length; intIndex++)
                {
                    if (values[intIndex] != null)
                    {
                        if (value == null)
                        {
                            value = values[intIndex];
                        }
                        else
                        {
                            value += values[intIndex];
                        }
                    }
                }
                return value;
            }
            return null;
        }

        /// <summary>
        /// 将传入的各个数值相加并返回相加后的结果，如果全部参数均为 null 则返回 null。
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static decimal? DoAdd(params decimal?[] values)
        {
            if (values != null && values.Length > 0)
            {
                decimal? value = null;
                for (int intIndex = 0; intIndex < values.Length; intIndex++)
                {
                    if (values[intIndex] != null)
                    {
                        if (value == null)
                        {
                            value = values[intIndex];
                        }
                        else
                        {
                            value += values[intIndex];
                        }
                    }
                }
                return value;
            }
            return null;
        }

        /// <summary>
        /// 返回 value1 - value2 的结果。如果其中一个数据为 null 则返回 null。
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static decimal? DoSub(decimal? value1, decimal? value2)
        {
            return null;
        }

        /// <summary>
        /// 计算两数相除的结果。如果其中一个数据为 null 或者分母等于 0 则返回 null。
        /// </summary>
        /// <param name="numerator">分子</param>
        /// <param name="denominator">分母</param>
        /// <returns></returns>
        public static decimal? DoDiv(decimal? numerator, decimal? denominator)
        {
            if (numerator != null && denominator != null && denominator != 0)
            {
                return numerator / denominator;
            }
            return null;
        }

        #endregion

        #region << 日期时间 >>

        /// <summary>
        /// 将可空的整型日期时间转为可空的日期类型。dateNum 和 timeNum 必须均不为空方进行转换。
        /// </summary>
        /// <param name="dateNum">整型日期 YYYYMMDD</param>
        /// <param name="timeNum">整型时间 hhmmss</param>
        /// <returns></returns>
        public static DateTime? ToDateTime(object dateNum, object timeNum)
        {
            if (dateNum != null && dateNum != System.DBNull.Value && timeNum != null && timeNum != System.DBNull.Value)
            {
                int intDateNum = Function.ToInt(dateNum);
                int intTimeNum = Function.ToInt(timeNum);
                if (intDateNum > 10000 && intDateNum <= 99991231)
                {
                    return new DateTime(intDateNum / 10000, (intDateNum / 100) % 100, intDateNum % 100, intTimeNum / 10000, (intTimeNum / 100) % 100, intTimeNum % 100);
                }
            }
            return null;
        }

        #endregion

        #region << 其它函数 >>

        /// <summary>
        /// 确定被比较的类型是否从指定的基类型继承。
        /// </summary>
        /// <param name="comparisonType">被比较的类型</param>
        /// <param name="baseType">基类型</param>
        /// <returns>如果被比较的类型从指定的基类型继承则返回 true，否则返回 false。</returns>
        public static bool IsInheritableBaseType(Type comparisonType, Type baseType)
        {
            bool bolIsInheritable = false;

            IsInheritableBaseType(comparisonType, baseType, ref bolIsInheritable);

            return bolIsInheritable;
        }

        private static void IsInheritableBaseType(Type comparisonType, Type baseType, ref bool isInheritable)
        {
            if (comparisonType != null)
            {
                if (comparisonType.Equals(baseType))
                {
                    isInheritable = true;
                }
                else
                {
                    IsInheritableBaseType(comparisonType.BaseType, baseType, ref isInheritable);
                }
            }
        }

        /// <summary>
        /// 获取指定类型名的类型对象。
        /// </summary>
        /// <param name="typeName">类型名称，如“命名空间1.命名空间2.类名”</param>
        /// <returns>类型声明</returns>
        public static Type GetType(string typeName)
        {
            Type typ = null;
            typ = Type.GetType(typeName);

            string strNamespace = typeName;
            while (typ == null)
            {
                if (typeName.LastIndexOf(".") > 0)
                {
                    strNamespace = strNamespace.Substring(0, strNamespace.LastIndexOf("."));

                    typ = Type.GetType(typeName + ", " + strNamespace);
                }
                else
                {
                    break;
                }
            }
            return typ;
        }

        /// <summary>
        /// 在传入的字符串中搜索“***.***.***.***”格式的 IP 地址，并将搜索到的 IP 地址计算为整型数字
        /// </summary>
        /// <param name="ip">“***.***.***.***”格式的 IP 地址</param>
        /// <returns>整型数字表示的 IP 地址</returns>
        public static int IpToInt(string ip)
        {
            long intIp = 0;
            string strIp = System.Text.RegularExpressions.Regex.Match(ip, "\\d+\\.\\d+\\.\\d+\\.\\d+").Value;

            if (strIp.Length > 0)
            {
                string[] arrIp = strIp.Split(new Char[] { '.' });

                for (int intIndex = 0; intIndex < arrIp.Length; intIndex++)
                {
                    intIp += Convert.ToInt32(arrIp[intIndex]) * (long)System.Math.Pow(256, -(intIndex - 3));
                }
            }

            return (int)(intIp - 2147483648);
        }

        /// <summary>
        /// 将传入的 Int32 整型数字转为“***.***.***.***”格式的 IP 地址
        /// </summary>
        /// <param name="intIp">整型数字表示的 IP 地址</param>
        /// <returns>“***.***.***.***”格式的 IP 地址</returns>
        public static string IntToIp(int intIp)
        {
            long lonIp = intIp + 2147483648;

            byte bytIp1 = (byte)((lonIp / 16777216) % 256);
            byte bytIp2 = (byte)((lonIp / 65536) % 256);
            byte bytIp3 = (byte)((lonIp / 256) % 256);
            byte bytIp4 = (byte)(lonIp % 256);

            return bytIp1.ToString() + "." + bytIp2.ToString() + "." + bytIp3.ToString() + "." + bytIp4.ToString();
        }

        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="password">欲加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string PasswordEncrypt(string password)
        {
            string strCharKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string strPasswordKey = "";

            for (int intIndexKey = 1; intIndexKey <= 8; intIndexKey++)
            {
                strPasswordKey += strCharKey.Substring(GetRnd(0, strCharKey.Length - 1), 1);
            }

            System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider();
            byte[] bytInput = System.Text.Encoding.Default.GetBytes(password);
            des.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(strPasswordKey);
            des.IV = System.Text.ASCIIEncoding.ASCII.GetBytes(strPasswordKey);

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream((System.IO.Stream)ms, des.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
            cs.Write(bytInput, 0, bytInput.Length);
            cs.FlushFinalBlock();

            System.Text.StringBuilder sbRet = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                sbRet.AppendFormat("{0:X2}", b);
            }

            return sbRet.ToString() + strPasswordKey;
        }

        /// <summary>
        /// 字符串解密
        /// </summary>
        /// <param name="encryptPassword">加密过的字符串</param>
        /// <returns>解密后的字符串</returns>
        public static string PasswordDecrypt(string encryptPassword)
        {
            try
            {
                string strPasswordKey = encryptPassword.Substring(encryptPassword.Length - 8, 8);
                string strEncryptPassword = encryptPassword.Substring(0, encryptPassword.Length - 8);

                System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider();
                int intEncryptPasswordLength = strEncryptPassword.Length / 2;
                byte[] bytInput = new byte[intEncryptPasswordLength];

                for (int intIndex = 0; intIndex < intEncryptPasswordLength; intIndex++)
                {
                    bytInput[intIndex] = Convert.ToByte(strEncryptPassword.Substring(intIndex * 2, 2), 16);
                }

                des.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(strPasswordKey);
                des.IV = System.Text.ASCIIEncoding.ASCII.GetBytes(strPasswordKey);

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream((System.IO.Stream)ms, des.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
                cs.Write(bytInput, 0, bytInput.Length);
                cs.FlushFinalBlock();

                return System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="password">欲加密的字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>加密后的字符串</returns>
        public static string PasswordEncrypt(string password, string key)
        {
            System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider();
            byte[] bytInput = System.Text.Encoding.Default.GetBytes(password);
            des.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(key);
            des.IV = System.Text.ASCIIEncoding.ASCII.GetBytes(key);

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream((System.IO.Stream)ms, des.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
            cs.Write(bytInput, 0, bytInput.Length);
            cs.FlushFinalBlock();

            System.Text.StringBuilder sbRet = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                sbRet.AppendFormat("{0:X2}", b);
            }

            return sbRet.ToString();
        }

        /// <summary>
        /// 字符串解密
        /// </summary>
        /// <param name="encryptPassword">加密过的字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>解密后的字符串</returns>
        public static string PasswordDecrypt(string encryptPassword, string key)
        {
            try
            {
                System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider();
                int intEncryptPasswordLength = encryptPassword.Length / 2;
                byte[] bytInput = new byte[intEncryptPasswordLength];

                for (int intIndex = 0; intIndex < intEncryptPasswordLength; intIndex++)
                {
                    bytInput[intIndex] = Convert.ToByte(encryptPassword.Substring(intIndex * 2, 2), 16);
                }

                des.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(key);
                des.IV = System.Text.ASCIIEncoding.ASCII.GetBytes(key);

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream((System.IO.Stream)ms, des.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
                cs.Write(bytInput, 0, bytInput.Length);
                cs.FlushFinalBlock();

                return System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获取路径2相对于路径1的相对路径
        /// </summary>
        /// <param name="strPath1">路径1</param>
        /// <param name="strPath2">路径2</param>
        /// <returns>返回路径2相对于路径1的路径</returns>
        /// <example>
        /// string strPath = GetRelativePath(@"C:\WINDOWS\system32", @"C:\WINDOWS\system\*.*" );
        /// //strPath == @"..\system\*.*"
        /// </example>
        public static string GetRelativePath(string strPath1, string strPath2)
        {
            if (!strPath1.EndsWith("\\")) strPath1 += "\\";    //如果不是以"\"结尾的加上"\"
            int intIndex = -1, intPos = strPath1.IndexOf('\\');
            //以"\"为分界比较从开始处到第一个"\"处对两个地址进行比较,如果相同则扩展到
            //下一个"\"处;直到比较出不同或第一个地址的结尾.
            while (intPos >= 0)
            {
                intPos++;
                if (string.Compare(strPath1, 0, strPath2, 0, intPos, true) != 0) break;
                intIndex = intPos;
                intPos = strPath1.IndexOf('\\', intPos);
            }

            //如果从不是第一个"\"处开始有不同,则从最后一个发现有不同的"\"处开始将strPath2
            //的后面部分付值给自己,在strPath1的同一个位置开始望后计算每有一个"\"则在strPath2
            //的前面加上一个"..\"(经过转义后就是"..\\").
            if (intIndex >= 0)
            {
                strPath2 = strPath2.Substring(intIndex);
                intPos = strPath1.IndexOf("\\", intIndex);
                while (intPos >= 0)
                {
                    strPath2 = "..\\" + strPath2;
                    intPos = strPath1.IndexOf("\\", intPos + 1);
                }
            }
            //否则直接返回strPath2
            return strPath2;
        }

        #endregion
    }
}

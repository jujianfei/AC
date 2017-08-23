using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace 三湘源涵普Test
{
    public class MeterClass
    {
        //降源
        [DllImport("DLL7000.dll")]
        public static extern int SourceClear_Fun(int comPort);

        //升源过程
        [DllImport("DLL7000.dll")]  //, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)] //, EntryPoint = "OutPutData_gSub", CharSet = CharSet.Auto, SetLastError = false, CallingConvention = CallingConvention.StdCall)]
        public static extern int OutPutData_gSub(int pLngAdjust, int pLngSourceType, int ComPort, int pIntPhase, int pIntStatus, int pIntSequence, float pSngVoltage, float pSngCurrent, float pSngFrequency, int pStrIABC, int pIntIB, int pStrLC, int pIntWave, int pIntWaveTimes, int pStrUWave, int pStrIWave, ref DelayTime timedelay);

        [DllImport("DLL7000.dll")] //CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall
        public static extern int DSPOutput(int pLngAdjust, int ComPort, int pIntID, int pIntPhase, float pSngFrequency, float Ua, float Ub, float Uc, float Ia, float Ib, float Ic, float DUab, float DUac, float DUIa, float DUIb, float DUIc, int WaveType, ref DelayTime timedelay);

        //标准表设置及读取
        [DllImport("DLL7000.dll")] //, EntryPoint = "OperationHC_Fun", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int OperationHC_Fun(int sourceType, int comPort, int hcAddress, int commandID, int Data, ref Standard strData);

        [DllImport("DLL7000.dll")]
        public static extern int OperationHC_HC310X_Fun(int pLngSourceType, int pIntComPort, int IntHcAddress, int IntCommandID, int pIntPhaseWire, int pIntStatus, float pSngVoltage, float pSngCurrent, float pSngStandConst, ref float pSngRealConst, ref Standard pStrData);

        //[DllImport("DLL7000.dll")]
        //public static extern int OperationHC_HC3100_200_Fun(int pLngSourceType, int pIntComPort, int IntHcAddress, int IntCommandID, int pIntPhaseWire, int pIntStatus, float pSngVoltage, float pSngCurrent, float pSngStandConst, ref float pSngRealConst, ref Standard pStrData);

        //设置误差仪
        [DllImport("DLL7000.DLL")]
        public static extern int ErrorOrder_gFun(int comPort, int pintID, int hcAddress, int pIntTime, ref Answer strData);

        //设置圈数
        [DllImport("DLL7000.DLL")]
        public static extern int ErrorCircles_gFun(int comPort, int lIntAddress, int lIntCircles, int lLngPules, Single lSngMaxError, ref Answer strData, ref Pules strPules);

        //设置秒脉冲测试状态
        [DllImport("DLL7000.dll")]
        public static extern int Ini_SecPules(int comPort, int pIntID);

        //读取秒脉冲误差
        [DllImport("DLL7000.dll")]
        public static extern int Error_SecPules(int comPort, int lIntAddress, Single lSngPules, ref Answer strData);

        //谐波参数
        [DllImport("DLL7000.dll")]
        public static extern int SetDSPHarmonic(int pIntComPort, int pIntID, float WaveDegreeA, float WaveDegreeB, float WaveDegreeC, int WaveTimesA, int WaveTimesB, int WaveTimesC, int VoltageWaveRateA, int VoltageWaveRateB, int VoltageWaveRateC, int CurrentWaveRateA, int CurrentWaveRateB, int CurrentWaveRateC);

        [DllImport("ComPort.dll")]
        public static extern int OpenComm(ref SerialPort pComPort);

        [DllImport("ComPort.dll")]
        public static extern int SingleClear(long pComPort);

       

        //public aferror[] afer = new aferror[47];
        //[DllImport("GetError.dll")]
        //public static extern bool funGetPowerError(ref);

        public static short HCARange_gFun(short pIntID, float pSngCurrent)
        {
            //HC3100电流量程判别
            //pSngCurrent  电流值
            //pIntID  1-返回脉冲输出电流量程 2-返回显示电流量程
            float lSngCurrent;
            short lIntOutA, lIntDispA;
            lSngCurrent = pSngCurrent;
            if (lSngCurrent <= 0.12)
            {
                lIntOutA = 1;
                lIntDispA = 1;
            }
            else if (lSngCurrent <= 0.6)
            {
                lIntOutA = 5;
                lIntDispA = 1;
            }
            else if (lSngCurrent <= 1.2)
            {
                lIntOutA = 10;
                lIntDispA = 1;
            }
            else if (lSngCurrent <= 6)
            {
                lIntOutA = 5;
                lIntDispA = 10;
            }
            else if (lSngCurrent <= 12)
            {
                lIntOutA = 10;
                lIntDispA = 10;
            }
            else if (lSngCurrent <= 60)
            {
                lIntOutA = 5;
                lIntDispA = 100;
            }
            else
            {
                lIntOutA = 10;
                lIntDispA = 100;
            }
            if (pIntID == 1)
                return lIntOutA;
            return lIntDispA;
        }

        public static Single LCtoDegree_gFun(int pIntStatus, String pStrLC)
        {
            Single lSngResult;
            String lStrLC;
            if ((pStrLC.Contains("C")) || (pStrLC.Contains("L")))
            {
                lStrLC = pStrLC.Remove(pStrLC.Length - 1);
            }
            else
            {
                lStrLC = pStrLC;
            }
            lSngResult = ArcCos_lFun(Convert.ToSingle(lStrLC));
            if ((pIntStatus == 0) || (pIntStatus == 1))
            {
                if (pStrLC.Contains("C"))
                {
                    lSngResult = 360 - lSngResult;
                }
            }
            else
            {
                if (pStrLC.Contains("C"))
                {
                    lSngResult = 270 + lSngResult;
                }
                else
                {
                    lSngResult = 90 - lSngResult;
                }
            }
            if ((pIntStatus == 1) || (pIntStatus == 3))
            {
                if (lSngResult > 180)
                {
                    lSngResult = lSngResult - 180;
                }
                else
                {
                    lSngResult = lSngResult + 180;
                }
            }
            return lSngResult;
        }

        public static Single ArcCos_lFun(Single X)
        {
            Single lSngResult;
            double Y;
            if (X == 0)
            {
                lSngResult = 90;
            }
            else if (X == 1)
            {
                lSngResult = 0;
            }
            else
            {
                Y = Convert.ToDouble(X);
                lSngResult = Convert.ToSingle(Math.Atan(-Y / Math.Sqrt(-Y * Y + 1)) + 2 * Math.Atan(1));
                lSngResult = Convert.ToSingle(lSngResult * 180 / 3.1415926);
            }
            return lSngResult;
        }

        public struct Pules
        {
            public Int32 pIntSourceVersion;
            public Int32 pIntConstFactor;
            public Int32 pIntPulesConst;
            public Int32 pIntRoundConst;
        }

        public struct aferror
        {
            public Single Error;
        }

        public struct Answer
        {
            public Single SngAnswer;
            public Single SngTimes;
        }

        public struct SerialPort  //串行口定义 typedef
        {
            public int ComNo;
            public int Baud;
            public int Parity;
            public Byte DataBit;
            public int StopBits;
        }

        // ComNO As Long          '串口号
        // Baud As Long           '波特率
        //  Parity As Long        '检验位
        // DataBit As Byte    '数据位
        //StopBits As Long     '停止位
        //RTS As Long         '串口RTS
        //DTR As Long         '串口DTR

        public struct DelayTime
        {
            public Int32 SteadyTime;      //升源稳定时间。
            public Int32 AdjustTime;      //升源稳定后调整电压、电流赋值所用时间。
        }

        public struct Standard
        {
            public float RealVA;        //A相电压(V)                   
            public float RealVB;        //B相电压(V)
            public float RealVC;        //C相电压(V)
            public float RealAA;        //A相电流(A)
            public float RealAB;        //B相电流(A)
            public float RealAC;        //C相电流(A)
            public float RealPFA;       //A相功率因数
            public float RealPFB;       //B相功率因数
            public float RealPFC;       //C相功率因数
            public float RealWA;        //A相功率，标准表在有功电能输出状态时为有功率(W)，无功时为无功功率(var)
            public float RealWB;        //B相功率
            public float RealWC;        //C相功率
            public float RealPF;        //平均功率因数
            public float RealAngle;     //角度
            public float RealFrequency; //频率(Hz)
            
            //public float RealAngle1;
            //public float RealAngle2;
            //public float RealAngle3;
            //public float RealUab;
            //public float RealUac;

            public float RealPVA;       //总视在功率(VA)
            public float RealP;         //总视在有功功率(W)
            public float RealQ;         //总视在无功功率(var)
        }

        public static Double StandardConst_gFun(Int32 pIntAPules, Int32 pIntARange, Single pSngVoltage)
        {
            Single lSngKI, lSngKU;
            Double lDblHCConst;
            lSngKI = 1 * pIntAPules * pIntARange;
            if (pSngVoltage < 80)
            {
                lSngKU = 1 / 2;
            }
            else if (pSngVoltage < 160)
            {
                lSngKU = 1;
            }
            else if (pSngVoltage < 320)
            {
                lSngKU = 2;
            }
            else
            {
                lSngKU = 4;
            }
            lDblHCConst = 1800000000 / lSngKU / lSngKI;
            return lDblHCConst;
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Drives
{
    
    public enum STATEtype
    {
        Waiting,            //空
        ChannelBusy,        //通道繁忙
        MeasureOK,          //召测数据完成！
        SetOK,              //设置完成!
        SetLOSE,            //设置失败!
        SetTimeERR,         //时钟不正确要先校时
        ReceiveDataERR,     //返回数据格式错误！
        DriveERR,           //未找到设备！
        OutTimeERR,         //等待数据返回超时！
        LinkErr,            //连接错误！
        getSendDataERR,     //发送数据拼装错误！
        getSendFDataERR,    //发送数据前置机拼装错误！
        SaveErr,            //数据保存错误！
        ChannelErr,         //通道错误
        NoDataErr,          //无所查数据！
        ERR,                //通常错误！
        ReceiveCDOK         //接收的子设备回码
    }

    /// <summary>
    /// RTU接收数据返回状态
    /// </summary>
    public interface IRTUSendReciveStateImplement : ISendReceiveStateImplement
    {

    }
}

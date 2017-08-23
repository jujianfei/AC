using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Drives;

namespace AC.Base
{
    /// <summary>
    /// 设备功能实现接口。该接口附加在继承 Device 接口的设备上，通常用于对某一通用功能进行规范，例如查询某项数据，设置某些档案参数的接口等。
    /// 实现该接口的“功能实现”如果可以向用户提供搜索实现该功能的所有设备的功能，则可以添加 DriveImplementAttribute 属性。
    /// </summary>
    public interface IDeviceImplement
    {
        /// <summary>
        /// 应用程序框架。
        /// </summary>
        ApplicationClass Application { get; }

        /// <summary>
        /// 获取当前设备的对象来源集合。该集合内保存了搜索设备时同一批搜索的设备，保留该集合的引用为后续读取设备数据时能够以高效的批量方式将该集合内的数据一次性读取。
        /// </summary>
        IDeviceCollection Source { get; }

        /// <summary>
        /// 当前设备的类型。
        /// </summary>
        DeviceType DeviceType { get; }

        /// <summary>
        /// 重新载入设备档案。
        /// </summary>
        void Reload();

        /// <summary>
        /// 获设备编号。
        /// </summary>
        int DeviceId { get; }

        /// <summary>
        /// 获取或设置设备名称。
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 获取或设置设备快捷码。
        /// </summary>
        string NameShortcut { get; set; }

        /// <summary>
        /// 设备通讯地址
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// 获取或设置设备标识码、条形码、资产编号等。
        /// </summary>
        string Identifier { get; set; }

        /// <summary>
        /// 获取或设置设备状态。默认为“运行”。
        /// </summary>
        DeviceStateOptions State { get; set; }

        /// <summary>
        /// 获取或设置设备状态描述。用于对设备状态做进一步描述。
        /// </summary>
        string StateDescription { get; set; }

        /// <summary>
        /// 获取或设置设备排序序号。
        /// </summary>
        int OrdinalNumber { get; set; }

        /// <summary>
        /// 获取或设置设备安装位置经度。
        /// </summary>
        decimal Longitude { get; set; }

        /// <summary>
        /// 获取或设置设备安装位置纬度。
        /// </summary>
        decimal Latitude { get; set; }

        /// <summary>
        /// 获取当前设备16*16像素含设备状态的图标。
        /// </summary>
        /// <returns></returns>
        System.Drawing.Image GetIcon16();

        /// <summary>
        /// 获取当前设备32*32像素含设备状态的图标。
        /// </summary>
        /// <returns></returns>
        System.Drawing.Image GetIcon32();

        /// <summary>
        /// 获取当前设备 16*16 像素含设备状态的图标路径。
        /// </summary>
        /// <returns></returns>
        string GetIcon16Url();

        /// <summary>
        /// 获取当前设备 32*32 像素含设备状态的图标路径。
        /// </summary>
        /// <returns></returns>
        string GetIcon32Url();

        /// <summary>
        /// 保存设备档案。
        /// </summary>
        void Save();

        /// <summary>
        /// 删除当前设备。
        /// </summary>
        void Delete();

        /// <summary>
        /// 当新建下级设备后产生的事件。
        /// </summary>
        event DeviceCreatedEventHandler CreatedChildren;

        /// <summary>
        /// 当前设备的档案、属性信息更改后产生的事件。
        /// </summary>
        event DeviceUpdatedEventHandler Updated;

        /// <summary>
        /// 当前设备被删除后产生的事件。
        /// </summary>
        event DeviceDeletedEventHandler Deleted;

        /// <summary>
        /// 获取当前设备驱动对象。如果当前设备无驱动则返回 null。
        /// </summary>
        IDrive Drive { get; }

        /// <summary>
        /// 获取上级设备，如果返回 null 则表示当前设备是根节点设备(根节点设备通常为通道)。
        /// </summary>
        Device Parent { get; }

        /// <summary>
        /// 获取下级子设备集合。
        /// </summary>
        AC.Base.Device.DeviceChildrenCollection Children { get; }

        /// <summary>
        /// 获取查询设备数据时所使用的日期范围。
        /// </summary>
        DateRange DateRange { get; }

        /// <summary>
        /// 为后续的数据查询操作预先设置数据日期范围。
        /// </summary>
        /// <param name="dateRange"></param>
        void SetDateRange(DateRange dateRange);

        /// <summary>
        /// 为后续的数据查询操作预先设置数据日期。
        /// </summary>
        /// <param name="date"></param>
        void SetDateRange(DateTime date);

        /// <summary>
        /// 为后续的数据查询操作预先设置数据日期范围。
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        void SetDateRange(DateTime startDate, DateTime endDate);

        /// <summary>
        /// 获取设备所属的分类。
        /// </summary>
        AC.Base.Device.DeviceClassifyCollection Classifys { get; }

        /// <summary>
        /// 获取设备所有的属性。
        /// </summary>
        AC.Base.Device.DevicePropertyCollection Propertys { get; }
    }
}

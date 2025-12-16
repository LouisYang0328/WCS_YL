using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.ServiceModel;
using GZ.Modular.Log;
using GZ.Modular.Redis;
using GZ.Modular.Database;
using MongoDB.Driver;
using Dapper;
using System.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using GZ.DB.Entity.wcs_yl;
using GZ.Common.Data;
using System.Globalization;
using System.Data.Entity;
using GZ.Common.Code;
using GZ.DB.IRepository.wcs_yl;
using GZ.DB.Repository.wcs_yl;
namespace GZ.Projects.WCS_YL
{
public class ObjectConvert:IMultiValueConverter
{
#region    [自定义类][20250516191646769][ObjectConvert]
	        #region IMultiValueConverter Members

        public static object ConverterObject;

        public object Convert(object[] values, Type targetType,
                              object parameter, System.Globalization.CultureInfo culture)
        {
            ConverterObject = values;
            string str = values.GetType().ToString();
            return values.ToArray();
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
                                    object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
#endregion [自定义类][20250516191646769][ObjectConvert]
}
public class DictItem
{
#region    [自定义类][20250516191710466][DictItem]
	        public string DictName { get; set; }
        public string DictValue { get; set; }
#endregion [自定义类][20250516191710466][DictItem]
}
public class PlcLogEntity
{
#region    [自定义类][20250517205715896][PlcLogEntity]
	        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string tag_id { get; set; }
        public int position { get; set; }
        public string old_value { get; set; }
        public string new_value { get; set; }
        public DateTime timestamp { get; set; }
        public double timespan { get; set; }
        public int next { get; set; }
        public int flag { get; set; }
#endregion [自定义类][20250517205715896][PlcLogEntity]
}
public class DeviceErrorRecordS
{
#region    [自定义类][20220604214903908][DeviceErrorRecordS]
	public string ErrorType { get; set; }		
public string DeviceType { get; set; }		
public string DeviceNo { get; set; }		
public string DeviceTaskNo { get; set; }		
public string ErrorCode { get; set; }		
public string ErrorDesc { get; set; }		
public DateTime? StartTime { get; set; }	
public DateTime? EndTime { get; set; }
public string ErrorState { get; set; }
public int ErrorCount { get; set; }		

#endregion [自定义类][20220604214903908][DeviceErrorRecordS]
}
public class DeviceErrorState_Convert:IValueConverter
{
#region    [自定义类][20220604214723219][DeviceErrorState_Convert]
	//故障状态
public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
{
	if (value != null)
	{
		string index = value.ToString();
		switch (index)
		{
			case "Error":
				return "故障中";
			case "OK":
				return "已解决";
			default:
				return index;
		}
	}
	else
	{
		throw new ArgumentNullException("value can not be null");
	}
}

public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
{
	throw new NotImplementedException();
}
#endregion [自定义类][20220604214723219][DeviceErrorState_Convert]
}
public class PasswordVisibilityConverter:IMultiValueConverter
{
#region    [自定义类][20250903130817783][PasswordVisibilityConverter]
	        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) return string.Empty;

            string password = values[0] as string;
            bool canShow = values[1].ToJson() == "是" ? true : false;

            // 有权限显示明文，无权限显示 ***
            return canShow ? password : "***";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // 编辑时走 CellEditingTemplate，这里无需处理
            throw new NotImplementedException();
        }
#endregion [自定义类][20250903130817783][PasswordVisibilityConverter]
}
public class TaskState_Convert:IValueConverter
{
#region    [自定义类][20250903132422798][TaskState_Convert]
	        //任务状态
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string index = value.ToString();
                switch (index)
                {
                    //Create 已创建
                    case "1":
                    case "Create":
                        return "已创建";

                    //Pre 待下发
                    case "2":
                    case "Pre":
                        return "待下发";

                    //Send 已下发
                    case "3":
                    case "Send":
                        return "已下发";

                    //Doing 执行中
                    case "4":
                    case "Doing":
                        return "执行中";

                    //Finish 执行完成
                    case "5":
                    case "Finish":
                        return "执行完成";
                    case "6":
                        return "订单取消中";
                    case "7":
                        return "订单取消完成";
                    case "8":
                        return "订单手动完成中";
                    case "10":
                        return "订单手动完成";
                    //Error 执行异常
                    case "9":
                    case "Error":
                        return "执行异常";

                    //Cancel 取消
                    case "0":
                    case "Cancel":
                        return "取消";

                    default:
                        return index;
                }
            }
            else
            {

                return value;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
#endregion [自定义类][20250903132422798][TaskState_Convert]
}
public class TaskType_Convert:IValueConverter
{
#region    [自定义类][20250903132628887][TaskType_Convert]
	        //任务类型
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                //任务细分类型（无细分时，等于单据类型），项目定制，例：[CH=出库需换箱，CN=出库不需换箱]、[YA=出库移库，YM=手动移库，YE=闲时移库]
                string index = value.ToString();
                switch (index)
                {
                    case "1":
                        return "物料入库";
                    case "2":
                        return "物料出库";
                    case "3":
                        return "托盘组入库";
                    case "4":
                        return "托盘组出库";
                    case "5":
                        return "不进库位，左右移动";
                    case "6":
                        return "库内移库";
                    case "7":
                        return "烘胶移库";
                    default:
                        return index;
                }
            }
            else
            {

                return value;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
#endregion [自定义类][20250903132628887][TaskType_Convert]
}
public class StackerRunMoudle_Convert:IValueConverter
{
#region    [自定义类][20250903132811643][StackerRunMoudle_Convert]
	        //堆垛机运行模式
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                int index = System.Convert.ToInt32(value);
                switch (index)
                {
                    //1 = 双叉同时运行；2 = 双叉分离运行；3 = 左叉运行；4 = 右叉运行
                    case 1:
                        return "双叉同时运行";
                    case 2:
                        return "双叉分离运行";
                    case 3:
                        return "左叉运行";
                    case 4:
                        return "右叉运行";
                    default:
                        return "";
                }
            }
            else
            {

                return value;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
#endregion [自定义类][20250903132811643][StackerRunMoudle_Convert]
}
public class station_Name_Convert:IValueConverter
{
#region    [自定义类][20250903132929311][station_Name_Convert]
	        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string index = value.ToString();
                switch (index)
                {
                    case "KQ_InStation1":
                        return "入库工位1";
                    case "KQ_InStation2":
                        return "入库工位2";
                    case "KQ_InStation3":
                        return "入库工位3";
                    case "KQ_InStation4":
                        return "入库工位4";
                    case "KQ_InStation5":
                        return "入库工位5";
                    case "KQ_OutStation6":
                        return "出库工位6";
                    case "KQ_PullCache":
                        return "拆盘机缓存位";
                    case "KQ_InWeight1":
                        return "入库称重位1";
                    case "KQ_InWeight2":
                        return "入库称重位2";
                    case "KQ_InWeight3":
                        return "入库称重位3";
                    case "KQ_InWeight4":
                        return "入库称重位4";
                    case "KQ_InWeight5":
                        return "入库称重位5";
                    case "KQ_InCheck1":
                        return "1#巷道入库复核口";
                    case "KQ_InCheck2":
                        return "2#巷道入库复核口";
                    case "KQ_InCheck3":
                        return "3#巷道入库复核口";
                    case "KQ_InCheck4":
                        return "4#巷道入库复核口";
                    case "KQ_InCheck5":
                        return "5#巷道入库复核口";
                    case "KH_InCheck1":
                        return "1#巷道回库复核口";
                    case "KH_InCheck2":
                        return "2#巷道回库复核口";
                    case "KH_InCheck3":
                        return "3#巷道回库复核口";
                    case "KH_InCheck4":
                        return "4#巷道回库复核口";
                    case "KH_InCheck5":
                        return "5#巷道回库复核口";
                    case "KH_PutCache":
                        return "码盘机工位";
                    case "KH_OutSort":
                        return "二层出库分拣口";
                    case "KH_InSort":
                        return "二层回库分拣口";
                    case "F2_OutStation":
                        return "二层出库站台";
                    case "F2_BackStation":
                        return "二层回库站台";
                    case "F2_LiftStation":
                        return "二层提升机接驳站台";
                    case "F1_LiftStationPos":
                        return "一层提升机接驳站台-正极";
                    case "F1_LiftStationNeg":
                        return "一层提升机接驳站台-负极";
                    case "F1_EndStationPos":
                        return "箔材出库终点站台-正极";
                    case "F1_EndStationNeg":
                        return "箔材出库终点站台-负极";
                    case "F3_OutStation":
                        return "三层出库站台";
                    case "F3_BackStation":
                        return "三层回库站台";
                    case "F3_EmptyCar1":
                        return "三层起点空车位1-AGV";
                    case "F3_FullCar1":
                        return "三层起点满车位1-AGV";
                    case "F3_EmptyCar2":
                        return "三层起点空车位2-AGV";
                    case "F3_FullCar2":
                        return "三层起点满车位2-AGV";
                    case "F3_EmptyStation1":
                        return "三层终点空车位1";
                    case "F3_EndStation1":
                        return "辅材出库终点站台2-1";
                    case "F3_EndStation2":
                        return "辅材出库终点站台2-2";
                    case "F4_OutStation":
                        return "四层出库站台";
                    case "F4_BackStation":
                        return "四层回库站台";
                    case "F4_LocationPos1":
                        return "正极粉体库位1";
                    case "F4_LocationPos2":
                        return "正极粉体库位2";
                    case "F4_LocationPos3":
                        return "正极粉体库位3";
                    case "F4_LocationPos4":
                        return "正极粉体库位4";
                    case "F4_LocationPos5":
                        return "正极粉体库位5";
                    case "F4_LocationPos6":
                        return "正极粉体库位6";
                    case "F4_LocationPos7":
                        return "正极粉体库位7";
                    case "F4_LocationPos8":
                        return "正极粉体库位8";
                    case "F4_LocationPos9":
                        return "正极粉体库位9";
                    case "F4_LocationPos10":
                        return "正极粉体库位10";
                    case "F4_LocationPos11":
                        return "正极粉体库位11";
                    case "F4_LocationPos12":
                        return "正极粉体库位12";
                    case "F4_LocationNeg1":
                        return "负极粉体库位1";
                    case "F4_LocationNeg2":
                        return "负极粉体库位2";
                    case "F4_LocationNeg3":
                        return "负极粉体库位3";
                    case "F4_LocationNeg4":
                        return "负极粉体库位4";
                    case "F4_LocationNeg5":
                        return "负极粉体库位5";
                    case "F4_LocationNeg6":
                        return "负极粉体库位6";
                    case "F4_LocationNeg7":
                        return "负极粉体库位7";
                    case "F4_LocationNeg8":
                        return "负极粉体库位8";
                    case "F4_LocationNeg9":
                        return "负极粉体库位9";
                    case "F4_LocationNeg10":
                        return "负极粉体库位10";
                    case "KQ_Out1":
                        return "1#巷道库前出库口";
                    case "KQ_Out2":
                        return "2#巷道库前出库口";
                    case "KQ_Out3":
                        return "3#巷道库前出库口";
                    case "KQ_Out4":
                        return "4#巷道库前出库口";
                    case "KQ_Out5":
                        return "5#巷道库前出库口";
                    case "KH_Out1":
                        return "1#巷道库后出库口";
                    case "KH_Out2":
                        return "2#巷道库后出库口";
                    case "KH_Out3":
                        return "3#巷道库后出库口";
                    case "KH_Out4":
                        return "4#巷道库后出库口";
                    case "KH_Out5":
                        return "5#巷道库后出库口";

                    default:
                        return index;
                }
            }
            else
            {

                return value;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
#endregion [自定义类][20250903132929311][station_Name_Convert]
}
public class Order_type_Convert:IValueConverter
{
#region    [自定义类][20250903140640515][Order_type_Convert]
	        //故障状态
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //R=入库，C=出库，Y=移库，D=调拨，C=抽检，T=退货，P=盘库
            if (value != null)
            {
                string index = value.ToString();
                switch (index)
                {
                    case "R":
                        return "入库";
                    case "C":
                        return "出库";
                    case "Y":
                        return "移库";
                    case "D":
                        return "调拨";
                    case "J":
                        return "抽检";
                    case "T":
                        return "退货";
                    case "P":
                        return "盘库";
                    default:
                        return index;
                }
            }
            else
            {

                return value;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
#endregion [自定义类][20250903140640515][Order_type_Convert]
}
public class AutoScanAPP
{
#region    [自定义类][20250905110811698][AutoScanAPP]
		        public bool MainMissionStackerUpdate(MainMissionEntity maentity, StackerTaskEntity stack, out string err)
        {
            err = "";
            try
            {
                err = "";

                using (var db = new RepositoryBase().BeginTrans())
                {
                    if (maentity != null)
                    {
                        db.Update(maentity);
                    }
                    if (stack != null)
                    {
                        db.Insert(stack);
                    }
                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                err = ex.ToString();
                return false;
            }

            return true;
        }
        public bool MainMissionUpdate(MainMissionEntity entity, out string err)
        {
            err = "";
            try
            {
                using (var db = new RepositoryBase().BeginTrans())
                {

                    if (entity != null)
                    {
                        db.Update(entity);
                    }
                };
            }
            catch (Exception ex)
            {
                err = ex.ToString();
                return false;
            }

            return true;

        }

        public bool MainMissionSyncFlagUpdate(MainMissionEntity mainEntity, SystemSynchronizationEntity sysEntity, out string err)
        {
            err = "";
            try
            {
                err = "";

                using (var db = new RepositoryBase().BeginTrans())
                {
                    if (mainEntity != null)
                    {
                        db.Update(mainEntity);
                    }
                    if (sysEntity != null)
                    {
                        db.Insert(sysEntity);
                    }
                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                err = ex.ToString();
                return false;
            }
            return true;
        }
        public bool MainMissionSyncFlagAdd(MainMissionEntity mainEntity, SystemSynchronizationEntity sysEntity, SystemSynchronizationEntity systemSynchronization, SystemSynchronizationEntity sysCanel, out string err)
        {
            err = "";
            try
            {
                err = "";

                using (var db = new RepositoryBase().BeginTrans())
                {
                    if (mainEntity != null)
                    {
                        db.Update(mainEntity);
                    }
                    if (sysEntity != null)
                    {
                        db.Insert(sysEntity);
                    }
                    if (systemSynchronization != null)
                    {
                        db.Insert(systemSynchronization);
                    }
                    if (sysCanel != null)
                    {
                        db.Insert(sysCanel);
                    }
                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                err = ex.ToString();
                return false;
            }
            return true;
        }
#endregion [自定义类][20250905110811698][AutoScanAPP]
}
}
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
	/// <summary>
	/// 变量数据库
	/// </summary>
	public class Tag : System.ComponentModel.INotifyPropertyChanged
	{
		#region 变量
		#region 实例化
        private static Tag tag;
        private static readonly object locker = new object();
        public static Tag CreateInstance()
        {
            if (tag == null)
            {
                lock (locker)
                {
					tag = new Tag();
                }
            }
			return tag;
        }
        private Tag()
        {

        }
		#endregion
		#region 内置变量
		private Dictionary<string, string> _Languages;
		public Dictionary<string,string> Languages
		{
            get
            {
				return _Languages;
            }
            set
            {
				_Languages = value;
				RaisePropertyChanged("Languages");
            }
        }

		private string _Lang;
		public string Lang
		{
			get
			{
				return _Lang;
			}
			set
			{
				_Lang = value;
				RaisePropertyChanged("Lang");
                try
				{
					if (_Languages.Any())
					{
						Application.Current.Resources.MergedDictionaries.Clear();
						Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(new StringBuilder().AppendFormat(@"资源/语言/{0}.xaml", _Lang).ToString(), UriKind.Relative) });
					}
				}
                catch(Exception ex)
                {
					MessageBox.Show("切换语言异常:" + ex.Message);
                }
			}
		}
		private System.String _ProjName = "WCS_YL";
		public System.String ProjName
		{
			get
			{

				return _ProjName;
			}
			set
			{
				_ProjName = value;
				RaisePropertyChanged("ProjName");
			}
		}
		private System.String _PlatformVer = "v1.0.2.0";
		public System.String PlatformVer
		{
			get
			{

				return _PlatformVer;
			}
			set
			{
				_PlatformVer = value;
				RaisePropertyChanged("PlatformVer");
			}
		}
		#endregion
		#endregion
		
		#region 变量组
		private TagUI _UI = new TagUI();
		public TagUI UI { get { return _UI; } set { _UI=value; RaisePropertyChanged("UI"); } }
		private Tag任务管理 _任务管理 = new Tag任务管理();
		public Tag任务管理 任务管理 { get { return _任务管理; } set { _任务管理=value; RaisePropertyChanged("任务管理"); } }
		private Tag用户权限 _用户权限 = new Tag用户权限();
		public Tag用户权限 用户权限 { get { return _用户权限; } set { _用户权限=value; RaisePropertyChanged("用户权限"); } }
		private Tag堆垛机报警 _堆垛机报警 = new Tag堆垛机报警();
		public Tag堆垛机报警 堆垛机报警 { get { return _堆垛机报警; } set { _堆垛机报警=value; RaisePropertyChanged("堆垛机报警"); } }
		private Tag报警统计 _报警统计 = new Tag报警统计();
		public Tag报警统计 报警统计 { get { return _报警统计; } set { _报警统计=value; RaisePropertyChanged("报警统计"); } }
		#endregion
		
		#region 内部类
			public class TagUI : System.ComponentModel.INotifyPropertyChanged
	{
		#region 变量
		#endregion
		
		#region 变量组
		private TagUI登录 _登录 = new TagUI登录();
		public TagUI登录 登录 { get { return _登录; } set { _登录=value; RaisePropertyChanged("登录"); } }
		private TagUI菜单按钮颜色 _菜单按钮颜色 = new TagUI菜单按钮颜色();
		public TagUI菜单按钮颜色 菜单按钮颜色 { get { return _菜单按钮颜色; } set { _菜单按钮颜色=value; RaisePropertyChanged("菜单按钮颜色"); } }
		private TagUI点位历史表更记录 _点位历史表更记录 = new TagUI点位历史表更记录();
		public TagUI点位历史表更记录 点位历史表更记录 { get { return _点位历史表更记录; } set { _点位历史表更记录=value; RaisePropertyChanged("点位历史表更记录"); } }
		private TagUI日志查询 _日志查询 = new TagUI日志查询();
		public TagUI日志查询 日志查询 { get { return _日志查询; } set { _日志查询=value; RaisePropertyChanged("日志查询"); } }
		private TagUI条码追溯 _条码追溯 = new TagUI条码追溯();
		public TagUI条码追溯 条码追溯 { get { return _条码追溯; } set { _条码追溯=value; RaisePropertyChanged("条码追溯"); } }
		#endregion
		
		#region 内部类
			public class TagUI登录 : System.ComponentModel.INotifyPropertyChanged
	{
		#region 变量
		private System.String _TXT_DeviceNo;
		public System.String TXT_DeviceNo
		{
			get 
			{
	return _TXT_DeviceNo; 
			}
			set 
			{
				_TXT_DeviceNo = value; 
				RaisePropertyChanged("TXT_DeviceNo"); 
			}
		}
		private System.String _TXT_DeviceName;
		public System.String TXT_DeviceName
		{
			get 
			{
	return _TXT_DeviceName; 
			}
			set 
			{
				_TXT_DeviceName = value; 
				RaisePropertyChanged("TXT_DeviceName"); 
			}
		}
		private System.String _登录人;
		public System.String 登录人
		{
			get 
			{
	return _登录人; 
			}
			set 
			{
				_登录人 = value; 
				RaisePropertyChanged("登录人"); 
			}
		}
		#endregion
		
		#region 变量组
		#endregion
		
		#region 内部类
		
		#endregion
			
		#region 属性变更
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}

	public class TagUI菜单按钮颜色 : System.ComponentModel.INotifyPropertyChanged
	{
		#region 变量
		private System.String _当前时间;
		public System.String 当前时间
		{
			get 
			{
	return _当前时间; 
			}
			set 
			{
				_当前时间 = value; 
				RaisePropertyChanged("当前时间"); 
			}
		}
		private System.String _首页="#007ACC";
		public System.String 首页
		{
			get 
			{
	return _首页; 
			}
			set 
			{
				_首页 = value; 
				RaisePropertyChanged("首页"); 
			}
		}
		private System.String _流程控制="#007ACC";
		public System.String 流程控制
		{
			get 
			{
	return _流程控制; 
			}
			set 
			{
				_流程控制 = value; 
				RaisePropertyChanged("流程控制"); 
			}
		}
		private System.String _退出="#007ACC";
		public System.String 退出
		{
			get 
			{
	return _退出; 
			}
			set 
			{
				_退出 = value; 
				RaisePropertyChanged("退出"); 
			}
		}
		private System.String _当前报警信息="无";
		public System.String 当前报警信息
		{
			get 
			{
	return _当前报警信息; 
			}
			set 
			{
				_当前报警信息 = value; 
				RaisePropertyChanged("当前报警信息"); 
			}
		}
		private System.String _任务管理="#007ACC";
		public System.String 任务管理
		{
			get 
			{
	return _任务管理; 
			}
			set 
			{
				_任务管理 = value; 
				RaisePropertyChanged("任务管理"); 
			}
		}
		private System.String _报警信息="#007ACC";
		public System.String 报警信息
		{
			get 
			{
	return _报警信息; 
			}
			set 
			{
				_报警信息 = value; 
				RaisePropertyChanged("报警信息"); 
			}
		}
		private System.String _日志查询="#007ACC";
		public System.String 日志查询
		{
			get 
			{
	return _日志查询; 
			}
			set 
			{
				_日志查询 = value; 
				RaisePropertyChanged("日志查询"); 
			}
		}
		private System.String _条码追溯="#007ACC";
		public System.String 条码追溯
		{
			get 
			{
	return _条码追溯; 
			}
			set 
			{
				_条码追溯 = value; 
				RaisePropertyChanged("条码追溯"); 
			}
		}
		private System.String _日志管理="#007ACC";
		public System.String 日志管理
		{
			get 
			{
	return _日志管理; 
			}
			set 
			{
				_日志管理 = value; 
				RaisePropertyChanged("日志管理"); 
			}
		}
		private System.String _点位历史表更记录="#007ACC";
		public System.String 点位历史表更记录
		{
			get 
			{
	return _点位历史表更记录; 
			}
			set 
			{
				_点位历史表更记录 = value; 
				RaisePropertyChanged("点位历史表更记录"); 
			}
		}
		private System.String _主任务管理="#007ACC";
		public System.String 主任务管理
		{
			get 
			{
	return _主任务管理; 
			}
			set 
			{
				_主任务管理 = value; 
				RaisePropertyChanged("主任务管理"); 
			}
		}
		private System.String _堆垛机任务管理="#007ACC";
		public System.String 堆垛机任务管理
		{
			get 
			{
	return _堆垛机任务管理; 
			}
			set 
			{
				_堆垛机任务管理 = value; 
				RaisePropertyChanged("堆垛机任务管理"); 
			}
		}
		private System.String _系统管理="#007ACC";
		public System.String 系统管理
		{
			get 
			{
	return _系统管理; 
			}
			set 
			{
				_系统管理 = value; 
				RaisePropertyChanged("系统管理"); 
			}
		}
		private System.String _自定义流程="#007ACC";
		public System.String 自定义流程
		{
			get 
			{
	return _自定义流程; 
			}
			set 
			{
				_自定义流程 = value; 
				RaisePropertyChanged("自定义流程"); 
			}
		}
		private System.String _堆垛机报警="#007ACC";
		public System.String 堆垛机报警
		{
			get 
			{
	return _堆垛机报警; 
			}
			set 
			{
				_堆垛机报警 = value; 
				RaisePropertyChanged("堆垛机报警"); 
			}
		}
		private System.String _用户信息="#007ACC";
		public System.String 用户信息
		{
			get 
			{
	return _用户信息; 
			}
			set 
			{
				_用户信息 = value; 
				RaisePropertyChanged("用户信息"); 
			}
		}
		private System.String _报警统计="#007ACC";
		public System.String 报警统计
		{
			get 
			{
	return _报警统计; 
			}
			set 
			{
				_报警统计 = value; 
				RaisePropertyChanged("报警统计"); 
			}
		}
		private System.String _设备信号="#007ACC";
		public System.String 设备信号
		{
			get 
			{
	return _设备信号; 
			}
			set 
			{
				_设备信号 = value; 
				RaisePropertyChanged("设备信号"); 
			}
		}
		private System.String _PLCTC01="#007ACC";
		public System.String PLCTC01
		{
			get 
			{
	return _PLCTC01; 
			}
			set 
			{
				_PLCTC01 = value; 
				RaisePropertyChanged("PLCTC01"); 
			}
		}
		private System.String _PLCTC02="#007ACC";
		public System.String PLCTC02
		{
			get 
			{
	return _PLCTC02; 
			}
			set 
			{
				_PLCTC02 = value; 
				RaisePropertyChanged("PLCTC02"); 
			}
		}
		private System.String _PLCTC03="#007ACC";
		public System.String PLCTC03
		{
			get 
			{
	return _PLCTC03; 
			}
			set 
			{
				_PLCTC03 = value; 
				RaisePropertyChanged("PLCTC03"); 
			}
		}
		private System.String _PLCCV01="#007ACC";
		public System.String PLCCV01
		{
			get 
			{
	return _PLCCV01; 
			}
			set 
			{
				_PLCCV01 = value; 
				RaisePropertyChanged("PLCCV01"); 
			}
		}
		private System.String _PLCCV02="#007ACC";
		public System.String PLCCV02
		{
			get 
			{
	return _PLCCV02; 
			}
			set 
			{
				_PLCCV02 = value; 
				RaisePropertyChanged("PLCCV02"); 
			}
		}
		private System.String _PLCCV03="#007ACC";
		public System.String PLCCV03
		{
			get 
			{
	return _PLCCV03; 
			}
			set 
			{
				_PLCCV03 = value; 
				RaisePropertyChanged("PLCCV03"); 
			}
		}
		private System.String _MEM="#007ACC";
		public System.String MEM
		{
			get 
			{
	return _MEM; 
			}
			set 
			{
				_MEM = value; 
				RaisePropertyChanged("MEM"); 
			}
		}
		#endregion
		
		#region 变量组
		#endregion
		
		#region 内部类
		
		#endregion
			
		#region 属性变更
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}

	public class TagUI点位历史表更记录 : System.ComponentModel.INotifyPropertyChanged
	{
		#region 变量
		private System.Object _List_Field;
		public System.Object List_Field
		{
			get 
			{
	return _List_Field; 
			}
			set 
			{
				_List_Field = value; 
				RaisePropertyChanged("List_Field"); 
			}
		}
		private System.String _Text_Field;
		public System.String Text_Field
		{
			get 
			{
	return _Text_Field; 
			}
			set 
			{
				_Text_Field = value; 
				RaisePropertyChanged("Text_Field"); 
			}
		}
		private System.String _Text_QueryConditions;
		public System.String Text_QueryConditions
		{
			get 
			{
	return _Text_QueryConditions; 
			}
			set 
			{
				_Text_QueryConditions = value; 
				RaisePropertyChanged("Text_QueryConditions"); 
			}
		}
		private System.Int32 _Page_Size=100;
		public System.Int32 Page_Size
		{
			get 
			{
	return _Page_Size; 
			}
			set 
			{
				_Page_Size = value; 
				RaisePropertyChanged("Page_Size"); 
			}
		}
		private System.String _Page_Message;
		public System.String Page_Message
		{
			get 
			{
	return _Page_Message; 
			}
			set 
			{
				_Page_Message = value; 
				RaisePropertyChanged("Page_Message"); 
			}
		}
		private System.Int32 _Page_Number=1;
		public System.Int32 Page_Number
		{
			get 
			{
	return _Page_Number; 
			}
			set 
			{
				_Page_Number = value; 
				RaisePropertyChanged("Page_Number"); 
			}
		}
		private System.String _TXT_StartTime;
		public System.String TXT_StartTime
		{
			get 
			{
	return _TXT_StartTime; 
			}
			set 
			{
				_TXT_StartTime = value; 
				RaisePropertyChanged("TXT_StartTime"); 
			}
		}
		private System.String _TXT_EndTime;
		public System.String TXT_EndTime
		{
			get 
			{
	return _TXT_EndTime; 
			}
			set 
			{
				_TXT_EndTime = value; 
				RaisePropertyChanged("TXT_EndTime"); 
			}
		}
		private System.Object _List_HistPlcLogs;
		public System.Object List_HistPlcLogs
		{
			get 
			{
	return _List_HistPlcLogs; 
			}
			set 
			{
				_List_HistPlcLogs = value; 
				RaisePropertyChanged("List_HistPlcLogs"); 
			}
		}
		private System.Int32 _TotalPages=1;
		public System.Int32 TotalPages
		{
			get 
			{
	return _TotalPages; 
			}
			set 
			{
				_TotalPages = value; 
				RaisePropertyChanged("TotalPages"); 
			}
		}
		private System.Object _List_TableType;
		public System.Object List_TableType
		{
			get 
			{
	return _List_TableType; 
			}
			set 
			{
				_List_TableType = value; 
				RaisePropertyChanged("List_TableType"); 
			}
		}
		private System.String _Text_TableType;
		public System.String Text_TableType
		{
			get 
			{
	return _Text_TableType; 
			}
			set 
			{
				_Text_TableType = value; 
				RaisePropertyChanged("Text_TableType"); 
			}
		}
		#endregion
		
		#region 变量组
		#endregion
		
		#region 内部类
		
		#endregion
			
		#region 属性变更
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}

	public class TagUI日志查询 : System.ComponentModel.INotifyPropertyChanged
	{
		#region 变量
		private System.Object _List_Field;
		public System.Object List_Field
		{
			get 
			{
	return _List_Field; 
			}
			set 
			{
				_List_Field = value; 
				RaisePropertyChanged("List_Field"); 
			}
		}
		private System.String _Text_Field;
		public System.String Text_Field
		{
			get 
			{
	return _Text_Field; 
			}
			set 
			{
				_Text_Field = value; 
				RaisePropertyChanged("Text_Field"); 
			}
		}
		private System.String _Text_QueryConditions;
		public System.String Text_QueryConditions
		{
			get 
			{
	return _Text_QueryConditions; 
			}
			set 
			{
				_Text_QueryConditions = value; 
				RaisePropertyChanged("Text_QueryConditions"); 
			}
		}
		private System.Int32 _Page_Size=100;
		public System.Int32 Page_Size
		{
			get 
			{
	return _Page_Size; 
			}
			set 
			{
				_Page_Size = value; 
				RaisePropertyChanged("Page_Size"); 
			}
		}
		private System.String _Page_Message;
		public System.String Page_Message
		{
			get 
			{
	return _Page_Message; 
			}
			set 
			{
				_Page_Message = value; 
				RaisePropertyChanged("Page_Message"); 
			}
		}
		private System.Int32 _Page_Number=1;
		public System.Int32 Page_Number
		{
			get 
			{
	return _Page_Number; 
			}
			set 
			{
				_Page_Number = value; 
				RaisePropertyChanged("Page_Number"); 
			}
		}
		private System.String _TXT_StartTime;
		public System.String TXT_StartTime
		{
			get 
			{
	return _TXT_StartTime; 
			}
			set 
			{
				_TXT_StartTime = value; 
				RaisePropertyChanged("TXT_StartTime"); 
			}
		}
		private System.String _TXT_EndTime;
		public System.String TXT_EndTime
		{
			get 
			{
	return _TXT_EndTime; 
			}
			set 
			{
				_TXT_EndTime = value; 
				RaisePropertyChanged("TXT_EndTime"); 
			}
		}
		private System.Object _List_LintLogs;
		public System.Object List_LintLogs
		{
			get 
			{
	return _List_LintLogs; 
			}
			set 
			{
				_List_LintLogs = value; 
				RaisePropertyChanged("List_LintLogs"); 
			}
		}
		private System.Int32 _TotalPages;
		public System.Int32 TotalPages
		{
			get 
			{
	return _TotalPages; 
			}
			set 
			{
				_TotalPages = value; 
				RaisePropertyChanged("TotalPages"); 
			}
		}
		private System.Object _List_TableType;
		public System.Object List_TableType
		{
			get 
			{
	return _List_TableType; 
			}
			set 
			{
				_List_TableType = value; 
				RaisePropertyChanged("List_TableType"); 
			}
		}
		private System.String _Text_TableType;
		public System.String Text_TableType
		{
			get 
			{
	return _Text_TableType; 
			}
			set 
			{
				_Text_TableType = value; 
				RaisePropertyChanged("Text_TableType"); 
			}
		}
		private System.Object _List_LogType;
		public System.Object List_LogType
		{
			get 
			{
	return _List_LogType; 
			}
			set 
			{
				_List_LogType = value; 
				RaisePropertyChanged("List_LogType"); 
			}
		}
		private System.String _Text_LogType;
		public System.String Text_LogType
		{
			get 
			{
	return _Text_LogType; 
			}
			set 
			{
				_Text_LogType = value; 
				RaisePropertyChanged("Text_LogType"); 
			}
		}
		#endregion
		
		#region 变量组
		#endregion
		
		#region 内部类
		
		#endregion
			
		#region 属性变更
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}

	public class TagUI条码追溯 : System.ComponentModel.INotifyPropertyChanged
	{
		#region 变量
		private System.String _Text_QueryConditions;
		public System.String Text_QueryConditions
		{
			get 
			{
	return _Text_QueryConditions; 
			}
			set 
			{
				_Text_QueryConditions = value; 
				RaisePropertyChanged("Text_QueryConditions"); 
			}
		}
		private System.String _Text_Massage;
		public System.String Text_Massage
		{
			get 
			{
	return _Text_Massage; 
			}
			set 
			{
				_Text_Massage = value; 
				RaisePropertyChanged("Text_Massage"); 
			}
		}
		#endregion
		
		#region 变量组
		#endregion
		
		#region 内部类
		
		#endregion
			
		#region 属性变更
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}


		#endregion
			
		#region 属性变更
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}

	public class Tag任务管理 : System.ComponentModel.INotifyPropertyChanged
	{
		#region 变量
		#endregion
		
		#region 变量组
		private Tag任务管理主任务管理 _主任务管理 = new Tag任务管理主任务管理();
		public Tag任务管理主任务管理 主任务管理 { get { return _主任务管理; } set { _主任务管理=value; RaisePropertyChanged("主任务管理"); } }
		private Tag任务管理堆垛机任务管理 _堆垛机任务管理 = new Tag任务管理堆垛机任务管理();
		public Tag任务管理堆垛机任务管理 堆垛机任务管理 { get { return _堆垛机任务管理; } set { _堆垛机任务管理=value; RaisePropertyChanged("堆垛机任务管理"); } }
		#endregion
		
		#region 内部类
			public class Tag任务管理主任务管理 : System.ComponentModel.INotifyPropertyChanged
	{
		#region 变量
		private System.String _TXT_material_no;
		public System.String TXT_material_no
		{
			get 
			{
	return _TXT_material_no; 
			}
			set 
			{
				_TXT_material_no = value; 
				RaisePropertyChanged("TXT_material_no"); 
			}
		}
		private System.String _TXT_material_name;
		public System.String TXT_material_name
		{
			get 
			{
	return _TXT_material_name; 
			}
			set 
			{
				_TXT_material_name = value; 
				RaisePropertyChanged("TXT_material_name"); 
			}
		}
		private System.String _TXT_order_no;
		public System.String TXT_order_no
		{
			get 
			{
	return _TXT_order_no; 
			}
			set 
			{
				_TXT_order_no = value; 
				RaisePropertyChanged("TXT_order_no"); 
			}
		}
		private System.String _TXT_task_no;
		public System.String TXT_task_no
		{
			get 
			{
	return _TXT_task_no; 
			}
			set 
			{
				_TXT_task_no = value; 
				RaisePropertyChanged("TXT_task_no"); 
			}
		}
		private System.Object _List_MissionState;
		public System.Object List_MissionState
		{
			get 
			{
	return _List_MissionState; 
			}
			set 
			{
				_List_MissionState = value; 
				RaisePropertyChanged("List_MissionState"); 
			}
		}
		private System.String _TXT_MissionState;
		public System.String TXT_MissionState
		{
			get 
			{
	return _TXT_MissionState; 
			}
			set 
			{
				_TXT_MissionState = value; 
				RaisePropertyChanged("TXT_MissionState"); 
			}
		}
		private System.Object _List_order_type;
		public System.Object List_order_type
		{
			get 
			{
	return _List_order_type; 
			}
			set 
			{
				_List_order_type = value; 
				RaisePropertyChanged("List_order_type"); 
			}
		}
		private System.String _TXT_order_type;
		public System.String TXT_order_type
		{
			get 
			{
	return _TXT_order_type; 
			}
			set 
			{
				_TXT_order_type = value; 
				RaisePropertyChanged("TXT_order_type"); 
			}
		}
		private System.Object _List_task_type;
		public System.Object List_task_type
		{
			get 
			{
	return _List_task_type; 
			}
			set 
			{
				_List_task_type = value; 
				RaisePropertyChanged("List_task_type"); 
			}
		}
		private System.String _TXT_task_type;
		public System.String TXT_task_type
		{
			get 
			{
	return _TXT_task_type; 
			}
			set 
			{
				_TXT_task_type = value; 
				RaisePropertyChanged("TXT_task_type"); 
			}
		}
		private System.Object _List_tunnel_no;
		public System.Object List_tunnel_no
		{
			get 
			{
	return _List_tunnel_no; 
			}
			set 
			{
				_List_tunnel_no = value; 
				RaisePropertyChanged("List_tunnel_no"); 
			}
		}
		private System.String _TXT_tunnel_no;
		public System.String TXT_tunnel_no
		{
			get 
			{
	return _TXT_tunnel_no; 
			}
			set 
			{
				_TXT_tunnel_no = value; 
				RaisePropertyChanged("TXT_tunnel_no"); 
			}
		}
		private System.String _TXT_start_position;
		public System.String TXT_start_position
		{
			get 
			{
	return _TXT_start_position; 
			}
			set 
			{
				_TXT_start_position = value; 
				RaisePropertyChanged("TXT_start_position"); 
			}
		}
		private System.String _TXT_end_position;
		public System.String TXT_end_position
		{
			get 
			{
	return _TXT_end_position; 
			}
			set 
			{
				_TXT_end_position = value; 
				RaisePropertyChanged("TXT_end_position"); 
			}
		}
		private System.String _TXT_StartTime;
		public System.String TXT_StartTime
		{
			get 
			{
	return _TXT_StartTime; 
			}
			set 
			{
				_TXT_StartTime = value; 
				RaisePropertyChanged("TXT_StartTime"); 
			}
		}
		private System.String _TXT_EndTime;
		public System.String TXT_EndTime
		{
			get 
			{
	return _TXT_EndTime; 
			}
			set 
			{
				_TXT_EndTime = value; 
				RaisePropertyChanged("TXT_EndTime"); 
			}
		}
		private System.Int32 _Page_Size=15;
		public System.Int32 Page_Size
		{
			get 
			{
	return _Page_Size; 
			}
			set 
			{
				_Page_Size = value; 
				RaisePropertyChanged("Page_Size"); 
			}
		}
		private System.String _Page_Message;
		public System.String Page_Message
		{
			get 
			{
	return _Page_Message; 
			}
			set 
			{
				_Page_Message = value; 
				RaisePropertyChanged("Page_Message"); 
			}
		}
		private System.Object _List_MainMission;
		public System.Object List_MainMission
		{
			get 
			{
	return _List_MainMission; 
			}
			set 
			{
				_List_MainMission = value; 
				RaisePropertyChanged("List_MainMission"); 
			}
		}
		private System.Int32 _Page_Number;
		public System.Int32 Page_Number
		{
			get 
			{
	return _Page_Number; 
			}
			set 
			{
				_Page_Number = value; 
				RaisePropertyChanged("Page_Number"); 
			}
		}
		private System.String _TXT_TrayCode;
		public System.String TXT_TrayCode
		{
			get 
			{
	return _TXT_TrayCode; 
			}
			set 
			{
				_TXT_TrayCode = value; 
				RaisePropertyChanged("TXT_TrayCode"); 
			}
		}
		#endregion
		
		#region 变量组
		#endregion
		
		#region 内部类
		
		#endregion
			
		#region 属性变更
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}

	public class Tag任务管理堆垛机任务管理 : System.ComponentModel.INotifyPropertyChanged
	{
		#region 变量
		private System.String _TXT_TaskNo;
		public System.String TXT_TaskNo
		{
			get 
			{
	return _TXT_TaskNo; 
			}
			set 
			{
				_TXT_TaskNo = value; 
				RaisePropertyChanged("TXT_TaskNo"); 
			}
		}
		private System.Object _List_DeviceNo;
		public System.Object List_DeviceNo
		{
			get 
			{
	return _List_DeviceNo; 
			}
			set 
			{
				_List_DeviceNo = value; 
				RaisePropertyChanged("List_DeviceNo"); 
			}
		}
		private System.String _TXT_DeviceNo;
		public System.String TXT_DeviceNo
		{
			get 
			{
	return _TXT_DeviceNo; 
			}
			set 
			{
				_TXT_DeviceNo = value; 
				RaisePropertyChanged("TXT_DeviceNo"); 
			}
		}
		private System.Object _List_TaskType;
		public System.Object List_TaskType
		{
			get 
			{
	return _List_TaskType; 
			}
			set 
			{
				_List_TaskType = value; 
				RaisePropertyChanged("List_TaskType"); 
			}
		}
		private System.String _TXT_TaskType;
		public System.String TXT_TaskType
		{
			get 
			{
	return _TXT_TaskType; 
			}
			set 
			{
				_TXT_TaskType = value; 
				RaisePropertyChanged("TXT_TaskType"); 
			}
		}
		private System.Object _List_TaskState;
		public System.Object List_TaskState
		{
			get 
			{
	return _List_TaskState; 
			}
			set 
			{
				_List_TaskState = value; 
				RaisePropertyChanged("List_TaskState"); 
			}
		}
		private System.String _TXT_TaskState;
		public System.String TXT_TaskState
		{
			get 
			{
	return _TXT_TaskState; 
			}
			set 
			{
				_TXT_TaskState = value; 
				RaisePropertyChanged("TXT_TaskState"); 
			}
		}
		private System.String _TXT_StartTime;
		public System.String TXT_StartTime
		{
			get 
			{
	return _TXT_StartTime; 
			}
			set 
			{
				_TXT_StartTime = value; 
				RaisePropertyChanged("TXT_StartTime"); 
			}
		}
		private System.String _TXT_EndTime;
		public System.String TXT_EndTime
		{
			get 
			{
	return _TXT_EndTime; 
			}
			set 
			{
				_TXT_EndTime = value; 
				RaisePropertyChanged("TXT_EndTime"); 
			}
		}
		private System.Object _List_StackerTask;
		public System.Object List_StackerTask
		{
			get 
			{
	return _List_StackerTask; 
			}
			set 
			{
				_List_StackerTask = value; 
				RaisePropertyChanged("List_StackerTask"); 
			}
		}
		private System.Int32 _Page_Number;
		public System.Int32 Page_Number
		{
			get 
			{
	return _Page_Number; 
			}
			set 
			{
				_Page_Number = value; 
				RaisePropertyChanged("Page_Number"); 
			}
		}
		private System.Int32 _Page_Size=15;
		public System.Int32 Page_Size
		{
			get 
			{
	return _Page_Size; 
			}
			set 
			{
				_Page_Size = value; 
				RaisePropertyChanged("Page_Size"); 
			}
		}
		private System.String _Page_Message;
		public System.String Page_Message
		{
			get 
			{
	return _Page_Message; 
			}
			set 
			{
				_Page_Message = value; 
				RaisePropertyChanged("Page_Message"); 
			}
		}
		private System.String _TXT_TrayCode;
		public System.String TXT_TrayCode
		{
			get 
			{
	return _TXT_TrayCode; 
			}
			set 
			{
				_TXT_TrayCode = value; 
				RaisePropertyChanged("TXT_TrayCode"); 
			}
		}
		#endregion
		
		#region 变量组
		#endregion
		
		#region 内部类
		
		#endregion
			
		#region 属性变更
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}


		#endregion
			
		#region 属性变更
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}

	public class Tag用户权限 : System.ComponentModel.INotifyPropertyChanged
	{
		#region 变量
		private System.Object _List_UserPermissions;
		public System.Object List_UserPermissions
		{
			get 
			{
	return _List_UserPermissions; 
			}
			set 
			{
				_List_UserPermissions = value; 
				RaisePropertyChanged("List_UserPermissions"); 
			}
		}
		private System.String _Page_Message;
		public System.String Page_Message
		{
			get 
			{
	return _Page_Message; 
			}
			set 
			{
				_Page_Message = value; 
				RaisePropertyChanged("Page_Message"); 
			}
		}
		private System.Int32 _Page_Number;
		public System.Int32 Page_Number
		{
			get 
			{
	return _Page_Number; 
			}
			set 
			{
				_Page_Number = value; 
				RaisePropertyChanged("Page_Number"); 
			}
		}
		private System.Int32 _Page_Size=15;
		public System.Int32 Page_Size
		{
			get 
			{
	return _Page_Size; 
			}
			set 
			{
				_Page_Size = value; 
				RaisePropertyChanged("Page_Size"); 
			}
		}
		private System.String _TXT_Total;
		public System.String TXT_Total
		{
			get 
			{
	return _TXT_Total; 
			}
			set 
			{
				_TXT_Total = value; 
				RaisePropertyChanged("TXT_Total"); 
			}
		}
		private System.String _TXT_PassWord;
		public System.String TXT_PassWord
		{
			get 
			{
	return _TXT_PassWord; 
			}
			set 
			{
				_TXT_PassWord = value; 
				RaisePropertyChanged("TXT_PassWord"); 
			}
		}
		private System.String _TXT_Name;
		public System.String TXT_Name
		{
			get 
			{
	return _TXT_Name; 
			}
			set 
			{
				_TXT_Name = value; 
				RaisePropertyChanged("TXT_Name"); 
			}
		}
		private System.Boolean _IsReadOnly;
		public System.Boolean IsReadOnly
		{
			get 
			{
	return _IsReadOnly; 
			}
			set 
			{
				_IsReadOnly = value; 
				RaisePropertyChanged("IsReadOnly"); 
			}
		}
		private System.String _User_Permissions;
		public System.String User_Permissions
		{
			get 
			{
	return _User_Permissions; 
			}
			set 
			{
				_User_Permissions = value; 
				RaisePropertyChanged("User_Permissions"); 
			}
		}
		private System.Boolean _Bool_Button;
		public System.Boolean Bool_Button
		{
			get 
			{
	return _Bool_Button; 
			}
			set 
			{
				_Bool_Button = value; 
				RaisePropertyChanged("Bool_Button"); 
			}
		}
		private System.String _Button="Visibility";
		public System.String Button
		{
			get 
			{
	return _Button; 
			}
			set 
			{
				_Button = value; 
				RaisePropertyChanged("Button"); 
			}
		}
		private System.Boolean _IsEnabled=false;
		public System.Boolean IsEnabled
		{
			get 
			{
	return _IsEnabled; 
			}
			set 
			{
				_IsEnabled = value; 
				RaisePropertyChanged("IsEnabled"); 
			}
		}
		#endregion
		
		#region 变量组
		#endregion
		
		#region 内部类
		
		#endregion
			
		#region 属性变更
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}

	public class Tag堆垛机报警 : System.ComponentModel.INotifyPropertyChanged
	{
		#region 变量
		private System.String _TXT_DeviceTaskNo;
		public System.String TXT_DeviceTaskNo
		{
			get 
			{
	return _TXT_DeviceTaskNo; 
			}
			set 
			{
				_TXT_DeviceTaskNo = value; 
				RaisePropertyChanged("TXT_DeviceTaskNo"); 
			}
		}
		private System.Object _List_DeviceNo;
		public System.Object List_DeviceNo
		{
			get 
			{
	return _List_DeviceNo; 
			}
			set 
			{
				_List_DeviceNo = value; 
				RaisePropertyChanged("List_DeviceNo"); 
			}
		}
		private System.String _TXT_DeviceNo;
		public System.String TXT_DeviceNo
		{
			get 
			{
	return _TXT_DeviceNo; 
			}
			set 
			{
				_TXT_DeviceNo = value; 
				RaisePropertyChanged("TXT_DeviceNo"); 
			}
		}
		private System.String _TXT_StartTime;
		public System.String TXT_StartTime
		{
			get 
			{
	return _TXT_StartTime; 
			}
			set 
			{
				_TXT_StartTime = value; 
				RaisePropertyChanged("TXT_StartTime"); 
			}
		}
		private System.String _TXT_EndTime;
		public System.String TXT_EndTime
		{
			get 
			{
	return _TXT_EndTime; 
			}
			set 
			{
				_TXT_EndTime = value; 
				RaisePropertyChanged("TXT_EndTime"); 
			}
		}
		private System.Object _List_StackerError;
		public System.Object List_StackerError
		{
			get 
			{
	return _List_StackerError; 
			}
			set 
			{
				_List_StackerError = value; 
				RaisePropertyChanged("List_StackerError"); 
			}
		}
		private System.Int32 _Page_Number;
		public System.Int32 Page_Number
		{
			get 
			{
	return _Page_Number; 
			}
			set 
			{
				_Page_Number = value; 
				RaisePropertyChanged("Page_Number"); 
			}
		}
		private System.Int32 _Page_Size;
		public System.Int32 Page_Size
		{
			get 
			{
	return _Page_Size; 
			}
			set 
			{
				_Page_Size = value; 
				RaisePropertyChanged("Page_Size"); 
			}
		}
		private System.String _Page_Message;
		public System.String Page_Message
		{
			get 
			{
	return _Page_Message; 
			}
			set 
			{
				_Page_Message = value; 
				RaisePropertyChanged("Page_Message"); 
			}
		}
		#endregion
		
		#region 变量组
		#endregion
		
		#region 内部类
		
		#endregion
			
		#region 属性变更
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}

	public class Tag报警统计 : System.ComponentModel.INotifyPropertyChanged
	{
		#region 变量
		private System.Object _List_DeviceNo;
		public System.Object List_DeviceNo
		{
			get 
			{
	return _List_DeviceNo; 
			}
			set 
			{
				_List_DeviceNo = value; 
				RaisePropertyChanged("List_DeviceNo"); 
			}
		}
		private System.String _TXT_DeviceNo;
		public System.String TXT_DeviceNo
		{
			get 
			{
	return _TXT_DeviceNo; 
			}
			set 
			{
				_TXT_DeviceNo = value; 
				RaisePropertyChanged("TXT_DeviceNo"); 
			}
		}
		private System.Object _List_ErrorCode;
		public System.Object List_ErrorCode
		{
			get 
			{
	return _List_ErrorCode; 
			}
			set 
			{
				_List_ErrorCode = value; 
				RaisePropertyChanged("List_ErrorCode"); 
			}
		}
		private System.String _TXT_ErrorCode;
		public System.String TXT_ErrorCode
		{
			get 
			{
	return _TXT_ErrorCode; 
			}
			set 
			{
				_TXT_ErrorCode = value; 
				RaisePropertyChanged("TXT_ErrorCode"); 
			}
		}
		private System.String _TXT_CountDate;
		public System.String TXT_CountDate
		{
			get 
			{
	return _TXT_CountDate; 
			}
			set 
			{
				_TXT_CountDate = value; 
				RaisePropertyChanged("TXT_CountDate"); 
			}
		}
		private System.Object _List_DeviceError;
		public System.Object List_DeviceError
		{
			get 
			{
	return _List_DeviceError; 
			}
			set 
			{
				_List_DeviceError = value; 
				RaisePropertyChanged("List_DeviceError"); 
			}
		}
		private System.Int32 _Page_Number;
		public System.Int32 Page_Number
		{
			get 
			{
	return _Page_Number; 
			}
			set 
			{
				_Page_Number = value; 
				RaisePropertyChanged("Page_Number"); 
			}
		}
		private System.Int32 _Page_Size;
		public System.Int32 Page_Size
		{
			get 
			{
	return _Page_Size; 
			}
			set 
			{
				_Page_Size = value; 
				RaisePropertyChanged("Page_Size"); 
			}
		}
		private System.String _Page_Message;
		public System.String Page_Message
		{
			get 
			{
	return _Page_Message; 
			}
			set 
			{
				_Page_Message = value; 
				RaisePropertyChanged("Page_Message"); 
			}
		}
		private System.String _TXT_DeviceNoL;
		public System.String TXT_DeviceNoL
		{
			get 
			{
	return _TXT_DeviceNoL; 
			}
			set 
			{
				_TXT_DeviceNoL = value; 
				RaisePropertyChanged("TXT_DeviceNoL"); 
			}
		}
		#endregion
		
		#region 变量组
		#endregion
		
		#region 内部类
		
		#endregion
			
		#region 属性变更
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}


		#endregion
			
		#region 属性变更
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion
	}

}

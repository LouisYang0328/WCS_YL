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
	#region 设备通信
	/// <summary>
	/// 设备通信
	/// </summary>
	public class Device
	{
		public GZ.Device.PLC.PlcBase MEM=null;
		public GZ.Device.PLC.PlcBase TC01=null;
		public GZ.Device.PLC.PlcBase TC02=null;
		public GZ.Device.PLC.PlcBase TC03=null;
		public GZ.Device.PLC.PlcBase CV01=null;
		public GZ.Device.PLC.PlcBase CV02=null;
		public GZ.Device.PLC.PlcBase CV03=null;
		private static Device device;
		private static readonly object locker = new object();
		public static Device CreateInstance()
        {
            if (device == null)
            {
                lock (locker)
				{
					device = new Device();
				}
            }
			return device;
        }
		private Device()
		{
			MEM = GZ.Device.PLC.PlcFactory.CreateInstanceByFile
			(
				System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "MEM"),
				null,null,null
			);
			TC01 = GZ.Device.PLC.PlcFactory.CreateInstanceByFile
			(
				System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "TC01"),
				null,null,null
			);
			TC02 = GZ.Device.PLC.PlcFactory.CreateInstanceByFile
			(
				System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "TC02"),
				null,null,null
			);
			TC03 = GZ.Device.PLC.PlcFactory.CreateInstanceByFile
			(
				System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "TC03"),
				null,null,null
			);
			CV01 = GZ.Device.PLC.PlcFactory.CreateInstanceByFile
			(
				System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "CV01"),
				null,null,null
			);
			CV02 = GZ.Device.PLC.PlcFactory.CreateInstanceByFile
			(
				System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "CV02"),
				null,null,null
			);
			CV03 = GZ.Device.PLC.PlcFactory.CreateInstanceByFile
			(
				System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "CV03"),
				null,null,null
			);
			if(System.Configuration.ConfigurationManager.AppSettings["AutoRunPLC"]=="1")
			{
				if(MEM!=null){MEM.Run();}
				if(TC01!=null){TC01.Run();}
				if(TC02!=null){TC02.Run();}
				if(TC03!=null){TC03.Run();}
				if(CV01!=null){CV01.Run();}
				if(CV02!=null){CV02.Run();}
				if(CV03!=null){CV03.Run();}
			}
        }
	}
	#endregion
	
	#region 简单PLC
		
	/// <summary>
	/// PLC变量组
	/// </summary>
	public class EasyPLC
	{
		private static EasyPLC easyPLC;
		private static readonly object locker = new object();
		public static EasyPLC CreateInstance()
        {
            if (easyPLC == null)
            {
                lock (locker)
				{
					easyPLC = new EasyPLC();
				}
            }
			return easyPLC;
        }
		private EasyPLC()
        {

        }
		
		#region MEM
		public _MEM MEM = new _MEM();
		/// <summary>
		/// 记录MEM的值
		/// </summary>		
		public class _MEM
		{
			public _TC01_INFO TC01_INFO = new _TC01_INFO();
			/// <summary>
			/// 
			/// </summary>	
			public class _TC01_INFO
			{
				public string TC01_DeviceTaskNo = "";
				public string TC01_ContNo = "";
				public string TC01_CurPos = "";
				public string TC01_ToPos = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _TC02_INFO TC02_INFO = new _TC02_INFO();
			/// <summary>
			/// 
			/// </summary>	
			public class _TC02_INFO
			{
				public string TC02_DeviceTaskNo = "";
				public string TC02_ContNo = "";
				public string TC02_CurPos = "";
				public string TC02_ToPos = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _TC03_INFO TC03_INFO = new _TC03_INFO();
			/// <summary>
			/// 
			/// </summary>	
			public class _TC03_INFO
			{
				public string TC03_DeviceTaskNo = "";
				public string TC03_ContNo = "";
				public string TC03_CurPos = "";
				public string TC03_ToPos = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
		}
		#endregion
		
		#region TC01
		public _TC01 TC01 = new _TC01();
		/// <summary>
		/// 嘉远老厂单伸堆垛机
		/// </summary>		
		public class _TC01
		{
			public _StackerFinishSend_01 StackerFinishSend_01 = new _StackerFinishSend_01();
			/// <summary>
			/// 
			/// </summary>	
			public class _StackerFinishSend_01
			{
				public string wcs_taskFinishAffirm = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _StackerAffirm_01 StackerAffirm_01 = new _StackerAffirm_01();
			/// <summary>
			/// 
			/// </summary>	
			public class _StackerAffirm_01
			{
				public string wcs_taskErrorAffirm = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _SendTask_01 SendTask_01 = new _SendTask_01();
			/// <summary>
			/// 
			/// </summary>	
			public class _SendTask_01
			{
				public string wcs_trayType = "";
				public string wcs_startPosX = "";
				public string wcs_startPosY = "";
				public string wcs_startPosZ = "";
				public string wcs_endPosX = "";
				public string wcs_endPosY = "";
				public string wcs_endPosZ = "";
				public string wcs_taskType = "";
				public string wcs_taskNo = "";
				public string wcs_taskExe = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _HeartBeat_01 HeartBeat_01 = new _HeartBeat_01();
			/// <summary>
			/// 
			/// </summary>	
			public class _HeartBeat_01
			{
				public string wcs_heartBeat = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _StackDelivery_01 StackDelivery_01 = new _StackDelivery_01();
			/// <summary>
			/// 
			/// </summary>	
			public class _StackDelivery_01
			{
				public string wcs_taskInAffirm = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
		}
		#endregion
		
		#region TC02
		public _TC02 TC02 = new _TC02();
		/// <summary>
		/// B03成品库单伸堆垛机
		/// </summary>		
		public class _TC02
		{
			public _StackerFinishSend_02 StackerFinishSend_02 = new _StackerFinishSend_02();
			/// <summary>
			/// 
			/// </summary>	
			public class _StackerFinishSend_02
			{
				public string wcs_taskFinishAffirm = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _StackerAffirm_02 StackerAffirm_02 = new _StackerAffirm_02();
			/// <summary>
			/// 
			/// </summary>	
			public class _StackerAffirm_02
			{
				public string wcs_taskErrorAffirm = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _SendTask_02 SendTask_02 = new _SendTask_02();
			/// <summary>
			/// 
			/// </summary>	
			public class _SendTask_02
			{
				public string wcs_trayType = "";
				public string wcs_startPosX = "";
				public string wcs_startPosY = "";
				public string wcs_startPosZ = "";
				public string wcs_endPosX = "";
				public string wcs_endPosY = "";
				public string wcs_endPosZ = "";
				public string wcs_taskType = "";
				public string wcs_taskNo = "";
				public string wcs_taskExe = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _HeartBeat_02 HeartBeat_02 = new _HeartBeat_02();
			/// <summary>
			/// 
			/// </summary>	
			public class _HeartBeat_02
			{
				public string wcs_heartBeat = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _StackDelivery_02 StackDelivery_02 = new _StackDelivery_02();
			/// <summary>
			/// 
			/// </summary>	
			public class _StackDelivery_02
			{
				public string wcs_taskInAffirm = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
		}
		#endregion
		
		#region TC03
		public _TC03 TC03 = new _TC03();
		/// <summary>
		/// B01毛坯库双伸堆垛机
		/// </summary>		
		public class _TC03
		{
			public _StackerFinishSend_03 StackerFinishSend_03 = new _StackerFinishSend_03();
			/// <summary>
			/// 
			/// </summary>	
			public class _StackerFinishSend_03
			{
				public string wcs_taskFinishAffirm = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _StackerAffirm_03 StackerAffirm_03 = new _StackerAffirm_03();
			/// <summary>
			/// 
			/// </summary>	
			public class _StackerAffirm_03
			{
				public string wcs_taskErrorAffirm = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _SendTask_03 SendTask_03 = new _SendTask_03();
			/// <summary>
			/// 
			/// </summary>	
			public class _SendTask_03
			{
				public string wcs_trayType = "";
				public string wcs_startPosX = "";
				public string wcs_startPosY = "";
				public string wcs_startPosZ = "";
				public string wcs_endPosX = "";
				public string wcs_endPosY = "";
				public string wcs_endPosZ = "";
				public string wcs_taskType = "";
				public string wcs_taskNo = "";
				public string wcs_taskExe = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _HeartBeat_03 HeartBeat_03 = new _HeartBeat_03();
			/// <summary>
			/// 
			/// </summary>	
			public class _HeartBeat_03
			{
				public string wcs_heartBeat = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _StackDelivery_03 StackDelivery_03 = new _StackDelivery_03();
			/// <summary>
			/// 
			/// </summary>	
			public class _StackDelivery_03
			{
				public string wcs_taskInAffirm = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
		}
		#endregion
		
		#region CV01
		public _CV01 CV01 = new _CV01();
		/// <summary>
		/// 嘉远老厂输送线
		/// </summary>		
		public class _CV01
		{
			public _wcs_sendTask_to_plc_1001 wcs_sendTask_to_plc_1001 = new _wcs_sendTask_to_plc_1001();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_1001
			{
				public string wcs_taskNo_1001 = "";
				public string wcs_toPos_1001 = "";
				public string wcs_type_1001 = "";
				public string wcs_stb_1001 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_1002 wcs_sendTask_to_plc_1002 = new _wcs_sendTask_to_plc_1002();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_1002
			{
				public string wcs_taskNo_1002 = "";
				public string wcs_toPos_1002 = "";
				public string wcs_type_1002 = "";
				public string wcs_stb_1002 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_1003 wcs_sendTask_to_plc_1003 = new _wcs_sendTask_to_plc_1003();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_1003
			{
				public string wcs_taskNo_1003 = "";
				public string wcs_toPos_1003 = "";
				public string wcs_type_1003 = "";
				public string wcs_stb_1003 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_1004 wcs_sendTask_to_plc_1004 = new _wcs_sendTask_to_plc_1004();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_1004
			{
				public string wcs_taskNo_1004 = "";
				public string wcs_toPos_1004 = "";
				public string wcs_type_1004 = "";
				public string wcs_stb_1004 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_1005 wcs_sendTask_to_plc_1005 = new _wcs_sendTask_to_plc_1005();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_1005
			{
				public string wcs_taskNo_1005 = "";
				public string wcs_toPos_1005 = "";
				public string wcs_type_1005 = "";
				public string wcs_stb_1005 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_1006 wcs_sendTask_to_plc_1006 = new _wcs_sendTask_to_plc_1006();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_1006
			{
				public string wcs_taskNo_1006 = "";
				public string wcs_toPos_1006 = "";
				public string wcs_type_1006 = "";
				public string wcs_stb_1006 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_1007 wcs_sendTask_to_plc_1007 = new _wcs_sendTask_to_plc_1007();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_1007
			{
				public string wcs_taskNo_1007 = "";
				public string wcs_toPos_1007 = "";
				public string wcs_type_1007 = "";
				public string wcs_stb_1007 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_1008 wcs_sendTask_to_plc_1008 = new _wcs_sendTask_to_plc_1008();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_1008
			{
				public string wcs_taskNo_1008 = "";
				public string wcs_toPos_1008 = "";
				public string wcs_type_1008 = "";
				public string wcs_stb_1008 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _HeartBeat_03 HeartBeat_03 = new _HeartBeat_03();
			/// <summary>
			/// 
			/// </summary>	
			public class _HeartBeat_03
			{
				public string wcs_heart = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_1001 wcs_sendStb_to_plc_1001 = new _wcs_sendStb_to_plc_1001();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_1001
			{
				public string wcs_stb_1001 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_1002 wcs_sendStb_to_plc_1002 = new _wcs_sendStb_to_plc_1002();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_1002
			{
				public string wcs_stb_1002 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_1003 wcs_sendStb_to_plc_1003 = new _wcs_sendStb_to_plc_1003();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_1003
			{
				public string wcs_stb_1003 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_1004 wcs_sendStb_to_plc_1004 = new _wcs_sendStb_to_plc_1004();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_1004
			{
				public string wcs_stb_1004 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_1005 wcs_sendStb_to_plc_1005 = new _wcs_sendStb_to_plc_1005();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_1005
			{
				public string wcs_stb_1005 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_1006 wcs_sendStb_to_plc_1006 = new _wcs_sendStb_to_plc_1006();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_1006
			{
				public string wcs_stb_1006 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_1007 wcs_sendStb_to_plc_1007 = new _wcs_sendStb_to_plc_1007();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_1007
			{
				public string wcs_stb_1007 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_1008 wcs_sendStb_to_plc_1008 = new _wcs_sendStb_to_plc_1008();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_1008
			{
				public string wcs_stb_1008 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
		}
		#endregion
		
		#region CV02
		public _CV02 CV02 = new _CV02();
		/// <summary>
		/// B03车间成品库
		/// </summary>		
		public class _CV02
		{
			public _wcs_sendTask_to_plc_2001 wcs_sendTask_to_plc_2001 = new _wcs_sendTask_to_plc_2001();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_2001
			{
				public string wcs_taskNo_2001 = "";
				public string wcs_toPos_2001 = "";
				public string wcs_type_2001 = "";
				public string wcs_stb_2001 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_2002 wcs_sendTask_to_plc_2002 = new _wcs_sendTask_to_plc_2002();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_2002
			{
				public string wcs_taskNo_2002 = "";
				public string wcs_toPos_2002 = "";
				public string wcs_type_2002 = "";
				public string wcs_stb_2002 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_2003 wcs_sendTask_to_plc_2003 = new _wcs_sendTask_to_plc_2003();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_2003
			{
				public string wcs_taskNo_2003 = "";
				public string wcs_toPos_2003 = "";
				public string wcs_type_2003 = "";
				public string wcs_stb_2003 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_2004 wcs_sendTask_to_plc_2004 = new _wcs_sendTask_to_plc_2004();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_2004
			{
				public string wcs_taskNo_2004 = "";
				public string wcs_toPos_2004 = "";
				public string wcs_type_2004 = "";
				public string wcs_stb_2004 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_2005 wcs_sendTask_to_plc_2005 = new _wcs_sendTask_to_plc_2005();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_2005
			{
				public string wcs_taskNo_2005 = "";
				public string wcs_toPos_2005 = "";
				public string wcs_type_2005 = "";
				public string wcs_stb_2005 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_2006 wcs_sendTask_to_plc_2006 = new _wcs_sendTask_to_plc_2006();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_2006
			{
				public string wcs_taskNo_2006 = "";
				public string wcs_toPos_2006 = "";
				public string wcs_type_2006 = "";
				public string wcs_stb_2006 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_2007 wcs_sendTask_to_plc_2007 = new _wcs_sendTask_to_plc_2007();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_2007
			{
				public string wcs_taskNo_2007 = "";
				public string wcs_toPos_2007 = "";
				public string wcs_type_2007 = "";
				public string wcs_stb_2007 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_2008 wcs_sendTask_to_plc_2008 = new _wcs_sendTask_to_plc_2008();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_2008
			{
				public string wcs_taskNo_2008 = "";
				public string wcs_toPos_2008 = "";
				public string wcs_type_2008 = "";
				public string wcs_stb_2008 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _HeartBeat_03 HeartBeat_03 = new _HeartBeat_03();
			/// <summary>
			/// 
			/// </summary>	
			public class _HeartBeat_03
			{
				public string wcs_heart = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_2001 wcs_sendStb_to_plc_2001 = new _wcs_sendStb_to_plc_2001();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_2001
			{
				public string wcs_stb_2001 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_2002 wcs_sendStb_to_plc_2002 = new _wcs_sendStb_to_plc_2002();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_2002
			{
				public string wcs_stb_2002 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_2003 wcs_sendStb_to_plc_2003 = new _wcs_sendStb_to_plc_2003();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_2003
			{
				public string wcs_stb_2003 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_2004 wcs_sendStb_to_plc_2004 = new _wcs_sendStb_to_plc_2004();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_2004
			{
				public string wcs_stb_2004 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_2005 wcs_sendStb_to_plc_2005 = new _wcs_sendStb_to_plc_2005();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_2005
			{
				public string wcs_stb_2005 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_2006 wcs_sendStb_to_plc_2006 = new _wcs_sendStb_to_plc_2006();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_2006
			{
				public string wcs_stb_2006 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_2007 wcs_sendStb_to_plc_2007 = new _wcs_sendStb_to_plc_2007();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_2007
			{
				public string wcs_stb_2007 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_2008 wcs_sendStb_to_plc_2008 = new _wcs_sendStb_to_plc_2008();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_2008
			{
				public string wcs_stb_2008 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
		}
		#endregion
		
		#region CV03
		public _CV03 CV03 = new _CV03();
		/// <summary>
		/// B01车间毛坯库
		/// </summary>		
		public class _CV03
		{
			public _wcs_sendTask_to_plc_3001 wcs_sendTask_to_plc_3001 = new _wcs_sendTask_to_plc_3001();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_3001
			{
				public string wcs_taskNo_3001 = "";
				public string wcs_toPos_3001 = "";
				public string wcs_type_3001 = "";
				public string wcs_stb_3001 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_3002 wcs_sendTask_to_plc_3002 = new _wcs_sendTask_to_plc_3002();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_3002
			{
				public string wcs_taskNo_3002 = "";
				public string wcs_toPos_3002 = "";
				public string wcs_type_3002 = "";
				public string wcs_stb_3002 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_3003 wcs_sendTask_to_plc_3003 = new _wcs_sendTask_to_plc_3003();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_3003
			{
				public string wcs_taskNo_3003 = "";
				public string wcs_toPos_3003 = "";
				public string wcs_type_3003 = "";
				public string wcs_stb_3003 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_3004 wcs_sendTask_to_plc_3004 = new _wcs_sendTask_to_plc_3004();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_3004
			{
				public string wcs_taskNo_3004 = "";
				public string wcs_toPos_3004 = "";
				public string wcs_type_3004 = "";
				public string wcs_stb_3004 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_3005 wcs_sendTask_to_plc_3005 = new _wcs_sendTask_to_plc_3005();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_3005
			{
				public string wcs_taskNo_3005 = "";
				public string wcs_toPos_3005 = "";
				public string wcs_type_3005 = "";
				public string wcs_stb_3005 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_3006 wcs_sendTask_to_plc_3006 = new _wcs_sendTask_to_plc_3006();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_3006
			{
				public string wcs_taskNo_3006 = "";
				public string wcs_toPos_3006 = "";
				public string wcs_type_3006 = "";
				public string wcs_stb_3006 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_3007 wcs_sendTask_to_plc_3007 = new _wcs_sendTask_to_plc_3007();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_3007
			{
				public string wcs_taskNo_3007 = "";
				public string wcs_toPos_3007 = "";
				public string wcs_type_3007 = "";
				public string wcs_stb_3007 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendTask_to_plc_3008 wcs_sendTask_to_plc_3008 = new _wcs_sendTask_to_plc_3008();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendTask_to_plc_3008
			{
				public string wcs_taskNo_3008 = "";
				public string wcs_toPos_3008 = "";
				public string wcs_type_3008 = "";
				public string wcs_stb_3008 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _HeartBeat_03 HeartBeat_03 = new _HeartBeat_03();
			/// <summary>
			/// 
			/// </summary>	
			public class _HeartBeat_03
			{
				public string wcs_heart = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_3001 wcs_sendStb_to_plc_3001 = new _wcs_sendStb_to_plc_3001();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_3001
			{
				public string wcs_stb_3001 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_3002 wcs_sendStb_to_plc_3002 = new _wcs_sendStb_to_plc_3002();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_3002
			{
				public string wcs_stb_3002 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_3003 wcs_sendStb_to_plc_3003 = new _wcs_sendStb_to_plc_3003();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_3003
			{
				public string wcs_stb_3003 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_3004 wcs_sendStb_to_plc_3004 = new _wcs_sendStb_to_plc_3004();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_3004
			{
				public string wcs_stb_3004 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_3005 wcs_sendStb_to_plc_3005 = new _wcs_sendStb_to_plc_3005();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_3005
			{
				public string wcs_stb_3005 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_3006 wcs_sendStb_to_plc_3006 = new _wcs_sendStb_to_plc_3006();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_3006
			{
				public string wcs_stb_3006 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_3007 wcs_sendStb_to_plc_3007 = new _wcs_sendStb_to_plc_3007();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_3007
			{
				public string wcs_stb_3007 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
			public _wcs_sendStb_to_plc_3008 wcs_sendStb_to_plc_3008 = new _wcs_sendStb_to_plc_3008();
			/// <summary>
			/// 
			/// </summary>	
			public class _wcs_sendStb_to_plc_3008
			{
				public string wcs_stb_3008 = "";

				public bool Write()
				{
					return Write(null);
				}
				
				public bool Write(string logicNo)
				{
					return true;
				}
			}
		}
		#endregion
		
	}

	#endregion
}
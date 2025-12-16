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
using GZ.DB.Entity.WCS;
using GZ.DB.IRepository.WCS;
using GZ.DB.Repository.WCS;
using GZ.Common.Code;
using GZ.Common.Data;
namespace GZ.Projects.WCS_SVC
{
	#region 设备通信
	/// <summary>
	/// 设备通信
	/// </summary>
	public class Device
	{
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
			if(System.Configuration.ConfigurationManager.AppSettings["AutoRunPLC"]=="1")
			{
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
		
	}

	#endregion
}
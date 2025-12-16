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
	/// <summary>
	/// 集成互联
	/// </summary>
	public class Conn
	{
		public static GZ.Modular.Log.ILogger YLLog = null;
		public static GZ.Modular.Redis.RedisTagHelper YLRedis = new GZ.Modular.Redis.RedisTagHelper(YLLog);

		private static Conn conn;
		private static readonly object locker = new object();
		public static Conn CreateInstance()
        {
            if (conn == null)
            {
				lock (locker)
				{
					conn = new Conn();
				}
            }
			return conn;
        }
		private Conn()
		{
			string[] allKeys = System.Configuration.ConfigurationManager.AppSettings.AllKeys;
				YLLog = new GZ.Modular.Log.NLogger("NLogger",7,"DEBUG,INFO,WARN",true,"mongodb://admin:123456@localhost:27017?connectTimeoutMS=3000&socketTimeoutMS=5000","WCS");
			
		}
	}
}
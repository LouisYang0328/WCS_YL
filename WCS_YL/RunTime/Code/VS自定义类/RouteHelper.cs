using System;
using System.Collections.Generic;
using System.Linq;

namespace GZ.Projects.WCS_YL
{
	/// <summary>
	/// 路径查找助手类 - 使用字典常量替代数据库查找next_stn 查找设备编号
	/// </summary>
	public static class RouteHelper
	{
		#region 路径字典常量定义

		/// <summary>
		/// 路径映射字典 - 键格式: "CUR_STN|TO_STN", 值: RouteInfo对象
		/// 考虑到可维护性，后面可以加前置后置站台到RouteInfo中
		/// </summary>
		private static readonly Dictionary<string, RouteInfo> RouteMap = new Dictionary<string, RouteInfo>
		{
			// 1楼入库路径 - FORWARD方向
			{ "1002|BACK", new RouteInfo { NextStn = "1001", Description = "嘉远老厂1001-1002入库回退" } },
			{ "1002|FORWARD", new RouteInfo { NextStn = "1002", Description = "嘉远老厂1001-1002入库" } },
			{ "1006|BACK", new RouteInfo { NextStn = "1005", Description = "嘉远老厂1005-1006入库回退" } },
			{ "1006|FORWARD", new RouteInfo { NextStn = "1006", Description = "嘉远老厂1005-1006入库" } },
			{ "1008|BACK", new RouteInfo { NextStn = "1007", Description = "嘉远老厂1007-1008入库回退" } },
			{ "1008|FORWARD", new RouteInfo { NextStn = "1008", Description = "嘉远老厂1007-1008入库" } },
			
			{ "YBLK-01|101010", new RouteInfo { NextStn = "101011", Description = "托盘库1排" } },
			{ "YBLK-02|101010", new RouteInfo { NextStn = "101011", Description = "托盘库2排" } },
			{ "YBLK-03|101010", new RouteInfo { NextStn = "101011", Description = "托盘库3排" } },
			{ "YBLK-04|101010", new RouteInfo { NextStn = "101011", Description = "托盘库4排" } },
			
			{ "YBLK-01|101012", new RouteInfo { NextStn = "101012", Description = "托盘库1排" } },
			{ "YBLK-02|101012", new RouteInfo { NextStn = "101012", Description = "托盘库2排" } },
			{ "YBLK-03|101012", new RouteInfo { NextStn = "101012", Description = "托盘库3排" } },
			{ "YBLK-04|101012", new RouteInfo { NextStn = "101012", Description = "托盘库4排" } },
			
			{ "YBLK-01|101001", new RouteInfo { NextStn = "101002", Description = "托盘库1排" } },
			{ "YBLK-02|101001", new RouteInfo { NextStn = "101002", Description = "托盘库2排" } },
			{ "YBLK-03|101001", new RouteInfo { NextStn = "101002", Description = "托盘库3排" } },
			{ "YBLK-04|101001", new RouteInfo { NextStn = "101002", Description = "托盘库4排" } },
		};

		#endregion

		#region 公共方法

		/// <summary>
		/// 根据当前站台和目标方向获取下一站台信息
		/// </summary>
		/// <param name="curStn">当前站台编号</param>
		/// <param name="toStn">目标方向(FORWARD/BACK)或目标站台</param>
		/// <returns>路径信息，如果未找到返回null</returns>
		public static RouteInfo GetNextStation(string curStn, string toStn)
		{
			try
			{
				string key = string.Empty;
				if (string.IsNullOrEmpty(curStn) || string.IsNullOrEmpty(toStn))
				{
					return null;
				}
				// 如果是TPK开头的站台，尝试前缀匹配
				if (curStn.StartsWith("YBLK-"))
				{
					var prefix = curStn.Substring(0, 7);
					key = $"{prefix}|{toStn}";
					if (RouteMap.ContainsKey(key))
					{
						return RouteMap[key];
					}
				}
				if (toStn.StartsWith("YBLK-"))
				{
					var prefix = toStn.Substring(0, 7);
					key = $"{curStn}|{prefix}";
					if (RouteMap.ContainsKey(key))
					{
						return RouteMap[key];
					}
				}
				// 直接匹配
				key = $"{curStn}|{toStn}";
				if (RouteMap.ContainsKey(key))
				{
					return RouteMap[key];
				}


				return null;
			}
			catch (Exception ex)
			{
				Conn.YLLog.Error(1, "[RouteHelper.GetNextStation调用异常]", 1, $"获取下一站台失败: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// 获取下一站台编号
		/// </summary>
		/// <param name="curStn">当前站台编号</param>
		/// <param name="toStn">目标方向或目标站台</param>
		/// <returns>下一站台编号，如果未找到返回空字符串</returns>
		public static string GetNextStn(string curStn, string toStn)
		{
			var routeInfo = GetNextStation(curStn, toStn);
			return routeInfo?.NextStn ?? string.Empty;
		}
		
		/// <summary>
		/// 获取设备编号
		/// </summary>
		/// <param name="station">站台编号</param>
		/// <param name="deviceType">设备类型</param>
		/// <returns>设备编号</returns>
		public static string GetDeviceNo(string station ,string deviceType)
		{
			// CV01 站台列表
			List<string> cv01Stations = new List<string>
			{
				"1001", "1002", "1003", "1004", "1005", "1006", "1007", "1008"
			};
			// CV02 站台列表
			List<string> cv02Stations = new List<string>
			{
				"2001", "2002", "2003", "2004", "2005", "2006", "2007", "2008"
			};
			// CV03 站台列表
			List<string> cv03Stations = new List<string>
			{
				"3001", "3002", "3003", "3004", "3005", "3006", "3007", "3008"
			};

			if (cv01Stations.Contains(station))
			{
				return $"{deviceType}01";
			}
			else if (cv02Stations.Contains(station))
			{
				return $"{deviceType}02";
			}
			else if (cv03Stations.Contains(station))
			{
				return $"{deviceType}03";
			}
			else
			{
				return "";
			}
		}
		
		#endregion
	}

	/// <summary>
	/// 路径信息类
	/// </summary>
	public class RouteInfo
	{
		/// <summary>
		/// 下一站台编号
		/// </summary>
		public string NextStn { get; set; }

		/// <summary>
		/// 前置站台编号（可选）
		/// </summary>
		public string PreStn { get; set; }

		/// <summary>
		/// 后置站台编号（可选）
		/// </summary>
		public string PostStn { get; set; }

		/// <summary>
		/// 路径描述（可选）
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 路径优先级（数值越小优先级越高）
		/// </summary>
		public int Priority { get; set; } = 0;

		/// <summary>
		/// 路径状态（是否可用）
		/// </summary>
		public bool IsActive { get; set; } = true;

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateTime { get; set; } = DateTime.Now;

		public override string ToString()
		{
			return $"NextStn: {NextStn}, Description: {Description}, Priority: {Priority}, IsActive: {IsActive}";
		}
	}
}
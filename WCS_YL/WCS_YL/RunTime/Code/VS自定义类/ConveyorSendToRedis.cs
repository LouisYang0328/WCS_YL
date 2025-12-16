using System;
using System.Collections.Generic;
using System.Threading;
using GZ.DB.Entity.wcs_yl;

namespace GZ.Projects.WCS_YL
{
	/// <summary>
	/// 输送线发送任务到Redis
	/// </summary>
	public class ConveyorSendToRedis
	{

		/// <summary>
		/// WCS下发输送线指令
		/// </summary>
		/// <param name="deviceNo">设备编号</param>
		/// <param name="station">站台编号</param>
		/// <param name="taskNo">任务号</param>
		/// <param name="toPos">目标地址</param>
		/// <param name="taskType">任务类型</param>
		/// <param name="taskExe">开始信号</param>
		/// <returns>是否成功</returns>
		public bool SendTask_Station(string deviceNo, string station, string taskNo, string toPos, string taskType, string taskExe, out string err)
		{
			err = "";
			try
			{
				string groupName = $"{deviceNo}.wcs_sendTask_to_plc_{station}";
				string setQueue = $"{deviceNo}Queue";
				string taskNoName = $"{deviceNo}.wcs_taskNo_{station}";
				string toPosName = $"{deviceNo}.wcs_toPos_{station}";
				string taskTypeName = $"{deviceNo}.wcs_type_{station}";
				string taskExeName = $"{deviceNo}.wcs_stb_{station}";

				GZ.Modular.Redis.WriteGroupEntity group = new GZ.Modular.Redis.WriteGroupEntity
				{
					groupName = groupName,
					queueStatus = 1,
					queueTime = DateTime.Now,
					writeList = new List<GZ.Modular.Redis.ParamData>
					{

						new GZ.Modular.Redis.ParamData { paramName = taskNoName, paramValue = taskNo },
						new GZ.Modular.Redis.ParamData { paramName = toPosName, paramValue = toPos },
						new GZ.Modular.Redis.ParamData { paramName = taskTypeName, paramValue = taskType },
						new GZ.Modular.Redis.ParamData { paramName = taskExeName, paramValue = taskExe }

					}
				};
				return Conn.YLRedis.SetQueue(group, setQueue, "", "");
			}
			catch (Exception ex)
			{
				err += ex.ToString();
				return false;
			}
		}
		
		/// <summary>
		/// WCS清除自身指令
		/// </summary>
		/// <param name="deviceNo">设备编号</param>
		/// <param name="station">站台编号</param>
		/// <returns>是否成功</returns>
		public bool ClearTask_Station(string deviceNo, string station, out string err)
		{
			err = "";
			try
			{
				string groupName = $"{deviceNo}.wcs_sendTask_to_plc_{station}";
				string setQueue = $"{deviceNo}Queue";
				string taskNoName = $"{deviceNo}.wcs_taskNo_{station}";
				string toPosName = $"{deviceNo}.wcs_toPos_{station}";
				string taskTypeName = $"{deviceNo}.wcs_type_{station}";
				string taskExeName = $"{deviceNo}.wcs_stb_{station}";

				GZ.Modular.Redis.WriteGroupEntity group = new GZ.Modular.Redis.WriteGroupEntity
				{
					groupName = groupName,
					queueStatus = 1,
					queueTime = DateTime.Now,
					writeList = new List<GZ.Modular.Redis.ParamData>
					{

						new GZ.Modular.Redis.ParamData { paramName = taskNoName, paramValue = "0" },
						new GZ.Modular.Redis.ParamData { paramName = toPosName, paramValue = "0" },
						new GZ.Modular.Redis.ParamData { paramName = taskTypeName, paramValue = "0" },
						new GZ.Modular.Redis.ParamData { paramName = taskExeName, paramValue = "0" }

					}
				};
				return Conn.YLRedis.SetQueue(group, setQueue, "", "");
			}
			catch (Exception ex)
			{
				err += ex.ToString();
				return false;
			}

		}
		
		/// <summary>
		/// WCS与PLC的交互信号进行握手
		/// </summary>
		/// <param name="deviceNo">设备编号</param>
		/// <param name="station">站台编号</param>
		/// <param name="taskExe">开始信号</param>
		/// <returns>是否成功</returns>
		public bool InteractTask_Station(string deviceNo, string station, string taskExe, out string err)
		{
			
			err = "";
			try
			{
				string groupName = $"{deviceNo}.wcs_sendStb_to_plc_{station}";
				string setQueue = $"{deviceNo}Queue";
				string taskExeName = $"{deviceNo}.wcs_stb_{station}";

				GZ.Modular.Redis.WriteGroupEntity group = new GZ.Modular.Redis.WriteGroupEntity
				{
					groupName = groupName,
					queueStatus = 1,
					queueTime = DateTime.Now,
					writeList = new List<GZ.Modular.Redis.ParamData>
					{
						new GZ.Modular.Redis.ParamData { paramName = taskExeName, paramValue = taskExe }
					}
				};
				return Conn.YLRedis.SetQueue(group, setQueue, "", "");
			}
			catch (Exception ex)
			{
				err += ex.ToString();
				return false;
			}

		}
	}
}
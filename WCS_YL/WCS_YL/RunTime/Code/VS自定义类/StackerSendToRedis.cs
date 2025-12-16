using System;
using System.Collections.Generic;
using System.Threading;
using GZ.DB.Entity.wcs_yl;

namespace GZ.Projects.WCS_YL
{
	/// <summary>
	/// 堆垛机发送任务到Redis
	/// </summary>
	public class StackerSendToRedis
	{

		/// <summary>
		/// WCS下发堆垛机调度任务
		/// </summary>
		/// <param name="task">堆垛机实体</param>
		/// <param name="num">堆垛机编号</param>
		/// <returns>是否成功</returns>
		public bool SendTask_SingleStation(StackerTaskEntity task, string num ,out string err)
		{
			err = "";
			try
			{
				string suffixName = $"TC{num}";
				string groupName = $"{suffixName}.SendTask_{num}";
				string setQueue = $"{suffixName}Queue";
				string contType = $"{suffixName}.wcs_trayType";
				string srcZ = $"{suffixName}.wcs_startPosX";
				string srcX = $"{suffixName}.wcs_startPosY";
				string srcY = $"{suffixName}.wcs_startPosZ";
				string destZ = $"{suffixName}.wcs_endPosX";
				string destX = $"{suffixName}.wcs_endPosY";
				string destY = $"{suffixName}.wcs_endPosZ";
				string taskType = $"{suffixName}.wcs_taskType";
				string taskNo = $"{suffixName}.wcs_taskNo";
				string taskExe = $"{suffixName}.wcs_taskExe";  //1下发 2数据异常

				GZ.Modular.Redis.WriteGroupEntity group = new GZ.Modular.Redis.WriteGroupEntity
				{
					groupName = groupName,
					queueStatus = 1,
					queueTime = DateTime.Now,
					writeList = new List<GZ.Modular.Redis.ParamData>
					{
						new GZ.Modular.Redis.ParamData { paramName = contType, paramValue = task.TrayType },
						new GZ.Modular.Redis.ParamData { paramName = srcZ, paramValue = task.FromX.ToString() }, //排
						new GZ.Modular.Redis.ParamData { paramName = srcX, paramValue = task.FromY.ToString() }, //列
						new GZ.Modular.Redis.ParamData { paramName = srcY, paramValue = task.FromZ.ToString() }, //层
						new GZ.Modular.Redis.ParamData { paramName = destZ, paramValue = task.ToX.ToString() },
						new GZ.Modular.Redis.ParamData { paramName = destX, paramValue = task.ToY.ToString() },
						new GZ.Modular.Redis.ParamData { paramName = destY, paramValue = task.ToZ.ToString() },
						new GZ.Modular.Redis.ParamData { paramName = taskType, paramValue = task.DeviceTaskType },
						new GZ.Modular.Redis.ParamData { paramName = taskNo, paramValue = task.DeviceTaskNo },
						new GZ.Modular.Redis.ParamData { paramName = taskExe, paramValue = "1" }

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
		/// <param name="task">堆垛机实体</param>
		/// <param name="num">堆垛机编号</param>
		/// <returns>是否成功</returns>
		public bool ClearTask_SingleStation(StackerTaskEntity task,string num , out string err)
		{
			
			err = "";
			try
			{
				string suffixName = $"TC{num}";
				string groupName = $"{suffixName}.SendTask_{num}";
				string setQueue = $"{suffixName}Queue";
				string contType = $"{suffixName}.wcs_trayType";
				string srcZ = $"{suffixName}.wcs_startPosX";
				string srcX = $"{suffixName}.wcs_startPosY";
				string srcY = $"{suffixName}.wcs_startPosZ";
				string destZ = $"{suffixName}.wcs_endPosX";
				string destX = $"{suffixName}.wcs_endPosY";
				string destY = $"{suffixName}.wcs_endPosZ";
				string taskType = $"{suffixName}.wcs_taskType";
				string taskNo = $"{suffixName}.wcs_taskNo";
				string taskExe = $"{suffixName}.wcs_taskExe";  //1下发 2数据异常

				GZ.Modular.Redis.WriteGroupEntity group = new GZ.Modular.Redis.WriteGroupEntity
				{
					groupName = groupName,
					queueStatus = 1,
					queueTime = DateTime.Now,
					writeList = new List<GZ.Modular.Redis.ParamData>
					{
						new GZ.Modular.Redis.ParamData { paramName = contType, paramValue = "0" },
						new GZ.Modular.Redis.ParamData { paramName = srcZ, paramValue = "0" }, //排
						new GZ.Modular.Redis.ParamData { paramName = srcX, paramValue = "0" }, //列
						new GZ.Modular.Redis.ParamData { paramName = srcY, paramValue = "0" }, //层
						new GZ.Modular.Redis.ParamData { paramName = destZ, paramValue = "0" },
						new GZ.Modular.Redis.ParamData { paramName = destX, paramValue = "0" },
						new GZ.Modular.Redis.ParamData { paramName = destY, paramValue = "0" },
						new GZ.Modular.Redis.ParamData { paramName = taskType, paramValue = "0" },
						new GZ.Modular.Redis.ParamData { paramName = taskNo, paramValue = "0" },
						new GZ.Modular.Redis.ParamData { paramName = taskExe, paramValue = "0" }

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
		/// 堆垛机完成信号确认
		/// </summary>
		/// <param name="confirmValue">设备反馈完成的值</param>
		/// <param name="num">堆垛机编号</param>
		/// <returns>是否成功</returns>
		public bool ConfirmTask_SingleStation(string confirmValue,string num, out string err)
		{
			
			err = "";
			try
			{
				string suffixName = $"TC{num}";
				string groupName = $"{suffixName}.StackerFinishSend_{num}";
				string setQueue = $"{suffixName}Queue";
				string wcsTaskFinishAffirm = $"{suffixName}.wcs_taskFinishAffirm";  //1下发 2数据异常

				GZ.Modular.Redis.WriteGroupEntity group = new GZ.Modular.Redis.WriteGroupEntity
				{
					groupName = groupName,
					queueStatus = 1,
					queueTime = DateTime.Now,
					writeList = new List<GZ.Modular.Redis.ParamData>
					{
						new GZ.Modular.Redis.ParamData { paramName = wcsTaskFinishAffirm, paramValue = confirmValue }
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
		/// 堆垛机异常信号确认
		/// </summary>
		/// <param name="manuValue">设备反馈异常的值</param>
		/// <param name="num">堆垛机编号</param>
		/// <returns>是否成功</returns>
		public bool ManuTask_SingleStation(string manuValue,string num, out string err)
		{
			
			err = "";
			try
			{
				string suffixName = $"TC{num}";
				string groupName = $"{suffixName}.StackerAffirm_{num}";
				string setQueue = $"{suffixName}Queue";
				string wcsTaskErrorAffirm = $"{suffixName}.wcs_taskErrorAffirm";  //1取货无货 2放货有货 3取深浅有 4放深浅有

				GZ.Modular.Redis.WriteGroupEntity group = new GZ.Modular.Redis.WriteGroupEntity
				{
					groupName = groupName,
					queueStatus = 1,
					queueTime = DateTime.Now,
					writeList = new List<GZ.Modular.Redis.ParamData>
					{
						new GZ.Modular.Redis.ParamData { paramName = wcsTaskErrorAffirm, paramValue = manuValue}
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
		/// 堆垛机取货完成信号确认
		/// </summary>
		/// <param name="manuValue">设备取货完成信号确认</param>
		/// <param name="num">堆垛机编号</param>
		/// <returns>是否成功</returns>
		public bool DeliveryTask_SingleStation(string deliValue,string num, out string err)
		{
			
			err = "";
			try
			{
				string suffixName = $"TC{num}";
				string groupName = $"{suffixName}.StackDelivery_{num}";
				string setQueue = $"{suffixName}Queue";
				string wcsTaskInAffirm = $"{suffixName}.wcs_taskInAffirm";  //1取货完成确认

				GZ.Modular.Redis.WriteGroupEntity group = new GZ.Modular.Redis.WriteGroupEntity
				{
					groupName = groupName,
					queueStatus = 1,
					queueTime = DateTime.Now,
					writeList = new List<GZ.Modular.Redis.ParamData>
					{
						new GZ.Modular.Redis.ParamData { paramName = wcsTaskInAffirm, paramValue = deliValue }
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
		/// 堆垛机内存任务记录
		/// </summary>
		/// <param name="deviceTaskNo">设备任务编号</param>
		/// <param name="contNum">托盘编码</param>
		/// <param name="curNum">当前位置</param>
		/// <param name="toNum">目标位置</param>
		/// <param name="num">堆垛机编号</param>
		/// <returns>是否成功</returns>
		public bool StackMemTaskInfo(string deviceTaskNo, string contNum, string curNum, string toNum, string num)
		{
			try
			{
				string setQueue = $"MEMQueue";
				string groupName = $"MEM.TC{num}_INFO";

				GZ.Modular.Redis.WriteGroupEntity group = new GZ.Modular.Redis.WriteGroupEntity
				{
					groupName = groupName,
					queueStatus = 1,
					queueTime = DateTime.Now,
					writeList = new List<GZ.Modular.Redis.ParamData>
					{
						new GZ.Modular.Redis.ParamData
						{
							paramName = $"MEM.TC{num}_DeviceTaskNo",
							paramValue = deviceTaskNo
						},
						new GZ.Modular.Redis.ParamData
						{
							paramName = $"MEM.TC{num}_ContNo",
							paramValue = contNum
						},
						new GZ.Modular.Redis.ParamData
						{
							paramName = $"MEM.TC{num}_CurPos",
							paramValue = curNum
						},
						new GZ.Modular.Redis.ParamData
						{
							paramName = $"MEM.TC{num}_ToPos",
							paramValue = toNum
						}
					}
				};
				bool result = Conn.YLRedis.SetQueue(group, setQueue, "", "");
				return result;
			}
			catch (Exception e)
			{
				Conn.YLLog.Error(1, $"[堆垛机MEM记录失败[TC{num}]]", 1, e.ToString());
				return false;
			}
		}
	}
}
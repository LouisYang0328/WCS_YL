using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GZ.DB.Entity.wcs_yl;
using GZ.DB.IRepository.wcs_yl;
using GZ.DB.Repository.wcs_yl;
using GZ.Common.Data;

namespace GZ.Projects.WCS_YL
{
	/// <summary>
	/// 任务确认反馈结果
	/// </summary>
	public class TaskConfirmResult
	{
		/// <summary>
		/// 是否成功执行
		/// </summary>
		public bool IsSuccess { get; set; }

		/// <summary>
		/// 执行结果消息
		/// </summary>
		public string Message { get; set; }

		public TaskConfirmResult(bool isSuccess, string message)
		{
			IsSuccess = isSuccess;
			Message = message;
		}
	}

	/// <summary>
	/// 任务确认反馈辅助类
	/// </summary>
	public class TaskConfirmHelper
	{
		
		/// <summary>
		/// 交互反馈  dev_taskFinishState == 0
		/// </summary>
		/// <param name="num">堆垛机编号</param>
		/// <returns>处理结果</returns>
		public TaskConfirmResult InteractionFeedback(string num)
		{
			string suffixName = $"TC{num}";
			string msg = $"\r\n\t[堆垛机任务完成反馈及更新[{suffixName}]]：Start{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

			bool isSuccess = true;
			try
			{
				StackerSendToRedis stackerSendToRedis = new StackerSendToRedis();
				bool result = stackerSendToRedis.ConfirmTask_SingleStation("0", num,out string eMsg);
				if (!result)
				{
					msg += $"\r\n\t[堆垛机完成反馈清除更新异常]：指令清除结果失败{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}" + eMsg;
				}
				msg += $"\r\n\t[堆垛机任务完成反馈信号清除[{suffixName}]] 信号清除结束，结果为：{result}";

			}
			catch (Exception ex)
			{
				msg += $"\r\n\t[Error]{ex}";
				isSuccess = false;
			}
			
			msg += $"\r\n\t[堆垛机任务置位交互[{suffixName}]]End";
			return new TaskConfirmResult(isSuccess, msg);
		}
		
		/// <summary>
		/// 正常完成反馈  dev_taskFinishState == 1
		/// </summary>
		/// <param name="num">堆垛机编号</param>
		/// <returns>处理结果</returns>
		public TaskConfirmResult NormalFinishFeedback(string num)
		{
			string suffixName = $"TC{num}";
			string msg = $"\r\n[[{suffixName}]任务正常完成处理]：Start{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

			bool isSuccess = true;
			bool dbResult = false;
			bool signalOk = false;

			try
			{
				// 获取任务号
				string plcTaskNo = Conn.YLRedis.GetValue($"{suffixName}.dev_task");
				string memDeviceTaskNo = Conn.YLRedis.GetValue($"MEM.{suffixName}_DeviceTaskNo");

				IStackerTaskRepository stackerRepo = new StackerTaskRepository();
				IMainMissionRepository mainMissionRepo = new MainMissionRepository();
				DBOpeation dbOpeation = new DBOpeation();

				// ======= 情况 1：MEM 任务号为 0，无法处理 =======
				if (memDeviceTaskNo == "0")
				{
					msg += $"\r\n\t[堆垛机信息完成异常]：MEM 中任务号为 0，无法查询任务信息 ，dev_task={plcTaskNo}";

					var redis = new StackerSendToRedis();
					bool clearTask = redis.ClearTask_SingleStation(null, num, out string eMsg);
					msg += $"\r\n\t清除下发的任务信号结果：{clearTask}";

					signalOk = FinishSignal(num, "1", ref msg);
					if (!signalOk)
					{
						return new TaskConfirmResult(false, msg);
					}

					return new TaskConfirmResult(false, msg);
				}

				// 查询堆垛机任务
				var stackerTask = stackerRepo
					.FindList(t =>
					          (t.TaskState == "Send" || t.TaskState == "Doing") &&
					          t.DeviceNo == suffixName &&
					          t.DeviceTaskNo == memDeviceTaskNo)
					.OrderByDescending(t => t.CreateTime)
					.FirstOrDefault();

				// ======= 情况 2：找到堆垛机任务 =======
				if (stackerTask != null)
				{
					msg += $"\r\n\t任务编号 {stackerTask.TaskNo}，设备任务号 {stackerTask.DeviceTaskNo}，托盘 {stackerTask.TrayCode} 完成 {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

					// 通用完成状态
					stackerTask.TaskState = "Finish";
					stackerTask.TaskEndTime = DateTime.Now;
					stackerTask.LastUpdatedTime = DateTime.Now;

					// ======= 入库 / 移库任务 =======
					if (stackerTask.TaskType == "ST_In" || stackerTask.TaskType == "ST_Move")
					{
						var mainEntity = mainMissionRepo
							.FindList(t => t.device_task_no == stackerTask.DeviceTaskNo &&
							          t.tray_code == stackerTask.TrayCode)
							.OrderByDescending(t => t.CreateTime)
							.FirstOrDefault();

						if (mainEntity != null)
						{
							// 修改主任务
							mainEntity.MissionState = "Finish";
							mainEntity.InteStationNo = "Finish";
							mainEntity.MissionEndTime = DateTime.Now;
							mainEntity.LastUpdatedTime = DateTime.Now;

							// 上报
							var systemSynchronization = new SystemSynchronizationEntity
							{
								syn_direction = 2,
								syn_status = 0,
								ope_type = "2",  //任务完成
								order_no = mainEntity.device_task_no,
								tray_code = mainEntity.tray_code,
								task_no = mainEntity.task_no,
								ope_time = DateTime.Now,
								cur_pos = mainEntity.end_position,
								err_cnt = 0
							};

							dbResult = dbOpeation.UpdateMainAndStackInsertSysTask(mainEntity, stackerTask, systemSynchronization, out string err);

							msg += $"\r\n\t入库/移库任务数据库更新结果：{dbResult}";
							if (dbResult)
							{
								signalOk = FinishSignal(num, "1", ref msg);
								if (!signalOk)
								{
									return new TaskConfirmResult(false, msg);
								}
							}
							else
							{
								msg += $"\r\n\t数据库更新失败：" + err;
							}
						}
						else
						{
							msg += $"\r\n\t[堆垛机信息完成异常]：无法查询主任务，设备任务号 {stackerTask.DeviceTaskNo}";
						}
					}
					// ======= 出库任务 =======
					else if (stackerTask.TaskType == "ST_Out")
					{
						var mainEntity = mainMissionRepo
							.FindList(t => t.device_task_no == stackerTask.DeviceTaskNo &&
							          t.tray_code == stackerTask.TrayCode)
							.OrderByDescending(t => t.CreateTime)
							.FirstOrDefault();

						if (mainEntity != null)
						{
							dbResult = dbOpeation.UpdateStackerTask(stackerTask, out string err1);

							msg += $"\r\n\t出库任务数据库更新结果：{dbResult}";
							if (dbResult)
							{
								signalOk = FinishSignal(num, "1", ref msg);
								if (!signalOk)
								{
									return new TaskConfirmResult(false, msg);
								}
							}
							else
							{
								msg += $"\r\n\t数据库更新失败：" + err1;
							}
						}
						else
						{
							msg += $"\r\n\t[堆垛机信息完成异常]：无法找到主任务，设备任务号 {stackerTask.DeviceTaskNo}";
						}
					}
				}
				else
				{
					msg += $"\r\n\t[堆垛机信息完成异常]：无法查询堆垛机任务，dev_task={plcTaskNo} MEM={memDeviceTaskNo}";
				}
			}
			catch (Exception ex)
			{
				msg += $"\r\n\t[Error]{ex}";
				isSuccess = false;
			}
			
			msg += $"\r\n\t[{suffixName}]任务正常完成]End";

			return new TaskConfirmResult(isSuccess, msg);
		}

		/// <summary>
		/// 强制完成反馈   dev_taskFinishState == 2
		/// </summary>
		/// <param name="num">堆垛机编号</param>
		/// <returns>处理结果</returns>
		public TaskConfirmResult ForceFinishFeedback(string num)
		{
			string suffixName = $"TC{num}";
			string msg = $"\r\n[[{suffixName}]任务强制完成处理]：Start{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

			bool isSuccess = true;
			bool dbResult = false;
			bool signalOk = false;

			try
			{
				// 查询任务号
				string plcTaskNo = Conn.YLRedis.GetValue($"{suffixName}.dev_task");
				string memDeviceTaskNo = Conn.YLRedis.GetValue($"MEM.{suffixName}_DeviceTaskNo");

				IStackerTaskRepository stackerRepo = new StackerTaskRepository();
				IMainMissionRepository mainMissionRepo = new MainMissionRepository();

				// =======  MEM 中设备任务号为 0，无法继续 =========
				if (memDeviceTaskNo == "0")
				{
					msg += $"\r\n\t[堆垛机强制完成异常]：MEM 中记录的设备任务号为 0，无法查询任务信息，dev_task={plcTaskNo}";

					var redis = new StackerSendToRedis();
					bool clearTask = redis.ClearTask_SingleStation(null, num, out string eMsg);
					msg += $"\r\n\t清除下发的任务信号结果：{clearTask} {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

					signalOk = FinishSignal(num, "2", ref msg);
					if (!signalOk)
					{
						return new TaskConfirmResult(false, msg);
					}

					return new TaskConfirmResult(false, msg);
				}

				DBOpeation dbOpeation = new DBOpeation();

				// 查找堆垛机任务
				var stackerTask = stackerRepo
					.FindList(t =>
					          (t.TaskState == "Send" || t.TaskState == "Doing") &&
					          t.DeviceNo == suffixName &&
					          t.DeviceTaskNo == memDeviceTaskNo)
					.OrderByDescending(t => t.CreateTime)
					.FirstOrDefault();

				// =======  找到堆垛机任务 =========
				if (stackerTask != null)
				{
					msg += $"\r\n\t任务编号 {stackerTask.TaskNo}，设备任务号 {stackerTask.DeviceTaskNo}，托盘 {stackerTask.TrayCode} 强制完成 {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

					// 通用状态修改
					stackerTask.TaskState = "Finish";
					stackerTask.TaskEndTime = DateTime.Now;
					stackerTask.LastUpdatedTime = DateTime.Now;

					// ======= 入库 / 移库任务 =========
					if (stackerTask.TaskType == "ST_In" || stackerTask.TaskType == "ST_Move")
					{
						var mainEntity = mainMissionRepo
							.FindList(t => t.device_task_no == stackerTask.DeviceTaskNo &&
							          t.tray_code == stackerTask.TrayCode)
							.OrderByDescending(t => t.CreateTime)
							.FirstOrDefault();

						if (mainEntity != null)
						{
							// 主任务更新
							mainEntity.MissionState = "Finish";
							mainEntity.InteStationNo = "Finish";
							mainEntity.MissionEndTime = DateTime.Now;
							mainEntity.LastUpdatedTime = DateTime.Now;

							// 上报状态
							var systemSynchronization = new SystemSynchronizationEntity
							{
								syn_direction = 2,
								syn_status = 0,
								ope_type = "2",  //任务完成
								order_no = mainEntity.device_task_no,
								tray_code = mainEntity.tray_code,
								task_no = mainEntity.task_no,
								ope_time = DateTime.Now,
								cur_pos = mainEntity.end_position,
								err_cnt = 0
							};

							dbResult = dbOpeation.UpdateMainAndStackInsertSysTask(mainEntity, stackerTask, systemSynchronization, out string err);

							msg += $"\r\n\t入库/移库任务数据库更新结果：{dbResult} {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

							if (dbResult)
							{
								signalOk = FinishSignal(num, "2", ref msg);
								if (!signalOk)
								{
									return new TaskConfirmResult(false, msg);
								}
							}
							else
							{
								msg += $"\r\n\t数据库更新失败：{err}";
							}
						}
						else
						{
							msg += $"\r\n\t[堆垛机强制完成异常]：主任务 {stackerTask.DeviceTaskNo} 未找到，请检查任务状态";
						}
					}
					// ======= 出库任务 =========
					else if (stackerTask.TaskType == "ST_Out")
					{
						dbResult = dbOpeation.UpdateStackerTask(stackerTask, out string err1);

						msg += $"\r\n\t出库任务数据库更新结果：{dbResult} {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

						if (dbResult)
						{
							signalOk = FinishSignal(num, "2", ref msg);
							if (!signalOk)
							{
								return new TaskConfirmResult(false, msg);
							}
						}
						else
						{
							msg += $"\r\n\t数据库更新失败：{err1}";
						}
					}
				}
				else
				{
					msg += $"\r\n\t[堆垛机强制完成异常]：未找到设备任务号为 {memDeviceTaskNo} 的任务，dev_task={plcTaskNo}";
				}
			}
			catch (Exception ex)
			{
				msg += $"\r\n\t[Error]{ex}";
				isSuccess = false;
			}

			msg += $"\r\n[{suffixName}]任务强制完成]End";
			return new TaskConfirmResult(isSuccess, msg);
		}

		/// <summary>
		/// 取消任务反馈    dev_taskFinishState == 3
		/// </summary>
		/// <param name="num">堆垛机编号</param>
		/// <returns>处理结果</returns>
		public TaskConfirmResult CancelFeedback(string num)
		{
			string suffixName = $"TC{num}";
			string msg = $"\r\n[[{suffixName}]取消任务处理]：Start{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

			bool clearTask = false;
			bool signalOk = false;
			bool dbResult = false;
			bool isSuccess = true;

			try
			{
				//查找任务号
				string plcTaskNo = Conn.YLRedis.GetValue($"{suffixName}.dev_task");
				string memDeviceTaskNo = Conn.YLRedis.GetValue($"MEM.{suffixName}_DeviceTaskNo");

				IStackerTaskRepository stackerRepo = new StackerTaskRepository();
				IMainMissionRepository mainMissionRepo = new MainMissionRepository();

				// =======  MEM 内任务号 = 0，无法查询到任务 =========
				if (memDeviceTaskNo == "0")
				{
					msg += $"\r\n\t[堆垛机信息完成异常]：堆垛机工位1的任务号为 0 无法查询到任务信息，打印[dev_task] 的值为 {plcTaskNo}";

					clearTask = new StackerSendToRedis().ClearTask_SingleStation(null, num, out string eMsg);
					msg += $"\r\n\t清除下发的任务信号结果：{clearTask} {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

					signalOk = FinishSignal(num, "3", ref msg);
					if (!signalOk)
					{
						return new TaskConfirmResult(false, msg);
					}
				}

				DBOpeation dbOpeation = new DBOpeation();

				// 查找堆垛机任务
				var stackerTask = stackerRepo
					.FindList(t =>
					          (t.TaskState == "Send" || t.TaskState == "Doing")
					          && t.DeviceNo == suffixName
					          && t.DeviceTaskNo == memDeviceTaskNo)
					.OrderByDescending(t => t.CreateTime)
					.FirstOrDefault();

				// ======= 情况 2：找到堆垛机任务 =========
				if (stackerTask != null)
				{
					msg += $"\r\n\t查找任务 TaskNo={stackerTask.TaskNo}，DeviceTaskNo={stackerTask.DeviceTaskNo}，Tray={stackerTask.TrayCode} 开始取消任务 {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

					// 修改堆垛机任务状态
					stackerTask.TaskState = "Cancel";
					stackerTask.TaskEndTime = DateTime.Now;
					stackerTask.LastUpdatedTime = DateTime.Now;

					dbResult = dbOpeation.UpdateStackerTask(stackerTask, out string error);

					// 查找主任务
					var mainEntity = mainMissionRepo
						.FindList(t => t.task_no == stackerTask.TaskNo)
						.OrderByDescending(t => t.CreateTime)
						.FirstOrDefault();

					if (mainEntity != null)
					{
						// 修改主任务状态
						mainEntity.MissionState = "Cancel";
						mainEntity.MissionEndTime = DateTime.Now;
						mainEntity.LastUpdatedTime = DateTime.Now;

						// 上报接口数据
						var systemSynchronization = new SystemSynchronizationEntity
						{
							syn_direction = 2,
							syn_status = 0,
							ope_type = "7",  //任务取消
							order_no = mainEntity.device_task_no,
							tray_code = mainEntity.tray_code,
							task_no = mainEntity.task_no,
							ope_time = DateTime.Now,
							cur_pos = mainEntity.end_position,
							err_cnt = 0
						};

						dbResult = dbOpeation.UpdateMainAndInsertSyn(mainEntity, systemSynchronization, out string err);

						msg += $"\r\n\t堆垛机任务与主任务均已取消，数据处理结果={dbResult} {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

						// 发送取消确认信号 + 清 MEM
						signalOk = FinishSignal(num, "3", ref msg);
						if (!signalOk)
						{
							return new TaskConfirmResult(false, msg);
						}
					}
					else
					{
						// ======= 主任务不存在 =========
						msg += $"\r\n\t获取主任务异常，无法通知 WMS 取消";

						signalOk = FinishSignal(num, "3", ref msg);
						msg += $"\r\n\t回复堆垛机取消任务确认结果 {signalOk} {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

						isSuccess = false;
					}
				}
				else
				{
					// ======= 堆垛机任务不存在 =========
					msg += $"\r\n\t获取堆垛机任务异常，请检查设备任务号 [{memDeviceTaskNo}] 的任务状态，dev_task={plcTaskNo}";

					clearTask = new StackerSendToRedis().ClearTask_SingleStation(null, num, out string eMsg);
					msg += $"\r\n\t清除下发的任务信号结果：{clearTask} {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

					signalOk = FinishSignal(num, "3", ref msg);
					msg += $"\r\n\t回复堆垛机取消任务确认结果 {signalOk} {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

					isSuccess = false;
				}
			}
			catch (Exception ex)
			{
				msg += $"\r\n\t[Error]{ex}";
				isSuccess = false;
			}

			msg += $"\r\n[{suffixName}]取消任务结束]End";
			return new TaskConfirmResult(isSuccess, msg);
		}

		
		
		/// <summary>
		/// 统一的信号确认 + 清MEM
		/// </summary>
		private bool FinishSignal(string num, string confirmType, ref string msg)
		{
			StackerSendToRedis redis = new StackerSendToRedis();

			bool confirm = redis.ConfirmTask_SingleStation(confirmType, num, out string confirmErr);
			bool memSend = redis.StackMemTaskInfo("0", "0", "0", "0", num);

			msg += $"\r\n\t[任务完成信号回复及清除MEM记录值],确认信号回复结果{confirm}，清除MEM结果{memSend}{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

			return confirm && memSend;
		}
	}
}
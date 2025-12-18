using System;
using System.Linq;
using GZ.DB.Entity.wcs_yl;
using GZ.DB.Repository.wcs_yl;
using GZ.DB.IRepository.wcs_yl;

namespace GZ.Projects.WCS_YL
{
	/// <summary>
	/// 堆垛机调度任务封装
	/// </summary>
	public class StackerScheduler
	{		
        /// <summary>
        /// 默认构造函数。
        /// </summary>
		public StackerScheduler()
		{
		}
		/// <summary>
		/// 按堆垛机编号调度并下发一条堆垛机任务。
		/// </summary>
		/// <param name="num">堆垛机编号，如01、02</param>
		public void DispatchStackerTask(string num)
		{
			string suffixName = $"TC{num}";
			string msg = $"\r\n\t[{num}号堆垛机任务调度：Start{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}]";
			DateTime startTime = DateTime.Now;
			string lastTaskTypeKey = $"MEM.{suffixName}_LastTaskType";
			bool isLog = false;

			try
			{
				//放置外部判断
				//string modeValue = Conn.YLRedis.GetValue($"{suffixName}.dev_pattern"); // 设备模式 1联机自动; 2手动; 3离线; 4维修;
				//string faultValue = Conn.YLRedis.GetValue($"{suffixName}.dev_error"); // 报警 0正常
				//string stateValue = Conn.YLRedis.GetValue($"{suffixName}.dev_dispatch"); // 1正常 允许下发任务
				//string memResend = Conn.YLRedis.GetValue($"MEM.{suffixName}_Resend"); // 内存重发标志 1重发 0不重发

				//if (modeValue != "1" || faultValue != "0" || stateValue != "1" || memResend == "1")
				//{
				//	msg += $"\r\n\t下发任务{suffixName}堆垛机，状态不满足，跳过{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
				//	return;
				//}

				// plcTaskNo 不为空或不为0就不能下发
				string plcTaskNo = Conn.YLRedis.GetValue($"{suffixName}.dev_taskNo");
				if (!string.IsNullOrEmpty(plcTaskNo) && plcTaskNo != "0")
				{
					msg += $"\r\n\t下发任务{suffixName}堆垛机dev_taskNo不为空或不为0，当前值[{plcTaskNo}]，跳过{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
					return;
				}

				// memTaskNo 不为空或不为0就不能下发 //卡控防止回读plc值比较慢
				string memTaskNo = Conn.YLRedis.GetValue($"MEM.{suffixName}_DeviceTaskNo");
				if (!string.IsNullOrEmpty(memTaskNo) && memTaskNo != "0")
				{
					msg += $"\r\n\t下发任务{suffixName}堆垛机mem_taskNo不为空或不为0，当前值[{memTaskNo}]，跳过{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
					return;
				}

				// 查找所有待下发的任务，优先级+创建时间排序
				var allPendingTasks = new StackerTaskRepository()
					.FindList(t => t.TaskState == "Create" && t.DeviceNo == suffixName)
					.OrderBy(t => t.PriorityLevel)
					.ThenBy(t => t.CreateTime)
					.ToList();

				// =============================================
				// 读取上次任务类型
				// =============================================
				string lastTaskType = Conn.YLRedis.GetValue(lastTaskTypeKey);
				msg += $"\r\n\t上次任务类型：{(string.IsNullOrEmpty(lastTaskType) ? "空" : lastTaskType)}";

				// 统计当前任务中是否存在入库、出库任务
				bool hasInTask = allPendingTasks.Any(t => t.TaskType == "ST_In");
				bool hasOutTask = allPendingTasks.Any(t => t.TaskType == "ST_Out");

				// 统计当前是否存在可执行的出库任务（终点到位）
				bool hasExecutableOutTask = allPendingTasks.Any(t =>
				                                                t.TaskType == "ST_Out" &&
				                                                Conn.YLRedis.GetValue($"{RouteHelper.GetDeviceNo(t.ToLocalNo, "CV")}.plc_ready_{t.ToLocalNo}") == "1");

				// 找到第一个满足执行条件的待下发任务
				StackerTaskEntity pendingTask = null;

				foreach (var task in allPendingTasks)
				{
					if (!string.IsNullOrEmpty(lastTaskType))
					{
						if (lastTaskType == "ST_Out" && hasInTask && task.TaskType != "ST_In")
						{
							continue;
						}
						else if (lastTaskType == "ST_In" && hasOutTask && hasExecutableOutTask && task.TaskType == "ST_In")
						{
							continue;
						}
						// 如果无反向任务或反向任务不可执行，则允许继续当前方向
					}

					if (task.TaskType == "ST_In")
					{
						// 入库任务：必须校验站台条码是否匹配且是否到位
						string deviceNo = RouteHelper.GetDeviceNo(task.FromLocalNo, "CV");
						string fromLocalNoContNo = Conn.YLRedis.GetValue($"{deviceNo}.plc_contNo_{task.FromLocalNo}");
						bool arrived = Conn.YLRedis.GetValue($"{deviceNo}.plc_arrived_{task.FromLocalNo}") == "1";
						if (fromLocalNoContNo == task.TrayCode && arrived)
						{
							pendingTask = task;
							break;
						}
						msg += $"\r\n\t 当前{task.FromLocalNo}入库任务托盘号{task.TrayCode}，线体托盘号{fromLocalNoContNo}，到位信号{arrived}，不能入库";
						continue;
					}

					if (task.TaskType == "ST_Out")
					{
						// 出库任务：必须保证 FIFO（光电满足才下发）
						string deviceNo = RouteHelper.GetDeviceNo(task.ToLocalNo, "CV");
						bool toLocalNoState = Conn.YLRedis.GetValue($"{deviceNo}.plc_ready_{task.ToLocalNo}") == "1";
						if (toLocalNoState)
						{
							var earliestTask = allPendingTasks
								.Where(t => t.TaskType == "ST_Out" && t.ToLocalNo == task.ToLocalNo)
								.OrderBy(t => t.CreateTime)
								.FirstOrDefault();
							if (earliestTask != null)
							{
								pendingTask = earliestTask;
								break;
							}
						}
						continue;
					}

					if (task.TaskType == "ST_Move")
					{
						// 移库任务：直接取最早的
						pendingTask = task;
						break;
					}
				}

				// ================= 下发逻辑 =================
				if (pendingTask != null)
				{
					isLog = true;
					msg += $"\r\n\t找到任务编号{pendingTask.TaskNo}，类型{pendingTask.TaskType}，条码{pendingTask.TrayCode}，开始处理{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

					bool dbResult;
					if (pendingTask.TaskType == "ST_In")
					{
						dbResult = HandleInBoundTask(pendingTask, suffixName, ref msg);
					}
					else
					{
						dbResult = HandleOutOrMoveTask(pendingTask, suffixName, ref msg);
					}

					if (dbResult)
					{
						var stackerSender = new StackerSendToRedis();
						msg += $"\r\n\t开始写入redis {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
						bool redisRes = stackerSender.SendTask_SingleStation(pendingTask, num, out _);
						msg += $"\r\n\tredis写入完成，结果：{(redisRes ? "成功" : "失败")} {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
						if (redisRes)
						{
							// 任务下发成功记录内存
							bool memResult = stackerSender.StackMemTaskInfo(pendingTask.DeviceTaskNo, pendingTask.TrayCode, pendingTask.FromLocalNo, pendingTask.ToLocalNo, num);
							msg += $"\r\n\t更新MEM任务信息结束，结果：{(memResult ? "成功" : "失败")} {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";

							// 任务成功下发后记录类型
							bool writeLastType = Conn.YLRedis.SetValue(lastTaskTypeKey, pendingTask.TaskType, "MEMQueue");
							msg += $"\r\n\t记录本次任务类型：{pendingTask.TaskType}，写入Redis结果：{(writeLastType ? "成功" : "失败")}";
						}
						else
						{
							msg += $"\r\n\t{suffixName}堆垛机任务的redis写入失败 {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
							Conn.YLLog.Error(1, $"[{suffixName}堆垛机任务下发失败]", 1, msg);
						}
					}
				}
				else
				{
					msg += $"\r\n\t未检测到待下发的{suffixName}堆垛机任务 {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
				}
			}
			catch (Exception ex)
			{
				msg += $"\r\n\t[Error：{suffixName}堆垛机任务下发异常] {ex}";
				Conn.YLLog.Error(1, $"[{suffixName}堆垛机任务下发异常]", 1, msg);
			}
			finally
			{
				msg += $"\r\n\t[EndTime] end {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
				msg += $"\r\n\t[SpanTime] 耗时：{(int)(DateTime.Now - startTime).TotalMilliseconds}ms";
				msg += $"\r\n\t[{suffixName}堆垛机任务下发逻辑End]";
				if (isLog)
				{
					Conn.YLLog.Info(1, $"[{suffixName}堆垛机任务下发]", 1, msg);
				}
			}
		}

		/// <summary>
		/// 处理堆垛机任务执行握手。
		/// </summary>
		/// <param name="num">堆垛机编号，如01、02。</param>
		public void HandleStackerHandshake(string num)
		{
			DateTime startTime = DateTime.Now;
			string suffixName = $"TC{num}";
			string msg = $"\r\n\t[{num}号]堆垛机任务下发反馈：Start{startTime:yyyy/MM/dd HH:mm:ss:fff}]";
			bool isLog = false;

			try
			{
				string wcsTaskExe = Conn.YLRedis.GetValue($"{suffixName}.wcs_taskExe");
				string plcTaskExe = Conn.YLRedis.GetValue($"{suffixName}.dev_taskExe");
				string plcTaskNo = Conn.YLRedis.GetValue($"{suffixName}.dev_task");

				msg += $"\r\n\tWCS执行信号[{wcsTaskExe}]，PLC执行信号[{plcTaskExe}] [{DateTime.Now:yyyy/MM/dd HH:mm:ss}]";

				if (wcsTaskExe == "1" && plcTaskExe == "1")
				{
					IStackerTaskRepository stackerRepo = new StackerTaskRepository();
					IMainMissionRepository mainMissionRepo = new MainMissionRepository();
					DBOpeation dbOpeation = new DBOpeation();

					var stackerTask = stackerRepo
						.FindList(t => t.DeviceNo == suffixName && t.DeviceTaskNo == plcTaskNo)
						.OrderByDescending(t => t.CreateTime)
						.LastOrDefault();

					if (stackerTask != null)
					{
						var mainEntity = mainMissionRepo
							.FindList(t => t.device_task_no == stackerTask.DeviceTaskNo && t.task_no == stackerTask.TaskNo)
							.OrderByDescending(t => t.CreateTime)
							.LastOrDefault();

						if (mainEntity != null)
						{
							isLog = true;
							DateTime now = DateTime.Now;
							string dbErr = string.Empty;

							stackerTask.TaskState = "Doing";
							stackerTask.TaskStartTime = now;
							stackerTask.LastUpdatedTime = now;
							mainEntity.MissionState = "Running";
							mainEntity.MissionStartTime = now;
							mainEntity.LastUpdatedTime = now;

							bool dbResult = dbOpeation.UpdateStackerAndMainTask(stackerTask, mainEntity, out dbErr);
							if (dbResult)
							{
								var stackerSendToRedis = new StackerSendToRedis();
								bool clearTaskResult = stackerSendToRedis.ClearTask_SingleStation(null, num, out string redisErr);

								msg += $"\r\n\tRedis清除任务信号[{clearTaskResult}][{DateTime.Now:yyyy/MM/dd HH:mm:ss}]";
								if (!clearTaskResult)
								{
									msg += $"\r\n\t [plcTaskNo]握手交互清除自身指令Redis写入失败：{redisErr}[{DateTime.Now:yyyy/MM/dd HH:mm:ss}]";
								}

								Conn.YLLog.Error(1, $"{num}号机任务下发反馈异常", 1, msg);
							}
						}
						else
						{
							msg += $"\r\n\t[主任务信息异常]：无法查询 mainTask 主任务，PLC携带任务号[{plcTaskNo}]，WMS任务号[{stackerTask.TaskNo}]";
						}
					}
					else
					{
						msg += $"\r\n\t[堆垛机信息异常]：无法查询 stackerTask 堆垛机任务，PLC携带任务号[{plcTaskNo}]";
					}
				}

				if (wcsTaskExe == "1" && plcTaskExe == "2")
				{
					IStackerTaskRepository stackerRepo = new StackerTaskRepository();
					IMainMissionRepository mainMissionRepo = new MainMissionRepository();
					DBOpeation dbOpeation = new DBOpeation();

					var stackerTask = stackerRepo
						.FindList(t => t.DeviceNo == suffixName && t.DeviceTaskNo == plcTaskNo)
						.OrderByDescending(t => t.CreateTime)
						.LastOrDefault();

					if (stackerTask != null)
					{
						var mainEntity = mainMissionRepo
							.FindList(t => t.device_task_no == stackerTask.DeviceTaskNo && t.task_no == stackerTask.TaskNo)
							.OrderByDescending(t => t.CreateTime)
							.LastOrDefault();

						if (mainEntity != null)
						{
							isLog = true;
							DateTime now = DateTime.Now;
							string dbErr = string.Empty;

							stackerTask.TaskState = "Cancel";
							mainEntity.MissionState = "Cancel";

							stackerTask.TaskStartTime = now;
							stackerTask.LastUpdatedTime = now;
							mainEntity.MissionStartTime = now;
							mainEntity.LastUpdatedTime = now;

							var sysSync = new SystemSynchronizationEntity
							{
								syn_direction = 2,
								syn_status = 0,
								ope_type = "7",
								task_no = stackerTask.TaskNo,
								tray_code = stackerTask.TrayCode,
								order_no = stackerTask.DeviceTaskNo,
								cur_pos = stackerTask.FromLocalNo,
								ope_time = now,
								err_cnt = 0
							};

							bool dbResult = dbOpeation.UpdateMainAndStackInsertSysTask(mainEntity, stackerTask, sysSync, out dbErr);
							msg += $"\r\n\t数据库更新任务[{stackerTask.TaskNo}]并通知WMS取结果：{dbResult} + dbErr[{DateTime.Now:yyyy/MM/dd HH:mm:ss}]";
							if (dbResult)
							{
								var stackerSendToRedis = new StackerSendToRedis();
								bool clearTaskResult = stackerSendToRedis.ClearTask_SingleStation(null, num, out string redisErr);
								bool redisMemResult = stackerSendToRedis.StackMemTaskInfo("0", "0", "0", "0", num);

								msg += $"\r\n\tRedis清除任务信号[{clearTaskResult}]，内部记录清空[{redisMemResult}][{DateTime.Now:yyyy/MM/dd HH:mm:ss}]";
								if (!clearTaskResult || !redisMemResult)
								{
									msg += $"\r\n\t[plcTaskNo]握手交互清除自身并置位MEMRedis写入失败：{redisErr}[{DateTime.Now:yyyy/MM/dd HH:mm:ss}]";
									Conn.YLLog.Error(1, $"{num}号机任务下发反馈异常", 1, msg);
								}
							}
						}
						else
						{
							msg += $"\r\n\t[主任务信息异常]：无法查询 mainTask 主任务，PLC携带任务号[{plcTaskNo}]，WMS任务号[{stackerTask.TaskNo}]";
						}
					}
					else
					{
						msg += $"\r\n\t[堆垛机信息异常]：无法查询 stackerTask 堆垛机任务，PLC携带任务号[{plcTaskNo}]";
					}
				}
			}
			catch (Exception ex)
			{
				Conn.YLLog.Error(1, $"{num}号机任务下发反馈异常", 1, ex.ToString());
			}
			finally
			{
				msg += $"\r\n\t[{num}号机任务反馈]End{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
				msg += $"\r\n\t[{num}号机任务反馈]耗时：{(DateTime.Now - startTime).TotalMilliseconds}ms";
				if (isLog)
				{
					Conn.YLLog.Info(1, $"{num}号机任务下发反馈处理完成", 1, msg);
				}
			}
		}
		
		private static bool HandleInBoundTask(StackerTaskEntity pendingTask, string suffixName, ref string msg)
		{
			pendingTask.TaskState = "Send";
			pendingTask.TaskStartTime = DateTime.Now;
			pendingTask.LastUpdatedTime = DateTime.Now;
			var systemSynchronization = new SystemSynchronizationEntity
			{
				syn_direction = 2,
				syn_status = 0,
				ope_type = "1",
				task_no = pendingTask.TaskNo,
				order_no = pendingTask.DeviceTaskNo,
				tray_code = pendingTask.TrayCode,
				cur_pos = pendingTask.FromLocalNo,
				ope_device_no = suffixName,
				ope_time = DateTime.Now,
				err_cnt = 0
			};

			bool dbResult = new DBOpeation().UpdateStackInsertSys(pendingTask, systemSynchronization, out string errorMsg);
			msg += $"\r\n\t更新任务信息结束，结果：{(dbResult ? "成功" : "失败")} {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff} {errorMsg}";
			return dbResult;
		}

		private static bool HandleOutOrMoveTask(StackerTaskEntity pendingTask, string suffixName, ref string msg)
		{
			pendingTask.TaskState = "Send";
			pendingTask.TaskStartTime = DateTime.Now;
			pendingTask.LastUpdatedTime = DateTime.Now;

			var systemSynchronization = new SystemSynchronizationEntity
			{
				syn_direction = 2,
				syn_status = 0,
				ope_type = "1",
				task_no = pendingTask.TaskNo,
				order_no = pendingTask.DeviceTaskNo,
				tray_code = pendingTask.TrayCode,
				cur_pos = pendingTask.FromLocalNo,
				ope_device_no = suffixName,
				ope_time = DateTime.Now,
				err_cnt = 0
			};

			bool dbResult = new DBOpeation().UpdateStackInsertSys(pendingTask, systemSynchronization, out string errorMsg);
			msg += $"\r\n\t更新任务信息结束，结果：{(dbResult ? "成功" : "失败")} {DateTime.Now:yyyy/MM/dd HH:mm:ss:fff} {errorMsg}";
			return dbResult;
		}
	}
}
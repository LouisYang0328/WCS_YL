using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.ServiceModel;
using GZ.DB.Entity.wcs_yl;
using GZ.DB.IRepository.wcs_yl;
using GZ.DB.Repository.wcs_yl;
using GZ.Common.Data;
using GZ.DB.Entity;
using System.Configuration;

namespace GZ.Projects.WCS_YL
{
	
	/// <summary>
	/// 任务下发结果
	/// </summary>
	public class TaskDecomposition
	{
		public bool Success { get; set; }
		public string Message { get; set; }
	}
	/// <summary>
	/// 数据库操作，修改删除提交
	/// </summary>
	public class DBOpeation
	{
		public TaskDecomposition StackerTaskOutCreate(List<MainMissionEntity> missList)
		{
			try
			{
				IMainMissionRepository mainMission = new MainMissionRepository();
				for (int i = 0; i < missList.Count; i++)
				{
					//首先去找这条任务有没有前置任务需要执行
					string pre_task_no = missList[i].pre_task_no;
					if (!string.IsNullOrEmpty(pre_task_no))
					{
						MainMissionEntity missPre =
							mainMission.FindEntity(t => t.MissionState == "Create" && t.task_no == pre_task_no);
						if (missPre != null)
						{
							missList[i] = missPre;
						}
					}

					#region 堆垛机任务处理 //LXK-01-001-01 ,TPK-05-001-01
					int fromX = 0, fromY = 0, fromZ = 0, fromP = 0, toX = 0, toY = 0, toZ = 0, toP = 0;
					string startPosition = missList[i].start_position;
					string endPosition = missList[i].end_position;
					string toLocalNo = "";
					string deviceTaskType = "";
					if (missList[i].task_type == "ST_Move")
					{
						//如果是移库，终点位置就是查找出来的位置直接解析TPK-05-001-01
						string[] pos = endPosition.Split('-'); //TPK-05-001-01
						toX = int.Parse(pos[1]); //排
						toY = int.Parse(pos[2]); //列
						toZ = int.Parse(pos[3]); //层
						toLocalNo = endPosition;
						deviceTaskType = "3";
					}
					else
					{
						string flow = RouteHelper.GetNextStn(startPosition, endPosition);
						if (!string.IsNullOrEmpty(flow))
						{
							//库前放货位
							string[] pos = ConfigurationManager.AppSettings[flow].Split('-');
							if(pos.Length == 4)
							{
								toX = int.Parse(pos[1]); //排
								toY = int.Parse(pos[2]); //列
								toZ = int.Parse(pos[3]); //层
								toLocalNo = flow;
								deviceTaskType = "2";
							}
							else
							{
								return new TaskDecomposition{Success = false , Message = $"堆垛机任务创建失败: {missList[i].task_no}，未正确配置出库口站台对应堆垛机排列层信息"};
							}
						}
						else
						{
							//路线异常
							return new TaskDecomposition{Success = false , Message = $"堆垛机任务创建失败: {missList[i].task_no}，未找到对应的出库路线，当前位置: {startPosition}，目标位置: {endPosition}"};
						}
					}
					//起始站台
					string[] startPos = missList[i].start_position.Split('-');
					fromX = int.Parse(startPos[1]); //排
					fromY = int.Parse(startPos[2]); //列
					fromZ = int.Parse(startPos[3]); //层

					StackerTaskEntity stack = new StackerTaskEntity();
					stack.OrderType = missList[i].order_type;
					stack.OrderNo = missList[i].order_no;
					stack.TaskType = missList[i].task_type;
					stack.TaskNo = missList[i].task_no;
					stack.DeviceNo = RouteHelper.GetDeviceNo(toLocalNo, "TC");
					stack.DeviceTaskType = deviceTaskType;
					stack.DeviceTaskNo = missList[i].device_task_no;
					stack.FromX = fromX;
					stack.FromY = fromY;
					stack.FromZ = fromZ;
					stack.FromP = fromP;

					stack.FromLocalNo = missList[i].start_position;
					stack.ToX = toX;
					stack.ToY = toY;
					stack.ToZ = toZ;
					stack.ToP = toP;
					stack.ToLocalNo = toLocalNo;
					stack.TaskState = "Create";
					stack.PriorityLevel = 0;
					stack.TrayType = missList[i].tray_type;
					stack.TrayCode = missList[i].tray_code;
					stack.GroupNo = missList[i].group_no;
					stack.CreateTime = DateTime.Now;
					stack.LastUpdatedTime = DateTime.Now;
					stack.TaskNo_Per = missList[i].pre_task_no;

					#endregion

					missList[i].MissionState = "Doing";
					missList[i].InteStationNo = toLocalNo;
					

					//提交堆垛机任务
					using (var db = new RepositoryBase().BeginTrans())
					{
						if (missList[i] != null)
						{
							db.Update(missList[i]);
						}
						if (stack != null)
						{
							db.Insert(stack);
						}
						db.Commit();
					}
				}
				return new TaskDecomposition{Success = true , Message = "堆垛机出库任务创建成功"};
			}
			catch (Exception ex)
			{
				return new TaskDecomposition{Success = false , Message = $"堆垛机出库任务创建失败: {ex.Message}"};
			}
		}

		public System.Int32 StackerTaskIdGen(System.Int32 TaskType)
		{
			//沿用鑫百勤
			int maxID = 0;
			var taskList = new StackerTaskRepository().FindDataTable("select ID from pcm_StackerTask order by ID desc ",
			                                                         new System.Data.Common.DbParameter[0]);

			if (taskList != null && taskList.Rows.Count > 0)
				//update 20240808
				maxID = (int)taskList.Rows[0][0] % 10000;

			string id = maxID.ToString().PadLeft(4, '0');
			string prex = string.Empty;
			prex = "2";
			return Convert.ToInt32(prex + id);
		}

		public bool UpdateStackerAndMainTask(StackerTaskEntity entity, MainMissionEntity mainList, out string err)
		{
			err = "";
			//int dbResult = 0;
			try
			{
				using (var db = new RepositoryBase().BeginTrans())
				{
					if (mainList != null)
					{
						db.Update(mainList);
					}

					if (entity != null)
					{
						db.Update(entity);
					}

					int dbResult = db.Commit();
					return dbResult >= 1;
				}
			}
			catch (Exception ex)
			{
				err = ex.ToString();
				return false;
			}
		}
		
		public bool UpdateMainInsertSys(MainMissionEntity entity,SystemSynchronizationEntity system ,out string err)
		{
			err = "";
			//int dbResult = 0;
			try
			{
				using (var db = new RepositoryBase().BeginTrans())
				{
					if (entity != null)
					{
						db.Update(entity);
					}
					if (system != null)
					{
						db.Insert(system);
					}
					int dbResult = db.Commit();
					return dbResult >= 1;
				}
			}
			catch (Exception ex)
			{
				err = ex.ToString();
				return false;
			}
		}

		public bool UpdateMainTask(MainMissionEntity entity, out string err)
		{
			err = "";
			//int dbResult = 0;
			try
			{
				using (var db = new RepositoryBase().BeginTrans())
				{
					if (entity != null)
					{
						db.Update(entity);
					}

					int dbResult = db.Commit();
					return dbResult >= 1;
				}
			}
			catch (Exception ex)
			{
				err = ex.ToString();
				return false;
			}
		}
		public bool UpdateStackerTask(StackerTaskEntity entity, out string err)
		{
			err = "";
			int dbResult = 0;
			try
			{
				using (var db = new RepositoryBase().BeginTrans())
				{
					if (entity != null)
					{
						db.Update(entity);
					}

					dbResult = db.Commit();
					return dbResult >= 1;
				}
			}
			catch (Exception ex)
			{
				err = ex.ToString();
				return false;
			}
		}

		/// <summary>
		/// 根据主任务获取堆垛机子任务
		/// </summary>
		public StackerTaskEntity GetChildTask(MainMissionEntity mainEntity)
		{
			IStackerTaskRepository stackerRepo = new StackerTaskRepository();
			StackerTaskEntity stackerEntity =
				stackerRepo.FindEntity(t => t.TrayCode == mainEntity.tray_code && t.TaskNo == mainEntity.task_no);

			return stackerEntity;
		}

		/// <summary>
		/// 修改主任务与生成堆垛机任务
		/// </summary>
		/// <param name="mainET"></param>
		/// <param name="stackerET"></param>
		/// <param name="msg"></param>
		/// <returns></returns>
		public bool UpdateMainAndInsertStack(MainMissionEntity mainET, StackerTaskEntity stackerET, out string msg)
		{
			msg = "";
			try
			{
				int dbResult = 0;
				using (var db = new RepositoryBase().BeginTrans())
				{
					if (mainET != null)
					{
						db.Update(mainET);
					}

					if (stackerET != null)
					{
						db.Insert(stackerET);
					}

					dbResult = db.Commit();
				}

				return dbResult >= 1;
			}
			catch (Exception ex)
			{
				msg = ex.Message;
				return false;
			}
		}
		
		
		/// <summary>
		/// 修改主任务、堆垛机任务并生成WMS同步表
		/// </summary>
		/// <param name="mainEntity"></param>
		/// <param name="stackEntity"></param>
		/// <param name="synEntity"></param>
		/// <param name="err"></param>
		/// <returns></returns>
		public bool UpdateMainAndStackInsertSysTask(MainMissionEntity mainEntity, StackerTaskEntity stackEntity,
		                                            SystemSynchronizationEntity synEntity, out string err)
		{
			err = "";
			try
			{
				using (var db = new RepositoryBase().BeginTrans())
				{
					if (mainEntity != null)
					{
						db.Update(mainEntity);
					}

					if (stackEntity != null)
					{
						db.Update(stackEntity);
					}

					if (synEntity != null)
					{
						db.Insert(synEntity);
					}

					int dbResult = db.Commit();
					return dbResult >= 1;
				}
			}
			catch (Exception ex)
			{
				err = ex.ToString();
				return false;
			}
		}
		/// <summary>
		/// 修改主任务与生成堆垛机任务
		/// </summary>
		/// <param name="mainEntity"></param>
		/// <param name="stackEntity"></param>
		/// <param name="msg"></param>
		/// <returns></returns>
		public bool UpdateMainAndUpdateStack(MainMissionEntity mainEntity, StackerTaskEntity stackEntity, out string msg)
		{
			msg = "";
			try
			{
				int dbResult = 0;
				using (var db = new RepositoryBase().BeginTrans())
				{
					if (mainEntity != null)
					{
						db.Update(mainEntity);
					}

					if (stackEntity != null)
					{
						db.Update(stackEntity);
					}

					dbResult = db.Commit();
				}

				return dbResult >= 1;
			}
			catch (Exception ex)
			{
				msg = ex.Message;
				return false;
			}
		}
		/// <summary>
		/// 修改主任务与新增同步表信息
		/// </summary>
		/// <param name="mainET"></param>
		/// <param name="systemET"></param>
		/// <param name="msg"></param>
		/// <returns></returns>
		public bool UpdateMainAndInsertSyn(MainMissionEntity mainET, SystemSynchronizationEntity systemET,
		                                   out string msg)
		{
			msg = "";
			try
			{
				//int dbResult = 0;
				using (var db = new RepositoryBase().BeginTrans())
				{
					if (mainET != null)
					{
						db.Update(mainET);
					}

					if (systemET != null)
					{
						db.Insert(systemET);
					}

					int dbResult = db.Commit();
					return dbResult >= 1;
				}
			}
			catch (Exception ex)
			{
				msg = ex.Message;
				return false;
			}
		}

		
		/// <summary>
		/// 修改主任务与新增同步表信息
		/// </summary>
		/// <param name="mainET"></param>
		/// <param name="systemET"></param>
		/// <param name="msg"></param>
		/// <returns></returns>
		public bool UpdateStackInsertSys( StackerTaskEntity stackEntity, SystemSynchronizationEntity systemET,
		                                 out string msg)
		{
			msg = "";
			try
			{
				//int dbResult = 0;
				using (var db = new RepositoryBase().BeginTrans())
				{
					if (stackEntity != null)
					{
						db.Update(stackEntity);
					}

					if (systemET != null)
					{
						db.Insert(systemET);
					}

					int dbResult = db.Commit();
					return dbResult >= 1;
				}
			}
			catch (Exception ex)
			{
				msg = ex.Message;
				return false;
			}
		}
		
		/// <summary>
		/// 更新wms同步表信息
		/// </summary>
		/// <returns></returns>
		public bool SystemSynchronizationUpdate(SystemSynchronizationEntity entity, out string err)
		{
			err = "";
			int dbResult = 0;
			try
			{
				using (var db = new RepositoryBase().BeginTrans())
				{
					if (entity != null)
					{
						db.Update(entity);
					}

					dbResult = db.Commit();
					return dbResult >= 1;
				}
			}
			catch (Exception ex)
			{
				err = ex.ToString();
				return false;
			}
		}

		/// <summary>
		/// 新增wms同步表信息
		/// </summary>
		/// <returns></returns>
		public bool SystemSynchronizationInsert(SystemSynchronizationEntity entity, out string err)
		{
			err = "";
			//int dbResult = 0;
			try
			{
				using (var db = new RepositoryBase().BeginTrans())
				{
					if (entity != null)
					{
						db.Insert(entity);
					}

					int dbResult = db.Commit();
					return dbResult >= 1;
				}
			}
			catch (Exception ex)
			{
				err = ex.ToString();
				return false;
			}
		}
	}
}
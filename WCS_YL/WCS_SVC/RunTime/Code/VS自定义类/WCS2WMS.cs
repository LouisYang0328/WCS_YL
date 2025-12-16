using GZ.Common.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Reflection;
using GZ.Common.Code;
using System.Net.Http;
using System.Security.Policy;
using System.Configuration;
using GZ.Modular.Redis;
using GZ.DB.Repository.WCS;
using GZ.DB.IRepository.WCS;
using GZ.DB.Entity.WCS;
using GZ.Projects.测试服务;

//wmstowcs
#region 001.推送任务

[Serializable]
	public class ToWmsPushTaskRequest
	{
		public string reqId { get; set; }
		public string reqTime { get; set; }
		public string taskType { get; set; }
		public string taskNo { get; set; }
		public string groupNo { get; set; }
		public string from { get; set; }
		public string to { get; set; }
		public string cntrNo { get; set; }
		public string cntrType { get; set; }
		//public List<object> extData;
	}

	[Serializable]
	public class ToWmsPushTaskResponse
	{
		public string code { get; set; }
		public string msg { get; set; }
		public List<Taskinfo> data;
	}

	public class Taskinfo
	{
		public string TaskNo { get; set; }
		public string reverse { get; set; }
	}
	#endregion

	#region 002.取消任务

	[Serializable]
	public class ToWmsCancelTaskRequest
	{
		public string reqId { get; set; }
		public string taskNo { get; set; }
		public string reqTime { get; set; }
	}

	[Serializable]
	public class ToWmsCancelTaskResponse
	{
		public string code { get; set; }
		public string msg { get; set; }
		public List<Taskinfo> data;
	}

	#endregion

	#region 004.修改任务优先级

	[Serializable]
	public class ToWmschangePriorityTaskRequest
	{
		public string reqId { get; set; }
		public string taskNo { get; set; }
		public string reqTime { get; set; }
		public string priority { get; set; }
	}

	[Serializable]
	public class ToWmschangePriorityTaskResponse
	{
		public string code { get; set; }
		public string msg { get; set; }
		public List<Taskinfo> data;
	}

	#endregion

	//wcstowms
	#region 006.任务状态反馈

	[Serializable]
	public class ToWmsNotifyTaskStatusRequest
	{
		public string reqId { get; set; }
		public string reqTime { get; set; }
		public string taskNo { get; set; }
		public string subTaskNo { get; set; }
		public string status { get; set; }
		public string deviceNo { get; set; }
		public string loc { get; set; }
		public string errCode { get; set; }
	}

	[Serializable]
	public class ToWmsNotifyTaskStatusResponse
	{
		public int code { get; set; }
		public string msg { get; set; }
		public object data { get; set; }
	}
	#endregion

	#region 009.设备报警上报

	[Serializable]
	public class ToWmsNotifyAlarmRequest
	{
		public string reqId { get; set; }
		public string reqTime { get; set; }
		public string deviceNo { get; set; }
		public string errCode { get; set; }
		public string errMsg { get; set; }
	}

	//[Serializable]
	//public class deviceinfo
	//{
	//	public string deviceNo;
	//	public string errCode;
	//	public string errMsg;
	//}

	[Serializable]
	public class ToWmsNotifyAlarmResponse
	{
		public int code { get; set; }
		public string msg { get; set; }
		public object data { get; set; }
	}
	#endregion

	//#region 货位状态反馈
	//[Serializable]
	//public class LocStateFeedBackRequest
	//{
	//	public string loc_code;
	//	public string type;
	//	public string req_no;
	//}
	//[Serializable]
	//public class LocStateFeedBackResponse
	//{
	//	public string result_flag;
	//	public string err_msg;
	//}
	//#endregion

	#region 货位是否可用
	[Serializable]
	public class LocStateRequest
	{
		public string loc_code { get; set; }
		public string type { get; set; }
		public string req_no { get; set; }
		public string taskNo { get; set; }
	}
	[Serializable]
	public class LocStateResponse
	{
		//public int result_flag { get; set; }
		//public string err_msg { get; set; }
		public int code { get; set; }
		public string msg { get; set; }
		public List<Taskinfo> data;
	}
	public class StationData
	{
		public string StationPLCName { get; set; }
		public string StationType { get; set; }
		public string StationGroupName { get; set; }
	}
	#endregion

	#region 站台状态
	public class StationStateRequest
	{
		/// <summary>
		/// 请求ID
		/// </summary>
		public string reqId { get; set; }

		/// <summary>
		/// 请求时间 (格式: yyyy-MM-dd HH:mm:ss)
		/// </summary>
		public string reqTime { get; set; }

		/// <summary>
		/// 设备编号列表
		/// </summary>
		public List<string> deviceNoList { get; set; } = new List<string>();
	}
	public class StationStateResponse
	{
		/// <summary>
		/// 状态码 (0表示成功)
		/// </summary>
		public int code { get; set; }

		/// <summary>
		/// 消息描述
		/// </summary>
		public string msg { get; set; }

		/// <summary>
		/// 返回数据集合
		/// </summary>
		public List<DeviceStatusData> data { get; set; } = new List<DeviceStatusData>();
	}
	/// <summary>
	/// 设备状态数据实体
	/// </summary>
	public class DeviceStatusData
	{
		public string deviceNo { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int workStatus { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int photoStatus { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int manualStatus { get; set; }
	}
	public class StationStateData
	{
		public string IsInStock { get; set; }
		public string ModeState { get; set; }
	}
	#endregion


	public static class ToWmsHandle
	{
		/// <summary>
		/// 001.推送任务
		/// </summary>
		/// <param name="requestJson"></param>
		/// <returns></returns>
		public static string PushTask(string requestJson)
		{
			var response = new ToWmsPushTaskResponse();
			string respJson = string.Empty;
			try
			{
				var request = JsonConvert.DeserializeObject<ToWmsPushTaskRequest>(requestJson);
				if (request == null)
				{
					response.code = "500";
					response.msg = "解析请求报文失败";
					response.data = null;
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}

				MainMissionEntity mission = new MainMissionEntity();
				mission.order_type = request.taskType.ToString();
				mission.order_no = request.reqId;
				mission.task_type = request.taskType.ToString();
				mission.task_no = request.taskNo;
				mission.group_no = request.groupNo;
				mission.start_position = request.from;
				mission.end_position = request.to;
				mission.tray_code = request.cntrNo;
				mission.material_no = request.cntrNo;
				mission.tray_type = request.cntrType;
				mission.order_time = Convert.ToDateTime(request.reqTime);
				mission.ope_time = DateTime.Now;
				mission.LastUpdatedTime = DateTime.Now;
				mission.MissionState = "Create";
				mission.CreateTime = DateTime.Now;
				mission.MissionStartTime = DateTime.Now;
				mission.InteStationNo = request.from;
				//mission.tunnel_no=//?？根据库位解析出对应的巷道/库区
				//mission.warehouse_no=//?？
				MainMissionEntity repeat = null;
				IMainMissionRepository mainMissionRepository = new MainMissionRepository();
				repeat=mainMissionRepository.FindEntity(t => t.task_no == request.taskNo);


				if (repeat == null)
				{
                AutoScanApp autosan = new AutoScanApp();
					bool mainbl = autosan.ADDMainMission(mission, out string mainessmage);
					if (!mainbl)
					{
						response.code = "424";
						response.msg = "操作失败:数据库更新失败";
						response.data = null;
						respJson = JsonConvert.SerializeObject(response);
						return respJson;
					}
				}
				else
				{
					response.code = "423";
					response.msg = "操作失败:任务重复";
					response.data = null;
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}

				response.code = "200";
				response.msg = "成功";
				response.data = null;
				respJson = JsonConvert.SerializeObject(response);
				return respJson;
			}
			catch (Exception ex)
			{
				response.code = "500";
				response.msg = "操作异常,异常原因" + ex;
				response.data = null;
				respJson = JsonConvert.SerializeObject(response);
				return respJson;
			}

		}
		/// <summary>
		/// 002.取消任务
		/// </summary>
		/// <param name="requestJson"></param>
		/// <returns></returns>
		public static string CancelTask(string requestJson)
		{
			var response = new ToWmsCancelTaskResponse();
			string respJson = string.Empty;
			try
			{
				var reqStr = JsonConvert.DeserializeObject<ToWmsCancelTaskRequest>(requestJson);
				if (reqStr == null)
				{
					response.code = "421";
					response.msg = "解析请求报文失败";
					response.data = null;
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}
				MainMissionEntity repeat = null;
				IMainMissionRepository mainMissionRepository = new MainMissionRepository();
				repeat= mainMissionRepository.FindEntity(t => t.task_no == reqStr.taskNo);
				if (repeat == null)
				{
					response.code = "422";
					response.msg = "操作失败:不存在该任务";
					//response.data.Add(new Taskinfo { TaskNo = reqStr.taskNo, reverse = "" });
					response.data = null;
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}

				if (repeat.MissionState != "Create")    //任务执行状态：执行中或者之后的状态时无法取消
				{
					response.code = "423";
					response.msg = "操作失败:当前任务不允许取消";
					//response.data.Add(new Taskinfo { TaskNo = reqStr.taskNo, reverse = "" });
					response.data = null;
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}

				//repeat.task_priority = Convert.ToDecimal(reqStr.priority); //修改任务优先级
				repeat.MissionState = "Cancel"; //任务执行状态 "6：异常取消"
				repeat.LastUpdatedTime = DateTime.Now;
				repeat.MissionEndTime = DateTime.Now;
				AutoScanApp autosan = new AutoScanApp();
				bool mainbl = autosan.UpMainMission(repeat, out string mainessmage);
				if (mainbl == true)
				{
					response.code = "200";
					response.msg = "操作成功:任务取消成功/success";
					//response.data.Add(new Taskinfo { TaskNo = reqStr.taskNo, reverse = "" });
					response.data = null;
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}
				else
				{
					response.code = "424";
					response.msg = "操作失败:数据库更新失败";
					//response.data.Add(new Taskinfo { TaskNo = reqStr.taskNo, reverse = "" });
					response.data = null;
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}
			}
			catch (Exception ex)
			{
				response.code = "500";
				response.msg = "操作异常,异常原因" + ex.Message;
				response.data = null;
				respJson = JsonConvert.SerializeObject(response);
				return respJson;
			}
		}

		/// <summary>
		/// 004.修改任务优先级
		/// </summary>
		/// <param name="requestJson"></param>
		/// <returns></returns>
		public static string ChangePriorityTask(string requestJson)
		{

			var response = new ToWmschangePriorityTaskResponse();
			string respJson = string.Empty;
			try
			{
				var reqStr = JsonConvert.DeserializeObject<ToWmschangePriorityTaskRequest>(requestJson);
				if (reqStr == null)
				{
					response.code = "421";
					response.msg = "操作失败:解析请求报文失败";
					response.data = null;
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}
				MainMissionEntity repeat = null;
				IMainMissionRepository mainMissionRepository = new MainMissionRepository();
				repeat = mainMissionRepository.FindEntity(t => t.task_no == reqStr.taskNo);
				if (repeat == null)
				{
					response.code = "422";
					response.msg = "操作失败:不存在该任务";
					response.data = null;
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}

				repeat.task_priority = Convert.ToInt32(reqStr.priority); //修改任务优先级

				AutoScanApp autosan = new AutoScanApp();
				List<MainMissionEntity> mainList = new List<MainMissionEntity>();
				mainList.Add(repeat);
				bool mainbl = autosan.UpMainMission(mainList, out string mainessmage);
				if (mainbl == true)
				{
					response.code = "200";
					response.msg = "操作成功:任务取消成功/success";
					response.data = null;
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}
				else
				{
					response.code = "424";
					response.msg = "操作失败:数据库更新失败";
					response.data = null;
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}
			}
			catch (Exception ex)
			{
				response.code = "500";
				response.msg = "操作异常,异常原因" + ex.Message;
				response.data = null;
				respJson = JsonConvert.SerializeObject(response);
				return respJson;
			}
		}

		/// <summary>
		/// 货位是否可用
		/// </summary>
		/// <param name="requestJson"></param>
		/// <returns></returns>
		public static string LocState(string requestJson)
		{
			var response = new LocStateResponse();
			string respJson = string.Empty;
			try
			{
				var reqStr = JsonConvert.DeserializeObject<LocStateRequest>(requestJson);
				if (reqStr == null)
				{
					response.code = 421;
					response.msg = "操作失败:解析请求报文失败";
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}
				if (string.IsNullOrEmpty(reqStr.loc_code))
				{
					response.code = 422;
					response.msg = "操作失败:站台不能为空";
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}
				if (string.IsNullOrEmpty(reqStr.type))
				{
					response.code = 423;
					response.msg = "操作失败:请求类型不能为空";
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}
				StationData stationData = null;
				// 检查字典中是否存在该 key
				if (AGVStationPlc.TryGetValue(reqStr.loc_code, out List<StationData> stationDataList))
				{
					// 校验 StationType 是否匹配
					stationData = stationDataList.FindLast(s => s.StationType == reqStr.type);
					if (stationData == null)//(stationData.StationType != reqStr.type)
					{
						response.code = 424;
						response.msg = "操作失败:站台类型不匹配";
						respJson = JsonConvert.SerializeObject(response);
						return respJson;
					}
				}
				else
				{
					response.code = 425;
					response.msg = "操作失败:站台配置不存在";
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}
				string deviceNo = stationData.StationPLCName;
				string groupName = stationData.StationGroupName;
				//添加数据库
				if(reqStr.type=="1" || reqStr.type=="2")
				{
					DictEntity dictEy = null;
					IDictRepository dictRepository = new DictRepository();
					dictEy = dictRepository.FindList(t => t.DictType == "AGVReqTask" && t.DictValue == reqStr.type && t.DictName == reqStr.loc_code).FirstOrDefault();

					if (dictEy==null)
					{
						response.code = 425;
						response.msg = "操作失败:缺少基础数据";
						respJson = JsonConvert.SerializeObject(response);
						return respJson;
					}
					dictEy.DictStr=reqStr.taskNo;
					AutoScanApp autosan = new AutoScanApp();
					bool mainbl = autosan.DictUpdate(dictEy, out string mainessmage);
					if (!mainbl)
					{
						response.code = 426;
						response.msg = "操作失败:数据修改失败";
						respJson = JsonConvert.SerializeObject(response);
						return respJson;
					}
				}
				#region 写入plc
				string setrqu = $"{deviceNo}Queue";
				string groupNameText = $"{deviceNo}.{groupName}";
				string paramNameSent = $"{groupNameText}_REQ";
				string setLast = $"{groupNameText}_REQ_LAST";

				WriteGroupEntity group = new WriteGroupEntity
				{
					groupName = groupNameText,
					queueStatus = 1,
					queueTime = DateTime.Now,
					writeList = new List<ParamData>
					{
						new ParamData { paramName = paramNameSent, paramValue = "1" }
					}
				};
				bool result = Conn.WCSRedis.SetQueue(group, setrqu, setLast, "1");
				if (result)
				{
					response.code = 200;
					response.msg = "操作成功:请求成功";
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}
				else
				{
					response.code = 426;
					response.msg = "操作失败:写入PLC失败";
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}
				#endregion
			}
			catch (Exception ex)
			{
				response.code = 500;
				response.msg = "操作异常,异常原因" + ex.Message;
				respJson = JsonConvert.SerializeObject(response);
				return respJson;
			}
		}
		/// <summary>
		/// 获取站台状态
		/// </summary>
		/// <param name="requestJson"></param>
		/// <returns></returns>
		public static string StationState(string requestJson)
		{
			var response = new StationStateResponse();
			string respJson = string.Empty;
			try
			{
				var reqStr = JsonConvert.DeserializeObject<StationStateRequest>(requestJson);
				if (reqStr == null)
				{
					response.code = 421;
					response.msg = "操作失败:解析请求报文失败";
					response.data = null;
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}
				if (reqStr.deviceNoList == null || reqStr.deviceNoList.Count <= 0)
				{
					response.code = 422;
					response.msg = "操作失败:解析请求报文失败,未存在对应的站台";
					response.data = null;
					respJson = JsonConvert.SerializeObject(response);
					return respJson;
				}
				List<DeviceStatusData> dataList = null;
				foreach (var item in reqStr.deviceNoList)
				{
					// 检查字典中是否存在该 key
					if (stationDataPlc.TryGetValue(item, out StationStateData stationData))
					{
						int photoStatus = Convert.ToInt32(Conn.WCSRedis.GetValue(stationData.IsInStock));
						if (string.IsNullOrEmpty(stationData.ModeState))
						{
							DeviceStatusData dataplc = new DeviceStatusData();
							dataplc.deviceNo = item;
							dataplc.workStatus = 1;
							dataplc.photoStatus = photoStatus;
							dataplc.manualStatus = 2;
							dataList.Add(dataplc);
						}
						else
						{
							int modeState = Convert.ToInt32(Conn.WCSRedis.GetValue(stationData.ModeState));
							DeviceStatusData dataplc = new DeviceStatusData();
							dataplc.deviceNo = item;
							dataplc.workStatus = 1;///?????????????????????????????????????根据plc调整
							dataplc.photoStatus = photoStatus;
							dataplc.manualStatus = 2;///?????????????????????????????????????根据plc调整
							dataList.Add(dataplc);
						}
					}
					else
					{
						response.code = 425;
						response.msg = "操作失败:站台配置不存在";
						response.data = null;
						respJson = JsonConvert.SerializeObject(response);
						return respJson;
					}
				}
				response.code = 200;
				response.msg = "操作成功";
				response.data = dataList;
				respJson = JsonConvert.SerializeObject(response);
				return respJson;
			}
			catch (Exception ex)
			{
				response.code = 500;
				response.msg = "操作异常,异常原因" + ex.Message;
				respJson = JsonConvert.SerializeObject(response);
				return respJson;
			}
		}

		//站台对应的plc状态
		private static Dictionary<string, StationStateData> stationDataPlc = new Dictionary<string, StationStateData>
		{
			//站台编码-----站台有载无载状态---------站台状态
			//钢包
			{"GB_OUT1", new StationStateData { IsInStock = "GB_OUT1_ISINSTOCK", ModeState = "GB_OUT1_MODESTATE" }},
			
		};

		///站台编码对应的PLC数据
		private static readonly Dictionary<string, List<StationData>> AGVStationPlc = new Dictionary<string, List<StationData>>
		{
			//--站台编码--PLC设备名--站台类型--PLC写入组名
			////钢包
			{ "GB_OUT1", new List<StationData>{new StationData { StationPLCName = "GBPLC", StationType = "1", StationGroupName = "GB_AGVGET1" },new StationData { StationPLCName = "GBPLC", StationType = "3", StationGroupName = "GB_AGVGTGO1" }}},
			
		};
	}

	public class WCSTOWMS
	{
		private static string urlnotifyTaskStatus = "http://" + ConfigurationManager.AppSettings["WMsPort"] + "/api/Wms/notifyTaskStatus";
		private static string urlDeviceErrors = "http://" + ConfigurationManager.AppSettings["WMsPort"] + "/api/Wms/notifyAlarm";
		/// <summary>
		/// 006.任务状态反馈
		/// </summary>
		/// <param name="requestJson"></param>
		/// <returns></returns>
		public ToWmsNotifyTaskStatusResponse FeedInfoToWms(ToWmsNotifyTaskStatusRequest mainMissionRequest, out string errorMsg)
		{
			string msg = $"\r\n[wcs任务状态反馈wms：Start{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
			errorMsg = null;
			ToWmsNotifyTaskStatusResponse response = null;
			try
			{
				string input = Json.ToJson(mainMissionRequest);
				msg += $"\r\n\t下发json:[{input}]{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
				string json = new HttpClient().Call(input, urlnotifyTaskStatus, out errorMsg);
				msg += $"\r\n\t收到回复json:[{json}]{(!string.IsNullOrEmpty(errorMsg) ? $"错误信息:[{errorMsg}]" : "")}{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
				response = Json.ToObject<ToWmsNotifyTaskStatusResponse>(json);
				return response;
			}
			catch (Exception ex)
			{
				msg += ex.Message;
				return null;
			}
			finally
			{
				msg = msg.Replace('\'', '\"');
				Conn.WCSLOG.Info(1, $"wcs任务状态反馈wms ", 1, msg);
			}
		}
		/// <summary>
		/// 009.设备报警上报
		/// </summary>
		/// <param name="requestJson"></param>
		/// <returns></returns>
		public ToWmsNotifyAlarmResponse DeviceErrorsToWms(ToWmsNotifyAlarmRequest mainMissionRequest, out string errorMsg)
		{
			string msg = $"\r\n[wcs设备报警上报wms：Start{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
			errorMsg = null;
			ToWmsNotifyAlarmResponse response = null;
			try
			{
				string input = Json.ToJson(mainMissionRequest);
				msg += $"\r\n\t下发json:[{input}]{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
				string json = new HttpClient().Call(input, urlDeviceErrors, out errorMsg);
				msg += $"\r\n\t收到回复json:[{json}]{(!string.IsNullOrEmpty(errorMsg) ? $"错误信息:[{errorMsg}]" : "")}{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
				response = Json.ToObject<ToWmsNotifyAlarmResponse>(json);
				return response;
			}
			catch (Exception ex)
			{
				msg += ex.Message;
				return null;
			}
			finally
			{
				msg = msg.Replace('\'', '\"');
				Conn.WCSLOG.Info(1, $"wcs设备报警上报wms ", 1, msg);
			}
		}
		///// <summary>
		///// 货位状态上报
		///// </summary>
		///// <param name="requestJson"></param>
		///// <returns></returns>
		//public bool LocStateFeedBackToWms(LocStateFeedBackRequest loctStateRequest, out string errorMsg)
		//{
		//	string msg = $"\r\n[wcs货位状态上报wms：Start{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
		//	errorMsg = null;
		//	LocStateFeedBackResponse response = null;
		//	LocStateFeedBackRequest Data = null;
		//	try
		//	{
		//		string input = Json.ToJson(loctStateRequest);
		//		msg += $"\r\n\t下发json:[{input}]{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
		//		string json = new HttpClient().Call(input, url, out errorMsg);
		//		msg += $"\r\n\t收到回复json:[{json}]{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}";
		//		response = Json.ToObject<LocStateFeedBackResponse>(json);
		//		if (response != null && response.code == "200")
		//			return true;
		//		else
		//		{
		//			errorMsg = response.msg;
		//			return false;
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		msg += ex.Message;
		//		return false;
		//	}
		//	finally
		//	{
		//		msg = msg.Replace('\'', '\"');
		//		Conn.WCSLOG.Info(1, $"wcs货位状态上报wms ", 1, msg);
		//	}
		//}
	}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

	/// <summary>
	/// 设备任务状态(堆垛机、衔架)
	/// </summary>
	public static class DeviceStates
	{	
		public const string Create = "Create";
		public const string Pre = "Pre";
		public const string Send = "Send";
		public const string Doing = "Doing";
		public const string Finish = "Finish";
		public const string FinishOK = "FinishOK";
		public const string Error = "Error";
		public const string Cancel = "Cancel";
	}
	/// <summary>
	///任务执行流程节点
	/// 1=任务开始；2=任务完成；3=任务异常；4=任务取消；5=任务更改（待定）
	///6=堆垛机开始执行（预留）
	///7=堆垛机执行结束（预留）
	///8=RGV开始执行（预留）
	///9=RGV执行结束（预留）
	/// </summary>
	public static class TaskFlow
	{
        //任务开始
        public const string Task_CA = "1";
        //任务完成
        public const string Task_CF = "2";
        //任务异常
        public const string Task_Error = "3";
        //任务取消
        public const string Task_Cancel = "4";
        //任务更改（待定）
        public const string Task_Change = "5";
        //流程节点上报（预留）
        public const string Task_ProNode = "6";
        //（预留）
        public const string Stacker_F = "7";
        //机械臂区域上报重量
        public const string Rgv_S = "8";
        //（预留）
        public const string Device_Error = "9";
    }
	/// <summary>
	/// 任务类型
	/// </summary>
	public static class OrderTypes
	{
        //物料入库
        public const string MT_In = "1";
        //物料出库
        public const string MT_Out = "2";
        //托盘组入库
        public const string KTP_In = "3";
        //托盘组出库
        public const string KTP_Out = "4";
        //不进库位，左右移动
        public const string InOt_Move = "5";
        //库内移库
        public const string ST_Move = "6";
        //不同巷道移库
        public const string DT_Move = "7";
    }
	/// <summary>
	/// 任务状态
	/// </summary>
	public static class OrderStates
	{
		public const string Create = "Create";
		public const string Pause = "Pause";
		public const string Doing = "Doing";
		public const string Finish = "Finish";
		public const string Error = "Error";
		public const string Cancel = "Cancel";
	}
	 /// <summary>
    /// 申请任务接口发送至wms任务类型
    /// </summary>
    public static class InterfaceToWmsTaskTypes
    {
        //1=货物入库/退库
        public const int MT_In = 1;
        //2=托盘组入库
        public const int KTP_In = 2;
        //3=RFID托盘查询
        public const int Rfid_Query = 3;
        //4=托盘组出库
        public const int KTP_Out = 4;
    }


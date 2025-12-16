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
public class ObjectConvert:IMultiValueConverter
{
#region    [自定义类][20250516191646769][ObjectConvert]
	
#region IMultiValueConverter Members

public static object ConverterObject;

public object Convert(object[] values, Type targetType,
                      object parameter, System.Globalization.CultureInfo culture)
{
	ConverterObject = values;
	string str = values.GetType().ToString();
	return values.ToArray();
}

public object[] ConvertBack(object value, Type[] targetTypes,
                            object parameter, System.Globalization.CultureInfo culture)
{
	throw new NotImplementedException();
}

#endregion
#endregion [自定义类][20250516191646769][ObjectConvert]
}
public class DictItem
{
#region    [自定义类][20250516191710466][DictItem]
	public string DictName { get; set; } 
public string DictValue { get; set; } 
#endregion [自定义类][20250516191710466][DictItem]
}
public class PlcLogEntity
{
#region    [自定义类][20250517205715896][PlcLogEntity]
	

[BsonId]
[BsonRepresentation(BsonType.ObjectId)]
public string Id { get; set; }
public string tag_id { get; set; }
public int position { get; set; }
public string old_value { get; set; }
public string new_value { get; set; }
public DateTime timestamp { get; set; }
public double timespan { get; set; }
public int next { get; set; }
public int flag { get; set; }

#endregion [自定义类][20250517205715896][PlcLogEntity]
}
public class AutoScanApp
{
#region    [自定义类][20250904102536947][AutoScanApp]
	
//public bool ADDMainMission(List<PCM_MainMission> mainList, out string err)
//{
//	err = "";
//	try
//	{
//		using (var context = new OracleContextOIMobox())
//		{
//			foreach (var entity in mainList)
//			{
//				context.PcmMainmission.Add(entity);
//			}
//			context.SaveChanges();
//		};
//	}
//	catch (Exception ex)
//	{
//		err = ex.ToString();
//		return false;
//	}

//	return true;

//}
//public bool ADDMainMission(PCM_MainMission mainList, out string err)
//{
//	err = "";
//	try
//	{
//		using (var context = new OracleContextOIMobox())
//		{
//			context.PcmMainmission.Add(mainList);
//			context.SaveChanges();
//		}
//		;
//	}
//	catch (Exception ex)
//	{
//		err = ex.ToString();
//		return false;
//	}

//	return true;

//}
//public bool UpMainMission(List<PCM_MainMission> mainList, out string err)
//{
//	err = "";
//	try
//	{
//		using (var context = new OracleContextOIMobox())
//		{
//			foreach (var entity in mainList)
//			{
//				context.PcmMainmission.Add(entity);
//				PCM_MainMission pCM_SYSTEMSYNCHRONIZATION = context.PcmMainmission.FirstOrDefault(t => t.ID == entity.ID);
//				pCM_SYSTEMSYNCHRONIZATION = entity;
//				context.SaveChanges();
//			}
//		};
//	}
//	catch (Exception ex)
//	{
//		err = ex.ToString();
//		return false;
//	}

//	return true;

//}
//public bool UpMainMission(PCM_MainMission entity, out string err)
//{
//	err = "";
//	try
//	{
//		using (var context = new OracleContextOIMobox())
//		{
//			if (entity != null)
//			{
//				context.PcmMainmission.Attach(entity);
//				context.Entry(entity).State = EntityState.Modified;
//				context.SaveChanges();
//			}
//		}
//		;
//	}
//	catch (Exception ex)
//	{
//		err = ex.ToString();
//		return false;
//	}

//	return true;

//}
//public bool DictUpdate(PCM_Dict dictEntity, out string err)
//{
//	err = "";
//	try
//	{
//		using (var context = new OracleContextOIMobox())
//		{
//			if (dictEntity != null)
//			{
//				context.PcmDict.Attach(dictEntity);
//				context.Entry(dictEntity).State = EntityState.Modified;
//			}
//			context.SaveChanges();
//		};
//	}
//	catch (Exception ex)
//	{
//		err = ex.ToString();
//		return false;
//	}
//	return true;
//}
//public bool SystemSynchronizationUpdate(PCM_SystemSynchronization entity, out string err)
//{
//	err = "";
//	try
//	{
//		using (var context = new OracleContextOIMobox())
//		{
//			if (entity != null)
//			{
//				context.PcmSystemsynchronization.Attach(entity);
//				context.Entry(entity).State = EntityState.Modified;
//				context.SaveChanges();
//			}
//		}
//	}
//	catch (Exception ex)
//	{
//		err = ex.ToString();
//		return false;
//	}
//	return true;
//}


public bool ADDMainMission(MainMissionEntity mainEntity, out string err)
{
	err = "";
	try
	{
		using (var db = new RepositoryBase().BeginTrans())
		{
			if (mainEntity != null)
			{
				db.Insert(mainEntity);
			}
			db.Commit();
		}
	}
	catch (Exception ex)
	{
		err = ex.ToString();
		return false;
	}
	return true;
}
public bool ADDMainMission(List<MainMissionEntity> mainList, out string err)
{
	err = "";
	try
	{
		using (var db = new RepositoryBase().BeginTrans())
		{
			foreach (var entity in mainList)
			{
				db.Insert(entity);
			}
			db.Commit();
		}
	}
	catch (Exception ex)
	{
		err = ex.ToString();
		return false;
	}
	return true;
}
public bool UpMainMission(List<MainMissionEntity> mainList, out string err)
{
	err = "";
	try
	{
		using (var db = new RepositoryBase().BeginTrans())
		{
			foreach (var entity in mainList)
			{
				db.Update(entity);
			}
			db.Commit();
		}
	}
	catch (Exception ex)
	{
		err = ex.ToString();
		return false;
	}
	return true;
}
public bool UpMainMission(MainMissionEntity mainEy, out string err)
{
	err = "";
	try
	{
		using (var db = new RepositoryBase().BeginTrans())
		{
			db.Update(mainEy);
			db.Commit();
		}
	}
	catch (Exception ex)
	{
		err = ex.ToString();
		return false;
	}
	return true;
}
public bool DictUpdate(DictEntity dictEy, out string err)
{
	err = "";
	try
	{
		using (var db = new RepositoryBase().BeginTrans())
		{
			db.Update(dictEy);
			db.Commit();
		}
	}
	catch (Exception ex)
	{
		err = ex.ToString();
		return false;
	}
	return true;
}
public bool SystemSynchronizationUpdate(SystemSynchronizationEntity sysEy, out string err)
{
	err = "";
	try
	{
		using (var db = new RepositoryBase().BeginTrans())
		{
			db.Update(sysEy);
			db.Commit();
		}
	}
	catch (Exception ex)
	{
		err = ex.ToString();
		return false;
	}
	return true;
}
#endregion [自定义类][20250904102536947][AutoScanApp]
}
}
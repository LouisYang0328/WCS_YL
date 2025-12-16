/*******************************************************************************
 * Copyright © 2016 NFine.Framework 版权所有
 * Author: NFine
 * Description: NFine快速开发平台
 * Website：http://www.nfine.cn
 *********************************************************************************/
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Data;
using System.IO;
using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NFine.Code;
using Newtonsoft.Json;



	public class NPOIExcel
	{
		private string _title;
		private string _sheetName;
		private string _filePath;
		/// <summary>
		/// json to table
		/// </summary>
		/// <param name="path">要保存的路径</param>
		/// <param name="json">要转成Table的json字符串</param>
		/// <param name="sheetname">xls的sheet标签命名</param>
		/// <returns></returns>
		
		/// <summary>
		/// json to table
		/// </summary>
		/// <param name="path">要保存的路径</param>
		/// <param name="json">要转成Table的json字符串</param>
		/// <param name="sheetname">xls的sheet标签命名</param>
		/// <param name="dic">对比字典集合</param>
		/// <returns></returns>
		public bool exportColume(string path, string json, string sheetname, Dictionary<string, string> dic)
		{
			bool result = true;
			try
			{
				//调整列顺序
				DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);//Json.ToTable(json);
				if (dt.Rows.Count > 0)
				{
					for (int i = 0; i < dic.Count; i++)
					{
						var element = dic.ElementAt(i);
						dt.Columns[element.Key].SetOrdinal(i);
					}
				}

				for (int i = 0; i < dt.Columns.Count; i++)
				{
					if (dic.ContainsKey(dt.Columns[i].ColumnName))
						dt.Columns[i].ColumnName = dic[dt.Columns[i].ColumnName];
					else
					{
						dt.Columns.Remove(dt.Columns[i].ColumnName);
						i--;
					}
				}

				result = ExportToExcel(dt, sheetname, sheetname, path);
			}
			catch (Exception ex)
			{
				result = false;
			}
			return result;
		}

		/// <summary>
		/// 导出到Excel
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public bool ExportToExcel(DataTable table, string title, string sheetName, string filePath)
		{
			try
			{
				this._title = title;
				this._sheetName = sheetName;
				this._filePath = filePath;
				IWorkbook workBook = new HSSFWorkbook();
				if (Path.GetExtension(filePath)==".xlsx")
				{
					workBook=new XSSFWorkbook();
				}
				
				this._sheetName = this._sheetName.IsEmpty() ? "sheet1" : this._sheetName;
				ISheet sheet = workBook.CreateSheet(this._sheetName);
				ICellStyle cellstyle = workBook.CreateCellStyle();
				cellstyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

				//处理表格列头
				IRow row = sheet.CreateRow(0);

				for (int i = 0; i < table.Columns.Count; i++)
				{
					ICell cell = row.CreateCell(i);
					cell.SetCellValue(table.Columns[i].ColumnName);
					row.Height = 350;
					sheet.AutoSizeColumn(i);
					cell.CellStyle = cellstyle;
				}

				//处理数据内容
				for (int i = 0; i < table.Rows.Count; i++)
				{
					row = sheet.CreateRow(1 + i);
					row.Height = 250;
					for (int j = 0; j < table.Columns.Count; j++)
					{
						ICell cell = row.CreateCell(j);
						cell.SetCellValue(table.Rows[i][j].ToString());
						sheet.SetColumnWidth(j, 256 * 10);
						//sheet.AutoSizeColumn(j);
						cell.CellStyle = cellstyle;
					}
				}

				
				//获取当前列的宽度，然后对比本列的长度，取最大值
				for (int columnNum = 0; columnNum < table.Columns.Count; columnNum++)
				{
					int columnWidth = sheet.GetColumnWidth(columnNum) / 256;
					for (int rowNum = 1; rowNum <= table.Rows.Count; rowNum++)
					{
						IRow currentRow;
						//当前行未被使用过
						if (sheet.GetRow(rowNum) == null)
						{
							currentRow = sheet.CreateRow(rowNum);
						}
						else
						{
							currentRow = sheet.GetRow(rowNum);
						}

						if (currentRow.GetCell(columnNum) != null)
						{
							ICell currentCell = currentRow.GetCell(columnNum);
							int length = Encoding.Default.GetBytes(currentCell.ToString()).Length;
							if (columnWidth < length)
							{
								columnWidth = length;
							}
						}
					}
					sheet.SetColumnWidth(columnNum, (columnWidth+2) * 256);
				}


				FileStream fs = new FileStream(this._filePath, FileMode.Create);
				//写入数据流
				workBook.Write(fs);

				if (fs.CanRead)
				{
					fs.Flush();
					fs.Close();
				}
				
			}
			catch (Exception ex)
			{

				return false;
			}
			return true;
		}



		
	}


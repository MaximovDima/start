﻿using System;
using System.Collections.Generic;
using MW.Utils;

namespace MW.Core
{	
	//Общий шаблон модели	
	public class TModel
	{		
		//Наименование
		public string Name;
		//Список полей в БД
		public string[] Fields;
		//Строки
		public List<Dictionary<string, string>> Rows;
		
		public TModel(string AName, string[] AFields)
		{
			Name = AName;
			Fields = AFields;			
			Rows = new List<Dictionary<string, string>>();
		}		
		
		//возвращает следующий ID (в виде строки)
		public string GetNextID()
		{
			return Format.IntToStr(Rows.Count + 1);
		}
		
		//Проверка на дубликат строки
		public bool ExistDataRow(string AType, string AName)
		{
			bool vResult = false;
			
			foreach(Dictionary<string, string> vRow in Rows)
			{
				if ((vRow["Type"] == AType) && (String.Compare(vRow["Name"], AName, true) == 0))
				{
					vResult = true;
					break;
				}
			}
			return vResult;			
		}
		
		//Определение ID по списку категорий (в виде строки)
		public string GetIDByTypeAndName(string AType, string AName)
		{
			string vResult = "";
			
			foreach(Dictionary<string, string> vRow in Rows)
			{
				if ((vRow["Type"] == AType) && (String.Compare(vRow["Name"], AName, true) == 0))
				{
					vResult = vRow["ID"];
					break;
				}
			}
			return vResult;	
		}
		
		//Определение наименования по списку категорий
		public string GetNameByID(string AType, string AID)
		{
			string vResult = "";
			
			foreach(Dictionary<string, string> vRow in Rows)
			{
				if ((vRow["Type"] == AType) && (vRow["ID"] == AID))
				{
					vResult = vRow["Name"];
					break;
				}
			}
			return vResult;	
		}
		
		//Строку которую надо удалить
		public int GetDeleteRowID()
		{
			int vID = -1;
			foreach(Dictionary<string, string> vRow in Rows)
			{
				if (vRow["State"] == "delete")
				{
					vID = Format.StrToInt(vRow["ID"]);
					break;
				}
			}
			return vID;
		}
		
		//Возвращает сумму по числовому полю
		public string GetTextSum(string AFieldName)
		{
			int vResult = 0;
			foreach(Dictionary<string, string> vRow in Rows)
			{
				vResult = vResult + Format.StrToInt(vRow[AFieldName]);
			}
			
			return Format.IntToStr(vResult);
		}
		
		//Возвращает среднее в сутки по числовому полю (по максимальный сущ. дату)
		public string GetTextAverageDay(string AFieldName)
		{
			double vResult = 0;
			DateTime vFirstDate = new DateTime(2020, 1, 1);
			string vDate = Convert.ToString(DateTime.Now);
			TimeSpan vTime = Convert.ToDateTime(vDate) - vFirstDate;

			foreach(Dictionary<string, string> vRow in Rows)
			{
				vResult = vResult + Format.StrToInt(vRow[AFieldName]);
			}
			
			return Format.ObjToStr(vResult / (vTime.Days));
		}
	}
}

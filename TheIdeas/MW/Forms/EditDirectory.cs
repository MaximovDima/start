﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using MW.Core;

namespace MW.Forms
{
	public partial class frmEditDirectory : Form
	{
		//Подгрузка справочника
		public TModel Directory;
		//Тип справочной информации
		public string Type;
		//Флаг расхода
		public bool IsCost;
		
		public frmEditDirectory(string ATypeName, TModel ADirectory, bool AIsCost)
		{
			InitializeComponent();
			Directory = ADirectory;
			IsCost = AIsCost;
			SyncForm(ATypeName);
		}
		
		//Инициализация справочника
		public void SyncForm(string ATypeName)
		{
			SyncName(ATypeName);					
		}
		
		//Синхронизация наименования
		public void SyncName(string ATypeName)
		{
			switch (ATypeName)
			{
				case "addType":
					Text = "Добавить тип";
					if(IsCost)
					{
						Type = "Cost";
					}
					else
					{
						Type= "Income";
					}
					break;
				case "addPlace":
					Text = "Добавить место расхода";
					Type = "Place";
					break; 
				case "SelectTags":
					Text = "Добавить тэги";
					eComment.Enabled = false ;
					break;
					
				default:
					Text = "Новый тип";
					break;
			}
		}	
		
		void BtnCancelClick(object sender, EventArgs e)
		{
			Close();			
		}
		
		void BtnOkClick(object sender, EventArgs e)
		{
			//Проверки
			if (Checks.IsNull("Наименование", eName) || IsExist())
			{
				return;
			}
			//Добавление в модель
			Dictionary<string, string> vRow = new Dictionary<string, string>();
			vRow.Add("ID", Directory.GetNextID());
			vRow.Add("Type", Type);
			vRow.Add("Name", eName.Text);
			vRow.Add("Comment", eComment.Text);
			vRow.Add("State", "add");
			Directory.Rows.Add(vRow);
			Close();
		}
		
		//Проверка на дубликат
		public bool IsExist()
		{
			Dictionary<string, string> vRow = new Dictionary<string, string>();
			vRow.Add("Type", Type);
			vRow.Add("Name", eName.Text);
			if (Directory.ExistDataRow(Type, eName.Text))
			{
				MessageBox.Show("Раздел '" + eName.Text + "' уже существует!", "Дубликация", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return true;
			}
			else return false;
		}
	}
}

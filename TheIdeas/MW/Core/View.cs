﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using MW.Drawing;

namespace MW.Core
{
	//Виртуальная сцена
	public class TScene
	{
		//Размеры сцены в пикселях
		public double X;
		public double Y;
		//Коллеккция моделей объектов сцены
		public List<TSceneObject> SceneObjectList;
		//Масштаб
		public double Scale;		
		
		public TScene(double AX, double AY)
		{
			X = AX;
			Y = AY;
			SceneObjectList = new List<TSceneObject>();
			Scale = 1;
		}	
		
		public void LoadModels(TModel ACosts, TModel AIncomes, bool AViewCosts,
		                      bool AViewIncomes, bool AViewBalance)
		{
			List<TFuncPoint> vPoints = new List<TFuncPoint>();
			AIncomes.ReFill(vPoints);
			//Построение системы координат
			CreateCoord(vPoints);
		}
		
		public void CreateCoord(List<TFuncPoint> APoints)
		{
			TScObjCoord Coord = new TScObjCoord(this);
			Coord.SetAxis(APoints);
			SceneObjectList.Add(Coord);
		}
		
	}
	
	//Шаблон модели объекта отрисовки
	public abstract class TSceneObject
	{
		//Идентификатор
    	public int ID;
    	//Наименование
    	public string Name;
    	//Тэги
    	public List<string> CodeList;
    	//Список примитивов
    	public List<TDrwShape> DrwObjList;
    	//ссылка на параметры сцены
    	public TScene Scene; 
    	
    	public TSceneObject()
    	{
    		CodeList = new List<string>();
    		DrwObjList = new List<TDrwShape>();
    	}
    	//Построение объекта из примитивов
    	public abstract void Build(double ACoeffX, double ACoeffY);
	}
	
	//Система координат
	public class TScObjCoord : TSceneObject
	{
		//Точка начала координат
		public double X0;
		public double X1;
		public double Y0;
		public double Y1;
		//Настройки для оси X
		public double XMin;
		public double XMax;
		public double CoeffX;
		public double GetXDrwByUsr(double AXUsr)
		{
			return (((X1-X0)*AXUsr)/(XMax - XMin))*CoeffX;
		}
		public double GetXUsrByDrw(double AXDrw)
		{
			return (((XMax-XMin)*AXDrw)/(X1 - X0))/CoeffX;
		}
		//Настройки для оси Y
		public double YMin;
		public double YMax;
		public double CoeffY;
		public double GetYUsrByDrw(double AYDrw)
		{
			return (((YMax-YMin)*AYDrw)/(Y1 - Y0))/CoeffY;
		}
		public double GetYDrwByUsr(double AYUsr)
		{
			return (((Y1-Y0)*AYUsr)/(YMax - YMin))*CoeffY;
		}
		
		public TScObjCoord(TScene AScene)
		{
			Scene = AScene;
			Name = "Coord";
			X0 = AScene.X*0.025;
			Y0 = AScene.Y*0.05;
			X1 = AScene.X - 5;
			Y1 = AScene.Y - 5;
		}
		
		public override void Build(double ACoeffX, double ACoeffY)
		{			
			DrwObjList.Clear();
			CoeffX = ACoeffX;
			CoeffY = ACoeffY;
			TDrwLine vXLine, vYLine;
			TDrwLabel vLabel;
			double vStep, vX, vY;
			//ось OX
			vXLine = DrwObjects.GetLine(X0, Y0, X1*ACoeffX, Y0, Color.Black, 2);
			DrwObjList.Add(vXLine);
			//ось OY
			vYLine = DrwObjects.GetLine(X0, Y0, X0, Y1*ACoeffY, Color.Black, 2);
			DrwObjList.Add(vYLine);
			//Сетка OX
			vStep = (X1 - X0) / XMax;
			for (int i = 0; i < Math.Round(XMax); i++)
			{
				if ((i % 7) == 0 & i!=0)
				{
					vX = X0 + i*vStep*ACoeffX;
					vXLine = DrwObjects.GetLine(vX, Y0, vX, Y0+4, Color.Black, 2);
					DrwObjList.Add(vXLine);
					vXLine = DrwObjects.GetLine(vX, Y0, vX, Y1*ACoeffY, Color.Black, 1, 20);				
					DrwObjList.Add(vXLine);

					vLabel = new TDrwLabel();
					vLabel.HAlig = TDrwLabel.THAlig.HCenter;
					vLabel.VAlig = TDrwLabel.TVAlig.VBottom;
					vLabel.Point.X = vX;
					vLabel.Point.Y = Y0-1;					
					DateTime d1 = new DateTime(2020, 1, 1);							
					vLabel.Text = d1.AddDays(i).ToString("d MMM");
					DrwObjList.Add(vLabel);
				}
			}
			//Сетка OY
			vStep = (Y1-Y0)/YMax;
			for (int i = 0; i < Math.Round(YMax); i++)
			{
				vY = Y0 + i*vStep*ACoeffY;
				vYLine = DrwObjects.GetLine(X0, vY, X0+3, vY, Color.Black, 2);
				DrwObjList.Add(vYLine);
				vYLine = DrwObjects.GetLine(X0, vY, X1*ACoeffX, vY, Color.Black, 1, 20);				
				DrwObjList.Add(vYLine);
				if ((i % 2) == 0 & i!=0)
				{
					vLabel = new TDrwLabel();
					vLabel.HAlig = TDrwLabel.THAlig.HLeft;
					vLabel.VAlig = TDrwLabel.TVAlig.VCenter;
					vLabel.Point.X = X0-1;
					vLabel.Point.Y = vY;
					vLabel.Text = Format.IntToStr(i)+"K";
					DrwObjList.Add(vLabel);
				}
			}
		}
		
		//Настройка осей
		public void SetAxis(List<TFuncPoint> APoints)
		{
			//Временная шкала
			XMin = 0;
			DateTime d1 = new DateTime(2020, 1, 1);
    		DateTime d2 = DateTime.Now;
    		TimeSpan time = d2 - d1;
			XMax = time.Days + 7;
			//Шкала значений
			//Анализ модельных значений
			YMax = 0;
			YMin = APoints[0].Value;
			foreach (TFuncPoint vPoint in APoints)
			{
				YMin = Math.Min(YMin, vPoint.Value);
				YMax = Math.Max(YMax, vPoint.Value);
			}
			YMax = YMax/1000;
			YMin = YMin/1000;
		}
	}
	
	//Класс-обертка для загрузки значений в функцию
	public class TFuncPoint
	{
		public int ID;
		public string Date;
		public double Value;
		
		public TFuncPoint(int AID, string ADate, double AValue)
		{
			ID = AID;
			Date = ADate;
			Value = AValue;
		}
	}
	
	public class TScObjFunc : TSceneObject
	{
		public TScObjFunc(TModel AModel)
		{
			Name = "Func" + AModel.Name;
		}
		
		public override void Build(double ACoeffX, double ACoeffY)
		{
			
		}
	}
}
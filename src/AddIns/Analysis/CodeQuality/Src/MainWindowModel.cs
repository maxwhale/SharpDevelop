﻿/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.09.2011
 * Time: 13:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using ICSharpCode.CodeQualityAnalysis.Utility.Localizeable;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.CodeQualityAnalysis
{
	/// <summary>
	/// Description of MainWindowViewModel.
	/// </summary>
	public enum MetricsLevel
	{
		Assembly,
		Namespace,
		Type,
		Method
	}
	
	
	public enum Metrics
	{
		[LocalizableDescription("IL Instructions")]
		ILInstructions,
		
		[LocalizableDescription("Cyclomatic Complexity")]
		CyclomaticComplexity,
		
		[LocalizableDescription("Variables")]
		Variables
	}
	
	public class MainWindowViewModel :ViewModelBase
	{
		
		public MainWindowViewModel():base()
		{
			this.FrmTitle = "Code Quality Analysis";
			//ResourceService.GetString("ICSharpCode.WepProjectOptionsPanel.Server");
			this.btnOpenAssembly = "Open Assembly";
			
			#region MainTab
			this.TabDependencyGraph = "Dependency Graph";
			this.TabDependencyMatrix = "Dependency Matrix";
			this.TabMetrics = "Metrics";
			#endregion
			
			ActivateMetrics = new RelayCommand(ActivateMetricsExecute);
			ShowTreeMap = new RelayCommand(ShowTreemapExecute,CanActivateTreemap);
		}
		
		
		public string FrmTitle {get;private set;}
		
		public string btnOpenAssembly {get; private set;}
		
		#region Main TabControl
		
		public string TabDependencyGraph {get; private set;}
		public string TabDependencyMatrix {get; private set;}
		public string TabMetrics {get;private set;}
		
		#endregion
		
		private string fileName;
		
		public string FileName {
			get { return fileName; }
			set { fileName = value;
			base.RaisePropertyChanged(() =>FileName);}
		}
		
		
		private Visibility progressbarVisibly = Visibility.Hidden;
		
		public Visibility ProgressbarVisible {
			get { return progressbarVisibly; }
			set { progressbarVisibly = value;
				base.RaisePropertyChanged(() =>ProgressbarVisible);
				}
		}
		
		private Visibility assemblyStatsVisible= Visibility.Hidden; 
		
		public Visibility AssemblyStatsVisible {
			get { return assemblyStatsVisible; }
			set { assemblyStatsVisible = value;
				base.RaisePropertyChanged(() => AssemblyStatsVisible);
			}
		}

		bool mainTabEnable;

		public bool MainTabEnable {
			get { return mainTabEnable; }
			set { mainTabEnable = value;
				base.RaisePropertyChanged(() => MainTabEnable);
			}
		}
		
		
		public bool MetrixTabEnable
		{
			get {return SelectedNode != null;}
		}
		
		string typeInfo;
		
		public string TypeInfo {
			get { return typeInfo; }
			set { typeInfo = value;
				base.RaisePropertyChanged(() =>this.TypeInfo);
			}
		}
		
		private Module mainModule;
		
		public Module MainModule {
			get { return mainModule; }
			set { mainModule = value;
				base.RaisePropertyChanged(() =>this.MainModule);
				Summary = String.Format("Module Name: {0}  Namespaces: {1}  Types {2} Methods: {3}  Fields: {4}",
				                                    mainModule.Name,
				                                    mainModule.Namespaces.Count,
				                                    mainModule.TypesCount,
				                                    mainModule.MethodsCount,
				                                    mainModule.FieldsCount);
			}
		}
		
		
		private INode selectedNode;
		
		public INode SelectedNode {
			get { return selectedNode; }
			set { selectedNode = value;
			base.RaisePropertyChanged(() =>this.SelectedNode);
			base.RaisePropertyChanged(() =>this.MetrixTabEnable);
			Summary = UpdateToolStrip();
			}
		}
		
		
		string UpdateToolStrip()
		{
			var t = SelectedNode as Type;
			if (t != null)
			{
				return string.Format("Type Namer {0}  Methods {1} Fields {2}",
				                     t.Name,
				                     t.GetAllMethods().Count(),
				                     t.GetAllFields().Count());
			}
			var ns = SelectedNode as Namespace;
			if ( ns != null) {
				return string.Format("Namespace Name {0}  Types : {1}  Methods: {2} Fields : {3}",
				                     ns.Name,
				                     ns.Types.Count,
				                     ns.GetAllMethods().Count(),
				                     ns.GetAllFields().Count());
			}
			return String.Empty;
		}
		
		
		private ObservableCollection<INode> nodes;
		
		public ObservableCollection<INode> Nodes {
			get { return nodes; }
			set { nodes = value;
			base.RaisePropertyChanged(() =>this.Nodes);}
		}
		
		
		private string treeValueProperty ;
		
		public string TreeValueProperty {
			get { return treeValueProperty; }
			set { treeValueProperty = value;
			base.RaisePropertyChanged(() =>this.TreeValueProperty);}
		}
		
		
		#region MetricsLevel Combo
		
		public MetricsLevel MetricsLevel {
			get {return MetricsLevel;}
		}
	
		private MetricsLevel selectedMetricsLevel;
		
		public MetricsLevel SelectedMetricsLevel {
			get { return selectedMetricsLevel; }
			set { selectedMetricsLevel = value;
			base.RaisePropertyChanged(() =>this.selectedMetricsLevel);}
		}
		
		
		#endregion
		
		#region ActivateMetrics
		
		public ICommand ActivateMetrics {get;private set;}
		
		bool metricsIsActive;
		
		void ActivateMetricsExecute ()
		{
			
			Console.WriteLine(SelectedMetricsLevel.ToString());
			switch (SelectedMetricsLevel) {
				case MetricsLevel.Assembly:
					Console.WriteLine("assembly");
					break;
				case MetricsLevel.Namespace:
					Console.WriteLine("namespace");
					break;
				case MetricsLevel.Type:
					Console.WriteLine("type");
					break;
				case MetricsLevel.Method:
					metricsIsActive = true;
					Console.WriteLine("method");
					break;
				default:
					throw new Exception("Invalid value for MetricsLevel");
			}
		}
		
		
		#endregion
		
		#region MetricsLevel Combo
		
		public Metrics Metrics
		{
			get {return Metrics;}
		}
		
		Metrics selectedMetrics;
		
		public Metrics SelectedMetrics {
			get { return selectedMetrics; }
			set { selectedMetrics = value;
				base.RaisePropertyChanged(() =>this.SelectedMetrics);
			}
		}
		
		#endregion
		
		#region ShowTreeMap Treemap
		
		public ICommand ShowTreeMap {get;private set;}
		
		
		bool CanActivateTreemap()
		{
			return metricsIsActive;
		}
		
		
		void ShowTreemapExecute ()
		{
			
			Nodes = PrepareNodes();
		
			var aa = SelectedNode as INode;
		var m = aa as Method;
		var t = aa as Type;
		var n = aa as Namespace;
		
			switch (selectedMetrics)
			{
				case Metrics.ILInstructions:
					TreeValueProperty = "Instructions.Count";
					break;
					
				case Metrics.CyclomaticComplexity:
					TreeValueProperty = Metrics.CyclomaticComplexity.ToString();
					break;
				case Metrics.Variables:
					TreeValueProperty = Metrics.Variables.ToString();
					break;
				default:
					throw new Exception("Invalid value for Metrics");
			}
		}
		
		
		ObservableCollection<INode> PrepareNodes()
		{
			IEnumerable<INode> list  = new List<INode>();
			switch (selectedMetricsLevel) {
				case MetricsLevel.Assembly:
					list = from ns in MainModule.Namespaces
						select ns;
					break;
					
				case MetricsLevel.Namespace:
					list = from ns in MainModule.Namespaces
						select ns;
					break;
				case MetricsLevel.Type:
					list = from ns in MainModule.Namespaces
						from type in ns.Types
						select ns;
					break;
				case MetricsLevel.Method:
					list  = from ns in MainModule.Namespaces
						from type in ns.Types
						from method in type.Methods
						select method;
					break;
				default:
					throw new Exception("Invalid value for MetricsLevel");
			}
			var nodes = new ObservableCollection<INode>(list.Distinct());
			Console.WriteLine("listcount for  {0} = {1}",selectedMetricsLevel.ToString(),nodes.Count);
			return nodes;
		}
		
		#endregion
		
		#region ToolStrip and ToolTip
		
		string summary;
		
		public string Summary {
			get { return summary; }
			set { summary = value;
				base.RaisePropertyChanged(() => Summary);
			}
		}
		
		#endregion
	}
}

﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ICSharpCode.SharpDevelop.Debugging;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// OpenAssemblyFromFileCommand.
	/// </summary>
	class OpenAssemblyFromFileCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			var classBrowser = SD.GetService<IClassBrowser>();
			var modelFactory = SD.GetService<IModelFactory>();
			if ((classBrowser != null) && (modelFactory != null)) {
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Filter = "Assembly files (*.exe, *.dll)|*.exe;*.dll";
				openFileDialog.CheckFileExists = true;
				openFileDialog.CheckPathExists = true;
				if (openFileDialog.ShowDialog() ?? false)
				{
					IAssemblyModel assemblyModel = modelFactory.SafelyCreateAssemblyModelFromFile(openFileDialog.FileName);
					if (assemblyModel != null)
						classBrowser.MainAssemblyList.Assemblies.Add(assemblyModel);
				}
			}
		}
	}
	
	/// <summary>
	/// OpenAssemblyFromGACCommand.
	/// </summary>
	class OpenAssemblyFromGACCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			var classBrowser = SD.GetService<IClassBrowser>();
			var modelFactory = SD.GetService<IModelFactory>();
			if ((classBrowser != null) && (modelFactory != null)) {
				OpenFromGacDialog gacDialog = new OpenFromGacDialog();
				if (gacDialog.ShowDialog() ?? false)
				{
					foreach (string assemblyFile in gacDialog.SelectedFileNames) {
						IAssemblyModel assemblyModel = modelFactory.SafelyCreateAssemblyModelFromFile(assemblyFile);
						if (assemblyModel != null)
							classBrowser.MainAssemblyList.Assemblies.Add(assemblyModel);
					}
				}
			}
		}
	}
	
	/// <summary>
	/// RemoveAssemblyCommand.
	/// </summary>
	class RemoveAssemblyCommand : SimpleCommand
	{
		public override bool CanExecute(object parameter)
		{
			return parameter is AssemblyModel;
		}
		
		public override void Execute(object parameter)
		{
			var classBrowser = SD.GetService<IClassBrowser>();
			if (classBrowser != null) {
				IAssemblyModel assemblyModel = (IAssemblyModel) parameter;
				classBrowser.MainAssemblyList.Assemblies.Remove(assemblyModel);
			}
		}
	}
	
	/// <summary>
	/// RunAssemblyWithDebuggerCommand.
	/// </summary>
	class RunAssemblyWithDebuggerCommand : SimpleCommand
	{
		public override bool CanExecute(object parameter)
		{
			IAssemblyModel assemblyModel = parameter as IAssemblyModel;
			return (assemblyModel != null) && assemblyModel.Context.IsValid;
		}
		
		public override void Execute(object parameter)
		{
			IAssemblyModel assemblyModel = (IAssemblyModel) parameter;
			
			// Start debugger with given assembly
			DebuggerService.CurrentDebugger.Start(new ProcessStartInfo {
			                                      	FileName = assemblyModel.Context.Location,
			                                      	WorkingDirectory = Path.GetDirectoryName(assemblyModel.Context.Location)
			                                      });
		}
	}
}
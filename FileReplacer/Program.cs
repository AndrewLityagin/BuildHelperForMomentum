using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BuildHelperForMomentumExtension;

namespace FileReplacer
{
	internal class Program
	{
		static void Main(string[] args)
		{
			MoveCompiledFilesProc();
		}
		private static void KillProcess()
		{
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine("  0. Killing process");
			var process = new string[]{"MEScontrol.DMS.Server","MEScontrol.MES.Server","MEScontrol.LMS.LabelServer",
								"MEScontrol.DataCenter.Server","MEScontrol.WMS.Server","MEScontrol.Shell",
								"Momentum.WebServer","Momentum.Manager","Momentum.PrintServer",
								"Momentum.Server","MEScontrol.Configuration"};
			foreach(var prc in process)
			{
				try
				{
					foreach(Process proc in Process.GetProcessesByName(prc))
					{
						proc.Kill();
					}
				}
				catch(Exception ex)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"  Error in killing process {ex.Message}");
				}
			}
		}

		private static string[] GetUpdatedFiles(string folder, byte timePeriod)
		{
			var binaryFiles = new List<string>();

			var exeFiles = Directory.GetFiles(folder, "*.exe", SearchOption.AllDirectories);
			var dllFiles = Directory.GetFiles(folder, "*.dll", SearchOption.AllDirectories);
			var pdbFiles = Directory.GetFiles(folder, "*.pdb", SearchOption.AllDirectories);

			if(dllFiles.Length != 0 && pdbFiles.Length != 0)
			{
				binaryFiles.AddRange(dllFiles);
				binaryFiles.AddRange(pdbFiles);
			}
			if(exeFiles.Length != 0)
				binaryFiles.AddRange(exeFiles);

			int i = 0;
			List<string> updatedFiles = new List<string>();
			var now = DateTime.Now;
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine("  Looking for updated files in repository folder.");

			foreach(var file in binaryFiles)
			{
				var fileUpdate = File.GetLastWriteTime(file);
				if(Math.Abs(now.Subtract(fileUpdate).TotalMinutes) <= timePeriod)
				{
					updatedFiles.Add(file);
					i++;
				}
			}
			Console.WriteLine($"  Found {i} files\n\n");
			return updatedFiles.ToArray();
		}

		private static void MoveCompiledFilesProc()
		{
			try
			{
				var settings = Settings.GetInstance();
				Console.ForegroundColor = ConsoleColor.DarkBlue;
				Console.WriteLine("Build Helper For Momentum extension");
				Console.WriteLine("  Settings:");
				Console.WriteLine($"  {nameof(settings.KillProcess)} : {settings.KillProcess}");
				Console.WriteLine($"  {nameof(settings.InstanceFolder)} : {settings.InstanceFolder}");
				Console.WriteLine($"  {nameof(settings.ReposFolder)} : {settings.ReposFolder}");
				Console.WriteLine($"  {nameof(settings.TimePeriod)} : {settings.TimePeriod}");
				Console.WriteLine($"  {nameof(settings.WebProjectOperatorFolderFromInstance)} : {settings.WebProjectOperatorFolderFromInstance}");
				Console.WriteLine($"  {nameof(settings.WebProjectOperatorFolderFromRepos)} : {settings.WebProjectOperatorFolderFromRepos}");
				Console.WriteLine($"  {nameof(settings.WebProjectSupervisorFolderFromInstance)} : {settings.WebProjectSupervisorFolderFromInstance}");
				Console.WriteLine($"  {nameof(settings.WebProjectSupervisorFolderFromRepos)} : {settings.WebProjectSupervisorFolderFromRepos}");

				if(settings.KillProcess)
					KillProcess();

				var updatedFiles = GetUpdatedFiles($"{settings.ReposFolder}\\Binaries", settings.TimePeriod).Where(file => !file.Contains("\\Binaries\\Web\\"));

				Console.ForegroundColor = ConsoleColor.DarkGreen;
				Console.WriteLine("  Replacing updated files to instance folder...");
				int i = 0;
				foreach(var uFile in updatedFiles)
				{
					var fileName = uFile.Remove(0, uFile.LastIndexOf("\\") + 1);
					var previousFolder = string.Empty;
					if(fileName.EndsWith(".resources.dll") || uFile.Contains("NiceLabel"))
					{
						var temp = uFile.Remove(uFile.LastIndexOf("\\"), uFile.Length - uFile.LastIndexOf("\\"));
						previousFolder = uFile.Remove(0, temp.LastIndexOf("\\"));
						previousFolder = previousFolder.Remove(previousFolder.LastIndexOf(fileName), fileName.Length);
					}

					var filesToRepalce = Directory.GetFiles(settings.InstanceFolder, $"{fileName}", SearchOption.AllDirectories).Where(file => !file.Contains("WebServer"));

					foreach(var rFile in filesToRepalce)
					{
						var sizeDifference = (new FileInfo(uFile)).Length / (new FileInfo(rFile)).Length;
						if(previousFolder == string.Empty && (sizeDifference <= 1.3) || (previousFolder != string.Empty && rFile.Contains(previousFolder)))
						{
							Console.WriteLine($"  #{i} {uFile} =>\n  {rFile}");
							File.Delete(rFile);
							File.Copy(uFile, rFile);
							i++;
						}
					}
				}
				Console.WriteLine($"  Replaced {i} files.\n\n");

				if(settings.ReplaceWebProjects && Directory.Exists($"{settings.ReposFolder}\\Binaries\\Web"))
				{
					Console.WriteLine("  Replacing updated files to instance folder (web applications)...");
					updatedFiles = GetUpdatedFiles($"{settings.ReposFolder}\\Binaries\\Web", settings.TimePeriod);
					i = 0;
					foreach(var uFile in updatedFiles)
					{
						var fileName = uFile.Remove(0, uFile.LastIndexOf("\\") + 1);
						var previousFolder = string.Empty;
						if(fileName.EndsWith(".resources.dll") || uFile.Contains("NiceLabel"))
						{
							var temp = uFile.Remove(uFile.LastIndexOf("\\"), uFile.Length - uFile.LastIndexOf("\\"));
							previousFolder = uFile.Remove(0, temp.LastIndexOf("\\"));
							previousFolder = previousFolder.Remove(previousFolder.LastIndexOf(fileName), fileName.Length);
						}

						var filesToRepalce = Directory.GetFiles(settings.InstanceFolder, $"{fileName}", SearchOption.AllDirectories).Where(file => file.Contains("\\WebServer\\"));

						foreach(var rFile in filesToRepalce)
						{
							var sizeDifference = (new FileInfo(uFile)).Length / (new FileInfo(rFile)).Length;
							if(previousFolder == string.Empty && (sizeDifference <= 1.3) || (previousFolder != string.Empty && rFile.Contains(previousFolder)))
							{
								Console.WriteLine($"  #{i} {uFile} =>\n  {rFile}");
								File.Delete(rFile);
								File.Copy(uFile, rFile);
								i++;
							}
						}
					}
					Console.WriteLine($"  Replaced {i} files.\n\n");
				}

				Console.ForegroundColor = ConsoleColor.DarkYellow;

				if(settings.ReplaceWebProjects && Directory.Exists(settings.WebProjectOperatorFolderFromInstance) && Directory.Exists(settings.WebProjectOperatorFolderFromRepos))
				{
					Console.WriteLine("Replacing Operator Web Project folder.");
					Directory.Delete(settings.WebProjectOperatorFolderFromInstance, true);
					Directory.Move(settings.WebProjectOperatorFolderFromRepos, settings.WebProjectOperatorFolderFromInstance);

				}

				if(settings.ReplaceWebProjects && Directory.Exists(settings.WebProjectSupervisorFolderFromInstance) && Directory.Exists(settings.WebProjectSupervisorFolderFromRepos))
				{
					Console.WriteLine("Replacing Supervisor Web Project folder.");
					Directory.Delete(settings.WebProjectSupervisorFolderFromInstance, true);
					Directory.Move(settings.WebProjectSupervisorFolderFromRepos, settings.WebProjectSupervisorFolderFromInstance);
				}

				if(Directory.Exists(settings.CustomFolderToRemoveFromInstance) && Directory.Exists(settings.CustomFolderToRemoveFromRepos))
				{
					Console.WriteLine("Replacing custom folder...");
					Directory.Delete(settings.CustomFolderToRemoveFromInstance, true);
					Directory.Move(settings.CustomFolderToRemoveFromRepos, settings.CustomFolderToRemoveFromInstance);
				}

				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("All operations finished.");
			}
			catch(Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Error: {ex.Message}");
				Console.WriteLine("Replacement unsuccessful");
			}
			Console.ReadKey();
		}
	}
}

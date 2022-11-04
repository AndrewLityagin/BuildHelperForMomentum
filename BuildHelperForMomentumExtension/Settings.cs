using System;
using System.IO;
using System.Text.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BuildHelperForMomentumExtension
{
	public class Settings : INotifyPropertyChanged
	{
		

		private bool replaceWebProjects;

		public bool ReplaceWebProjects
		{
			get { return replaceWebProjects; }
			set
			{
				if(value != replaceWebProjects)
				{
					replaceWebProjects = value;
					SaveToFile();
					OnPropertyChanged(nameof(ReplaceWebProjects));
				}
			}
		}

		private bool killProcess;
		public bool KillProcess
		{
			get { return killProcess; }
			set
			{
				if(value != killProcess)
				{
					killProcess = value;
					SaveToFile();
					OnPropertyChanged(nameof(KillProcess));
				}
			}
		}

		private bool autoChoosingRepos;
		public bool AutoChoosingRepos
		{
			get { return autoChoosingRepos; }
			set
			{
				if(value != autoChoosingRepos)
				{
					autoChoosingRepos = value;
					SaveToFile();
					OnPropertyChanged(nameof(AutoChoosingRepos));
				}
			}
		}

		private byte timePeriod;
		public byte TimePeriod
		{
			get { return timePeriod; }
			set
			{
				if(value != timePeriod)
				{
					timePeriod = value;
					SaveToFile();
					OnPropertyChanged(nameof(TimePeriod));
				}
			}
		}

		private string reposFolder;
		public string ReposFolder
		{
			get { return reposFolder; }
			set
			{	
				if(value != reposFolder)
				{
					if(autoChoosingRepos)
						reposFolder = value;
					else
						reposFolder = Directory.GetCurrentDirectory();
					webProjectSupervisorFolderFromRepos = $"{reposFolder}\\Binaries\\Web\\Momentum.Web.Supervisor\\net6.0\\publish\\wwwroot";
					webProjectOperatorFolderFromRepos = $"{reposFolder}\\Binaries\\Web\\Momentum.Web.Operator\\net6.0\\publish\\wwwroot";
					SaveToFile();
					OnPropertyChanged(nameof(ReposFolder));
					OnPropertyChanged(nameof(WebProjectSupervisorFolderFromRepos));
					OnPropertyChanged(nameof(WebProjectSupervisorFolderFromRepos));
				}
			}
		}

		private string instanceFolder;
		public string InstanceFolder
		{
			get { return instanceFolder; }
			set
			{
				if(value != instanceFolder)
				{
					instanceFolder = value;
					webProjectSupervisorFolderFromInstance = $"{instanceFolder}\\WebServer\\Default\\Momentum.Supervisor\\wwwroot";
					webProjectOperatorFolderFromInstance = $"{instanceFolder}\\WebServer\\Default\\Momentum.Operator\\wwwroot";
					SaveToFile();
					OnPropertyChanged(nameof(InstanceFolder));
					OnPropertyChanged(nameof(WebProjectOperatorFolderFromInstance));
					OnPropertyChanged(nameof(WebProjectOperatorFolderFromInstance));
				}
			}
		}

		private string webProjectSupervisorFolderFromRepos;
		public string WebProjectSupervisorFolderFromRepos
		{
			get { return webProjectSupervisorFolderFromRepos; }
			set
			{
				if(value != webProjectSupervisorFolderFromRepos)
				{
					webProjectSupervisorFolderFromRepos = value;
					SaveToFile();
					OnPropertyChanged(nameof(WebProjectSupervisorFolderFromRepos));
				}
			}
		}

		private string webProjectSupervisorFolderFromInstance;
		public string WebProjectSupervisorFolderFromInstance
		{
			get { return webProjectSupervisorFolderFromInstance; }
			set
			{
				if(value != webProjectSupervisorFolderFromInstance)
				{
					webProjectSupervisorFolderFromInstance = value;
					SaveToFile();
					OnPropertyChanged(nameof(WebProjectSupervisorFolderFromInstance));
				}
			}
		}

		private string webProjectOperatorFolderFromRepos;
		public string WebProjectOperatorFolderFromRepos
		{
			get { return webProjectOperatorFolderFromRepos; }
			set
			{
				if(value != webProjectOperatorFolderFromRepos)
				{
					webProjectOperatorFolderFromRepos = value;
					SaveToFile();
					OnPropertyChanged(nameof(WebProjectOperatorFolderFromRepos));
				}
			}
		}

		private string webProjectOperatorFolderFromInstance;
		public string WebProjectOperatorFolderFromInstance
		{
			get { return webProjectOperatorFolderFromInstance; }
			set
			{
				if(value != webProjectOperatorFolderFromInstance)
				{
					webProjectOperatorFolderFromInstance = value;
					SaveToFile();
					OnPropertyChanged(nameof(WebProjectOperatorFolderFromInstance));
				}
			}
		}


		private string customFolderToRemoveFromInstance;
		public string CustomFolderToRemoveFromInstance
		{
			get { return customFolderToRemoveFromInstance; }
			set
			{
				if(value != customFolderToRemoveFromInstance)
				{
					customFolderToRemoveFromInstance = value;
					SaveToFile();
					OnPropertyChanged(nameof(CustomFolderToRemoveFromInstance));
				}
			}
		}


		private string customFolderToRemoveFromRepos;
		public string CustomFolderToRemoveFromRepos
		{
			get { return customFolderToRemoveFromRepos; }
			set
			{
				if(value != customFolderToRemoveFromRepos)
				{
					customFolderToRemoveFromRepos = value;
					SaveToFile();
					OnPropertyChanged(nameof(CustomFolderToRemoveFromRepos));
				}
			}
		}

		private string programDirectory;
		public string ProgramDirectory { 
			get { 
				return programDirectory; 
			} 
			set
			{
				if(value != programDirectory)
				{
					programDirectory = value;
					SaveToFile();
					OnPropertyChanged(nameof(ProgramDirectory));
				}
			}
		}

		private static Settings instance = null;
		private static string confDirectory = $"C:\\Users\\{Environment.UserName}\\.buildHelperExtension";
		private static string confFileName = "build_helper_extension.json";
		private static string confFile = $"{confDirectory}\\{confFileName}";
										   
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
		public static Settings GetInstance()
		{
			if(instance == null)
			{
				if(!File.Exists(confFile))
				{
					if(!Directory.Exists(confDirectory))
						Directory.CreateDirectory(confDirectory);
					using(FileStream fs = new FileStream(confFile, FileMode.OpenOrCreate))
					{
						var userDirectory = $"C:\\Users\\{Environment.UserName}";
						instance = new Settings(5,
							 false,
							 true,
							 true,
							 $"{userDirectory}\\source\\repos\\MEScontrol.net Standard",
							 $"C:\\Program Files (x86)\\BrightEye\\Momentum\\Happy Sauce 17.3",
							 $"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)}\\BuildHelperForMomentumExtension",
							 $"{userDirectory}\\source\\repos\\MEScontrol.net Standard\\Binaries\\Web\\Momentum.Web.Supervisor\\net6.0\\publish\\wwwroot",
							 $"{userDirectory}\\source\\repos\\MEScontrol.net Standard\\Binaries\\Web\\Momentum.Web.Operator\\net6.0\\publish\\wwwroot",
							 $"C:\\Program Files (x86)\\BrightEye\\Momentum\\Happy Sauce 17.3\\WebServer\\Default\\Momentum.Supervisor\\wwwroot",
							 $"C:\\Program Files (x86)\\BrightEye\\Momentum\\Happy Sauce 17.3\\WebServer\\Default\\Momentum.Operator\\wwwroot",
							 "",
							 "");

						JsonSerializer.Serialize<Settings>(fs, instance);
					}
				}
				else
				{
					using(FileStream fs = new FileStream(confFile, FileMode.Open))
					{
						try
						{
							instance = JsonSerializer.Deserialize<Settings>(fs);

						}
						catch(Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
					}
				}
			}
			return instance;
		}

		public static void SaveToFile()
		{
			using(FileStream fs = new FileStream(confFile, FileMode.Create))
			{
				JsonSerializer.Serialize<Settings>(fs, instance);
			}
		}

		public Settings(byte timePeriod, bool killProcess, bool replaceWebProjects, bool autoChoosingRepos, 
			string reposFolder, string instanceFolder,string programDirectory,
			string webProjectSupervisorFolderFromRepos, string webProjectOperatorFolderFromRepos, 
			string webProjectSupervisorFolderFromInstance, string webProjectOperatorFolderFromInstance, 
			string customFolderToRemoveFromRepos,string customFolderToRemoveFromInstance)
		{
			this.timePeriod = timePeriod;
			this.killProcess = killProcess;
			this.replaceWebProjects = replaceWebProjects;
			this.autoChoosingRepos = autoChoosingRepos;
			this.instanceFolder = instanceFolder;
			this.reposFolder = reposFolder;
			this.instanceFolder = instanceFolder;
			this.programDirectory = programDirectory;
			this.webProjectSupervisorFolderFromRepos = webProjectSupervisorFolderFromRepos;
			this.webProjectOperatorFolderFromRepos = webProjectOperatorFolderFromRepos;
			this.webProjectSupervisorFolderFromInstance = webProjectSupervisorFolderFromInstance;
			this.webProjectOperatorFolderFromInstance = webProjectOperatorFolderFromInstance;
			this.customFolderToRemoveFromInstance = customFolderToRemoveFromInstance;
			this.customFolderToRemoveFromRepos = customFolderToRemoveFromRepos;
		}
	}
}

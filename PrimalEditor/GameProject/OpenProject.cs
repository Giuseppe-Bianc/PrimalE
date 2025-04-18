using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PrimalEditor.GameProject {
	[DataContract]
	public class ProjectData {
		[DataMember]
		public string? ProjectName { get; set; }
		[DataMember]
		public string? ProjectPath { get; set; }
		[DataMember]
		public DateTime? LastOpenTime { get; set; }
		public string FilePath => Path.Combine(ProjectPath ?? "", $"{ProjectName}{Project.Extention}");
		public byte[]? Icon { get; set; }
		public byte[]? ScreenShot { get; set; }
	}

	[DataContract]
	public class ProjectDataList {
		[DataMember]
		public List<ProjectData> projectsData { get; set; }
	}
	internal class OpenProject {
		static private readonly string _projectsDataDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PrimalEditor");
		static private string _projectsDataPath = string.Empty;

		static private ObservableCollection<ProjectData> _projects = new ObservableCollection<ProjectData>();
		static public ReadOnlyObservableCollection<ProjectData> Projects {
			get;
		}

		/*从recentProjectsData.xml读取项目信息，保存到属性‘Projects’中*/
		static private void ReadRecentProjectsData() {
			if (!File.Exists(_projectsDataPath))
				return;
			try {
				List<ProjectData>? projectsData = Serializer.FromFile<ProjectDataList>(_projectsDataPath)?.projectsData.OrderByDescending(x => x.LastOpenTime).ToList();
				if (projectsData != null) {
					_projects.Clear();
					foreach (var projectData in projectsData) {
						if (File.Exists(projectData.FilePath)) {
							projectData.Icon = File.ReadAllBytes(Path.Combine(Path.Combine(projectData.ProjectPath ?? "", ".Primal"), "icon.png"));
							projectData.ScreenShot = File.ReadAllBytes(Path.Combine(Path.Combine(projectData.ProjectPath ?? "", ".Primal"), "screenShot.png"));
							_projects.Add(projectData);
						}
					}
				}
			} catch (Exception ex) {
				Debug.Write(ex.Message);
				throw;
			}

		}

		static private void WriteRecentProjectsData() {
			try {
				// 写入的时候按照“从老到新”的顺序
				Serializer.ToFile(new ProjectDataList() { projectsData = _projects.OrderBy(x => x.LastOpenTime).ToList() }, _projectsDataPath);
			} catch (Exception ex) {
				Debug.Write(ex.Message);
				throw;
			}
		}

		static public Project Open(ProjectData projectData) {
			ReadRecentProjectsData();

			var project = _projects.FirstOrDefault(x => projectData.FilePath == x.FilePath);
			if (project == null) {
				projectData.LastOpenTime = DateTime.Now;
				_projects.Add(projectData);
				project = projectData;
			} else {
				project.LastOpenTime = DateTime.Now;
			}
			WriteRecentProjectsData();

			return Project.Load(project.FilePath);
		}

		static OpenProject() {
			Projects = new ReadOnlyObservableCollection<ProjectData>(_projects);
			try {
				if (!Directory.Exists(_projectsDataDirPath)) {
					Directory.CreateDirectory(_projectsDataDirPath);
				}
				_projectsDataPath = Path.Combine(_projectsDataDirPath, "ProjectsData.xml");

				ReadRecentProjectsData();
			} catch (Exception ex) {
				Debug.Write(ex.Message);
				throw;
			}
		}
	}
}
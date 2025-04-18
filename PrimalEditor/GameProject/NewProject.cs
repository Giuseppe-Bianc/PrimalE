using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PrimalEditor.GameProject {
	[DataContract]
	public class ProjectTemplate {
		[DataMember]
		public string ProjectType { get; set; }
		[DataMember]
		public string ProjectFile { get; set; }
		[DataMember]
		public List<string> Folders { get; set; }

		public byte[] Icon { get; set; }
		public byte[] Screenshot { get; set; }
		public string IconFilePath { get; set; }
		public string ScreenshotFilePath { get; set; }
		public string ProjectFilePath { get; set; }
	}
	public class NewProject : ViewModelBase {
		private readonly string _templatePath = $@"..\..\PrimalEditor\ProjectTemplates";
		private string _projectName = "NewProject";

		public string ProjectName {
			get => _projectName;
			set {
				if (_projectName != value) {
					_projectName = value;
					OnPropertyChanged(nameof(ProjectName));
				}
			}
		}

		private string _projectpath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\PrimalProject\";

		public string ProjectPath {
			get => _projectpath;
			set {
				if (_projectpath != value) {
					_projectpath = value;
					OnPropertyChanged(nameof(ProjectPath));
				}
			}
		}

		private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
		public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

		public NewProject() {
			ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
			try {
				var templatesFiles = System.IO.Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
				Debug.Assert(templatesFiles.Any());
				foreach (var file in templatesFiles) {
					var temaplate = Serializer.FromFile<ProjectTemplate>(file);
					temaplate.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "icon.png"));
					temaplate.Icon = File.ReadAllBytes(temaplate.IconFilePath);
					temaplate.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png"));
					temaplate.Screenshot = File.ReadAllBytes(temaplate.ScreenshotFilePath);
					temaplate.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), temaplate.ProjectFile));
					_projectTemplates.Add(temaplate);
				}
			} catch (Exception ex) {
				// Handle exceptions here
				Console.WriteLine($"Error: {ex.Message}");
			}

		}
	}
}


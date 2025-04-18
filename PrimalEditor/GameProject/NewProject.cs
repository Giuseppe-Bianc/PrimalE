using PrimalEditor.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

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
		private readonly string _templatePath = $@"..\..\PrimalEditor\ProjectTemplates\";
		private string _projectName = "NewProject";

		public string ProjectName {
			get => _projectName;
			set {
				if (_projectName != value) {
					_projectName = value;
					ValidateProjectPath();
					OnPropertyChanged(nameof(ProjectName));
				}
			}
		}

		private string _projectpath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\PrimalProjects\";

		public string ProjectPath {
			get => _projectpath;
			set {
				if (_projectpath != value) {
					_projectpath = value;
					ValidateProjectPath();
					OnPropertyChanged(nameof(ProjectPath));
				}
			}
		}

		private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
		public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

		private bool _isValid;

		public bool IsValid {
			get => _isValid;
			set {
				if (_isValid != value) {
					_isValid = value;
					OnPropertyChanged(nameof(IsValid));
				}
			}
		}

		private string _errorMsg;

		public string ErrorMsg {
			get => _errorMsg;
			set {
				if (_errorMsg != value) {
					_errorMsg = value;
					OnPropertyChanged(nameof(ErrorMsg));
				}
			}
		}

		private bool ValidateProjectPath() {
			var path = ProjectPath;
			if (!Path.EndsInDirectorySeparator(path)) path += @"\";
			path += $@"{ProjectName}\";
			IsValid = false;
			if (string.IsNullOrEmpty(ProjectName.Trim())) {
				ErrorMsg = "Project name cannot be empty.";
			} else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1) {
				ErrorMsg = "Project name contains invalid characters.";
			} else if (string.IsNullOrEmpty(ProjectPath.Trim())) {
				ErrorMsg = "Project path cannot be empty.";
			} else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1) {
				ErrorMsg = "Project path contains invalid characters.";
			} else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any()) {
				ErrorMsg = "Project already exists.";
			} else {
				IsValid = true;
				ErrorMsg = string.Empty;
			}
			return IsValid;
		}

		public string CreateProject(ProjectTemplate template) {
			ValidateProjectPath();
			if (!IsValid) {
				return string.Empty;
			}

			if (!Path.EndsInDirectorySeparator(ProjectPath)) ProjectPath += @"\";
			var path = $@"{ProjectPath}{ProjectName}\";
			try {
				if (!Directory.Exists(path)) {
					Directory.CreateDirectory(path);
				}
				foreach (string fileName in template.Folders) {
					Directory.CreateDirectory(Path.GetFullPath(Path.Combine(path, fileName)));
				}
				DirectoryInfo dotPrimalDir = new DirectoryInfo((Path.GetFullPath(Path.Combine(path, ".Primal"))));
				dotPrimalDir.Attributes |= FileAttributes.Hidden;
				File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dotPrimalDir.FullName, "icon.png")));
				File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dotPrimalDir.FullName, "ScreenShot.png")));

				var projectXML = File.ReadAllText(template.ProjectFilePath);
				projectXML = string.Format(projectXML, ProjectName, path);
				var projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extention}"));
				File.WriteAllText(projectPath, projectXML);

				return path;
			} catch (Exception ex) {
				Debug.WriteLine($"Error: {ex.Message}");
				return string.Empty;
			}
		}

		public NewProject() {
			ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
			try {
				var templatesFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
				Debug.Assert(templatesFiles.Any());
				foreach (var file in templatesFiles) {
					var temaplate = Serializer.FromFile<ProjectTemplate>(file);
					var dir = Path.GetDirectoryName(file);
					temaplate.IconFilePath = Path.GetFullPath(Path.Combine(dir, "icon.png"));
					temaplate.Icon = File.ReadAllBytes(temaplate.IconFilePath);
					temaplate.ScreenshotFilePath = Path.GetFullPath(Path.Combine(dir, "Screenshot.png"));
					temaplate.Screenshot = File.ReadAllBytes(temaplate.ScreenshotFilePath);
					temaplate.ProjectFilePath = Path.GetFullPath(Path.Combine(dir, temaplate.ProjectFile));
					_projectTemplates.Add(temaplate);
				}
				ValidateProjectPath();
			} catch (Exception ex) {
				// Handle exceptions here
				Debug.WriteLine($"Error: {ex.Message}");
			}

		}
	}
}


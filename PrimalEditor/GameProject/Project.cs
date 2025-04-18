﻿using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PrimalEditor.GameProject {

	[DataContract(Name = "Game")]
	public class Project : ViewModelBase {
		static public string Extention => ".primal";
		
		[DataMember]
		public string Name { get; private set; } = "New Project";
		[DataMember]
		public string Path { get; private set; }


		public string FilePath => System.IO.Path.GetFullPath(System.IO.Path.Combine($"{Path}", $"{Name}{Extention}"));

		[DataMember(Name = "Scenes")]
		private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
		public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

		private Scene _activeScene;
		public Scene ActiveScene {
			get => _activeScene;
			set {
				if (_activeScene != value) {
					_activeScene = value;
					OnPropertyChanged(nameof(ActiveScene));
				}
			}
		}

		public static Project Current => Application.Current.MainWindow.DataContext as Project;

		public static Project Load(string file) {
			Debug.Assert(File.Exists(file));
			return Serializer.FromFile<Project>(file);
		}

		public static void save(Project project) {
			Serializer.ToFile(project, project.FilePath);
		}

		public void Unload() {
			
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context) {
			if (_scenes == null) {
				Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
				OnPropertyChanged(nameof(Scenes));
			}
			ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);
		}

#pragma warning disable CS8618
		public Project(string name, string path) {
			Name = name;
			Path = path;
			OnDeserialized(new StreamingContext());
		}
#pragma warning restore CS8618
	}
}

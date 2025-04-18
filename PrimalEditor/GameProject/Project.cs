﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PrimalEditor.GameProject {

	[DataContract(Name = "Game")]
	public class Project : ViewModelBase {
		[DataMember]
		public string Name { get; private set; }
		[DataMember]
		public string Path { get; private set; }

		static public string Extention => ".primal";

		public string FilePath => System.IO.Path.GetFullPath(System.IO.Path.Combine($"{Path}", $"{Name}{Extention}"));

		[DataMember(Name = "Scenes")]
		private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
		public ReadOnlyObservableCollection<Scene> Scenes { get; }

#pragma warning disable CS8618
		public Project(string name, string path) {
			Name = name;
			Path = path;
			_scenes.Add(new Scene(this, "Default Scene"));
		}
#pragma warning restore CS8618
	}
}

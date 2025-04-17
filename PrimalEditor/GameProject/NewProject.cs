using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimalEditor.GameProject {
	public class NewProject : ViewModelBase {
		private string _name = "NewProject";

		public string Name {
			get => _name;
			set {
				if (_name != value) {
					_name = value;
					OnPropertyChanged(nameof(Name));
				}
			}
		}

		private string _path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\PrimalProject\";

		public string Path {
			get => _path;
			set {
				if (_path != value) {
					_path = value;
					OnPropertyChanged(nameof(Path));
				}
			}
		}

	}
}


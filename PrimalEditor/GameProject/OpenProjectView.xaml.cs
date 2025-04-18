﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PrimalEditor.GameProject {
	/// <summary>
	/// Logica di interazione per OpenProjectView.xaml
	/// </summary>
	public partial class OpenProjectView : UserControl {
		public OpenProjectView() {
			InitializeComponent();
		}
#pragma warning disable CS8602, CS8604
		private void OnOpen_Button_Click(object sender, RoutedEventArgs e) {
			OpenSelectedProject();
		}

		private void OnListBoxItem_Mouse_DoubleClick(object sender, MouseButtonEventArgs e) {
			OpenSelectedProject();
		}

		private void OpenSelectedProject() {
			var project = OpenProject.Open(projectsDataListBox.SelectedItem as ProjectData);
			bool dialogResult = false;
			var win = Window.GetWindow(this);
			if (project != null) {
				dialogResult = true;
			}
			win.DialogResult = dialogResult;
			win.Close();
		}

#pragma warning restore CS8602, CS8604

	}
}

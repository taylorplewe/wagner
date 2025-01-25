using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Wagner
{
	/// <summary>
	/// Interaction logic for IObjs.xaml
	/// </summary>
	public partial class IObjs : Window
	{
		public List<string> names;
		private WindowSet windows;

		public IObjs(WindowSet windows)
		{
			InitializeComponent();

			this.windows = windows;
			names = new List<string>();
			names.Add("");
		}

		// window
		private void button_close_Click(object sender, RoutedEventArgs e)
		{
			Hide();
		}
		private void window_iobjs_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && grid_top_bar.IsMouseOver)
				DragMove();
		}

		public void button_new_Click(object sender, RoutedEventArgs e)
		{
			StackPanel panel = new StackPanel()
			{
				Orientation = Orientation.Horizontal,
				Margin = new Thickness(0,8,0,0),
				HorizontalAlignment = HorizontalAlignment.Right
			};
			Label lbl = new Label() { Content = panel_iobjs.Children.Count };
			TextBox box = new TextBox { Width = 180 };
			box.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);
			Button up = new Button() { Margin = new Thickness(8, 0, 0, 0), Content = "↑" };
			up.Click += new RoutedEventHandler(button_up_Click);
			Button down = new Button() { Visibility = Visibility.Hidden, Margin = new Thickness(8, 0, 0, 0), Content = "↓" };
			down.Click += new RoutedEventHandler(button_down_Click);
			Button ex = new Button() { Margin = new Thickness(8, 0, 0, 0), Content = "×" };
			ex.Click += new RoutedEventHandler(button_ex_Click);
			CheckBox check = new CheckBox() { Content = "Visible" };
			panel.Children.Add(lbl);
			panel.Children.Add(box);
			panel.Children.Add(up);
			panel.Children.Add(down);
			panel.Children.Add(ex);
			panel.Children.Add(check);
			panel_iobjs.Children.Add(panel);
			EnableExes();
			RefreshButtons();

			// add to code-behind string array
			names.Add("");
		}
		private void button_up_Click(object sender, RoutedEventArgs e)
		{
			StackPanel panel = ((Button)sender).Parent as StackPanel;
			int ind = panel_iobjs.Children.IndexOf(panel);
			panel_iobjs.Children.Remove(panel);
			panel_iobjs.Children.Insert(ind - 1, panel);
			RefreshButtons();

			// update code-behind version
			string name = names[ind];
			names.RemoveAt(ind);
			names.Insert(ind - 1, name);

			// update map.iobjs
			List<int> oldOnes = new List<int>();
			for (int i = 0; i < windows.map.iobjs.Count; i++)
			{
				if (windows.map.iobjs[i].type == ind)
					oldOnes.Add(i);
				else if (windows.map.iobjs[i].type == ind - 1)
					windows.map.iobjs[i].type++;
			}
			foreach (int i in oldOnes)
				windows.map.iobjs[i].type--;
		}
		private void button_down_Click(object sender, RoutedEventArgs e)
		{
			StackPanel panel = ((Button)sender).Parent as StackPanel;
			int ind = panel_iobjs.Children.IndexOf(panel);
			panel_iobjs.Children.Remove(panel);
			panel_iobjs.Children.Insert(ind + 1, panel);
			RefreshButtons();

			// update code-behind version
			string name = names[ind];
			names.RemoveAt(ind);
			names.Insert(ind + 1, name);

			// update map.iobjs
			List<int> oldOnes = new List<int>();
			for (int i = 0; i < windows.map.iobjs.Count; i++)
			{
				if (windows.map.iobjs[i].type == ind)
					oldOnes.Add(i);
				else if (windows.map.iobjs[i].type == ind + 1)
					windows.map.iobjs[i].type--;
			}
			foreach (int i in oldOnes)
				windows.map.iobjs[i].type++;
		}
		private void button_ex_Click(object sender, RoutedEventArgs e)
		{
			StackPanel panel = ((Button)sender).Parent as StackPanel;
			int ind = panel_iobjs.Children.IndexOf(panel);
			panel_iobjs.Children.Remove(panel);
			RefreshButtons();
			if (panel_iobjs.Children.Count == 1)
				((StackPanel)panel_iobjs.Children[0]).Children[4].Visibility = Visibility.Hidden;

			// update code-behind version
			names.RemoveAt(ind);

			// update iobjs on map
			int iobjInd = 0;
			while (iobjInd < windows.map.iobjs.Count)
			{
				if (windows.map.iobjs[iobjInd].type == ind)
				{
					windows.map.iobjs.RemoveAt(iobjInd);
					windows.map.grid_iobjs.Children.RemoveAt(iobjInd);
				}
				else
					iobjInd++;
			}

			// update type #'s of windows.map.iobjs
			for (int i = ind + 1; i < panel_iobjs.Children.Count + 1; i++)
			{
				foreach (PlaceObj iobj in windows.map.iobjs)
				{
					if (iobj.type == i)
						iobj.type--;
				}
			}
		}

		private void EnableExes()
		{
			foreach (Panel child in panel_iobjs.Children)
				child.Children[4].Visibility = Visibility.Visible;
		}
		private void RefreshButtons()
		{
			for (int i = 0; i < panel_iobjs.Children.Count; i++)
			{
				((Label)((StackPanel)panel_iobjs.Children[i]).Children[0]).Content = i;

				if (i == 0)
					((StackPanel)panel_iobjs.Children[i]).Children[2].Visibility = Visibility.Hidden;
				else
					((StackPanel)panel_iobjs.Children[i]).Children[2].Visibility = Visibility.Visible;
				if (i == panel_iobjs.Children.Count - 1)
					((StackPanel)panel_iobjs.Children[i]).Children[3].Visibility = Visibility.Hidden;
				else
					((StackPanel)panel_iobjs.Children[i]).Children[3].Visibility = Visibility.Visible;
			}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			// update code-behind name
			StackPanel panel = ((TextBox)sender).Parent as StackPanel;
			int ind = panel_iobjs.Children.IndexOf(panel);
			names[ind] = ((TextBox)sender).Text;
		}
	}
}

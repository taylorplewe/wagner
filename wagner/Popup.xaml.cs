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
using Microsoft.Win32;

namespace Wagner
{
	/// <summary>
	/// Interaction logic for Popup.xaml
	/// </summary>
	public partial class Popup : Window
	{
		public Popup(string titleName, string descrName, string okText = "OK", string cancelText = "Cancel", bool showTopBar = true, bool showButtons = true)
		{
			InitializeComponent();

			label_title.Content = titleName;
			text_descr.Text = descrName;
			if (okText == "")
			{
				button_ok.Visibility = Visibility.Collapsed;
				button_cancel.IsDefault = true;
			}
			if (cancelText == "")
				button_cancel.Visibility = Visibility.Collapsed;
			button_ok.Content = okText;
			button_cancel.Content = cancelText;
			if (!showTopBar)
				grid_top_bar.Visibility = Visibility.Collapsed;
			if (!showButtons)
				panel_buttons.Visibility = Visibility.Collapsed;
		}
		public Popup(string titleName, string textboxLabelText)
		{
			InitializeComponent();

			label_title.Content = titleName;
			text_descr.Text = textboxLabelText;
			textbox_main.Visibility = Visibility.Visible;
			label_spacer.Visibility = Visibility.Hidden;
			textbox_main.Focus();
		}

		private void button_ok_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
		private void button_close_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
		private void window_popup_KeyDown(object sender, KeyEventArgs e)
		{

		}
		private void window_popup_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && grid_top_bar.IsMouseOver)
				DragMove();
		}
	}
}

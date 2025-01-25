using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace Wagner
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private FileSet files;
		public WindowSet windows;
		public bool changesMade = false;
		public UndoHistory undoHist;
		public readonly string DEF_COL_MODE = "mesen";

        public MainWindow()
		{
			InitializeComponent();

			files = new FileSet();
			windows = new WindowSet();

			// initiate windows
			windows.main = this;
			windows.tilePicker = new TilePicker(windows);
			windows.meta = new Metatiles(windows, files);
			windows.objs = new Objects(windows);
			windows.iobjs = new IObjs(windows);
			windows.map = new Map(windows);

			windows.tilePicker.KeyDown += Window_KeyDown;
			windows.meta.KeyDown += Window_KeyDown;
			windows.objs.KeyDown += Window_KeyDown;
			windows.map.KeyDown += Window_KeyDown;
			windows.iobjs.KeyDown += Window_KeyDown;

			undoHist = new UndoHistory(windows);

			// tie their visibility to the menu controls
			windows.tilePicker.IsVisibleChanged += window_IsVisibleChanged;
			windows.meta.IsVisibleChanged += window_IsVisibleChanged;
			windows.objs.IsVisibleChanged += window_IsVisibleChanged;
			windows.map.IsVisibleChanged += window_IsVisibleChanged;
			windows.iobjs.IsVisibleChanged += window_IsVisibleChanged;

			// use the colors from Mesen as default
			windows.tilePicker.UpdateColorPicker(DEF_COL_MODE);

			// if opened with a file
			string[] prms = Environment.GetCommandLineArgs();
			if (prms.Length > 1)
				OpenWAG(File.ReadAllBytes(prms[1]));
		}

		public void ChangesMade()
		{
			// let program know we've made changes
			changesMade = true;
			menu_saveas.IsEnabled = true;
			if (files.wag != "")
			{
				menu_save.IsEnabled = true;
				menu_save_export.IsEnabled = true;
			}
			undoHist.RecordState();
		}
		public void RedrawAll()
		{
			//windows.tilePicker.InitPalettes();
			windows.tilePicker.UpdatePalette();
			windows.tilePicker.UpdatePaletteCanvases();
			windows.tilePicker.LoadChr();
			windows.objs.UpdateObjects(windows.objs.combobox_objects.SelectedIndex);
			windows.objs.UpdateStates(windows.objs.combobox_states.SelectedIndex);
			windows.meta.DrawMetatiles();
			windows.map.InitMap();
			windows.map.RedrawObjs();
		}
		private string ToBinaryString(byte num)
		{
			string retString = Convert.ToString(num, 2);
			while (retString.Length < 8)
				retString = retString.Insert(0, "0");
			return retString;
		}
		private void FilterNumber(object sender)
		{
			((TextBox)sender).Text = Regex.Replace(((TextBox)sender).Text, "[^0-9.]", "");
		}

		// window
		private void Window_Closed(object sender, EventArgs e)
		{
			windows.map.Close();
			windows.tilePicker.Close();
			windows.meta.Close();
			windows.objs.Close();
			windows.iobjs.Close();
		}
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			bool ctrlDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
			bool shiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
			bool altDown = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
			if (e.Key == Key.M && ctrlDown)
			{
				if (sender == windows.map)
					return;
				menu_Click(menu_map, null);
			}
			else if (e.Key == Key.T && ctrlDown)
			{
				if (sender == windows.tilePicker)
					return;
				menu_Click(menu_tile_picker, null);
			}
			else if (e.Key == Key.E && ctrlDown)
			{
				if (sender == windows.meta)
					return;
				menu_Click(menu_meta, null);
			}
			else if (e.Key == Key.B && ctrlDown)
			{
				if (sender == windows.objs)
					return;
				menu_Click(menu_objs, null);
			}
			else if (e.Key == Key.I && ctrlDown)
			{
				if (sender == windows.iobjs)
					return;
				menu_Click(menu_iobjs, null);
			}
			else if (e.Key == Key.S && ctrlDown)
			{
				if (menu_save.IsEnabled)
				{
					if (shiftDown)
						menu_save_export_Click(null, null);
					else
						menu_save_Click(null, null);
				}
			}
			else if (e.Key == Key.W && ctrlDown)
			{
				menu_exit_Click(null, null);
			}
			else if (e.Key == Key.O && ctrlDown)
			{
				menu_open_Click(null, null);
			}
			else if (e.Key == Key.Z && ctrlDown)
			{
				undoHist.Undo();
			}
			else if (e.Key == Key.Y && ctrlDown)
			{
				undoHist.Redo();
			}
			else if (e.Key == Key.W && shiftDown)
			{
				menu_Click(menu_wiki, null);
			}
			else if (e.Key == Key.M && shiftDown)
			{
				menu_Click(menu_mesen, null);
			}
			else if (e.Key == Key.A && shiftDown)
			{
				menu_Click(menu_ase, null);
			}
			else if (e.Key == Key.T && shiftDown)
			{
				menu_Click(menu_theo, null);
			}
			else if (e.Key == Key.Y && shiftDown)
			{
				menu_Click(menu_yychr, null);
			}
			else if (e.Key == Key.F && shiftDown)
			{
				menu_Click(menu_fceux, null);
			}
			else if (e.Key == Key.U)
			{
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						Console.WriteLine(windows.tilePicker.bgPaletteInds[i][j]);
					}
					Console.WriteLine();
				}
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						Console.WriteLine(windows.tilePicker.sprPaletteInds[i][j]);
					}
					Console.WriteLine();
				}
			}
		}
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (changesMade)
			{
				Popup popup = new Popup("Exit", "You have unsaved changes.  Are you sure you want to exit?", "Yes");
				popup.Owner = this;
				if (!(bool)popup.ShowDialog())
					e.Cancel = true;
			}
		}

		// menus
		private void menu_exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		private void menu_open_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Wagner editor files (*.wag) | *.wag";
			if ((bool)dialog.ShowDialog())
			{
				if (changesMade)
				{
					Popup popup = new Popup("Exit", "You have unsaved changes.  Are you sure you want to exit?", "Yes");
					popup.Owner = this;
					if (!(bool)popup.ShowDialog())
						return;
				}
				OpenWAG(File.ReadAllBytes(dialog.FileName));
			}
		}
		private void menu_save_Click(object sender, RoutedEventArgs e)
		{
			// save to the .wag file on file, if there is one; otherwise act as "save as..."
			if (files.wag != "" && File.Exists(files.wag))
				File.WriteAllBytes(files.wag, SaveWAG());
			else
				menu_saveas_Click(null, null);
		}
		private void menu_saveas_Click(object sender, RoutedEventArgs e)
		{
			// save to a .wag file
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = "Wagner editor files (*.wag) | *.wag";
			if ((bool)dialog.ShowDialog())
			{
				files.wag = dialog.FileName;
				File.WriteAllBytes(dialog.FileName, SaveWAG());
			}
		}
		private void menu_save_export_Click(object sender, RoutedEventArgs e)
		{
			menu_save_Click(null, null);
			button_ex_all_Click(null, null);
		}
		private void menu_Click(object sender, RoutedEventArgs e)
		{
			if (sender == menu_tile_picker)
			{
				if (windows.tilePicker.IsVisible)
					windows.tilePicker.Hide();
				else
					windows.tilePicker.Show();
			}
			else if (sender == menu_map)
			{
				if (windows.map.IsVisible)
					windows.map.Hide();
				else
					windows.map.Show();
			}
			else if (sender == menu_meta)
			{
				if (windows.meta.IsVisible)
					windows.meta.Hide();
				else
					windows.meta.Show();
			}
			else if (sender == menu_objs)
			{
				if (windows.objs.IsVisible)
					windows.objs.Hide();
				else
					windows.objs.Show();
			}
			else if (sender == menu_iobjs)
			{
				if (windows.iobjs.IsVisible)
					windows.iobjs.Hide();
				else
					windows.iobjs.Show();
			}
			else if (sender == menu_wiki)
			{
				windows.tilePicker.UpdateColorPicker("wiki");
				RedrawAll();
			}
			else if (sender == menu_mesen)
			{
				windows.tilePicker.UpdateColorPicker("mesen");
				RedrawAll();
			}
			else if (sender == menu_ase)
			{
				windows.tilePicker.UpdateColorPicker("ase");
				RedrawAll();
			}
			else if (sender == menu_theo)
			{
				windows.tilePicker.UpdateColorPicker("theo");
				RedrawAll();
			}
			else if (sender == menu_yychr)
			{
				windows.tilePicker.UpdateColorPicker("yychr");
				RedrawAll();
			}
			else if (sender == menu_fceux)
			{
				windows.tilePicker.UpdateColorPicker("fceux");
				RedrawAll();
			}
		}
		private void window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender == windows.tilePicker)
				menu_tile_picker.IsChecked = windows.tilePicker.IsVisible;
			else if (sender == windows.map)
				menu_map.IsChecked = windows.map.IsVisible;
			else if (sender == windows.meta)
				menu_meta.IsChecked = windows.meta.IsVisible;
			else if (sender == windows.objs)
				menu_objs.IsChecked = windows.objs.IsVisible;
			else if (sender == windows.iobjs)
				menu_iobjs.IsChecked = windows.iobjs.IsVisible;
		}
		private void menu_about_Click(object sender, RoutedEventArgs e)
		{
			Popup aboutPopup = new Popup("About", "NES Graphics Editor\nby Taylor Plewe\nVersion 1.0 - November 15, 2021", "", "OK");
			aboutPopup.ShowDialog();
		}

		// forms (buttons, checkboxes)
		private void button_im_all_Click(object sender, RoutedEventArgs e)
		{
			// do the import code
			ImportPal();
			ImportCHR();
			ImportMetatiles();
			ImportMapTiles();
			ImportMapAttrs();
			ImportWalls();
			ImportObjs();
			ImportPlr();

			// open windows
			RedrawAll();
		}
		private void button_ex_all_Click(object sender, RoutedEventArgs e)
		{
			Popup exportPopup = new Popup("Export", "Are you sure you want to overwrite ALL the files?", "Yes");
			exportPopup.Owner = this;
			if ((bool)exportPopup.ShowDialog())
			{
				ExportMetatiles();
				ExportMapTiles();
				ExportMapAttrs();
				ExportWalls();
				ExportRoom();
			}
		}
		private void button_open_Click(object sender, RoutedEventArgs e)
		{
			string name = ((FrameworkElement)sender).Name; // must be in format button_{variant}....
			string variant = name.Split('_')[1];
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Title = $"Open {TypeMap(variant)} file";
			if (variant == "chr")
				dialog.Filter = "NES CHR files (*.chr) | *.chr";
			else
				dialog.Filter = "6502 assembly files (*.asm) | *.asm";
			if ((bool)dialog.ShowDialog())
			{
				((TextBox)FindName("textbox_" + variant + "file")).Text = FormatFileString(dialog.FileName);
				((Button)FindName("button_" + variant + "_im")).IsEnabled = true;
				((Button)FindName("button_" + variant + "_ex")).IsEnabled = true;
				if ((TextBox)FindName("textbox_" + variant + "_data_appear") != null)
					((TextBox)FindName("textbox_" + variant + "_data_appear")).Text = FormatFileString(dialog.FileName);
				files.SetVariantString(variant, dialog.FileName);
			}
		}
		private void button_im_Click(object sender, RoutedEventArgs e)
		{
			string name = ((FrameworkElement)sender).Name; // must be in format button_{variant}....
			string variant = name.Split('_')[1];
			if (variant == "chr")
			{
				ImportCHR();
				RedrawAll();
			}
			else if (variant == "pal")
			{
				ImportPal();
				ImportCHR();
				RedrawAll();
			}
			else if (variant == "maptiles")
			{
				Popup metaPopup = new Popup(
					"Import tiles",
					"You have \"Use metatiles\" set to:\n  " + ((bool)checkbox_meta.IsChecked).ToString() + "\nAre you sure this is correct?",
					"Yes");
				if ((bool)metaPopup.ShowDialog())
				{
					if (windows.map.tiles == null)
					{
						ImportMapTiles();
						windows.map.InitMap();
					}
					else
					{
						Popup popup = new Popup("Import tiles", "Are you sure you want to import a new set of tiles?  The existing ones you've been working on will be lost.", "Yes");
						if ((bool)popup.ShowDialog())
						{
							ImportMapTiles();
							windows.map.InitMap();
						}

					}
				}
			}
			else if (variant == "mapattrs")
			{
				if (windows.map.attrs == null)
				{
					ImportMapAttrs();
					windows.map.InitMap();
				}
				else
				{
					Popup popup = new Popup("Import attributes", "Are you sure you want to import a new set of attributes?  The existing ones you've been working on will be lost.", "Yes");
					if ((bool)popup.ShowDialog())
					{
						ImportMapAttrs();
						windows.map.InitMap();
					}
				}
			}
			else if (variant == "metatiles")
			{
				if (windows.meta.metatiles == null)
				{
					ImportMetatiles();
				}
				else
				{
					Popup popup = new Popup("Import metatiles", "Are you sure you want to import a new set of metatiles?  The existing ones you've been working on will be lost.", "Yes");
					if ((bool)popup.ShowDialog())
					{
						ImportMetatiles();
					}
				}
			}
			else if (variant == "walls")
			{
				ImportWalls();
				windows.map.InitMap();
			}
			else if (variant == "objs")
			{
				ImportObjs();
				windows.objs.UpdateObjects(0);
				windows.objs.UpdateStates(0);
			}
			else if (variant == "plr")
			{
				ImportPlr();
				windows.objs.UpdateObjects(0);
				windows.objs.UpdateStates(0);
			}
		}
		private void button_ex_Click(object sender, RoutedEventArgs e)
		{
			string name = ((FrameworkElement)sender).Name; // must be in format button_{variant}....
			string variant = name.Split('_')[1];
			string fullName = TypeMap(variant);
			Popup exportPopup = new Popup("Export", "Are you sure you want to overwrite the original " + fullName + " file?", "Yes");
			exportPopup.Owner = this;
			if ((bool)exportPopup.ShowDialog())
			{
				if (variant == "maptiles")
					ExportMapTiles();
				else if (variant == "mapattrs")
					ExportMapAttrs();
				else if (variant == "metatiles")
					ExportMetatiles();
				else if (variant == "walls")
					ExportWalls();
				else if (variant == "objs")
					ExportObjs();
				else if (variant == "plr")
					ExportPlr();
				else if (variant == "room")
					ExportRoom();
			}
		}
		private void checkbox_meta_Checked(object sender, RoutedEventArgs e)
		{
			windows.map.radio_tile.Content = "Metatile";
			windows.map.label_height_units.Content = "16x16 metatiles";
			windows.map.label_width_units.Content = "16x16 metatiles";
			windows.map.checkbox_attr_from_meta.Visibility = Visibility.Visible;
			windows.map.checkbox_attr_from_meta.IsChecked = true;
			windows.map.radio_palette.IsEnabled = false;
			if ((bool)windows.map.radio_palette.IsChecked) windows.map.radio_tile.IsChecked = true;
			windows.map.textbox_width.Text = (windows.map.mapWidth / 2).ToString();
			windows.map.textbox_height.Text = (windows.map.mapHeight / 2).ToString();
			windows.map.InitMap();
		}
		private void checkbox_meta_Unchecked(object sender, RoutedEventArgs e)
		{
			windows.map.radio_tile.Content = "Tile";
			windows.map.label_height_units.Content = "8x8 tiles";
			windows.map.label_width_units.Content = "8x8 tiles";
			windows.map.checkbox_attr_from_meta.Visibility = Visibility.Collapsed;
			windows.map.radio_palette.IsEnabled = true;
			windows.map.textbox_width.Text = windows.map.mapWidth.ToString();
			windows.map.textbox_height.Text = windows.map.mapHeight.ToString();
			windows.map.InitMap();
		}
		private void textbox_bank_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterNumber(sender);
			if (textbox_bank.Text.Length > 0)
			{
				if (int.Parse(textbox_bank.Text) > 6)
					textbox_bank.Text = "6";
				else if (int.Parse(textbox_bank.Text) < 0)
					textbox_bank.Text = "0";
			}
			else
				textbox_bank.Text = "0";
		}

		private string FormatFileString(string text)
		{
			if (text.Length == 0)
				return text;
			string fitTxt = text;
			int i = fitTxt.Length - 1;
			byte slashes = 0;
			while (i > 0 && slashes < 4)
			{
				i--;
				if (fitTxt[i] == '\\')
					slashes++;
			}
			if (i == 0)
				return fitTxt;
			else
				return fitTxt.Substring(i, fitTxt.Length - i).Insert(0, "...");
		}
		private string TypeMap(string variant)
		{
			return
				variant == "pal" ? "palettes" :
				variant == "maptiles" ? "tiles" :
				variant == "mapattrs" ? "attributes" :
				variant == "objs" ? "object graphics" :
				variant == "plr" ? "player graphics" : variant;
		}

		// import
		private void ImportCHR()
		{
			if (files.chr == "")
				return;
			if (!File.Exists(files.chr))
            {
				Popup exportPopup = new Popup("File doesn't exist", $"File \"{files.chr}\" not found!", "OK");
				exportPopup.Owner = this;
				exportPopup.ShowDialog();
				return;
			}
			byte[] allBytes = File.ReadAllBytes(files.chr);

			if (allBytes.Length != 8192)
				return;

			for (int i = 0; i < 512; i++)
			{
				byte[] byteBatch = new byte[16];
				for (int j = 0; j < 16; j++)
				{
					byteBatch[j] = (j & 1) == 0
						? (byte)
							(
								((allBytes[(i * 16) + (j / 2) + 0] >> 1) & 0b01000000) |
								((allBytes[(i * 16) + (j / 2) + 0] >> 2) & 0b00010000) |
								((allBytes[(i * 16) + (j / 2) + 0] >> 3) & 0b00000100) |
								((allBytes[(i * 16) + (j / 2) + 0] >> 4) & 0b00000001) |
								((allBytes[(i * 16) + (j / 2) + 8] >> 0) & 0b10000000) |
								((allBytes[(i * 16) + (j / 2) + 8] >> 1) & 0b00100000) |
								((allBytes[(i * 16) + (j / 2) + 8] >> 2) & 0b00001000) |
								((allBytes[(i * 16) + (j / 2) + 8] >> 3) & 0b00000010)
							)
						: (byte)
							(
								((allBytes[(i * 16) + ((j - 1) / 2) + 0] << 0) & 0b00000001) |
								((allBytes[(i * 16) + ((j - 1) / 2) + 0] << 1) & 0b00000100) |
								((allBytes[(i * 16) + ((j - 1) / 2) + 0] << 2) & 0b00010000) |
								((allBytes[(i * 16) + ((j - 1) / 2) + 0] << 3) & 0b01000000) |
								((allBytes[(i * 16) + ((j - 1) / 2) + 8] << 1) & 0b00000010) |
								((allBytes[(i * 16) + ((j - 1) / 2) + 8] << 2) & 0b00001000) |
								((allBytes[(i * 16) + ((j - 1) / 2) + 8] << 3) & 0b00100000) |
								((allBytes[(i * 16) + ((j - 1) / 2) + 8] << 4) & 0b10000000)
							);
				}
				windows.tilePicker.bmps[i] = BitmapSource.Create(
					8, 8, 2, 2, PixelFormats.Indexed2, new BitmapPalette(windows.tilePicker.bgPalettes[windows.tilePicker.selectedPal - 1]), byteBatch, 2);
			}
		}
		private void ImportPal()
		{
			if (files.pal == "")
				return;
			if (!File.Exists(files.pal))
			{
				Popup noFilePopup = new Popup("File doesn't exist", $"File \"{files.pal}\" not found!", "OK");
				noFilePopup.Owner = this;
				noFilePopup.ShowDialog();
				return;
			}
			windows.tilePicker.bgPalettes = new Color[4][] {
					new Color[4],
					new Color[4],
					new Color[4],
					new Color[4]
				};
			string palFileText = File.ReadAllText(files.pal);
			string[] vals = palFileText.Split('$');
			for (int i = 1; i < vals.Length; i++)
			{
				windows.tilePicker.bgPalettes[(i - 1) / 4][(i - 1) % 4] = windows.tilePicker.colors[vals[i].Substring(0, 2)];
			}
		}
		private void ImportMapTiles()
		{
			if (files.tiles == "")
				return;
			if (!File.Exists(files.tiles))
			{
				Popup noFilePopup = new Popup("File doesn't exist", $"File \"{files.tiles}\" not found!", "OK");
				noFilePopup.Owner = this;
				noFilePopup.ShowDialog();
				return;
			}
			string allText = File.ReadAllText(files.tiles);

			// get map width and height
			int mapWidth, mapHeight;
			string[] lines = allText.Split('\n');
			mapWidth = lines[1].Split('$').Length - 1;
			string[] dbs = allText.Split(new string[] { ".db" }, StringSplitOptions.None);
			mapHeight = dbs.Length - 1;



			// convert raw tile text data into a very readable array
			string[] rawBytes = allText.Split('$');
			int[] bytes = new int[mapWidth * mapHeight];
			for (int i = 1; i < rawBytes.Length; i++)
			{
				bytes[i - 1] = int.Parse(rawBytes[i].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			}

			int[][] attrs;
			if ((bool)checkbox_meta.IsChecked)
			{
				mapWidth *= 2;
				mapHeight *= 2;

				// not too small right?
				if (mapWidth < 32 || mapHeight < 30)
				{
					Popup tooSmallPopup = new Popup("Error", "Map must be at least the size of one NES nametable!  (32 8x8 tiles wide, 30 8x8 tiles high)", "", "OK");
					tooSmallPopup.ShowDialog();
					return;
				}

				// draw metatiles
				int[][] metas = new int[mapHeight / 2][];
				attrs = new int[mapHeight / 2][];
				for (int y = 0; y < mapHeight / 2; y++)
				{
					metas[y] = new int[mapWidth / 2];
					attrs[y] = new int[mapWidth / 2];
					for (int x = 0; x < mapWidth / 2; x++)
					{
						int sel = bytes[(y * (mapWidth / 2)) + x];
						metas[y][x] = sel;
						if (sel >= 64)
							metas[y][x] = 0;
					}
				}
				windows.map.metas = metas;
			}
			else
			{
				// not too small right?
				if (mapWidth < 32 || mapHeight < 30)
				{
					Popup tooSmallPopup = new Popup("Error", "Map must be at least the size of one NES nametable!  (32 8x8 tiles wide, 30 8x8 tiles high)", "", "OK");
					tooSmallPopup.ShowDialog();
					return;
				}

				// draw map and init arrays
				int[][] tiles = new int[mapHeight][];
				for (int y = 0; y < mapHeight; y++)
				{
					tiles[y] = new int[mapWidth];
					for (int x = 0; x < mapWidth; x++)
					{
						int sel = bytes[(y * mapWidth) + x];
						tiles[y][x] = sel;
					}
				}
				windows.map.tiles = tiles;
			}

			//windows.map.attrs = attrs;
			windows.map.mapWidth = mapWidth;
			windows.map.mapHeight = mapHeight;
			windows.map.button_size_revert_Click(null, null);
			windows.map.UpdateZoom(0.5, 0.5);
		}
		private void ImportMapAttrs()
		{
			if (files.attrs == "")
				return;
			if (!File.Exists(files.attrs))
			{
				Popup noFilePopup = new Popup("File doesn't exist", $"File \"{files.attrs}\" not found!", "OK");
				noFilePopup.Owner = this;
				noFilePopup.ShowDialog();
				return;
			}
			string allText = File.ReadAllText(files.attrs);

			// get map width and height
			int mapAttrWidth, mapAttrHeight;
			string[] lines = allText.Split('\n');
			mapAttrWidth = (lines[1].Split('$').Length - 1) * 2;
			string[] dbs = allText.Split(new string[] { ".db" }, StringSplitOptions.None);
			int lineNum = (dbs.Length - 1);
			mapAttrHeight = (lineNum * 2) - ((lineNum * 2) / 16);

			if (mapAttrWidth != windows.map.mapWidth / 2 || mapAttrHeight != windows.map.mapHeight / 2)
			{
				Popup badSizePopup = new Popup("Error", "Attributes file must be of the same dimensions as the current map, and each byte must start with '$'.", "");
				badSizePopup.ShowDialog();
				return;
			}

			// convert raw tile text data into a very readable array
			string[] rawBytes = allText.Split('$');
			int[] bytes = new int[mapAttrWidth * mapAttrHeight];
			for (int i = 1; i < rawBytes.Length; i++)
			{
				bytes[i - 1] = int.Parse(rawBytes[i].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			}

			// draw map and init arrays
			int[][] attrs;
			attrs = new int[mapAttrHeight][];
			int cutCounter = 0;
			int cutoffs = 0; // for getting the right y-index into bytes[] in relation to the 8th line cutoff thing
			for (int y = 0; y < mapAttrHeight; y++)
			{
				attrs[y] = new int[mapAttrWidth];
				if (cutCounter != 7)
					attrs[y + 1] = new int[mapAttrWidth];
				for (int x = 0; x < mapAttrWidth; x += 2)
				{
					int fullByte = bytes[(((y / 2) + cutoffs) * (mapAttrWidth / 2)) + (x / 2)];
					attrs[y][x] = fullByte & 0b00000011;
					attrs[y][x + 1] = (fullByte & 0b00001100) >> 2;
					if (cutCounter != 7)
					{
						attrs[y + 1][x] = (fullByte & 0b00110000) >> 4;
						attrs[y + 1][x + 1] = (fullByte & 0b11000000) >> 6;
					}
				}
				cutCounter = (cutCounter + 1) % 8;
				if (cutCounter > 0) y++;
				else if (y % 2 == 0) cutoffs++;
			}

			windows.map.attrs = attrs;
		}
		private void ImportMetatiles()
		{
			if (files.metas == "")
				return;
			if (!File.Exists(files.metas))
			{
				Popup noFilePopup = new Popup("File doesn't exist", $"File \"{files.metas}\" not found!", "OK");
				noFilePopup.Owner = this;
				noFilePopup.ShowDialog();
				return;
			}
			string allText = File.ReadAllText(files.metas);
			string[] bytes = allText.Split('$').Skip(1).ToArray();

			windows.meta.metatiles = new int[8][][];
			for (int y = 0; y < 8; y++)
			{
				windows.meta.metatiles[y] = new int[8][];
				for (int x = 0; x < 8; x++)
				{
					windows.meta.metatiles[y][x] = new int[4];
					for (int i = 0; i < 4; i++)
					{
						windows.meta.metatiles[y][x][i] = int.Parse(bytes[(((y * 8) + x) * 4) + i].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
					}
				}
			}

			windows.meta.DrawMetatiles();
			checkbox_meta.IsChecked = true;
		}
		private void ImportWalls()
		{
			if (files.walls == "")
				return;
			if (!File.Exists(files.walls))
			{
				Popup noFilePopup = new Popup("File doesn't exist", $"File \"{files.walls}\" not found!", "OK");
				noFilePopup.Owner = this;
				noFilePopup.ShowDialog();
				return;
			}

			// check size
			string allText = File.ReadAllText(files.walls);
			string[] bytes = allText.Split('$').Skip(1).ToArray();
			if (bytes.Length != ((windows.map.mapWidth / 2) / 8) * (windows.map.mapHeight / 2))
			{
				Popup badSizePopup = new Popup("Error", "Walls file must be of the same dimensions as the current map, and each byte must start with '$'.", "");
				badSizePopup.ShowDialog();
				return;
			}

			windows.map.walls = new bool[windows.map.mapHeight / 2][];
			for (int y = 0; y < windows.map.mapHeight / 2; y++)
			{
				windows.map.walls[y] = new bool[windows.map.mapWidth / 2];
				for (int x = 0; x < windows.map.mapWidth / 2; x++)
				{
					int thisByte = int.Parse(bytes[(y * ((windows.map.mapWidth / 2) / 8)) + (x / 8)].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
					bool bit = (thisByte & (0b10000000 >> (x % 8))) != 0;
					windows.map.walls[y][x] = bit;
				}
			}
		}
		private void ImportObjs()
		{
			if (files.objs == "")
				return;
			if (!File.Exists(files.objs))
			{
				Popup noFilePopup = new Popup("File doesn't exist", $"File \"{files.objs}\" not found!", "OK");
				noFilePopup.Owner = this;
				noFilePopup.ShowDialog();
				return;
			}

			string allText = File.ReadAllText(files.objs);
			string[] lines = allText.Split('\n');

			// remove all but the player
			while (windows.objs.objs.Count > 1)
				windows.objs.objs.RemoveAt(1);

			short line = 0;
			while (lines.Length > line)
			{
				if (lines[line].Length > 0 && lines[line][0] == '.')
				{
					windows.objs.objs.Add(new Objekt());
					windows.objs.objs.Last().name = lines[line].Substring(1, lines[line].IndexOf(':') - 1);
					line++;

					// get number of times each state is listed
					windows.objs.objs.Last().states = new List<AnimState>();
					string lastName = "";
					string name = lines[line].Substring(lines[line].IndexOf(".db") + 3);
					List<byte> lsts = new List<byte>();
					bool loop = true;
					while (loop)
					{
						name = name.Substring(name.IndexOf('.') + 1);
						string shortName = name;
						if (name.Contains(','))
						{
							shortName = name.Substring(0, name.IndexOf(','));
						}
						else
							loop = false;
						if (shortName == lastName)
						{
							lsts[lsts.Count - 1]++;
						}
						else
						{
							lsts.Add(1);
							lastName = shortName;
						}
					}

					line++;

					// states
					int ind = 0;
					while (lines[line].Contains("\t") && lines[line].Contains(":"))
					{
						windows.objs.objs.Last().states.Add(new AnimState(windows.objs.objs.Last()));
						windows.objs.objs.Last().states.Last().name = lines[line].Substring(lines[line].LastIndexOf('_') + 1, lines[line].IndexOf(':') - lines[line].LastIndexOf('_') - 1);
						windows.objs.objs.Last().states.Last().lst = lsts[ind];
						ind++;
						line++;
						windows.objs.objs.Last().states.Last().width = byte.Parse(lines[line].Substring(6, 2));
						int width = byte.Parse(lines[line].Substring(6, 2));
						line++;
						windows.objs.objs.Last().states.Last().height = byte.Parse(lines[line].Substring(6, 2));
						int height = byte.Parse(lines[line].Substring(6, 2));
						line++;
						int numFrames = byte.Parse(lines[line].Substring(6, 2));
						line++;
						windows.objs.objs.Last().states.Last().speed = (byte)Array.IndexOf(windows.objs.MASK_LOOKUP, Convert.ToByte(lines[line].Substring(7, 8), 2));
						line++;
						line++;

						// frames
						windows.objs.objs.Last().states.Last().frames = new List<AnimFrame>();
						while (numFrames > 0)
						{
							windows.objs.objs.Last().states.Last().frames.Add(new AnimFrame());
							line++;
							string frameContent = "";
							while (lines.Length > line && lines[line].Contains(".db"))
							{
								frameContent += lines[line];
								line++;
							}
							string[] tileBytes = frameContent.Split('$').Skip(1).ToArray();
							string[] attrBytes = frameContent.Split('%').Skip(1).ToArray();
							windows.objs.objs.Last().states.Last().frames.Last().tiles = new int[height][];
							windows.objs.objs.Last().states.Last().frames.Last().attrs = new byte[height][];
							for (int y = 0; y < height; y++)
							{
								windows.objs.objs.Last().states.Last().frames.Last().tiles[y] = new int[width];
								windows.objs.objs.Last().states.Last().frames.Last().attrs[y] = new byte[width];
								for (int x = 0; x < width; x++)
								{
									windows.objs.objs.Last().states.Last().frames.Last().tiles[y][x] = int.Parse(tileBytes[0].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
									tileBytes = tileBytes.Skip(1).ToArray();
									windows.objs.objs.Last().states.Last().frames.Last().attrs[y][x] = Convert.ToByte(attrBytes[0].Substring(0, 8), 2);
									attrBytes = attrBytes.Skip(1).ToArray();
								}
							}
							numFrames--;
						}
					}
				}
				else if (lines.Length > 0 && lines[line] == "objDims:")
				{
					line++;
					line++; // over the empty 0, 0 one
					foreach (Objekt obj in windows.objs.objs)
					{
						if (obj.name == "player") continue;
						byte width = byte.Parse(lines[line].Split(',')[0].Substring(lines[line].Split(',')[0].Length - 1));
						byte height = byte.Parse(lines[line].Split(',')[1]); // must be a space (w, h)
						obj.width = width;
						obj.height = height;
						line++;
					}
				}
				else
					line++;
			}
		}
		private void ImportPlr()
		{
			if (files.plr == "")
				return;
			if (!File.Exists(files.plr))
			{
				Popup noFilePopup = new Popup("File doesn't exist", $"File \"{files.plr}\" not found!", "OK");
				noFilePopup.Owner = this;
				noFilePopup.ShowDialog();
				return;
			}

			string allText = File.ReadAllText(files.plr);
			string[] lines = allText.Split('\n');

			windows.objs.objs[0].states = new List<AnimState>();
			short line = 1;
			while (lines.Length > line)
			{
				// keep going down until a label
				if (!lines[line].Contains(":"))
                {
					line++;
					continue;
                }

				// states
				windows.objs.objs[0].states.Add(new AnimState(windows.objs.objs[0]));
				windows.objs.objs[0].states.Last().name = lines[line].Substring(lines[line].IndexOf('.') + 1, lines[line].IndexOf(':') - lines[line].IndexOf('.') - 1);
				line++;
				windows.objs.objs[0].states.Last().width = byte.Parse(lines[line].Substring(6, 2));
				int width = byte.Parse(lines[line].Substring(6, 2));
				line++;
				windows.objs.objs[0].states.Last().height = byte.Parse(lines[line].Substring(6, 2));
				int height = byte.Parse(lines[line].Substring(6, 2));
				line++;
				int numFrames = byte.Parse(lines[line].Substring(6, 2));
				line++;
				windows.objs.objs.Last().states.Last().speed = (byte)Array.IndexOf(windows.objs.MASK_LOOKUP, Convert.ToByte(lines[line].Substring(7, 8), 2));
				line++;
				line++;

				// frames
				windows.objs.objs[0].states.Last().frames = new List<AnimFrame>();
				while (numFrames > 0)
				{
					windows.objs.objs[0].states.Last().frames.Add(new AnimFrame());
					line++;
					string frameContent = "";
					while (lines.Length > line && lines[line].Contains(".db"))
					{
						if (!lines[line].Contains(";"))
							frameContent += lines[line];
						line++;
					}
					string[] tileBytes = frameContent.Split('$').Skip(1).ToArray();
					string[] attrBytes = frameContent.Split('%').Skip(1).ToArray();
					windows.objs.objs[0].states.Last().frames.Last().tiles = new int[height][];
					windows.objs.objs[0].states.Last().frames.Last().attrs = new byte[height][];
					for (int y = 0; y < height; y++)
					{
						windows.objs.objs[0].states.Last().frames.Last().tiles[y] = new int[width];
						windows.objs.objs[0].states.Last().frames.Last().attrs[y] = new byte[width];
						for (int x = 0; x < width; x++)
						{
							windows.objs.objs[0].states.Last().frames.Last().tiles[y][x] = int.Parse(tileBytes[0].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
							tileBytes = tileBytes.Skip(1).ToArray();
							windows.objs.objs[0].states.Last().frames.Last().attrs[y][x] = Convert.ToByte(attrBytes[0].Substring(0, 8), 2);
							attrBytes = attrBytes.Skip(1).ToArray();
						}
					}
					numFrames--;
				}
			}
		}

		// export
		private void ExportMapTiles()
		{
			if (files.tiles == "")
				return;
			if (!File.Exists(files.tiles))
			{
				Popup noFilePopup = new Popup("File doesn't exist", $"File \"{files.tiles}\" not found!", "OK");
				noFilePopup.Owner = this;
				noFilePopup.ShowDialog();
				return;
			}
			string output = "";

			if ((bool)checkbox_meta.IsChecked)
			{
				// create an array of visible i-objs, split up into 16x16 chunks, to place in the map array
				List<PlaceObj> visIobjs = new List<PlaceObj>();
				for (int i = 0; i < windows.map.iobjs.Count; i++)
				{
					PlaceObj iobj = windows.map.iobjs[i];
					if ((bool)((CheckBox)((StackPanel)windows.iobjs.panel_iobjs.Children[iobj.type]).Children[5]).IsChecked)
					{
						for (int y = 0; y < iobj.height; y++)
						{
							for (int x = 0; x < iobj.width; x++)
							{
								visIobjs.Add(new PlaceObj()
								{
									x = (byte)(iobj.x + x),
									y = (byte)(iobj.y + y),
									type = (byte)(64 + (i * 4))
								});
							}
						}
					}
				}

				for (int y = windows.map.tileYOffs / 2; y < (windows.map.mapHeight + windows.map.tileYOffs) / 2; y++)
				{
					output += "\t.db ";
					for (int x = windows.map.tileXOffs / 2; x < (windows.map.mapWidth + windows.map.tileXOffs) / 2; x++)
					{
						bool iobjHere = false;
						foreach (PlaceObj iobj in visIobjs)
						{
							if (iobj.x == x && iobj.y == y)
							{
								output += "$" + iobj.type.ToString("x2") + ", ";
								iobjHere = true;
							}
						}
						if (iobjHere)
							continue;
						output += "$" + windows.map.metas[y][x].ToString("x2") + ", ";
					}
					output = output.Substring(0, output.Length - 2); // chop off final ", "
					output += "\n";
				}
			}
			else
			{
				for (int y = windows.map.tileYOffs; y < windows.map.mapHeight + windows.map.tileYOffs; y++)
				{
					output += "\t.db ";
					for (int x = windows.map.tileXOffs; x < windows.map.mapWidth + windows.map.tileXOffs; x++)
					{
						output += "$" + windows.map.tiles[y][x].ToString("x2") + ", ";
					}
					output = output.Substring(0, output.Length - 2); // chop off final ", "
					output += "\n";
				}
			}

			File.WriteAllText(files.tiles, output);
		}
		private void ExportMapAttrs()
		{
			if (files.attrs == "")
				return;
			if (!File.Exists(files.attrs))
			{
				Popup noFilePopup = new Popup("File doesn't exist", $"File \"{files.attrs}\" not found!", "OK");
				noFilePopup.Owner = this;
				noFilePopup.ShowDialog();
				return;
			}
			string output = "";
			int cutCounter = 0; // for chopping off every 15th row, just like NES nametables do it

			for (int y = windows.map.tileYOffs / 2; y < (windows.map.tileYOffs / 2) + (windows.map.mapHeight / 2); y++)
			{
				output += "\t.db ";
				for (int x = windows.map.tileXOffs / 2; x < (windows.map.tileXOffs / 2) + (windows.map.mapWidth / 2); x += 2)
				{
					output += "$";
					int _byte =
						windows.map.attrs[y][x] +
						(windows.map.attrs[y][x + 1] << 2);
					if (cutCounter != 7)
					{
						_byte +=
							(windows.map.attrs[y + 1][x] << 4) +
							(windows.map.attrs[y + 1][x + 1] << 6);
					}
					output += _byte.ToString("x2") + ", ";
				}
				cutCounter = (cutCounter + 1) % 8;
				if (cutCounter > 0) y++;
				output = output.Substring(0, output.Length - 2); // chop off final ", "
				output += "\n";
			}

			File.WriteAllText(files.attrs, output);
		}
		private void ExportMetatiles()
		{
			if (files.metas == "")
				return;
			if (!File.Exists(files.metas))
			{
				Popup noFilePopup = new Popup("File doesn't exist", $"File \"{files.metas}\" not found!", "OK");
				noFilePopup.Owner = this;
				noFilePopup.ShowDialog();
				return;
			}
			string output = "";

			for (int y = 0; y < 8; y++)
			{
				output += "\t.db ";
				for (int x = 0; x < 8; x++)
				{
					for (int i = 0; i < 4; i++)
					{
						output += "$" + windows.meta.metatiles[y][x][i].ToString("x2") + ",";
					}
					output += " ";
				}
				output = output.Substring(0, output.Length - 2); // chop off final ", " of the line
				output += "\n";
			}

			File.WriteAllText(files.metas, output);
		}
		private void ExportWalls()
		{
			if (files.walls == "")
				return;
			if (!File.Exists(files.walls))
			{
				Popup noFilePopup = new Popup("File doesn't exist", $"File \"{files.walls}\" not found!", "OK");
				noFilePopup.Owner = this;
				noFilePopup.ShowDialog();
				return;
			}
			string output = "";

			for (int y = 0; y < windows.map.mapHeight / 2; y++)
			{
				output += "\t.db ";
				for (int x = 0; x < (windows.map.mapWidth / 2) / 8; x++)
				{
					output += "$";
					int currByte = 0;
					for (int lilX = 0; lilX < 8; lilX++)
					{
						if (windows.map.walls[y].Length > (x * 8) + lilX && windows.map.walls[y][(x * 8) + lilX])
							currByte += 0b10000000 >> lilX;
					}
					output += currByte.ToString("x2") + ", ";
				}
				output = output.Substring(0, output.Length - 2);
				output += "\n";
			}

			File.WriteAllText(files.walls, output);
		}
		private void ExportObjs()
		{
			if (files.objs == "")
				return;
			if (!File.Exists(files.objs))
			{
				Popup noFilePopup = new Popup("File doesn't exist", $"File \"{files.objs}\" not found!", "OK");
				noFilePopup.Owner = this;
				noFilePopup.ShowDialog();
				return;
			}

			if (windows.objs.objs.Count == 1)
			{
				Popup tooFewPopup = new Popup("Error", "There are no objects to export! (The player does not count as an object.)", "");
				tooFewPopup.ShowDialog();
				return;
			}

			// obj names
			string output = "\t.dw ";
			Objekt[] objs = windows.objs.objs.Skip(1).ToArray();
			foreach (Objekt obj in objs)
				output += "." + obj.name + ", ";
			output = output.Substring(0, output.Length - 2) + "\n"; // chop off final ", "

			// objs
			foreach (Objekt obj in objs)
			{
				output += "." + obj.name + ":\n\t.dw ";
				foreach (AnimState state in obj.states)
				{
					for (int i = 0; i < state.lst; i++)
						output += "." + obj.name + "_" + state.name + ", ";
				}
				output = output.Substring(0, output.Length - 2) + "\n"; // remove final ", "
				foreach (AnimState state in obj.states)
				{
					output += "\t." + obj.name + "_" + state.name + ":\n";
					output += "\t\t.db " + state.width.ToString() + " ; width\n";
					output += "\t\t.db " + state.height.ToString() + " ; height\n";
					output += "\t\t.db " + state.frames.Count.ToString() + " ; # frames\n";
					output += "\t\t.db %" + ToBinaryString(windows.objs.MASK_LOOKUP[state.speed]) + " ; animation mask for speed\n";
					output += "\t\t.dw ";
					for (int f = 0; f < state.frames.Count; f++)
						output += "." + obj.name + "_" + state.name + "_" + f + ", ";
					output = output.Substring(0, output.Length - 2) + "\n";
					for (int f = 0; f < state.frames.Count; f++)
					{
						output += "\t\t." + obj.name + "_" + state.name + "_" + f + ":\n";
						for (int y = 0; y < state.height; y++)
						{
							output += "\t\t\t.db ";
							for (int x = 0; x < state.width; x++)
							{
								output += "$" + (state.frames[f].tiles[y][x] % 256).ToString("x2") + ", ";
								output += "%" + ToBinaryString(state.frames[f].attrs[y][x]) + ", ";
							}
							output = output.Substring(0, output.Length - 2) + "\n";
						}
					}
				}
			}
			// then object data at the bottom
			output += "\n";
			for (int i = 0; i < objs.Length; i++)
				output += "OBJ_TYPE_" + objs[i].name.ToUpper() + " = " + (i + 1) + "\n";
			// dimensions of each object
			output += "\nobjDims:\n";
			output += "\t.db 0, 0 ; empty\n";
			foreach (Objekt obj in objs)
				output += "\t.db " + obj.width + ", " + obj.height + "\n";
			File.WriteAllText(files.objs, output);
		}
		private void ExportPlr()
		{
			if (files.plr == "")
				return;
			if (!File.Exists(files.plr))
			{
				Popup noFilePopup = new Popup("File doesn't exist", $"File \"{files.plr}\" not found!", "OK");
				noFilePopup.Owner = this;
				noFilePopup.ShowDialog();
				return;
			}

			string output = "";
			Objekt plr = windows.objs.objs[0];

			// output list of plr states
			for (int s = 0; s < plr.states.Count; s++)
				output += $"PLR_STATE_{plr.states[s].name.ToUpper()} = {s}\n";

			// output the start addresses of each animation
			output += "\n\t.dw ";
			foreach (AnimState state in plr.states)
				output += "." + state.name + ", ";

			// output animations
			output = output.Substring(0, output.Length - 2) + "\n";
			foreach (AnimState state in plr.states)
			{
				output += "\t." + state.name + ":\n";
				output += "\t\t.db " + state.width.ToString() + " ; width\n";
				output += "\t\t.db " + state.height.ToString() + " ; height\n";
				output += "\t\t.db " + state.frames.Count.ToString() + " ; # frames\n";
				output += "\t\t.db %" + ToBinaryString(windows.objs.MASK_LOOKUP[state.speed]) + " ; animation mask for speed\n";
				output += "\t\t.dw ";
				for (int f = 0; f < state.frames.Count; f++)
					output += "." + state.name + "_" + f + ", ";
				output = output.Substring(0, output.Length - 2) + "\n";
				for (int f = 0; f < state.frames.Count; f++)
				{
					output += "\t\t." + state.name + "_" + f + ":\n";
					for (int y = 0; y < state.height; y++)
					{
						output += "\t\t\t.db ";
						for (int x = 0; x < state.width; x++)
						{
							output += "$" + (state.frames[f].tiles[y][x] % 256).ToString("x2") + ", ";
							output += "%" + ToBinaryString(state.frames[f].attrs[y][x]) + ", ";
						}
						output = output.Substring(0, output.Length - 2) + "\n";
					}
				}
			}
			File.WriteAllText(files.plr, output);
		}
		private void ExportRoom()
		{
			if (files.room == "")
				return;
			if (!File.Exists(files.room))
			{
				Popup noFilePopup = new Popup("File doesn't exist", $"File \"{files.room}\" not found!", "OK");
				noFilePopup.Owner = this;
				noFilePopup.ShowDialog();
				return;
			}
			string output = "";

			// camera leads
			output += $"camLeads_{int.Parse(textbox_bank.Text) * 2}:\n";
			if (windows.map.camLeads.Count > 0)
			{
				output += "\t.dw ";
				for (int i = 0; i < windows.map.camLeads.Count; i++)
					output += ".leads" + i + ", ";
				output = output.Substring(0, output.Length - 2) + "\n";
				for (int i = 0; i < windows.map.camLeads.Count; i++)
				{
					output += "\t.leads" + i + ":\n";
					if (windows.map.camLeads[i].loc)
						output += "\t\t.db " + windows.map.camLeads[i].x + ", " + windows.map.camLeads[i].y + "\n";
					else
						output += "\t\t.db $ff, $ff\n";
					output += "\t\t.db " + windows.map.camLeads[i].act + "\n";
					output += $"\t\t.dw objs" + windows.map.camLeads[i].batch + $"_{int.Parse(textbox_bank.Text) * 2}\n";
				}
			}

			// i-objs
			output += $"iobjs_{int.Parse(textbox_bank.Text) * 2}:\n\t.db " + windows.map.iobjs.Count + "\n\n";
			foreach (PlaceObj iobj in windows.map.iobjs)
			{
				output += "\t.db iobj.TYPE_" + ((TextBox)((StackPanel)windows.iobjs.panel_iobjs.Children[iobj.type]).Children[1]).Text.ToUpper() + "\n";
				output += "\t.db " + iobj.x + ", " + iobj.y + ", " + iobj.width + ", " + iobj.height + "\n";
			}

			// obj batches
			for (int b = 0; b < windows.map.objs.Count; b++)
			{
				output += "objs" + b + $"_{int.Parse(textbox_bank.Text) * 2}:\n";
				output += "\t.db " + windows.map.objs[b].Count + "\n\n";
				foreach (PlaceObj obj in windows.map.objs[b])
				{
					output += "\t.db OBJ_TYPE_" + ((string)windows.objs.combobox_objects.Items[obj.type]).ToUpper() + "\n";
					output += "\t.db " + obj.x + ", " + obj.y + "\n";
				}

			}

			// walls, attr data
			output += $"walls_{int.Parse(textbox_bank.Text) * 2}:\n";
			output += "\t.include \"" + textbox_walls_data_appear.Text + "\"\n";
			output += $"attrs_{int.Parse(textbox_bank.Text) * 2}:\n";
			output += "\t.include \"" + textbox_mapattrs_data_appear.Text + "\"\n";

			// chr file (TEMP eventually there will be a whole bank for all game CHR and music will go here)
			output += $"chr_{int.Parse(textbox_bank.Text) * 2}:\n";
			output += $"\t.incbin \"{textbox_chr_data_appear.Text}.cut.cmp\"\n";

			// bank-specific code
			string codeStr = files.wag.Substring(files.wag.LastIndexOf("\\"));
			codeStr = codeStr.Substring(0, codeStr.Length - 4); // chop of .wag
			codeStr = codeStr.Insert(0, textbox_walls_data_appear.Text.Substring(0, textbox_walls_data_appear.Text.LastIndexOf("\\")));
			codeStr += "-code.asm";
            output += $"\t.include \"{codeStr}\"\n";


            ///////// HALF-BANK BREAK


            output += "\n\t; second half of 16kb bank\n";
			output += $"\t.bank {(int.Parse(textbox_bank.Text) * 2) + 1}\n";
			output += "\t.org $a000\n\n";


			// map width and height
			int mapWidth = windows.map.mapWidth < 512 ? windows.map.mapWidth : 0;
			output += "\t.db " + (mapWidth / 2) + ", " + (windows.map.mapHeight / 2) + "\n";
			// player position
			output += "\t.db " + (windows.map.plrX % 256) + ", " + (windows.map.plrX / 256) + ", " + (windows.map.plrY % 256) + ", " + (windows.map.plrY / 256) + "\n";
			// starting index into IOBJ_ADDR of slopes, ladders, everything else
			output += "\t.db " +
				(windows.map.numVisibles * 4) + ", " +
				((windows.map.numVisibles + windows.map.numSlopes) * 4) + ", " +
				((windows.map.numVisibles + windows.map.numSlopes + windows.map.numLadders) * 4) + "\n";
			// pointer lines always constant
			output +=
				// camleads first, always at $8007
				$"\t.dw iobjs_{int.Parse(textbox_bank.Text) * 2}\n" +
				$"\t.dw objs0_{int.Parse(textbox_bank.Text) * 2}\n" +
				$"\t.dw walls_{int.Parse(textbox_bank.Text) * 2}\n" +
				$"\t.dw attrs_{int.Parse(textbox_bank.Text) * 2}\n" +
				$"\t.dw chr_{int.Parse(textbox_bank.Text) * 2}\n";
			// palettes
			// background
			output += "\t;palettes:\n";
			output += "\t\t.db ";
			for (int outer = 0; outer < 4; outer++)
			{
				for (int inner = 0; inner < 4; inner++)
				{
					//output += "$" + windows.tilePicker.palettes[outer][inner].ToString("x2")
					string colString = windows.tilePicker.colors.First(col => col.Value == windows.tilePicker.bgPalettes[outer][inner]).Key;
					output += "$" + colString + ", ";
				}
				output += " ";
			}
			output = output.Substring(0, output.Length - 3) + "\n";
			// sprites
			output += "\t\t.db ";
			for (int outer = 0; outer < 4; outer++)
			{
				for (int inner = 0; inner < 4; inner++)
				{
					//output += "$" + windows.tilePicker.palettes[outer][inner].ToString("x2")
					string colString = windows.tilePicker.colors.First(col => col.Value == windows.tilePicker.sprPalettes[outer][inner]).Key;
					output += "$" + colString + ", ";
				}
				output += " ";
			}
			output = output.Substring(0, output.Length - 3) + "\n";
			// metatiles
			output += "\t.include \"" + textbox_metatiles_data_appear.Text + "\"\n";

			// map tiles
			output += $"\t.include \"{textbox_maptiles_data_appear.Text}\"\n";

			File.WriteAllText(files.room, output);
		}

		private byte[] SaveWAG()
		{
			List<byte> res = new List<byte>();

			// first byte: some bools telling which windows are open, if using metatiles
			res.Add(
				(byte)(
					(windows.tilePicker.IsVisible ? 0b1 : 0) |
					(windows.map.IsVisible ? 0b10 : 0) |
					(windows.map.WindowState == WindowState.Maximized ? 0b100 : 0) |
					(windows.meta.IsVisible ? 0b1000 : 0) |
					(windows.objs.IsVisible ? 0b10000 : 0) |
					(windows.iobjs.IsVisible ? 0b100000 : 0)
				)
			);

			// metatile window bools
			res.Add(
				(byte)(
					((bool)checkbox_meta.IsChecked ? 1 : 0) |
					((bool)windows.meta.radio_picking.IsChecked ? 0b10 : 0) |
					(Canvas.GetLeft(windows.meta.panel_settings) == 0 ? 0b100 : 0) |
					((bool)windows.meta.radio_2x.IsChecked ? 0b1000 : 0)
				)
			);

			// map bools
			res.Add(
				(byte)(
					((bool)windows.map.radio_tile.IsChecked ? 1 : 0) |
					((bool)windows.map.radio_palette.IsChecked ? 0b10 : 0) |
					((bool)windows.map.radio_walls.IsChecked ? 0b100 : 0) |
					((bool)windows.map.radio_objs.IsChecked ? 0b1000 : 0) |
					((bool)windows.map.radio_iobjs.IsChecked ? 0b10000 : 0) |
					// if all those are 0 then camleads mode is checked
					(Canvas.GetLeft(windows.map.panel_settings) == 0 ? 0b100000 : 0)
				)
			);

			// map bools pt. 2
			res.Add(
				(byte)(
					((bool)windows.map.checkbox_view_grid.IsChecked ? 0b1 : 0) |
					((bool)windows.map.checkbox_view_objs.IsChecked ? 0b10 : 0) |
					((bool)windows.map.checkbox_view_iobjs.IsChecked ? 0b100 : 0) |
					((bool)windows.map.checkbox_view_camleads.IsChecked ? 0b1000 : 0)
				)
			);

			// bank no
			res.Add(byte.Parse(textbox_bank.Text));

			// main window .Left and .Top
			res.AddRange(BitConverter.GetBytes(Left));
			res.AddRange(BitConverter.GetBytes(Top));

			// tile picker window .Left and .Top
			res.AddRange(BitConverter.GetBytes(windows.tilePicker.Left));
			res.AddRange(BitConverter.GetBytes(windows.tilePicker.Top));

			// map window .Left and .Top and dimensions
			res.AddRange(BitConverter.GetBytes(windows.map.Left));
			res.AddRange(BitConverter.GetBytes(windows.map.Top));
			res.AddRange(BitConverter.GetBytes(windows.map.Width));
			res.AddRange(BitConverter.GetBytes(windows.map.Height));

			// metatiles window .Left and .Top
			res.AddRange(BitConverter.GetBytes(windows.meta.Left));
			res.AddRange(BitConverter.GetBytes(windows.meta.Top));

			// objects window
			res.AddRange(BitConverter.GetBytes(windows.objs.Left));
			res.AddRange(BitConverter.GetBytes(windows.objs.Top));

			// i-objects window
			res.AddRange(BitConverter.GetBytes(windows.iobjs.Left));
			res.AddRange(BitConverter.GetBytes(windows.iobjs.Top));

			// files
			res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(files.wag).Length));
			res.AddRange(Encoding.Unicode.GetBytes(files.wag));
			res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(files.chr).Length));
			res.AddRange(Encoding.Unicode.GetBytes(files.chr));
			res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(files.pal).Length));
			res.AddRange(Encoding.Unicode.GetBytes(files.pal));
			res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(files.tiles).Length));
			res.AddRange(Encoding.Unicode.GetBytes(files.tiles));
			res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(files.attrs).Length));
			res.AddRange(Encoding.Unicode.GetBytes(files.attrs));
			res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(files.metas).Length));
			res.AddRange(Encoding.Unicode.GetBytes(files.metas));
			res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(files.walls).Length));
			res.AddRange(Encoding.Unicode.GetBytes(files.walls));
			res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(files.objs).Length));
			res.AddRange(Encoding.Unicode.GetBytes(files.objs));
			res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(files.plr).Length));
			res.AddRange(Encoding.Unicode.GetBytes(files.plr));
			res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(files.room).Length));
			res.AddRange(Encoding.Unicode.GetBytes(files.room));
			res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(textbox_chr_data_appear.Text).Length));
			res.AddRange(Encoding.Unicode.GetBytes(textbox_chr_data_appear.Text));
			res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(textbox_maptiles_data_appear.Text).Length));
			res.AddRange(Encoding.Unicode.GetBytes(textbox_maptiles_data_appear.Text));
			res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(textbox_mapattrs_data_appear.Text).Length));
			res.AddRange(Encoding.Unicode.GetBytes(textbox_mapattrs_data_appear.Text));
			res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(textbox_walls_data_appear.Text).Length));
			res.AddRange(Encoding.Unicode.GetBytes(textbox_walls_data_appear.Text));
			res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(textbox_metatiles_data_appear.Text).Length));
			res.AddRange(Encoding.Unicode.GetBytes(textbox_metatiles_data_appear.Text));

			// palettes (always the same size), palette indexes & selectedPal
			for (int outer = 0; outer < 4; outer++)
			{
				for (int inner = 0; inner < 4; inner++)
				{
					res.Add(windows.tilePicker.bgPalettes[outer][inner].R);
					res.Add(windows.tilePicker.bgPalettes[outer][inner].G);
					res.Add(windows.tilePicker.bgPalettes[outer][inner].B);
					res.Add(windows.tilePicker.sprPalettes[outer][inner].R);
					res.Add(windows.tilePicker.sprPalettes[outer][inner].G);
					res.Add(windows.tilePicker.sprPalettes[outer][inner].B);

					// indexes
					res.AddRange(Encoding.ASCII.GetBytes(windows.tilePicker.bgPaletteInds[outer][inner]));
					res.AddRange(Encoding.ASCII.GetBytes(windows.tilePicker.sprPaletteInds[outer][inner]));
				}
			}
			res.Add((byte)(windows.tilePicker.selectedPal & 0b111)); // 1, 2, 3 or 4 because I'm an idiot
			res.Add((byte)(windows.tilePicker.selectedSprPal & 0b111));

			// bmps, also always the same size
			for (int i = 0; i < 512; i++)
			{
				byte[] pixels = new byte[16];
				windows.tilePicker.bmps[i].CopyPixels(pixels, 2, 0);
				res.AddRange(pixels);
			}

			// metatiles, always the same size (for now)
			for (int y = 0; y < 8; y++)
			{
				for (int x = 0; x < 8; x++)
				{
					for (int i = 0; i < 4; i++)
					{
						res.AddRange(BitConverter.GetBytes(windows.meta.metatiles[y][x][i]));
					}
				}
			}

			// metaAttrs, always the same size (for now)
			for (int y = 0; y < 8; y++)
			{
				for (int x = 0; x < 8; x++)
				{
					res.AddRange(BitConverter.GetBytes(windows.meta.metaAttrs[y][x]));
				}
			}

			// attrs, variable size
			res.AddRange(BitConverter.GetBytes(windows.map.attrs[0].Length));
			res.AddRange(BitConverter.GetBytes(windows.map.attrs.Length));
			for (int y = 0; y < windows.map.attrs.Length; y++)
			{
				for (int x = 0; x < windows.map.attrs[y].Length; x++)
				{
					res.AddRange(BitConverter.GetBytes(windows.map.attrs[y][x]));
				}
			}

			// tiles, variable size; thus we store the size first
			res.AddRange(BitConverter.GetBytes(windows.map.tiles[0].Length));
			res.AddRange(BitConverter.GetBytes(windows.map.tiles.Length));
			for (int y = 0; y < windows.map.tiles.Length; y++)
			{
				for (int x = 0; x < windows.map.tiles[y].Length; x++)
				{
					res.AddRange(BitConverter.GetBytes(windows.map.tiles[y][x]));
				}
			}

			// metas, variable size
			res.AddRange(BitConverter.GetBytes(windows.map.metas[0].Length));
			res.AddRange(BitConverter.GetBytes(windows.map.metas.Length));
			for (int y = 0; y < windows.map.metas.Length; y++)
			{
				for (int x = 0; x < windows.map.metas[y].Length; x++)
				{
					res.AddRange(BitConverter.GetBytes(windows.map.metas[y][x]));
				}
			}

			// tileXOffset, tileYOffset, mapWidth, mapHeight
			res.AddRange(BitConverter.GetBytes(windows.map.tileXOffs));
			res.AddRange(BitConverter.GetBytes(windows.map.tileYOffs));
			res.AddRange(BitConverter.GetBytes(windows.map.mapWidth));
			res.AddRange(BitConverter.GetBytes(windows.map.mapHeight));

			// map zoom and translation
			res.AddRange(BitConverter.GetBytes(windows.map.zoom));
			res.AddRange(BitConverter.GetBytes(Canvas.GetLeft(windows.map.viewbox_map)));
			res.AddRange(BitConverter.GetBytes(Canvas.GetTop(windows.map.viewbox_map)));

			// walls, fixed to mapWidth / 2, mapHeight / 2
			for (int y = 0; y < windows.map.mapHeight / 2; y++)
			{
				for (int x = 0; x < (windows.map.mapWidth / 2) / 8; x++)
				{
					int currByte = 0;
					for (int lilX = 0; lilX < 8; lilX++)
					{
						if (windows.map.walls[y].Length > (x * 8) + lilX && windows.map.walls[y][(x * 8) + lilX])
							currByte += 0b10000000 >> lilX;
					}
					res.Add((byte)currByte);
				}
			}

			// objects
			res.AddRange(BitConverter.GetBytes(windows.objs.objs.Count));
			foreach (Objekt obj in windows.objs.objs)
			{
				res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(obj.name).Length));
				res.AddRange(Encoding.Unicode.GetBytes(obj.name));
				res.Add(obj.width);
				res.Add(obj.height);
				res.AddRange(BitConverter.GetBytes(obj.states.Count));
				foreach (AnimState state in obj.states)
				{
					res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(state.name).Length));
					res.AddRange(Encoding.Unicode.GetBytes(state.name));
					res.Add(state.width);
					res.Add(state.height);
					res.Add(state.speed);
					res.Add(state.lst);
					res.AddRange(BitConverter.GetBytes(state.frames.Count));
					foreach (AnimFrame frame in state.frames)
					{
						for (int y = 0; y < state.height; y++)
						{
							for (int x = 0; x < state.width; x++)
							{
								res.AddRange(BitConverter.GetBytes(frame.tiles[y][x]));
								res.Add(frame.attrs[y][x]);
							}
						}
					}
				}
			}
			res.AddRange(BitConverter.GetBytes(windows.objs.combobox_objects.SelectedIndex));
			res.AddRange(BitConverter.GetBytes(windows.objs.combobox_states.SelectedIndex));
			res.Add(byte.Parse(windows.objs.textbox_frame.Text));
			res.AddRange(BitConverter.GetBytes(Canvas.GetLeft(windows.objs.viewbox_obj)));
			res.AddRange(BitConverter.GetBytes(Canvas.GetTop(windows.objs.viewbox_obj)));
			res.AddRange(BitConverter.GetBytes(windows.objs.zoom));
			// map objs
			res.Add((byte)windows.map.objs.Count); // num batches
			for (int b = 0; b < windows.map.objs.Count; b++)
			{
				res.Add((byte)((bool)((CheckBox)windows.map.panel_view_obj_batches.Children[b]).IsChecked ? 0xff : 0));
				res.Add((byte)windows.map.objs[b].Count); // num objs in batch
				foreach (PlaceObj obj in windows.map.objs[b])
				{
					res.Add(obj.type);
					res.Add(obj.x);
					res.Add(obj.y);
				}
			}
			// player position
			res.AddRange(BitConverter.GetBytes(windows.map.plrX));
			res.AddRange(BitConverter.GetBytes(windows.map.plrY));

			// iobjs
			res.Add((byte)windows.iobjs.panel_iobjs.Children.Count);
			foreach (StackPanel panel in windows.iobjs.panel_iobjs.Children) {
				// name
				res.AddRange(BitConverter.GetBytes(Encoding.Unicode.GetBytes(((TextBox)panel.Children[1]).Text).Length));
				res.AddRange(Encoding.Unicode.GetBytes(((TextBox)panel.Children[1]).Text));
				// visible?
				res.Add((byte)((bool)((CheckBox)panel.Children[5]).IsChecked ? 0xff : 0));
			}
			// map iobjs
			res.Add((byte)windows.map.iobjs.Count);
			foreach (PlaceObj iobj in windows.map.iobjs)
			{
				res.Add(iobj.type);
				res.Add(iobj.x);
				res.Add(iobj.y);
				res.Add(iobj.width);
				res.Add(iobj.height);
			}

			// camera leads
			res.Add((byte)windows.map.camLeads.Count);
			foreach (CamLead lead in windows.map.camLeads)
			{
				res.Add(lead.x);
				res.Add(lead.y);
				res.Add(lead.batch);
				res.Add(lead.act);
				res.Add(lead.loc ? (byte)0xff : (byte)0);
			}

			changesMade = false;
			return res.ToArray();
		}
		private void OpenWAG(byte[] file)
		{
			Popup loadingPopup = new Popup("", "Opening...", "", "", false, false);
			loadingPopup.Show();

			int ind = 0;
			changesMade = false;
			undoHist = new UndoHistory(windows);

			// general bools
			if ((file[0] & 0b1) != 0)
				windows.tilePicker.Show();
			else
				windows.tilePicker.Hide();
			if ((file[0] & 0b10) != 0)
				windows.map.Show();
			else
				windows.map.Hide();
			if ((file[0] & 0b100) != 0)
				windows.map.WindowState = WindowState.Maximized;
			else
				windows.map.WindowState = WindowState.Normal;
			if ((file[0] & 0b1000) != 0)
				windows.meta.Show();
			else
				windows.meta.Hide();
			if ((file[0] & 0b10000) != 0)
				windows.objs.Show();
			else
				windows.objs.Hide();
			if ((file[0] & 0b100000) != 0)
				windows.iobjs.Show();
			else
				windows.iobjs.Hide();
			ind++;

			// metatile window bools
			if ((file[ind] & 0b1) != 0)
				checkbox_meta.IsChecked = true;
			else
				checkbox_meta.IsChecked = false;
			if ((file[ind] & 0b10) != 0)
				windows.meta.radio_picking.IsChecked = true;
			else
				windows.meta.radio_editing.IsChecked = true;
			if ((file[ind] & 0b100) == 0)
				Canvas.SetLeft(windows.meta.panel_settings, -256);
			else
				Canvas.SetLeft(windows.meta.panel_settings, 0);
			if ((file[ind] & 0b1000) != 0)
				windows.meta.radio_2x.IsChecked = true;
			else
				windows.meta.radio_4x.IsChecked = true;
			ind++;

			// map bools
			if ((file[ind] & 1) != 0)
				windows.map.radio_tile.IsChecked = true;
			else if ((file[ind] & 0b10) != 0)
				windows.map.radio_palette.IsChecked = true;
			else if ((file[ind] & 0b100) != 0)
				windows.map.radio_walls.IsChecked = true;
			else if ((file[ind] & 0b1000) != 0)
				windows.map.radio_objs.IsChecked = true;
			else if ((file[ind] & 0b10000) != 0)
				windows.map.radio_iobjs.IsChecked = true;
			else
				windows.map.radio_camleads.IsChecked = true;
			if ((file[ind] & 0b100000) == 0)
				Canvas.SetLeft(windows.map.panel_settings, -256); // NOTE: If I ever decide settings tab needs to be closed by default, this needs to be flipped!
			else
				Canvas.SetLeft(windows.map.panel_settings, 0);
			ind++;

			// map bools pt. 2
			if ((file[ind] & 1) != 0)
				windows.map.checkbox_view_grid.IsChecked = true;
			else
				windows.map.checkbox_view_grid.IsChecked = false;
			if ((file[ind] & 0b10) != 0)
				windows.map.checkbox_view_objs.IsChecked = true;
			else
				windows.map.checkbox_view_objs.IsChecked = false;
			if ((file[ind] & 0b100) != 0)
				windows.map.checkbox_view_iobjs.IsChecked = true;
			else
				windows.map.checkbox_view_iobjs.IsChecked = false;
			if ((file[ind] & 0b1000) != 0)
				windows.map.checkbox_view_camleads.IsChecked = true;
			else
				windows.map.checkbox_view_camleads.IsChecked = false;
			ind++;

			// bank no
			textbox_bank.Text = file[ind].ToString();
			ind++;

			// position windows
			Left = BitConverter.ToDouble(file, ind);
			ind += 8;
			Top = BitConverter.ToDouble(file, ind);
			ind += 8;
			windows.tilePicker.Left = BitConverter.ToDouble(file, ind);
			ind += 8;
			windows.tilePicker.Top = BitConverter.ToDouble(file, ind);
			ind += 8;
			windows.map.Left = BitConverter.ToDouble(file, ind);
			ind += 8;
			windows.map.Top = BitConverter.ToDouble(file, ind);
			ind += 8;
			windows.map.Width = BitConverter.ToDouble(file, ind);
			ind += 8;
			windows.map.Height = BitConverter.ToDouble(file, ind);
			ind += 8;
			windows.meta.Left = BitConverter.ToDouble(file, ind);
			ind += 8;
			windows.meta.Top = BitConverter.ToDouble(file, ind);
			ind += 8;
			windows.objs.Left = BitConverter.ToDouble(file, ind);
			ind += 8;
			windows.objs.Top = BitConverter.ToDouble(file, ind);
			ind += 8;
			windows.iobjs.Left = BitConverter.ToDouble(file, ind);
			ind += 8;
			windows.iobjs.Top = BitConverter.ToDouble(file, ind);
			ind += 8;

			// files
			// wag
			int len = BitConverter.ToInt32(file, ind);
			ind += 4;
			files.wag = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			ind += len;
			// chr
			len = BitConverter.ToInt32(file, ind);
			ind += 4;
			files.chr = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			textbox_chrfile.Text = FormatFileString(files.chr);
			if (files.chr != "") button_chr_im.IsEnabled = true;
			if (files.chr != "") button_chr_ex.IsEnabled = true;
			ind += len;
			// palettes
			len = BitConverter.ToInt32(file, ind);
			ind += 4;
			files.pal = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			//textbox_palfile.Text = FormatFileString(files.pal);
			//if (files.pal != "") button_pal_im.IsEnabled = true;
			//if (files.pal != "") button_pal_ex.IsEnabled = true;
			ind += len;
			// tiles
			len = BitConverter.ToInt32(file, ind);
			ind += 4;
			files.tiles = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			textbox_maptilesfile.Text = FormatFileString(files.tiles);
			if (files.tiles != "") button_maptiles_im.IsEnabled = true;
			if (files.tiles != "") button_maptiles_ex.IsEnabled = true;
			ind += len;
			// attrs
			len = BitConverter.ToInt32(file, ind);
			ind += 4;
			files.attrs = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			textbox_mapattrsfile.Text = FormatFileString(files.attrs);
			if (files.attrs != "") button_mapattrs_im.IsEnabled = true;
			if (files.attrs != "") button_mapattrs_ex.IsEnabled = true;
			ind += len;
			// metatiles
			len = BitConverter.ToInt32(file, ind);
			ind += 4;
			files.metas = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			textbox_metatilesfile.Text = FormatFileString(files.metas);
			if (files.metas != "") button_metatiles_im.IsEnabled = true;
			if (files.metas != "") button_metatiles_ex.IsEnabled = true;
			ind += len;
			// walls
			len = BitConverter.ToInt32(file, ind);
			ind += 4;
			files.walls = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			textbox_wallsfile.Text = FormatFileString(files.walls);
			if (files.walls != "") button_metatiles_im.IsEnabled = true;
			if (files.walls != "") button_metatiles_ex.IsEnabled = true;
			ind += len;
			// obj graphics
			len = BitConverter.ToInt32(file, ind);
			ind += 4;
			files.objs = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			textbox_objsfile.Text = FormatFileString(files.objs);
			if (files.objs != "") button_objs_im.IsEnabled = true;
			if (files.objs != "") button_objs_ex.IsEnabled = true;
			ind += len;
			// plr graphics
			len = BitConverter.ToInt32(file, ind);
			ind += 4;
			files.plr = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			textbox_plrfile.Text = FormatFileString(files.plr);
			if (files.plr != "") button_plr_im.IsEnabled = true;
			if (files.plr != "") button_plr_ex.IsEnabled = true;
			ind += len;
			// room data graphics
			len = BitConverter.ToInt32(file, ind);
			ind += 4;
			files.room = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			textbox_roomfile.Text = FormatFileString(files.room);
			if (files.room != "") button_room_im.IsEnabled = true;
			if (files.room != "") button_room_ex.IsEnabled = true;
			ind += len;
			// chr file used in room data
			len = BitConverter.ToInt32(file, ind);
			ind += 4;
			textbox_chr_data_appear.Text = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			ind += len;
			// map tiles file used in room data
			len = BitConverter.ToInt32(file, ind);
			ind += 4;
			textbox_maptiles_data_appear.Text = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			ind += len;
			// map attrs file used in room data
			len = BitConverter.ToInt32(file, ind);
			ind += 4;
			textbox_mapattrs_data_appear.Text = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			ind += len;
			// map walls file used in room data
			len = BitConverter.ToInt32(file, ind);
			ind += 4;
			textbox_walls_data_appear.Text = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			ind += len;
			// metatile walls file used in room data
			len = BitConverter.ToInt32(file, ind);
			ind += 4;
			textbox_metatiles_data_appear.Text = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			ind += len;

			// palettes & palette indexes
			for (int outer = 0; outer < 4; outer++)
			{
				for (int inner = 0; inner < 4; inner++)
				{
					windows.tilePicker.bgPalettes[outer][inner] = Color.FromRgb(
						file[ind], file[ind + 1], file[ind + 2]
					);
					ind += 3;
					windows.tilePicker.sprPalettes[outer][inner] = Color.FromRgb(
						file[ind], file[ind + 1], file[ind + 2]
					);
					ind += 3;

					// indexes
					windows.tilePicker.bgPaletteInds[outer][inner] = Encoding.ASCII.GetString(file.Skip(ind).Take(2).ToArray());
					ind += 2;
					windows.tilePicker.sprPaletteInds[outer][inner] = Encoding.ASCII.GetString(file.Skip(ind).Take(2).ToArray());
					ind += 2;
				}
			}
			windows.tilePicker.selectedPal = file[ind];
			ind++;
			windows.tilePicker.selectedSprPal = file[ind];
			ind++;

			// bmps
			for (int i = 0; i < 512; i++)
			{
				byte[] pixels = file.Skip(ind).Take(16).ToArray();
				windows.tilePicker.bmps[i] = BitmapSource.Create(8, 8, 2, 2, PixelFormats.Indexed2,
					new BitmapPalette(windows.tilePicker.bgPalettes[windows.tilePicker.selectedPal - 1]),
					pixels, 2);
				ind += 16;
			}

			// metatiles
			windows.meta.metatiles = new int[8][][];
			for (int y = 0; y < 8; y++)
			{
				windows.meta.metatiles[y] = new int[8][];
				for (int x = 0; x < 8; x++)
				{
					windows.meta.metatiles[y][x] = new int[4];
					for (int i = 0; i < 4; i++)
					{
						windows.meta.metatiles[y][x][i] = BitConverter.ToInt32(file, ind);
						ind += 4;
					}
				}
			}

			// metatile attributes
			windows.meta.metaAttrs = new int[8][];
			for (int y = 0; y < 8; y++)
			{
				windows.meta.metaAttrs[y] = new int[8];
				for (int x = 0; x < 8; x++)
				{
					windows.meta.metaAttrs[y][x] = BitConverter.ToInt32(file, ind);
					ind += 4;
				}
			}

			// attrs
			int attrsWidth = BitConverter.ToInt32(file, ind);
			ind += 4;
			int attrsHeight = BitConverter.ToInt32(file, ind);
			ind += 4;
			windows.map.attrs = new int[attrsHeight][];
			for (int y = 0; y < attrsHeight; y++)
			{
				windows.map.attrs[y] = new int[attrsWidth];
				for (int x = 0; x < attrsWidth; x++)
				{
					windows.map.attrs[y][x] = BitConverter.ToInt32(file, ind);
					ind += 4;
				}
			}

			// tiles
			int tilesWidth = BitConverter.ToInt32(file, ind);
			ind += 4;
			int tilesHeight = BitConverter.ToInt32(file, ind);
			ind += 4;
			windows.map.tiles = new int[tilesHeight][];
			for (int y = 0; y < tilesHeight; y++)
			{
				windows.map.tiles[y] = new int[tilesWidth];
				for (int x = 0; x < tilesWidth; x++)
				{
					windows.map.tiles[y][x] = BitConverter.ToInt32(file, ind);
					ind += 4;
				}
			}

			// metas
			int metasWidth = BitConverter.ToInt32(file, ind);
			ind += 4;
			int metasHeight = BitConverter.ToInt32(file, ind);
			ind += 4;
			windows.map.metas = new int[metasHeight][];
			for (int y = 0; y < metasHeight; y++)
			{
				windows.map.metas[y] = new int[metasWidth];
				for (int x = 0; x < metasWidth; x++)
				{
					windows.map.metas[y][x] = BitConverter.ToInt32(file, ind);
					ind += 4;
				}
			}

			// tileXOffset, tileYOffset, mapWidth, mapHeight
			windows.map.tileXOffs = BitConverter.ToInt32(file, ind);
			ind += 4;
			windows.map.tileYOffs = BitConverter.ToInt32(file, ind);
			ind += 4;
			windows.map.mapWidth = BitConverter.ToInt32(file, ind);
			windows.map.textbox_width.Text = (bool)checkbox_meta.IsChecked ? (windows.map.mapWidth / 2).ToString() : windows.map.mapWidth.ToString();
			ind += 4;
			windows.map.mapHeight = BitConverter.ToInt32(file, ind);
			windows.map.textbox_height.Text = (bool)checkbox_meta.IsChecked ? (windows.map.mapHeight / 2).ToString() : windows.map.mapHeight.ToString();
			ind += 4;

			// map zoom and translation
			windows.map.zoom = BitConverter.ToDouble(file, ind);
			ind += 8;
			windows.map.UpdateZoom(0.5, 0.5);
			Canvas.SetLeft(windows.map.viewbox_map, BitConverter.ToDouble(file, ind));
			ind += 8;
			Canvas.SetTop(windows.map.viewbox_map, BitConverter.ToDouble(file, ind));
			ind += 8;

			// walls, fixed to mapWidth / 2, mapHeight / 2
			windows.map.walls = new bool[windows.map.mapHeight / 2][];
			for (int y = 0; y < windows.map.mapHeight / 2; y++)
			{
				windows.map.walls[y] = new bool[windows.map.mapWidth / 2];
				for (int x = 0; x < (windows.map.mapWidth / 2) / 8; x++)
				{
					byte currByte = file[ind];
					ind++;
					for (int lilX = 0; lilX < 8; lilX++)
					{
						if ((currByte & (0b10000000 >> lilX)) != 0)
							windows.map.walls[y][(x * 8) + lilX] = true;
					}
				}
			}

			// objects
			int numObjs = BitConverter.ToInt32(file, ind);
			ind += 4;
			windows.objs.objs.Clear();
			for (int o = 0; o < numObjs; o++)
			{
				windows.objs.objs.Add(new Objekt());

				// name
				len = BitConverter.ToInt32(file, ind);
				ind += 4;
				windows.objs.objs[o].name = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
				ind += len;

				windows.objs.objs[o].width = file[ind];
				ind++;
				windows.objs.objs[o].height = file[ind];
				ind++;
				int numStates = BitConverter.ToInt32(file, ind);
				ind += 4;

				// states
				windows.objs.objs[o].states = new List<AnimState>();
				for (int s = 0; s < numStates; s++)
				{
					windows.objs.objs[o].states.Add(new AnimState(windows.objs.objs[o]));

					// name
					len = BitConverter.ToInt32(file, ind);
					ind += 4;
					windows.objs.objs[o].states[s].name = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
					ind += len;

					windows.objs.objs[o].states[s].width = file[ind];
					byte stateWidth = file[ind];
					ind++;
					windows.objs.objs[o].states[s].height = file[ind];
					byte stateHeight = file[ind];
					ind++;
					windows.objs.objs[o].states[s].speed = file[ind];
					ind++;
					windows.objs.objs[o].states[s].lst = file[ind];
					ind++;
					int numFrames = BitConverter.ToInt32(file, ind);
					ind += 4;
					windows.objs.objs[o].states[s].frames = new List<AnimFrame>();
					for (int f = 0; f < numFrames; f++)
					{
						windows.objs.objs[o].states[s].frames.Add(new AnimFrame());
						windows.objs.objs[o].states[s].frames[f].tiles = new int[stateHeight][];
						windows.objs.objs[o].states[s].frames[f].attrs = new byte[stateHeight][];
						for (byte fY = 0; fY < stateHeight; fY++)
						{
							windows.objs.objs[o].states[s].frames[f].tiles[fY] = new int[stateWidth];
							windows.objs.objs[o].states[s].frames[f].attrs[fY] = new byte[stateWidth];
							for (byte fX = 0; fX < stateWidth; fX++)
							{
								windows.objs.objs[o].states[s].frames[f].tiles[fY][fX] = BitConverter.ToInt32(file, ind);
								ind += 4;
								windows.objs.objs[o].states[s].frames[f].attrs[fY][fX] = file[ind];
								ind++;
							}
						}
					}
				}
			}

			// update visuals
			windows.tilePicker.InitPalettes();
			windows.tilePicker.UpdatePaletteCanvases();
			windows.tilePicker.LoadChr();
			windows.meta.DrawMetatiles();
			windows.map.InitMap();

			// render obj stuff
			windows.objs.UpdateObjects(BitConverter.ToInt32(file, ind));
			ind += 4;
			windows.objs.UpdateStates(BitConverter.ToInt32(file, ind));
			ind += 4;
			windows.objs.textbox_frame.Text = file[ind].ToString();
			ind++;
			windows.objs.RedrawObj();
			Canvas.SetLeft(windows.objs.viewbox_obj, BitConverter.ToDouble(file, ind));
			ind += 8;
			Canvas.SetTop(windows.objs.viewbox_obj, BitConverter.ToDouble(file, ind));
			ind += 8;
			windows.objs.zoom = BitConverter.ToDouble(file, ind);
			ind += 8;
			windows.objs.UpdateZoom(0, 0);

			// map objs
			windows.map.UpdateObjects(0);
			// clear all current ones
			windows.map.objs = new List<List<PlaceObj>>();
			((Grid)windows.map.grid_objs.Children[1]).Children.Clear();
			if (windows.map.grid_objs.Children.Count > 2)
				windows.map.grid_objs.Children.RemoveRange(2, windows.map.grid_objs.Children.Count - 2);
			windows.map.panel_view_obj_batches.Children.Clear();
			byte numBatches = file[ind];
			ind++;
			windows.map.textbox_num_batches.Text = "";
			windows.map.textbox_num_batches.Text = numBatches.ToString();
			for (byte b = 0; b < numBatches; b++)
			{
				((CheckBox)windows.map.panel_view_obj_batches.Children[b]).IsChecked = file[ind] != 0 ? true : false;
				ind++;
				byte Os = file[ind];
				ind++;
				windows.map.textbox_batch.Text = b.ToString();
				for (byte o = 0; o < Os; o++)
				{
					windows.map.combo_objs.SelectedIndex = file[ind];
					ind++;
					double x = file[ind] * 16.0;
					ind++;
					double y = file[ind] * 16.0;
					ind++;
					windows.map.DrawObj(x, y, true, false);
				}
			}
			// player position
			double plrX = BitConverter.ToInt32(file, ind);
			ind += 4;
			double plrY = BitConverter.ToInt32(file, ind);
			ind += 4;
			windows.map.combo_objs.SelectedIndex = 0;
			windows.map.DrawObj(plrX, plrY, true, false);

			// iobjs
			byte numIobjs = file[ind];
			ind++;
			// first one
			len = BitConverter.ToInt32(file, ind);
			ind += 4;
			((TextBox)((StackPanel)windows.iobjs.panel_iobjs.Children[0]).Children[1]).Text = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
			ind += len;
			if (file[ind] != 0)
				((CheckBox)((StackPanel)windows.iobjs.panel_iobjs.Children[0]).Children[5]).IsChecked = true;
			ind++;
			// clear all current ones after first one
			windows.iobjs.panel_iobjs.Children.RemoveRange(1, windows.iobjs.panel_iobjs.Children.Count - 1);
			for (byte i = 1; i < numIobjs; i++)
			{
				windows.iobjs.button_new_Click(null, null);
				// name
				len = BitConverter.ToInt32(file, ind);
				ind += 4;
				((TextBox)((StackPanel)windows.iobjs.panel_iobjs.Children[i]).Children[1]).Text = Encoding.Unicode.GetString(file.Skip(ind).Take(len).ToArray());
				ind += len;
				// visible or not
				if (file[ind] != 0)
					((CheckBox)((StackPanel)windows.iobjs.panel_iobjs.Children[i]).Children[5]).IsChecked = true;
				ind++;
			}
			windows.map.UpdateIObjects(0);
			// map iobjs
			numIobjs = file[ind];
			ind++;
			windows.map.iobjs = new List<PlaceObj>();
			// clear all current ones
			windows.map.grid_iobjs.Children.Clear();
			windows.map.numVisibles = 0;
			windows.map.numSlopes = 0;
			windows.map.numLadders = 0;
			for (int i = 0; i < numIobjs; i++)
			{
				windows.map.combo_iobjs.SelectedIndex = file[ind];
				ind++;
				byte x = file[ind];
				ind++;
				byte y = file[ind];
				ind++;
				windows.map.StartIObj(x * 16.0, y * 16.0, true, false, false);
				byte width = file[ind];
				ind++;
				byte height = file[ind];
				ind++;
				windows.map.SizeIObj((x + (width - 1)) * 16.0, (y + (height - 1)) * 16.0, false, false);
			}

			// camera leads
			byte numLeads = file[ind];
			ind++;
			windows.map.camLeads = new List<CamLead>();
			windows.map.grid_camleads.Children.Clear();
			for (int i = 0; i < numLeads; i++)
			{
				double x = file[ind];
				ind++;
				double y = file[ind];
				ind++;
				windows.map.textbox_batch.Text = file[ind].ToString();
				ind++;
				windows.map.textbox_cin_chain.Text = file[ind].ToString();
				ind++;
				windows.map.checkbox_cin_loc.IsChecked = file[ind] != 0 ? true : false;
				ind++;
				windows.map.DrawCamLead(x * 16, y * 16, true, false);
			}

			loadingPopup.Close();
			string wagDispName = files.wag;
			if (wagDispName.Contains("\\"))
				wagDispName = wagDispName.Substring(files.wag.LastIndexOf("\\") + 1);
			this.Title = $"Wagner - {wagDispName}";
			windows.map.label_title.Content = $"Map - {wagDispName}";
			windows.tilePicker.label_title.Content = $"Tile picker - {wagDispName}";
			windows.objs.label_title.Content = $"Objects - {wagDispName}";
			windows.iobjs.label_title.Content = $"I-objects - {wagDispName}";
			windows.meta.label_title.Content = $"Metatiles - {wagDispName}";
		}
    }
}

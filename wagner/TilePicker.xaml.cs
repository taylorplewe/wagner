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
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;

namespace Wagner
{
	/// <summary>
	/// Interaction logic for TilePicker.xaml
	/// </summary>
	public partial class TilePicker : Window
	{
		const double SHADE_OPACITY = 0.5;

		private WindowSet windows;
		private bool palEditing = false;
		private bool bgSelected = true; // false if the last palette clicked was a sprite one
		private int[] selectedPalCol = { -1, -1 };
		private int[] selectedSprPalCol = { -1, -1 };
		private BitmapPalette currPal;
		private Storyboard storyboard;

		public int selTile1 = 0;
		public int selTile2 = 0;
		public BitmapSource[] bmps;
		public Color[][] bgPalettes;
		public Color[][] sprPalettes;
		public string[][] bgPaletteInds;
		public string[][] sprPaletteInds;
		public int selectedPal = 1;
		public int selectedSprPal = 1;
		public SortedDictionary<string, Color> colors;

		public TilePicker(WindowSet windows)
		{
			InitializeComponent();

			this.windows = windows;

			storyboard = new Storyboard();
			bmps = new BitmapSource[512];

			UpdateColorPicker(windows.main.DEF_COL_MODE);
			InitPalettes();
			UpdatePaletteCanvases();
			LoadChr();
		}

		// helper functions
		public void InitPalettes()
		{
			if (bgPalettes == null)
			{
				bgPalettes = new Color[4][]
				{
					new Color[4] {
						colors["0f"],
						colors["17"],
						colors["28"],
						colors["39"]
					},
					new Color[4] {
						colors["0f"],
						colors["1c"],
						colors["2b"],
						colors["39"]
					},
					new Color[4] {
						colors["0f"],
						colors["30"],
						colors["26"],
						colors["05"]
					},
					new Color[4] {
						colors["0f"],
						colors["06"],
						colors["15"],
						colors["36"]
					}
				};
				sprPalettes = new Color[4][]
				{
					new Color[4] {
						colors["0f"],
						colors["20"],
						colors["10"],
						colors["00"]
					},
					new Color[4] {
						colors["0f"],
						colors["05"],
						colors["26"],
						colors["30"]
					},
					new Color[4] {
						colors["0f"],
						colors["13"],
						colors["23"],
						colors["33"]
					},
					new Color[4] {
						colors["0f"],
						colors["16"],
						colors["27"],
						colors["18"]
					}
				};

				// set indexes too
				bgPaletteInds = new string[4][]
				{
					new string[4] {
						"0f",
						"17",
						"28",
						"39"
					},
					new string[4] {
						"0f",
						"1c",
						"2b",
						"39"
					},
					new string[4] {
						"0f",
						"30",
						"26",
						"05"
					},
					new string[4] {
						"0f",
						"06",
						"15",
						"36"
					}
				};
				sprPaletteInds = new string[4][]
				{
					new string[4] {
						"0f",
						"20",
						"10",
						"00"
					},
					new string[4] {
						"0f",
						"05",
						"26",
						"30"
					},
					new string[4] {
						"0f",
						"13",
						"23",
						"33"
					},
					new string[4] {
						"0f",
						"16",
						"27",
						"18"
					}
				};
			}

			currPal = new BitmapPalette(bgPalettes[0]);
		}
		public void UpdatePalette()
        {
			if (windows.map.IsVisible && windows.map.radio_palette.IsEnabled)
			{
				windows.map.radio_palette.IsChecked = true;
				windows.map.UpdateDrawingBox();
			}
			if (windows.meta.IsVisible && windows.meta.radio_palette.IsEnabled)
				windows.meta.radio_palette.IsChecked = true;
			if (windows.objs.IsVisible)
				windows.objs.radio_palette.IsChecked = true;
			UpdatePaletteCanvases();
			if (img_chr.Source != null)
				UpdateTileColors();
		}
		public void UpdatePalette(int pal, bool bg)
		{
			if (bg) selectedPal = pal;
			else selectedSprPal = pal;
			bgSelected = bg;
			if (windows.map.IsVisible && windows.map.radio_palette.IsEnabled)
			{
				windows.map.radio_palette.IsChecked = true;
				windows.map.UpdateDrawingBox();
			}
			if (windows.meta.IsVisible && windows.meta.radio_palette.IsEnabled)
				windows.meta.radio_palette.IsChecked = true;
			if (windows.objs.IsVisible)
				windows.objs.radio_palette.IsChecked = true;
			UpdatePaletteCanvases();
			if (img_chr.Source != null)
				UpdateTileColors();
		}
		public void UpdatePaletteCanvases()
		{
			for (int o = 1; o <= 4; o++)
			{
				for (int i = 1; i <= 4; i++)
				{
					bgPalettes[o - 1][i - 1] = colors[bgPaletteInds[o - 1][i - 1]];
					sprPalettes[o - 1][i - 1] = colors[sprPaletteInds[o - 1][i - 1]];
					((Rectangle)FindName("rect_pal_" + o + "_" + i)).Fill = new SolidColorBrush(bgPalettes[o - 1][i - 1]);
					((Rectangle)FindName("rect_spr_pal_" + o + "_" + i)).Fill = new SolidColorBrush(sprPalettes[o - 1][i - 1]);
				}
				((Rectangle)FindName("rect_pal_" + o + "_sel")).Visibility = selectedPal == o && !palEditing ? Visibility.Visible : Visibility.Hidden;
				BringToFront((Rectangle)FindName("rect_pal_" + o + "_sel"));
				((Rectangle)FindName("rect_spr_pal_" + o + "_sel")).Visibility = selectedSprPal == o && !palEditing ? Visibility.Visible : Visibility.Hidden;
				BringToFront((Rectangle)FindName("rect_spr_pal_" + o + "_sel"));
			}
		}
		void UpdateTileColors()
		{
			currPal = bgSelected ? new BitmapPalette(bgPalettes[selectedPal - 1]) : new BitmapPalette(sprPalettes[selectedSprPal - 1]);
			DrawingGroup dGroup = new DrawingGroup();
			for (int i = 0; i < bmps.Length; i++)
			{
				byte[] bytes = new byte[16];
				bmps[i].CopyPixels(bytes, 2, 0);
				bmps[i] = BitmapSource.Create(8, 8, 2, 2, PixelFormats.Indexed2, currPal, bytes, 2);

				ImageDrawing bmpImg = new ImageDrawing(bmps[i], new Rect((i % 16) * 8, (i / 16) * 8, 8, 8));
				dGroup.Children.Add(bmpImg);
			}

			DrawingImage dImg = new DrawingImage(dGroup);
			dImg.Freeze();
			img_chr.Source = dImg;
		}
		public void LoadChr()
		{
			DrawingGroup dGroup = new DrawingGroup();

			for (int i = 0; i < 512; i++)
			{
				if (bmps[i] == null) bmps[i] = BitmapSource.Create(8, 8, 2, 2, PixelFormats.Indexed2, currPal, new byte[16], 2);
				ImageDrawing bmpImg = new ImageDrawing(bmps[i], new Rect((i % 16) * 8, (i / 16) * 8, 8, 8));
				dGroup.Children.Add(bmpImg);
			}

			DrawingImage dImg = new DrawingImage(dGroup);
			dImg.Freeze();
			img_chr.Source = dImg;

			// appear selected squares
			rect_chr_sel1.Visibility = Visibility.Visible;
			rect_chr_sel2.Visibility = Visibility.Visible;
		}
		void BringToFront(object sender)
		{
			DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)sender);
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
			{
				Panel.SetZIndex((UIElement)VisualTreeHelper.GetChild(parent, i), 0);
			}
			Panel.SetZIndex((UIElement)sender, 1);
		}
		public void UpdateColorPicker(string variant)
		{
			string colorMapText = Properties.Resources.colmap_wiki;
			// initialize colors object
			switch (variant)
			{
				case "wiki":
					img_colors.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/nes-palette-wiki.png"));
					label_color_mode.Content = "Wikipedia";
					colorMapText = Properties.Resources.colmap_wiki;
					break;
				case "mesen":
					img_colors.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/nes-palette-mesen.png"));
					label_color_mode.Content = "Mesen";
					colorMapText = Properties.Resources.colmap_mesen;
					break;
				case "ase":
					img_colors.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/nes-palette-ase.png"));
					label_color_mode.Content = "Aseprite";
					colorMapText = Properties.Resources.colmap_ase;
					break;
				case "theo":
					img_colors.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/nes-palette-theo.png"));
					label_color_mode.Content = "Theoretical";
					colorMapText = Properties.Resources.colmap_theo;
					break;
				case "yychr":
					img_colors.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/nes-palette-yychr.png"));
					label_color_mode.Content = "yy-chr";
					colorMapText = Properties.Resources.colmap_yychr;
					break;
				case "fceux":
					img_colors.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/nes-palette-fceux.png"));
					label_color_mode.Content = "FCEUX";
					colorMapText = Properties.Resources.colmap_fceux;
					break;
				default:
					break;
			}
            if (windows.map != null) windows.map.label_color_mode.Content = label_color_mode.Content;
            img_colors.Width = 512; // my computer's zoom is set to 125% and it messes with the sizing of this image
			colors = new SortedDictionary<string, Color>();
			string[] colorMap = colorMapText.Split('\n');
			foreach (string line in colorMap)
			{
				string key = line.Substring(0, 2);
				string[] vals = line.Substring(3).Split(',');
				Color col = Color.FromRgb(byte.Parse(vals[0]), byte.Parse(vals[1]), byte.Parse(vals[2]));
				colors.Add(key, col);
			}
		}
		private void RedrawAll()
		{
			windows.map.UpdatePalette();
			windows.meta.DrawMetatiles();
			windows.objs.RedrawObj();
			windows.map.RedrawObjs();
		}

		// window
		private void button_close_Click(object sender, RoutedEventArgs e)
		{
			Hide();
		}
		private void window_tile_picker_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && grid_top_bar.IsMouseOver)
				DragMove();
		}
		private void window_tile_picker_KeyDown(object sender, KeyEventArgs e)
		{
			if (img_chr.Source != null)
			{
				if (e.Key == Key.W)
					TileCursorUp();
				else if (e.Key == Key.A)
					TileCursorLeft();
				else if (e.Key == Key.S)
					TileCursorDown();
				else if (e.Key == Key.D)
					TileCursorRight();
			}
			if (e.Key == Key.I)
				rect_isolate_color.Visibility = Visibility.Visible;
			if (e.Key == Key.Escape && palEditing)
			{
				palEditing = false;
				if (selectedPalCol[0] != -1)
					((Canvas)panel_palettes.Children[selectedPalCol[0]]).Children.RemoveAt(0);
				else if (selectedSprPalCol[0] != -1)
					((Canvas)panel_palettes.Children[selectedSprPalCol[0] + 4]).Children.RemoveAt(0);
				selectedPalCol[0] = -1;
				selectedPalCol[1] = -1;
				selectedSprPalCol[0] = -1;
				selectedSprPalCol[1] = -1;
				UpdatePaletteCanvases();
			}
		}
		private void window_tile_picker_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.I)
				rect_isolate_color.Visibility = Visibility.Collapsed;
		}
		private void window_tile_picker_Activated(object sender, EventArgs e)
		{
			windows.meta.radio_editing.IsChecked = true;
		}

		public void TileCursorUp()
		{
			if (selTile1 < 16) return;
			selTile1 -= 16;
			Canvas.SetTop(rect_chr_sel1, Canvas.GetTop(rect_chr_sel1) - 32);
			windows.map.UpdateDrawingBox();
		}
		public void TileCursorDown()
		{
			if (selTile1 >= 496) return;
			selTile1 += 16;
			Canvas.SetTop(rect_chr_sel1, Canvas.GetTop(rect_chr_sel1) + 32);
			windows.map.UpdateDrawingBox();
		}
		public void TileCursorLeft()
		{
			if (selTile1 % 16 == 0) return;
			selTile1--;
			Canvas.SetLeft(rect_chr_sel1, Canvas.GetLeft(rect_chr_sel1) - 32);
			windows.map.UpdateDrawingBox();
		}
		public void TileCursorRight()
		{
			if (selTile1 % 16 == 15) return;
			selTile1++;
			Canvas.SetLeft(rect_chr_sel1, Canvas.GetLeft(rect_chr_sel1) + 32);
			windows.map.UpdateDrawingBox();
		}

		// palettes
		private void button_pal_edit_Click(object sender, RoutedEventArgs e)
		{
			palEditing = true;
			UpdatePaletteCanvases();
		}
		private void PaletteMouseEnter(object sender, MouseEventArgs e)
		{
			if (
				(selectedPalCol[0] == -1 && selectedSprPalCol[0] == -1) &&
				(
					(!palEditing && sender is Canvas) ||
					(palEditing && sender is Rectangle))
				)
			{
				BringToFront(sender);
				((UIElement)sender).Effect = new DropShadowEffect()
				{
					Color = Colors.White,
					BlurRadius = 20,
					Opacity = 0.5,
					ShadowDepth = 0
				};
			}
		}
		private void PaletteMouseLeave(object sender, MouseEventArgs e)
		{
			((UIElement)sender).Effect = null;
		}
		private void PaletteColClick(object sender, MouseButtonEventArgs e)
		{
			if (palEditing)
			{
				Rectangle rect = new Rectangle()
				{
					Width = 64,
					Height = 16,
					Stroke = new SolidColorBrush(Colors.White),
					StrokeThickness = 2,
					Name = "rect_color_select"
				};
				Canvas.SetTop(rect, Canvas.GetTop((Rectangle)sender));
				((Canvas)VisualTreeHelper.GetParent((DependencyObject)sender)).Children.Insert(0, rect);
				BringToFront(rect);
				((UIElement)sender).Effect = null;
				string name = ((FrameworkElement)sender).Name;
				if (!name.Contains("spr"))
				{
					selectedPalCol[0] = int.Parse(name.Substring(name.Length - 3, 1)) - 1;
					selectedPalCol[1] = int.Parse(name.Substring(name.Length - 1, 1)) - 1;
				}
				else
				{
					selectedSprPalCol[0] = int.Parse(name.Substring(name.Length - 3, 1)) - 1;
					selectedSprPalCol[1] = int.Parse(name.Substring(name.Length - 1, 1)) - 1;
				}
			}
		}
		private void canvas_palette_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!palEditing)
			{
				string name = ((FrameworkElement)sender).Name;
				UpdatePalette(int.Parse(name.Substring(name.Length - 1)), !name.Contains("spr"));
			}
		}
		private void canvas_color_picker_MouseMove(object sender, MouseEventArgs e)
		{
			string loc = "";
			int y = (int)e.GetPosition(canvas_color_picker).Y / 32;
			int x = (int)e.GetPosition(canvas_color_picker).X / 32;
			loc = y.ToString("X") + x.ToString("X");
			Canvas.SetLeft(grid_color_picker_hex, x * 32);
			Canvas.SetTop(grid_color_picker_hex, y * 32);
			label_color_picker_hex.Content = loc;
			label_color_picker_hex.Visibility = Visibility.Visible;
			rect_isolate_color.Fill = new SolidColorBrush(colors[loc.ToLower()]);

			// make text black on certain squares
			if ((y == 3 && x < 14) || (x == 0 && y > 0))
				label_color_picker_hex.Foreground = new SolidColorBrush(Colors.Black);
			else
				label_color_picker_hex.Foreground = new SolidColorBrush(Colors.White);

			// white square
			rect_color_picker_sel.Visibility = Visibility.Visible;
			Canvas.SetLeft(rect_color_picker_sel, (int)(e.GetPosition(canvas_color_picker).X / 32) * 32);
			Canvas.SetTop(rect_color_picker_sel, (int)(e.GetPosition(canvas_color_picker).Y / 32) * 32);
		}
		private void canvas_color_picker_MouseLeave(object sender, MouseEventArgs e)
		{
			//label_color_picker_loc.Content = "";
			rect_color_picker_sel.Visibility = Visibility.Hidden;
			label_color_picker_hex.Visibility = Visibility.Hidden;
		}
		private void canvas_color_picker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (selectedPalCol[0] != -1 || selectedSprPalCol[0] != -1)
			{
				string loc;
				int y = (int)e.GetPosition(canvas_color_picker).Y / 32;
				int x = (int)e.GetPosition(canvas_color_picker).X / 32;
				loc = y.ToString("X") + x.ToString("X");

				// if the first color i.e. the background color, change all of them
				if (selectedPalCol[1] == 0 || selectedSprPalCol[1] == 0)
				{
					for (int o = 0; o < 4; o++)
					{
						bgPalettes[o][0] = colors[loc.ToLower()];
						sprPalettes[o][0] = colors[loc.ToLower()];
						bgPaletteInds[o][0] = loc.ToLower();
						sprPaletteInds[o][0] = loc.ToLower();
					}
				}

				palEditing = false;
				windows.main.ChangesMade();
				if (selectedPalCol[0] != -1)
				{
					bgPalettes[selectedPalCol[0]][selectedPalCol[1]] = colors[loc.ToLower()];
					bgPaletteInds[selectedPalCol[0]][selectedPalCol[1]] = loc.ToLower();
					((Canvas)panel_palettes.Children[selectedPalCol[0]]).Children.RemoveAt(0);
					UpdatePalette(selectedPalCol[0] + 1, true);
				}
				else
				{
					sprPalettes[selectedSprPalCol[0]][selectedSprPalCol[1]] = colors[loc.ToLower()];
					sprPaletteInds[selectedSprPalCol[0]][selectedSprPalCol[1]] = loc.ToLower();
					((Canvas)panel_palettes.Children[selectedSprPalCol[0] + 4]).Children.RemoveAt(0);
					UpdatePalette(selectedSprPalCol[0] + 1, false);
				}

				RedrawAll();
				selectedPalCol[0] = -1;
				selectedPalCol[1] = -1;
				selectedSprPalCol[0] = -1;
				selectedSprPalCol[1] = -1;
			}
		}

		// tiles
		private void canvas_chr_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (img_chr.Source != null)
			{
				if (e.Delta < 0)
					Canvas.SetTop(canvas_chr_scroll,
						Math.Max(Canvas.GetTop(canvas_chr_scroll) - 32, -512));
				else
					Canvas.SetTop(canvas_chr_scroll,
						Math.Min(Canvas.GetTop(canvas_chr_scroll) + 32, 0));
			}
		}
		private void canvas_chr_MouseMove(object sender, MouseEventArgs e)
		{
			if (img_chr.Source != null)
			{
				int xPos = (int)e.GetPosition(canvas_chr).X / 32;
				int yPos = ((int)e.GetPosition(canvas_chr).Y - (int)Canvas.GetTop(canvas_chr_scroll)) / 32;
				Canvas.SetLeft(img_chr_sel, xPos * 32);
				Canvas.SetTop(img_chr_sel, yPos * 32);
				Canvas.SetLeft(rect_chr_hover, xPos * 32);
				Canvas.SetTop(rect_chr_hover, yPos * 32);
				img_chr_sel.Source = bmps[(yPos * 16) + xPos];
			}
		}
		private void canvas_chr_MouseLeave(object sender, MouseEventArgs e)
		{
			if (img_chr.Source != null)
			{
				img_chr_sel.Source = null;
				rect_chr_hover.Visibility = Visibility.Hidden;

				// vanish shade
				storyboard.Remove();
				storyboard.Children.Add(new DoubleAnimation()
				{
					From = SHADE_OPACITY,
					To = 0,
					Duration = new Duration(TimeSpan.FromSeconds(0.3))
				});
				Storyboard.SetTarget(storyboard, rect_chr_shade);
				Storyboard.SetTargetProperty(storyboard, new PropertyPath(OpacityProperty));
				storyboard.Begin();
			}
		}
		private void canvas_chr_MouseEnter(object sender, MouseEventArgs e)
		{
			if (img_chr.Source != null)
			{
				rect_chr_hover.Visibility = Visibility.Visible;

				// appear shade
				storyboard.Remove();
				storyboard.Children.Add(new DoubleAnimation()
				{
					From = 0,
					To = SHADE_OPACITY,
					Duration = new Duration(TimeSpan.FromSeconds(0.3))
				});
				Storyboard.SetTarget(storyboard, rect_chr_shade);
				Storyboard.SetTargetProperty(storyboard, new PropertyPath(OpacityProperty));
				storyboard.Begin();
			}
		}
		private void canvas_chr_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (img_chr.Source != null)
			{
				int xPos = (int)e.GetPosition(canvas_chr).X / 32;
				int yPos = ((int)e.GetPosition(canvas_chr).Y - (int)Canvas.GetTop(canvas_chr_scroll)) / 32;
				Canvas.SetLeft(rect_chr_sel1, xPos * 32);
				Canvas.SetTop(rect_chr_sel1, yPos * 32);
				selTile1 = (yPos * 16) + xPos;
				if (windows.map.IsVisible && !(bool)windows.main.checkbox_meta.IsChecked)
				{
					windows.map.radio_tile.IsChecked = true;
					windows.map.UpdateDrawingBox();
				}
				if (windows.meta.IsVisible && windows.meta.radio_tile.IsEnabled)
					windows.meta.radio_tile.IsChecked = true;
				if (windows.objs.IsVisible)
					windows.objs.radio_tile.IsChecked = true;
			}
		}
		private void canvas_chr_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (img_chr.Source != null)
			{
				int xPos = (int)e.GetPosition(canvas_chr).X / 32;
				int yPos = ((int)e.GetPosition(canvas_chr).Y - (int)Canvas.GetTop(canvas_chr_scroll)) / 32;
				Canvas.SetLeft(rect_chr_sel2, xPos * 32);
				Canvas.SetTop(rect_chr_sel2, yPos * 32);
				selTile2 = (yPos * 16) + xPos;
			}
		}
	}
}

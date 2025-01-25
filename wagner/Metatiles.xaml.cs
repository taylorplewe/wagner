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
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Wagner
{
	/// <summary>
	/// Interaction logic for Metatiles.xaml
	/// </summary>
	public partial class Metatiles : Window
	{
		private const double SHADE_OPACITY = 0.5;

		private WindowSet windows;
		private FileSet files;
		private Storyboard board;
		private Storyboard tabBoard;
		private double zoom = 4.0;

		// backbone of this file
		public int[][][] metatiles; // [y] [x] [tl, tr, bl, br]
		public int[][] metaAttrs;
		public BitmapSource[] metaBmps; // 16x16 metatile bitmaps
		public int selMeta1 = 0;
		public int selMeta2 = 0;

		public Metatiles(WindowSet windows, FileSet files)
		{
			InitializeComponent();

			this.windows = windows;
			this.files = files;

			board = new Storyboard();
			tabBoard = new Storyboard();

			DrawMetatiles();
		}
		public void DrawMetatiles()
		{
			// initialize metatile array if it hasn't been
			if (metatiles == null)
			{
				metatiles = new int[8][][];
				for (int y = 0; y < 8; y++)
				{
					metatiles[y] = new int[8][];
					for (int x = 0; x < 8; x++)
					{
						metatiles[y][x] = new int[4];
					}
				}
			}
			if (metaAttrs == null)
			{
				metaAttrs = new int[8][];
				for (int y = 0; y < 8; y++)
				{
					metaAttrs[y] = new int[8];
				}
			}

			DrawingGroup dGroup = new DrawingGroup();

			metaBmps = new BitmapSource[64];
			for (int y = 0; y < 8; y++)
			{
				for (int x = 0; x < 8; x++)
				{
					byte[] fourTileBytes = new byte[64];

					for (int i = 0; i < 4; i++)
					{
						int tileInd = metatiles[y][x][i];

						byte[] thisTileBytes = new byte[16];
						windows.tilePicker.bmps[tileInd].CopyPixels(thisTileBytes, 2, 0);
						thisTileBytes.CopyTo(fourTileBytes, i * 16);
					}

					byte[] newBytes = new byte[64];
					for (int firstHalf = 0; firstHalf < 8; firstHalf++)
					{
						newBytes[firstHalf * 4] = fourTileBytes[firstHalf * 2];
						newBytes[(firstHalf * 4) + 1] = fourTileBytes[(firstHalf * 2) + 1];
						newBytes[(firstHalf * 4) + 2] = fourTileBytes[(firstHalf * 2) + 16];
						newBytes[(firstHalf * 4) + 3] = fourTileBytes[(firstHalf * 2) + 17];
					}
					for (int secondHalf = 0; secondHalf < 8; secondHalf++)
					{
						newBytes[(secondHalf * 4) + 32] = fourTileBytes[(secondHalf * 2) + 32];
						newBytes[((secondHalf * 4) + 1) + 32] = fourTileBytes[((secondHalf * 2) + 1) + 32];
						newBytes[((secondHalf * 4) + 2) + 32] = fourTileBytes[((secondHalf * 2) + 16) + 32];
						newBytes[((secondHalf * 4) + 3) + 32] = fourTileBytes[((secondHalf * 2) + 17) + 32];
					}

					BitmapSource newBmp = BitmapSource.Create(16, 16, 2, 2, PixelFormats.Indexed2,
						new BitmapPalette(windows.tilePicker.bgPalettes[metaAttrs[y][x]]),
						newBytes, 4);
					ImageDrawing bmpImg = new ImageDrawing(newBmp, new Rect(x * 16, y * 16, 16, 16));
					dGroup.Children.Add(bmpImg);
					metaBmps[(y * 8) + x] = newBmp;
				}
			}

			DrawingImage dImg = new DrawingImage(dGroup);
			img_metatiles.Source = dImg;

			DrawGrid();
		}

		public void MetaCursorUp()
		{
			if (selMeta1 < 8) return;
			selMeta1 -= 8;
			Canvas.SetTop(rect_meta_sel1, Canvas.GetTop(rect_meta_sel1) - (16 * zoom));
			windows.map.UpdateDrawingBox();
		}
		public void MetaCursorDown()
		{
			if (selMeta1 >= 56) return;
			selMeta1 += 8;
			Canvas.SetTop(rect_meta_sel1, Canvas.GetTop(rect_meta_sel1) + (16 * zoom));
			windows.map.UpdateDrawingBox();
		}
		public void MetaCursorLeft()
		{
			if (selMeta1 % 8 == 0) return;
			selMeta1--;
			Canvas.SetLeft(rect_meta_sel1, Canvas.GetLeft(rect_meta_sel1) - (16 * zoom));
			windows.map.UpdateDrawingBox();
		}
		public void MetaCursorRight()
		{
			if (selMeta1 % 8 == 7) return;
			selMeta1++;
			Canvas.SetLeft(rect_meta_sel1, Canvas.GetLeft(rect_meta_sel1) + (16 * zoom));
			windows.map.UpdateDrawingBox();
		}

		public void DrawTile(int x, int y, int i, bool left)
		{
			metatiles[y][x][i] = left ? windows.tilePicker.selTile1 : windows.tilePicker.selTile2;
			DrawMetatiles(); // just redraw the whole thing, there's only 256 8x8 tiles
			if ((bool)windows.main.checkbox_meta.IsChecked)
				windows.map.InitMap();
		}
		public void DrawAttr(int x, int y)
		{
			metaAttrs[y][x] = windows.tilePicker.selectedPal - 1;
			DrawMetatiles();
		}
		private void UpdateZoom()
		{
			img_metatiles.Width = 128 * zoom;
			img_metatiles.Height = 128 * zoom;
			rect_meta_shade.Width = 128 * zoom;
			rect_meta_shade.Height = 128 * zoom;
			rect_meta_sel1.Width = 16 * zoom;
			rect_meta_sel1.Height = 16 * zoom;
			rect_meta_sel2.Width = 16 * zoom;
			rect_meta_sel2.Height = 16 * zoom;
			Canvas.SetLeft(rect_meta_sel1, zoom == 4.0 ? Canvas.GetLeft(rect_meta_sel1) * 2.0 : Canvas.GetLeft(rect_meta_sel1) * 0.5);
			Canvas.SetTop(rect_meta_sel1, zoom == 4.0 ? Canvas.GetTop(rect_meta_sel1) * 2.0 : Canvas.GetTop(rect_meta_sel1) * 0.5);
			Canvas.SetLeft(rect_meta_sel2, zoom == 4.0 ? Canvas.GetLeft(rect_meta_sel2) * 2.0 : Canvas.GetLeft(rect_meta_sel2) * 0.5);
			Canvas.SetTop(rect_meta_sel2, zoom == 4.0 ? Canvas.GetTop(rect_meta_sel2) * 2.0 : Canvas.GetTop(rect_meta_sel2) * 0.5);
			rect_meta_hover.Width = 16 * zoom;
			rect_meta_hover.Height = 16 * zoom;
			if ((bool)radio_editing.IsChecked && (bool)radio_tile.IsChecked)
			{
				rect_meta_hover.Width = 8 * zoom;
				rect_meta_hover.Height = 8 * zoom;
			}
			img_meta_sel.Width = 16 * zoom;
			img_meta_sel.Height = 16 * zoom;
			canvas_meta.Width = 128 * zoom;
			canvas_meta.Height = 128 * zoom;
			scroller_settings.Height = 128 * zoom;
			DrawGrid();
		}
		private void DrawGrid()
		{
			if (checkbox_grid_8x8 == null)
				return; // I genuinely hate that I have to do this crap
			grid_grid.Children.Clear();
			Brush darkBrush = new SolidColorBrush(Color.FromArgb(128, 80, 80, 80));
			Brush lighterBrush = new SolidColorBrush(Color.FromArgb(128, 120, 120, 120));
			if ((bool)checkbox_grid_8x8.IsChecked)
			{
				for (int y = 1; y < 16; y++)
					grid_grid.Children.Add(new Line() { StrokeThickness = 1, Stroke = darkBrush, X1 = 0, X2 = canvas_meta.Width, Y1 = y * 8 * zoom, Y2 = y * 8 * zoom });
				for (int x = 1; x < 16; x++)
					grid_grid.Children.Add(new Line() { StrokeThickness = 1, Stroke = darkBrush, X1 = x * 8 * zoom, X2 = x * 8 * zoom, Y1 = 0, Y2 = canvas_meta.Height });
			}
			if ((bool)checkbox_grid_16x16.IsChecked)
			{
				for (int y = 1; y < 8; y++)
					grid_grid.Children.Add(new Line() { StrokeThickness = 1, Stroke = lighterBrush, X1 = 0, X2 = canvas_meta.Width, Y1 = y * 16 * zoom, Y2 = y * 16 * zoom });
				for (int x = 1; x < 8; x++)
					grid_grid.Children.Add(new Line() { StrokeThickness = 1, Stroke = lighterBrush, X1 = x * 16 * zoom, X2 = x * 16 * zoom, Y1 = 0, Y2 = canvas_meta.Height });
			}
		}

		// window
		private void window_metatiles_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && grid_top_bar.IsMouseOver)
				DragMove();
		}
		private void window_metatiles_KeyDown(object sender, KeyEventArgs e)
		{
			bool ctrlDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
			bool shiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
			if (ctrlDown || shiftDown)
				return;
			if ((bool)radio_picking.IsChecked)
			{
				if (e.Key == Key.W)
					MetaCursorUp();
				else if (e.Key == Key.A)
					MetaCursorLeft();
				else if (e.Key == Key.S)
					MetaCursorDown();
				else if (e.Key == Key.D)
					MetaCursorRight();
			}
			else
			{
				if (e.Key == Key.W)
					windows.tilePicker.TileCursorUp();
				else if (e.Key == Key.A)
					windows.tilePicker.TileCursorLeft();
				else if (e.Key == Key.S)
					windows.tilePicker.TileCursorDown();
				else if (e.Key == Key.D)
					windows.tilePicker.TileCursorRight();
			}
			if (e.Key == Key.I)
				ToggleSettingsTab();
		}
		private void button_close_Click(object sender, RoutedEventArgs e)
		{
			Visibility = Visibility.Hidden;
		}
		private void button_max_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
		}
		private void button_min_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}
		private void checkbox_grid_8x8_Click(object sender, RoutedEventArgs e)
		{
			DrawMetatiles();
		}
		private void checkbox_grid_16x16_Click(object sender, RoutedEventArgs e)
		{
			DrawMetatiles();
		}

		// settings tab
		private void ToggleSettingsTab()
		{
			tabBoard.Remove();
			if (Canvas.GetLeft(panel_settings) == 0)
			{
				tabBoard.Children.Add(new DoubleAnimation()
				{
					From = 0,
					To = -256,
					Duration = new Duration(TimeSpan.FromSeconds(0.4)),
					EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut }
				});
			}
			else
			{
				tabBoard.Children.Add(new DoubleAnimation()
				{
					From = -256,
					To = 0,
					Duration = new Duration(TimeSpan.FromSeconds(0.4)),
					EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut }
				});
			}
			Storyboard.SetTarget(tabBoard, panel_settings);
			Storyboard.SetTargetProperty(tabBoard, new PropertyPath(Canvas.LeftProperty));
			tabBoard.Begin();
		}
		private void radio_picking_Checked(object sender, RoutedEventArgs e)
		{
			rect_meta_sel1.Visibility = Visibility.Visible;
			rect_meta_sel2.Visibility = Visibility.Visible;
			rect_meta_shade.Visibility = Visibility.Visible;
			rect_meta_hover.Width = 16 * zoom;
			rect_meta_hover.Height = 16 * zoom;
			rect_meta_hover.Stroke = Brushes.White;
			button_fill_tile.IsEnabled = false;
			button_fill_pal.IsEnabled = false;
			radio_tile.IsEnabled = false;
			radio_palette.IsEnabled = false;
		}
		private void radio_editing_Checked(object sender, RoutedEventArgs e)
		{
			if (rect_meta_sel1 == null || radio_tile == null)
				return;
			rect_meta_sel1.Visibility = Visibility.Hidden;
			rect_meta_sel2.Visibility = Visibility.Hidden;
			rect_meta_shade.Visibility = Visibility.Hidden;

			if ((bool)radio_tile.IsChecked)
				radio_tile_Checked(null, null);
			else
				radio_palette_Checked(null, null);
			button_fill_tile.IsEnabled = true;
			button_fill_pal.IsEnabled = true;
			radio_tile.IsEnabled = true;
			radio_palette.IsEnabled = true;
		}
		private void radio_tile_Checked(object sender, RoutedEventArgs e)
		{
			if (rect_meta_hover == null || !(bool)radio_editing.IsChecked)
				return;
			rect_meta_hover.Width = 8 * zoom;
			rect_meta_hover.Height = 8 * zoom;
			rect_meta_hover.Stroke = Brushes.White;
		}
		private void radio_palette_Checked(object sender, RoutedEventArgs e)
		{
			if (!(bool)radio_editing.IsChecked)
				return;
			rect_meta_hover.Width = 16 * zoom;
			rect_meta_hover.Height = 16 * zoom;
			rect_meta_hover.Stroke = Brushes.Magenta;
		}
		private void radio_2x_Checked(object sender, RoutedEventArgs e)
		{
			zoom = 2.0;
			UpdateZoom();
		}
		private void radio_4x_Checked(object sender, RoutedEventArgs e)
		{
			zoom = 4.0;
			UpdateZoom();
		}
		private void button_fill_tile_Click(object sender, RoutedEventArgs e)
		{
			windows.main.ChangesMade();
			for (int y = 0; y < 8; y++)
			{
				for (int x = 0; x < 8; x++)
				{
					for (int i = 0; i < 4; i++)
					{
						metatiles[y][x][i] = windows.tilePicker.selTile1;
					}
				}
			}
			DrawMetatiles();
		}
		private void button_fill_pal_Click(object sender, RoutedEventArgs e)
		{
			windows.main.ChangesMade();
			for (int y = 0; y < 8; y++)
			{
				for (int x = 0; x < 8; x++)
				{
					metaAttrs[y][x] = windows.tilePicker.selectedPal - 1;
				}
			}
			DrawMetatiles();
		}

		// main canvas
		private void canvas_meta_MouseEnter(object sender, MouseEventArgs e)
		{
			rect_meta_hover.Visibility = Visibility.Visible;

			// appear shade
			board.Remove();
			board.Children.Add(new DoubleAnimation()
			{
				From = 0,
				To = SHADE_OPACITY,
				Duration = new Duration(TimeSpan.FromSeconds(0.3))
			});
			Storyboard.SetTarget(board, rect_meta_shade);
			Storyboard.SetTargetProperty(board, new PropertyPath(OpacityProperty));
			board.Begin();
		}
		private void canvas_meta_MouseLeave(object sender, MouseEventArgs e)
		{
			img_meta_sel.Source = null;
			rect_meta_hover.Visibility = Visibility.Hidden;

			// vanish shade
			board.Remove();
			board.Children.Add(new DoubleAnimation()
			{
				From = SHADE_OPACITY,
				To = 0,
				Duration = new Duration(TimeSpan.FromSeconds(0.3))
			});
			Storyboard.SetTarget(board, rect_meta_shade);
			Storyboard.SetTargetProperty(board, new PropertyPath(OpacityProperty));
			board.Begin();
		}
		private void canvas_meta_MouseMove(object sender, MouseEventArgs e)
		{
			if ((bool)radio_editing.IsChecked)
			{
				int xPos, yPos;
				if ((bool)radio_tile.IsChecked)
				{
					xPos = ((int)e.GetPosition(canvas_meta).X / (int)(8 * zoom)) * (int)(8 * zoom);
					yPos = ((int)e.GetPosition(canvas_meta).Y / (int)(8 * zoom)) * (int)(8 * zoom);
				}
				else
				{
					xPos = ((int)e.GetPosition(canvas_meta).X / (int)(16 * zoom)) * (int)(16 * zoom);
					yPos = ((int)e.GetPosition(canvas_meta).Y / (int)(16 * zoom)) * (int)(16 * zoom);
				}
				Canvas.SetLeft(rect_meta_hover, xPos);
				Canvas.SetTop(rect_meta_hover, yPos);

				// drawing
				if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
				{
					int bigX = (int)e.GetPosition(canvas_meta).X / (int)(16 * zoom);
					int bigY = (int)e.GetPosition(canvas_meta).Y / (int)(16 * zoom);
					int littleX = ((int)e.GetPosition(canvas_meta).X / (int)(8 * zoom)) % 2;
					int littleY = ((int)e.GetPosition(canvas_meta).Y / (int)(8 * zoom)) % 2;
					if ((bool)radio_tile.IsChecked)
						DrawTile(bigX, bigY, (littleY * 2) + littleX, e.LeftButton == MouseButtonState.Pressed);
					else
						DrawAttr(bigX, bigY);
				}
			}
			else
			{
				int xPos = ((int)e.GetPosition(canvas_meta).X / (int)(16 * zoom)) * (int)(16 * zoom);
				int yPos = ((int)e.GetPosition(canvas_meta).Y / (int)(16 * zoom)) * (int)(16 * zoom);
				Canvas.SetLeft(img_meta_sel, xPos);
				Canvas.SetTop(img_meta_sel, yPos);
				img_meta_sel.Source = metaBmps[((yPos / (int)(16 * zoom)) * 8) + (xPos / (int)(16 * zoom))];
				Canvas.SetLeft(rect_meta_hover, xPos);
				Canvas.SetTop(rect_meta_hover, yPos);
			}
		}
		private void canvas_meta_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			windows.main.ChangesMade();
			if ((bool)radio_editing.IsChecked)
			{
				// draw tile
				int bigX = (int)e.GetPosition(canvas_meta).X / (int)(16 * zoom);
				int bigY = (int)e.GetPosition(canvas_meta).Y / (int)(16 * zoom);
				int littleX = ((int)e.GetPosition(canvas_meta).X / (int)(8 * zoom)) % 2;
				int littleY = ((int)e.GetPosition(canvas_meta).Y / (int)(8 * zoom)) % 2;
				if ((bool)radio_tile.IsChecked)
					DrawTile(bigX, bigY, (littleY * 2) + littleX, true);
				else
					DrawAttr(bigX, bigY);
			}
			else
			{
				int xPos = (int)e.GetPosition(canvas_meta).X / (int)(16 * zoom);
				int yPos = (int)e.GetPosition(canvas_meta).Y / (int)(16 * zoom);
				Canvas.SetLeft(rect_meta_sel1, xPos * (16 * zoom));
				Canvas.SetTop(rect_meta_sel1, yPos * (16 * zoom));
				selMeta1 = (yPos * 8) + xPos;
				if ((bool)windows.main.checkbox_meta.IsChecked)
				{
					windows.map.radio_tile.IsChecked = true;
					windows.map.UpdateDrawingBox();
				}
			}
		}
		private void canvas_meta_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			windows.main.ChangesMade();
			if ((bool)radio_picking.IsChecked)
			{
				int xPos = (int)e.GetPosition(canvas_meta).X / (int)(16 * zoom);
				int yPos = (int)e.GetPosition(canvas_meta).Y / (int)(16 * zoom);
				Canvas.SetLeft(rect_meta_sel2, xPos * (16 * zoom));
				Canvas.SetTop(rect_meta_sel2, yPos * (16 * zoom));
				selMeta2 = (yPos * 8) + xPos;
			}
			else
			{
				// draw tile
				int bigX = (int)e.GetPosition(canvas_meta).X / (int)(16 * zoom);
				int bigY = (int)e.GetPosition(canvas_meta).Y / (int)(16 * zoom);
				int littleX = ((int)e.GetPosition(canvas_meta).X / (int)(8 * zoom)) % 2;
				int littleY = ((int)e.GetPosition(canvas_meta).Y / (int)(8 * zoom)) % 2;

				if ((bool)radio_tile.IsChecked)
					DrawTile(bigX, bigY, (littleY * 2) + littleX, false);
			}
		}
	}
}

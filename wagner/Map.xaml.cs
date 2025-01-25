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
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;

namespace Wagner
{
	/// <summary>
	/// Interaction logic for Map.xaml
	/// </summary>
	public partial class Map : Window
	{
		private const double ZOOM_PERCENT = 1.2;
		private const double ZOOM_MAX = 8.0;
		private const double ZOOM_MIN = 0.5;
		private const double ZOOM_START = 2.0;
		private const int KEYS_PAN_SPEED = 24;
		private const byte DRAWING_BOX_MARGIN = 24;

		private WindowSet windows;
		private DrawingGroup tileDrawingGroup;
		private DrawingImage tileDrawingImg;
		private DrawingGroup metaDrawingGroup;
		private DrawingImage metaDrawingImg;
		private Point dragStartPos;
		private double dragStartLeft;
		private double dragStartTop;
		private double imgTopRatio; // the percentage, in relation to the height of the canvas, of the Canvas.Top of the img, so it stays centered when resizing
		private double imgLeftRatio;
		private Storyboard board;
		public int mapWidth = 32;
		public int mapHeight = 30;
		public double zoom = ZOOM_START;
		private Rectangle currIObjRect;
		private Point currIObjStartPos;
		private byte currIobjType; // 0 - visible 1 - slope 2 - ladder 3 - everything else

		// walls
		private DrawingGroup wallsDrawingGroup;
		private DrawingImage wallsDrawingImg;
		private BitmapPalette wallsPal;
		private BitmapSource wallsBmp;
		private BitmapSource emptyWallsBmp;

		// backbone of this window, and used for exporting
		public int[][] tiles;
		public int[][] metas;
		public int[][] attrs;
		public bool[][] walls;
		public int plrX, plrY;
		public List<List<PlaceObj>> objs; // why a double list? Because the objects are split up into batches that get loaded in as you move through the level, idiot
		public List<PlaceObj> iobjs;
		public List<CamLead> camLeads;
		public int numVisibles = 0;
		public int numSlopes = 0;
		public int numLadders = 0;

		// anchor stuff
		private bool anchorUp = true;
		private bool anchorLeft = true;
		public int tileXOffs = 0;
		public int tileYOffs = 0;

		public Map(WindowSet windows)
		{
			InitializeComponent();

			// settings forms
			textbox_width.Text = mapWidth.ToString();
			textbox_height.Text = mapHeight.ToString();
			radio_tile.IsChecked = true;
			textbox_zoom.Text = (zoom * 100).ToString();

			// init zoom
			UpdateZoom(0.5, 0.5);

			board = new Storyboard();
			dragStartPos = new Point(-1, -1);
			this.windows = windows;

			objs = new List<List<PlaceObj>>(); // init batches
			objs.Add(new List<PlaceObj>()); // init first batch
			iobjs = new List<PlaceObj>();
			camLeads = new List<CamLead>();

			// walls bmp
			wallsPal = new BitmapPalette(new Color[2] { Colors.Transparent, Colors.DeepSkyBlue });
			byte[] pxls = new byte[32]
			{
				0b11110000, 0b11110000,
				0b11110000, 0b11110000,
				0b11110000, 0b11110000,
				0b11110000, 0b11110000,
				0b00001111, 0b00001111,
				0b00001111, 0b00001111,
				0b00001111, 0b00001111,
				0b00001111, 0b00001111,
				0b11110000, 0b11110000,
				0b11110000, 0b11110000,
				0b11110000, 0b11110000,
				0b11110000, 0b11110000,
				0b00001111, 0b00001111,
				0b00001111, 0b00001111,
				0b00001111, 0b00001111,
				0b00001111, 0b00001111
			};
			wallsBmp = BitmapSource.Create(16, 16, 2, 2, PixelFormats.Indexed1, wallsPal, pxls, 2);
			emptyWallsBmp = BitmapSource.Create(16, 16, 2, 2, PixelFormats.Indexed1, wallsPal, new byte[32], 2);

			InitMap();
			UpdateObjects(0);
			UpdateIObjects(0);
			//combo_iobjs.ItemsSource = windows.iobjs.names;
		}

		// helper methods
		public void InitMap()
		{
			// 2 sides of the same coin: drawing w/ tiles and drawing w/ metatiles
			// tiles
			tileDrawingGroup = new DrawingGroup();
			tileDrawingImg = new DrawingImage(tileDrawingGroup);
			// metas
			metaDrawingGroup = new DrawingGroup();
			metaDrawingImg = new DrawingImage(metaDrawingGroup);

			img_map.Source = ((bool)windows.main.checkbox_meta.IsChecked) ? metaDrawingImg : tileDrawingImg;

			// walls
			wallsDrawingGroup = new DrawingGroup();
			wallsDrawingImg = new DrawingImage(wallsDrawingGroup);
			img_walls.Source = wallsDrawingImg;

			if (mapHeight == 0 || mapWidth == 0) return;

			// attrs
			if (attrs == null || (attrs.Length < mapHeight / 2 || attrs[0].Length < mapWidth / 2))
			{
				attrs = new int[mapHeight / 2][];
				for (int y = 0; y < mapHeight / 2; y++)
				{
					attrs[y] = new int[mapWidth / 2];
				}
			}

			// tiles
			if (tiles != null && tiles.Length >= mapHeight && tiles[0].Length >= mapWidth)
			{
				for (int y = tileYOffs; y < tileYOffs + mapHeight; y++)
				{
					for (int x = tileXOffs; x < tileXOffs + mapWidth; x++)
					{
						BitmapSource bmp = windows.tilePicker.bmps[tiles[y][x]];
						byte[] pxls = new byte[16];
						bmp.CopyPixels(pxls, 2, 0);
						bmp = BitmapSource.Create(8, 8, 2, 2, PixelFormats.Indexed2, new BitmapPalette(windows.tilePicker.bgPalettes[attrs[y / 2][x / 2]]), pxls, 2);
						tileDrawingGroup.Children.Add(new ImageDrawing(
							bmp,
							new Rect((x - tileXOffs) * 8, (y - tileYOffs) * 8, 8, 8))
						);
					}
				}
			}
			else
			{
				// fill with selTile2 & init arrays
				tiles = new int[mapHeight][];
				for (int y = 0; y < mapHeight; y++)
				{
					tiles[y] = new int[mapWidth];
					for (int x = 0; x < mapWidth; x++)
					{
						tiles[y][x] = windows.tilePicker.selTile2;
						tileDrawingGroup.Children.Add(new ImageDrawing(
							windows.tilePicker.bmps[windows.tilePicker.selTile2],
							new Rect(x * 8, y * 8, 8, 8))
						);
					}
				}
			}
			
			// metatiles
			if (metas != null)
			{
				for (int y = tileYOffs / 2; y < (tileYOffs + mapHeight) / 2; y++)
				{
					for (int x = tileXOffs / 2; x < (tileXOffs + mapWidth) / 2; x++)
					{
						BitmapSource bmp = windows.meta.metaBmps[metas[y][x]];
						byte[] pxls = new byte[64];
						bmp.CopyPixels(pxls, 4, 0);
						bmp = BitmapSource.Create(16, 16, 2, 2, PixelFormats.Indexed2, new BitmapPalette(windows.tilePicker.bgPalettes[attrs[y][x]]), pxls, 4);
						metaDrawingGroup.Children.Add(new ImageDrawing(
							bmp,
							new Rect((x - (tileXOffs / 2)) * 16, (y - (tileYOffs / 2)) * 16, 16, 16))
						);
					}
				}
			}
			else
			{
				// fill with selMeta2 & init arrays
				metas = new int[mapHeight / 2][];
				for (int y = 0; y < mapHeight / 2; y++)
				{
					metas[y] = new int[mapWidth / 2];
					for (int x = 0; x < mapWidth / 2; x++)
					{
						metas[y][x] = windows.meta.selMeta2;
						metaDrawingGroup.Children.Add(new ImageDrawing(
							windows.meta.metaBmps[windows.meta.selMeta2],
							new Rect(x * 16, y * 16, 16, 16))
						);
					}
				}
			}

			// walls
			if (walls != null && walls.Length >= mapHeight / 2 && walls[0].Length >= mapWidth / 2)
			{
				for (int y = tileYOffs / 2; y < (tileYOffs + mapHeight) / 2; y++)
				{
					for (int x = tileXOffs / 2; x < (tileXOffs + mapWidth) / 2; x++)
					{
						wallsDrawingGroup.Children.Add(new ImageDrawing(
								walls[y][x] ? wallsBmp : emptyWallsBmp, new Rect(x * 16, y * 16, 16, 16)
							)
						);
					}
				}
			}
			else
			{
				walls = new bool[mapHeight / 2][];
				for (int y = 0; y < mapHeight / 2; y++)
				{
					walls[y] = new bool[mapWidth / 2];
					for (int x = 0; x < mapWidth / 2; x++)
					{
						wallsDrawingGroup.Children.Add(new ImageDrawing(
							emptyWallsBmp, new Rect(x * 16, y * 16, 16, 16))
						);
					}
				}
			}

			UpdateDrawingBox();
		}
		private void ClampMapLoc()
		{
			Canvas.SetLeft(viewbox_map,
				Math.Max(
					Math.Min(
						Canvas.GetLeft(viewbox_map),
						canvas_map.ActualWidth - (canvas_map.ActualWidth * .25)),
					0 - (viewbox_map.Width - (canvas_map.ActualWidth * .25))) // leaving 25% visible
				);
			Canvas.SetTop(viewbox_map,
				Math.Max(
					Math.Min(
						Canvas.GetTop(viewbox_map),
						canvas_map.ActualHeight - (canvas_map.ActualHeight * .25)),
					0 - (viewbox_map.Height - (canvas_map.ActualHeight * .25))) // leaving 25% visible
				);
			DrawGrid();
		}
		private void ClampMapZoom()
		{
			if (zoom > ZOOM_MAX)
			{
				zoom = ZOOM_MAX;
				viewbox_map.Width = (mapWidth * 8) * ZOOM_MAX;
				viewbox_map.Height = (mapHeight * 8) * ZOOM_MAX;
			} else if (zoom < ZOOM_MIN)
			{
				zoom = ZOOM_MIN;
				viewbox_map.Width = (mapWidth * 8) * ZOOM_MIN;
				viewbox_map.Height = (mapWidth * 8) * ZOOM_MIN;
			}
		}
		public void UpdatePalette()
		{
			for (int y = 0; y < attrs.Length; y++)
			{
				for (int x = 0; x < attrs[y].Length; x++)
				{
					if (attrs[y][x] == windows.tilePicker.selectedPal - 1)
						DrawAttr(x * 16, y * 16);
				}
			}
		}
		private void UpdateSelRect(double mouseX, double mouseY)
		{
			if (mouseX < 0 || mouseY < 0 || mouseX > img_map.ActualWidth || mouseY > img_map.ActualHeight)
			{
				rect_map_sel.Visibility = Visibility.Hidden;
				HideMeasures();
				return;
			}
			if (panel_measure.IsVisible && !Keyboard.IsKeyDown(Key.M))
			{
				rect_map_sel.Visibility = Visibility.Hidden;
				return;
			}
			int sizeX = (bool)windows.main.checkbox_meta.IsChecked ? 16 : 8;
			int sizeY = sizeX;
			int gridX = 0;
			int gridY = 0;
			Brush brush = Brushes.White;
			if ((bool)radio_tile.IsChecked)
				brush = Brushes.White;
			else if ((bool)radio_palette.IsChecked)
			{
				sizeX = 16;
				sizeY = 16;
				brush = Brushes.Violet;
			}
			else if ((bool)radio_walls.IsChecked)
			{
				sizeX = 16;
				sizeY = 16;
				brush = Brushes.DeepSkyBlue;
			}
			else if ((bool)radio_objs.IsChecked)
			{
				sizeX = windows.objs.objs[combo_objs.SelectedIndex].states[0].width * 8;
				sizeY = windows.objs.objs[combo_objs.SelectedIndex].states[0].height * 8;
				gridX = combo_objs.SelectedIndex == 0 ? 8 : 16;
				gridY = gridX;
				brush = Brushes.PaleGreen;
			}
			else if ((bool)radio_iobjs.IsChecked)
			{
				sizeX = 16;
				sizeY = 16;
				brush = Brushes.BurlyWood;
			}
			else if ((bool)radio_camleads.IsChecked)
			{
				sizeX = 16;
				sizeY = 16;
				brush = Brushes.White;
			}
			if (gridX == 0)
			{
				gridX = sizeX;
				gridY = sizeY;
			}
			double scale = viewbox_map.ActualWidth / (mapWidth * 8);
			double left = (((int)mouseX / gridX) * gridX) * scale;
			double top = (((int)mouseY / gridY) * gridY) * scale;

			// hold "N" to make the cursor nametable-size
			rect_title_safe.Visibility = Visibility.Hidden;
			if (Keyboard.IsKeyDown(Key.N) || (bool)radio_camleads.IsChecked)
			{
				left =
					Math.Max(
						Math.Min(
							(((int)(mouseX - 128) / sizeX) * sizeX) * scale,
							(img_map.ActualWidth - 256) * scale
						),
						0
					);
				top =
					Math.Max(
						Math.Min(
							(((int)(mouseY - 120) / sizeY) * sizeY) * scale,
							(img_map.ActualHeight - 240) * scale
						),
						0
					);
				sizeX = 256;
				sizeY = 240;

				// hold B to display title safe area
				if (Keyboard.IsKeyDown(Key.B))
				{
					rect_title_safe.Visibility = Visibility.Visible;
					rect_title_safe.Width = 224 * scale;
					rect_title_safe.Height = 192 * scale;
					Canvas.SetLeft(rect_title_safe, Canvas.GetLeft(viewbox_map) + left + (16 * scale));
					Canvas.SetTop(rect_title_safe, Canvas.GetTop(viewbox_map) + top + (24 * scale));
				}
			}

			rect_map_sel.Visibility = Visibility.Visible;
			rect_map_sel.Width = sizeX * scale;
			rect_map_sel.Height = sizeY * scale;
			rect_map_sel.Stroke = brush;
			Canvas.SetLeft(rect_map_sel, Canvas.GetLeft(viewbox_map) + left);
			Canvas.SetTop(rect_map_sel, Canvas.GetTop(viewbox_map) + top);

			// hold "M" to display the measure box
			if (Keyboard.IsKeyDown(Key.M))
			{
				ShowMeasures(mouseX, mouseY, scale, sizeX);
			}
		}
		private void ShowMeasures(double mouseX, double mouseY, double scale, int sizeX)
		{
			panel_measure.Visibility = Visibility.Visible;
			text_measure.Inlines.Clear();
			text_measure.Inlines.Add(new Run("8x8") { FontWeight = FontWeights.Bold });
			text_measure.Inlines.Add("\t(" + ((int)mouseX / 8).ToString() + ", " + ((int)mouseY / 8).ToString() + ")\n");
			text_measure.Inlines.Add(new Run("16x16") { FontWeight = FontWeights.Bold });
			text_measure.Inlines.Add("\t{" + ((int)mouseX / 16).ToString() + ", " + ((int)mouseY / 16).ToString() + ")\n");
			text_measure.Inlines.Add(new Run("N.table") { FontWeight = FontWeights.Bold });
			text_measure.Inlines.Add("\t(" + ((int)mouseX / 256).ToString() + ", " + ((int)mouseY / 240).ToString() + ")\n");

			// PPU addresses
			int ppuTile = (
				// low byte
				(((int)mouseX / 8) % 32) +
				((((((int)mouseY % 240) / 8) * 2) % 16) * 16) +
				// high byte
				((((int)mouseY % 240) / 64) * 0x0100) +
				((((int)mouseX / 256) % 2) * 0x0400) +
				0x2000
			);
			int ppuAttr = (
				// low byte
				(((int)mouseX / 32) % 8) +
				((((int)mouseY % 240) / 32) * 8) +
				// high byte
				((((int)mouseX / 256) % 2) * 0x0400) +
				0x23c0
			);

			text_measure.Inlines.Add(new Run("PPU tile") { FontWeight = FontWeights.Bold });
			text_measure.Inlines.Add("\t$" + ppuTile.ToString("x4") + "\n");
			text_measure.Inlines.Add(new Run("PPU attr") { FontWeight = FontWeights.Bold });
			text_measure.Inlines.Add("\t$" + ppuAttr.ToString("x4"));

			// make other rectangles visible
			rect_measure_nt.Visibility = Visibility.Visible;
			rect_measure_8.Visibility = Visibility.Visible;
			rect_measure_16.Visibility = Visibility.Visible;
			rect_map_sel.Visibility = Visibility.Hidden;

			// 8x8 square
			rect_measure_8.Width = 8 * scale;
			rect_measure_8.Height = 8 * scale;
			Canvas.SetLeft(rect_measure_8, Canvas.GetLeft(viewbox_map) + ((((int)mouseX / 8) * 8) * scale));
			Canvas.SetTop(rect_measure_8, Canvas.GetTop(viewbox_map) + ((((int)mouseY / 8) * 8) * scale));

			// 16x16 square
			rect_measure_16.Width = 16 * scale;
			rect_measure_16.Height = 16 * scale;
			Canvas.SetLeft(rect_measure_16, Canvas.GetLeft(viewbox_map) + ((((int)mouseX / 16) * 16) * scale));
			Canvas.SetTop(rect_measure_16, Canvas.GetTop(viewbox_map) + ((((int)mouseY / 16) * 16) * scale));

			// nametable square
			Canvas.SetLeft(rect_measure_nt, Canvas.GetLeft(viewbox_map) + ((((int)mouseX / 256) * 256) * scale));
			Canvas.SetTop(rect_measure_nt, Canvas.GetTop(viewbox_map) + ((((int)mouseY / 240) * 240) * scale));
			rect_measure_nt.Width = 256 * scale;
			rect_measure_nt.Height = 240 * scale;

			// set position of measure window
			Canvas.SetLeft(panel_measure,
				Math.Min(
					Canvas.GetLeft(rect_measure_16) + rect_measure_16.Width,
					canvas_map.ActualWidth - panel_measure.ActualWidth
				)
			);
			Canvas.SetTop(panel_measure, Canvas.GetTop(rect_measure_16) + rect_measure_16.Height);
			if (Canvas.GetTop(panel_measure) + panel_measure.ActualHeight > canvas_map.ActualHeight)
				Canvas.SetTop(panel_measure, Canvas.GetTop(rect_measure_16) - panel_measure.ActualHeight);
		}
		private void HideMeasures()
		{
			panel_measure.Visibility = Visibility.Hidden;
			rect_measure_8.Visibility = Visibility.Hidden;
			rect_measure_16.Visibility = Visibility.Hidden;
			rect_measure_nt.Visibility = Visibility.Hidden;
			rect_title_safe.Visibility = Visibility.Hidden;
			rect_map_sel.Visibility = Visibility.Visible;
		}
		private void FilterNumber(object sender)
		{
			((TextBox)sender).Text = Regex.Replace(((TextBox)sender).Text, "[^0-9.]", "");
		}
		public void UpdateZoom(double x, double y)
		{
			ClampMapZoom();
			viewbox_map.Width = mapWidth * 8 * zoom;
			viewbox_map.Height = mapHeight * 8 * zoom;
			double diffX = viewbox_map.Width - viewbox_map.ActualWidth;
			double diffY = viewbox_map.Height - viewbox_map.ActualHeight;
			Canvas.SetLeft(viewbox_map, Canvas.GetLeft(viewbox_map) - (diffX * x));
			Canvas.SetTop(viewbox_map, Canvas.GetTop(viewbox_map) - (diffY * y));

			ClampMapLoc();
			rect_map_sel.Visibility = Visibility.Hidden;
			rect_title_safe.Visibility = Visibility.Hidden;
			panel_measure.Visibility = Visibility.Hidden;
			textbox_zoom.Text = (zoom * 100).ToString("F0");

			DrawGrid();
		}
		private void ResetAnchorButtons()
		{
			anchor_ul.Fill = anchor_ul.TryFindResource("brush_light2") as Brush;
			anchor_ur.Fill = anchor_ur.TryFindResource("brush_light2") as Brush;
			anchor_dl.Fill = anchor_dl.TryFindResource("brush_light2") as Brush;
			anchor_dr.Fill = anchor_dr.TryFindResource("brush_light2") as Brush;
		}
		private void UpdateAnchorButtons()
		{
			ResetAnchorButtons();

			if (anchorUp)
			{
				if (anchorLeft)
					anchor_ul.Fill = anchor_ul.TryFindResource("brush_light4") as Brush;
				else
					anchor_ur.Fill = anchor_ul.TryFindResource("brush_light4") as Brush;
			}
			else
			{
				if (anchorLeft)
					anchor_dl.Fill = anchor_ul.TryFindResource("brush_light4") as Brush;
				else
					anchor_dr.Fill = anchor_ul.TryFindResource("brush_light4") as Brush;
			}
		}
		private void DrawGrid()
		{
			if (checkbox_grid_8x8 == null)
				return; // I genuinely hate that I have to do this crap
			grid_grid.Children.Clear();
			Brush darkBrush = new SolidColorBrush(Color.FromRgb(60, 60, 60));
			Brush lighterBrush = new SolidColorBrush(Color.FromRgb(100, 100, 100));
			Brush light3Brush = new SolidColorBrush(Color.FromRgb(140, 140, 140));
			Canvas.SetLeft(grid_grid, Canvas.GetLeft(viewbox_map));
			Canvas.SetTop(grid_grid, Canvas.GetTop(viewbox_map));
			if ((bool)checkbox_grid_8x8.IsChecked)
			{
				for (int y = 1; y < mapHeight; y++)
					grid_grid.Children.Add(new Line() { StrokeThickness = 1, Stroke = darkBrush, X1 = 0, X2 = mapWidth * 8 * zoom, Y1 = y * 8 * zoom, Y2 = y * 8 * zoom });
				for (int x = 1; x < mapWidth; x++)
					grid_grid.Children.Add(new Line() { StrokeThickness = 1, Stroke = darkBrush, X1 = x * 8 * zoom, X2 = x * 8 * zoom, Y1 = 0, Y2 = mapHeight * 8 * zoom });
			}
			if ((bool)checkbox_grid_16x16.IsChecked)
			{
				for (int y = 1; y < mapHeight / 2; y++)
					grid_grid.Children.Add(new Line() { StrokeThickness = 1, Stroke = lighterBrush, X1 = 0, X2 = mapWidth * 8 * zoom, Y1 = y * 16 * zoom, Y2 = y * 16 * zoom });
				for (int x = 1; x < mapWidth / 2; x++)
					grid_grid.Children.Add(new Line() { StrokeThickness = 1, Stroke = lighterBrush, X1 = x * 16 * zoom, X2 = x * 16 * zoom, Y1 = 0, Y2 = mapHeight * 8 * zoom });
			}
			if ((bool)checkbox_grid_attribute.IsChecked)
			{
				byte cutoff = 0;
				for (int y = 1; y < mapHeight / 2; y++)
				{
					// go down in 16x16 tiles.  Normally we skip every other one and draw a line, but every 8 of them is cut in half
					cutoff++;
					if (cutoff < 15)
					{
						if (cutoff % 2 == 0)
							grid_grid.Children.Add(new Line() { StrokeThickness = 1, Stroke = light3Brush, X1 = 0, X2 = mapWidth * 8 * zoom, Y1 = y * 16 * zoom, Y2 = y * 16 * zoom });
					}
					else
					{
						grid_grid.Children.Add(new Line() { StrokeThickness = 1, Stroke = light3Brush, X1 = 0, X2 = mapWidth * 8 * zoom, Y1 = y * 16 * zoom, Y2 = y * 16 * zoom });
						cutoff = 0;
					}
				}
				for (int x = 1; x < mapWidth / 4; x++)
					grid_grid.Children.Add(new Line() { StrokeThickness = 1, Stroke = light3Brush, X1 = x * 32 * zoom, X2 = x * 32 * zoom, Y1 = 0, Y2 = mapHeight * 8 * zoom });
			}
			if ((bool)checkbox_grid_nametable.IsChecked)
			{
				for (int y = 1; y < mapHeight / 30; y++)
					grid_grid.Children.Add(new Line() { StrokeThickness = 1, Stroke = light3Brush, X1 = 0, X2 = mapWidth * 8 * zoom, Y1 = y * 240 * zoom, Y2 = y * 240 * zoom });
				for (int x = 1; x < mapWidth / 32; x++)
					grid_grid.Children.Add(new Line() { StrokeThickness = 1, Stroke = light3Brush, X1 = x * 256 * zoom, X2 = x * 256 * zoom, Y1 = 0, Y2 = mapHeight * 8 * zoom });
			}
		}
		public void UpdateDrawingBox()
		{
			BitmapPalette grayPal = new BitmapPalette(new Color[4] {
				Color.FromRgb(40, 40, 40), 
				Color.FromRgb(80, 80, 80),
				Color.FromRgb(120, 120, 120),
				Color.FromRgb(160, 160, 160)
			});
			if ((bool)radio_tile.IsChecked && windows != null)
			{
				if ((bool)windows.main.checkbox_meta.IsChecked)
				{
					BitmapPalette palToUse = (bool)checkbox_attr_from_meta.IsChecked
						? new BitmapPalette(windows.tilePicker.bgPalettes[windows.meta.metaAttrs[windows.meta.selMeta1 / 8][windows.meta.selMeta1 % 8]])
						: grayPal;
					byte[] pxls = new byte[64];
					windows.meta.metaBmps[windows.meta.selMeta1].CopyPixels(pxls, 4, 0);
					BitmapSource newBmp = BitmapSource.Create(16, 16, 2, 2, PixelFormats.Indexed2, palToUse, pxls, 4);
					img_drawing.Source = newBmp;
				}
				else
				{
					byte[] pxls = new byte[16];
					windows.tilePicker.bmps[windows.tilePicker.selTile1].CopyPixels(pxls, 2, 0);
					BitmapSource newBmp = BitmapSource.Create(8, 8, 2, 2, PixelFormats.Indexed2, grayPal, pxls, 2);
					img_drawing.Source = newBmp;
				}
			}
			else if ((bool)radio_palette.IsChecked)
			{
				byte[] pxls = new byte[4] {
					0b00000000,
					0b01010101,
					0b10101010,
					0b11111111
				};
				BitmapSource bmp = BitmapSource.Create(4, 4, 2, 2, PixelFormats.Indexed2,
					new BitmapPalette(windows.tilePicker.bgPalettes[windows.tilePicker.selectedPal - 1]), pxls, 1);
				img_drawing.Source = bmp;
			}
			else if ((bool)radio_walls.IsChecked)
			{
				img_drawing.Source = null;
				label_drawing.Content = "(walls)";
			}
			else if ((bool)radio_objs.IsChecked)
			{
				img_drawing.Source = null;
				label_drawing.Content = $"(object: {combo_objs.SelectedItem})";
			}
			else if ((bool)radio_iobjs.IsChecked)
			{
				img_drawing.Source = null;
				label_drawing.Content = $"(i-object: {combo_iobjs.SelectedItem})";
			}
			else
			{
				img_drawing.Source = null;
				label_drawing.Content = "(cam leads)";
			}
		}
		public void UpdateObjects(int targSelInd)
		{
			combo_objs.ItemsSource = windows.objs.objs.Select(obj => obj.name).ToArray();
			combo_objs.SelectedIndex =
				Math.Max(
					Math.Min(targSelInd, windows.objs.objs.Count - 1),
					0
				);
			UpdateDrawingBox();
		}
		public void UpdateIObjects(int targSelInd)
		{
			combo_iobjs.ItemsSource = windows.iobjs.names.ToArray();
			combo_iobjs.SelectedIndex =
				Math.Max(
					Math.Min(targSelInd, windows.iobjs.names.Count),
					0
				);
			UpdateDrawingBox();
		}
		public void RedrawObjs()
		{
			// clear all current ones
			for (int i = 1; i < grid_objs.Children.Count; i++)
			{
				((Grid)grid_objs.Children[i]).Children.Clear();
			}

			// draw player
			combo_objs.SelectedIndex = 0;
			DrawObj(plrX, plrY, true, false, true);

			// draw objects
			string oldBatchText = textbox_batch.Text;
			for (byte b = 0; b < int.Parse(textbox_num_batches.Text); b++)
			{
				textbox_batch.Text = b.ToString(); // switch to that batch so we're not redrawing objects in the worng batches
				for (byte o = 0; o < objs[b].Count; o++)
				{
					combo_objs.SelectedIndex = objs[b][o].type; // select that object
					DrawObj(objs[b][o].x * 16, objs[b][o].y * 16, true, false, true);
				}
			}
			textbox_batch.Text = oldBatchText;
		}
		private void UpdateRemObjsIObjs()
        {
			if (objs.Count <= int.Parse(textbox_batch.Text)) return;
			label_remobjs.Content = "Remaining objs in batch " + textbox_batch.Text + ": " + (4 - objs[int.Parse(textbox_batch.Text)].Count).ToString();
			label_remiobjs.Content = "Remaining i-objs: " + (64 - iobjs.Count).ToString();
        }

		// draw methods
		private void DrawTile(double mouseX, double mouseY, bool left)
		{
			if (mouseX < 0 || mouseY < 0 || mouseX > img_map.ActualWidth || mouseY > img_map.ActualHeight)
				return;
			if ((bool)windows.main.checkbox_meta.IsChecked)
			{
				int x = (int)mouseX / 16;
				int y = (int)mouseY / 16;

				// get current selected tile from the TilePicker
				BitmapSource bmp = left ? windows.meta.metaBmps[windows.meta.selMeta1] : windows.meta.metaBmps[windows.meta.selMeta2];

				// if "pull attributes from metatile" is checked, also set the attribute here
				if ((bool)checkbox_attr_from_meta.IsChecked)
				{
					int palInd = left ? windows.meta.selMeta1 : windows.meta.selMeta2;
					attrs[y + (tileYOffs / 2)][x + (tileXOffs / 2)] = windows.meta.metaAttrs[palInd / 8][palInd % 8];
				}
				// convert it into whatever color attribute is drawn here, if "pull attribute from metatile" is NOT checked
				else
				{
					byte[] pxls = new byte[64];
					bmp.CopyPixels(pxls, 4, 0);
					bmp = BitmapSource.Create(16, 16, 2, 2, PixelFormats.Indexed2, new BitmapPalette(windows.tilePicker.bgPalettes[attrs[y + tileYOffs][x + tileXOffs]]), pxls, 4);
				}

				// draw that tile with correctd color here
				int ind = (y * (mapWidth / 2)) + x;
				metaDrawingGroup.Children[ind] = new ImageDrawing(bmp, new Rect(x * 16, y * 16, 16, 16));
				metas[y + (tileYOffs / 2)][x + (tileXOffs / 2)] = left ? windows.meta.selMeta1 : windows.meta.selMeta2;
			}
			else
			{
				int x = (int)mouseX / 8;
				int y = (int)mouseY / 8;

				// get current selected tile from the TilePicker
				BitmapSource bmp = left ? windows.tilePicker.bmps[windows.tilePicker.selTile1] : windows.tilePicker.bmps[windows.tilePicker.selTile2];

				// convert it into whatever color attribute is drawn here
				byte[] pxls = new byte[16];
				bmp.CopyPixels(pxls, 2, 0);
				bmp = BitmapSource.Create(8, 8, 2, 2, PixelFormats.Indexed2, new BitmapPalette(windows.tilePicker.bgPalettes[attrs[(y + tileYOffs) / 2][(x + tileXOffs) / 2]]), pxls, 2);

				// draw that tile with correctd color here
				int ind = (y * mapWidth) + x;
				tileDrawingGroup.Children[ind] = new ImageDrawing(bmp, new Rect(x * 8, y * 8, 8, 8));
				tiles[y + tileYOffs][x + tileXOffs] = left ? windows.tilePicker.selTile1 : windows.tilePicker.selTile2;
			}
		}
		private void DrawAttr(double mouseX, double mouseY)
		{
			if (mouseX < 0 || mouseY < 0 || mouseX > img_map.ActualWidth || mouseY > img_map.ActualHeight)
				return;
			int x = (int)mouseX / 16;
			int y = (int)mouseY / 16;
			int xTileInd = x + (tileXOffs / 2);
			int yTileInd = y + (tileYOffs / 2);

			if ((bool)windows.main.checkbox_meta.IsChecked)
			{
				BitmapSource bmp = windows.meta.metaBmps[metas[y + (tileYOffs / 2)][x + (tileXOffs / 2)]];
				byte[] pxls = new byte[64];
				bmp.CopyPixels(pxls, 4, 0);
				bmp = BitmapSource.Create(16, 16, 2, 2, PixelFormats.Indexed2,
					new BitmapPalette(windows.tilePicker.bgPalettes[windows.tilePicker.selectedPal - 1]), pxls, 4);
				int ind = (y * (mapWidth / 2)) + x;
				if (metaDrawingGroup.Children.Count > ind)
					metaDrawingGroup.Children[ind] = new ImageDrawing(bmp, new Rect(x * 16, y * 16, 16, 16));
			}
			else
			{
				for (int yInd = 0; yInd < 2; yInd++)
				{
					if ((y * 2) + yInd >= mapHeight)
						break;
					for (int xInd = 0; xInd < 2; xInd++)
					{
						if ((x * 2) + xInd >= mapWidth)
							break;
						int tile = tiles[(yTileInd * 2) + yInd][(xTileInd * 2) + xInd];
						BitmapSource bmp = windows.tilePicker.bmps[tile]; // this will just naturally have selectedPal as its colors
						int ind = ((y * mapWidth * 2) + (yInd * mapWidth)) + ((x * 2) + xInd);
						tileDrawingGroup.Children[ind] = new ImageDrawing(bmp, new Rect((x * 16) + (xInd * 8), (y * 16) + (yInd * 8), 8, 8));
					}
				}
			}
			attrs[y + (tileYOffs / 2)][x + (tileXOffs / 2)] = windows.tilePicker.selectedPal - 1;
		}
		private void DrawWall(double mouseX, double mouseY, bool left)
		{
			if (mouseX < 0 || mouseY < 0 || mouseX > img_walls.ActualWidth || mouseY > img_walls.ActualHeight)
				return;
			if (!(bool)radio_walls.IsChecked)
				return;
			int x = (int)mouseX / 16;
			int y = (int)mouseY / 16;
			int ind = (y * (mapWidth / 2)) + x;
			wallsDrawingGroup.Children[ind] = new ImageDrawing(left ? wallsBmp : emptyWallsBmp, new Rect(x * 16, y * 16, 16, 16));
			walls[y][x] = left;
		}
		public void DrawObj(double mouseX, double mouseY, bool left, bool constrain = true, bool dontAddToArray = false)
		{
			if (constrain && (mouseX < 0 || mouseY < 0 || mouseX > img_map.ActualWidth || mouseY > img_map.ActualHeight))
				return;

			if (!left)
				return;

			int scale = combo_objs.SelectedIndex == 0 ? 8 : 16;
			double gridX = ((int)mouseX / scale) * scale;
			double gridY = ((int)mouseY / scale) * scale;

			// draw object bitmap
			Objekt obj = windows.objs.objs[combo_objs.SelectedIndex];
			DrawingGroup objDg = new DrawingGroup();
			DrawingImage objDi = new DrawingImage(objDg);
			for (int y = 0; y < obj.states[0].height; y++)
			{
				for (int x = 0; x < obj.states[0].width; x++)
				{
					// convert it into whatever color attribute is drawn here
					BitmapSource bmp = windows.tilePicker.bmps[(obj.states[0].frames[0].tiles[y][x] % 256) + 256];
					byte[] pxls = new byte[16];
					bmp.CopyPixels(pxls, 2, 0);
					// flipped vertically
					if ((obj.states[0].frames[0].attrs[y][x] & 0b10000000) != 0)
					{
						byte[] flippedPxls = new byte[16];
						for (int i = 0; i < 8; i++)
						{
							flippedPxls[i * 2] = pxls[14 - (i * 2)];
							flippedPxls[(i * 2) + 1] = pxls[15 - (i * 2)];
						}
						pxls = flippedPxls;
					}
					// flipped horizontally
					if ((obj.states[0].frames[0].attrs[y][x] & 0b1000000) != 0)
					{
						byte[] flippedPxls = new byte[16];
						for (int i = 0; i < 8; i++)
						{
							flippedPxls[i * 2] = (byte)(
								((pxls[(i * 2) + 1] & 0b11) << 6) |
								((pxls[(i * 2) + 1] & 0b1100) << 2) |
								((pxls[(i * 2) + 1] & 0b110000) >> 2) |
								((pxls[(i * 2) + 1] & 0b11000000) >> 6)
							);
							flippedPxls[(i * 2) + 1] = (byte)(
								((pxls[i * 2] & 0b11) << 6) |
								((pxls[i * 2] & 0b1100) << 2) |
								((pxls[i * 2] & 0b110000) >> 2) |
								((pxls[i * 2] & 0b11000000) >> 6)
							);
						}
						pxls = flippedPxls;
					}
					BitmapPalette pal = new BitmapPalette(new Color[4] {
							Colors.Transparent,
							windows.tilePicker.sprPalettes[obj.states[0].frames[0].attrs[y][x] & 0b11][1],
							windows.tilePicker.sprPalettes[obj.states[0].frames[0].attrs[y][x] & 0b11][2],
							windows.tilePicker.sprPalettes[obj.states[0].frames[0].attrs[y][x] & 0b11][3]
						});
					bmp = BitmapSource.Create(8, 8, 2, 2, PixelFormats.Indexed2, pal, pxls, 2);

					objDg.Children.Add(new ImageDrawing(bmp, new Rect(x * 8, y * 8, 8, 8)));
				}
			}

			// if obj, not player
			if (combo_objs.SelectedIndex > 0)
			{
				Image objImg = new Image()
				{
					Width = obj.states[0].width * 8,
					Height = obj.states[0].height * 8,
					Margin = new Thickness(gridX, gridY, 0, 0),
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Top,
					Source = objDi,
					Name = "img_obj_" + textbox_batch.Text + "_" + objs[int.Parse(textbox_batch.Text)].Count
				};
				RenderOptions.SetBitmapScalingMode(objImg, BitmapScalingMode.NearestNeighbor);
				objImg.MouseDown += new MouseButtonEventHandler(img_obj_MouseDown);
				((Grid)grid_objs.Children[int.Parse(textbox_batch.Text) + 1]).Children.Add(objImg);

				// add obj to list of objects
				if (!dontAddToArray)
				{
					objs[int.Parse(textbox_batch.Text)].Add(new PlaceObj()
					{
						type = (byte)combo_objs.SelectedIndex,
						x = (byte)(gridX / 16),
						y = (byte)(gridY / 16)
					});
				}

				UpdateRemObjsIObjs();
			}
			// if player
			else
			{
				img_plr.Width = obj.states[0].width * 8;
				img_plr.Height = obj.states[0].height * 8;
				img_plr.Margin = new Thickness(gridX, gridY, 0, 0);
				img_plr.HorizontalAlignment = HorizontalAlignment.Left;
				img_plr.VerticalAlignment = VerticalAlignment.Top;
				img_plr.Source = objDi;
				plrX = (int)gridX;
				plrY = (int)gridY;
			}
		}
		public void StartIObj(double mouseX, double mouseY, bool leftButton, bool constrain = true, bool visFirst = true)
		{
			if (constrain && (mouseX < 0 || mouseY < 0 || mouseX > img_map.ActualWidth || mouseY > img_map.ActualHeight || !leftButton))
				return;

			double left = ((int)mouseX / 16) * 16;
			double top = ((int)mouseY / 16) * 16;

			Rectangle newRect = new Rectangle()
			{
				Margin = new Thickness(left, top, 0, 0),
				VerticalAlignment = VerticalAlignment.Top,
				HorizontalAlignment = HorizontalAlignment.Left,
				Width = 16,
				Height = 16,
				Fill = new SolidColorBrush(Color.FromArgb(128, 0, 200, 60)),
				StrokeThickness = 1,
				Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 200, 60))
			};
			newRect.MouseDown += new MouseButtonEventHandler(rect_iobj_MouseDown);
			newRect.MouseMove += new MouseEventHandler(rect_iobj_MouseMove);
			newRect.MouseLeave += new MouseEventHandler(rect_iobj_MouseLeave);
			currIObjRect = newRect;
			currIObjStartPos = new Point(left, top);

			PlaceObj newObj = new PlaceObj()
			{
				type = (byte)combo_iobjs.SelectedIndex,
				x = (byte)(left / 16),
				y = (byte)(top / 16),
				width = 1,
				height = 1
			};
			
			// if it's a visible i-obj, put it first
			if ((bool)((CheckBox)((StackPanel)windows.iobjs.panel_iobjs.Children[combo_iobjs.SelectedIndex]).Children[5]).IsChecked && visFirst)
			{
				grid_iobjs.Children.Insert(0, newRect);
				iobjs.Insert(0, newObj);
				numVisibles++;
				currIobjType = 0;
			}
			else
			{
				// add to either slopes or ladders
				if (((string)combo_iobjs.SelectedItem).ToLower().Contains("slope"))
				{
					grid_iobjs.Children.Insert(numVisibles, newRect);
					iobjs.Insert(numVisibles, newObj);
					numSlopes++;
					currIobjType = 1;
				}
				else if (((string)combo_iobjs.SelectedItem).ToLower().Contains("ladder"))
				{
					grid_iobjs.Children.Insert(numVisibles + numSlopes, newRect);
					iobjs.Insert(numVisibles + numSlopes, newObj);
					numLadders++;
					currIobjType = 2;
				}
				else
				{
					grid_iobjs.Children.Add(newRect);
					iobjs.Add(newObj);
					currIobjType = 3;
				}
			}

			UpdateRemObjsIObjs();
		}
		public void SizeIObj(double mouseX, double mouseY, bool constrain = true, bool visFirst = true)
		{
			if (constrain && (mouseX < 0 || mouseY < 0 || mouseX > img_map.ActualWidth || mouseY > img_map.ActualHeight || currIObjRect == null))
				return;

			double left = ((int)mouseX / 16) * 16;
			double top = ((int)mouseY / 16) * 16;

			// x
			if (left > currIObjStartPos.X)
				currIObjRect.Width =
					Math.Min(
						((left - currIObjRect.Margin.Left) + 16),
						16 * 15
					);
			else
			{
				currIObjRect.Margin = new Thickness(
					Math.Max(
						left,
						currIObjStartPos.X - (14 * 16)
					), currIObjRect.Margin.Top, 0, 0);
				currIObjRect.Width = 
					Math.Min(
						(currIObjStartPos.X - left) + 16,
						16 * 15
					);
			}

			// y
			if (top > currIObjStartPos.Y)
				currIObjRect.Height = 
					Math.Min(
						((top - currIObjRect.Margin.Top) + 16),
						16 * 15
					);
			else
			{
				currIObjRect.Margin = new Thickness(currIObjRect.Margin.Left,
					Math.Max(
						top,
						currIObjStartPos.Y - (14 * 16)
					), 0, 0);
				currIObjRect.Height =
					Math.Min(
						(currIObjStartPos.Y - top) + 16,
						16 * 15
					);
			}

			switch (currIobjType)
            {
				case 0: // visible
					iobjs.First().x = (byte)(currIObjRect.Margin.Left / 16);
					iobjs.First().y = (byte)(currIObjRect.Margin.Top / 16);
					iobjs.First().width = (byte)(currIObjRect.Width / 16);
					iobjs.First().height = (byte)(currIObjRect.Height / 16);
					break;
				case 1: // slopes
					iobjs[numVisibles].x = (byte)(currIObjRect.Margin.Left / 16);
					iobjs[numVisibles].y = (byte)(currIObjRect.Margin.Top / 16);
					iobjs[numVisibles].width = (byte)(currIObjRect.Width / 16);
					iobjs[numVisibles].height = (byte)(currIObjRect.Height / 16);
					break;
				case 2: // ladders
					iobjs[numVisibles + numSlopes].x = (byte)(currIObjRect.Margin.Left / 16);
					iobjs[numVisibles + numSlopes].y = (byte)(currIObjRect.Margin.Top / 16);
					iobjs[numVisibles + numSlopes].width = (byte)(currIObjRect.Width / 16);
					iobjs[numVisibles + numSlopes].height = (byte)(currIObjRect.Height / 16);
					break;
				default: // other
					iobjs.Last().x = (byte)(currIObjRect.Margin.Left / 16);
					iobjs.Last().y = (byte)(currIObjRect.Margin.Top / 16);
					iobjs.Last().width = (byte)(currIObjRect.Width / 16);
					iobjs.Last().height = (byte)(currIObjRect.Height / 16);
					break;
            }
		}
		public void DrawCamLead(double mouseX, double mouseY, bool leftButton, bool constrain = true)
		{
			if (constrain && (mouseX < 0 || mouseY < 0 || mouseX > img_map.ActualWidth || mouseY > img_map.ActualHeight))
				return;

			double left, top;
			if (constrain)
			{
				left =
						Math.Max(
							Math.Min(
								((int)(mouseX - 128) / 16) * 16,
								img_map.ActualWidth - 256
							),
							0
						);
				top =
					Math.Max(
						Math.Min(
							((int)(mouseY - 120) / 16) * 16,
							img_map.ActualHeight - 240
						),
						0
					);
			}
			else
			{
				left = ((int)mouseX / 16) * 16;
				top = ((int)mouseY / 16) * 16;
			}

			if (leftButton)
			{
				Rectangle newRect = new Rectangle()
				{
					Margin = new Thickness(left, top, 0, 0),
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Left,
					Width = 256,
					Height = 240,
					Fill = (bool)checkbox_cin_loc.IsChecked ? new SolidColorBrush(Color.FromArgb(64, 255, 255, 255)) : new SolidColorBrush(Colors.Transparent),
					StrokeThickness = 1,
					Stroke = Brushes.White
				};
				newRect.MouseDown += new MouseButtonEventHandler(rect_camlead_MouseDown);
				newRect.MouseMove += new MouseEventHandler(rect_camlead_MouseMove);
				newRect.MouseLeave += new MouseEventHandler(rect_camlead_MouseLeave);
				camLeads.Add(new CamLead()
				{
					x = (byte)(left / 16),
					y = (byte)(top / 16),
					batch = byte.Parse(textbox_batch.Text),
					act = byte.Parse(textbox_cin_chain.Text),
					loc = (bool)checkbox_cin_loc.IsChecked
				});
				grid_camleads.Children.Add(newRect);
			}
		}

		// window
		private void window_map_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && grid_top_bar.IsMouseOver)
				DragMove();
		}
		private void window_map_KeyDown(object sender, KeyEventArgs e)
		{
			bool ctrlDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
			bool shiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
			if (ctrlDown || shiftDown)
				return;
			if ((bool)windows.main.checkbox_meta.IsChecked)
			{
				if (e.Key == Key.W)
					windows.meta.MetaCursorUp();
				else if (e.Key == Key.A)
					windows.meta.MetaCursorLeft();
				else if (e.Key == Key.S)
					windows.meta.MetaCursorDown();
				else if (e.Key == Key.D)
					windows.meta.MetaCursorRight();
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
			else if (e.Key == Key.M || e.Key == Key.N || e.Key == Key.B)
				UpdateSelRect(Mouse.GetPosition(img_map).X, Mouse.GetPosition(img_map).Y);
			else if (e.Key == Key.R)
				panel_drawing.Visibility = panel_drawing.IsVisible ? Visibility.Collapsed : Visibility.Visible;
			else if (e.Key == Key.H)
			{
				Console.WriteLine(plrX + ", " + plrY);
			}

			// enter key clicks the apply button for sizing
			else if (e.Key == Key.Enter && button_size_apply.IsEnabled)
				button_size_apply_Click(null, null);

			// directional keys for those mouseless
			else if (e.Key == Key.Up)
			{
				Canvas.SetTop(viewbox_map, Canvas.GetTop(viewbox_map) + KEYS_PAN_SPEED);
				ClampMapLoc();
				imgTopRatio = Canvas.GetTop(viewbox_map) / (canvas_map.ActualHeight - viewbox_map.ActualHeight);
				imgLeftRatio = Canvas.GetLeft(viewbox_map) / (canvas_map.ActualWidth - viewbox_map.ActualWidth);
			}
			else if (e.Key == Key.Down)
			{
				Canvas.SetTop(viewbox_map, Canvas.GetTop(viewbox_map) - KEYS_PAN_SPEED);
				ClampMapLoc();
				imgTopRatio = Canvas.GetTop(viewbox_map) / (canvas_map.ActualHeight - viewbox_map.ActualHeight);
				imgLeftRatio = Canvas.GetLeft(viewbox_map) / (canvas_map.ActualWidth - viewbox_map.ActualWidth);
			}
			if (e.Key == Key.Left)
			{
				Canvas.SetLeft(viewbox_map, Canvas.GetLeft(viewbox_map) + KEYS_PAN_SPEED);
				ClampMapLoc();
				imgTopRatio = Canvas.GetTop(viewbox_map) / (canvas_map.ActualHeight - viewbox_map.ActualHeight);
				imgLeftRatio = Canvas.GetLeft(viewbox_map) / (canvas_map.ActualWidth - viewbox_map.ActualWidth);
			}
			else if (e.Key == Key.Right)
			{
				Canvas.SetLeft(viewbox_map, Canvas.GetLeft(viewbox_map) - KEYS_PAN_SPEED);
				ClampMapLoc();
				imgTopRatio = Canvas.GetTop(viewbox_map) / (canvas_map.ActualHeight - viewbox_map.ActualHeight);
				imgLeftRatio = Canvas.GetLeft(viewbox_map) / (canvas_map.ActualWidth - viewbox_map.ActualWidth);
			}
			else if (e.Key == Key.P)
            {
				label_color_mode.Content = numVisibles + ", " + numSlopes + ", " + numLadders;
			}
		}
		private void window_map_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.M)
				HideMeasures();
			else if (e.Key == Key.N || e.Key == Key.B)
				UpdateSelRect(Mouse.GetPosition(img_map).X, Mouse.GetPosition(img_map).Y);
		}
		private void window_map_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Canvas.SetTop(viewbox_map, (canvas_map.ActualHeight - viewbox_map.ActualHeight) * imgTopRatio);
			Canvas.SetLeft(viewbox_map, (canvas_map.ActualWidth - viewbox_map.ActualWidth) * imgLeftRatio);

			// drawing box
			Canvas.SetLeft(panel_drawing, canvas_map.ActualWidth - panel_drawing.ActualWidth - DRAWING_BOX_MARGIN);
			Canvas.SetTop(panel_drawing, canvas_map.ActualHeight - panel_drawing.ActualHeight - DRAWING_BOX_MARGIN);

			scroller_settings.Height = canvas_map.ActualHeight;

			ClampMapLoc();
		}
		private void window_map_Activated(object sender, EventArgs e)
		{
			windows.tilePicker.Topmost = true;
			windows.meta.Topmost = true;
			windows.meta.radio_picking.IsChecked = true;
			
		}
		private void window_map_Deactivated(object sender, EventArgs e)
		{
			windows.tilePicker.Topmost = false;
			windows.meta.Topmost = false;
			rect_map_sel.Visibility = Visibility.Hidden;
		}
		private void window_map_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			UpdateObjects(combo_objs.SelectedIndex);
			UpdateIObjects(combo_iobjs.SelectedIndex);
		}
		private void button_close_Click(object sender, RoutedEventArgs e)
		{
			this.Visibility = Visibility.Hidden;
		}
		private void button_max_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
		}
		private void button_min_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		// main section
		private void canvas_map_MouseDown(object sender, MouseButtonEventArgs e)
		{
			// start to move map around
			if (e.ChangedButton == MouseButton.Middle)
			{
				Mouse.Capture(canvas_map);
				dragStartPos = e.GetPosition(canvas_map);
				dragStartLeft = Canvas.GetLeft(viewbox_map);
				dragStartTop = Canvas.GetTop(viewbox_map);
				rect_map_sel.Visibility = Visibility.Hidden;
			}

			// draw tiles and attributes
			if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right)
			{
				windows.main.ChangesMade();
				if ((bool)radio_tile.IsChecked)
					DrawTile(e.GetPosition(img_map).X, e.GetPosition(img_map).Y, e.LeftButton == MouseButtonState.Pressed);
				else if ((bool)radio_palette.IsChecked)
					DrawAttr(e.GetPosition(img_map).X, e.GetPosition(img_map).Y);
				else if ((bool)radio_walls.IsChecked)
					DrawWall(e.GetPosition(img_walls).X, e.GetPosition(img_walls).Y, e.ChangedButton == MouseButton.Left);
				else if ((bool)radio_objs.IsChecked)
					DrawObj(e.GetPosition(img_map).X, e.GetPosition(img_map).Y, e.ChangedButton == MouseButton.Left);
				else if ((bool)radio_iobjs.IsChecked)
					StartIObj(e.GetPosition(img_map).X, e.GetPosition(img_map).Y, e.ChangedButton == MouseButton.Left);
				else
					DrawCamLead(e.GetPosition(img_map).X, e.GetPosition(img_map).Y, e.ChangedButton == MouseButton.Left);

			}
		}
		private void canvas_map_MouseMove(object sender, MouseEventArgs e)
		{
			// move map around
			if (dragStartPos.X != -1)
			{
				Canvas.SetLeft(viewbox_map,
					dragStartLeft + (e.GetPosition(canvas_map).X - dragStartPos.X));
				Canvas.SetTop(viewbox_map,
					dragStartTop + (e.GetPosition(canvas_map).Y - dragStartPos.Y));

				imgTopRatio = Canvas.GetTop(viewbox_map) / (canvas_map.ActualHeight - viewbox_map.ActualHeight);
				imgLeftRatio = Canvas.GetLeft(viewbox_map) / (canvas_map.ActualWidth - viewbox_map.ActualWidth);

				ClampMapLoc();
			}
			UpdateSelRect(e.GetPosition(img_map).X, e.GetPosition(img_map).Y);

			// draw tiles and attributes
			if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
			{
				if ((bool)radio_tile.IsChecked)
					DrawTile(e.GetPosition(img_map).X, e.GetPosition(img_map).Y, e.LeftButton == MouseButtonState.Pressed);
				else if ((bool)radio_palette.IsChecked)
					DrawAttr(e.GetPosition(img_map).X, e.GetPosition(img_map).Y);
				else if ((bool)radio_walls.IsChecked)
					DrawWall(e.GetPosition(img_walls).X, e.GetPosition(img_walls).Y, e.LeftButton == MouseButtonState.Pressed);
				else if ((bool)radio_iobjs.IsChecked)
					SizeIObj(e.GetPosition(img_map).X, e.GetPosition(img_map).Y);
			}
		}
		private void canvas_map_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Middle)
			{
				Mouse.Capture(null);
				dragStartPos.X = -1;
			}
			currIObjRect = null;
		}
		private void canvas_map_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			double mousePercentX = e.GetPosition(viewbox_map).X / viewbox_map.ActualWidth;
			double mousePercentY = e.GetPosition(viewbox_map).Y / viewbox_map.ActualHeight;
			zoom *= e.Delta > 0 ? ZOOM_PERCENT : 1 / ZOOM_PERCENT;
			UpdateZoom(mousePercentX, mousePercentY);
		}
		private void img_map_MouseLeave(object sender, MouseEventArgs e)
		{
			rect_map_sel.Visibility = Visibility.Hidden;
			HideMeasures();
		}
		private void img_obj_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Right && (bool)radio_objs.IsChecked)
			{
				Grid batch = ((Image)sender).Parent as Grid;
				int batchInd = grid_objs.Children.IndexOf(batch) - 1;
				int objInd = batch.Children.IndexOf(sender as Image);
				batch.Children.Remove(sender as Image);

				// code-behind version
				objs[batchInd].RemoveAt(objInd);

				UpdateRemObjsIObjs();
			}
		}
		private void rect_iobj_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Right && (bool)radio_iobjs.IsChecked)
			{
				int ind = grid_iobjs.Children.IndexOf(sender as Rectangle);
				grid_iobjs.Children.Remove(sender as Rectangle);

				// decrement visible, slopes or ladders
				if ((bool)((CheckBox)((StackPanel)windows.iobjs.panel_iobjs.Children[iobjs[ind].type]).Children[5]).IsChecked)
					numVisibles--;
				else if (((string)combo_iobjs.Items[iobjs[ind].type]).ToLower().Contains("slope"))
					numSlopes--;
				else if (((string)combo_iobjs.Items[iobjs[ind].type]).ToLower().Contains("ladder"))
					numLadders--;
				
				iobjs.RemoveAt(ind);
				UpdateRemObjsIObjs();
			}
		}
		private void rect_iobj_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				return;
			panel_measure.Visibility = Visibility.Visible;

			int ind = grid_iobjs.Children.IndexOf(sender as Rectangle);
			text_measure.Text = ((TextBox)((StackPanel)windows.iobjs.panel_iobjs.Children[iobjs[ind].type]).Children[1]).Text;

			// set position of measure window
			Canvas.SetLeft(panel_measure,
				Math.Min(
					Canvas.GetLeft(viewbox_map) + (((Rectangle)sender).Margin.Left * zoom),
					canvas_map.ActualWidth - panel_measure.ActualWidth
				)
			);
			Canvas.SetTop(panel_measure, (Canvas.GetTop(viewbox_map) + (((Rectangle)sender).Margin.Top * zoom)) - panel_measure.ActualHeight);
			if (Canvas.GetTop(panel_measure) < 0)
				Canvas.SetTop(panel_measure, 0);
		}
		private void rect_iobj_MouseLeave(object sender, MouseEventArgs e)
		{
			panel_measure.Visibility = Visibility.Hidden;
		}
		private void rect_camlead_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Right && (bool)radio_camleads.IsChecked)
			{
				int ind = grid_camleads.Children.IndexOf(sender as Rectangle);
				grid_camleads.Children.Remove(sender as Rectangle);
				camLeads.RemoveAt(ind);
			}
		}
		private void rect_camlead_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				return;
			panel_measure.Visibility = Visibility.Visible;

			int ind = grid_camleads.Children.IndexOf(sender as Rectangle);
			text_measure.Text = "Camera lead: " + ind.ToString() + "\nObject batch: " + camLeads[ind].batch + "\nCinematic chain: " + camLeads[ind].act;

			// set position of measure window
			Canvas.SetLeft(panel_measure,
				Math.Min(
					Canvas.GetLeft(viewbox_map) + (((Rectangle)sender).Margin.Left * zoom),
					canvas_map.ActualWidth - panel_measure.ActualWidth
				)
			);
			Canvas.SetTop(panel_measure, (Canvas.GetTop(viewbox_map) + (((Rectangle)sender).Margin.Top * zoom)) - panel_measure.ActualHeight);
			if (Canvas.GetTop(panel_measure) < 0)
				Canvas.SetTop(panel_measure, 0);
		}
		private void rect_camlead_MouseLeave(object sender, MouseEventArgs e)
		{
			panel_measure.Visibility = Visibility.Hidden;
		}

		// settings tab
		private void radio_palette_Checked(object sender, RoutedEventArgs e)
		{
			UpdateDrawingBox();
		}
		private void radio_tile_Checked(object sender, RoutedEventArgs e)
		{
			UpdateDrawingBox();
		}
		private void radio_walls_Checked(object sender, RoutedEventArgs e)
		{
			checkbox_view_grid.IsChecked = true;
			UpdateDrawingBox();
		}
		private void radio_objs_Checked(object sender, RoutedEventArgs e)
		{
			label_objs.Visibility = Visibility.Visible;
			combo_objs.Visibility = Visibility.Visible;
			label_remobjs.Visibility = Visibility.Visible;
			UpdateDrawingBox();
			checkbox_view_objs.IsChecked = true;
		}
		private void radio_objs_Unchecked(object sender, RoutedEventArgs e)
		{
			label_objs.Visibility = Visibility.Collapsed;
			combo_objs.Visibility = Visibility.Collapsed;
			label_remobjs.Visibility = Visibility.Collapsed;
		}
		private void radio_iobjs_Checked(object sender, RoutedEventArgs e)
		{
			label_iobjs.Visibility = Visibility.Visible;
			combo_iobjs.Visibility = Visibility.Visible;
			label_remiobjs.Visibility = Visibility.Visible;
			UpdateDrawingBox();
			checkbox_view_iobjs.IsChecked = true;
		}
		private void radio_iobjs_Unchecked(object sender, RoutedEventArgs e)
		{
			label_iobjs.Visibility = Visibility.Collapsed;
			combo_iobjs.Visibility = Visibility.Collapsed;
			label_remiobjs.Visibility = Visibility.Collapsed;
		}
		private void radio_camleads_Checked(object sender, RoutedEventArgs e)
		{
			panel_camleads_0.Visibility = Visibility.Visible;
			UpdateDrawingBox();
			checkbox_view_camleads.IsChecked = true;
		}
		private void radio_camleads_Unchecked(object sender, RoutedEventArgs e)
		{
			panel_camleads_0.Visibility = Visibility.Collapsed;
		}
		private void textbox_cin_chain_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterNumber(sender);
			if (textbox_cin_chain.Text == "")
				textbox_cin_chain.Text = "0";
		}
		private void textbox_width_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterNumber(sender);
			if (textbox_width.Text != mapWidth.ToString())
			{
				UpdateAnchorButtons();
				button_size_apply.IsEnabled = true;
				button_size_revert.IsEnabled = true;
			}
		}
		private void textbox_height_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterNumber(sender);
			if (textbox_height.Text != mapHeight.ToString())
			{
				UpdateAnchorButtons();
				button_size_apply.IsEnabled = true;
				button_size_revert.IsEnabled = true;
			}
		}
		private void grid_anchors_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (button_size_apply.IsEnabled)
			{
				if (e.GetPosition(grid_anchors).X < ((Grid)sender).ActualWidth / 2)
					anchorLeft = true;
				else
					anchorLeft = false;
				if (e.GetPosition(grid_anchors).Y < ((Grid)sender).ActualHeight / 2)
					anchorUp = true;
				else
					anchorUp = false;
				UpdateAnchorButtons();
			}
		}
		private void grid_anchors_MouseLeave(object sender, MouseEventArgs e)
		{
			if (button_size_apply.IsEnabled)
				UpdateAnchorButtons();
		}
		private void grid_anchors_MouseMove(object sender, MouseEventArgs e)
		{
			if (button_size_apply.IsEnabled)
			{
				UpdateAnchorButtons();
				if (e.GetPosition(grid_anchors).X < ((Grid)sender).ActualWidth / 2)
				{
					if (e.GetPosition(grid_anchors).Y < ((Grid)sender).ActualHeight / 2)
						anchor_ul.Fill = anchor_ul.TryFindResource("brush_light3") as Brush;
					else
						anchor_dl.Fill = anchor_dl.TryFindResource("brush_light3") as Brush;
				}
				else
				{
					if (e.GetPosition(grid_anchors).Y < ((Grid)sender).ActualHeight / 2)
						anchor_ur.Fill = anchor_ur.TryFindResource("brush_light3") as Brush;
					else
						anchor_dr.Fill = anchor_dr.TryFindResource("brush_light3") as Brush;
				}
			}
		}
		private void button_size_apply_Click(object sender, RoutedEventArgs e)
		{
			// disable those Size section forms
			button_size_apply.IsEnabled = false;
			button_size_revert.IsEnabled = false;
			ResetAnchorButtons();

			// not allow odd numbers (if we're in 8x8 mode)
			if (!(bool)windows.main.checkbox_meta.IsChecked)
			{
				if (int.Parse(textbox_width.Text) % 2 != 0 || int.Parse(textbox_height.Text) % 2 != 0)
				{
					Popup noOddPopup = new Popup("Error", "Odd number width/height not allowed!", "", "OK");
					noOddPopup.ShowDialog();
					button_size_revert_Click(null, null);
					return;
				}
			}

			int newWidth = (bool)windows.main.checkbox_meta.IsChecked ? int.Parse(textbox_width.Text) * 2 : int.Parse(textbox_width.Text);
			int newHeight = (bool)windows.main.checkbox_meta.IsChecked ? int.Parse(textbox_height.Text) * 2 : int.Parse(textbox_height.Text);

			// no smaller than a nametable
			if (newWidth < 32 || newHeight < 30)
			{
				Popup tooSmallPopup = new Popup("Error", "Map must be at least the size of one NES nametable!  (32 8x8 tiles wide, 30 8x8 tiles high)", "", "OK");
				tooSmallPopup.ShowDialog();
				button_size_revert_Click(null, null);
				return;
			}

			// anchor the visible tile image inside of the larger tile array (set tileXOffs etc.)
			int restXOffs = 0;
			int restYOffs = 0;
			if (!anchorLeft)
			{
				tileXOffs += mapWidth - newWidth; // oldWidth - newWidth
				if (tileXOffs < 0)
				{
					restXOffs = -tileXOffs; // use this remainder to grow the tile array to the left
					tileXOffs = 0;
				}
			}
			if (!anchorUp)
			{
				tileYOffs += mapHeight - newHeight;
				if (tileYOffs < 0)
				{
					restYOffs = -tileYOffs;
					tileYOffs = 0;
				}
			}

			int fullWidth = newWidth + tileXOffs + restXOffs;
			int fullHeight = newHeight + tileYOffs;

			// resize map to dimensions given
			if (tiles.Length < fullHeight)
			{
				int[][] newRows = new int[fullHeight][];
				tiles.CopyTo(newRows, restYOffs);
				tiles = newRows;
			}
			for (int y = 0; y < fullHeight; y++)
			{
				if (tiles[y] == null)
					tiles[y] = new int[fullWidth];
				if (tiles[y].Length < fullWidth)
				{
					int[] newRow = new int[fullWidth];
					tiles[y].CopyTo(newRow, restXOffs);
					tiles[y] = newRow;
				}
			}

			// metatiles
			if (metas != null)
			{
				if (metas.Length < fullHeight / 2)
				{
					int[][] newRows = new int[fullHeight / 2][];
					metas.CopyTo(newRows, restYOffs / 2);
					metas = newRows;
				}
				for (int y = 0; y < fullHeight / 2; y++)
				{
					if (metas[y] == null)
						metas[y] = new int[fullWidth / 2];
					if (metas[y].Length < fullWidth / 2)
					{
						int[] newRow = new int[fullWidth / 2];
						metas[y].CopyTo(newRow, restXOffs / 2);
						metas[y] = newRow;
					}
				}
			}

			// walls
			if (walls.Length < fullHeight / 2)
			{
				bool[][] newRows = new bool[fullHeight / 2][];
				walls.CopyTo(newRows, restYOffs / 2);
				walls = newRows;
			}
			for (int y = 0; y < fullHeight / 2; y++)
			{
				if (walls[y] == null)
					walls[y] = new bool[fullWidth / 2];
				if (walls[y].Length < fullWidth / 2)
				{
					bool[] newRow = new bool[fullWidth / 2];
					walls[y].CopyTo(newRow, restXOffs / 2);
					walls[y] = newRow;
				}
			}

			// attributes
			if (attrs.Length < fullHeight / 2)
			{
				int[][] newRows = new int[fullHeight / 2][];
				attrs.CopyTo(newRows, restYOffs / 2);
				attrs = newRows;
			}
			for (int y = 0; y < fullHeight / 2; y++)
			{
				if (attrs[y] == null)
					attrs[y] = new int[fullWidth / 2];
				if (attrs[y].Length < fullWidth / 2)
				{
					int[] newRow = new int[fullWidth / 2];
					attrs[y].CopyTo(newRow, restXOffs / 2);
					attrs[y] = newRow;
				}
			}

			// objects
			for (int b = 0; b < objs.Count; b++)
			{
				textbox_batch.Text = b.ToString();
				for (int o = 0; o < objs[b].Count; o++)
				{
					if (!anchorLeft)
						objs[b][o].x += (byte)((newWidth - mapWidth) / 2);
					if (!anchorUp)
						objs[b][o].y += (byte)((newHeight - mapHeight) / 2);
					((Image)((Grid)grid_objs.Children[b + 1]).Children[o]).Margin = new Thickness(
						objs[b][o].x * 16,
						objs[b][o].y * 16,
						0,
						0
					);
				}
			}
			// player
			if (!anchorLeft)
				plrX += (newWidth - mapWidth) * 8;
			if (!anchorUp)
				plrY += (newHeight - mapHeight) * 8;
			combo_objs.SelectedIndex = 0;
			DrawObj(plrX, plrY, true, false);
			// iobjs
			for (int i = 0; i < iobjs.Count; i++)
			{
				if (!anchorLeft)
					iobjs[i].x += (byte)((newWidth - mapWidth) / 2);
				if (!anchorUp)
					iobjs[i].y += (byte)((newHeight - mapHeight) / 2);
				((Rectangle)grid_iobjs.Children[i]).Margin = new Thickness(
					iobjs[i].x * 16,
					iobjs[i].y * 16,
					0,
					0
				);
			}
			// camleads
			for (int c = 0; c < camLeads.Count; c++)
			{
				if (!anchorLeft)
					camLeads[c].x += (byte)((newWidth - mapWidth) / 2);
				if (!anchorUp)
					camLeads[c].y += (byte)((newHeight - mapHeight) / 2);
				((Rectangle)grid_camleads.Children[c]).Margin = new Thickness(
					camLeads[c].x * 16,
					camLeads[c].y * 16,
					0,
					0
				);
			}

			mapWidth = newWidth;
			mapHeight = newHeight;
			InitMap();
			UpdateZoom(anchorLeft ? 0 : 1, anchorUp ? 0 : 1);
		}
		public void button_size_revert_Click(object sender, RoutedEventArgs e)
		{
			textbox_width.Text = (bool)windows.main.checkbox_meta.IsChecked ? (mapWidth / 2).ToString() : mapWidth.ToString();
			textbox_height.Text = (bool)windows.main.checkbox_meta.IsChecked ? (mapHeight / 2).ToString() : mapHeight.ToString();
			button_size_apply.IsEnabled = false;
			button_size_revert.IsEnabled = false;
			ResetAnchorButtons();
		}
		private void ToggleSettingsTab()
		{
			board.Remove();
			if (Canvas.GetLeft(panel_settings) == 0)
			{
				board.Children.Add(new DoubleAnimation()
				{
					From = 0,
					To = -256,
					Duration = new Duration(TimeSpan.FromSeconds(0.4)),
					EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut }
				});
			}
			else
			{
				board.Children.Add(new DoubleAnimation()
				{
					From = -256,
					To = 0,
					Duration = new Duration(TimeSpan.FromSeconds(0.4)),
					EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut }
				});
			}
			Storyboard.SetTarget(board, panel_settings);
			Storyboard.SetTargetProperty(board, new PropertyPath(Canvas.LeftProperty));
			board.Begin();
		}
		private void button_zoom_reset_Click(object sender, RoutedEventArgs e)
		{
			zoom = ZOOM_START;
			UpdateZoom(0.5, 0.5);
		}
		private void button_fill_tile_Click(object sender, RoutedEventArgs e)
		{
			for (int y = 0; y < mapHeight; y++)
			{
				for (int x = 0; x < mapWidth; x++)
				{
					DrawTile(x * 8, y * 8, true);
				}
			}
		}
		private void button_fill_pal_Click(object sender, RoutedEventArgs e)
		{
			for (int y = 0; y < mapHeight / 2; y++)
			{
				for (int x = 0; x < mapWidth / 2; x++)
				{
					DrawAttr(x * 16, y * 16);
				}
			}
		}
		private void button_fill_wall_Click(object sender, RoutedEventArgs e)
		{
			for (int y = 0; y < mapHeight / 2; y++)
			{
				for (int x = 0; x < mapWidth / 2; x++)
				{
					walls[y][x] = true;
				}
			}
			InitMap();
		}
		private void button_clear_wall_Click(object sender, RoutedEventArgs e)
		{
			for (int y = 0; y < mapHeight / 2; y++)
			{
				for (int x = 0; x < mapWidth / 2; x++)
				{
					walls[y][x] = false;
				}
			}
			InitMap();
		}
		private void checkbox_attr_from_meta_Checked(object sender, RoutedEventArgs e)
		{
			radio_palette.IsEnabled = false;
			radio_tile.IsChecked = true;
			UpdateDrawingBox();
		}
		private void checkbox_attr_from_meta_Unchecked(object sender, RoutedEventArgs e)
		{
			radio_palette.IsEnabled = true;
			UpdateDrawingBox();
		}
		private void checkbox_view_grid_Checked(object sender, RoutedEventArgs e)
		{
			img_walls.Visibility = Visibility.Visible;
		}
		private void checkbox_view_grid_Unchecked(object sender, RoutedEventArgs e)
		{
			img_walls.Visibility = Visibility.Collapsed;
		}
		private void checkbox_view_objs_Checked(object sender, RoutedEventArgs e)
		{
			grid_objs.Visibility = Visibility.Visible;
			panel_view_obj_batches.Visibility = Visibility.Visible;
			foreach (CheckBox box in panel_view_obj_batches.Children)
				if ((bool)box.IsChecked)
					return;
			((CheckBox)panel_view_obj_batches.Children[0]).IsChecked = true;
		}
		private void checkbox_view_objs_Unchecked(object sender, RoutedEventArgs e)
		{
			grid_objs.Visibility = Visibility.Collapsed;
			panel_view_obj_batches.Visibility = Visibility.Collapsed;
		}
		private void checkbox_view_iobjs_Checked(object sender, RoutedEventArgs e)
		{
			grid_iobjs.Visibility = Visibility.Visible;
		}
		private void checkbox_view_iobjs_Unchecked(object sender, RoutedEventArgs e)
		{
			grid_iobjs.Visibility = Visibility.Collapsed;
		}
		private void checkbox_view_camleads_Checked(object sender, RoutedEventArgs e)
		{
			grid_camleads.Visibility = Visibility.Visible;
		}
		private void checkbox_view_camleads_Unchecked(object sender, RoutedEventArgs e)
		{
			grid_camleads.Visibility = Visibility.Collapsed;
		}
		private void textbox_batch_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (textbox_num_batches == null)
				return;
			FilterNumber(sender);
			if (textbox_batch.Text == "")
				textbox_batch.Text = "0";
			if (byte.Parse(textbox_batch.Text) >= byte.Parse(textbox_num_batches.Text))
				textbox_batch.Text = (byte.Parse(textbox_num_batches.Text) - 1).ToString();
			UpdateRemObjsIObjs();
		}
		private void textbox_num_batches_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (objs == null)
				return;
			if (textbox_num_batches.Text == "0")
				textbox_num_batches.Text = "1";
			FilterNumber(sender);
			if (textbox_num_batches.Text == "")
				textbox_num_batches.Text = "1";
			if (byte.Parse(textbox_batch.Text) >= byte.Parse(textbox_num_batches.Text))
				textbox_batch.Text = (byte.Parse(textbox_num_batches.Text) - 1).ToString();
			if (byte.Parse(textbox_num_batches.Text) > 16)
				textbox_num_batches.Text = "16";

			int numBatches = int.Parse(textbox_num_batches.Text);

			// create new batches as needed
			if (objs.Count < numBatches)
			{
				for (int i = objs.Count; i < numBatches; i++)
				{
					objs.Add(new List<PlaceObj>());
					grid_objs.Children.Add(new Grid());
					CheckBox check = new CheckBox();
					check.Content = "Batch " + i;
					check.IsChecked = true;
					check.Checked += new RoutedEventHandler(checkbox_view_batch_Checked);
					check.Unchecked += new RoutedEventHandler(checkbox_view_batch_Unchecked);
					panel_view_obj_batches.Children.Add(check);
				}
			}
			// delete batches as needed
			else if (objs.Count > numBatches)
			{
				panel_view_obj_batches.Children.RemoveRange(numBatches, objs.Count - numBatches);
				grid_objs.Children.RemoveRange(numBatches + 1, objs.Count - numBatches);
				objs.RemoveRange(numBatches, objs.Count - numBatches);
			}
		}

		private void checkbox_view_batch_Checked(object sender, RoutedEventArgs e)
		{
			string batchStr = ((CheckBox)sender).Content as string;
			int batchNum = int.Parse(batchStr.Split(' ')[1]);
			grid_objs.Children[batchNum + 1].Visibility = Visibility.Visible;
		}
		private void checkbox_view_batch_Unchecked(object sender, RoutedEventArgs e)
		{
			string batchStr = ((CheckBox)sender).Content as string;
			int batchNum = int.Parse(batchStr.Split(' ')[1]);
			grid_objs.Children[batchNum + 1].Visibility = Visibility.Collapsed;
		}

		private void checkbox_grid_any_size_Click(object sender, RoutedEventArgs e)
		{
			DrawGrid();
		}
	}
}

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
using System.Windows.Threading;

namespace Wagner
{
	/// <summary>
	/// Interaction logic for Objects.xaml
	/// </summary>
	public partial class Objects : Window
	{
		private const double ZOOM_PERCENT = 1.2;
		private const double ZOOM_START = 4.0;
		private const double ZOOM_MAX = 8.0;
		private const double ZOOM_MIN = 1.0;
		private const int MAX_NUM_FRAMES = 16;
		private readonly int[] SPEED_LOOKUP = { 544, 272, 136, 68, 34 }; // milliseconds
		public readonly byte[] MASK_LOOKUP = { 0b11111, 0b1111, 0b111, 0b11, 0b1 };

		private WindowSet windows;
		private DrawingGroup dg;
		private DrawingImage di;
		int[][] tiles;
		byte[][] attrs;
		private DispatcherTimer playTimer;

		// dragging, zooming
		private Point dragStartPos;
		private double dragStartLeft;
		private double dragStartTop;
		public double zoom = ZOOM_START;

		// backbone of this window
		public List<Objekt> objs;

		public Objects(WindowSet windows)
		{
			InitializeComponent();

			this.windows = windows;
			dragStartPos = new Point(-1, -1);

			// initiate objects with just "player"
			objs = new List<Objekt>();
			objs.Add(new Objekt() { name = "player" });
			combobox_objects.SelectedIndex = 0;
			UpdateObjects(0);
			UpdateStates(0);
			UpdateZoom(0.5, 0.5);
			RedrawObj();

			Canvas.SetLeft(viewbox_obj, 128);
			Canvas.SetTop(viewbox_obj, 128);
			UpdateBorder();
		}

		// helper methods
		public void UpdateObjects(int targSelInd)
		{
			combobox_objects.ItemsSource = objs.Select(obj => obj.name).ToArray();
			combobox_objects.SelectedIndex =
				Math.Max(
					Math.Min(targSelInd, objs.Count - 1),
					0
				);
			textbox_obj_width.Text = objs[combobox_objects.SelectedIndex].width.ToString();
			textbox_obj_height.Text = objs[combobox_objects.SelectedIndex].height.ToString();
		}
		public void UpdateStates(int targSelInd)
		{
			combobox_states.ItemsSource = objs[combobox_objects.SelectedIndex].states.Select(state => state.name).ToArray();
			combobox_states.SelectedIndex =
				Math.Max(
					Math.Min(targSelInd, objs[combobox_objects.SelectedIndex].states.Count - 1),
					0
				);
			textbox_frame.Text = "0";
			UpdateObjVisual();
			textbox_num_frames.Text = objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames.Count.ToString();
			textbox_speed.Text = objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].speed.ToString();
			textbox_state_width.Text = objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].width.ToString();
			textbox_state_height.Text = objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].height.ToString();
			textbox_state_dup.Text = objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].lst.ToString();
		}
		private void FilterNumber(object sender)
		{
			string filteredStr = System.Text.RegularExpressions.Regex.Replace(((TextBox)sender).Text, "[^0-9.]", "");
			((TextBox)sender).Text = filteredStr == "" ? "0" : filteredStr;
		}
		private void NextFrame()
		{
			int frem = int.Parse(textbox_frame.Text);
			frem++;
			if (frem >= int.Parse(textbox_num_frames.Text))
				frem = 0;
			textbox_frame.Text = frem.ToString();
			UpdateObjVisual();
		}
		private void PrevFrame()
		{
			int frem = int.Parse(textbox_frame.Text);
			frem--;
			if (frem < 0)
				frem = int.Parse(textbox_num_frames.Text) - 1;
			textbox_frame.Text = frem.ToString();
			UpdateObjVisual();
		}

		public void RedrawObj()
		{
			if (textbox_state_height.Text == "")
				return;
			dg = new DrawingGroup();
			di = new DrawingImage(dg);
			img_obj.Source = di;
			int width = int.Parse(textbox_state_width.Text);
			int height = int.Parse(textbox_state_height.Text);
			if (tiles == null)
			{
				objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames[int.Parse(textbox_frame.Text)].bmps.Clear();
				tiles = new int[height][];
				attrs = new byte[height][];
				for (int y = 0; y < height; y++)
				{
					tiles[y] = new int[width];
					attrs[y] = new byte[width];
					for (int x = 0; x < width; x++)
					{
						byte[] pxls = new byte[16];
						windows.tilePicker.bmps[0].CopyPixels(pxls, 2, 0);
						BitmapPalette pal = new BitmapPalette(new Color[4] {
							Colors.Transparent,
							windows.tilePicker.sprPalettes[0][1],
							windows.tilePicker.sprPalettes[0][2],
							windows.tilePicker.sprPalettes[0][3]
						});
						BitmapSource bmp = BitmapSource.Create(8, 8, 2, 2, PixelFormats.Indexed2, pal, pxls, 2);
						dg.Children.Add(new ImageDrawing(bmp, new Rect(x * 8, y * 8, 8, 8)));
						objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames[int.Parse(textbox_frame.Text)].bmps.Add(bmp);
					}
				}
			}
			else
			{
				objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames[int.Parse(textbox_frame.Text)].bmps.Clear();
				if (tiles.Length < height)
				{
					tiles.CopyTo(tiles = new int[height][], 0);
					attrs.CopyTo(attrs = new byte[height][], 0);
				}
				for (int y = 0; y < height; y++)
				{
					if (tiles[y] == null)
					{
						tiles[y] = new int[width];
						attrs[y] = new byte[width];
					}	
					if (tiles[y].Length < width)
					{
						tiles[y].CopyTo(tiles[y] = new int[width], 0);
						attrs[y].CopyTo(attrs[y] = new byte[width], 0);
					}
					for (int x = 0; x < width; x++)
					{
						byte[] pxls = new byte[16];
						int tileInd = (tiles[y][x] % 256) + 256;
						windows.tilePicker.bmps[tileInd].CopyPixels(pxls, 2, 0);
						// flipped vertically
						if ((attrs[y][x] & 0b10000000) != 0)
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
						if ((attrs[y][x] & 0b1000000) != 0)
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
							windows.tilePicker.sprPalettes[attrs[y][x] & 0b11][1],
							windows.tilePicker.sprPalettes[attrs[y][x] & 0b11][2],
							windows.tilePicker.sprPalettes[attrs[y][x] & 0b11][3]
						});
						BitmapSource bmp = BitmapSource.Create(8, 8, 2, 2, PixelFormats.Indexed2, pal, pxls, 2);
						dg.Children.Add(new ImageDrawing(bmp, new Rect(x * 8, y * 8, 8, 8)));
						objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames[int.Parse(textbox_frame.Text)].bmps.Add(bmp);
					}
				}

				objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames[int.Parse(textbox_frame.Text)].tiles = AnimFrame.DeepCopyDoubleInt(tiles);
				objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames[int.Parse(textbox_frame.Text)].attrs = AnimFrame.DeepCopyDoubleInt(attrs);
			}
			UpdateBorder();
		}
		private void ClampMapLoc()
		{
			Canvas.SetLeft(viewbox_obj,
				Math.Max(
					Math.Min(
						Canvas.GetLeft(viewbox_obj),
						canvas_obj.ActualWidth - (canvas_obj.ActualWidth * .25)),
					0 - (viewbox_obj.Width - (canvas_obj.ActualWidth * .25))) // leaving 25% visible
				);
			Canvas.SetTop(viewbox_obj,
				Math.Max(
					Math.Min(
						Canvas.GetTop(viewbox_obj),
						canvas_obj.ActualHeight - (canvas_obj.ActualHeight * .25)),
					0 - (viewbox_obj.Height - (canvas_obj.ActualHeight * .25))) // leaving 25% visible
				);
			UpdateBorder();
		}
		private void ClampMapZoom()
		{
			if (zoom > ZOOM_MAX)
			{
				zoom = ZOOM_MAX;
				viewbox_obj.Width = (int.Parse(textbox_state_width.Text) * 8) * ZOOM_MAX;
				viewbox_obj.Height = (int.Parse(textbox_state_height.Text) * 8) * ZOOM_MAX;
			}
			else if (zoom < ZOOM_MIN)
			{
				zoom = ZOOM_MIN;
				viewbox_obj.Width = (int.Parse(textbox_state_width.Text) * 8) * ZOOM_MIN;
				viewbox_obj.Height = (int.Parse(textbox_state_height.Text) * 8) * ZOOM_MIN;
			}
			UpdateBorder();
		}
		public void UpdateZoom(double x, double y)
		{
			if (textbox_state_height.Text == "")
				return;
			ClampMapZoom();
			viewbox_obj.Width = int.Parse(textbox_state_width.Text) * 8 * zoom;
			viewbox_obj.Height = int.Parse(textbox_state_height.Text) * 8 * zoom;
			double diffX = viewbox_obj.Width - viewbox_obj.ActualWidth;
			double diffY = viewbox_obj.Height - viewbox_obj.ActualHeight;
			Canvas.SetLeft(viewbox_obj, Canvas.GetLeft(viewbox_obj) - (diffX * x));
			Canvas.SetTop(viewbox_obj, Canvas.GetTop(viewbox_obj) - (diffY * y));

			ClampMapLoc();
			rect_map_sel.Visibility = Visibility.Hidden;
			textbox_zoom.Text = (zoom * 100).ToString("F0");
		}
		private void UpdateSelRect(double mouseX, double mouseY)
		{
			if (mouseX < 0 || mouseY < 0 || mouseX > img_obj.ActualWidth || mouseY > img_obj.ActualHeight)
			{
				rect_map_sel.Visibility = Visibility.Hidden;
				return;
			}
			int sizeX = 8;
			int sizeY = 8;
			double scale = viewbox_obj.ActualWidth / (int.Parse(textbox_state_width.Text) * 8);
			double left = (((int)mouseX / sizeX) * sizeX) * scale;
			double top = (((int)mouseY / sizeY) * sizeY) * scale;

			rect_map_sel.Visibility = Visibility.Visible;
			rect_map_sel.Width = sizeX * scale;
			rect_map_sel.Height = sizeY * scale;
			rect_map_sel.Stroke = Brushes.White;
			Canvas.SetLeft(rect_map_sel, Canvas.GetLeft(viewbox_obj) + left);
			Canvas.SetTop(rect_map_sel, Canvas.GetTop(viewbox_obj) + top);
		}
		private void UpdateBorder()
		{
			rect_obj_border.Width = viewbox_obj.Width;
			rect_obj_border.Height = viewbox_obj.Height;
			Canvas.SetLeft(rect_obj_border, Canvas.GetLeft(viewbox_obj));
			Canvas.SetTop(rect_obj_border, Canvas.GetTop(viewbox_obj));
		}
		private void DrawTile(double mouseX, double mouseY, bool left)
		{
			if (mouseX < 0 || mouseY < 0 || mouseX > img_obj.ActualWidth || mouseY > img_obj.ActualHeight)
				return;
			int x = (int)mouseX / 8;
			int y = (int)mouseY / 8;
			tiles[y][x] = left ? windows.tilePicker.selTile1 : windows.tilePicker.selTile2;

			// flipped
			attrs[y][x] = (byte)(
				(attrs[y][x] & 0b00111111) |
				((bool)checkbox_flip_hor.IsChecked ? 0b1000000 : 0) |
				((bool)checkbox_flip_ver.IsChecked ? 0b10000000 : 0)
			);

			// update frame object
			objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames[int.Parse(textbox_frame.Text)].tiles = AnimFrame.DeepCopyDoubleInt(tiles);
			objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames[int.Parse(textbox_frame.Text)].attrs = AnimFrame.DeepCopyDoubleInt(attrs);

			RedrawObj();
			windows.map.RedrawObjs();
		}
		private void DrawPal(double mouseX, double mouseY)
		{
			if (mouseX < 0 || mouseY < 0 || mouseX > img_obj.ActualWidth || mouseY > img_obj.ActualHeight)
				return;
			int x = (int)mouseX / 8;
			int y = (int)mouseY / 8;
			attrs[y][x] = (byte)((attrs[y][x] & 0b11111100) | (windows.tilePicker.selectedSprPal - 1));

			// update frame object
			objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames[int.Parse(textbox_frame.Text)].attrs = AnimFrame.DeepCopyDoubleInt(attrs);

			RedrawObj();
			windows.map.RedrawObjs();
		}
		public void UpdateObjVisual()
		{
			tiles = objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames[int.Parse(textbox_frame.Text)].tiles;
			attrs = objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames[int.Parse(textbox_frame.Text)].attrs;
			RedrawObj();
		}
		private void PlayTimerElapsed(object sender, EventArgs e)
		{
			NextFrame();
		}

		// window
		private void button_close_Click(object sender, RoutedEventArgs e)
		{
			Hide();
		}
		private void window_objects_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && grid_top_bar.IsMouseOver)
				DragMove();
		}
		private void window_objects_KeyDown(object sender, KeyEventArgs e)
		{
			bool ctrlDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
			bool shiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
			if (!ctrlDown && !shiftDown)
			{
				if (e.Key == Key.Z)
					PrevFrame();
				else if (e.Key == Key.C)
					NextFrame();
				else if (e.Key == Key.E)
					checkbox_flip_hor.IsChecked = (bool)checkbox_flip_hor.IsChecked ? false : true;
				else if (e.Key == Key.Q)
					checkbox_flip_ver.IsChecked = (bool)checkbox_flip_ver.IsChecked ? false : true;
				else if (e.Key == Key.W)
					windows.tilePicker.TileCursorUp();
				else if (e.Key == Key.A)
					windows.tilePicker.TileCursorLeft();
				else if (e.Key == Key.S)
					windows.tilePicker.TileCursorDown();
				else if (e.Key == Key.D)
					windows.tilePicker.TileCursorRight();
				else if (e.Key == Key.Space)
					button_play_stop_Click(null, null);
			}
		}

		// settings panel (left)
		private void button_new_obj_Click(object sender, RoutedEventArgs e)
		{
			Popup newObjPopup = new Popup("New object", "Name:");
			if ((bool)newObjPopup.ShowDialog())
			{
				// first check that there's not already an object with that name
				foreach (Objekt obj in objs)
				{
					if (obj.name == newObjPopup.textbox_main.Text)
					{
						Popup alreadyPopup = new Popup("Error", "There's already an object called \"" + newObjPopup.textbox_main.Text + "\".", "", "OK");
						alreadyPopup.ShowDialog();
						return;
					}
				}
				int newInd = combobox_objects.SelectedIndex + 1;
				objs.Insert(newInd, new Objekt() { name = newObjPopup.textbox_main.Text });
				UpdateObjects(newInd);
				UpdateStates(0);
			}
		}
		private void button_remove_obj_Click(object sender, RoutedEventArgs e)
		{
			Popup surePopup = new Popup("Remove object", "Are you sure you want to remove the object \"" + objs[combobox_objects.SelectedIndex].name + "\"?", "Yes");
			if ((bool)surePopup.ShowDialog())
			{
				if (combobox_objects.SelectedIndex == 0)
				{
					Popup noDelPopup = new Popup("Error", "You cannot delete the player object.", "");
					noDelPopup.ShowDialog();
					return;
				}
				objs.RemoveAt(combobox_objects.SelectedIndex);
				UpdateObjects(combobox_objects.SelectedIndex);
				UpdateStates(0);
			}
		}
		private void button_rename_obj_Click(object sender, RoutedEventArgs e)
		{
			Popup renameObjPopup = new Popup("Rename object", "Name:");
			renameObjPopup.textbox_main.Text = objs[combobox_objects.SelectedIndex].name;
			renameObjPopup.textbox_main.SelectAll();
			if ((bool)renameObjPopup.ShowDialog())
			{
				// first check that there's not already an object with that name
				foreach (Objekt obj in objs)
				{
					if (obj.name == renameObjPopup.textbox_main.Text)
					{
						Popup alreadyPopup = new Popup("Error", "There's already an object called \"" + renameObjPopup.textbox_main.Text + "\".", "", "OK");
						alreadyPopup.ShowDialog();
						return;
					}
				}
				objs[combobox_objects.SelectedIndex].name = renameObjPopup.textbox_main.Text;
				UpdateObjects(combobox_objects.SelectedIndex);
			}
		}
		private void combobox_objects_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (combobox_objects.SelectedIndex >= 0)
			{
				UpdateObjects(combobox_objects.SelectedIndex);
				UpdateStates(0);
			}
		}
		private void textbox_obj_width_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterNumber(sender);
			if (byte.Parse(textbox_obj_width.Text) < 1)
			{
				Popup tooSmallPopup = new Popup("Error", "Objects must have a width of at least 1.", "");
				tooSmallPopup.ShowDialog();
				textbox_obj_width.Text = objs[combobox_objects.SelectedIndex].width.ToString();
			}
			objs[combobox_objects.SelectedIndex].width = byte.Parse(textbox_obj_width.Text);
		}
		private void textbox_obj_height_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterNumber(sender);
			if (byte.Parse(textbox_obj_height.Text) < 1)
			{
				Popup tooSmallPopup = new Popup("Error", "Objects must have a height of at least 1.", "");
				tooSmallPopup.ShowDialog();
				textbox_obj_height.Text = objs[combobox_objects.SelectedIndex].height.ToString();
			}
			objs[combobox_objects.SelectedIndex].height = byte.Parse(textbox_obj_height.Text);
		}
		private void checkbox_second_page_Click(object sender, RoutedEventArgs e)
		{
			RedrawObj();
		}
		private void button_zoom_reset_Click(object sender, RoutedEventArgs e)
		{
			zoom = ZOOM_START;
			UpdateZoom(0.5, 0.5);
		}

		// state panel (below)
		private void textbox_num_frames_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterNumber(sender);
			if (int.Parse(textbox_num_frames.Text) > MAX_NUM_FRAMES)
				textbox_num_frames.Text = MAX_NUM_FRAMES.ToString();
			if (int.Parse(textbox_frame.Text) >= int.Parse(textbox_num_frames.Text))
				textbox_frame.Text = (int.Parse(textbox_num_frames.Text) - 1).ToString();
			else if (int.Parse(textbox_num_frames.Text) < 1)
				textbox_frame.Text = "1";

			if (textbox_num_frames.Text == "1")
				button_play_stop.IsEnabled = false;
			else
				button_play_stop.IsEnabled = true;

			// create new frames if needed
			if (objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames.Count < int.Parse(textbox_num_frames.Text))
			{
				for (int i = objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames.Count; i < int.Parse(textbox_num_frames.Text); i++)
				{
					objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames.Add(new AnimFrame());
					objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].frames[i].InitiateArrays(int.Parse(textbox_state_width.Text), int.Parse(textbox_state_height.Text));
				}
			}
		}
		private void textbox_speed_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterNumber(sender);
			if (int.Parse(textbox_speed.Text) >= SPEED_LOOKUP.Length)
				textbox_speed.Text = (SPEED_LOOKUP.Length - 1).ToString();
			objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].speed = byte.Parse(textbox_speed.Text);
		}
		private void textbox_state_height_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterNumber(sender);
			if (int.Parse(textbox_state_height.Text) < 1)
				textbox_state_height.Text = "1";
			else if (int.Parse(textbox_state_height.Text) > 255)
				textbox_state_height.Text = "255";
			objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].height = byte.Parse(textbox_state_height.Text);
			RedrawObj();
			UpdateZoom(0.5, 0.5);
		}
		private void textbox_state_width_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterNumber(sender);
			if (int.Parse(textbox_state_width.Text) < 1)
				textbox_state_width.Text = "1";
			else if (int.Parse(textbox_state_width.Text) > 255)
				textbox_state_width.Text = "255";
			objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].width = byte.Parse(textbox_state_width.Text);
			RedrawObj();
			UpdateZoom(0.5, 0.5);
		}
		private void textbox_state_dup_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterNumber(sender);
			if (int.Parse(textbox_state_dup.Text) < 1)
				textbox_state_dup.Text = "1";
			else if (int.Parse(textbox_state_dup.Text) > 255)
				textbox_state_dup.Text = "255";
			objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].lst = byte.Parse(textbox_state_dup.Text);
		}
		private void button_new_state_Click(object sender, RoutedEventArgs e)
		{
			Popup newStatePopup = new Popup("New state", "Name:");
			if ((bool)newStatePopup.ShowDialog())
			{
				// first check that there's not already an object with that name
				foreach (AnimState state in objs[combobox_objects.SelectedIndex].states)
				{
					if (state.name == newStatePopup.textbox_main.Text)
					{
						Popup alreadyPopup = new Popup("Error", "There's already a state called \"" + newStatePopup.textbox_main.Text + "\".", "", "OK");
						alreadyPopup.ShowDialog();
						return;
					}
				}
				objs[combobox_objects.SelectedIndex].states.Add(new AnimState(objs[combobox_objects.SelectedIndex]) { name = newStatePopup.textbox_main.Text });
				UpdateStates(objs[combobox_objects.SelectedIndex].states.Count - 1);
			}
		}
		private void button_remove_state_Click(object sender, RoutedEventArgs e)
		{
			Popup surePopup = new Popup("Remove state", "Are you sure you want to remove the state \"" + objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].name + "\"?", "Yes");
			if ((bool)surePopup.ShowDialog())
			{
				if (objs[combobox_objects.SelectedIndex].states.Count == 1)
				{
					Popup noDelPopup = new Popup("Error", "There must be at least one state for each object.", "");
					noDelPopup.ShowDialog();
					return;
				}
				objs[combobox_objects.SelectedIndex].states.RemoveAt(combobox_states.SelectedIndex);
				UpdateStates(combobox_states.SelectedIndex);
			}
		}
		private void button_rename_state_Click(object sender, RoutedEventArgs e)
		{
			Popup renameStatePopup = new Popup("Rename state", "Name:");
			renameStatePopup.textbox_main.Text = combobox_states.SelectedItem as string;
			renameStatePopup.textbox_main.SelectAll();
			if ((bool)renameStatePopup.ShowDialog())
			{
				// first check that there's not already an object with that name
				foreach (AnimState state in objs[combobox_objects.SelectedIndex].states)
				{
					if (state.name == renameStatePopup.textbox_main.Text)
					{
						Popup alreadyPopup = new Popup("Error", "There's already a state called \"" + renameStatePopup.textbox_main.Text + "\".", "", "OK");
						alreadyPopup.ShowDialog();
						return;
					}
				}
				objs[combobox_objects.SelectedIndex].states[combobox_states.SelectedIndex].name = renameStatePopup.textbox_main.Text;
				UpdateStates(combobox_states.SelectedIndex);
			}
		}
		private void combobox_states_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (combobox_states.SelectedIndex >= 0)
				UpdateStates(combobox_states.SelectedIndex);
		}
		private void button_play_stop_Click(object sender, RoutedEventArgs e)
		{
			if ((string)button_play_stop.Content == "Play")
			{
				playTimer = new DispatcherTimer()
				{
					Interval = new TimeSpan(0, 0, 0, 0, SPEED_LOOKUP[int.Parse(textbox_speed.Text)])
				};
				playTimer.Tick += PlayTimerElapsed;
				playTimer.Start();
				button_play_stop.Content = "Stop";
			}
			else
			{
				playTimer.Stop();
				button_play_stop.Content = "Play";
			}

		}

		// main graphics section
		private void canvas_obj_MouseDown(object sender, MouseButtonEventArgs e)
		{
			// start to move map around
			if (e.ChangedButton == MouseButton.Middle)
			{
				Mouse.Capture(canvas_obj);
				dragStartPos = e.GetPosition(canvas_obj);
				dragStartLeft = Canvas.GetLeft(viewbox_obj);
				dragStartTop = Canvas.GetTop(viewbox_obj);
				rect_map_sel.Visibility = Visibility.Hidden;
			}

			// draw tiles and attributes
			if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right)
			{
				windows.main.ChangesMade();
				if ((bool)radio_tile.IsChecked)
					DrawTile(e.GetPosition(img_obj).X, e.GetPosition(img_obj).Y, e.LeftButton == MouseButtonState.Pressed);
				else
					DrawPal(e.GetPosition(img_obj).X, e.GetPosition(img_obj).Y);
			}
		}
		private void canvas_obj_MouseMove(object sender, MouseEventArgs e)
		{
			UpdateSelRect(e.GetPosition(img_obj).X, e.GetPosition(img_obj).Y);
			// move map around
			if (dragStartPos.X != -1)
			{
				Canvas.SetLeft(viewbox_obj,
					dragStartLeft + (e.GetPosition(canvas_obj).X - dragStartPos.X));
				Canvas.SetTop(viewbox_obj,
					dragStartTop + (e.GetPosition(canvas_obj).Y - dragStartPos.Y));

				ClampMapLoc();
			}
			// draw stuff
			if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
			{
				if ((bool)radio_tile.IsChecked)
					DrawTile(e.GetPosition(img_obj).X, e.GetPosition(img_obj).Y, e.LeftButton == MouseButtonState.Pressed);
				else
					DrawPal(e.GetPosition(img_obj).X, e.GetPosition(img_obj).Y);
			}
		}
		private void canvas_obj_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Middle)
			{
				Mouse.Capture(null);
				dragStartPos.X = -1;
			}
		}
		private void canvas_obj_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			double mousePercentX = e.GetPosition(viewbox_obj).X / viewbox_obj.ActualWidth;
			double mousePercentY = e.GetPosition(viewbox_obj).Y / viewbox_obj.ActualHeight;
			zoom *= e.Delta > 0 ? ZOOM_PERCENT : 1 / ZOOM_PERCENT;
			UpdateZoom(mousePercentX, mousePercentY);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Wagner
{
	public class UndoHistory
	{
		private List<object[]> history;
		private int ind;
		private WindowSet windows;

		public UndoHistory(WindowSet windows)
		{
			history = new List<object[]>();
			this.windows = windows;
		}

		public void RecordState(bool endState = false)
		{
			if (ind < history.Count)
				history.RemoveRange(ind, history.Count - ind);
			history.Add(new object[8]
				{
					DeepCopyPalette(windows.tilePicker.bgPalettes),
					DeepCopyTripleInt(windows.meta.metatiles),
					DeepCopyDoubleInt(windows.meta.metaAttrs),
					DeepCopyDoubleInt(windows.map.tiles),
					DeepCopyDoubleInt(windows.map.metas),
					DeepCopyDoubleInt(windows.map.attrs),
					DeepCopyDoubleBool(windows.map.walls),
					DeepCopyObjectData(windows.objs.objs)
				});
			if (!endState)
				ind++;
		}

		private void ApplyState()
		{
			object[] currState = history[ind];

			windows.tilePicker.bgPalettes = currState[0] as Color[][];
			windows.meta.metatiles = currState[1] as int[][][];
			windows.meta.metaAttrs = currState[2] as int[][];
			windows.map.tiles = currState[3] as int[][];
			windows.map.metas = currState[4] as int[][];
			windows.map.attrs = currState[5] as int[][];
			windows.map.walls = currState[6] as bool[][];
			windows.objs.objs = currState[7] as List<Objekt>;

			windows.main.RedrawAll();
		}

		public void Undo()
		{
			if (ind == history.Count)
				RecordState(true);
			if (ind == 0)
				return;
			// decrement ind and get that state
			ind--;
			ApplyState();
		}

		public void Redo()
		{
			if (ind < history.Count - 1)
			{
				ind++;
				ApplyState();
			}
		}

		private Color[][] DeepCopyPalette(Color[][] oldPal)
		{
			Color[][] newPal = new Color[4][];
			for (int i = 0; i < 4; i++)
			{
				newPal[i] = new Color[4];
				for (int j = 0; j < 4; j++)
				{
					newPal[i][j] = oldPal[i][j];
				}
			}
			return newPal;
		}

		private int[][][] DeepCopyTripleInt(int[][][] oldArray)
		{
			int[][][] newArray = new int[oldArray.Length][][];
			for (int y = 0; y < oldArray.Length; y++)
			{
				newArray[y] = new int[oldArray[y].Length][];
				for (int x = 0; x < oldArray[y].Length; x++)
				{
					newArray[y][x] = new int[oldArray[y][x].Length];
					for (int i = 0; i < oldArray[y][x].Length; i++)
					{
						newArray[y][x][i] = oldArray[y][x][i];
					}
				}
			}
			return newArray;
		}

		private int[][] DeepCopyDoubleInt(int[][] oldArray)
		{
			int[][] newArray = new int[oldArray.Length][];
			for (int y = 0; y < oldArray.Length; y++)
			{
				newArray[y] = new int[oldArray[y].Length];
				for (int x = 0; x < oldArray[y].Length; x++)
				{
					newArray[y][x] = oldArray[y][x];
				}
			}
			return newArray;
		}
		private byte[][] DeepCopyDoubleInt(byte[][] oldArray)
		{
			byte[][] newArray = new byte[oldArray.Length][];
			for (int y = 0; y < oldArray.Length; y++)
			{
				newArray[y] = new byte[oldArray[y].Length];
				for (int x = 0; x < oldArray[y].Length; x++)
				{
					newArray[y][x] = oldArray[y][x];
				}
			}
			return newArray;
		}

		private bool[][] DeepCopyDoubleBool(bool[][] oldArray)
		{
			bool[][] newArray = new bool[oldArray.Length][];
			for (int y = 0; y < oldArray.Length; y++)
			{
				newArray[y] = new bool[oldArray[y].Length];
				for (int x = 0; x < oldArray[y].Length; x++)
				{
					newArray[y][x] = oldArray[y][x];
				}
			}
			return newArray;
		}

		private List<Objekt> DeepCopyObjectData(List<Objekt> oldObjs)
		{
			List<Objekt> newObjs = new List<Objekt>();

			for (int o = 0; o < oldObjs.Count; o++)
			{
				newObjs.Add(new Objekt());
				newObjs[o].name = oldObjs[o].name;
				newObjs[o].width = oldObjs[o].width;
				newObjs[o].height = oldObjs[o].height;
				newObjs[o].states = new List<AnimState>();
				for (int s = 0; s < oldObjs[o].states.Count; s++)
				{
					newObjs[o].states.Add(new AnimState(newObjs[o]));
					newObjs[o].states[s].name = oldObjs[o].states[s].name;
					newObjs[o].states[s].width = oldObjs[o].states[s].width;
					newObjs[o].states[s].height = oldObjs[o].states[s].height;
					newObjs[o].states[s].speed = oldObjs[o].states[s].speed;
					newObjs[o].states[s].frames = new List<AnimFrame>();
					for (int f = 0; f < oldObjs[o].states[s].frames.Count; f++)
					{
						newObjs[o].states[s].frames.Add(new AnimFrame());
						newObjs[o].states[s].frames[f].tiles = DeepCopyDoubleInt(oldObjs[o].states[s].frames[f].tiles);
						newObjs[o].states[s].frames[f].attrs = DeepCopyDoubleInt(oldObjs[o].states[s].frames[f].attrs);
					}
				}
			}

			return newObjs;
		}
	}
}

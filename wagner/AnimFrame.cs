using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Wagner
{
	public class AnimFrame
	{
		public int[][] tiles;
		public byte[][] attrs;
		public List<BitmapSource> bmps;

		public AnimFrame()
		{
			bmps = new List<BitmapSource>();
		}

		public void InitiateArrays(int width, int height)
		{
			tiles = new int[height][];
			attrs = new byte[height][];
			for (int y = 0; y < height; y++)
			{
				tiles[y] = new int[width];
				attrs[y] = new byte[width];
			}
		}

		public static int[][] DeepCopyDoubleInt(int[][] oldArray)
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

		public static byte[][] DeepCopyDoubleInt(byte[][] oldArray)
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
	}
}

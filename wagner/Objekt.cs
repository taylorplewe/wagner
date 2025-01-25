using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wagner
{
	public class Objekt
	{
		private const byte DEFAULT_WIDTH = 1; // 16x16
		private const byte DEFAULT_HEIGHT = 1;

		public string name;
		public List<AnimState> states;
		public byte width, height;

		public Objekt()
		{
			width = DEFAULT_WIDTH;
			height = DEFAULT_HEIGHT;
			states = new List<AnimState>();
			states.Add(new AnimState(this) { name = "idle" });
		}
	}
}

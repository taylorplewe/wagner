using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wagner
{
	public class AnimState
	{
		public string name;
		public byte speed;
		public byte width; // 8x8
		public byte height;
		public byte lst; // number of times this state is listed
						 // for instance a box always looks the same no matter what state, so it just has 'idle' with an lst of like 4
		public List<AnimFrame> frames;

		public AnimState(Objekt owner)
		{
			speed = 0;
			width = (byte)(owner.width * 2);
			height = (byte)(owner.height * 2);
			frames = new List<AnimFrame>();
			frames.Add(new AnimFrame());
			frames[0].InitiateArrays(width, height);
			lst = 1;
		}
	}
}

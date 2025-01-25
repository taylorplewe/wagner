using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wagner
{
	public class FileSet
	{
		public string wag = "";
		public string chr = "";
		public string pal = "";
		public string tiles = "";
		public string attrs = "";
		public string metas = "";
		public string walls = "";
		public string objs = "";
		public string plr = "";
		public string room = "";

		public void SetVariantString(string variant, string val)
		{
			switch (variant)
			{
				case "chr":
					chr = val;
					break;
				case "pal":
					pal = val;
					break;
				case "maptiles":
					tiles = val;
					break;
				case "mapattrs":
					attrs = val;
					break;
				case "metatiles":
					metas = val;
					break;
				case "walls":
					walls = val;
					break;
				case "objs":
					objs = val;
					break;
				case "plr":
					plr = val;
					break;
				case "room":
					room = val;
					break;
				default:
					break;
			}
		}
	}
}

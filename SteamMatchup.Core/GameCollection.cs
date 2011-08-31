using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamMatchUp
{
	public class GameCollection : List<Game>
	{
		public string Username { get; set; }
		public string ProfileUrl { get; set; }

		public GameCollection(IEnumerable<Game> games)
			: base(games)
		{
		}
	}
}

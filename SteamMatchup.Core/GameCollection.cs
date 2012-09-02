using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamMatchUp
{
	public class GameCollection : List<Game>
	{
        public string SteamId { get; set; }
		public string Username { get; set; }

		public GameCollection(IEnumerable<Game> games)
			: base(games)
		{
		}
	}
}

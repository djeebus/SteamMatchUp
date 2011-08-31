using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamMatchUp
{
	public class FriendCollection : List<Friend>
	{
		public string Username { get; set; }
		public string ProfileUrl { get; set; }

		public FriendCollection(IEnumerable<Friend> friends)
			: base(friends)
		{
		}
	}
}

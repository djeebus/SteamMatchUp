using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamMatchUp
{
	public class FriendCollection : List<User>
	{
		public string Username { get; set; }

        public FriendCollection(IEnumerable<User> friends)
			: base(friends)
		{
		}
	}
}

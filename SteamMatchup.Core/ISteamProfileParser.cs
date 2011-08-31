using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamMatchUp
{
	public interface ISteamProfileParser
	{
		FriendCollection GetFriends(string steamCommunityId);
		GameCollection GetGames(string steamCommunityId);
	}
}

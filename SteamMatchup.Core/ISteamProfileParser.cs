using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamMatchUp
{
	public interface ISteamProfileParser
	{
        User GetUser(Uri profileUrl);
        FriendCollection GetFriends(Uri profileUrl);
        GameCollection GetGames(Uri profileUrl);
	}
}

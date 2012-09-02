using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamMatchUp.SteamApi
{
    public enum CommunityVisibilityState
    {
        Private = 1, 
        FriendsOnly = 2, 
        FriendsOfFriends = 3, 
        UsersOnly = 4, 
        Public = 5,
    }
}

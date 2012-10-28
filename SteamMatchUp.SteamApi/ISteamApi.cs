using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamMatchUp.SteamApi
{
    public interface ISteamApi
    {
        PlayerSummary[] GetPlayerSummaries(string[] steamIds);
        FriendSummary[] GetFriendList(string steamId);
    }

    public class GetPlayerSummariesResponseWrapper
    {
        public GetPlayerSummariesResponse Response { get; set; }
    }

    public class GetPlayerSummariesResponse
    {
        public PlayerSummary[] Players { get; set; }
    }

    public class GetFriendSummaryResponseWrapper
    {
        public GetFriendSummaryResponse FriendsList { get; set; }
    }

    public class GetFriendSummaryResponse
    {
        public FriendSummary[] Friends { get; set; }
    }

    public class FriendSummary
    {
        public string SteamId { get; set; }
        public string Relationship { get; set; }
        public string Friend_Since { get; set; }
    }

    public class PlayerSummary
    {
        /// <summary>
        /// 64bit SteamID of the user
        /// </summary>
        public string SteamId { get; set; }

        /// <summary>
        /// The player's persona name (display name)
        /// </summary>
        public string PersonaName { get; set; }

        /// <summary>
        /// The full URL of the player's Steam Community profile.
        /// </summary>
        public Uri ProfileUrl { get; set; }
        /// <summary>
        /// The full URL of the player's 32x32px avatar. If the user hasn't configured an avatar, this will be the default ? avatar.
        /// </summary>
        public Uri Avatar { get; set; }
        /// <summary>
        /// The full URL of the player's 64x64px avatar. If the user hasn't configured an avatar, this will be the default ? avatar.
        /// </summary>
        public Uri AvatarMedium { get; set; }
        /// <summary>
        /// The full URL of the player's 184x184px avatar. If the user hasn't configured an avatar, this will be the default ? avatar.
        /// </summary>
        public Uri AvatarFull { get; set; }
        /// <summary>
        /// The user's current status. 0 - Offline, 1 - Online, 2 - Busy, 3 - Away, 4 - Snooze, 5 - looking to trade, 6 - looking to play. 
        /// If the player's profile is private, this will always be "0", except is the user has set his status to looking to trade or 
        /// looking to play, because a bug makes those status appear even if the profile is private.
        /// </summary>
        public PersonaState PersonaState { get; set; }
        /// <summary>
        /// One of five values: 1 - the profile is "Private", 2 - the profile is "Friends Only", 3 - the profile is "Friends of Friends", 
        /// 4 - the profile is "Users Only", 5 - the profile is "Public". Since the update where the values "Friends of Friends" and 
        /// "Users Only" were introduced this value is always 3 regardless of the visibility of the profile. This value is broken.
        /// </summary>
        public CommunityVisibilityState CommunityVisibilityState { get; set; }
        /// <summary>
        /// If set, indicates the user has a community profile configured (will be set to '1')
        /// </summary>
        public bool ProfileState { get; set; }
        /// <summary>
        /// The last time the user was online, in unix time.
        /// </summary>
        public int LastLogOff { get; set; }
        /// <summary>
        /// If set, indicates the profile allows public comments.
        /// </summary>
        public bool CommentPermission { get; set; }
        /// <summary>
        /// The player's "Real Name", if they have set it.
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// The player's primary group, as configured in their Steam Community profile.
        /// </summary>
        public string PrimaryClanId { get; set; }
        /// <summary>
        /// The time the player's account was created.
        /// </summary>
        public long TimeCreated { get; set; }
        /// <summary>
        /// If the user is currently in-game, this value will be returned and set to the gameid of that game.
        /// </summary>
        public string CurrentGameId { get; set; }
        /// <summary>
        /// The ip and port of the game server the user is currently playing on, if they are playing on-line in a game using Steam matchmaking. Otherwise will be set to "0.0.0.0:0".
        /// </summary>
        public System.Net.IPEndPoint GameServerIp { get; set; }
        /// <summary>
        /// If the user is currently in-game, this will be the name of the game they are playing. This may be the name of a non-Steam game shortcut.
        /// </summary>
        public string GameExtraInfo { get; set; }
        /// <summary>
        /// This value will be removed in a future update (see loccityid)
        /// </summary>
        public string CityId { get; set; }
        /// <summary>
        /// If set on the user's Steam Community profile, The user's country of residence, 2-character ISO country code
        /// </summary>
        public string LocCountryCode { get; set; }
        /// <summary>
        /// If set on the user's Steam Community profile, The user's state of residence
        /// </summary>
        public string LocStateCode { get; set; }
        /// <summary>
        /// An internal code indicating the user's city of residence. A future update will provide this data in a more useful way.
        /// </summary>
        public string LocCityId { get; set; }
    }
}

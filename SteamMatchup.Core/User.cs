using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamMatchUp
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string CommunityUrl { get; set; }
        public string IconUrl { get; set; }

        public List<Stat> Stats { get; set; }
    }

    public class Stat
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
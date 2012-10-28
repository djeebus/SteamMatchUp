using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamMatchUp
{
    public class GameInfo
    {
        public string Name { get; set; }
        public Uri IconUrl { get; set; }
        public string[] Features { get; set; }
        public string[] Genres { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: ({1}) [{2}]",
                this.Name,
                string.Join(", ", this.Genres),
                string.Join(", ", this.Features));
        }
    }
}

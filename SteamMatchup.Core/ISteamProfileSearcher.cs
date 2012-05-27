using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamMatchUp
{
    public interface ISteamProfileSearcher
    {
        ProfileSearchResult[] Search(string text);
    }
}

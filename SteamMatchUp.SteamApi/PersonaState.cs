using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamMatchUp.SteamApi
{
    public enum PersonaState
    {
        Offline = 0, 
        Online = 1, 
        Busy = 2, 
        Away = 3, 
        Snooze = 4, 
        LookingToTrade = 5, 
        LookingToPlay = 6,
    }
}

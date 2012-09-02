using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamMatchUp.SteamApi
{
    public class InvalidKeyException : Exception
    {
        public InvalidKeyException(Exception innerException)
            : base("Invalid Key", innerException)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SteamMatchUp.Website.Models
{
    public class AuthModel
    {
        [OpenIdIdentifier]
        [Required]
        public string Id { get; set; }
    }

    public class OpenIdIdentifierAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (!(value is string))
                return false;

            return DotNetOpenAuth.OpenId.Identifier.IsValid((string)value);
        }
    }
}
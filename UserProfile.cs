using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBotTest
{
    public class UserProfile
    {       
        // The ID of the user's channel.
        public string Mail { get; set; }

        public string Mail2 { get; set; }

        // Rastea si ya se ha preguntado el nombre al usuario
        public bool PromptedUserForName { get; set; } = false;

        public bool PromptedUserForMail { get; set; } = false;

        public string Name { get; set; }

        public string Name2 { get; set; }
    }
}

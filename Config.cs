using System.IO;
using System.Reflection;
using System.Diagnostics;
using botzival.HandlingFiles;
using System.Collections.Generic;

namespace botzival
{
    class Config
    {
        public static string DiscordToken;

        public static readonly List<ulong> MyBots = new List<ulong>
        {
            794106267550351400
        };

        static Config()
        {
            DiscordToken = System.IO.File.ReadAllText(null); 
        }
    }
}

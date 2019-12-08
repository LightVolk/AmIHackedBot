using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AmIHackedBot.Emails
{
    public class SimpleEmailNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name;
        }
    }
}

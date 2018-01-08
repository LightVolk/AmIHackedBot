using AmIHackedBot.Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmIHackedBot.Emails
{
    /// <summary>
    /// email
    /// </summary>
    public class Email
    {
        /// <summary>
        /// email 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// collection of breaches
        /// </summary>
        public List<Breach> BreachColl { get;  set; }

        public Email(string name,List<Breach> breachColl)
        {
            Name = name;
            BreachColl = breachColl;
        }
    }
}

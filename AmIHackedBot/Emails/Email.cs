using AmIHackedBot.Sharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

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
        [JsonPropertyName("name")]
        public string Name { get;  set; }

        /// <summary>
        /// collection of breaches
        /// </summary>
        [JsonPropertyName("breachCollection")]
        public List<Breach> BreachCollection { get;  set; }

        public Email()
        {

        }
        public Email(string name,List<Breach> breachCollection)
        {
            Name = name;
            BreachCollection = breachCollection;
        }
    }
}

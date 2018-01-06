using AmIHackedBot.Emails;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmIHackedBot.Comparers
{
    /// <summary>
    /// Equality comparer for email
    /// </summary>
    public class EmailEqualityComparer : IEqualityComparer<Email>
    {
        /// <summary>
        /// equals
        /// </summary>
        /// <param name="x">first parameter</param>
        /// <param name="y">second parameter</param>
        /// <returns></returns>
        public bool Equals(Email x, Email y)
        {
            if (x.Name == y.Name)
                return true;
            return false;
        }


        /// <summary>
        /// get hach code
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns></returns>
        public int GetHashCode(Email obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}

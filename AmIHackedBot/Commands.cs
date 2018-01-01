using System;
using System.Collections.Generic;
using System.Text;

namespace AmIHackedBot
{
    public static class Commands
    { /// <summary>
      /// add email to subscribe list
      /// </summary>
        public const String ADD_EMAIL_COMMAND = "/add";

        /// <summary>
        /// remove email from subscribe list
        /// </summary>
        public const String REMOVE_EMAIL_COMMAND = "/remove";

        /// <summary>
        /// show subscribed emails
        /// </summary>
        public const String SHOW_EMAIL_LIST = "/showemails";
    }
}

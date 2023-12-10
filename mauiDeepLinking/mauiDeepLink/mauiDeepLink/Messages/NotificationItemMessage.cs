using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mauiDeepLink.Messages
{
    public class NotificationItemMessage : ValueChangedMessage<String>
    {
        public NotificationItemMessage(string value) : base(value)
        {

        }
    }
}

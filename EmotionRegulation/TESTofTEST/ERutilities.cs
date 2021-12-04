using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERconfiguration
{
    public class ERutilities
    {
        /*
        public string RelatedEvent { get; private set; }
        public string RelatedAction { get; private set; }
        */

        public ERutilities()
        {

        }


        public (string relatedAction, string eventName) SplitAction(string actionEvent)
        {
            
            var SpecialCharacter = actionEvent.Split("|");
            var RelatedAction = SpecialCharacter[0].Trim();
            var RelatedEvent = SpecialCharacter[1].Trim();
            (string, string) EventsActions = (RelatedAction, RelatedEvent);
            return EventsActions;
        }


    }
}

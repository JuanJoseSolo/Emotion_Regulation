using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WellFormedNames;

namespace ERconfiguration
{
    public class ERutilities
    {

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

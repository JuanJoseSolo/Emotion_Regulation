using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WellFormedNames;
using EmotionalAppraisal;

namespace TestEmotion
{
    class RebuildEvent : AvoidEvents
    {
        public struct EventsConstruc
        {
            public bool Var_AvoidEvent;
            public Name EventFati;
            public Name EventTemplate;
            public string TypeVar;
            public int Index;

        }

        public static List<Name> EvTemList;

        public static EventsConstruc BuildEvent(Name events, EmotionalAppraisalAsset ea, bool Est)
        {
            EventsConstruc ConstrEve = new();

            //obtain the last term of the events
            Console.WriteLine("\n------------------------RebuildEvent---------------------------");
            ConstrEve.Var_AvoidEvent = bool.Parse(events.GetNTerm(5).ToString());
                
            Console.WriteLine("\nCurrent Event with 6 term------> " + events);
            Console.WriteLine("Current Event is Avoided------> " + ConstrEve.Var_AvoidEvent);

            //remove the last term (True or False)
            var ListEvent = events.GetLiterals().ToList();
            ListEvent.RemoveAt(5);
            var Var_EventName = ListEvent[3].ToString();
            ConstrEve.EventFati = Name.BuildName(ListEvent);
            
            for (int j = 0; j < ea.GetAllAppraisalRules().ToList().Count; j++)
            {
                var EventTemplate = ea.GetAllAppraisalRules().ElementAt(j).EventMatchingTemplate;
                //ConstrEve.Var_AvoidEvent
                if ((EventTemplate.GetNTerm(3).ToString().Equals(Var_EventName)) && Est && ConstrEve.Var_AvoidEvent)
                {
                    //Build new eventTemplate with a word NOT
                    EvTemList = EventTemplate.GetTerms().ToList();
                    ListEvent.RemoveAt(3);
                    EvTemList.RemoveAt(3);
                    var NEwNAmeEvent = Name.BuildName("Not-" + Var_EventName);
                    EvTemList.Insert(3, NEwNAmeEvent);
                    ListEvent.Insert(3, NEwNAmeEvent);
                    ConstrEve.EventTemplate = Name.BuildName(EvTemList);
                    ConstrEve.EventFati = Name.BuildName(ListEvent);
                    
                    //Get Appraisal Var. values
                    var SplitVar = ea.GetAllAppraisalRules().ElementAt(j).AppraisalVariables;
                    var Splitd = SplitVar.ToString().Split("=");
                    ConstrEve.TypeVar = Splitd[0];
                    float ValueVar = float.Parse(Splitd[1]);
                    ConstrEve.Index = j;
                }

            }
        return ConstrEve;
        }
    }
}

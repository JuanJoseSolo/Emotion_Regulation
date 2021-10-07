using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutobiographicMemory;
using EmotionalAppraisal;
using WellFormedNames;
using KnowledgeBase;
using Fuzzy_Personalities;



namespace TestEmotion
{
    
    public class AvoidEventes : Strategies
    {
        public static Strategies _Personality;
        public static Name ChangeEvent(Name evento, EmotionalAppraisalAsset ea)
        {
            if (evento.NumberOfTerms == 6)
            {

            

                Console.WriteLine("Var Applied Strategy" );
                var Var_AvoidEvent = evento.GetNTerm(5).ToString(); ;


                Console.WriteLine("\nEvent Hello------> " + evento);
                Console.WriteLine("Variable_Avoid------> " + Var_AvoidEvent);


                var Literals_Event = evento.GetLiterals();

                var ListEvent = Literals_Event.ToList();

                ListEvent.Remove((Name)"0");
                ListEvent.Remove((Name)"1");

                var Hello_Event1_1 = Name.BuildName(ListEvent);
                Console.WriteLine("Evento------> " + Hello_Event1_1.ToString());
                Console.ReadKey();

                if (true)
                {

                    var Events = ea.GetAllAppraisalRules().ToList();

                    Console.WriteLine("\n");
                    foreach (var n in Events)
                    {

                       string Even = n.AppraisalVariables.ToString();
                        var x = n.AppraisalVariables;
                        x.appraisalVariables.AsParallel();

                        Console.WriteLine("Event---> " + Even);
                        Console.WriteLine("Event---> " + x);


                    }

                    Console.ReadKey();

                }

                return evento;
            }
            else { return evento; }

        }
    }
}

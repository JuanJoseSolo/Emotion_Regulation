using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutobiographicMemory;
using EmotionalAppraisal;
using WellFormedNames;
using KnowledgeBase;
//using Fuzzy_Personalities;



namespace TestEmotion
{
    
    public class Events : Strategies
    {
        //public static Strategies _Personality;
        public static string NameStrategy_test = string.Empty;
        public static bool Apply;


        public static void Personality_test(float Consientioness, float Extraversion)
        {
            Strategies _Personality = new Strategies();

            float Situation_Selection =_Personality.SitSele(Consientioness, Extraversion);

            if (Situation_Selection > 4.5)
            {
                NameStrategy_test = "Situation Selection is applied";
                Apply = true;

            }
            else
            {
                NameStrategy_test = "Situation Selection isn't applied";
                Apply = false;

            }

            return;

        }


        public static Name ChangeEvent(Name evento, EmotionalAppraisalAsset ea)
        {

                if (evento.NumberOfTerms == 6)
                {
                    
                    Console.WriteLine("Var Applied Strategy");
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

                    }return evento;

                }
            
            
            else { return evento; }
       
        
        
        }
    }
}

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
using EmotionalAppraisal.AppraisalRules;
using EmotionalAppraisal.DTOs;



namespace TestEmotion
{
    
    public class AvoidEvents : Strategies
    {
        public static string StrategyName = string.Empty;
        public static string TypeVar = string.Empty;
        public static int count;
        public static bool StrategyApplied;
        public static bool flag = false;
        public static List<Name> EvTemList = new List<Name>();
        public static Name changedName = Name.NIL_SYMBOL;
        //Commit

        public static void StrategyTest(float Consientioness, float Extraversion, KB Character)
        {
                       
            Console.WriteLine(" \n Name Character" + Character.Perspective);
            Strategies strategies = new Strategies();
            float Situation_Selection = strategies.SitSele(Consientioness, Extraversion);

            if (Situation_Selection > 4.5)
            {
                StrategyName = "Situation Selection is applied";
                StrategyApplied = true;

            }
            else
            {
                StrategyName = "Situation Selection isn't applied";
                StrategyApplied = false;

            }

            return;
        }

        public static Tuple<Name, EmotionalAppraisalAsset> ChangeEvent(Name evento, EmotionalAppraisalAsset ea_Character)
        {
            dynamic TestSending2 = Name.NIL_SYMBOL;

            if (evento.NumberOfTerms == 6)
            {
                           
                //obtain the last term of the events
                Console.WriteLine("\n---------------------ChangeEvent----------------------" );
                bool Var_AvoidEvent = bool.Parse(evento.GetNTerm(5).ToString());
                var  Var_EventName  = evento.GetNTerm(3).ToString();

                Console.WriteLine("\nCurrent Event------> " + evento);
                Console.WriteLine("Current Event is Avoided------> " + Var_AvoidEvent);

                //remove the last term (True or False)
                var ListEvent = evento.GetLiterals().ToList();
                ListEvent.RemoveAt(5);
                var EvWithout = Name.BuildName(ListEvent);
                var TestSending = Name.BuildName("Event(Action-End, Pedro, TestBye, Sarah)");
                Console.WriteLine("Event without Aviod Value------> " + EvWithout.ToString()+"\n");
                
                for (int jj = 0; jj < ea_Character.GetAllAppraisalRules().ToList().Count; jj++)
                {
                    var EventTemplate = ea_Character.GetAllAppraisalRules().ElementAt(jj).EventMatchingTemplate;

                    if (EventTemplate.GetNTerm(3).ToString().Equals(Var_EventName))
                    {
                        //Build new event with a word NOT
                        EvTemList = EventTemplate.GetTerms().ToList();
                        EvTemList.RemoveAt(3);
                        EvTemList.Insert(3, Name.BuildName("Not-" + Var_EventName));
                        changedName = Name.BuildName(EvTemList);
                        //Get Appraisal Var. values
                        var SplitVar = ea_Character.GetAllAppraisalRules().ElementAt(jj).AppraisalVariables;
                        var Splitd = SplitVar.ToString().Split("=");
                        TypeVar = Splitd[0];
                        float ValueVar = float.Parse(Splitd[1]);
                        flag = true;
                        count = jj;
                    }
                }


                //modify of Appraisal variables specify of a event
                if (StrategyApplied && Var_AvoidEvent && flag)
                {
                    //Search in ea_Character Appraisal rules
                   /*
                    for (int j = 0; j < ea_Character.GetAllAppraisalRules().ToList().Count; j++)
                    {
                        var EventTemplate = ea_Character.GetAllAppraisalRules().ElementAt(j).EventMatchingTemplate;
                        
                        if (EventTemplate.GetNTerm(3).ToString().Equals(Var_EventName))
                        {
                            //Build new event with a word NOT
                            var EvTemList = EventTemplate.GetTerms().ToList();
                            EvTemList.RemoveAt(3);
                            EvTemList.Insert(3, Name.BuildName("Not-" + Var_EventName));
                            var changedName = Name.BuildName(EvTemList);
                            //Get Appraisal Var. values
                            var SplitVar = ea_Character.GetAllAppraisalRules().ElementAt(j).AppraisalVariables;
                            var Splitd = SplitVar.ToString().Split("=");
                            string TypeVar = Splitd[0];
                            float ValueVar = float.Parse(Splitd[1]);
                            */
                            //New Appraisal variables
                            var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                            {
                                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() 
                                { 
                                    Name = TypeVar , Value = (Name.BuildName(0)) 
                                }
                            };
                            var rule = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                            {
                                EventMatchingTemplate = changedName,
                                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                            };
                            ea_Character.AddOrUpdateAppraisalRule(rule); //update new ARule
                            
                            //Finding a specific new ARule
                            var NewEvMaTe = ea_Character.GetAllAppraisalRules().ElementAt(count + 2);///Check this option
                            //To remove old event and inserted the new
                            var ea_CharacterList = ea_Character.GetAllAppraisalRules().ToList();
                            ea_CharacterList.RemoveAt(count);
                            ea_CharacterList.Insert(count, NewEvMaTe);
                            //To remove the new EMT from the last position
                            ea_CharacterList.RemoveAt(count + 2);
                            var deleteRules = ea_Character.GetAllAppraisalRules().ToList();

                            /*
                            var BeforeDel = ea_Character.GetAllAppraisalRules();                            
                            foreach (var oldEMT in BeforeDel)
                            {
                                Console.WriteLine("Before_Removed----> "+oldEMT.EventMatchingTemplate);

                            }
                            */
                            //Remove all Appraisal Rules from ea_Character
                            ea_Character.RemoveAppraisalRules(deleteRules);
                            /*
                            var ApRul = ea_Character.GetAllAppraisalRules();
                            foreach (var newEMT in ApRul)
                            {
                                Console.WriteLine("Delete....." + newEMT.EventMatchingTemplate);

                            }
                            */
                            //To insert the new EMT into ea_Character
                            for (int ff = 0; ff < ea_CharacterList.Count; ff++)
                            {
                                ea_Character.AddOrUpdateAppraisalRule(ea_CharacterList[ff]);
                            }
                            
                            foreach (var newEMT2 in ea_Character.GetAllAppraisalRules())
                            {
                                Console.WriteLine("After Delete....." + newEMT2.EventMatchingTemplate);

                            }
                            
                            //return Tuple.Create(changedName, ea_Character);
                       // }
                        
                    //}
                }
                return Tuple.Create(changedName, ea_Character);
            }
            else { return Tuple.Create(evento, ea_Character);}
        }
    }
}

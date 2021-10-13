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
    //Commit02
    public class AvoidEvents : Strategies
    {
        public static string StrategyName = string.Empty;
        public static string TypeVar = string.Empty;
        public static int Index;
        public static bool StrategyApplied;
        public static bool flag = false;
        public static List<Name> EvTemList = new List<Name>();
        public static Name changedName = Name.NIL_SYMBOL;
        

        public static void StrategyTest(float Consientioness, float Extraversion, KB Character)
        {
            Console.WriteLine("\n--------------------AvoidEvents_StrategyTest------------------------");
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
            if (evento.NumberOfTerms == 6)
            {
                           
                //obtain the last term of the events
                Console.WriteLine("\n------------------------AvoidEvents---------------------------");
                bool Var_AvoidEvent = bool.Parse(evento.GetNTerm(5).ToString());
                var  Var_EventName  = evento.GetNTerm(3).ToString();

                Console.WriteLine("\nCurrent Event------> " + evento);
                Console.WriteLine("Current Event is Avoided------> " + Var_AvoidEvent);

                //remove the last term (True or False)
                var ListEvent = evento.GetLiterals().ToList();
                ListEvent.RemoveAt(5);
                var EvWithout = Name.BuildName(ListEvent);
                Console.WriteLine("Event without Aviod Value------> " + EvWithout.ToString()+"\n");

                //Search in ea_Character Appraisal rules
                for (int j = 0; j < ea_Character.GetAllAppraisalRules().ToList().Count; j++)
                {
                    var EventTemplate = ea_Character.GetAllAppraisalRules().ElementAt(j).EventMatchingTemplate;

                    if (EventTemplate.GetNTerm(3).ToString().Equals(Var_EventName))
                    {
                        //Build new event with a word NOT
                        EvTemList = EventTemplate.GetTerms().ToList();
                        EvTemList.RemoveAt(3);
                        EvTemList.Insert(3, Name.BuildName("Not-" + Var_EventName));
                        changedName = Name.BuildName(EvTemList);
                        //Get Appraisal Var. values
                        var SplitVar = ea_Character.GetAllAppraisalRules().ElementAt(j).AppraisalVariables;
                        var Splitd = SplitVar.ToString().Split("=");
                        TypeVar = Splitd[0];
                        float ValueVar = float.Parse(Splitd[1]);
                        flag = true;
                        Index = j+1;                        
                    }
                }

                //modify of Appraisal variables specify of a event
                if (StrategyApplied && Var_AvoidEvent && flag)
                {
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
                    var NewEvMaTe = ea_Character.GetAllAppraisalRules().ElementAt(Index);///Check this option

                    //To remove old event and inserted the new
                    var ea_CharacterList = ea_Character.GetAllAppraisalRules().ToList();
                    ea_CharacterList.RemoveAt(Index);
                    ea_CharacterList.Insert(Index, NewEvMaTe);

                    //To remove the new EMT from the last position
                    ea_CharacterList.RemoveAt(Index);
                    var deleteRules = ea_Character.GetAllAppraisalRules().ToList();

                    //Remove all Appraisal Rules from ea_Character
                    ea_Character.RemoveAppraisalRules(deleteRules);
                    
                    //To insert the new EMT into ea_Character
                    for (int ff = 0; ff < ea_CharacterList.Count; ff++)
                    {
                        ea_Character.AddOrUpdateAppraisalRule(ea_CharacterList[ff]);
                    }

                return Tuple.Create(changedName, ea_Character);
                }
            return Tuple.Create(EvWithout, ea_Character);
            }
        else { return Tuple.Create(evento, ea_Character);}
        }
    }
}

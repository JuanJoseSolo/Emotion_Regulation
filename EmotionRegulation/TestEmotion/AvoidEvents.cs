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
        public static bool StrategyApplied;
    
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
            if (evento.NumberOfTerms == 6)
            {
                           
                //obtain the last term of the events
                Console.WriteLine("\n---------------------ChangeEvent----------------------" );
                bool Var_AvoidEvent = bool.Parse(evento.GetNTerm(5).ToString());
                var  Var_EventName  = evento.GetNTerm(3).ToString();

                Console.WriteLine("\nCurrent Event------> " + evento);
                Console.WriteLine("Current Event is Avoided------> " + Var_AvoidEvent);

                //remove the last term
                var ListEvent = evento.GetLiterals().ToList();
                ListEvent.RemoveAt(5);
              
                //reconstructed event for to send
                var EventSent = Name.BuildName(ListEvent);
                Console.WriteLine("Event Sent------> " + EventSent.ToString()+"\n");

                //modify of Appraisal variables specify of event
                
                if (StrategyApplied && Var_AvoidEvent)
                {
                    /*
                    foreach (var FindEvent in ea_Character.GetAllAppraisalRules())
                    {
                        Console.WriteLine("AppraisalRulesBy eaCharacter = " + FindEvent.EventMatchingTemplate);
                        
                        if (FindEvent.EventMatchingTemplate.GetNTerm(3).Equals((Name)Var_EventName))
                        {

                            var EMT = FindEvent.EventMatchingTemplate;
                            var A = FindEvent.EventMatchingTemplate.GetTerms().ToList();
                            A.RemoveAt(3);
                            A.Insert(3, Name.BuildName("Not-" + Var_EventName));
                            var Namechange = Name.BuildName(A);

                            //Appraisal Variables

                            //var newEvenChara = ea_Character;
                            /////// EVENT 3 //////
                            var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                            {
                                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "NewTest" , Value = (Name.BuildName(0)) }
                            };
                            var rule = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                            {
                                EventMatchingTemplate = Namechange,
                                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                            };
                            //newEvenChara.AddOrUpdateAppraisalRule(rule);



                        }
                    }*/
                    //Search in ea_Character Appraisal rules
                    for (int j = 0; j < ea_Character.GetAllAppraisalRules().ToList().Count; j++)
                    {
                        var EventTemplate = ea_Character.GetAllAppraisalRules().ElementAt(j).EventMatchingTemplate;

                        if (EventTemplate.GetNTerm(3).ToString().Equals(Var_EventName))
                        {
                            //Change Event
                            var EvTemList = EventTemplate.GetTerms().ToList();
                            EvTemList.RemoveAt(3);
                            EvTemList.Insert(3, Name.BuildName("Not-" + Var_EventName));
                            var changedName = Name.BuildName(EvTemList);
                            var SplitVar = ea_Character.GetAllAppraisalRules().ElementAt(j).AppraisalVariables;
                            var testSplit = SplitVar.ToString().Split("=");

                            string TypeVar = testSplit[0];
                            float ValueVar = float.Parse(testSplit[1]);

                            var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                            {
                                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = TypeVar , Value = (Name.BuildName(0)) }
                            };
                            var rule = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                            {
                                EventMatchingTemplate = changedName,
                                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                            };
                            ea_Character.AddOrUpdateAppraisalRule(rule);

                            var ea_CharacterList = ea_Character.GetAllAppraisalRules().ToList();

                            //var DeleteAR = ea_Character.GetAllAppraisalRules();
                            var xx = ea_Character.GetAllAppraisalRules().ElementAt(j + 2);
                            ea_CharacterList.RemoveAt(j);
                            ea_CharacterList.Insert(j, xx);
                            ea_CharacterList.RemoveAt(j + 2);
                            var deleteRules = ea_Character.GetAllAppraisalRules().ToList();

                            var BeforeDel = ea_Character.GetAllAppraisalRules();
                            foreach (var evv in BeforeDel)
                            {
                                Console.WriteLine("Before_Removed----> "+evv.EventMatchingTemplate);

                            }

                            ea_Character.RemoveAppraisalRules(deleteRules);

                            var ApRul = ea_Character.GetAllAppraisalRules();
                            foreach (var bb in ApRul)
                            {
                                Console.WriteLine("Delete....." + bb.EventMatchingTemplate);

                            }

                            for (int ff = 0; ff < ea_CharacterList.Count; ff++)
                            {
                                ea_Character.AddOrUpdateAppraisalRule(ea_CharacterList[ff]);
                            }

                            //var asset = BuildTestAsset();

                            //var ea_Character2 = new EmotionalAppraisal.AppraisalRules.ReactiveAppraisalDerivator();



                            //Console.WriteLine("\n ea_CharacterSend------> " + ea_CharacterList.ElementAt(j).EventMatchingTemplate);


                        }

                        
                    }
                }

                return Tuple.Create(EventSent, ea_Character);
            }
            else { return Tuple.Create(evento, ea_Character);}

        }
    }
}

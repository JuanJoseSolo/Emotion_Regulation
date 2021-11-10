using System;
using System.Collections.Generic;
using System.Linq;
using EmotionalAppraisal;
using WellFormedNames;
using Fuzzy_Personalities;
using ActionLibrary.DTOs;
using EmotionalDecisionMaking;

namespace TestEmotion
{
    //Commit02
    public class EmotionRegulation
    {
        
        public struct Values
        {
            public Name NewEvent;
            public EmotionalAppraisalAsset NewEA;
        }

        public static Values ChangeEvent(Name evento, EmotionalAppraisalAsset ea_Character , EmotionalDecisionMakingAsset edm)
        {

            ///////  FUZZY PERSONALITY   ////// 

            float Cons = 60, Extrav = 10, Neuro = 0, Oppen = 0, Agree = 0;

            var ER = AppliedStrategies.SelectStrategy(Cons, Extrav, Neuro, Oppen, Agree);
            Console.WriteLine("\n Strategy Applied------>> " + ER.StrategyApplied + "\n Strategy Name---->> "
                                                             + ER.StrategyName);

            Values Data = new();
            //Para regresar los mismos parámetros
            Console.WriteLine("\n------------------------ChangeEvents----------------------------------");

            
            switch (ER.StrategyName)
            {
                
                case "Situation Selection":

                    //Recontruir el evento
                    var rebuildEvents = RebuildEvent.BuildEvent(evento, ea_Character, ER);
                    Data.NewEvent = rebuildEvents.EventFati;
                    Data.NewEA = ea_Character;

                    //modify of Appraisal variables specify of a event
                    if (ER.StrategyApplied && rebuildEvents.Var_AvoidEvent)
                    {

                        //New Appraisal variables
                        var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                        {
                        new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                            {
                            Name = rebuildEvents.TypeVar , Value = (Name.BuildName(-4))
                            }
                        };
                        var rule = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                        {
                            EventMatchingTemplate = rebuildEvents.EventTemplate,
                            AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                        };
                        Data.NewEA.AddOrUpdateAppraisalRule(rule); //update new ARule

                        //Finding a specific new ARule
                        var NewEvMaTe = Data.NewEA.GetAllAppraisalRules().ElementAt(rebuildEvents.Index);///Check this option

                        //To remove old event and inserted the new
                        var ea_CharacterList = Data.NewEA.GetAllAppraisalRules().ToList();
                        ea_CharacterList.RemoveAt(rebuildEvents.Index);
                        ea_CharacterList.Insert(rebuildEvents.Index, NewEvMaTe);

                        //To remove the new EMT from the last position
                        ea_CharacterList.RemoveAt(rebuildEvents.Index);
                        var deleteRules = Data.NewEA.GetAllAppraisalRules().ToList();

                        //Remove all Appraisal Rules from ea_Character
                        Data.NewEA.RemoveAppraisalRules(deleteRules);

                        //To insert the new EMT into ea_Character
                        for (int ff = 0; ff < ea_CharacterList.Count; ff++)
                        {
                            Data.NewEA.AddOrUpdateAppraisalRule(ea_CharacterList[ff]);
                        }
                    }
                    break;

                case "Situation Modification":

                    var VarEvent = RebuildEvent.BuildEvent(evento, ea_Character,ER);
                    Data.NewEvent = VarEvent.EventFati;
                    Data.NewEA = ea_Character;

                    if (ER.StrategyApplied && VarEvent.ValueVar<-4)
                    {


                        Console.WriteLine("\n--------------------Actions------------------");

                        var actionRule = new ActionRuleDTO 
                        { 
                            Action = Name.BuildName("ToHug"), Priority = Name.BuildName("3"), Target = (Name)"Sarah" 
                        };

                        
                        var id = edm.AddActionRule(actionRule);
                        var ruleEDM = edm.GetActionRule(id);
                        edm.AddRuleCondition(id, "Like(Sarah) = True");
                        var actions = edm.Decide(Name.UNIVERSAL_SYMBOL);
                        edm.Save();

                        Console.WriteLine("\n Decisions: ");
                        foreach (var a in actions)
                        {
                            Console.WriteLine("\n " + a.Name.ToString() + " To: " + a.Target);
                        }

                        //New Appraisal variables
                        var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                        {
                        new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                            {
                            Name = VarEvent.TypeVar, Value = (Name.BuildName(-1))
                            }
                        };
                        var rule = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                        {
                            EventMatchingTemplate = VarEvent.EventTemplate,
                            AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                        };
                        Data.NewEA.AddOrUpdateAppraisalRule(rule); //update new ARule
                        
                        //Finding a specific new ARule
                        var NewEvMaTe = Data.NewEA.GetAllAppraisalRules().ElementAt(VarEvent.Index);///Check this option

                        //To remove old event and inserted the new
                        var ea_CharacterList = Data.NewEA.GetAllAppraisalRules().ToList();
                        ea_CharacterList.RemoveAt(VarEvent.Index);
                        ea_CharacterList.Insert(VarEvent.Index, NewEvMaTe);

                        //To remove the new EMT from the last position
                        ea_CharacterList.RemoveAt(VarEvent.Index);
                        var deleteRules = Data.NewEA.GetAllAppraisalRules().ToList();



                        //Remove all Appraisal Rules from ea_Character
                        Data.NewEA.RemoveAppraisalRules(deleteRules);

                        //To insert the new EMT into ea_Character
                        for (int ff = 0; ff < ea_CharacterList.Count; ff++)
                        {
                            Data.NewEA.AddOrUpdateAppraisalRule(ea_CharacterList[ff]);
                        }

                        foreach (var list in ea_CharacterList)
                        {
                            Console.WriteLine("Lista de EMT  " + list.EventMatchingTemplate);
                        }

                    }

                    break;
            }
            return Data;
        }
    }
}
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
    public class AvoidEvents: Strategies
    {
        
        public struct Values
        {
            public Name NewEvent;
            public EmotionalAppraisalAsset NewEA;
        }

        public static Values ChangeEvent(Name evento, EmotionalAppraisalAsset ea_Character)
        {
            Values Data = new();
            //Para regresar los eventos intactos
            Data.NewEA = ea_Character;
            Data.NewEvent = evento;
            Console.WriteLine("\n------------------------ChangeEvents---------------------------");

            if (evento.NumberOfTerms == 6)
            {

                ///////  FUZZY PERSONALITY   ////// 
                float Cons = 80, Extrav = 40, Neuro = 0, Oppen = 0, Agree = 0;
                var ER = AppliedStrategies.SelectStrategy(Cons, Extrav, Neuro, Oppen, Agree);
                Console.WriteLine("\n Variable Avoid------>> " + ER.StrategyApplied + "\n Strategy---->> "
                                                               + ER.StrategyName);

                var Strategia = ER.StrategyApplied;

                var rebuildEvents = RebuildEvent.BuildEvent(evento, ea_Character, Strategia);
                var ValueEvent = rebuildEvents.Var_AvoidEvent;
                var EventNews = rebuildEvents.EventFati;
                var NewEvenTemplate = rebuildEvents.EventTemplate;

                Data.NewEvent = EventNews;
                Data.NewEA = ea_Character;

                //modify of Appraisal variables specify of a event
                if (Strategia && ValueEvent)
                {
                    //New Appraisal variables
                    var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                    {
                        new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                        {
                            Name = rebuildEvents.TypeVar , Value = (Name.BuildName(0))
                        }
                    };
                    var rule = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                    {
                        EventMatchingTemplate = NewEvenTemplate,
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
                
            }
            return Data;
        }
    }
}

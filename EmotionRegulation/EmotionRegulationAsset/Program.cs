using System;
using System.Linq;
using EmotionalAppraisal;
using EmotionalDecisionMaking;
using AutobiographicMemory;
using WellFormedNames;
using KnowledgeBase;
using GAIPS.Rage;
using System.Collections.Generic;
using ActionLibrary;
using ActionLibrary.DTOs;
using WorldModel;
using WorldModel.DTOs;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using RolePlayCharacter;
using EmotionalAppraisal.OCCModel;
using ERconfiguration;
using SpreadsheetLight;
using System.Diagnostics;
using EmotionalAppraisal.DTOs;
using IntegratedAuthoringTool;
using System.Timers;

namespace EmotionRegulationAsset
{


    class Program
    {
        static List<(string events, string strategy)> Results = new(); // guarda los resultados, pero de la primera prueba Pedro
        static string path { get; set; }
        public struct ERcharacter //estructura para crear al agente
        {
            public PersonalityTraits Personality {get; set;}
            public EmotionRegulationAsset eR { get; set; }
            public KB kB { get; set; }
            public ConcreteEmotionalState eS { get; set; }
            public EmotionalAppraisalAsset eA { get; set; }
            public EmotionalDecisionMakingAsset eDM { get; set; }
            public RolePlayCharacterAsset rPC { get; set; }
            public AM aM { get; set; }

        }
        public struct AppVariables
        {
            public string OCC_Variable { get; set; }
            public float Value { get; set; }
            public string Target { get; set; }
        }
        public struct DF
        {
            public System.Data.DataTable dataTable { get; set; }
            public SLDocument SLdocument { get; set; }
            public string pathFile { get; set; }
        }
        static (string relatedAction, string eventName) SplitActionName(string actionEvent)
        {
            var SpecialCharacter = actionEvent.Split("|");
            var RelatedAction = SpecialCharacter[0].Trim();
            var RelatedEvent = SpecialCharacter[1].Trim();
            (string, string) EventsActions = (RelatedAction, RelatedEvent);
            return EventsActions;
        }


        static ERcharacter BuildRPCharacter(string name, string body)
        {
            EmotionalAppraisalAsset ea_Character = EmotionalAppraisalAsset.CreateInstance(new AssetStorage());
            var storage = new AssetStorage();

            var character = new ERcharacter
            {
                kB = new KB((Name)name),
                aM = new AM() { Tick = 0, },
                eS = new ConcreteEmotionalState(),
                eA = ea_Character,
                eDM = EmotionalDecisionMakingAsset.CreateInstance(storage)
            };

            character.rPC = new RolePlayCharacterAsset
            {
                BodyName = body,
                VoiceName = body,
                CharacterName = (Name)name,
                m_kb = character.kB,
            };
            character.rPC.LoadAssociatedAssets(new GAIPS.Rage.AssetStorage());


            ///instancias para emotion regulation
            

            return character;
        }
        static DF CreateDataframe(string AgentName, PersonalityTraits Personality)
        {
            DF DF = new();
            var DominantPersonality = Personality.DominantPersonality;
            //path df
            var origen = "C:/Users/JuanJoseAsus/source/repos/FAtiMA-Toolkit-master/EmotionRegulation/EmotionRegulationAsset/Results/";

            var Dominant = string.Concat("_"+ DominantPersonality);
            if (string.IsNullOrEmpty(DominantPersonality))
            {
                path = origen + AgentName + "_NotPersonalityDominant" + ".xlsx";
            }
            else
                path = origen + AgentName + Dominant + ".xlsx";

            //Data frama
            string pathFile = AppDomain.CurrentDomain.DynamicDirectory + path;
            SLDocument oSLDocument = new();
            System.Data.DataTable df = new();


            //columnas
            df.Columns.Add("MOOD     ", typeof(float));
            df.Columns.Add("EMOTION  ", typeof(string));
            df.Columns.Add("INTENSITY", typeof(float));
            df.Columns.Add("   EVENT    ", typeof(string));
            df.Columns.Add(" APPLIED STRATEGY    ", typeof(string));
            df.Columns.Add(DominantPersonality.ToUpper(), typeof(string));
            Personality.List_PersonalityType.ForEach(p => df.Rows.Add(null, null, null, null, null, p));

            //df.Columns.Add(" PERSONALITY TRAITS ", typeof(string));
            //df.Columns.Add(" STRATEGIES RELATED ", typeof(string));
            //df.Columns.Add(" DOMINANT PERSONALITY ", typeof(string));

            DF.dataTable = df;
            DF.SLdocument = oSLDocument;
            DF.pathFile = pathFile;

            return DF;
        }
        static void UpdateAppraisalRulesComposed(EmotionalAppraisalAsset EA, List<AppVariables> variable, string eventMatch)
        {
            var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>();
            foreach (var Appraisal in variable)
            {
                appraisalVariableDTO.Add(new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                {
                    Name = Appraisal.OCC_Variable,
                    Value = Name.BuildName(Appraisal.Value),
                    Target = Name.BuildName(Appraisal.Target)
                });
            }
            var rule = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
            {

                EventMatchingTemplate = Name.BuildName("Event(Action-End, *," + eventMatch + ", *)"),
                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
            };
            EA.AddOrUpdateAppraisalRule(rule);
        }
        static void UpdateAppraisalRules(EmotionalAppraisalAsset EA, string variable, float value, string target, Name eventMatch)
        {
            var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                    {

                    new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                    {
                        Name = variable,
                        Value = Name.BuildName(value),
                        Target = Name.BuildName(target)
                    }
                };
            var rule = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
            {

                EventMatchingTemplate = Name.BuildName("Event(Action-End, *," + eventMatch.ToString() + ", *)"),
                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
            };
            EA.AddOrUpdateAppraisalRule(rule);
        }
        static Dictionary<string, List<Name>> UpdateEvents()
        {
            //Past events for Attetional deployment
            var Party = Name.BuildName("Event(Action-End, Matt, Party, Office)");//Office
            var Workmates = Name.BuildName("Event(Action-End, Matt, Workmates, Office)");//Office
            var Jobs = Name.BuildName("Event(Action-End, Matt, OtherWorks, Boss)");//Boss
            var AnotherBye = Name.BuildName("Event(Action-End, Matt, AnotherBye, Sarah)");//Sarah
            var AllMoney = Name.BuildName("Event(Action-End, Matt, AllMoney, Company)");//becomeRich
            List<Name> PastEvents = new() { Party, Workmates, AnotherBye, Jobs, AllMoney }; //Events past for AttentionDeployment

            //CurrentEvents
            var TalktoBoss = Name.BuildName("Event(Action-End, Matt, Talk, Boss)");
            var Hello = Name.BuildName("Event(Action-End, Sarah, Hello, Matt)");
            var Discussion = Name.BuildName("Event(Action-End, Matt, Discussion, Others)");
            var Bye = Name.BuildName("Event(Action-End, Matt, Bye, Sarah)");
            var Congrat = Name.BuildName("Event(Action-End, Matt, Congrat, Sarah)");
            var Conversation = Name.BuildName("Event(Action-End, Matt, Conversation, Sarah)");
            var Hug = Name.BuildName("Event(Action-End, Matt, Hug, Sarah)");
            var Fired = Name.BuildName("Event(Action-End, Matt, Fired, Boss)");
            var Crash = Name.BuildName("Event(Action-End, Matt, Crash, Car)");
            var Profits = Name.BuildName("Event(Action-End, Matt, Profits, Cash)");
            var BecomeRich = Name.BuildName("Event(Action-End, Boss, BecomeRich, Company)");//Test emotion type                
            List<Name> CurrentEvents = new()
            {
                TalktoBoss,
                Hello,
                Conversation,
                Hug,
                Discussion,
                Congrat,
                Bye,
                Fired,
                Crash,
                Profits,
                BecomeRich
            };

            //Emotion regulation events
            var TalktoBoss_ER = Name.BuildName("Event(Action-End, Pedro, Talk, Boss, [False])");
            var Hello_ER = Name.BuildName("Event(Action-End, Pedro, Hello, Sarah)");
            var Discussion_ER = Name.BuildName("Event(Action-End, Pedro, Discussion, Others, [True])");
            var Bye_ER = Name.BuildName("Event(Action-End, Pedro, Bye, Sarah, [False])");
            var Congrat_ER = Name.BuildName("Event(Action-End, Pedro, Congrat, Sarah)");
            var Conversation_ER = Name.BuildName("Event(Action-End, Pedro, Conversation, Sarah)");
            var Hug_ER = Name.BuildName("Event(Action-End, Pedro, Hug, Sarah)");
            var Fired_ER = Name.BuildName("Event(Action-End, Pedro, Fired, Boss, [False])");
            var Crash_ER = Name.BuildName("Event(Action-End, Pedro, Crash, Car, [False])");
            var Profits_ER = Name.BuildName("Event(Action-End, Pedro, Profits, Pedro)");
            var Fly_ER = Name.BuildName("Event(Action-End, Pedro, Fly, Sky, [False])");
            var BecomeRich_ER = Name.BuildName("Event(Action-End, Pedro, BecomeRich, Company, [False])");
            List<Name> ERevents = new()
            {
                //TalktoBoss_ER, Hello_ER, Conversation_ER, Hug_ER, Discussion_ER, Congrat_ER,
                //Bye_ER, Fired_ER, Crash_ER, Profits_ER
                //Fly_ER,
                BecomeRich_ER
            };

            //Events for alternative events (cognitive change strategy)
            var Event1 = Name.BuildName("Event(Action-End, Matt, FindNewJob, Fired)");
            var Event2 = Name.BuildName("Event(Action-End, Matt, MeetNewPeople, Fired)");
            var Event3 = Name.BuildName("Event(Action-End, Matt, BetterSalary, Fired)");
            var Event4 = Name.BuildName("Event(Action-End, Matt, BetterSalary, Talk)");
            var Event5 = Name.BuildName("Event(Action-End, Matt, NewCar, Crash)");
            var Event6 = Name.BuildName("Event(Action-End, Matt, NewHouse, BecomeRich)");
            List<Name> AlternativeEvents = new()
            {
                Event1,
                Event2,
                Event3,
                Event4,
                Event5,
                Event6 //Cognitive change
            };

            Dictionary<string, List<Name>> Events = new()
            {
                { "CurrentEvents", CurrentEvents },
                { "PastEvents", PastEvents },
                { "ERevents", ERevents },
                { "AlternativeEvents", AlternativeEvents }
            };

            return Events;
        }
        static Dictionary<string, string> CreateActions(EmotionalDecisionMakingAsset eDMcharacter)
        {
            Dictionary<string, string> ActionsToEvents = new();
            //Action for the event: Talk 
            var ActionEventEnter = "Joke | Talk";
            var TeventActionEnter = SplitActionName(ActionEventEnter);

            var ER_EnterAction = new ActionRuleDTO
            {
                Action = Name.BuildName(TeventActionEnter.relatedAction),
                Priority = Name.BuildName("1"),
                Target = (Name)"Office",
            };
            var idER_Enter = eDMcharacter.AddActionRule(ER_EnterAction);
            eDMcharacter.AddRuleCondition(idER_Enter, "Current(Location) = Office");
            eDMcharacter.Save();
            ActionsToEvents.Add(TeventActionEnter.relatedAction, TeventActionEnter.eventName);

            //Action for the event: Talk 
            var ActionEventEnter2 = "Wait | Talk";
            var TeventActionEnter2 = SplitActionName(ActionEventEnter2);

            var ER_EnterAction2 = new ActionRuleDTO
            {
                Action = Name.BuildName(TeventActionEnter2.relatedAction),
                Priority = Name.BuildName("5"),
                Target = (Name)"Office",
            };
            var idER_Enter2 = eDMcharacter.AddActionRule(ER_EnterAction2);
            eDMcharacter.AddRuleCondition(idER_Enter2, "Current(Location) = Office");
            eDMcharacter.Save();
            ActionsToEvents.Add(TeventActionEnter2.relatedAction, TeventActionEnter2.eventName);

            //Action for the event: Bye
            var ActionNameBye = "ToHug | Bye";
            var DiccEventActionBye = SplitActionName(ActionNameBye);

            var ER_ByeAction = new ActionRuleDTO
            {
                Action = Name.BuildName(DiccEventActionBye.relatedAction),
                Priority = Name.BuildName("1"),
                Target = (Name)"Sarah",
            };
            var idER_Bye = eDMcharacter.AddActionRule(ER_ByeAction);
            eDMcharacter.AddRuleCondition(idER_Bye, "Like(Sarah) = True");
            eDMcharacter.Save();
            ActionsToEvents.Add(DiccEventActionBye.relatedAction, DiccEventActionBye.eventName);


            //Action for event Fired
            var ActionNameFired = "TalkToBoss|Fired";
            var DiccEventActionFired = SplitActionName(ActionNameFired);
            var ER_FiredAction = new ActionRuleDTO
            {
                Action = Name.BuildName(DiccEventActionFired.relatedAction),
                Priority = Name.BuildName("1"),
                Target = (Name)"SELF",
            };
            var idER_Fired = eDMcharacter.AddActionRule(ER_FiredAction);
            eDMcharacter.AddRuleCondition(idER_Fired, "Current(Location) = Office");
            eDMcharacter.Save();
            ActionsToEvents.Add(DiccEventActionFired.relatedAction, DiccEventActionFired.eventName);

            var ActionNameRich = "BuyAll|-";
            var DiccEventActionRich = SplitActionName(ActionNameRich);
            var ER_RichAction = new ActionRuleDTO
            {
                Action = Name.BuildName(DiccEventActionRich.relatedAction),
                Priority = Name.BuildName("1"),
                Target = (Name)"SELF",
            };
            var idER_Rich = eDMcharacter.AddActionRule(ER_RichAction);
            eDMcharacter.AddRuleCondition(idER_Rich, "Current(Location) = Office");
            eDMcharacter.Save();
            ActionsToEvents.Add(DiccEventActionRich.relatedAction, DiccEventActionRich.eventName);

            return ActionsToEvents;
        }

        static void Simulations(ERcharacter character, List<Name> EventsEvaluation, bool IsPastEvent, bool haveER)
        {
            var agentName = character.kB.Perspective.ToString();
            DF df = new();
            Console.WriteLine(" \n        " + agentName.ToUpper() + "'s PERSPECTIVE");
            
            foreach (var Event in EventsEvaluation)
            {
                if (!IsPastEvent) { df = CreateDataframe(agentName, character.Personality); }
                
                if (haveER)
                {
                    var strategyName = character.Personality.DStrategyPower.Where(v => v.Value == "Strongly").Select(s => s.Key).ToList();
                    var StrategiesResults = character.eR.AntecedentFocusedFrame(strategyName, Event);

                    character.eA.AppraiseEvents(new[] { character.eR.EventFatima }, character.eS, character.aM, character.kB, null);

                    Console.WriteLine(" \n Events occured so far: "
                        + string.Concat(character.aM.RecallAllEvents().Select(e => "\n Id: "
                        + e.Id + " Event: " + e.EventName.ToString()).LastOrDefault()));
                    
                    if (!StrategiesResults.StrategySuccessful)
                    {
                        StrategiesResults = character.eR.ResponseFocusedFrame(strategyName, Event);
                    }

                    character.aM.Tick++;
                    character.eS.Decay(character.aM.Tick);
                    Console.WriteLine(" \n  Mood on tick '" + character.aM.Tick + "': " + character.eS.Mood);
                    Console.WriteLine("  Active Emotions \n  "
                            + string.Concat(character.eS.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));
                    character.eA.Save();
                    Console.WriteLine("\n-------------------------- RESUMEN ----------------------------\n ");
                    StrategiesResults.Results.ForEach(r => Console.WriteLine(r));
                    //information to dataframe
                    var MOOD = character.eS.Mood;
                    var EMOTION = character.eS.GetAllEmotions().Select(e => e.EmotionType).LastOrDefault();
                    var INTESITY = character.eS.GetAllEmotions().Select(e => e.Intensity).LastOrDefault();
                    var EVENT = StrategiesResults.Results.Select(e => e.Event).LastOrDefault();
                    var STRATEGY = StrategiesResults.Results.Select(s => s.Strategy).LastOrDefault();
                    
                    df.dataTable.Rows.Add(MOOD, EMOTION, INTESITY, EVENT, STRATEGY);
                    df.SLdocument.ImportDataTable(1,1, df.dataTable, true);
                    df.SLdocument.SaveAs(df.pathFile);
                    Console.WriteLine("\n---------------------------------------------------------------\n ");
                    continue;
                }

                character.eA.AppraiseEvents(new[] { Event }, character.eS, character.aM, character.kB, null);
                Console.WriteLine(" \n Events occured so far: "
                    + string.Concat(character.aM.RecallAllEvents().Select(e => "\n Id: "
                    + e.Id + " Event: " + e.EventName.ToString())));

                character.aM.Tick++;
                character.eS.Decay(character.aM.Tick);
                Console.WriteLine(" \n  Mood on tick '" + character.aM.Tick + "': " + character.eS.Mood);
                Console.WriteLine("  Active Emotions \n  "
                        + string.Concat(character.eS.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));
                character.eA.Save();
                Console.WriteLine("---------------------------------------------------------------");
                if (IsPastEvent)
                {
                    while (true)
                    {
                        character.aM.Tick++;
                        character.eS.Decay(character.aM.Tick);
                        var Intensity = character.eS.GetAllEmotions().Select(e => e.Intensity > 0).FirstOrDefault();
                        var Mood = character.eS.Mood > 0;
                        if (!Intensity && !Mood) break;
                        Console.WriteLine("   " + "Mood: " + character.eS.Mood);
                        Console.WriteLine("   " + string.Concat(character.eS.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));
                        
                        //Console.Clear();
                    }
                }
            }
        }
        
        static void Main(string[] args)
        {
            #region First test
            var PathOrigen = "C:/Users/JuanJoseAsus/source/repos/FAtiMA-Toolkit-master/EmotionRegulation/EmotionRegulationAsset/Results/";
            var ER_utilities = new ERutilities();
            var StrategyWasApplied = new Boolean(); //No se si se va seguir utilizando
            var _Personality = new PersonalityTraits(); 
            //Pedro   
            var am_Pedro = new AM();
            var kb_Pedro = new KB((Name)"Pedro");
            var storage = new AssetStorage();
            var edm_Pedro = EmotionalDecisionMakingAsset.CreateInstance(storage);
            var emotionalState_Pedro = new ConcreteEmotionalState();
            var rpc_Pedro = new RolePlayCharacterAsset();// Actualmente no está en uso
            EmotionalAppraisalAsset ea_Pedro = EmotionalAppraisalAsset.CreateInstance(new AssetStorage());
            //knowledgeBase Pedro
            kb_Pedro.Tell(Name.BuildName("Like(Sarah)"), Name.BuildName("True"), Name.BuildName("SELF"));
            kb_Pedro.Tell(Name.BuildName("Dislike(Usuario)"), Name.BuildName("True"), Name.BuildName("SELF"));
            kb_Pedro.Tell(Name.BuildName("Current(Location)"), Name.BuildName("Office"), Name.BuildName("SELF"));
            kb_Pedro.Tell(Name.BuildName("Relation(True)"), Name.BuildName("Pedro"), Name.BuildName("SELF"));
            edm_Pedro.RegisterKnowledgeBase(kb_Pedro);
            //Past events for Attetional deployment
            var Party = Name.BuildName("Event(Action-End, Pedro, Party, Office)");//Office
            var Workmates = Name.BuildName("Event(Action-End, Pedro, Workmates, Office)");//Office
            var Jobs = Name.BuildName("Event(Action-End, Pedro, OtherWorks, Boss)");//Boss
            var AnotherBye = Name.BuildName("Event(Action-End, Pedro, AnotherBye, Sarah)");//Sarah
            var SuperHero = Name.BuildName("Event(Action-End, Pedro, SuperHero, Sarah)");//Sky
            var AllMoney = Name.BuildName("Event(Action-End, Pedro, AllMoney, Company)");//become rich
            //Events
            var TalktoBoss = Name.BuildName("Event(Action-End, Pedro, Talk, Boss)");
            var Hello = Name.BuildName("Event(Action-End, Sarah, Hello, Pedro)");
            var Discussion = Name.BuildName("Event(Action-End, Pedro, Discussion, Others)");
            var Bye = Name.BuildName("Event(Action-End, Pedro, Bye, Sarah)");
            var Congrat = Name.BuildName("Event(Action-End, Pedro, Congrat, Sarah)");
            var Conversation = Name.BuildName("Event(Action-End, Pedro, Conversation, Sarah)");
            var Hug = Name.BuildName("Event(Action-End, Pedro, Hug, Sarah)");
            var Fired = Name.BuildName("Event(Action-End, Pedro, Fired, Boss)");
            var Crash = Name.BuildName("Event(Action-End, Pedro, Crash, Car)");
            var Profits = Name.BuildName("Event(Action-End, Pedro, Profits, Cash)");
            var Fly = Name.BuildName("Event(Action-End, Pedro, Fly, Sky)");
            var BecomeRich = Name.BuildName("Event(Action-End, Boss, BecomeRich, Company)");//Test emotion type 
            //SequenceEvents
            List<Name> PastCharacterEvents = new() { Party, Workmates, AnotherBye, Jobs, AllMoney }; //Events past for AttentionDeployment
            List<Name> CharacterEvents = new()
            {
                //TalktoBoss, Hello, Conversation, Hug, Discussion, Congrat, Bye, Fired, Crash, Profits
                BecomeRich
            };
            //Events for Attetional Deplyment
            void PastEventEvaluation()
            {
                // EVENT = Party 
                var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(6)) }
                };
                var rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Party, Office)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
                // EVENT = Workmates
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                {
                    Name = "Like", Value = (Name.BuildName(2))
                }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {

                    EventMatchingTemplate = (Name)"Event(Action-End, *, Workmates, Office)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
                // EVENT = OtherWorks
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                {
                    Name = "Desirability", Value = (Name.BuildName(5))
                }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {

                    EventMatchingTemplate = (Name)"Event(Action-End, *, OtherWorks, Boss)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
                // EVENT = AnotherBye
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                {
                    Name = "Desirability", Value = (Name.BuildName(3))
                }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {

                    EventMatchingTemplate = (Name)"Event(Action-End, *, AnotherBye, Sarah)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
                // EVENT = SuperHero
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                {
                    Name = "Desirability", Value = (Name.BuildName(3))
                }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {

                    EventMatchingTemplate = (Name)"Event(Action-End, *, SuperHero, Sky)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
                // EVENT = Become rich
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                    new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                    {
                        Name = OCCAppraisalVariables.DESIRABILITY_FOR_OTHER,
                        Value = (Name.BuildName(-8)),
                        Target = (Name)"Other"
                    },
                    new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                    {
                        Name = OCCAppraisalVariables.DESIRABILITY,
                        Value = (Name.BuildName(5)),
                        Target = (Name)"SELF",
                    }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {

                    EventMatchingTemplate = (Name)"Event(Action-End, *, AllMoney, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
            }
            void PedroEventEvaluation()
            {
                /////// EVENT = Talk //////
                var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(-5)) }
                };
                var rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Talk, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);

                ////////// EVENT = HELLO ///////
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                {
                    Name = "Like", Value = (Name.BuildName(3))
                }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {

                    EventMatchingTemplate = (Name)"Event(Action-End, *, Hello, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);

                /////// EVENT = BYE //////
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(-5)) }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Bye, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);

                /////// EVENT = CONVERSATION //////
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Like", Value = (Name.BuildName(4)) }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Conversation, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);

                /////// EVENT = HUG //////
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Like", Value = (Name.BuildName(7)) }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Hug, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)

                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);

                /////// EVENT = Congrat //////
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(2)) }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Congrat, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);

                /////// EVENT = Argue //////
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(-5)) }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Discussion, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);

                /////// EVENT = Fired //////
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Like", Value = (Name.BuildName(-8)) }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Fired, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);

                /////// EVENT = Crash //////
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Like", Value = (Name.BuildName(-9)) }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Crash, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
                /////// EVENT = Profits //////
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(8)) }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Profits, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
                ////// EVENT = FlyToSky //////
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {

                    new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                    {
                        Name = OCCAppraisalVariables.PRAISEWORTHINESS,
                        Value = (Name.BuildName(10)),
                        Target = (Name)"SELF"
                    },
                    new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                    {
                        Name = OCCAppraisalVariables.DESIRABILITY,
                        Value = (Name.BuildName(-1)),
                    },
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Fly, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
                ////// EVENT = BecomeRich\ test different emotions //////
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                   new AppraisalVariableDTO()
                   {
                       Name = OCCAppraisalVariables.GOALSUCCESSPROBABILITY,
                       Value = (Name)"-6",
                       Target = (Name)"StillAlive"

                   }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *,BecomeRich, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
            }
            void PedroActions()
            {
                //Action for the event: Eat 
                var Eat = new ActionRuleDTO
                {
                    Action = Name.BuildName("Eat"),
                    Priority = Name.BuildName("1"),
                    Target = (Name)"Office",
                };
                var idER_Enter = edm_Pedro.AddActionRule(Eat);
                edm_Pedro.AddRuleCondition(idER_Enter, "Current(Location) = Office");
                edm_Pedro.Save();
            } //Normal Actions
            PedroActions(); //Revisar la forma de como quedaron la nueva manera de declarar las acciones en el nuevo agente

            //Goals
            Goal goal = new();
            goal.Name = (Name)"StillAlive";
            goal.Significance = 4f;
            goal.Likelihood = 5f;
            Dictionary<string, Goal> Goals = new();
            Goals.Add("StillAlive", goal);

            ///para el cálculo de la intensidad basada en goals, primero se toma el likelihood y se le suma el valor de la variabl de 
            ///valoración, si el resultado de esta suma es menor a cero, entoces goalProbability es igual a cero lo que genera una valencia de 
            ///emoción negativa y la intensidad se calcula como: (1 - goalProbability) * potential, donde el potencial es igual a Significance,
            ///o lo que es lo mismo, Significance = Intesity

            #endregion

            var Matt = BuildRPCharacter("Matt", "Male");
            Matt.kB.Tell((Name)"Location(Home)", (Name)"True"); //Actualmente sirve para las acciones
            Matt.eDM.RegisterKnowledgeBase(Matt.kB);
            var events = UpdateEvents();
            var pastEvents        = events.Aggregate((k, v)=> k.Key == "PastEvents" ? k:v).Value;
            var currentEvents     = events.Aggregate((k, v)=> k.Key == "CurrentEvents" ? k:v).Value;
            var alternativeEvents = events.Aggregate((k, v) => k.Key == "AlternativeEvents" ? k : v).Value;
            var ERevents = events.Aggregate((k, v) => k.Key == "ERevents" ? k : v).Value;
            
            pastEvents.ForEach(       Event => UpdateAppraisalRules(Matt.eA, OCCAppraisalVariables.DESIRABILITY, 5, null, Event.GetNTerm(3)));
            //alternativeEvents.ForEach(Event => UpdateAppraisalRules(Matt.eA, OCCAppraisalVariables.DESIRABILITY, 4, null, Event.GetNTerm(3)));
            var Event = currentEvents.Select(e => e.GetNTerm(3)).ToArray();
            //TalktoBoss, Hello, Conversation, Hug, Discussion, Congrat, Bye, Fired, Crash, Profits, BecomeRich
            UpdateAppraisalRules(Matt.eA, OCCAppraisalVariables.DESIRABILITY, -6, null,     Event[0]);
            UpdateAppraisalRules(Matt.eA, OCCAppraisalVariables.LIKE, 4, null,              Event[1]);
            UpdateAppraisalRules(Matt.eA, OCCAppraisalVariables.PRAISEWORTHINESS, 5, "SELF",Event[2]);
            UpdateAppraisalRules(Matt.eA, OCCAppraisalVariables.LIKE, 9, null,              Event[3]);
            UpdateAppraisalRules(Matt.eA, OCCAppraisalVariables.DESIRABILITY, -4, null,     Event[4]);
            UpdateAppraisalRules(Matt.eA, OCCAppraisalVariables.DESIRABILITY, 6, null,      Event[5]);
            UpdateAppraisalRules(Matt.eA, OCCAppraisalVariables.DESIRABILITY, -6, null,     Event[6]);
            UpdateAppraisalRules(Matt.eA, OCCAppraisalVariables.LIKE, -9, null,             Event[7]);
            UpdateAppraisalRules(Matt.eA, OCCAppraisalVariables.DESIRABILITY, -7, null,     Event[8]);
            UpdateAppraisalRules(Matt.eA, OCCAppraisalVariables.LIKE, 4, null,              Event[9]);

            
            List<AppVariables> BeRich = new()
            {
                new AppVariables { OCC_Variable = OCCAppraisalVariables.DESIRABILITY, Value = -2, Target = null },
                new AppVariables { OCC_Variable = OCCAppraisalVariables.DESIRABILITY_FOR_OTHER, Value = 10, Target = "SELF" }
            };
            UpdateAppraisalRulesComposed(Matt.eA, BeRich, "BecomeRich");

            float O = 25.674458f, C = 12.197678f, E = 5.76437f, A = 79.439323f, N = 10.6815f;
            Matt.Personality = new(O, C, E, A, N);
            var ERactions = CreateActions(Matt.eDM);
            Matt.eR = new(Matt.eA, Matt.eDM, Matt.aM, Matt.eS, Matt.Personality, ERactions, alternativeEvents, 4);

            Console.WriteLine("\n\n\n------------------------ PAST EVENTS -----------------------");
            //Simulations(Matt, pastEvents, IsPastEvent: true, haveER: false);//past events

            Console.WriteLine("\n\n\n------------------------ CURRENT EVENTS -----------------------");
            Simulations(Matt, ERevents, IsPastEvent: false, haveER: true); 

            //parte del tutorial tests que esta en FAtiMA
            Matt.rPC.Perceive(TalktoBoss);
            var conditionsSet = new Conditions.ConditionSet();

            //--------------------//




            #region First test 

            Console.WriteLine("\n=====> Do you want to use Emotion Regulation Asset? -----> y/N ");
            //var Decision = Console.ReadLine();
            var Decision = "yw";

            EmotionRegulationAsset emotionRegulationSimulated = new();
            if (Decision == "y")
            {
                
                PedroEventEvaluation();
                //Emotional Appraisal Events, Creates events in character memory for attentional deployment
                PastEventEvaluation();
                PastEventSimulation();

                ///Para la primera estrategia: Selección de la situación.
                ///
                ///Cuando se va a hacer uso del modelo de regulación emocional, los eventos del escenario deberán de ser declarados siguiendo
                ///la siguiente estructura: si el agente puede evitar el evento ('seleccionar la situación') debido a las características del
                ///entorno, el último parametro deberá indicarse con un valor booleano True, de lo contrarío deberá indicarse con un valor 
                ///False o no contar con un valor definido.
                ///
                ///
                ///Para la segunda estrategia: Modificación de la situación.
                ///
                ///Para que el agente sea capaz de aplicar la segunda estrategia, el evento deberá tener una (o más) acción(es) relacionada(s)
                ///que le permitirán reaccionar al evento y de esta forma modificar su situación (ejerciendo la mejor acción a este evento). 
                ///Además, este esvento deberá contar una valoración relativamente negativa, por el momento, esta valoración negativa deberá 
                ///ser menor o igual a: -5.
                ///
                ///
                ///Para la tercera estrategia: Despliegue de la atención.
                ///
                ///Esta estrategia podrá ser aplicada siempre que existan eventos (por el momento, positivos) en la memoria del agente que
                ///esten relacionados con el agente objetivo causante del evento.
                ///
                ///Además, las estrategias de regulación podrán ser aplicadas o no, según sea el tipo de personalidad configurado en el agente.

                ////Events For Emotion Regulation Asset
                var TalktoBoss_ER = Name.BuildName("Event(Action-End, Pedro, Talk, Boss, [False])");
                var Hello_ER = Name.BuildName("Event(Action-End, Pedro, Hello, Sarah)");
                var Discussion_ER = Name.BuildName("Event(Action-End, Pedro, Discussion, Others, [True])");
                var Bye_ER = Name.BuildName("Event(Action-End, Pedro, Bye, Sarah, [False])");
                var Congrat_ER = Name.BuildName("Event(Action-End, Pedro, Congrat, Sarah)");
                var Conversation_ER = Name.BuildName("Event(Action-End, Pedro, Conversation, Sarah)");
                var Hug_ER = Name.BuildName("Event(Action-End, Pedro, Hug, Sarah)");
                var Fired_ER = Name.BuildName("Event(Action-End, Pedro, Fired, Boss, [False])");
                var Crash_ER = Name.BuildName("Event(Action-End, Pedro, Crash, Car, [False])");
                var Profits_ER = Name.BuildName("Event(Action-End, Pedro, Profits, Pedro)");
                var Fly_ER = Name.BuildName("Event(Action-End, Pedro, Fly, Sky, [False])");
                var BecomeRich_ER = Name.BuildName("Event(Action-End, Pedro, BecomeRich, Company, [False])");

                //Actions for Situation Modification
                Dictionary<string, string> Dictionary_relatedActionEvent = new();
                void ActionPedro_ER()
                {
                    //Action for the event: Talk 
                    var ActionEventEnter = "Joke | Talk";
                    var TeventActionEnter = ER_utilities.SplitAction(ActionEventEnter);

                    var ER_EnterAction = new ActionRuleDTO
                    {
                        Action = Name.BuildName(TeventActionEnter.relatedAction),
                        Priority = Name.BuildName("1"),
                        Target = (Name)"Office",
                    };
                    var idER_Enter = edm_Pedro.AddActionRule(ER_EnterAction);
                    edm_Pedro.AddRuleCondition(idER_Enter, "Current(Location) = Office");
                    edm_Pedro.Save();
                    Dictionary_relatedActionEvent.Add(TeventActionEnter.relatedAction, TeventActionEnter.eventName);

                    //Action for the event: Talk 
                    var ActionEventEnter2 = "Wait | Talk";
                    var TeventActionEnter2 = ER_utilities.SplitAction(ActionEventEnter2);

                    var ER_EnterAction2 = new ActionRuleDTO
                    {
                        Action = Name.BuildName(TeventActionEnter2.relatedAction),
                        Priority = Name.BuildName("5"),
                        Target = (Name)"Office",
                    };
                    var idER_Enter2 = edm_Pedro.AddActionRule(ER_EnterAction2);
                    edm_Pedro.AddRuleCondition(idER_Enter2, "Current(Location) = Office");
                    edm_Pedro.Save();
                    Dictionary_relatedActionEvent.Add(TeventActionEnter2.relatedAction, TeventActionEnter2.eventName);

                    //Action for the event: Bye
                    var ActionNameBye = "ToHug | Bye";
                    var DiccEventActionBye = ER_utilities.SplitAction(ActionNameBye);

                    var ER_ByeAction = new ActionRuleDTO
                    {
                        Action = Name.BuildName(DiccEventActionBye.relatedAction),
                        Priority = Name.BuildName("1"),
                        Target = (Name)"Sarah",
                    };
                    var idER_Bye = edm_Pedro.AddActionRule(ER_ByeAction);
                    edm_Pedro.AddRuleCondition(idER_Bye, "Like(Sarah) = True");
                    edm_Pedro.Save();
                    Dictionary_relatedActionEvent.Add(DiccEventActionBye.relatedAction, DiccEventActionBye.eventName);

                    
                    //Action for event Fired
                    var ActionNameFired = "TalkToBoss|Fired";
                    var DiccEventActionFired = ER_utilities.SplitAction(ActionNameFired);
                    var ER_FiredAction = new ActionRuleDTO
                    {
                        Action = Name.BuildName(DiccEventActionFired.relatedAction),
                        Priority = Name.BuildName("1"),
                        Target = (Name)"SELF",
                    };
                    var idER_Fired = edm_Pedro.AddActionRule(ER_FiredAction);
                    edm_Pedro.AddRuleCondition(idER_Fired, "Current(Location) = Office");
                    edm_Pedro.Save();
                    Dictionary_relatedActionEvent.Add(DiccEventActionFired.relatedAction, DiccEventActionFired.eventName);

                    var ActionNameRich = "BuyAll|BecomeRich";
                    var DiccEventActionRich = ER_utilities.SplitAction(ActionNameRich);
                    var ER_RichAction = new ActionRuleDTO
                    {
                        Action = Name.BuildName(DiccEventActionRich.relatedAction),
                        Priority = Name.BuildName("1"),
                        Target = (Name)"SELF",
                    };
                    var idER_Rich = edm_Pedro.AddActionRule(ER_RichAction);
                    edm_Pedro.AddRuleCondition(idER_Rich, "Current(Location) = Office");
                    edm_Pedro.Save();
                    Dictionary_relatedActionEvent.Add(DiccEventActionRich.relatedAction, DiccEventActionRich.eventName);

                }
                ActionPedro_ER();

                //Events for alternative events (cognitive change strategy)
                var Event1 = Name.BuildName("Event(Action-End, Pedro, FindNewJob, Fired)");
                var Event2 = Name.BuildName("Event(Action-End, Pedro, MeetNewPeople, Fired)");
                var Event3 = Name.BuildName("Event(Action-End, Pedro, BetterSalary, Fired)");
                var Event4 = Name.BuildName("Event(Action-End, Pedro, BetterSalary, Talk)");
                var Event5 = Name.BuildName("Event(Action-End, Pedro, NewCar, Crash)");
                var Event6 = Name.BuildName("Event(Action-End, Pedro, NewHouse, BecomeRich)");
                void EventReinterpret()
                {
                    /////// EVENT = Event1 //////
                    var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                    {
                        new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(2)) }
                    };
                    var rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                    {
                        EventMatchingTemplate = (Name)"Event(Action-End, *, FindNewJob, *)",
                        AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                    };
                    ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
                    /////// EVENT = Event2 //////
                    appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                    {
                        new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(3)) }
                    };
                    rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                    {
                        EventMatchingTemplate = (Name)"Event(Action-End, *, MeetNewPeople, *)",
                        AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                    };
                    ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
                    /////// EVENT = Event3 //////
                    appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                    {
                        new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(5)) }
                    };
                    rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                    {
                        EventMatchingTemplate = (Name)"Event(Action-End, *, BetterSalary, *)",
                        AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                    };
                    ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
                    /////// EVENT = Event4 ////// (Esta relacionado con better salary)
                    appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                    {
                        new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(1)) }
                    };
                    rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                    {
                        EventMatchingTemplate = (Name)"Event(Action-End, *, Better___Sal, *)",
                        AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                    };
                    ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
                    /////// EVENT = Event5 ////// 
                    appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                    {
                        new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(9)) }
                    };
                    rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                    {
                        EventMatchingTemplate = (Name)"Event(Action-End, *, NewCar, *)",
                        AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                    };
                    ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
                    /////// EVENT = Event6 ////// 
                    appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                    {
                        new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                        {
                            Name = OCCAppraisalVariables.DESIRABILITY_FOR_OTHER,
                            Value = (Name.BuildName(-4)),
                            Target = (Name)"Other"
                        },
                        new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                        {
                            Name = OCCAppraisalVariables.DESIRABILITY,
                            Value = (Name.BuildName(-5)),
                            Target = (Name)"SELF",
                        }
                    };
                    rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                    {
                        EventMatchingTemplate = (Name)"Event(Action-End, Pedro, NewHouse, *)",
                        AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                    };
                    ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
                } //Cognitive Change
                EventReinterpret();
                List<Name> LAlternativeEvents = new()
                {
                    Event1, Event2, Event3, Event4, Event5, Event6 //Cognitive change
                };

                //List of events for the Emotion Regulation Asset
                List<Name> List_EventsER = new()
                {
                    //TalktoBoss_ER, Hello_ER, Conversation_ER, Hug_ER, Discussion_ER, Congrat_ER,
                    //Bye_ER, Fired_ER, Crash_ER, Profits_ER
                    //Fly_ER,
                    BecomeRich_ER
                };
                /*
                var rand = new Random();
                var c = (float)rand.NextDouble() * 65f;
                var e = (float)rand.NextDouble() * 65f;
                var n = (float)rand.NextDouble() * 65f;
                var o = (float)rand.NextDouble() * 65f;
                var a = (float)rand.NextDouble() * 65f;
                */
                //float C = 24.197678f, E = 29.76437f, N = 26.6815f, O = 11.674458f, A = 20.439323f;

                //float C = 95.197678f, E = 29.76437f, N = 26.6815f, O = 11.674458f, A = 20.439323f;
                //Personalities
                //float C = c, E = e, N = n, O = o, A = a;
                               
                Console.WriteLine(" C = " + C + " E = "+ E + " N = " + N + " O = " + O + " A = "+ A);
                PersonalityTraits personalityTraitsPedro = new(C, E, N, O, A);
                ///personalityTraitsPedro.FuzzyAppliedStrategyTest();
                
                
                //Personality information for Data frame 
                _Personality = personalityTraitsPedro;

                var negativeEmotionIntensity = 4;
                //Emotion Regulation Asset
                EmotionRegulationAsset emotionRegulation = new(
                                    ea_Pedro, edm_Pedro, am_Pedro, emotionalState_Pedro,personalityTraitsPedro, Dictionary_relatedActionEvent, 
                                    LAlternativeEvents, negativeEmotionIntensity);
                emotionRegulation.GoalSignificance = goal.Significance;
                
                //path df
                string DominantPerson = personalityTraitsPedro.DominantPersonality;
                if (string.IsNullOrEmpty(DominantPerson))
                {
                    path = PathOrigen + "NotDominantP" + ".xlsx";
                }else
                    path = PathOrigen + DominantPerson + ".xlsx";

                //return control to Fatima
                emotionRegulationSimulated = emotionRegulation;
                CharacterEvents = List_EventsER;
                //Simulate events
                EvaluationSeveralEvents();
            }
            else
            {
                //Events without ERA
                path = PathOrigen + "NormalEvents.xlsx";

                PastEventEvaluation();
                PedroEventEvaluation();
                //Simulation
                PastEventSimulation();
                EvaluationSeveralEvents();
            }

            void EvaluationSeveralEvents()
            {                
                //Data frama
                string pathFile = AppDomain.CurrentDomain.DynamicDirectory + path;
                SLDocument oSLDocument = new();
                System.Data.DataTable df = new();
                //columnas
                df.Columns.Add("MOOD     ", typeof(float));
                df.Columns.Add("EMOTION  ", typeof(string));
                df.Columns.Add("INTENSITY", typeof(float));
                df.Columns.Add("   EVENT    ", typeof(string));
                df.Columns.Add(" APPLIED STRATEGY    ", typeof(string));
                df.Columns.Add(" PERSONALITY TRAITS ", typeof(string));
                df.Columns.Add(" STRATEGIES RELATED ", typeof(string));
                df.Columns.Add(" DOMINANT PERSONALITY ", typeof(string));

                Console.WriteLine("\n\n\n\n  Events simulations .  . . . ");
                //Console.ReadKey();
                Console.WriteLine("\n ACTUAL EVENTS: ");
                //Console.WriteLine("--------------------------------------------------------------------");
                Console.WriteLine(" Character: "+ kb_Pedro.Perspective.ToString().ToUpper());
                
                var index = 0;
                int eventNum = 0;
                float AuxMood = 0f;

                foreach (var events in CharacterEvents)
                {
                    //Extract result information to dictionario
                    var Event = emotionRegulationSimulated.ReName(events).EventTypeFatima; //aux
                    var eventToevaluated = Event.GetNTerm(3).ToString();
                    var strategy = string.Empty;
                    var result = (string.Empty, string.Empty);
                    eventNum++;

                    Console.WriteLine("\n\n                         EVENT: " + eventNum);
                    //Console.ReadKey();
                    var isApplied = emotionRegulationSimulated.EventMatchingAppraisal(ea_Pedro, Event.GetNTerm(3));
                    bool ForER = isApplied.IsEventNegative && !isApplied.IsEquals; ///IsEquals, revisa si la variable de valoración es Praiseworhiness,
                    /// y si se contraresta con Desirability

                    if (Decision == "y" && ForER)
                    {
                        StrategyWasApplied = emotionRegulationSimulated.SituationSelection(events);
                        if (StrategyWasApplied)
                        {
                            strategy = "Situation Selection";
                            result = (eventToevaluated, strategy);
                        }
                        if (!StrategyWasApplied)
                        {
                            StrategyWasApplied = emotionRegulationSimulated.SituationModification(events);
                            strategy = "Situation Modification";
                            result = (eventToevaluated, strategy);
                        }
                        if (!StrategyWasApplied)
                        {
                            StrategyWasApplied = emotionRegulationSimulated.AttentionDeployment(events);
                            strategy = "Attention Deployment";
                            result = (eventToevaluated, strategy);

                        }
                        if (!StrategyWasApplied)
                        {
                            StrategyWasApplied = emotionRegulationSimulated.CognitiveChange(events);
                            strategy = "Cognitive Change";
                            result = (eventToevaluated, strategy);
                        }
                        ea_Pedro = emotionRegulationSimulated.NewEA_character;
                        edm_Pedro = emotionRegulationSimulated.NewEdm_character;
                        am_Pedro = emotionRegulationSimulated.NewAm_character;
                        emotionalState_Pedro = emotionRegulationSimulated.NewEmotionalState_Character;
                        Event = emotionRegulationSimulated.EventFatima;
                    }

                    //Fatima Simulation
                    Console.WriteLine("\n----------------------------------------------------------------");

                    var j = emotionalState_Pedro.GetAllEmotions().Count(); ///Se uso para cuando no se generaba una nueva emoción, es decir,
                    ///para cuando la emoción del evento anterior y la actual es la misma y sólo cambiaba la intensidad debido al decaimiento.
                    ///Se utiliza para crear el dataset y graficar.
                    ea_Pedro.AppraiseEvents(new[] { Event }, emotionalState_Pedro, am_Pedro, kb_Pedro, Goals); 
                    Console.WriteLine(" \n Pedro's perspective ");
                    Console.WriteLine(" \n Events occured so far: "
                        + string.Concat(am_Pedro.RecallAllEvents().Select(e => "\n Id: "
                        + e.Id + " Event: " + e.EventName.ToString())));
                    var i = emotionalState_Pedro.GetAllEmotions().Count();///Se uso para cuando no se generaba una nueva emoción, es decir,
                    ///para cuando la emoción del evento anterior y la actual es la misma y sólo cambiaba la intensidad debido al decaimiento.
                    ///ToDo: revisar si esto sigue funcionando

                    if (!StrategyWasApplied && ForER && Decision == "y")
                    {
                        StrategyWasApplied = emotionRegulationSimulated.ResponseModulation(events);
                        strategy = "Response Modulation";
                        result = (eventToevaluated, strategy);
                    }
                    if (!StrategyWasApplied)
                    {
                        if (ForER)
                        {
                            string message = "The agent couldn't apply any strategy for the event : ";
                            Console.WriteLine(" \n  " + message.ToUpper() +
                                               Event.GetNTerm(3).ToString().ToUpper());
                        }
                        strategy = "None";
                        result = (eventToevaluated, strategy);
                    }
                    if (!(Decision == "y" && ForER)) { strategy = "None"; result = (eventToevaluated, strategy); }

                    //Show results strategies
                    Results.Add(result);

                    //Fatima Simulation
                    Console.WriteLine("\n\n----------------------------------------------------------------\n ");
                    ///para poder evaluar una nueva emoción como nula, el mood debe de ser cero, de lo contrario se dispara el mínimo valor
                    ///generado por FAtiMA
                    if (Results.LastOrDefault().strategy == "Situation Selection")
                        emotionalState_Pedro.Mood = AuxMood;
                    am_Pedro.Tick++;
                    emotionalState_Pedro.Decay(am_Pedro.Tick);
                    Console.WriteLine(" \n  Mood on tick '" + am_Pedro.Tick + "': " + emotionalState_Pedro.Mood);
                    Console.WriteLine("  Active Emotions \n  "
                    + string.Concat(emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));
                    ea_Pedro.Save();
                    AuxMood = emotionalState_Pedro.Mood;
                    Console.WriteLine("\n-------------------------- RESUMEN ----------------------------\n ");
                    foreach (var r in Results)
                        Console.WriteLine(r);
                    Console.WriteLine("\n----------------------------------------------------------------\n ");

                    var EmotionName = emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType).LastOrDefault();

                    //writing in data frame
                    var Toexcel = Results.Select(r => r.strategy);
                    var s = (Toexcel.Any());

                    if (!string.IsNullOrEmpty(EmotionName) && s && !(i == j))
                    {
                        df.Rows.Add(emotionalState_Pedro.Mood,
                            EmotionName,
                            emotionalState_Pedro.GetAllEmotions().Select(e => e.Intensity).LastOrDefault(), Event.GetNTerm(3).ToString(),
                            Toexcel.ElementAt(index),null,null,null);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(EmotionName)&& !(i == j))
                        {
                            df.Rows.Add(emotionalState_Pedro.Mood,
                                EmotionName,
                                emotionalState_Pedro.GetAllEmotions().Select(e => e.Intensity).LastOrDefault(), Event.GetNTerm(3).ToString(),
                               null, null, null, null);
                        }
                        else 
                        {
                            df.Rows.Add(emotionalState_Pedro.Mood,
                                    "--",
                                    0.0, Event.GetNTerm(3).ToString(), Toexcel.ElementAt(index), null, null, null);
                        }
                    }
                    index++;
                    Console.WriteLine(" ====> NEXT EVENT...");
                }

                //New columns in data set
                var DominantPersonality = _Personality.DominantPersonality;
                var StrategyPower = _Personality.DStrategyPower;
                var PersonalitiesType = _Personality.List_PersonalityType;
                var IsNull = _Personality.TfuzzyResults.PersonalityType == null;
                if (!IsNull)
                {
                    foreach (var personality in PersonalitiesType)
                    {

                        df.Rows.Add(null, null, null, null, null, personality);
                    }
                    foreach (var strategy in StrategyPower)
                    {
                        df.Rows.Add(null, null, null, null, null, null, strategy);
                    }
                    df.Rows.Add(null, null, null, null, null, null, null, DominantPersonality);
                }
                
                //save information of data frame
                oSLDocument.ImportDataTable(1, 1, df, true);
                oSLDocument.SaveAs(pathFile);
                
            }
            void PastEventSimulation()
            {
                Console.WriteLine("\nPAST EVENTS: ");
                Console.WriteLine("----------------------------------------------------");
                foreach (var e in PastCharacterEvents)
                {
                    ea_Pedro.AppraiseEvents(new[] { e }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);
                    Console.WriteLine(" \n Pedro's perspective ");
                    Console.WriteLine(" \n Events occured so far: "
                        + string.Concat(am_Pedro.RecallAllEvents().Select(e => "\n Id: "
                        + e.Id + " Event: " + e.EventName.ToString())));

                    am_Pedro.Tick++;
                    emotionalState_Pedro.Decay(am_Pedro.Tick);
                    Console.WriteLine(" \n  Mood on tick '" + am_Pedro.Tick + "': " + emotionalState_Pedro.Mood);
                    Console.WriteLine("  Active Emotions \n  "
                            + string.Concat(emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));
                    ea_Pedro.Save();
                    for (int i = 0; i < 120; i++)
                    {
                        am_Pedro.Tick++;
                        emotionalState_Pedro.Decay(am_Pedro.Tick);
                        //Console.WriteLine("\nMood on tick '" + am_Pedro.Tick + "': " + emotionalState_Pedro.Mood);
                        //Console.WriteLine("Active Emotions: " + string.Concat(emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity)));
                    }
                }
                Console.WriteLine("----------------------------------------------------\n\n");
            }
        }
        #endregion
    }
}

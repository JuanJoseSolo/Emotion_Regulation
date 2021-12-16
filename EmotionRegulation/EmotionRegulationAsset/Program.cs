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

namespace EmotionRegulationAsset
{
    class Program
    {
        public static List<(string events, string strategy)> Results = new();
        public static string path { get; set; }
        static void Main(string[] args)
        {
            var Origen = "C:/Users/JuanJoseAsus/source/repos/FAtiMA-Toolkit-master/EmotionRegulation/EmotionRegulationAsset/Results/";
            var ER_utilities = new ERutilities();
            var StrategyWasApplied = new Boolean();
            var _Personality = new PersonalityTraits();
            
            //Pedro   
            var am_Pedro = new AM();
            var kb_Pedro = new KB((Name)"Pedro");
            var storage = new AssetStorage();
            var edm_Pedro = EmotionalDecisionMakingAsset.CreateInstance(storage);
            var emotionalState_Pedro = new ConcreteEmotionalState();
            var rpc_Pedro = new RolePlayCharacterAsset();
            EmotionalAppraisalAsset ea_Pedro = EmotionalAppraisalAsset.CreateInstance(new AssetStorage());

            var wm = new WorldModelAsset();

            //Sarah
            var am_Sarah = new AM();
            var kb_Sarah = new KB((Name)"Sarah");
            var edm_Sarah = EmotionalDecisionMakingAsset.CreateInstance(storage);
            var emotionalState_Sarah = new ConcreteEmotionalState();
            var rpc_Sarah = new RolePlayCharacterAsset();
            EmotionalAppraisalAsset ea_Sarah = EmotionalAppraisalAsset.CreateInstance(new AssetStorage());

            //knowledgeBase Pedro
            kb_Pedro.Tell(Name.BuildName("Like(Sarah)"), Name.BuildName("True"), Name.BuildName("SELF"), 1);
            kb_Pedro.Tell(Name.BuildName("Dislike(Usuario)"), Name.BuildName("True"), Name.BuildName("SELF"), 1);
            kb_Pedro.Tell(Name.BuildName("Current(Location)"), Name.BuildName("Office"), Name.BuildName("SELF"), 1);
            edm_Pedro.RegisterKnowledgeBase(kb_Pedro);

            //knowledgeBase Sarah
            kb_Sarah.Tell(Name.BuildName("Current(Location)"), Name.BuildName("Office"), Name.BuildName("SELF"), 1);
            kb_Sarah.Tell(Name.BuildName("See(Someone)"), Name.BuildName("False"), Name.BuildName("SELF"), 1);
            edm_Sarah.RegisterKnowledgeBase(kb_Sarah);

            //Show knowledge Base in console
            void KBasePedro()
            {
                var nameBelief = kb_Pedro.GetAllBeliefs().Select(B => B.Name);
                var ValueBelief = kb_Pedro.GetAllBeliefs().Select(B => B.Value);
                var index = 0;
                foreach (var Bel in nameBelief)
                {
                    Console.WriteLine("Knownledge Base Pedro: " + Bel + " Value = " + ValueBelief.ElementAt(index));
                    index += 1;
                }
            }
            void KBaseSarah()
            {
                var nameBelief = kb_Sarah.GetAllBeliefs().Select(B => B.Name);
                var ValueBelief = kb_Sarah.GetAllBeliefs().Select(B => B.Value);
                var index = 0;
                foreach (var Bel in nameBelief)
                {
                    Console.WriteLine("Knownledge Base Sarah: " + Bel + " Value = " + ValueBelief.ElementAt(index).ToString().Trim());
                    index += 1;
                }
            }
            KBasePedro();
            Console.WriteLine("\n");
            KBaseSarah();

            //Past events
            var Party = Name.BuildName("Event(Action-End, Pedro, Party, Office)");//Office
            var Workmates = Name.BuildName("Event(Action-End, Pedro, Workmates, Office)");//Office
            var Jobs = Name.BuildName("Event(Action-End, Pedro, OtherWorks, Boss)");//Boss
            var AnotherBye = Name.BuildName("Event(Action-End, Pedro, AnotherBye, Sarah)");//Sarah

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

            //SequenceEvents
            List<Name> PastCharacterEvents = new() { Party, Workmates, AnotherBye };
            List<Name> CharacterEvents = new() 
            {
                TalktoBoss, Hello, Conversation, Hug, Discussion, Congrat, Bye, Fired, Crash, Profits,
                Fly
            };

            //Appraisal events and Action configuarations
            void PastEventEvaluation()
            {
                // EVENT = ENTER 
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
                    Name = "Desirability", Value = (Name.BuildName(3))
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
            }
            void PedroEventEvaluation()
            {
                /////// EVENT = ENTER //////
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

                /////// EVENT = PRAISE //////
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
                ////// EVENT = Fly //////
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                    new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                    {
                        Name = OCCAppraisalVariables.PRAISEWORTHINESS,
                        Value = (Name.BuildName(-6)),
                        Target = (Name)"SELF"
                    },
                    new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                    {
                        Name = OCCAppraisalVariables.DESIRABILITY,
                        Value = (Name.BuildName(-6)),
                    }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Fly, *)",
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
            }
            PedroActions();

            //--------------------//
            Console.WriteLine("\n=====> Do you want to use Emotion Regulation Asset? -----> y/N ");
            //var Decision = Console.ReadLine();
            var Decision = "y";

            EmotionRegulation emotionRegulationSimulated = new();
            if (Decision == "y")
            {
                //Emotional Appraisal Events, Creates events in character memory
                PedroEventEvaluation();
                PastEventEvaluation();
                //PastEventSimulation();

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
                var TalktoBoss_ER = Name.BuildName("Event(Action-End, Pedro, Talk, Boss, [True])");
                var Hello_ER = Name.BuildName("Event(Action-End, Pedro, Hello, Sarah, [False])");
                var Discussion_ER = Name.BuildName("Event(Action-End, Pedro, Discussion, Others, [True])");
                var Bye_ER = Name.BuildName("Event(Action-End, Pedro, Bye, Sarah, [False])");
                var Congrat_ER = Name.BuildName("Event(Action-End, Pedro, Congrat, Sarah, [False])");
                var Conversation_ER = Name.BuildName("Event(Action-End, Pedro, Conversation, Sarah, [False])");
                var Hug_ER = Name.BuildName("Event(Action-End, Pedro, Hug, Sarah, [False])");
                var Fired_ER = Name.BuildName("Event(Action-End, Pedro, Fired, Boss, [False])");
                var Crash_ER = Name.BuildName("Event(Action-End, Pedro, Crash, Car, [False])");
                var Profits_ER = Name.BuildName("Event(Action-End, Pedro, Profits, Cash, [False])");
                var Fly_ER = Name.BuildName("Event(Action-End, Pedro, Fly, Sky, [False])");

                //Action for Emotion Regulation
                Dictionary<string, string> Dictionary_relatedActionEvent = new();
                void ActionPedro_ER()
                {
                    //Action for the event: Enter 
                    var ActionEventEnter = "Enter_Faster | Enter";
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

                    //Action for the event: Enter 
                    var ActionEventEnter2 = "wait | Enter";
                    var TeventActionEnter2 = ER_utilities.SplitAction(ActionEventEnter2);

                    var ER_EnterAction2 = new ActionRuleDTO
                    {
                        Action = Name.BuildName(TeventActionEnter2.relatedAction),
                        Priority = Name.BuildName("1"),
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
                    //var ruleEDM2 = edm_Pedro.GetActionRule(idER_Bye);
                    edm_Pedro.AddRuleCondition(idER_Bye, "Like(Sarah) = True");
                    edm_Pedro.Save();
                    Dictionary_relatedActionEvent.Add(DiccEventActionBye.relatedAction, DiccEventActionBye.eventName);

                    /*
                    //Action for event Fired
                    var ActionNameFired = "ToTalk|Fired";
                    var DiccEventActionFired = ER_utilities.SplitAction(ActionNameFired);
                    var ER_FiredAction = new ActionRuleDTO
                    {
                        Action = Name.BuildName(DiccEventActionFired.relatedAction),
                        Priority = Name.BuildName("1"),
                        Target = (Name)"Boss",
                    };
                    var idER_Fired = edm_Pedro.AddActionRule(ER_FiredAction);
                    //var ruleEDM2 = edm_Pedro.GetActionRule(idR2);
                    edm_Pedro.AddRuleCondition(idER_Fired, "Current(Location) = Office");
                    edm_Pedro.Save();
                    Dictionary_relatedEvents.Add(DiccEventActionFired.relatedAction, DiccEventActionFired.eventName);
                    */
                }
                //ActionPedro_ER();

                //Events for alternative events (cognitive change strategy)
                var Event1 = Name.BuildName("Event(Action-End, Pedro, FindNewJob, Fired)");
                var Event2 = Name.BuildName("Event(Action-End, Pedro, MeetNewPeople, Fired)");
                var Event3 = Name.BuildName("Event(Action-End, Pedro, BetterSalary, Fired)");
                var Event4 = Name.BuildName("Event(Action-End, Pedro, BetterSalary, Talk)");
                var Event5 = Name.BuildName("Event(Action-End, Pedro, NewCar, Crash)");
                var Event6 = Name.BuildName("Event(Action-End, Pedro, ShewillBack, Bye)");
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
                        new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "like", Value = (Name.BuildName(6)) }
                    };
                    rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                    {
                        EventMatchingTemplate = (Name)"Event(Action-End, *, ShewillBack, *)",
                        AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                    };
                    ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
                }

                EventReinterpret();
                List<Name> LAlternativeEvents = new()
                {
                    // Event1, Event2, Event3, Event4, Event5, Event6
                };

                //List of events for the Emotion Regulation Asset
                List<Name> List_EventsER = new()
                {
                    TalktoBoss_ER, Hello_ER, Conversation_ER, Hug_ER, Discussion_ER, Congrat_ER, 
                    Bye_ER, Fired_ER, Crash_ER, Profits_ER, Fly_ER

                };

                //Personalities
                ///// C = 20, E = 25, N = 15, O = 12, A = 30; // max=95
                float C = 100, E = 0, N = 15, O = 0, A = 30;
                PersonalityTraits personalityTraitsPedro = new(C, E, N, O, A);
                    personalityTraitsPedro.FuzzyAppliedStrategyTest();
                
                //Personality information for Data frame 
                _Personality = personalityTraitsPedro;

                //Emotion Regulation Asset
                EmotionRegulation emotionRegulation = new(
                   ea_Pedro, edm_Pedro, am_Pedro, emotionalState_Pedro, 
                   personalityTraitsPedro, Dictionary_relatedActionEvent, 
                   LAlternativeEvents, 3);

                //path df
                string DominantPerson = personalityTraitsPedro.DominantPersonality;
                path = Origen + DominantPerson + ".xlsx";

                //return control to Fatima
                emotionRegulationSimulated = emotionRegulation;
                CharacterEvents = List_EventsER;
                //Simulate events
                EvaluationSeveralEvents();
            }
            else
            {
                //Events without ERA
                path = Origen + "NormalEvents.xlsx";
                PastEventSimulation();
                PastEventEvaluation();
                PedroEventEvaluation();                
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
                    float IsNegative = emotionRegulationSimulated.EventMatchingName(ea_Pedro, Event.GetNTerm(3)).AppraisalValue;
                    
                    if (Decision == "y" && IsNegative < 0)
                    {
                        StrategyWasApplied = emotionRegulationSimulated.SituationSelection(Event);
                        if (StrategyWasApplied)
                        {
                            strategy = "Situation Selection";
                            result = (eventToevaluated, strategy);
                        }
                        if (!StrategyWasApplied)
                        {
                            StrategyWasApplied = emotionRegulationSimulated.SituationModification(Event);
                            strategy = "Situation Modification";
                            result = (eventToevaluated, strategy);

                        }
                        if (!StrategyWasApplied)
                        {
                            StrategyWasApplied = emotionRegulationSimulated.AttentionDeployment(Event);
                            strategy = "Attention Deployment";
                            result = (eventToevaluated, strategy);

                        }
                        if (!StrategyWasApplied)
                        {
                            StrategyWasApplied = emotionRegulationSimulated.CognitiveChange(Event);
                            strategy = "Cognitive Change";
                            result = (eventToevaluated, strategy);
                        }
                        ea_Pedro  = emotionRegulationSimulated.NewEA_character;
                        edm_Pedro = emotionRegulationSimulated.NewEdm_character;
                        am_Pedro  = emotionRegulationSimulated.NewAm_character;
                        emotionalState_Pedro = emotionRegulationSimulated.NewEmotionalState_Character;
                        Event     = emotionRegulationSimulated.EventFatima;
                    }

                    //Fatima Simulation
                    Console.WriteLine("\n----------------------------------------------------------------");

                    ea_Pedro.AppraiseEvents(new[] { Event }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);
                    Console.WriteLine(" \n Pedro's perspective ");
                    Console.WriteLine(" \n Events occured so far: "
                        + string.Concat(am_Pedro.RecallAllEvents().Select(e => "\n Id: "
                        + e.Id + " Event: " + e.EventName.ToString())));

                    if (!StrategyWasApplied && IsNegative < 0 && Decision == "y")
                    {
                        StrategyWasApplied = emotionRegulationSimulated.Test4(Event);
                        strategy = "Response Modulation";
                        result = (eventToevaluated, strategy);
                    }
                    if (!StrategyWasApplied)
                    {
                        if (IsNegative < 0) 
                        {
                            string message = "The agent couldn't apply any strategy for the event : ";
                            Console.WriteLine(" \n  " + message.ToUpper() +
                                               Event.GetNTerm(3).ToString().ToUpper()); 
                        }
                        strategy = "None";
                        result = (eventToevaluated, strategy);
                    }
                    if(!(Decision == "y" && IsNegative < 0)){ strategy = "None";result = (eventToevaluated, strategy);}
                    
                    //Show results strategies
                    Results.Add(result);
                    Console.WriteLine("\n-------------------------- RESUMEN ----------------------------\n ");
                    foreach (var r in Results)
                        Console.WriteLine(r);

                    //Fatima Simulation
                    am_Pedro.Tick++;
                    emotionalState_Pedro.Decay(am_Pedro.Tick);
                    Console.WriteLine(" \n  Mood on tick '" + am_Pedro.Tick + "': " + emotionalState_Pedro.Mood);
                    Console.WriteLine("  Active Emotions \n  "
                    + string.Concat(emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));
                    ea_Pedro.Save();
                    Console.WriteLine("\n----------------------------------------------------------------");

                    //writing in data frame
                    var EmotionName = emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType).LastOrDefault();
                    var Toexcel = Results.Select(r => r.strategy);
                    var s = (Toexcel.Any());

                    if (!string.IsNullOrEmpty(EmotionName) && s )
                    {
                        df.Rows.Add(emotionalState_Pedro.Mood,
                            EmotionName,
                            emotionalState_Pedro.GetAllEmotions().Select(e => e.Intensity).LastOrDefault(), Event.GetNTerm(3).ToString(),
                            Toexcel.ElementAt(index),null,null,null);
                    }
                    else
                    {

                        if (!string.IsNullOrEmpty(EmotionName))
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
                    //Console.WriteLine("----------------------------------------------------------------");
                    Console.WriteLine(" ====> NEXT EVENT...");
                }

                //New columns in data set
                var DominantPersonality = _Personality.DominantPersonality;
                var StrategyPower = _Personality.DStrategyAndPower;
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
    }
}

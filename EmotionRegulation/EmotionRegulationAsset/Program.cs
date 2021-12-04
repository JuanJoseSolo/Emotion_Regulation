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

namespace EmotionRegulationAsset
{
    class Program
    {
        public static List<(string, string)> ActionToFatima = new();
        public static List<(string events, string strategy)> Results = new();
        public static string path { get; set; }
        static void Main(string[] args)
        {
            Console.WriteLine("\n-------------------------------MAIN---------------------------------\n");



            List<IAction> _actions = new List<IAction>();
            var ER_utilities = new ERutilities();
            var StrategyWasApplied = new Boolean();
            var _Personality = new PersonalityTraits(0f,0f,0f,0f,0f);
            //Pedro   
            var am_Pedro = new AM();
            var kb_Pedro = new KB((Name)"Pedro");
            var storage = new AssetStorage();
            var edm_Pedro = EmotionalDecisionMakingAsset.CreateInstance(storage);
            var wm = new WorldModelAsset();
            var emotionalState_Pedro = new ConcreteEmotionalState();
            var rpc_Pedro = new RolePlayCharacterAsset();
            EmotionalAppraisalAsset ea_Pedro = EmotionalAppraisalAsset.CreateInstance(new AssetStorage());

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
            var Party = Name.BuildName("Event(Action-End, Pedro, Party, Office)");
            var Workmates = Name.BuildName("Event(Action-End, Pedro, Workmates, Office)");
            var Jobs = Name.BuildName("Event(Action-End, Pedro, OtherWork, Boss)");
            //Events of the day
            var Enter = Name.BuildName("Event(Action-End, Pedro, Enter, Office)");
            var Hello = Name.BuildName("Event(Action-End, Sarah, Hello, Pedro)");
            var Discussion = Name.BuildName("Event(Action-End, Pedro, Discussion, Others)");
            var Bye = Name.BuildName("Event(Action-End, Pedro, Bye, Sarah)");
            var Congrat = Name.BuildName("Event(Action-End, Pedro, Congrat, Sarah)");
            var Conversation = Name.BuildName("Event(Action-End, Pedro, Conversation, Sarah)");
            var Hug = Name.BuildName("Event(Action-End, Pedro, Hug, Sarah)");
            var Fired = Name.BuildName("Event(Action-End, Pedro, Fired, Boss)");
            var Crash = Name.BuildName("Event(Action-End, Pedro, Crash, Car)");
            var Profits = Name.BuildName("Event(Action-End, Pedro, Profits, Cash)");
            //SequenceEvents
            List<Name> PastCharacterEvents = new() { Party,Workmates, Jobs };
            List<Name> CharacterEvents = new() {
                Enter, Hello, Conversation, Hug, Discussion, Congrat, Bye, Fired, Crash, Profits };

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

                    EventMatchingTemplate = (Name)"Event(Action-End, *, OtherWork, Boss)",
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
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Enter, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);

                ////////// EVENT = HELLO ///////
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                {
                    Name = "Like", Value = (Name.BuildName(2))
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
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(-7)) }
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
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Like", Value = (Name.BuildName(-7)) }
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
            }
            

            void SarahEvents()
            {
                /////// EVENT = HELLO //////
                var appraisalVariableDTO_Sarah = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                { Name = "Desirability", Value = (Name.BuildName(1)) }
                };
                var rule_Sarah = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Hello, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO_Sarah),
                };
                ea_Sarah.AddOrUpdateAppraisalRule(rule_Sarah);
            }
            void PedroActions()
            {
                Console.WriteLine("\nPerspective: Pedro");
                //Action 1
                var EnterOffice = new ActionRuleDTO
                {
                    Action = Name.BuildName("Enter"),
                    Priority = Name.BuildName("1"),
                    Target = (Name)"Office",
                };
                var actype = EnterOffice.GetType();
                var id = edm_Pedro.AddActionRule(EnterOffice);
                var ruleEDM = edm_Pedro.GetActionRule(id);
                edm_Pedro.AddRuleCondition(id, "Current(Location) = Office");
                edm_Pedro.Save();

            }
            void SarahActions()
            {
                Console.WriteLine("\nPerspective: Sarah");
                //Action 1
                var Hello = new ActionRuleDTO
                {
                    Action = Name.BuildName("Hello"),
                    Priority = Name.BuildName("1"),
                    Target = (Name)"Pedro",
                };
                var actype = Hello.GetType();
                var id = edm_Sarah.AddActionRule(Hello);
                //var ruleEDM = edm_Sarah.GetActionRule(id);
                edm_Sarah.AddRuleCondition(id, "Current(Location) = Office");
                //edm_Sarah.AddRuleCondition(id, "Event(Action-End) = Not-Enter");
                edm_Sarah.Save();
                var Layer = Hello.Layer;


                var decisions = edm_Sarah.Decide(Layer);
                _actions.Add(decisions.FirstOrDefault());
                foreach (var decision in decisions)
                {
                    Console.WriteLine("Decisions = " + decision.Name);
                }
            }
            void SarahAction() 
            {

                //Action 2
                var AnnouncePromotion = new ActionRuleDTO
                {
                    Action = Name.BuildName("AnnouncePromotion"),
                    Priority = Name.BuildName("1"),
                    Target = (Name)"Pedro",
                };
                var actype = AnnouncePromotion.GetType();
                var id = edm_Sarah.AddActionRule(AnnouncePromotion);
                var ruleEDM = edm_Sarah.GetActionRule(id);
                edm_Sarah.AddRuleCondition(id, "Current(Location) = Office");
                edm_Sarah.Save();
                var Layer = AnnouncePromotion.Layer;


                var decisions = edm_Sarah.Decide(Layer);

                foreach (var decision in decisions)
                {
                    Console.WriteLine("Decisions = " + decision.Name);
                }

            }
            void WorldModel()
            {
                
                //// WORLD MODEL
                var effectList = new List<EffectDTO>();
                var _eventTemplate = WellFormedNames.Name.BuildName(
                (Name)AMConsts.EVENT,
                (Name)"Action-End",
                (Name)"[s]",
                (Name)"Enter",
                (Name)"[O]");

                wm.addActionTemplate(_eventTemplate, 1);

                effectList.Add(new EffectDTO()
                {
                    PropertyName = (Name)"Current(Location)",
                    NewValue = (Name)"Casa",
                    ObserverAgent = (Name)"[s]"
                });


                var changed = new[] { EventHelper.ActionEnd(rpc_Sarah.CharacterName.ToString(), "Enter", "Office") };
                rpc_Sarah.Perceive(changed);
                rpc_Sarah.Perceive(CharacterEvents);
                var decisions = rpc_Pedro.Decide();
                _actions.Add(decisions.FirstOrDefault());
                foreach (var act in decisions)
                {
                    Console.WriteLine(rpc_Pedro.CharacterName + " has this action: " + act.Name);

                }


                wm.AddActionEffectsDTOs(_eventTemplate, effectList);

                var efects = wm.Simulate(CharacterEvents.ToArray()).ElementAt(0);

                var a1 = EventHelper.ActionEnd("Pedro", "Enter", "Office");
                var a  = EventHelper.PropertyChange(efects.PropertyName, efects.NewValue, (Name)"Pedro");

                //Console.WriteLine("Efectos = " + efects);
                Console.WriteLine("Efectos = " + a1);
                //Console.WriteLine("Efectos = " + a);

                foreach (var be in kb_Pedro.GetAllBeliefs())
                {
                    Console.WriteLine("Believes: " +be.Value);
                }
                
            }

            //--------------------//
            Console.WriteLine("\n=====> Do you want to use Emotion Regulation Asset? -----> y/N ");
            var Decision = Console.ReadLine();
            //var Decision = "qy";

            if (Decision == "y")
            {
                //Emotional Appraisal Events, Creates events in character memory
                PedroEventEvaluation();
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
                var Enter_ER = Name.BuildName("Event(Action-End, Pedro, Enter, Office, True)");
                var Hello_ER = Name.BuildName("Event(Action-End, Sarah, Hello, Pedro)");
                var Discussion_ER = Name.BuildName("Event(Action-End, Pedro, Discussion , Others, True)");
                var Bye_ER   = Name.BuildName("Event(Action-End, Pedro, Bye, Sarah)");
                var Congrat_ER = Name.BuildName("Event(Action-End, Pedro, Congrat, Sarah)");
                var Conversation_ER = Name.BuildName("Event(Action-End, Pedro, Conversation, Sarah)");
                var Hug_ER = Name.BuildName("Event(Action-End, Pedro, Hug, Sarah)");
                var Fired_ER = Name.BuildName("Event(Action-End, Pedro, Fired, Boss, False)");
                var Crash_ER = Name.BuildName("Event(Action-End, Pedro, Crash, Car)");
                var Profits_ER = Name.BuildName("Event(Action-End, Pedro, Profits, Cash)");
                
                //Action for Emotion Regulation
                Dictionary<string, string> Dictionary_relatedEvents = new();
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
                    //var ruleEDM2 = edm_Pedro.GetActionRule(idER_Bye);
                    edm_Pedro.AddRuleCondition(idER_Enter, "Like(Sarah) = True");
                    edm_Pedro.Save();
                    Dictionary_relatedEvents.Add(TeventActionEnter.relatedAction, TeventActionEnter.eventName);

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
                    Dictionary_relatedEvents.Add(DiccEventActionBye.relatedAction, DiccEventActionBye.eventName);

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
                ActionPedro_ER();
                

                //List of events with Emotion Regulation Asset
                List<Name> List_Events = new()
                { Enter_ER, Hello_ER, Conversation_ER, Hug_ER, Discussion_ER, Congrat_ER, Bye_ER, Fired_ER, Crash_ER, Profits_ER };

                //path df
                path = "C:/Users/JuanJoseAsus/source/repos/FAtiMA-Toolkit-master" +
                        "/EmotionRegulation/EmotionRegulationAsset/Conscientiousness.xlsx";

                //Personalities
                //Cons = 20, Extrav = 25, Neuro = 15, Oppen = 12, Agree = 35;
                float Cons = 90, Extrav = 25, Neuro = 15, Oppen = 12, Agree = 35;
                PersonalityTraits personalityTraitsPedro = new(Cons, Extrav, Neuro, Oppen, Agree);
                    personalityTraitsPedro.FuzzyAppliedStrategyTest();

                _Personality = personalityTraitsPedro;

                //Emotion Regulation Asset
                EmotionRegulation emotionRegulation = new(
                    List_Events, ea_Pedro, personalityTraitsPedro, Dictionary_relatedEvents, -1);

                List<Name> List_RegulatedEvents = new();
                //List<(string events, string strategy)> Results = new();
                _Personality = personalityTraitsPedro;
                foreach (var e in List_Events)
                {
                    var eventToevaluated = e.GetNTerm(3).ToString();
                    var strategy = string.Empty;
                    var result = (string.Empty, string.Empty);
                    StrategyWasApplied = emotionRegulation.SituationSelection(e);
                    
                    if (StrategyWasApplied)
                    {
                        strategy = "Situation Selection";
                        result = (eventToevaluated, strategy);
                        Console.WriteLine("addsss " + result.Item1);
                    }
                    if (!StrategyWasApplied)
                    {
                        StrategyWasApplied = emotionRegulation.SituationModification(edm_Pedro, e, -8f);
                        strategy = "Situation Modification";
                        result = (eventToevaluated, strategy);
                        
                    }
                    if (!StrategyWasApplied)
                    {
                        StrategyWasApplied = emotionRegulation.AttentionDeployment(e, am_Pedro, -8f);
                        strategy = "Attention Deployment";
                        result = (eventToevaluated, strategy);
                        
                    }
                    if(!StrategyWasApplied)
                    {
                        string message = "The agent couldn't apply any strategy for the event : ";
                        Console.WriteLine(" \n  " + message.ToUpper() +
                                           e.GetNTerm(3).ToString().ToUpper());

                        strategy = "None";
                        result = (eventToevaluated, strategy);
                       
                    }

                    Results.Add(result);

                    List_RegulatedEvents.Add(emotionRegulation.EventFatima);
                    ActionToFatima.Add(emotionRegulation.RelatedEventsToFatima);
                }

                Console.WriteLine("\n-------------------------- RESUMEN ----------------------------\n ");
                foreach (var r in Results)
                    Console.WriteLine(r);
                Console.WriteLine("\n----------------------------------------------------------------");


                Console.WriteLine("\n\n\n FAtiMA simulation" +
                    "\n--------------------------------------------------------------------");
                foreach (var e2 in List_RegulatedEvents)
                    Console.WriteLine("Nuevos eventos para FAtiMA: " + e2);

                //return control to Fatima
                CharacterEvents = List_RegulatedEvents;
                ea_Pedro = emotionRegulation.NewEA_character;
                edm_Pedro = emotionRegulation.NewEdm_character;
                

                EvaluationSeveralEvents();
            }
            else
            {
                path = "C:/Users/JuanJoseAsus/source/repos/FAtiMA-Toolkit-master/EmotionRegulation/EmotionRegulationAsset" +
                    "/HistoryWithoutER.xlsx";
                PastEventSimulation();
                PastEventEvaluation();
                PedroEventEvaluation();
                //SarahEvents();
                
                EvaluationSeveralEvents();
            }


            //Events Evaluation
            void FewEventsActions()
            {
                Console.WriteLine("\nEvent one: ");
                ea_Pedro.AppraiseEvents(new[] { Enter }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);
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
                
                if (Enter.GetNTerm(3).Equals((Name)"Enter"))
                {


                    SarahActions();

                    Console.WriteLine("\nEvent Two: ");
                    ea_Pedro.AppraiseEvents(new[] { Hello }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);
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

                    Console.WriteLine("\nEvent Two: ");
                    ea_Sarah.AppraiseEvents(new[] { Hello }, emotionalState_Sarah, am_Sarah, kb_Sarah, null);
                    Console.WriteLine(" \n Sarah's perspective ");
                    Console.WriteLine(" \n Events occured so far: "
                        + string.Concat(am_Sarah.RecallAllEvents().Select(e => "\n Id: "
                        + e.Id + " Event: " + e.EventName.ToString())));

                    am_Sarah.Tick++;
                    emotionalState_Sarah.Decay(am_Sarah.Tick);
                    Console.WriteLine(" \n  Mood on tick '" + am_Sarah.Tick + "': " + emotionalState_Sarah.Mood);
                    Console.WriteLine("  Active Emotions \n  "
                            + string.Concat(emotionalState_Sarah.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));
                    ea_Sarah.Save();
                }
                Console.ReadKey();
            }
            void EvaluationSeveralEvents()
            {                
                //df
                string pathFile = AppDomain.CurrentDomain.DynamicDirectory + path;
                SLDocument oSLDocument = new SLDocument();
                System.Data.DataTable df = new System.Data.DataTable();
                //columnas
                df.Columns.Add("MOOD     ", typeof(float));
                df.Columns.Add("EMOTION  ", typeof(string));
                df.Columns.Add("INTENSITY", typeof(float));
                df.Columns.Add("   EVENT    ", typeof(string));
                df.Columns.Add(" APPLIED STRATEGY    ", typeof(string));
                df.Columns.Add(" PERSONALITY TRAITS ", typeof(string));
                df.Columns.Add(" STRATEGIES RELATED ", typeof(string));
                df.Columns.Add(" DOMINANT PERSONALITY ", typeof(string));
                

                Console.WriteLine("\n ACTUAL EVENTS: ");
                Console.WriteLine("--------------------------------------------------------------------");
                Console.WriteLine(" Character: "+ kb_Pedro.Perspective.ToString().ToUpper());
                var index = 0;

                foreach (var evento in CharacterEvents)
                {
                    ea_Pedro.AppraiseEvents(new[] { evento }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);
                    //Console.WriteLine(" \n Pedro's perspective ");
                    Console.WriteLine(" \n Events occured so far: "
                        + string.Concat(am_Pedro.RecallAllEvents().Select(e => "\n Id: "
                        + e.Id + " Event: " + e.EventName.ToString())));
                    
                    //simulate the action
                    if (ActionToFatima.Select(e => e.Item1 == evento.GetNTerm(3).ToString()).FirstOrDefault())
                    {
                        var eve = ActionToFatima.Where(e => e.Item1 == evento.GetNTerm(3).ToString()).FirstOrDefault();
                        var v = edm_Pedro.Decide(Name.UNIVERSAL_SYMBOL);
                        Console.WriteLine(" Action: " + v.Where(d => d.Name.ToString() == eve.Item2).FirstOrDefault());

                    }

                    am_Pedro.Tick++;
                    emotionalState_Pedro.Decay(am_Pedro.Tick);
                    Console.WriteLine(" \n  Mood on tick '" + am_Pedro.Tick + "': " + emotionalState_Pedro.Mood);
                    Console.WriteLine("  Active Emotions \n  "
                                + string.Concat(emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));
                    //SAVE DF
                    var EmotionName = emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType).LastOrDefault();
                    var Toexcel = Results.Select(r => r.strategy);
                    var s = (Toexcel.Any());
                    if (!string.IsNullOrEmpty(EmotionName) && s )
                    {
                        df.Rows.Add(emotionalState_Pedro.Mood,
                            EmotionName,
                            emotionalState_Pedro.GetAllEmotions().Select(e => e.Intensity).LastOrDefault(), evento.GetNTerm(3).ToString(),
                            Toexcel.ElementAt(index),null,null,null);
                    }
                    else
                    {

                        if (!string.IsNullOrEmpty(EmotionName))
                        {
                            df.Rows.Add(emotionalState_Pedro.Mood,
                                EmotionName,
                                emotionalState_Pedro.GetAllEmotions().Select(e => e.Intensity).LastOrDefault(), evento.GetNTerm(3).ToString(),
                               null, null, null, null);
                        }
                        else 
                        {
                            df.Rows.Add(emotionalState_Pedro.Mood,
                                    "--",
                                    0.0, evento.GetNTerm(3).ToString(), Toexcel.ElementAt(index), null, null, null);
                        }
                    }

                    Console.WriteLine("\nCheck decay emotion? y/N");
                    /*
                    if (Console.ReadLine() == "y")
                    {
                        Console.WriteLine("How long? ");
                        var tick = Console.ReadLine();
                        var t = int.Parse(tick);
                        for (int i = 0; i < t; i++)
                        {
                            am_Pedro.Tick++;
                            emotionalState_Pedro.Decay(am_Pedro.Tick);
                            Console.WriteLine("\nMood on tick '" + am_Pedro.Tick + "': " + emotionalState_Pedro.Mood);
                            Console.WriteLine("Active Emotions: " + string.Concat(emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity)));
                        }
                    }

                    */
                    index++;
                    ea_Pedro.Save();
                    Console.WriteLine("----------------------------------------------------------------");
                    
                }
                var DominantPersonality = _Personality.DominantPersonality;
                var StrategyPower = _Personality.DStrategyAndPower;
                var PersonalitiesType = _Personality.List_PersonalityType;

                foreach (var personality in PersonalitiesType)
                {

                    df.Rows.Add(null, null, null, null, null, personality);
                }
                foreach(var strategy in StrategyPower)
                {
                    df.Rows.Add(null, null, null, null, null, null, strategy);
                }
                df.Rows.Add(null, null, null, null, null, null, null, DominantPersonality);


                oSLDocument.ImportDataTable(1, 1, df, true);
                
                oSLDocument.SaveAs(pathFile);
                //Console.ReadKey();
            }
            void FewEventsActionsER()
            {
                Console.WriteLine("\nEvent one: ");
                ea_Pedro.AppraiseEvents(new[] { Enter }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);
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

                if (Enter.GetNTerm(3).Equals((Name)"Enter"))
                {
                    SarahActions();

                    Console.WriteLine("\nEvent Two: ");
                    ea_Pedro.AppraiseEvents(new[] { Hello }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);
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

                    Console.WriteLine("\nEvent Two: ");
                    ea_Sarah.AppraiseEvents(new[] { Hello }, emotionalState_Sarah, am_Sarah, kb_Sarah, null);
                    Console.WriteLine(" \n Sarah's perspective ");
                    Console.WriteLine(" \n Events occured so far: "
                        + string.Concat(am_Sarah.RecallAllEvents().Select(e => "\n Id: "
                        + e.Id + " Event: " + e.EventName.ToString())));

                    am_Sarah.Tick++;
                    emotionalState_Sarah.Decay(am_Sarah.Tick);
                    Console.WriteLine(" \n  Mood on tick '" + am_Sarah.Tick + "': " + emotionalState_Sarah.Mood);
                    Console.WriteLine("  Active Emotions \n  "
                            + string.Concat(emotionalState_Sarah.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));
                    ea_Sarah.Save();
                }
                Console.ReadKey();
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

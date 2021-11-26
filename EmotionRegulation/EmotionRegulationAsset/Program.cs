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

namespace EmotionRegulationAsset
{
    class Program
    {
        public static List<(string,string)> ActionToFatima = new();
        static void Main(string[] args)
        {
            Console.WriteLine("\n-------------------------------MAIN---------------------------------\n");

            List<IAction> _actions = new List<IAction>();

            var ER_utilities = new ERutilities();
            var StrategyWasApply = new Boolean();


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
            //Events of the day
            var Enter = Name.BuildName("Event(Action-End, Pedro, Enter, Office)");
            var Hello = Name.BuildName("Event(Action-End, Sarah, Hello, Pedro)");
            var Argue = Name.BuildName("Event(Action-End, Pedro, Argue, Others)");
            var Bye = Name.BuildName("Event(Action-End, Pedro, Bye, Sarah)");
            var Praise = Name.BuildName("Event(Action-End, Pedro, Praise, Sarah)");
            var Conversation = Name.BuildName("Event(Action-End, Pedro, Conversation, Sarah)");
            var Hug = Name.BuildName("Event(Action-End, Pedro, Hug, Sarah)");
            var Fired = Name.BuildName("Event(Action-End, Boss, Fired, Pedro)");
            //SequenceEvents
            List<Name> PastEventsCharacter = new() { Party,Workmates };
            List<Name> EventsCharacter = new() { Enter, Hello, Conversation, Hug, Argue, Praise, Bye, Fired };

            //Appraisal events and Action configuarations
            void EventsPedro()
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
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Praise, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);

                /////// EVENT = Argue //////
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Like", Value = (Name.BuildName(-5)) }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Argue, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);

                /////// EVENT = Fired //////
                appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
                {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Like", Value = (Name.BuildName(-10)) }
                };
                rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
                {
                    EventMatchingTemplate = (Name)"Event(Action-End, *, Fired, *)",
                    AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
                };
                ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
            }
            void PastEvents()
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
            }

            void EventsSarah()
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

            void ActionsPedro()
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
            ActionsPedro();
            void ActionsSarah()
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
            void ActionSarah() 
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
                rpc_Sarah.Perceive(EventsCharacter);
                var decisions = rpc_Pedro.Decide();
                _actions.Add(decisions.FirstOrDefault());
                foreach (var act in decisions)
                {
                    Console.WriteLine(rpc_Pedro.CharacterName + " has this action: " + act.Name);

                }


                wm.AddActionEffectsDTOs(_eventTemplate, effectList);

                var efects = wm.Simulate(EventsCharacter.ToArray()).ElementAt(0);

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
            Console.WriteLine("\n=====> Do you want to use Emotion Regulation Asset? \n        ---> y/N ");
            var Decision = Console.ReadLine();
            

            if (Decision == "y")
            {
                //Create events in memory of character
                PastEvents();
                EvaluationPastEvent();

                ////Events For Emotion Regulation Asset
                var Enter_ER = Name.BuildName("Event(Action-End, Pedro, Enter, Office, False)");
                var Hello_ER = Name.BuildName("Event(Action-End, Sarah, Hello, Pedro)");
                var Argue_ER = Name.BuildName("Event(Action-End, Pedro, Argue , Others, True)");
                var Bye_ER   = Name.BuildName("Event(Action-End, Pedro, Bye, Sarah, True)");
                var Praise_ER = Name.BuildName("Event(Action-End, Pedro, Praise, Sarah)");
                var Conversation_ER = Name.BuildName("Event(Action-End, Pedro, Conversation, Sarah)");
                var Hug_ER = Name.BuildName("Event(Action-End, Pedro, Hug, Sarah)");
                var Fired_ER = Name.BuildName("Event(Action-End, Boss, Fired, Pedro, False)");
                //List of events with Emotion Regulation Asset
                List<Name> EventsCharacter_ER = new()
                { Enter_ER, Hello_ER, Conversation_ER, Hug_ER, Argue_ER, Praise_ER, Bye_ER, Fired_ER };

                //Emotional Appraisal Events
                EventsPedro();

                //Action for Emotion Regulation
                Dictionary<string,string> EventsRelated = new();
                
                void ActionPedro_ER()
                {
                    //Action for event Enter 
                    var ActionNameEnter = "Enter_Faster | Enter";
                    var DiccEventActionEnter= ER_utilities.SplitAction(ActionNameEnter);

                    var ER_EnterAction = new ActionRuleDTO
                    {
                        Action = Name.BuildName(DiccEventActionEnter.Item1),
                        Priority = Name.BuildName("1"),
                        Target = (Name)"Office",
                    };
                    var idER_Enter = edm_Pedro.AddActionRule(ER_EnterAction);
                    //var ruleEDM2 = edm_Pedro.GetActionRule(idER_Bye);
                    edm_Pedro.AddRuleCondition(idER_Enter, "Like(Sarah) = True");
                    edm_Pedro.Save();
                    EventsRelated.Add(DiccEventActionEnter.Item1, DiccEventActionEnter.Item2);


                    //Action for event Bye
                    var ActionNameBye = "ToHug | Bye";
                    var DiccEventActionBye = ER_utilities.SplitAction(ActionNameBye);

                    var ER_ByeAction = new ActionRuleDTO
                    {
                        Action = Name.BuildName(DiccEventActionBye.Item1),
                        Priority = Name.BuildName("1"),
                        Target = (Name)"Sarah",
                    };
                    var idER_Bye = edm_Pedro.AddActionRule(ER_ByeAction);
                    //var ruleEDM2 = edm_Pedro.GetActionRule(idER_Bye);
                    edm_Pedro.AddRuleCondition(idER_Bye, "Like(Sarah) = True");
                    edm_Pedro.Save();
                    EventsRelated.Add(DiccEventActionBye.Item1,DiccEventActionBye.Item2);

                    //Action for event Fired
                    var ActionNameFired = "ToTalk|Fired";
                    var DiccEventActionFired = ER_utilities.SplitAction(ActionNameFired);
                    var ER_FiredAction = new ActionRuleDTO
                    {
                        Action = Name.BuildName(DiccEventActionFired.Item1),
                        Priority = Name.BuildName("1"),
                        Target = (Name)"Sarah",
                    };
                    var idER_Fired = edm_Pedro.AddActionRule(ER_FiredAction);
                    //var ruleEDM2 = edm_Pedro.GetActionRule(idR2);
                    edm_Pedro.AddRuleCondition(idER_Fired, "Current(Location) = Office");
                    edm_Pedro.Save();
                    EventsRelated.Add(DiccEventActionFired.Item1,DiccEventActionFired.Item2);
                }
                ActionPedro_ER();

                //Personalities
                float Cons = 85, Extrav = 30, Neuro = 0, Oppen = 0, Agree = 0;
                PersonalityTraits personalityTraits = new(Cons, Extrav, Neuro, Oppen, Agree);
                personalityTraits.FuzzyAppliedStrategy();

                //Emotion Regulation Asset
                EmotionRegulation emotionRegulation = new(
                    EventsCharacter_ER, ea_Pedro, personalityTraits, EventsRelated, -2.5);

                List<Name> EventsCharacter_ER2 = new();

                foreach (var e in EventsCharacter_ER)
                {
                    StrategyWasApply = emotionRegulation.SituationSelection(e);

                    if (!StrategyWasApply)
                    {
                        StrategyWasApply = emotionRegulation.SituationModification(edm_Pedro, e, -2.5f);
                        Console.WriteLine("valor bool" + StrategyWasApply);
                    }
                    if (!StrategyWasApply)
                    {
                        StrategyWasApply = emotionRegulation.AttentionDeployment(e, am_Pedro, -2.5f);
                    }
                    else
                    {
                        Console.WriteLine("Without Emotianl Regulation");
                    }

                    EventsCharacter_ER2.Add(emotionRegulation.EventFatima);
                    ActionToFatima.Add(emotionRegulation.RelatedEventsToFatima);
                }
                
                Console.WriteLine("\n\n\n FAtiMA Control" +
                    "\n--------------------------------------------------------------------");
                foreach (var e2 in EventsCharacter_ER2)
                    Console.WriteLine("Nuevos eventos para FAtiMA: " + e2);
                
                //return control to Fatima
                EventsCharacter = EventsCharacter_ER2;
                ea_Pedro = emotionRegulation.NewEA_character;
                edm_Pedro = emotionRegulation.NewEdm_character;
                

                EvaluationSeveralEvents();
            }
            else
            {
                PastEvents();
                EventsPedro();
                EventsSarah();
                EvaluationPastEvent();
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
                    
                    
                    ActionsSarah();

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
                Console.WriteLine("\n ACTUAL EVENTS: ");
                Console.WriteLine("--------------------------------------------------------------------");
                foreach (var evento in EventsCharacter)
                {
                    ea_Pedro.AppraiseEvents(new[] { evento }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);
                    Console.WriteLine(" \n Pedro's perspective ");
                    Console.WriteLine(" \n Events occured so far: "
                        + string.Concat(am_Pedro.RecallAllEvents().Select(e => "\n Id: "
                        + e.Id + " Event: " + e.EventName.ToString())));

                    //simulate the action
                    if(ActionToFatima.Select(e => e.Item1 == evento.GetNTerm(3).ToString()).FirstOrDefault())
                    {
                        var eve = ActionToFatima.Where(e => e.Item1 == evento.GetNTerm(3).ToString()).FirstOrDefault();
                        var v = edm_Pedro.Decide(Name.UNIVERSAL_SYMBOL);
                        Console.WriteLine("Action: " + v.Where(d => d.Name.ToString() == eve.Item2).FirstOrDefault());

                    }

                    am_Pedro.Tick++;
                    emotionalState_Pedro.Decay(am_Pedro.Tick);
                    Console.WriteLine(" \n  Mood on tick '" + am_Pedro.Tick + "': " + emotionalState_Pedro.Mood);
                    Console.WriteLine("  Active Emotions \n  "
                                + string.Concat(emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));

                    Console.WriteLine("\nCheck decay emotion? y/N");

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
                    ea_Pedro.Save();
                    Console.ReadKey();
                    Console.WriteLine("----------------------------------------------------------------");
                }
                
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

                    
                    ActionsSarah();

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
            void EvaluationPastEvent()
            {
                Console.WriteLine("\nPAST EVENTS: ");
                Console.WriteLine("----------------------------------------------------");
                foreach (var e in PastEventsCharacter)
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

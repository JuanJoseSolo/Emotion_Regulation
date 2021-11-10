using System;
using System.Linq;
using EmotionalAppraisal;
using EmotionalDecisionMaking;
using AutobiographicMemory;
using WellFormedNames;
using KnowledgeBase;
using GAIPS.Rage;
using System.Collections.Generic;
using ActionLibrary.DTOs;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;

namespace EmotionRegulationAsset
{
    class Program
    {

        private static string IdeAction { get; set; }
        private static Name EventWaction { get; set; }

        public Name eventWithActionID { get; } = EventWaction;

        static void Main(string[] args)
        {
            Console.WriteLine("\n-------------------------------MAIN---------------------------------\n");
            //Program EmotionRegulation = new();

            ////   Character Pedro   /////
            var am_Pedro = new AM();
            var kb_Pedro = new KB((Name)"Pedro");
            //// Actions
            var storage = new AssetStorage();
            var edm = EmotionalDecisionMakingAsset.CreateInstance(storage);

            ///////   knowledge Base and Emotion Estate   /////////
            kb_Pedro.Tell(Name.BuildName("Like(Sarah)"), Name.BuildName("True"), Name.BuildName("SELF"), 1);
            kb_Pedro.Tell(Name.BuildName("Dislike(Usuario)"), Name.BuildName("True"), Name.BuildName("SELF"), 1);
            kb_Pedro.Tell(Name.BuildName("Location(Office)"), Name.BuildName("False"), Name.BuildName("SELF"), 1);
            kb_Pedro.Tell(Name.BuildName("Current(Location)"), Name.BuildName("Home"), Name.BuildName("SELF"), 1);
            var emotionalState_Pedro = new ConcreteEmotionalState();
            edm.RegisterKnowledgeBase(kb_Pedro);

            ////Events
            var EnterOffice   = Name.BuildName("Event(Action-End, Pedro, Enter, Office)");
            var Hello         = Name.BuildName("Event(Action-End, Pedro, Hello, Sarah)");
            var Bye           = Name.BuildName("Event(Action-End, Pedro, Bye, Sarah,True)");
            var FallDown      = Name.BuildName("Event(Action-End, Pedro, FallDown, somewhere)");
            var DialogueWith  = Name.BuildName("Event(Action-End, Pedro, DialogueWith, Sarah)");
            var hug           = Name.BuildName("Event(Action-End, Pedro, hug, Sarah)");


            List<Name> Eventos = new() { EnterOffice, Hello, FallDown, DialogueWith, hug };

            EmotionalAppraisalAsset ea_Pedro = EmotionalAppraisalAsset.CreateInstance(new AssetStorage());


            /////// EVENT 1 //////
            var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
            {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(-2)) }
            };
            var rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
            {
                EventMatchingTemplate = (Name)"Event(Action-End, *, Enter, *)",
                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
            };
            ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
            ////////// EVENT 2 ///////
            appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
            {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                {
                    Name = "Desirability", Value = (Name.BuildName(3))
                }
            };
            rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
            {

                EventMatchingTemplate = (Name)"Event(Action-End, *, Hello, *)",
                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
            };
            ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
            /////// EVENT 3 //////
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
            /////// EVENT 4 //////
            appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
            {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Like", Value = (Name.BuildName(4)) }
            };
            rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
            {
                EventMatchingTemplate = (Name)"Event(Action-End, *, DialogueWith, *)",
                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
            };
            ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
            /////// EVENT 5 //////
            appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
            {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Like", Value = (Name.BuildName(8)) }
            };
            rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
            {
                EventMatchingTemplate = (Name)"Event(Action-End, *, hug, *)",
                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
            };
            ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);
            /////// EVENT 6 //////
            appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
            {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Like", Value = (Name.BuildName(-4)) }
            };
            rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
            {
                EventMatchingTemplate = (Name)"Event(Action-End, *, FallDown, *)",
                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
            };
            ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);

            //Action 1
            var actionRule = new ActionRuleDTO
            {
                Action = Name.BuildName("ToHug"),
                Priority = Name.BuildName("3"),
                Target = (Name)"Sarah",
            };

            var id = edm.AddActionRule(actionRule);
            var ruleEDM = edm.GetActionRule(id);
            edm.AddRuleCondition(id, "Like(Sarah) = True");
            edm.Save();

            //Action 2
            var actionRule2 = new ActionRuleDTO
            {
                Action = Name.BuildName("Run"),
                Priority = Name.BuildName("1"),
                Target = (Name)"Sarah",
            };

            var idR2 = edm.AddActionRule(actionRule2);
            var ruleEDM2 = edm.GetActionRule(idR2);
            edm.AddRuleCondition(idR2, "Like(Sarah) = True");
            edm.Save();

            Console.WriteLine("         ACTIONS       ");
            foreach (var k in edm.GetAllActionRules())
            {
                Console.WriteLine("Actions in edm character: " + k.Action);
            }

            //Personality
            float Cons = 80, Extrav = 80, Neuro = 30, Oppen = 10, Agree = 35;
            //función provisional para capturar Id de los eventos
            EventAction();          

            PersonalityTraits personalityTraits = new(Cons, Extrav, Neuro, Oppen, Agree);
            personalityTraits.FuzzyAppliedStrategy();

            
            EmotionRegulation emotionRegulation = new(ea_Pedro, personalityTraits);




            foreach (var evento in Eventos)
            {
                ea_Pedro.AppraiseEvents(new[] { evento }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);
                Console.WriteLine(" \n Pedro's perspective ");
                Console.WriteLine(" \n Events occured so far: "
                    + string.Concat(am_Pedro.RecallAllEvents().Select(e => "\n Id: "
                    + e.Id + " Event: " + e.EventName.ToString())));

                am_Pedro.Tick++;
                emotionalState_Pedro.Decay(am_Pedro.Tick);
                Console.WriteLine(" \n  Mood on tick '" + am_Pedro.Tick + "': " + emotionalState_Pedro.Mood);
                Console.WriteLine("  Active Emotions \n  "
                        + string.Concat(emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity)));

                ea_Pedro.Save();
                Console.ReadKey();
            }

            //emotionRegulation.SituationSelection(Bye);
            emotionRegulation.SituationModification(edm, -8f);

            //Show
            /*
            Console.WriteLine("\n---------------------MAIN-------------------------\n");

            foreach(var r in emotionRegulation.NewEA_character.GetAllAppraisalRules())
            {
                Console.WriteLine("Valores recividos<>>> "+r.EventMatchingTemplate + "---->"+r.AppraisalVariables);
               
            }
            Console.WriteLine("\nValores recividos<>>> " + emotionRegulation.EventFatima);
            */

            emotionRegulation.AttentionDeployment(Bye, am_Pedro);



            Console.WriteLine("\nEVENTO A VALORAR");
            emotionRegulation.NewEA_character.AppraiseEvents(new[] { emotionRegulation.EventFatima }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);
            Console.WriteLine(" \n Pedro's perspective ");
            Console.WriteLine(" \n Events occured so far: "
                + string.Concat(am_Pedro.RecallAllEvents().Select(e => "\n Id: "
                + e.Id + " Event: " + e.EventName.ToString())));

            am_Pedro.Tick++;
            emotionalState_Pedro.Decay(am_Pedro.Tick);
            Console.WriteLine(" \n  Mood on tick '" + am_Pedro.Tick + "': " + emotionalState_Pedro.Mood);
            Console.WriteLine("  Active Emotions \n  "
                    + string.Concat(emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity)));

            ea_Pedro.Save();
            Console.ReadKey();



























            ///Aggregate the action Id to related event
            void EventAction()
            {
                Console.WriteLine("\n------------------------void EventAction()------------------------\n");
                var IdEvent = ea_Pedro.GetAllAppraisalRules().Where(
                    n => n.EventMatchingTemplate.GetNTerm(3).ToString().Equals("Bye")).FirstOrDefault().Id.ToString();
                Console.WriteLine("ID Event " + IdEvent);

                IdeAction = edm.GetAllActionRules().Where(n => n.Action.ToString().Equals("ToHug")).FirstOrDefault().Id.ToString();
                Console.WriteLine("ID Action " + IdeAction);

                var IDitem = Bye.GetLiterals().ToList();
                IDitem.Add(Name.BuildName(IdeAction));
                EventWaction = Name.BuildName(IDitem);
                //Bye.SwapTerms(Bye, EventWaction);
                Console.WriteLine("Evento con ID "+ EventWaction);
                return;
            }
        }
    }
}

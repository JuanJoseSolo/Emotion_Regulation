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
using Fuzzy_Personalities;

namespace TestEmotion
{
    public class MainTestER_01 : Strategies
    {
        public static string Emotion_Pedro = string.Empty;
        public static float  Intensity_Pedro = float.NaN;
        public static float  Mood_Pedro = float.NaN;
        public static float  IntensityiN_Pedro = float.NaN;
        public Strategies    _Personality;
       

        public static void Character()
        {
            //////////////////      CREATE CHARACTER      /////////////////////   
            
            ////   Character Pedro   /////
            var am_Pedro = new AM();                    //AutobiographicMemory
            var kb_Pedro = new KB((Name)"Pedro");      //KnowledgeBase
            ////   Character Sarah  ////
            var am_Sarah = new AM();
            var kb_Sarah = new KB((Name)"Sarah");
            /////  EmotionalDecisionMaking  /////
            var storage = new AssetStorage();
            var edm = EmotionalDecisionMakingAsset.CreateInstance(storage);

            

            ///////   knowledge Base and Emotion Estate   /////////
            kb_Pedro.Tell(Name.BuildName("Hate(Sarah)"      ), Name.BuildName("True"   ), Name.BuildName("SELF"), 1);
            kb_Pedro.Tell(Name.BuildName("Location(Office)" ), Name.BuildName("False"  ), Name.BuildName("SELF"), 1);
            kb_Pedro.Tell(Name.BuildName("Current(Location)"), Name.BuildName("Home"   ), Name.BuildName("SELF"), 1);
            var emotionalState_Pedro = new ConcreteEmotionalState();
            edm.RegisterKnowledgeBase(kb_Pedro);


            kb_Sarah.Tell(Name.BuildName("Loves(Pedro)"     ), Name.BuildName("True"  ), Name.BuildName("SELF"), 1);
            kb_Sarah.Tell(Name.BuildName("Current(Location)"), Name.BuildName("Office"), Name.BuildName("SELF"), 1);
            kb_Sarah.Tell(Name.BuildName("Location(Office)" ), Name.BuildName("False" ), Name.BuildName("SELF"), 1);
            var emotionalState_Sarah = new ConcreteEmotionalState();

            ////////////////////////    EVENTS     ///////////////////////

            var Hello_Event1 = Name.BuildName("Event(Action-End, Pedro, Hello, Sarah)");
            var Bye_Event2   = Name.BuildName("Event(Action-End, Pedro, Bye  , Sarah)");


           

            //create an action rule
            var actionRule = new ActionRuleDTO { Action   = Name.BuildName("Kick"),
                                                 Priority = Name.BuildName("4"), 
                                                 Target   = (Name)"Player"
            };


            //add the reaction rule
            var id = edm.AddActionRule(actionRule);
            var rule = edm.GetActionRule(id);
            edm.AddRuleCondition(id, "Hate(Sarah) = True");
            var actions = edm.Decide(Name.UNIVERSAL_SYMBOL);
            edm.Save();

            Console.WriteLine("Decisions: ");
            foreach (var a in actions)
            {
                Console.WriteLine(a.Name.ToString() + " p: " + a.Utility);
            }



            ////////////////////    EMOTIONAL APPRAISAL     /////////////////
            EmotionalAppraisalAsset ea_Pedro = EmotionalAppraisalAsset.CreateInstance(new AssetStorage());
            EmotionalAppraisalAsset ea_Sarah = EmotionalAppraisalAsset.CreateInstance(new AssetStorage());

            ////////////////    List of appraisal variables    ////////////////
            var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
            {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                {
                    Name = "Desirability", Value = (Name.BuildName(4)) //Value = (Name.BuildName("[d]") //---> "INSTEAD 4"
                }
            };

            //////      List of rules for events     ////////------>Event 1
            var rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
            {
                //EventMatchingTemplate = (Name)"Event(Action-End, *, Hello([env],[econ]), *)" //-----> "NECESSARY FOR [D]"
                EventMatchingTemplate = (Name)"Event(Action-End, *, Hello, *)",
                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
                /*---->///"NECESSARY FOR [D]"
                Conditions = new Conditions.DTOs.ConditionSetDTO() 
                {
                    ConditionSet = new string[] { "Math(Math(Math([env], Minus, [econ]), Div, Math([env], Plus, [econ])), Times, 10) = [d]" } 
                }
                */
            };
            ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);

            Console.WriteLine("\n <-----------------------------------------------------> ");
            Console.WriteLine("Test Events ----->" + rule_Pedro.AppraisalVariables +
                              " \n           ----->" + rule_Pedro.EventMatchingTemplate.NumberOfTerms);
            Console.WriteLine(" <-----------------------------------------------------> \n ");


            ////////    Event 2     ///////////
            appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
            {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(-2)) }
            };
            rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
            {
                EventMatchingTemplate = (Name)"Event(Action-End, *, Bye, *)",
                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
            };
            ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);

            /////////     Appraisal Variable  ////////////------> Sarah, Event 1
            var appraisalVariableDTO_Sarah = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
            {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                {
                    Name = "Desirability", Value = (Name.BuildName(-3)) //Value = (Name.BuildName("[d]")
                }
            };
            var rule_Sarah = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
            {
                //EventMatchingTemplate = (Name)"Event(Action-End, *, Hello([env],[econ]), *)"
                EventMatchingTemplate = (Name)"Event(Action-End, *, Hello, *)",
                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO_Sarah),
                /*
                Conditions = new Conditions.DTOs.ConditionSetDTO() 
                {
                    ConditionSet = new string[] { "Math(Math(Math([env], Minus, [econ]), Div, Math([env], Plus, [econ])), Times, 10) = [d]" } 
                }
                */
            };
            ea_Sarah.AddOrUpdateAppraisalRule(rule_Sarah);

            /////////////////////////////////////////////////
            /////New appraisal variable  Event02 (Sarah)/////
            appraisalVariableDTO_Sarah = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
            {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(5)) }
            };
            rule_Sarah = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
            {
                EventMatchingTemplate = (Name)"Event(Action-End, *, Bye, *)",
                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO_Sarah)
            };
            ea_Sarah.AddOrUpdateAppraisalRule(rule_Sarah);


            /////////   SHOW ON THE CONSOLE  /////////////-------> Pedro's perspective

            ///////////   Event 1  //////////////
            ea_Pedro.AppraiseEvents(new[] { Hello_Event1 }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);

            Console.WriteLine(" \n Pedro's perspective ");
            Console.WriteLine(" \n Events occured so far: "
                + string.Concat(am_Pedro.RecallAllEvents().Select(e => "\n Id: "
                + e.Id + " Event: " + e.EventName.ToString())));

            am_Pedro.Tick++;
            emotionalState_Pedro.Decay(am_Pedro.Tick);
            Console.WriteLine(" \n  Mood on tick '" + am_Pedro.Tick + "': " + emotionalState_Pedro.Mood);
            Console.WriteLine("  Active Emotions \n  "
                    + string.Concat(emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity)));

            ////////////     TEST SEND EMOTION     ////////////////////////////////
            Emotion_Pedro = emotionalState_Pedro.GetStrongestEmotion().EmotionType;
            Intensity_Pedro = emotionalState_Pedro.GetStrongestEmotion().Intensity;
            Mood_Pedro = emotionalState_Pedro.Mood;
            ///////////////////////////////////////////////////////////////////////

            ////////////     TEST GET EMOTION     ////////////////////////////////
            //IntensityiN_Pedro = emotionalState_Pedro

            ///////////////////////////////////////////////////////////////////////

            Console.WriteLine(" \n  Num Events occured so far: " + am_Pedro.RecallAllEvents().Count());




            //////////    Occurr New Event    //////////////------> Event 2
            ea_Pedro.AppraiseEvents(new[] { Hello_Event1, Bye_Event2 }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);
            Console.WriteLine(" \n Appraising new event! '" + string.Concat(am_Pedro.RecallAllEvents().Select(e => "\nId: "
                               + e.Id + " Event: " + e.EventName.ToString())));
            am_Pedro.Tick++;
            emotionalState_Pedro.Decay(am_Pedro.Tick);
            Console.WriteLine(" \nMood on tick '" + am_Pedro.Tick + "': " + emotionalState_Pedro.Mood);
            Console.WriteLine(" Active Emotions: "
                    + string.Concat(emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));

            Console.WriteLine(" \nNum Events occured so far: " + am_Pedro.RecallAllEvents().Count());

            ///////////////    The same Event again   ////////////------>  Event 2
            ea_Pedro.AppraiseEvents(new[] { Bye_Event2 }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);
            Console.WriteLine(" \n Repeated Event 2'" + string.Concat(am_Pedro.RecallAllEvents().Select(e => "\nId: "
                               + e.Id + " Event: " + e.EventName.ToString())));
            am_Pedro.Tick++;
            emotionalState_Pedro.Decay(am_Pedro.Tick);
            Console.WriteLine(" \nMood on tick '" + am_Pedro.Tick + "': " + emotionalState_Pedro.Mood);
            Console.WriteLine(" Active Emotions: "
                    + string.Concat(emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));

            Console.WriteLine(" \nNum Events occured so far (Pedro): " + am_Pedro.RecallAllEvents().Count());

            ///////////////////   Event 1   /////////////////--------> Sarah's perspective
            ea_Sarah.AppraiseEvents(new[] { Hello_Event1 }, emotionalState_Sarah, am_Sarah, kb_Sarah, null);

            Console.WriteLine(" \n Sarah's perspective");
            Console.WriteLine(" \nEvents occured so far: "
                + string.Concat(am_Sarah.RecallAllEvents().Select(e => "\nId: "
                + e.Id + " Event: " + e.EventName.ToString())));

            Console.WriteLine(" \nNum Events occured so far (Pedro's perspective): " + am_Pedro.RecallAllEvents().Count());
            Console.WriteLine(" \nNum Events occured so far (Sarah's perspective): " + am_Sarah.RecallAllEvents().Count());

            am_Sarah.Tick++;
            emotionalState_Sarah.Decay(am_Sarah.Tick);
            Console.WriteLine(" \nMood on tick '" + am_Sarah.Tick + "': " + emotionalState_Sarah.Mood);
            Console.WriteLine(" Active Emotions: "
                    + string.Concat(emotionalState_Sarah.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));

            ///////////    Occurr New Event    //////////////------> Event 2
            ea_Sarah.AppraiseEvents(new[] { Hello_Event1, Bye_Event2 }, emotionalState_Sarah, am_Sarah, kb_Sarah, null);
            Console.WriteLine(" \n Appraising new event! '\n" + string.Concat(am_Sarah.RecallAllEvents().Select(e => "\nId: "
                               + e.Id + " Event: " + e.EventName.ToString())));
            am_Sarah.Tick++;
            emotionalState_Sarah.Decay(am_Sarah.Tick);
            Console.WriteLine(" \nMood on tick '" + am_Sarah.Tick + "': " + emotionalState_Sarah.Mood);
            Console.WriteLine(" Active Emotions: "
                    + string.Concat(emotionalState_Sarah.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));

            ///////////////    The same Event again   ////////////------>  Event 2
            ea_Sarah.AppraiseEvents(new[] { Bye_Event2 }, emotionalState_Sarah, am_Sarah, kb_Sarah, null);
            Console.WriteLine(" \n Repeated Event 2'\n" + string.Concat(am_Sarah.RecallAllEvents().Select(e => "\nId: "
                               + e.Id + " Event: " + e.EventName.ToString())));
            am_Sarah.Tick++;
            emotionalState_Sarah.Decay(am_Sarah.Tick);
            Console.WriteLine(" \nMood on tick '" + am_Sarah.Tick + "': " + emotionalState_Sarah.Mood);
            Console.WriteLine(" Active Emotions: "
                    + string.Concat(emotionalState_Sarah.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));

            Console.WriteLine(" \nNum Events occured so far (Sarah's perspective): " + am_Sarah.RecallAllEvents().Count());

            ea_Sarah.Save();
            ea_Pedro.Save();

            Console.ReadKey();

        }


        static void Main(string[] args)
        {
            Character();
            Console.WriteLine("The emotion is: " + Emotion_Pedro);
        }
    }
}

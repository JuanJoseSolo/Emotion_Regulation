﻿using System;
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
        public static string Even = string.Empty;

        public static Name Event = Name.NIL_SYMBOL;
        public Strategies _Personality;

        public AvoidEvents avoidEventes;

        public static void Character()
        {
            //////////////////      CREATE CHARACTER      /////////////////////   

            ////   Character Pedro   /////
            var am_Pedro = new AM(); //AutobiographicMemory
            var kb_Pedro = new KB((Name)"Pedro");      //KnowledgeBase
            ////   Character Sarah  ////
            var am_Sarah = new AM();
            var kb_Sarah = new KB((Name)"Sarah");
            ////   Character Usuario  ////
            var am_Usuario = new AM();
            var kb_Usuario = new KB((Name)"Usuario");
            /////  EmotionRegulationEstrategies  /////
            Strategies _Personalities = new Strategies();


            ///////   knowledge Base and Emotion Estate   /////////
            kb_Pedro.Tell(Name.BuildName("like(Sarah)"), Name.BuildName("True"), Name.BuildName("SELF"), 1);
            kb_Pedro.Tell(Name.BuildName("Dislike(Usuario)"), Name.BuildName("True"), Name.BuildName("SELF"), 1);
            kb_Pedro.Tell(Name.BuildName("Location(Office)"), Name.BuildName("False"), Name.BuildName("SELF"), 1);
            kb_Pedro.Tell(Name.BuildName("Current(Location)"), Name.BuildName("Home"), Name.BuildName("SELF"), 1);
            var emotionalState_Pedro = new ConcreteEmotionalState();

            kb_Sarah.Tell(Name.BuildName("Like(Pedro)"), Name.BuildName("True"), Name.BuildName("SELF"), 1);
            kb_Sarah.Tell(Name.BuildName("Current(Location)"), Name.BuildName("Office"), Name.BuildName("SELF"), 1);
            kb_Sarah.Tell(Name.BuildName("Location(Office)"), Name.BuildName("False"), Name.BuildName("SELF"), 1);
            var emotionalState_Sarah = new ConcreteEmotionalState();

            kb_Usuario.Tell(Name.BuildName("Dislike(Sarah)"), Name.BuildName("True"), Name.BuildName("SELF"), 1);
            kb_Usuario.Tell(Name.BuildName("Current(Location)"), Name.BuildName("Office"), Name.BuildName("SELF"), 1);
            kb_Usuario.Tell(Name.BuildName("Location(Office)"), Name.BuildName("False"), Name.BuildName("SELF"), 1);
            var emotionalState_Usuario = new ConcreteEmotionalState();



            ///////  FUZZY PERSONALITY   ////// 
            ///
            float Cons = 70, Extrav = 10;

            AvoidEvents.StrategyTest(Cons, Extrav, kb_Pedro);
            Console.WriteLine("\n Variable Avoid------>> " + AvoidEvents.StrategyApplied + "\n Strategy---->> "
                                                           + AvoidEvents.StrategyName);

            ////////////////////////    EVENTS     ///////////////////////

            var EnterOffice = Name.BuildName("Event(Action-End, Pedro, Enter, Office)");
            var Hello_Event1 = Name.BuildName("Event(Action-End, Pedro, Hello, Sarah)");
            var Bye_Event2 = Name.BuildName("Event(Action-End, Pedro, Bye, Sarah, True)");

            ////////////////////    EMOTIONAL APPRAISAL     /////////////////
            EmotionalAppraisalAsset ea_Pedro = EmotionalAppraisalAsset.CreateInstance(new AssetStorage());
            EmotionalAppraisalAsset ea_Sarah = EmotionalAppraisalAsset.CreateInstance(new AssetStorage());



            ////////// EVENT 1 ///////
            var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
            {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                {
                    Name = "Desirability", Value = (Name.BuildName(4)) //Value = (Name.BuildName("[d]") //---> "INSTEAD 4"
                }
            };
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


            /////// EVENT 2 //////
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


            /////// EVENT 3 //////
            appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
            {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(-3)) }
            };
            rule_Pedro = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
            {
                EventMatchingTemplate = (Name)"Event(Action-End, *, Enter, *)",
                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
            };
            ea_Pedro.AddOrUpdateAppraisalRule(rule_Pedro);


            /////////   EVENT 1  (Sarah) //////////
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


            /////Event 2 (Sarah)/////
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

            //it sends events and appraisal variables
            var ea_Pedro_Received = AvoidEvents.ChangeEvent(Bye_Event2, ea_Pedro).Item2;
            var EventReceived = AvoidEvents.ChangeEvent(Bye_Event2, ea_Pedro).Item1;

            foreach (var CharacterNew in ea_Pedro_Received.GetAllAppraisalRules())
            {
                Console.WriteLine("ea_CharacterReceived------> " + CharacterNew.EventMatchingTemplate);
            }

            /*
           Console.WriteLine("Event Received----->>>" + EventReceived);
           Console.WriteLine("eaPedro " + 
               ea_Pedro.GetAllAppraisalRules().ElementAt(2).AppraisalVariables + 
               " eaReceived " + Received_ea_New.GetAllAppraisalRules().ElementAt(2).AppraisalVariables);
           */




            /////////   SHOW ON THE CONSOLE  /////////////-------> Pedro's perspective

            ///////////   Event 1  //////////////
            //ea_Pedro.AppraiseEvents(new[] { EventReceived }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);
            ea_Pedro_Received.AppraiseEvents(new[] { EventReceived }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);

            foreach (var AM_Pedro in am_Pedro.RecallAllEvents()) 
            { 
                Console.WriteLine("Inside of am_Pedro---->> " + AM_Pedro.EventName);
            }
            Console.WriteLine(" \n Pedro's perspective ");
            Console.WriteLine(" \n Events occured so far: "
                + string.Concat(am_Pedro.RecallAllEvents().Select(e => "\n Id: "
                + e.Id + " Event: " + e.EventName.ToString())));
            
            am_Pedro.Tick++;
            emotionalState_Pedro.Decay(am_Pedro.Tick);
            Console.WriteLine(" \n  Mood on tick '" + am_Pedro.Tick + "': " + emotionalState_Pedro.Mood);
            Console.WriteLine("  Active Emotions \n  "
                    + string.Concat(emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity)));

            Console.ReadKey();

            /*
            ////////////     TEST SEND EMOTION     ////////////////////////////////
            Emotion_Pedro = emotionalState_Pedro.GetStrongestEmotion().EmotionType;
            Intensity_Pedro = emotionalState_Pedro.GetStrongestEmotion().Intensity;
            Mood_Pedro = emotionalState_Pedro.Mood;
            ///////////////////////////////////////////////////////////////////////

            Console.WriteLine(" \n  Num Events occured so far: " + am_Pedro.RecallAllEvents().Count());

            //////////    Occurr New Event    //////////////------> Event 2
            ea_Pedro.AppraiseEvents(new[] { Hello_Event1, EventReceived2 }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);
            Console.WriteLine(" \n Appraising new event! '" + string.Concat(am_Pedro.RecallAllEvents().Select(e => "\nId: "
                               + e.Id + " Event: " + e.EventName.ToString())));
            am_Pedro.Tick++;
            emotionalState_Pedro.Decay(am_Pedro.Tick);
            Console.WriteLine(" \nMood on tick '" + am_Pedro.Tick + "': " + emotionalState_Pedro.Mood);
            Console.WriteLine(" Active Emotions: "
                    + string.Concat(emotionalState_Pedro.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));

            Console.WriteLine(" \nNum Events occured so far: " + am_Pedro.RecallAllEvents().Count());

            ///////////////    The same Event again   ////////////------>  Event 2
            ea_Pedro.AppraiseEvents(new[] { EventReceived2 }, emotionalState_Pedro, am_Pedro, kb_Pedro, null);
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
            ea_Sarah.AppraiseEvents(new[] { Hello_Event1, EventReceived2 }, emotionalState_Sarah, am_Sarah, kb_Sarah, null);
            Console.WriteLine(" \n Appraising new event! '\n" + string.Concat(am_Sarah.RecallAllEvents().Select(e => "\nId: "
                               + e.Id + " Event: " + e.EventName.ToString())));
            am_Sarah.Tick++;
            emotionalState_Sarah.Decay(am_Sarah.Tick);
            Console.WriteLine(" \nMood on tick '" + am_Sarah.Tick + "': " + emotionalState_Sarah.Mood);
            Console.WriteLine(" Active Emotions: "
                    + string.Concat(emotionalState_Sarah.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));

            ///////////////    The same Event again   ////////////------>  Event 2
            ea_Sarah.AppraiseEvents(new[] { EventReceived2 }, emotionalState_Sarah, am_Sarah, kb_Sarah, null);
            Console.WriteLine(" \n Repeated Event 2'\n" + string.Concat(am_Sarah.RecallAllEvents().Select(e => "\nId: "
                               + e.Id + " Event: " + e.EventName.ToString())));
            am_Sarah.Tick++;
            emotionalState_Sarah.Decay(am_Sarah.Tick);
            Console.WriteLine(" \nMood on tick '" + am_Sarah.Tick + "': " + emotionalState_Sarah.Mood);
            Console.WriteLine(" Active Emotions: "
                    + string.Concat(emotionalState_Sarah.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity + " ")));

            Console.WriteLine(" \nNum Events occured so far (Sarah's perspective): " + am_Sarah.RecallAllEvents().Count());
            */
            ea_Sarah.Save();
            ea_Pedro_Received.Save();

            Console.ReadKey();

        }

        static void Main(string[] args)
        {
            Character();
            
        }
    }
}

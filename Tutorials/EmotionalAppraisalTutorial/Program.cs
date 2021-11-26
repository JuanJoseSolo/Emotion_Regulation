﻿using System;
using System.Linq;
using EmotionalAppraisal;
using AutobiographicMemory;
using WellFormedNames;
using KnowledgeBase;
using GAIPS.Rage;
using System.Collections.Generic;

namespace EmotionalAppraisalTutorial
{
    class Program
    {
        //This is a small console program to exemplify the main functionality of the Emotional Appraisal Asset
        static void Main(string[] args)
        {
			var helloEvent1 = Name.BuildName("Event(Action-End, John, Hello(0,5), Sarah)");
            var helloEvent2= Name.BuildName("Event(Action-End, John, Hello(5,0), Sarah)");
            var helloEvent3 = Name.BuildName("Event(Action-End, John, Hello(2,3), Sarah)");

            var helloBack = Name.BuildName("Event(Action-End, John, Seeya, Sarah)");

            var helloTwice = Name.BuildName("Event(Action-End, John, Goodbye, Sarah)");

            EmotionalAppraisalAsset ea = EmotionalAppraisalAsset.CreateInstance(new AssetStorage());



            //The following lines add an appraisal rule that will make the kickEvent be perceived as undesirable
            //Normally, these rules should be authored using the AuthoringTool provided with the asset but they can also be added dynamically
            var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>() 
            { 
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO(){ Name = "Desirability", Value = (Name.BuildName("[d]")) } 
            };
            var rule = new EmotionalAppraisal.DTOs.AppraisalRuleDTO() 
            { 
                EventMatchingTemplate =(Name)"Event(Action-End, *, Hello([env],[econ]), *)", 
                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO),
                Conditions = new Conditions.DTOs.ConditionSetDTO() 
            
                { 
                    ConditionSet = new string[] { "Math(Math(Math([env], Minus, [econ]), Div, Math([env], Plus, [econ])), Times, 10) = [d]" } 
                } 
            };

            ea.AddOrUpdateAppraisalRule(rule);


            appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>() { new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(2)) } };
            rule = new EmotionalAppraisal.DTOs.AppraisalRuleDTO() { EventMatchingTemplate = (Name)"Event(Action-End, *, Seeya, *)", AppraisalVariables = new AppraisalVariables(appraisalVariableDTO) };
            ea.AddOrUpdateAppraisalRule(rule);

            appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>() { new EmotionalAppraisal.DTOs.AppraisalVariableDTO() { Name = "Desirability", Value = (Name.BuildName(10)) } };
            rule = new EmotionalAppraisal.DTOs.AppraisalRuleDTO() { EventMatchingTemplate = (Name)"Event(Action-End, *, Goodbye, *)", AppraisalVariables = new AppraisalVariables(appraisalVariableDTO) };
            ea.AddOrUpdateAppraisalRule(rule);


            var am = new AM();
            var kb = new KB((Name)"John");

            kb.Tell(Name.BuildName("Likes(Mary)"), Name.BuildName("John"), Name.BuildName("SELF"));

            var emotionalState = new ConcreteEmotionalState();

            //Emotions are generated by the appraisal of the events that occur in the game world 
            ea.AppraiseEvents(new[] { helloEvent1 }, emotionalState, am, kb, null);
            
            Console.WriteLine("\nMood on tick '" + am.Tick + "': " + emotionalState.Mood);
            Console.WriteLine("Active Emotions: " + string.Concat(emotionalState.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity)));

            //Each event that is appraised will be stored in the autobiographical memory that was passed as a parameter
            Console.WriteLine("\nEvents occured so far: " + string.Concat(am.RecallAllEvents().Select(e => "\nId: " + e.Id + " Event: " + e.EventName.ToString())));
            
            //The update function will increase the current tick by 1. Each active emotion will decay to 0 and the mood will slowly return to 0
            for (int i = 0; i < 3; i++)
            {
                am.Tick++;
                emotionalState.Decay(am.Tick);
                Console.WriteLine("\nMood on tick '" + am.Tick + "': " + emotionalState.Mood);
                Console.WriteLine("Active Emotions: " + string.Concat(emotionalState.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity)));
            }


            //Emotions are generated by the appraisal of the events that occur in the game world 

            Console.WriteLine("\n Appraising new event! '");
            ea.AppraiseEvents(new[] { helloEvent1, helloEvent3 }, emotionalState, am, kb, null);

            Console.WriteLine("\nMood on tick '" + am.Tick + "': " + emotionalState.Mood);
            Console.WriteLine("Active Emotions: " + string.Concat(emotionalState.GetAllEmotions().Select(e => e.EmotionType + ": " + e.Intensity)));

            //Each event that is appraised will be stored in the autobiographical memory that was passed as a parameter
            Console.WriteLine("\nEvents occured so far: " + string.Concat(am.RecallAllEvents().Select(e => "\nId: " + e.Id + " Event: " + e.EventName.ToString())));

            //The update function will increase the current tick by 1. Each active emotion will decay to 0 and the mood will slowly return to 0
            for (int i = 0; i < 3; i++)
            {
                am.Tick++;
                emotionalState.Decay(am.Tick);
                Console.WriteLine("\nMood on tick '" + am.Tick + "': " + emotionalState.Mood);
                var ems = emotionalState.GetAllEmotions().ToArray();
                Console.WriteLine("Active Emotions: " + string.Concat(emotionalState.GetAllEmotions().Select(e => e.EmotionType + "-" + e.Intensity + " ")));
            }
            

            //Emotions are generated by the appraisal of the events that occur in the game world 

            Console.WriteLine("\n Appraising new event! '");
            ea.AppraiseEvents(new[] { helloTwice }, emotionalState, am, kb, null);

            Console.WriteLine("\nMood on tick '" + am.Tick + "': " + emotionalState.Mood);
            Console.WriteLine("Active Emotions: " + string.Concat(emotionalState.GetAllEmotions().Select(e => e.EmotionType + "-" + e.Intensity + " ")));

            //Each event that is appraised will be stored in the autobiographical memory that was passed as a parameter
            Console.WriteLine("\nEvents occured so far: " + string.Concat(am.RecallAllEvents().Select(e => "\nId: " + e.Id + " Event: " + e.EventName.ToString())));

            //The update function will increase the current tick by 1. Each active emotion will decay to 0 and the mood will slowly return to 0
            for (int i = 0; i < 3; i++)
            {
                am.Tick++;
                emotionalState.Decay(am.Tick);
                Console.WriteLine("\nMood on tick '" + am.Tick + "': " + emotionalState.Mood);
                var ems = emotionalState.GetAllEmotions().ToArray();
                Console.WriteLine("Active Emotions: " + string.Concat(emotionalState.GetAllEmotions().Select(e => e.EmotionType + "-" + e.Intensity + " ")));
            }
            ea.Save();

            Console.ReadKey();
        }
    }
}


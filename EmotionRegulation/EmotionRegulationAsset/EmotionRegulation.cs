using System;
using System.Collections.Generic;
using System.Linq;
using WellFormedNames;
using EmotionalAppraisal;
using EmotionalDecisionMaking;
using AutobiographicMemory;
using KnowledgeBase;
using EmotionalAppraisal.DTOs;
using GAIPS.Rage;
using EmotionalAppraisal.OCCModel;
using Utilities;
using EmotionalAppraisal.AppraisalRules;



namespace EmotionRegulationAsset
{

    class EmotionRegulation
    {
        public Name EventFatima { get; set; }
        public EmotionalAppraisalAsset NewEA_character { get; private set; }
        public EmotionalDecisionMakingAsset NewEdm_character { get; private set; }
        public AM NewAm_character { get; private set; }
        public List<Name> LalternativeEvents { get; private set; }
        public bool AppliedStrategy { get; private set; }
        public ConcreteEmotionalState NewEmotionalState_Character { get; set; }
        private PersonalityTraits Personality { get; set; }
        public double ObjetiveEmotion { get; private set; }
        public Dictionary<string, string> Dic_RelatedActions { get; private set; }
        private float ERmood;

        public EmotionRegulation() { }
        public EmotionRegulation(
            EmotionalAppraisalAsset ea_character,
            EmotionalDecisionMakingAsset edm_character,
            AM am_character,
            ConcreteEmotionalState emotionalEstate_character,
            //KB kb_character,
            PersonalityTraits personalityTraits,
            Dictionary<string, string> relatedActions,
            List<Name> AlternativeEvents,
            double objetiveEmotion)
        {
            this.NewEA_character = ea_character;
            this.NewEdm_character = edm_character;
            this.NewAm_character = am_character;
            this.NewEmotionalState_Character = emotionalEstate_character;
            //this.NewKB_character = kb_character;
            this.Personality = personalityTraits;
            this.ObjetiveEmotion = objetiveEmotion;
            this.LalternativeEvents = AlternativeEvents;
            this.Dic_RelatedActions = relatedActions;

            EventFatima = Name.NIL_SYMBOL;
            AppliedStrategy = false;
            ERmood = emotionalEstate_character.Mood;
        }

        public struct DataName
        {
            public Name EventTypeFatima { get; set; }
            public bool IsAvoided { get; set; }

        }
        public struct EventAppraisalValues
        {
            public string AppraisalType { get; set; }
            public float AppraisalValue { get; set; }
            public int index { get; set; }
        }

        //Situation Selection 
        public bool SituationSelection(Name events)
        {
            Console.WriteLine("\n---------------------Situation Selection-------------------------");
            //Personality
            AppliedStrategy = false;
            var DSituationSelection = this.Personality.DStrategyAndPower.Where((strategy, power) => strategy.Key == "Situation Selection");
            var ExistStrategy = DSituationSelection.Any();
            var StronglyStrategyPower = DSituationSelection.Select(p => p.Value.Trim() == "Strongly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy && StronglyStrategyPower;
            //Event
            var NewEvent = ReName(events);
            EventFatima = NewEvent.EventTypeFatima;

            Console.WriteLine("Event name: " + events.GetNTerm(3) +
                "                         Target: " + EventFatima.GetNTerm(4));
            Console.WriteLine("\nCan be avoided: " + NewEvent.IsAvoided);

            //Conditions
            if (ApplyStrategy && NewEvent.IsAvoided)
            {
                Console.WriteLine(" \n In progress...  ");
                Console.WriteLine(" Evaluating new event...  ");
                var eventName = NewEvent.EventTypeFatima.GetNTerm(3);
                var Event = EventMatchingName(this.NewEA_character, eventName);
                var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(Event.index).EventMatchingTemplate;
                var ListEvent = NewEvent.EventTypeFatima.GetLiterals().ToList();

                //Build new eventTemplate with a word NOT
                var EmotionMatchTemplate_list = EventTemplate.GetTerms().ToList();
                ListEvent.RemoveAt(3);
                EmotionMatchTemplate_list.RemoveAt(3);
                var NewEventName = Name.BuildName("Not-" + eventName);
                EmotionMatchTemplate_list.Insert(3, NewEventName);
                ListEvent.Insert(3, NewEventName);
                var NewEventMatchingTemplate = Name.BuildName(EmotionMatchTemplate_list);
                EventFatima = Name.BuildName(ListEvent);

                UpdateEmotionalAppraisal(this.NewEA_character, Event.AppraisalType, 0, NewEventMatchingTemplate, Event.index);
                AppliedStrategy = true;
                Console.WriteLine(" \nNew event: " + EventFatima.GetNTerm(3));
            }
            else { Console.WriteLine("\n Strategy not applied due to :\n Agent's personality can apply the strategy ===> "
                                                         + ApplyStrategy + " or : Event Is Avoided is ===> " + NewEvent.IsAvoided); }
            Console.WriteLine("\nSituation Selection could be applied: " + AppliedStrategy);
            return AppliedStrategy;
        }
        //Situation Modification 
        public bool SituationModification(Name events)
        {
            Console.WriteLine("\n--------------------Situation Modification-----------------------");
            //get the personality 
            AppliedStrategy = false;
            var DSituationSelection = this.Personality.DStrategyAndPower.Where((strategy, power) => strategy.Key == "Situation Modification");
            var ExistStrategy = DSituationSelection.Any();
            var StronglyStrategyPower = DSituationSelection.Select(p => p.Value.Trim() == "Strongly").FirstOrDefault();
            var LightlyStrategyPower = DSituationSelection.Select(p => p.Value.Trim() == "Lightly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy && (StronglyStrategyPower || LightlyStrategyPower);

            //Event
            var newEvent = ReName(events);
            EventFatima = newEvent.EventTypeFatima;
            var target = EventFatima.GetNTerm(4).ToString();
            var eventName = EventFatima.GetNTerm(3).ToString();
            var existRelatedActions = Dic_RelatedActions.Where(e => e.Value == eventName).Any();

            //finds the appraisal value of event
            var Event = EventMatchingName(this.NewEA_character, (Name)eventName);

            Console.WriteLine("\nEvent name: " + eventName +
                "                          Target: " + target);
            Console.WriteLine("\nCould do any actions ? : " + existRelatedActions);

            //Conditions
            if (ApplyStrategy && existRelatedActions && Event.AppraisalValue <= -5)//*-5
            {
                var DRelatedActions = Dic_RelatedActions.Where(e => e.Value == eventName);
                var LRelatedActions = DRelatedActions.Select(a => a.Key).ToList();
                List<ActionLibrary.DTOs.ActionRuleDTO> LActionsEDM = new();

                foreach (var act in LRelatedActions)
                {
                    LActionsEDM.Add(this.NewEdm_character.GetAllActionRules().Where(
                    a => a.Action.ToString() == act).FirstOrDefault());
                }

                var RelatedActions = this.NewEdm_character.GetAllActionRules().Where(
                    a => a.Action.ToString() == DRelatedActions.Select(ea => ea.Key).FirstOrDefault());

                Console.WriteLine(" \n In progress...  ");
                Console.WriteLine(" Evaluating actions value...  ");

                foreach (var AedmCharacter in LActionsEDM)
                {
                    var avg = ((this.Personality.Neuroticism + this.Personality.Agreeableness) / 2);
                    var tanh = (Math.Abs((Event.AppraisalValue / 2)) * (Math.Tanh(-(2 * avg - 100) / 40))) - (Math.Abs(Event.AppraisalValue / 2));
                    var ModifiedValue = (float)tanh;

                    var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(Event.index).EventMatchingTemplate;

                    if (ModifiedValue >= -this.ObjetiveEmotion)
                    {
                        var actionTo = AedmCharacter.Action.ToString();
                        Console.WriteLine(" Would decide: " + actionTo);
                        UpdateEmotionalAppraisal(this.NewEA_character, Event.AppraisalType, ModifiedValue, EventTemplate, Event.index);
                        AppliedStrategy = true;
                    }
                    else
                    {
                        Console.WriteLine("\n Strategy not applied due to : Emotion limit was achieved ===> " + (ModifiedValue >= -this.ObjetiveEmotion));
                        Console.WriteLine("\n New possible value = " + Math.Abs(ModifiedValue) + " -User defined limit = " + this.ObjetiveEmotion);
                    }
                }
            }
            else { Console.WriteLine("\n Strategy not applied due to :\n Agent's personality can apply the strategy ===> " + ApplyStrategy +
                ", or : Exist Related Actions is ===> " + existRelatedActions + ", or :\n " +
                "Defined Event Appraisal Value is greater than -5 : " + (Event.AppraisalValue < -5) + ", is " + Event.AppraisalValue); }
            Console.WriteLine("\nSituation Modification could be applied: " + AppliedStrategy);
            return AppliedStrategy;
        }
        //Attention Deployment 
        public bool AttentionDeployment(Name events)
        {
            Console.WriteLine("\n---------------------Attention Deployment------------------------");

            AppliedStrategy = false;
            var DAttentionDeployment = this.Personality.DStrategyAndPower.Where((strategy, power) => strategy.Key == "Attention Deployment");
            var ExistStrategy = DAttentionDeployment.Any();
            var StronglyStrategyPower = DAttentionDeployment.Select(p => p.Value.Trim() == "Strongly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy && StronglyStrategyPower;

            var target = events.GetNTerm(4);
            var eventName = events.GetNTerm(3);

            var NewEvent = ReName(events);
            EventFatima = NewEvent.EventTypeFatima;

            Console.WriteLine("\nEvent name: " + eventName +
                "                           Target: " + this.EventFatima.GetNTerm(4));

            //Only when the current event is negative
            var CurrentEventValue = EventMatchingName(this.NewEA_character, events);
            IEnumerable<Name> RelatedEvent = Enumerable.Empty<Name>();
            if (CurrentEventValue.AppraisalValue < 0)
            {
                var EventNameAM = this.NewAm_character.RecallAllEvents().Select(ep => ep.EventName);
                RelatedEvent = EventNameAM.Where(e => e.GetNTerm(4) == target).Select(a => a.GetNTerm(3));

                var ExistEvent = RelatedEvent.Any();
                Console.WriteLine("Does Exist any event related ? : " + ExistEvent);
                foreach (var e in RelatedEvent)
                    Console.WriteLine("Related events : " + e);
            }
            var ExistEventRelated = RelatedEvent.Any();


            //Conditions
            if (ApplyStrategy && ExistEventRelated)
            {
                List<float> ListEventsValue = new();
                Console.WriteLine(" \n In progress...  ");
                Console.WriteLine(" Evaluating past events...  ");

                foreach (var n in RelatedEvent)
                {
                    var EventValues = EventMatchingName(this.NewEA_character, n);
                    ListEventsValue.Add(EventValues.AppraisalValue);
                }

                var average = ListEventsValue.Average();

                var EventToER = EventMatchingName(this.NewEA_character, eventName);
                var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(
                    EventToER.index).EventMatchingTemplate;

                var tanh = (Math.Abs((EventToER.AppraisalValue / 2)) * (Math.Tanh((2 * average - 10) / 5))) - (Math.Abs(EventToER.AppraisalValue / 2));

                var ModifiedValue = (float)tanh;
                if (ModifiedValue >= -this.ObjetiveEmotion)
                {
                    UpdateEmotionalAppraisal(this.NewEA_character, EventToER.AppraisalType, ModifiedValue, EventTemplate, EventToER.index);
                    AppliedStrategy = true;
                }
                else
                {
                    Console.WriteLine("\n Strategy not applied due to : Emotion limit was achieved ===> " + (ModifiedValue >= -this.ObjetiveEmotion));
                    Console.WriteLine("\n New possible value = " + Math.Abs(ModifiedValue) + " -User defined limit = " + this.ObjetiveEmotion);
                }
            }
            else { Console.WriteLine("\n Strategy not applied due to :\n Agent's personality can apply the strategy ===> " + ApplyStrategy +
                                            ", or Exist Event Related is ===> " + ExistEventRelated);
            }

            Console.WriteLine("\n Attention Deployment could be applied : " + AppliedStrategy);
            return AppliedStrategy;
        }

        // Cognitive Change
        public bool CognitiveChange(Name events)
        {
            Console.WriteLine("\n---------------------Cognitive Change------------------------");
            ERmood = this.NewEmotionalState_Character.Mood;
            List<float> LAlternativeEventsValue = new();

            AppliedStrategy = false;
            //check personality
            var DAttentionDeployment = this.Personality.DStrategyAndPower.Where(
                (strategy, power) => strategy.Key == "Cognitive Change");
            var ExistStrategy = DAttentionDeployment.Any();
            var StronglyStrategyPower = DAttentionDeployment.Select(p => p.Value.Trim() == "Strongly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy && StronglyStrategyPower;

            //get event name and it construct the event
            var eventName = events.GetNTerm(3);
            var NewEvent = ReName(events);
            EventFatima = NewEvent.EventTypeFatima;

            //Bucar eventos relacionados con el evento actual para reinterpretar
            var AlternativeEventsName = this.LalternativeEvents.Where(
                e => e.GetNTerm(4) == EventFatima.GetNTerm(3));

            var existAlternativeEvents = AlternativeEventsName.Any();
            var target = EventFatima.GetNTerm(4).ToString();

            Console.WriteLine("\nEvent name: " + eventName +
               "                          Target: " + target);
            Console.WriteLine("\nCould it've any reinterpretation ? : " + existAlternativeEvents);

            if (ApplyStrategy && existAlternativeEvents)
            {
                Console.WriteLine(" \n In progress...  ");
                Console.WriteLine(" reinterpreting the event...  ");
                //Obtiene el valor de la valoracion de los eventos relacionados
                foreach (var AE in AlternativeEventsName)
                {
                    /*var AlternativeEvents = this.NewEA_character.GetAllAppraisalRules().Select(
                    e => e.EventMatchingTemplate).Where(eM => eM.GetNTerm(3) == AE.GetNTerm(3)).FirstOrDefault().GetNTerm(3);
                    */
                    var EventValueAlternative = EventMatchingName(this.NewEA_character, AE.GetNTerm(3));
                    Console.WriteLine("\n '" + AE.GetNTerm(3) + "'");
                    LAlternativeEventsValue.Add(EventValueAlternative.AppraisalValue);
                }

                var averange = LAlternativeEventsValue.Average();//Promedio de los nuevos eventos

                var Personality = (this.Personality.Openness - this.Personality.Neuroticism);
                var MoodAndAvg = (ERmood + averange);

                var CurrentEventValue = EventMatchingName(this.NewEA_character, eventName);//valoración del evento actual

                var tanh1 = (Math.Abs((CurrentEventValue.AppraisalValue / 2)) * (Math.Tanh(-(2 * this.Personality.Neuroticism - 100) / 40))) - (Math.Abs(CurrentEventValue.AppraisalValue / 2));
                var tanh2 = (Math.Abs((CurrentEventValue.AppraisalValue / 2)) * (Math.Tanh((2 * Personality - 20) / 8))) - (Math.Abs(CurrentEventValue.AppraisalValue / 2));

                var ModifiedValue = (float)((tanh1 + tanh2) / 2);

                var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(
                                    CurrentEventValue.index).EventMatchingTemplate;

                if (ModifiedValue >= -this.ObjetiveEmotion)
                {
                    UpdateEmotionalAppraisal(this.NewEA_character, CurrentEventValue.AppraisalType, ModifiedValue, EventTemplate, CurrentEventValue.index);
                    AppliedStrategy = true;
                }
                else
                {
                    Console.WriteLine("\n Strategy not applied due to : Emotion limit was achieved ===> " + (ModifiedValue >= -this.ObjetiveEmotion));
                    Console.WriteLine("\n New possible value = " + Math.Abs(ModifiedValue) + " -User defined limit = " + this.ObjetiveEmotion);
                }
            }
            else
            {
                Console.WriteLine("\n Strategy not applied due to :\n Agent's personality can apply the strategy ===> " + ApplyStrategy +
                                        ", or Exist Alternative Event is ===> " + existAlternativeEvents);
            }

            Console.WriteLine("\n Cognitive Change could be applied : " + AppliedStrategy);
            return AppliedStrategy;
        }

        //Response Modulation
        public bool ResponseModulation(Name events)
        {
            Console.WriteLine("\n---------------------Response Modulation------------------------");
            AppliedStrategy = false;

            //check personality traits
            var DAttentionDeployment = this.Personality.DStrategyAndPower.Where(
                (strategy, power) => strategy.Key == "Response Modulation");
            var ExistStrategy = DAttentionDeployment.Any();
            var StronglyStrategyPower = DAttentionDeployment.Select(p => p.Value.Trim() == "Strongly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy && StronglyStrategyPower;

            //get event name, construction of the event
            var eventName = events.GetNTerm(3);
            var NewEvent = ReName(events);
            EventFatima = NewEvent.EventTypeFatima;
            var target = EventFatima.GetNTerm(4).ToString();

            Console.WriteLine("\nEvent name: " + eventName +
                "                          Target: " + target);

            //Appraisal value of the event
            var Event = EventMatchingName(this.NewEA_character, eventName);

            if (ApplyStrategy && Event.AppraisalValue < 0)
            {
                Console.WriteLine(" \n In progress...  ");
                Console.WriteLine(" Evaluating emotion intensity...  \n");

                Console.WriteLine(" \n  Mood with old intensity  = " + NewEmotionalState_Character.Mood);
                Console.WriteLine("  New Emotion: \n  "
                + string.Concat(NewEmotionalState_Character.GetAllEmotions().Select(
                e => e.EmotionType + ": " + e.Intensity + " ")));

                var ActiveEmotion = this.NewEmotionalState_Character.GetAllEmotions().LastOrDefault();

                //max averange contrary personality = 36
                var avg = ((this.Personality.Extraversion + this.Personality.Openness) / 2);
                var tanh = (float)(((Math.Abs(ActiveEmotion.Intensity / 2)) * (Math.Tanh((2 * avg - 100) / 40))) + (Math.Abs(ActiveEmotion.Intensity / 2)));
                var ModifiedValue = tanh;

                if (ModifiedValue < this.ObjetiveEmotion)
                {
                    var FAtiMAconfigs = new EmotionalAppraisalConfiguration();//Constant values
                    var MoodDueToEvent = (float)ActiveEmotion.Valence * (ActiveEmotion.Intensity * FAtiMAconfigs.MoodInfluenceOnEmotionFactor);
                    var valueMoodDueToEvent = MoodDueToEvent < -10 ? -10 : (MoodDueToEvent > 10 ? 10 : MoodDueToEvent);
                    var CurrentMoodDueToEvent = this.NewEmotionalState_Character.Mood;
                    var MoodWithoutEventValue = CurrentMoodDueToEvent - MoodDueToEvent;

                    //To create and update the emotion
                    var NewEmotion = new EmotionalAppraisal.DTOs.EmotionDTO
                    {
                        CauseEventId = ActiveEmotion.CauseId,
                        Type = ActiveEmotion.EmotionType,
                        Intensity = ModifiedValue - 1,
                        CauseEventName = events.GetNTerm(3).ToString(),
                        Target = EventFatima.GetNTerm(4).ToString(),
                    };

                    var newConcreteEmotionCharacter = new ConcreteEmotionalState(); //EmotionalState Aux
                    this.NewEmotionalState_Character.RemoveEmotion(ActiveEmotion, this.NewAm_character); //Remove the emotion from agent
                    newConcreteEmotionCharacter.AddActiveEmotion(NewEmotion, this.NewAm_character); //Add new emotion in agent
                    var NewActiveEmotion = newConcreteEmotionCharacter.GetAllEmotions().LastOrDefault(); //gets the last emotion (that we already add)
                    var emoDisp = this.NewEA_character.GetEmotionDisposition(ActiveEmotion.EmotionType); //Making new emotion
                    emoDisp.Threshold = -1;
                    var tick = this.NewAm_character.Tick;//time in simulation
                    var NewEmotionalIntensity = this.NewEmotionalState_Character.AddEmotion(//add new intensity emotion in the agent
                    NewActiveEmotion, this.NewAm_character, emoDisp, tick);
                    var e = this.NewEmotionalState_Character.GetAllEmotions().LastOrDefault().Intensity;
                    if (NewEmotionalIntensity != null)
                        Console.WriteLine("\n Calculated Intensity = " + NewEmotionalIntensity.Intensity + "  New Intesity = " + e);
                    
                    //Update the Mood
                    var NewEmoValence = this.NewEmotionalState_Character.GetAllEmotions().Select(e => (float)e.Valence).LastOrDefault();
                    var NewEmoIntencity = this.NewEmotionalState_Character.GetAllEmotions().Select(e => e.Intensity).LastOrDefault();
                    var moodDouToNewIntensity = NewEmoValence * (NewEmoIntencity * FAtiMAconfigs.MoodInfluenceOnEmotionFactor);
                    var MoodDouToNewIntensity = moodDouToNewIntensity < -10 ? -10 : (moodDouToNewIntensity > 10 ? 10 : moodDouToNewIntensity);
                    var NewMood = MoodWithoutEventValue + MoodDouToNewIntensity;
                    this.NewEmotionalState_Character.Mood = NewMood;
                    
                    Console.WriteLine(" \n  New Mood = " + NewEmotionalState_Character.Mood);
                    Console.WriteLine("  New Emotion: \n  "
                    + string.Concat(NewEmotionalState_Character.GetAllEmotions().Select(
                    e => e.EmotionType + ": " + e.Intensity + " ")));
                    AppliedStrategy = true;
                }
                else
                {
                    Console.WriteLine("\n Strategy not applied due to : Emotion limit was achieved ===> " + (ModifiedValue >= this.ObjetiveEmotion));
                    Console.WriteLine("\n New possible value = " + ModifiedValue + " -User defined limit = " + this.ObjetiveEmotion);
                }
            }
            else
            {
                Console.WriteLine("\n Strategy not applied due to :\n Agent's personality can apply the strategy ===> positive emotion = +" + Event.AppraisalValue);
            }
            Console.WriteLine("\n Response Modulation was applied: " + AppliedStrategy);
            return AppliedStrategy;
        }





        //Utilities
        internal void UpdateEmotionalAppraisal(EmotionalAppraisalAsset character, string TypeAppraisal, float ValueAppraisal, Name EventMatchingTemplate, int index)
        {
            //New Appraisal variables
            var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>()
            {
                new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                {
                    Name = TypeAppraisal, Value = (Name.BuildName(ValueAppraisal))
                }
            };
            var rule = new EmotionalAppraisal.DTOs.AppraisalRuleDTO()
            {
                EventMatchingTemplate = EventMatchingTemplate,
                AppraisalVariables = new AppraisalVariables(appraisalVariableDTO)
            };

            //To remove old EvenMatTem and insert the new
            var ea_CharacterList = character.GetAllAppraisalRules().ToList();
            ea_CharacterList.RemoveAt(index);
            ea_CharacterList.Insert(index, rule);

            //To remove all old EvenMatTem 
            var deleteRules = character.GetAllAppraisalRules().ToList();
            character.RemoveAppraisalRules(deleteRules);

            //To insert the new EvenMatTem into ea_Character
            for (int i = 0; i < ea_CharacterList.Count; i++)
            {
                character.AddOrUpdateAppraisalRule(ea_CharacterList[i]);
            }
        }

        public DataName ReName(Name events)
        {
            DataName dataName = new();

            if (events.NumberOfTerms > 5)
            {
                var EReventsVariables = events.GetTerms();
                var EventValues = string.Join(
                    "", EReventsVariables.Last().ToString().Split('[', ']')).Split("-");

                dataName.IsAvoided = bool.Parse(EventValues[0]);
                var ListEvent = events.GetLiterals().ToList();
                for (int j = 5; j <= ListEvent.Count; j++)
                {
                    ListEvent.RemoveAt(5);

                }
                dataName.EventTypeFatima = Name.BuildName(ListEvent);
            }
            else { dataName.EventTypeFatima = events; return dataName; }

            return dataName;
        }

        public EventAppraisalValues EventMatchingName(EmotionalAppraisalAsset ea_character, Name eventName)
        {
            EventAppraisalValues EventAppraisal = new();
            for (int j = 0; j < ea_character.GetAllAppraisalRules().ToList().Count; j++) //find a specific event
            {
                var EventTemplate = ea_character.GetAllAppraisalRules().ElementAt(j).EventMatchingTemplate;
                //match the event 
                if ((EventTemplate.GetNTerm(3).Equals(eventName)))
                {
                    EventAppraisal.index = j;                                      
                }
            }
            
            //Apraisal variable and value
            var SplitAppraisalVar = ea_character.GetAllAppraisalRules().ElementAt(EventAppraisal.index).AppraisalVariables;
            var NumAppVar = SplitAppraisalVar.appraisalVariables;
            var AppraisalVariable = SplitAppraisalVar.ToString().Split("=");
            if (NumAppVar.Count >= 2) //REVISAR CUANDO ES MÁS DE UNA VARIABLE DE VALORACIÓN
            { 
                //no se tenia configurafo para más de una variable de valoración
                EventAppraisal.AppraisalType  = NumAppVar[0].Name.Trim();
                EventAppraisal.AppraisalValue = float.Parse(NumAppVar[0].Value.ToString().Trim());
            }
            else
            {
                EventAppraisal.AppraisalType = AppraisalVariable[0].Trim();
                EventAppraisal.AppraisalValue = float.Parse(AppraisalVariable[1].Trim());
            }
            return EventAppraisal;
        }

        public bool negativeAppraisal(Name events)
        {
            //Event
            bool IsNegative = false;
            var newEvent = ReName(events);
            EventFatima = newEvent.EventTypeFatima;
            var eventName = EventFatima.GetNTerm(3).ToString();

            //finds the appraisal value of event
            var Event = EventMatchingName(this.NewEA_character, (Name)eventName);
            if (Event.AppraisalValue < 0)
                IsNegative = true;

            return IsNegative;
        }


    }
}
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
        public double ObjectiveIntensityEmotion { get; private set; }
        public Dictionary<string, string> Dic_RelatedActions { get; private set; }
        private float ERmood;

        public EmotionRegulation() { }
        public EmotionRegulation(
            EmotionalAppraisalAsset ea_character,
            EmotionalDecisionMakingAsset edm_character,
            AM am_character,
            ConcreteEmotionalState emotionalEstate_character,
            PersonalityTraits personalityTraits,
            Dictionary<string, string> relatedActions,
            List<Name> AlternativeEvents,
            double objetiveEmotion)
        {
            this.NewEA_character = ea_character;
            this.NewEdm_character = edm_character;
            this.NewAm_character = am_character;
            this.NewEmotionalState_Character = emotionalEstate_character;
            this.Personality = personalityTraits;
            this.ObjectiveIntensityEmotion = objetiveEmotion;
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
            public Dictionary<string,(float value, Name target)> DAppraisalVariblesData { get; set; }
            public int index { get; set; }
            public bool IsEventNegative { get; set; }
        }

        //Situation Selection 
        public bool SituationSelection(Name events)
        {
            Console.WriteLine("\n---------------------Situation Selection-------------------------");
            //Personality
            AppliedStrategy = false;
            var DSituationSelection = this.Personality.DStrategyPower.Where((strategy, power) => strategy.Key == "Situation Selection");
            var ExistStrategy = DSituationSelection.Any();
            var StrategyPower = DSituationSelection.Select(p => p.Value.Trim() == "Strongly"|| p.Value.Trim() == "Lightly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy && StrategyPower;
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
                var EventData = EventMatchingAppraisal(this.NewEA_character, eventName);
                var EventMatchingAppRule = this.NewEA_character.GetAllAppraisalRules().ElementAt(EventData.index).EventMatchingTemplate;
                var ListEvent = NewEvent.EventTypeFatima.GetLiterals().ToList();

                //Building new eventTemplate adding word NOT at the event
                var LEmotionMatchTemplate = EventMatchingAppRule.GetTerms().ToList(); //Elements of Event matching of the event
                ListEvent.RemoveAt(3);
                LEmotionMatchTemplate.RemoveAt(3);
                var NewEventName = Name.BuildName("Not-" + eventName);
                LEmotionMatchTemplate.Insert(3, NewEventName);
                ListEvent.Insert(3, NewEventName);
                var NewEventMatchingTemplate = Name.BuildName(LEmotionMatchTemplate); //new EventMatchingTemplate with NOT word
                EventFatima = Name.BuildName(ListEvent); //new event with NOT word

                //modifíca las variables de valoración y las vulve cero para no generar ninguna emoción en el agente.
                var NameVariable = EventData.DAppraisalVariblesData.Select(e => e.Key);
                foreach (var name in NameVariable)
                {
                    var target = EventData.DAppraisalVariblesData[name].target;
                    EventData.DAppraisalVariblesData[name] = (0, target); //convert to cero the appraisal value
                }

                UpdateEmotionalAppraisalRules(this.NewEA_character, EventData.DAppraisalVariblesData, NewEventMatchingTemplate, EventData.index);
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
            var DSituationSelection = this.Personality.DStrategyPower.Where((strategy, power) => strategy.Key == "Situation Modification");
            var ExistStrategy = DSituationSelection.Any();
            var StrategyPower = DSituationSelection.Select(p => p.Value.Trim() == "Strongly"|| p.Value.Trim() == "Lightly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy && StrategyPower;


            //Event
            var newEvent = ReName(events);
            EventFatima = newEvent.EventTypeFatima;
            var target = EventFatima.GetNTerm(4).ToString();
            var eventName = EventFatima.GetNTerm(3).ToString();
            var existRelatedActions = Dic_RelatedActions.Where(e => e.Value == eventName).Any();

            //finds the appraisal value of event
            var EventData = EventMatchingAppraisal(this.NewEA_character, (Name)eventName);

            Console.WriteLine("\nEvent name: " + eventName +
                "                          Target: " + target);
            Console.WriteLine("\nCould do any actions ? : " + existRelatedActions);
            
            //Variables de valoración compuestas?
            bool GreaterPower = false;
            if (EventData.DAppraisalVariblesData.Count>1)
            {
                if (EventData.DAppraisalVariblesData.Select(v => v.Key == OCCAppraisalVariables.DESIRABILITY_FOR_OTHER).Any()) 
                {
                    var AppValues = EventData.DAppraisalVariblesData.Select(e => e.Value.value);
                    GreaterPower = (Math.Abs(AppValues.Sum() * 0.5)) > 4; //get the final intensity of emotion 
                }
                else
                {
                    //ToDo: Test the rest compound emotions 
                }
            }
            else
            {
                if (EventData.DAppraisalVariblesData.Select(v => v.Key == OCCAppraisalVariables.LIKE).Any())
                {
                    var AppValues = EventData.DAppraisalVariblesData.Select(e => e.Value.value).FirstOrDefault(); // get the level of app. var. 
                    const float magicFactor = -0.7f;
                    AppValues *= magicFactor;
                    GreaterPower = (AppValues > 4);
                }
                else
                {
                    //If OCCAppraisalVariables == DESIRABILITY ...
                    var AppValues = EventData.DAppraisalVariblesData.Select(e => e.Value.value).FirstOrDefault(); // get the level of app. var. 
                    const float Factor = -1f;
                    AppValues *= Factor;
                    GreaterPower = (AppValues > 4);
                }

            }

            //Conditions
            if (ApplyStrategy && existRelatedActions && GreaterPower)//*-4
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

                var avg = ((this.Personality.Neuroticism + this.Personality.Agreeableness) / 2);
                foreach (var AedmCharacter in LActionsEDM) 
                {
                    //ToDo: check when we have more than one related action to the event
                    var ModifiedValue = 0f;
                    var NameVariable = EventData.DAppraisalVariblesData.Select(e => e.Key);
                    foreach (var name in NameVariable)
                    {

                        avg = ((this.Personality.Neuroticism + this.Personality.Agreeableness) / 2);
                        var tanh = (float)((Math.Abs((EventData.DAppraisalVariblesData[name].value / 2)) * (Math.Tanh(-(2 * avg - 100) / 40))) - (Math.Abs(EventData.DAppraisalVariblesData[name].value / 2)));
                        var t = EventData.DAppraisalVariblesData[name].target;
                        EventData.DAppraisalVariblesData[name] = (tanh, t); 

                    }

                    var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(EventData.index).EventMatchingTemplate;

                    
                    if (EventData.DAppraisalVariblesData.Count > 1 & EventData.DAppraisalVariblesData.Select(v => v.Key == OCCAppraisalVariables.DESIRABILITY_FOR_OTHER).Any())
                    {
                        var AppValues = EventData.DAppraisalVariblesData.Select(e => e.Value.value);
                        ModifiedValue = (Math.Abs(AppValues.Sum() * 0.5f));
                    }
                    else
                    {
                        if (EventData.DAppraisalVariblesData.Select(v => v.Key == OCCAppraisalVariables.LIKE).Any())
                        {
                            var AppValues = EventData.DAppraisalVariblesData.Select(e => e.Value.value).FirstOrDefault();
                            const float magicFactor = -0.7f;
                            ModifiedValue =  AppValues = magicFactor;
                        }
                        else
                        {
                            //If OCCAppraisalVariables == DESIRABILITY ...
                            //ToDo: Test the rest of the compound emotions, this is for the other strategies too
                        }
                    }

                    if (ModifiedValue <= this.ObjectiveIntensityEmotion)
                    {
                        var actionTo = AedmCharacter.Action.ToString();
                        Console.WriteLine(" Would decide: " + actionTo);
                        UpdateEmotionalAppraisalRules(this.NewEA_character, EventData.DAppraisalVariblesData, EventTemplate, EventData.index);
                        AppliedStrategy = true;
                    }
                    else
                    {
                        Console.WriteLine("\n Strategy not applied due to : Emotion limit was achieved ===> " + (ModifiedValue >= -this.ObjectiveIntensityEmotion));
                        Console.WriteLine("\n New possible value = " + Math.Abs(ModifiedValue) + " -User defined limit = " + this.ObjectiveIntensityEmotion);
                    }
                }
            }
                else { Console.WriteLine("\n Strategy not applied due to :\n Agent's personality can apply the strategy ===> " + ApplyStrategy +
                    ", or : Exist Related Actions is ===> " + existRelatedActions + ", or :\n " +
                    "Defined Event Appraisal Value is greater than -5 : "+ GreaterPower); }
                Console.WriteLine("\nSituation Modification could be applied: " + AppliedStrategy);
        
            return AppliedStrategy;
        }
        
        //Attention Deployment 
        public bool AttentionDeployment(Name events)
        {
            Console.WriteLine("\n---------------------Attention Deployment------------------------");

            AppliedStrategy = false;
            var DAttentionDeployment = this.Personality.DStrategyPower.Where((strategy, power) => strategy.Key == "Attention Deployment");
            var ExistStrategy = DAttentionDeployment.Any();
            var StrategyPower = DAttentionDeployment.Select(p => p.Value.Trim() == "Strongly"|| p.Value.Trim() == "Lightly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy && StrategyPower;

            var target = events.GetNTerm(4);
            var eventName = events.GetNTerm(3);

            var NewEvent = ReName(events);
            EventFatima = NewEvent.EventTypeFatima;

            Console.WriteLine("\nEvent name: " + eventName +
                "                           Target: " + this.EventFatima.GetNTerm(4));

            //Only when the current event is negative
            var EventData = EventMatchingAppraisal(this.NewEA_character, eventName);
            IEnumerable<Name> RelatedEvent = Enumerable.Empty<Name>();
            bool ExistEventRelated = false;
            if (EventData.IsEventNegative)
            {
                var EventNameAM = this.NewAm_character.RecallAllEvents().Select(ep => ep.EventName);
                RelatedEvent = EventNameAM.Where(e => e.GetNTerm(4) == target).Select(a => a.GetNTerm(3));
                ExistEventRelated = RelatedEvent.Any();
                Console.WriteLine("Does Exist any event related ? : " + ExistEventRelated);
                foreach (var e in RelatedEvent)
                    Console.WriteLine("Related events : " + e);
            }

            //Conditions
            if (ApplyStrategy && ExistEventRelated)
            {
                List<float> ListEventsValue = new();
                Console.WriteLine(" \n In progress...  ");
                Console.WriteLine(" Evaluating past events...  ");
                
                foreach (var n in RelatedEvent)
                {
                    var EventValues = EventMatchingAppraisal(this.NewEA_character, n); //get the values of the events past
                    //ToDo: evaluating among different appraisal variables, which are compound appraisals.
                    ///Qué pasa cuando hay eventos pasados con emiciones compuestas?
                    ///Actualmente se esta tomando la primera appraisal variable (porque es la unica que hay en los eventos pasados de prueba),
                    ///pero cuando existan más variables de valoración esto no va a funcionar, se debe de considerar el tipo de valance de la
                    ///emoción que se va a generar.

                    ListEventsValue.Add(EventValues.DAppraisalVariblesData.Values.FirstOrDefault().value);
                }

                var avg = ListEventsValue.Average();
                var ModifiedValue = 0f;
                var NameVariable = EventData.DAppraisalVariblesData.Select(e => e.Key);
                foreach (var name in NameVariable)
                {
                        var tanh = (float)((Math.Abs((EventData.DAppraisalVariblesData[name].value / 2)) * (Math.Tanh((2 * avg - 10) / 4))) - (Math.Abs(EventData.DAppraisalVariblesData[name].value / 2)));
                        var t = EventData.DAppraisalVariblesData[name].target;
                        EventData.DAppraisalVariblesData[name] = (tanh, t);
                }
                var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(EventData.index).EventMatchingTemplate;

                if (EventData.DAppraisalVariblesData.Count > 1)
                {
                    if (EventData.DAppraisalVariblesData.Select(v => v.Key == OCCAppraisalVariables.DESIRABILITY_FOR_OTHER).Any())
                    {
                        var AppValues = EventData.DAppraisalVariblesData.Select(e => e.Value.value);
                        ModifiedValue = (Math.Abs(AppValues.Sum() * 0.5f));
                    }
                    else
                    {

                    }


                }
                else
                {
                    if (EventData.DAppraisalVariblesData.Select(v => v.Key == OCCAppraisalVariables.LIKE).Any())
                    {
                        var AppValues = EventData.DAppraisalVariblesData.Select(e => e.Value.value).FirstOrDefault();
                        const float magicFactor = -0.7f;
                        ModifiedValue = AppValues * magicFactor; //get the intensity of emotion
                    }
                    else
                    {
                        //If OCCAppraisalVariables == DESIRABILITY ...
                    }
                }


                if (ModifiedValue <= this.ObjectiveIntensityEmotion)
                {
                    UpdateEmotionalAppraisalRules(this.NewEA_character, EventData.DAppraisalVariblesData, EventTemplate, EventData.index);
                    AppliedStrategy = true;
                }
                else
                {
                    Console.WriteLine("\n Strategy not applied due to : Emotion limit was achieved ===> " + (ModifiedValue >= -this.ObjectiveIntensityEmotion));
                    Console.WriteLine("\n New possible value = " + Math.Abs(ModifiedValue) + " -User defined limit = " + this.ObjectiveIntensityEmotion);
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
            var DAttentionDeployment = this.Personality.DStrategyPower.Where(
                (strategy, power) => strategy.Key == "Cognitive Change");
            var ExistStrategy = DAttentionDeployment.Any();
            var StrategyPower = DAttentionDeployment.Select(p => p.Value.Trim() == "Strongly" || p.Value.Trim() == "Lightly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy && StrategyPower;

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
                foreach (var alternativeEvent in AlternativeEventsName)
                {
                    var AlternativeEventData = EventMatchingAppraisal(this.NewEA_character, alternativeEvent.GetNTerm(3));
                    Console.WriteLine("\n '" + alternativeEvent.GetNTerm(3) + "'");
                    LAlternativeEventsValue.Add(AlternativeEventData.DAppraisalVariblesData.Values.FirstOrDefault().value);
                }
                var avg = LAlternativeEventsValue.Average();//Promedio de los nuevos eventos
                //var ContraryPersonality = (this.Personality.Openness - this.Personality.Neuroticism);
                var MoodAndAvg = ((ERmood+8) + avg);
                
                var CurrentEventData = EventMatchingAppraisal(this.NewEA_character, eventName);//valoración del evento actual

                var ModifiedValue = 0f;
                var NameVariable = CurrentEventData.DAppraisalVariblesData.Select(e => e.Key);
                foreach (var name in NameVariable)
                {
                    var Valance = (CurrentEventData.DAppraisalVariblesData[name].value / Math.Abs(CurrentEventData.DAppraisalVariblesData[name].value));
                    var tanh1 = (float)(((CurrentEventData.DAppraisalVariblesData[name].value / 2) * (Math.Tanh((Valance) * (2 * this.Personality.Neuroticism - 100) / 40))) + (CurrentEventData.DAppraisalVariblesData[name].value / 2));
                    var tanh2 = (float)(((CurrentEventData.DAppraisalVariblesData[name].value / 2) * (Math.Tanh((Valance) * (2 * MoodAndAvg - 20) / 8))) + (CurrentEventData.DAppraisalVariblesData[name].value / 2));
                    ModifiedValue = (float)((tanh1 + tanh2) / 2); var t = CurrentEventData.DAppraisalVariblesData[name].target;
                    CurrentEventData.DAppraisalVariblesData[name] = (ModifiedValue, t);
                }
                var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(CurrentEventData.index).EventMatchingTemplate;

                if (CurrentEventData.DAppraisalVariblesData.Count>1 & CurrentEventData.DAppraisalVariblesData.Select(v => v.Key == OCCAppraisalVariables.DESIRABILITY_FOR_OTHER).Any())
                {
                    var AppValues = CurrentEventData.DAppraisalVariblesData.Select(e => e.Value.value);
                    ModifiedValue = (Math.Abs(AppValues.Sum() * 0.5f));
                    //If OCCAppraisalVariables == DESIRABILITY_FOR_OTHER ...
                }
                else
                {
                    if (CurrentEventData.DAppraisalVariblesData.Select(v => v.Key == OCCAppraisalVariables.LIKE).Any())
                    {
                        var AppValues = CurrentEventData.DAppraisalVariblesData.Select(e => e.Value.value).FirstOrDefault();
                        const float magicFactor = -0.7f;
                        ModifiedValue = AppValues * magicFactor; //get the intensity of emotion
                    }
                    else
                    {
                        //If OCCAppraisalVariables == DESIRABILITY ...
                    }
                }

                if (ModifiedValue <= this.ObjectiveIntensityEmotion)
                {
                    UpdateEmotionalAppraisalRules(this.NewEA_character, CurrentEventData.DAppraisalVariblesData, EventTemplate, CurrentEventData.index);
                    AppliedStrategy = true;
                }
                else
                {
                    Console.WriteLine("\n Strategy not applied due to : Emotion limit was achieved ===> " + (ModifiedValue >= -this.ObjectiveIntensityEmotion));
                    Console.WriteLine("\n New possible value = " + Math.Abs(ModifiedValue) + " -User defined limit = " + this.ObjectiveIntensityEmotion);
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
            var DAttentionDeployment = this.Personality.DStrategyPower.Where(
                (strategy, power) => strategy.Key == "Response Modulation");
            var ExistStrategy = DAttentionDeployment.Any();
            var StrategyPower = DAttentionDeployment.Select(p => p.Value.Trim() == "Strongly" || p.Value.Trim() == "Lightly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy && StrategyPower;

            //get event name, construction of the event
            var eventName = events.GetNTerm(3);
            var NewEvent = ReName(events);
            EventFatima = NewEvent.EventTypeFatima;
            var target = EventFatima.GetNTerm(4).ToString();

            Console.WriteLine("\nEvent name: " + eventName +
                "                          Target: " + target);

            //Appraisal value of the event
            var EventData = EventMatchingAppraisal(this.NewEA_character, eventName);

            if (ApplyStrategy && EventData.IsEventNegative)
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

                if (ModifiedValue < this.ObjectiveIntensityEmotion)
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
                    Console.WriteLine("\n Strategy not applied due to : Emotion limit was achieved ===> " + (ModifiedValue <= this.ObjectiveIntensityEmotion));
                    Console.WriteLine("\n New possible value = " + ModifiedValue + " -User defined limit = " + this.ObjectiveIntensityEmotion);
                }
            }
            else
            {
                Console.WriteLine("\n Strategy not applied due to :\n  Agent's personality can apply the strategy ====> " + AppliedStrategy);
            }
            Console.WriteLine("\n Response Modulation was applied: " + AppliedStrategy);
            return AppliedStrategy;
        }

        //Utilities
        internal void UpdateEmotionalAppraisalRules(EmotionalAppraisalAsset character, Dictionary<string,(float value, Name target)> DTypeAppraisal, Name EventMatchingTemplate, int index)
        {
            //New Appraisal variables
            var appraisalVariableDTO = new List<EmotionalAppraisal.DTOs.AppraisalVariableDTO>();
            foreach (var AppEvent in DTypeAppraisal)
            {

                appraisalVariableDTO.Add(new EmotionalAppraisal.DTOs.AppraisalVariableDTO()
                {
                    Name = AppEvent.Key,
                    Value = (Name.BuildName(AppEvent.Value.value)),
                    Target = (Name)AppEvent.Value.target
                });
            }

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

                dataName.IsAvoided = bool.Parse(EventValues[0].ToLower());
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

        public EventAppraisalValues EventMatchingAppraisal(EmotionalAppraisalAsset ea_character, Name eventName)
        {
            //ToDo: check when is used, because in many lines we use them to different proposes, mainly to know the level of appraisal.
            //An example is on the line 419
            EventAppraisalValues EventAppraisal = new();
            for (int j = 0; j < ea_character.GetAllAppraisalRules().ToList().Count; j++) //find a specific index eevent
            {
                var EventTemplate = ea_character.GetAllAppraisalRules().ElementAt(j).EventMatchingTemplate;
                //match the event 
                if (EventTemplate.GetNTerm(3).Equals(eventName))
                {
                    EventAppraisal.index = j;
                }
            }


            ///////////////////////////////////////////////////////////////
            ///  OCCAppraisalVariables.DESIRABILITY
            ///  OCCAppraisalVariables.DESIRABILITY_FOR_OTHER
            ///  
            ///             #### OCCAppraiseFortuneOfOthers #####
            ///             
            ///  Lo que hace que una emición sea negativa es la variable 'Desirability for Otrher'.
            ///  Ejemplo del código:
            ///  
            ///         float potential = (Math.Abs(desirabilityForOther) + Math.Abs(desirability)) * 0.5f;
            ///         if (desirability >= 0)
            ///                 emoType = (desirabilityForOther >= 0) ? OCCEmotionType.HappyFor : OCCEmotionType.Gloating;
            ///         else
            ///                 emoType = (desirabilityForOther >= 0) ? OCCEmotionType.Resentment : OCCEmotionType.Pity;
            ///
            /// Class: OCCAffectDerivationComponent
            //////////////////////////////////////////////////////////////


            /////////////////////////////////////////////////////////////
            ///  OCCAppraisalVariables.DESIRABILITY
            ///  OCCAppraisalVariables.PRAISEWORTHINESS
            ///          
            ///             #### OCCAppraiseCompoundEmotions ####
            ///             
            /// Lo que hace que una emición sea negativa es la variable 'Desirability'.
            /// Ejemplo del código:
            /// 
            ///         float potential = Math.Abs(desirability + praiseworthiness) * 0.5f;
            ///         if (target == "" || target == Name.SELF_STRING)
            ///         {
            ///             direction = Name.SELF_SYMBOL;
            ///             emoType = (desirability > 0) ? OCCEmotionType.Gratification : OCCEmotionType.Remorse;
            ///         }
            ///         else
            ///         {
            ///             direction = Name.BuildName(target);
            ///             emoType = (desirability > 0) ? OCCEmotionType.Gratitude : OCCEmotionType.Anger;
            ///         }
            ///         
            /// Class: OCCAffectDerivationComponent
            ////////////////////////////////////////////////////////////




            //Apraisal variable and value

            Dictionary<string, (float value, Name target)> DAuxDataEvent = new();
            var SplitAppraisalVar = ea_character.GetAllAppraisalRules().ElementAt(EventAppraisal.index).AppraisalVariables;
            var ListAppVar = SplitAppraisalVar.appraisalVariables;
            foreach (var AppVar in ListAppVar)
            {
                var NameVariable = AppVar.Name.ToString().Trim();
                var ValueVariable = float.Parse(AppVar.Value.ToString());
                var target = AppVar.Target;
                DAuxDataEvent.Add(NameVariable, (ValueVariable, target));
            }
            EventAppraisal.DAppraisalVariblesData = DAuxDataEvent;
            float valDes = 0f;
            if (EventAppraisal.DAppraisalVariblesData.Count > 1)
                valDes = EventAppraisal.DAppraisalVariblesData.Where(e => e.Key == OCCAppraisalVariables.DESIRABILITY).Select(r => r.Value.value).Sum();
            else
            {
                valDes = EventAppraisal.DAppraisalVariblesData.Values.FirstOrDefault().value;
            }
            if (valDes < 0)
                EventAppraisal.IsEventNegative = true;
            return EventAppraisal;
        }

    }
}
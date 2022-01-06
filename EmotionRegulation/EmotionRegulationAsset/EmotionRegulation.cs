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

    public class EmotionRegulationAsset
    {
        public InputDF OutputData = new();
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

        public const string SituationSelectionString    = "Situation Selection";
        public const string SituationModificationString = "Situation Modification";
        public const string AttentionDeploymentString   = "Attention Deployment";
        public const string CognitiveChangeString       = "Cognitive Change";
        public const string ResponseModulationString    = "Response Modulation";
        public float GoalSignificance { get; set; } //testing the intensity based on goals;
        /// este valor sirve por el momento para calcular la intesidad de las emociones basadas en variables de valoración 
        /// GOALSUCCESS, pero es posible (muy seguramente) que cuando se utilize RolePlayCharacter ya no sea necesario.

 
        public EmotionRegulationAsset() { }
        public EmotionRegulationAsset(
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
            OutputData =  new();
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
            public bool IsEquals { get; set; }
            public bool ExistMatch { get; set; }
        }
        public struct InputDF
        {
            public bool StrategySuccessful { get; set; }
            public List<(string Event, string Strategy)> Results { get; set; }

        }

        //Situation Selection 
        public bool SituationSelection(Name events)
        {
            Console.WriteLine("\n---------------------Situation Selection-------------------------");
            
            AppliedStrategy = false;
            var NewEvent = ReName(events);
            EventFatima = NewEvent.EventTypeFatima;

            Console.WriteLine("Event name: " + events.GetNTerm(3) +
                "                         Target: " + EventFatima.GetNTerm(4));
            Console.WriteLine("\nCan be avoided: " + NewEvent.IsAvoided);
            
            //Conditions
            if (NewEvent.IsAvoided)
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

                var NameVariable = EventData.DAppraisalVariblesData.Select(e => e.Key);
                foreach (var name in NameVariable)
                {
                    var target = EventData.DAppraisalVariblesData[name].target;
                    EventData.DAppraisalVariblesData[name] = (0, target); //convert to cero the appraisal value
                }
                
                UpdateEmotionalAppraisalRules(this.NewEA_character, EventData.DAppraisalVariblesData, NewEventMatchingTemplate, EventData.index);
                ///Por qué hice esto?????
                ///para poder evaluar una nueva emoción como nula, el mood debe de ser cero, de lo contrario se dispara el mínimo valor
                ///generado por FAtiMA
                //this.NewEmotionalState_Character.Mood = 0; 
                AppliedStrategy = true;
                Console.WriteLine(" \nNew event: " + EventFatima.GetNTerm(3));
            }
            else { Console.WriteLine("\n Strategy not applied due to :\n Event cannot be Avoided : " + !NewEvent.IsAvoided); }
            Console.WriteLine("\nSituation Selection was applied: " + AppliedStrategy);
            Console.WriteLine("---------------------------------------------------------------");
            return AppliedStrategy;
        }

        //Situation Modification 
        public bool SituationModification(Name events)
        {
            Console.WriteLine("\n--------------------Situation Modification-----------------------");
            AppliedStrategy = false;
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
            Console.WriteLine("\nAgent could take any action ? : " + existRelatedActions);

            bool GreaterIntensity = false;
            if (EventData.IsEventNegative) 
            {
                /// Se determinó qué se aplicaría la estrategia sólo si el nivel de valoración supera cierto límite (appraisal variable), 
                /// se debería considerar para todas las estrategias? Actualmente solo se está considerando para esta estrategia.
                /// /// Aquí se está confundiendo el la intensidad de la emoción final con en el nivel de valoracion (appraisal variable),
                /// IsEventNegative posiblemente no sea necesario dentro de las estrategias, ya que si se está utilizando Emotion Regulation
                /// es porque el evento está considerado como negativo (revisar en que parte se considera está cuestión).
                var intensity = IntensityDerivation(EventData);
                GreaterIntensity = (intensity > 4);
            }

            //Conditions
            if (existRelatedActions && GreaterIntensity)//*-4
            {
                var DRelatedActions = Dic_RelatedActions.Where(e => e.Value == eventName);
                var LRelatedActions = DRelatedActions.Select(a => a.Key).ToList();
                List<ActionLibrary.DTOs.ActionRuleDTO> LActionsToEvent = new();

                foreach (var act in LRelatedActions)
                {
                    LActionsToEvent.Add(this.NewEdm_character.GetAllActionRules().Where(
                    a => a.Action.ToString() == act).FirstOrDefault());
                }

                var RelatedActions = this.NewEdm_character.GetAllActionRules().Where(
                    a => a.Action.ToString() == DRelatedActions.Select(ea => ea.Key).FirstOrDefault());

                Console.WriteLine(" \n In progress...  ");
                Console.WriteLine(" Evaluating actions value...  ");
                
                var PersonalityAgainst = (float)((this.Personality.Neuroticism + this.Personality.Agreeableness) / 2);
                var PersonalityPro = (float)((this.Personality.Conscientiousness + this.Personality.Extraversion + this.Personality.Openness) / 3);
                
                foreach (var Action in LActionsToEvent)
                { 
                    ///Cómo valorar las diferentes acciones.. con base a qué se puede calcular las nuevas variables de valoración 
                    ///actualmente no se tiene niguna distinción entre las diferentes acciones que se puedan ejucutar.
                    var NameAppraisalVariable = EventData.DAppraisalVariblesData.Select(e => e.Key);
                    foreach (var name in NameAppraisalVariable)
                    {
                        ///Para el cálculo de la nueva intensidad, se toma en cuenta los valores de personalidad.
                        
                        var valoration = EventData.DAppraisalVariblesData[name].value;
                        int valance1 =  1, valance2 = -1;
                        if (valoration >= 0) { valance1 = -1; valance2 = 1; }
                        
                        var tanhAgainst = LevelFunction(valoration, PersonalityAgainst, 100, valance1);
                        var tanhPro     = LevelFunction(valoration, PersonalityPro, 100, valance2);
                        var t = EventData.DAppraisalVariblesData[name].target;
                        var tanh = (tanhAgainst + tanhPro) / 2;

                        EventData.DAppraisalVariblesData[name] = (tanh, t); // update the appraisal variables
                    }

                    var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(EventData.index).EventMatchingTemplate;
                    var newIntensity = IntensityDerivation(EventData);
                    var Threshold = newIntensity <= this.ObjectiveIntensityEmotion;
                    if (Threshold)
                    {
                        var actionTo = Action.Action.ToString();
                        Console.WriteLine("Agent Would decide to do : " + actionTo);
                        UpdateEmotionalAppraisalRules(this.NewEA_character, EventData.DAppraisalVariblesData, EventTemplate, EventData.index);
                        AppliedStrategy = true;
                    }
                    else
                    {
                        Console.WriteLine("\n Strategy hasn't applied due to : Intensity threshold failed = " + !Threshold);
                        Console.WriteLine(" New possible intensity = " + Math.Abs(newIntensity) + " (User defined limit = " + this.ObjectiveIntensityEmotion + ")");
                    }
                }
            }
            else
            {
                Console.WriteLine("\n Strategy hasn't applied due to :\n Doesn't exists Related Actions = " + !existRelatedActions +
                                                                               "\n Appraisal variable isn't significant = " + GreaterIntensity);
            }
            Console.WriteLine("\nSituation Modification was applied: " + AppliedStrategy);
            Console.WriteLine("---------------------------------------------------------------");

            return AppliedStrategy;
        }
        
        //Attention Deployment 
        public bool AttentionDeployment(Name events)
        {
            Console.WriteLine("\n---------------------Attention Deployment------------------------");

            AppliedStrategy = false;
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
            List<float> LValuesRelatedEvents = new();
            if (EventData.IsEventNegative)
            {
                /// IsEventNegative posiblemente no sea necesario dentro de las estrategias, ya que si se está utilizando Emotion Regulation
                /// es porque el evento está considerado como negativo (revisar en que parte se considera está cuestión).
                var PastEventName = this.NewAm_character.RecallAllEvents().Select(ep => ep.EventName);
                RelatedEvent = PastEventName.Where(e => e.GetNTerm(4) == target).Select(a => a.GetNTerm(3));
                var valance = "negative"; // sólo faltaria varificar que se muestre en la consola, pero sí funciona como debería. Esto muestra en consola si el evento alternativo es positivo... 
                foreach (var n in RelatedEvent)
                {
                    var EventValues = EventMatchingAppraisal(this.NewEA_character, n); //get the values of the events past
                    if (!EventValues.IsEventNegative)
                    {
                        valance = "positive";
                        var appValue = EventValues.DAppraisalVariblesData;
                        var intensity = IntensityDerivation(EventValues);
                        LValuesRelatedEvents.Add(intensity);
                        ExistEventRelated = true;
                    }
                }
                Console.WriteLine("Does exist a event related ? : " + ExistEventRelated);
                foreach (var e in RelatedEvent)
                    Console.WriteLine("Related events : " + e + " ---> "+ valance);
            }

            //Conditions
            if (ExistEventRelated)
            {
                Console.WriteLine(" \n In progress...  ");
                Console.WriteLine(" Evaluating past events...  ");

                var avgRelatedEvents = LValuesRelatedEvents.Average();
                var NameAppraisalVariable = EventData.DAppraisalVariblesData.Select(e => e.Key);
                foreach (var name in NameAppraisalVariable)
                {
                    var valoration = EventData.DAppraisalVariblesData[name].value;
                    int valance1 = 1;
                    if (valoration >= 0) { valance1 = -1; }
                    var tanh = LevelFunction(valoration, avgRelatedEvents, 10, valance1);
                    var t = EventData.DAppraisalVariblesData[name].target;
                    EventData.DAppraisalVariblesData[name] = (tanh, t);
                }

                var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(EventData.index).EventMatchingTemplate;
                var newIntensity = IntensityDerivation(EventData);
                var Threshold = newIntensity <= this.ObjectiveIntensityEmotion;

                if (Threshold)
                {
                    UpdateEmotionalAppraisalRules(this.NewEA_character, EventData.DAppraisalVariblesData, EventTemplate, EventData.index);
                    AppliedStrategy = true;
                }
                else
                {
                    Console.WriteLine("\n Strategy hasn't applied due to : Intensity threshold failed = " + !Threshold);
                    Console.WriteLine(" New possible intensity = " + Math.Abs(newIntensity) + " (User defined limit = " + this.ObjectiveIntensityEmotion + ")");
                }
            }
            else { Console.WriteLine("\n Strategy hasn't applied due to :\n Doesn't exist related events : " + !ExistEventRelated);
            }

            Console.WriteLine("\n Attention Deployment was applied : " + AppliedStrategy);
            Console.WriteLine("---------------------------------------------------------------");
            return AppliedStrategy;
        }
        
        // Cognitive Change
        public bool CognitiveChange(Name events)
        {
            Console.WriteLine("\n---------------------Cognitive Change------------------------");
            var ERmood = this.NewEmotionalState_Character.Mood;
            
            AppliedStrategy = false;
            //get event name and it construct the event
            var eventName = events.GetNTerm(3);
            var NewEvent = ReName(events);
            EventFatima = NewEvent.EventTypeFatima;

            //Buscar eventos relacionados con el evento actual para reinterpretar
            var AlternativeEventsName = this.LalternativeEvents.Where(
                e => e.GetNTerm(4) == EventFatima.GetNTerm(3));

            List<float> LAlternativeEventsValues = new();
            bool existAlternativeEvents = false;
            //Obtiene el valor de la valoracion de los eventos relacionados
            foreach (var alternativeEvent in AlternativeEventsName)
            {
                var AlternativeEventData = EventMatchingAppraisal(this.NewEA_character, alternativeEvent.GetNTerm(3));
                if (!AlternativeEventData.IsEventNegative && AlternativeEventData.ExistMatch)
                {
                    ///ExistMatch no estoy seguro de si se deba implementar, ya que verifica que el usuario haya declarado variables de valoración
                    ///para ese evento.
                    ///De igual manera,IsEventNegative, se utilizá para la estrategia anterior, pero para esta estratgia se acordó que el usuario
                    ///debería de generar solo evaluciones positivas para los eventos a interpretar. Esto se puede delimitar al momento de 
                    ///implementar el asset.
                    Console.WriteLine("\n '" + alternativeEvent.GetNTerm(3) + "'");
                    var appValue = AlternativeEventData.DAppraisalVariblesData;
                    var intensity = IntensityDerivation(AlternativeEventData);
                    LAlternativeEventsValues.Add(intensity);
                    existAlternativeEvents = true;
                }
            }
      
            var target = EventFatima.GetNTerm(4).ToString();
            Console.WriteLine("\nEvent name: " + eventName +
               "                          Target: " + target);
            Console.WriteLine("\nCould it've any reinterpretation ? : " + existAlternativeEvents);

            if (existAlternativeEvents)
            {
                Console.WriteLine(" \n In progress...  ");
                Console.WriteLine(" reinterpreting the event...  ");

                var avg = LAlternativeEventsValues.Average();//Promedio de los nuevos eventos alternativos.
                var MoodAndAlternativeEvents = ((ERmood) + avg);
                var PersonalityAgainst = (float)this.Personality.Neuroticism;
                var PersonalityPro = (float)(this.Personality.Conscientiousness + this.Personality.Extraversion + this.Personality.Openness + this.Personality.Agreeableness) / 4;
                
                var CurrentEventData = EventMatchingAppraisal(this.NewEA_character, eventName);//valoración del evento actual
                var NameAppraisalVariable = CurrentEventData.DAppraisalVariblesData.Select(e => e.Key);

                foreach (var name in NameAppraisalVariable)
                {
                    var valoration = CurrentEventData.DAppraisalVariblesData[name].value;
                    int valance1 = 1, valance2 = -1;
                    if (valoration >= 0) { valance1 = -1; valance2 = 1; }
                    
                    var tanhAgainst = LevelFunction(valoration, PersonalityAgainst, 100, valance1);
                    var tanhPro = LevelFunction(valoration, PersonalityPro, 100, valance2);
                    var intensityAccordMoodEvents = LevelFunction(valoration, MoodAndAlternativeEvents, 10, valance2);
                    var tanh = (float)((tanhAgainst + intensityAccordMoodEvents + tanhPro) / 3); 
                    var t = CurrentEventData.DAppraisalVariblesData[name].target;
                    CurrentEventData.DAppraisalVariblesData[name] = (tanh, t);
                }

                var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(CurrentEventData.index).EventMatchingTemplate;
                var newIntensity = IntensityDerivation(CurrentEventData);
                var Threshold = newIntensity <= this.ObjectiveIntensityEmotion;
                if (Threshold)
                {
                    UpdateEmotionalAppraisalRules(this.NewEA_character, CurrentEventData.DAppraisalVariblesData, EventTemplate, CurrentEventData.index);
                    AppliedStrategy = true;
                }
                else
                {
                    Console.WriteLine("\n Strategy hasn't applied due to : Intensity threshold failed = " + !Threshold);
                    Console.WriteLine(" New possible intensity = " + Math.Abs(newIntensity) + " (User defined limit = " + this.ObjectiveIntensityEmotion + ")");
                }
            }
            else
            {
                Console.WriteLine("\n Strategy hasn't applied due to :\n Doesn't exist alternative events : " + !existAlternativeEvents);
            }

            Console.WriteLine("\n Cognitive Change was applied : " + AppliedStrategy);
            Console.WriteLine("---------------------------------------------------------------");
            return AppliedStrategy;
        }
        
        //Response Modulation
        public bool ResponseModulation(Name events)
        {
            Console.WriteLine("\n---------------------Response Modulation------------------------");
            AppliedStrategy = false;

            //get event name, construction of the event
            var eventName = events.GetNTerm(3);
            var NewEvent = ReName(events);
            EventFatima = NewEvent.EventTypeFatima;
            var target = EventFatima.GetNTerm(4).ToString();

            Console.WriteLine("\nEvent name: " + eventName +
                "                          Target: " + target);

            //Appraisal value of the event
            var EventData = EventMatchingAppraisal(this.NewEA_character, eventName);

            if (EventData.IsEventNegative)
            {///De igual manera,IsEventNegative, se utilizá para la estrategia anterior, pero para esta estratgia se acordó que el usuario
             ///debería de generar solo evaluciones positivas para los eventos a interpretar. Esto se puede delimitar al momento de 
             ///implementar el asset.
                Console.WriteLine(" \n In progress...  ");
                Console.WriteLine(" Evaluating emotion intensity...  \n");

                Console.WriteLine(" \n  Mood with old intensity  = " + NewEmotionalState_Character.Mood);
                Console.WriteLine("  New Emotion: \n  "
                + string.Concat(NewEmotionalState_Character.GetAllEmotions().Select(
                e => e.EmotionType + ": " + e.Intensity + " ").LastOrDefault()));

                var ActiveEmotion = this.NewEmotionalState_Character.GetAllEmotions().LastOrDefault();

                var PersonalityAgainst = (float)((
                    this.Personality.Conscientiousness + 
                    this.Personality.Openness +
                    this.Personality.Neuroticism + 
                    this.Personality.Extraversion +
                    this.Personality.Agreeableness) / 4);

                var newIntensity = LevelFunction(ActiveEmotion.Intensity, PersonalityAgainst, 100, -1);
                var Threshold = newIntensity <= this.ObjectiveIntensityEmotion;
                if (Threshold)
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
                        Intensity = newIntensity,
                        CauseEventName = events.GetNTerm(3).ToString(),
                        Target = EventFatima.GetNTerm(4).ToString(),
                    };

                    var newConcreteEmotionCharacter = new ConcreteEmotionalState(); //EmotionalState Aux
                    this.NewEmotionalState_Character.RemoveEmotion(ActiveEmotion, this.NewAm_character); //Remove the emotion from agent
                    newConcreteEmotionCharacter.AddActiveEmotion(NewEmotion, this.NewAm_character); //Add new emotion in agent
                    var NewActiveEmotion = newConcreteEmotionCharacter.GetAllEmotions().LastOrDefault(); //gets the last emotion (that we already add)
                    var emoDisp = this.NewEA_character.GetEmotionDisposition(ActiveEmotion.EmotionType); //Making new emotion
                    emoDisp.Threshold = 1;
                    var tick = this.NewAm_character.Tick;//time in simulation
                    this.NewEmotionalState_Character.Mood = 0;
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
                    Console.WriteLine("\n Strategy hasn't applied due to : Intensity threshold failed = " + !Threshold);
                    Console.WriteLine(" New possible intensity = " + Math.Abs(newIntensity) + " (User defined limit = " + this.ObjectiveIntensityEmotion + ")");
                }
            }
            else
            {
                Console.WriteLine("\n Strategy not applied due to :\n  Agent's personality can apply the strategy ====> " + AppliedStrategy);
            }
            Console.WriteLine("\n Response Modulation was applied: " + AppliedStrategy);
            Console.WriteLine("---------------------------------------------------------------");
            return AppliedStrategy;
        }


        #region Resources
        public InputDF AntecedentFocusedFrame(List<string> strategies, Name e)
        {
            bool StrategySuccess = false;
            string strategyName = string.Empty;
            List<(string Event, string Strategy)> Results = new();
            (string, string) result;

            foreach (var strategy in strategies)
            {
                strategyName = strategy;

                if (strategy == SituationSelectionString)
                { StrategySuccess = SituationSelection(e);  if(StrategySuccess) break; continue; }
                    
                if (strategy == SituationModificationString && !StrategySuccess) 
                { StrategySuccess = SituationModification(e); if (StrategySuccess) break; continue; }

                if (strategy == AttentionDeploymentString && !StrategySuccess) 
                { StrategySuccess = AttentionDeployment(e);   if (StrategySuccess) break; continue; }

                if (strategy == CognitiveChangeString       && !StrategySuccess) 
                { StrategySuccess = CognitiveChange(e);       if (StrategySuccess) break; continue; }
                
            }
            
            if (StrategySuccess) { result = (e.GetNTerm(3).ToString(), strategyName); }
            else { EventFatima = ReName(e).EventTypeFatima; result = (e.GetNTerm(3).ToString(), "None"); }///posible problema
            Results.Add(result);

            OutputData.Results = Results;
            OutputData.StrategySuccessful = StrategySuccess;
            return OutputData;
        }
        public InputDF ResponseFocusedFrame(List<string> strategies, Name e)
        {
            bool StrategySuccess = false;
            List<(string Event, string Strategy)> Results = new();
            (string, string) result;

            if (strategies.Contains(ResponseModulationString))
            {
                StrategySuccess = ResponseModulation(e);
            }
            if (StrategySuccess) { result = (e.GetNTerm(3).ToString(), ResponseModulationString); }
            else { result = (e.GetNTerm(3).ToString(), "None"); }
            Results.Add(result);
            OutputData.Results = Results;
            OutputData.StrategySuccessful = StrategySuccess;
            return OutputData;
        }
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
            EventAppraisal.ExistMatch = false;
            for (int j = 0; j < ea_character.GetAllAppraisalRules().ToList().Count; j++) //find a specific index eevent
            {
                var EventTemplate = ea_character.GetAllAppraisalRules().ElementAt(j).EventMatchingTemplate;
                //match the event 
                if (EventTemplate.GetNTerm(3).Equals(eventName))
                {
                    EventAppraisal.index = j;
                    EventAppraisal.ExistMatch = true;
                    break;
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
            if (EventAppraisal.ExistMatch)
            {
                Dictionary<string, (float value, Name target)> DAuxDataEvent = new();
                var SplitAppraisalVar = ea_character.GetAllAppraisalRules().ElementAt(EventAppraisal.index).AppraisalVariables;
                var ListAppVar = SplitAppraisalVar.appraisalVariables;
                EventAppraisal.IsEquals = false;

                if (ListAppVar.Count == 2 && ListAppVar.Where(a => a.Name == OCCAppraisalVariables.PRAISEWORTHINESS.ToString()).Any())
                {
                    var Values = ListAppVar.Select(v => v.Value);
                    EventAppraisal.IsEquals = float.Parse(Values.FirstOrDefault().ToString()) == -1 * float.Parse(Values.LastOrDefault().ToString());
                }

                foreach (var AppVar in ListAppVar)
                {
                    var NameVariable = AppVar.Name.ToString().Trim();
                    var ValueVariable = float.Parse(AppVar.Value.ToString());
                    var target = AppVar.Target;
                    DAuxDataEvent.Add(NameVariable, (ValueVariable, target));
                }
                EventAppraisal.DAppraisalVariblesData = DAuxDataEvent;
                float DesirabilityValue = 0f;
                if (EventAppraisal.DAppraisalVariblesData.Count > 1)
                    DesirabilityValue = EventAppraisal.DAppraisalVariblesData.Where(e => e.Key == OCCAppraisalVariables.DESIRABILITY).Select(r => r.Value.value).Sum();
                else
                {
                    DesirabilityValue = EventAppraisal.DAppraisalVariblesData.Values.FirstOrDefault().value;
                }
                if (DesirabilityValue < 0)
                    EventAppraisal.IsEventNegative = true;
            }
            return EventAppraisal;
        }

        private float LevelFunction(float valoration, float evaluation, int lim, int valance)
        {
            var lowLim = lim * 0.4;

            var IntensityValance = 1;
            if (valoration < 0)
                IntensityValance = -1;

            var tanh = (float)((Math.Abs((valoration / 2)) * (Math.Tanh(valance*(2 * evaluation - lim) / lowLim))) + (Math.Abs(valoration / 2)));
            var Intensity = IntensityValance * Math.Abs(tanh);
            return Intensity;
        }

        private float IntensityDerivation(EventAppraisalValues EventData)
        {
            bool IsMoreAppVar = EventData.DAppraisalVariblesData.Count > 1;
            bool IsdesirabilityForOther = EventData.DAppraisalVariblesData.Where(v => v.Key == OCCAppraisalVariables.DESIRABILITY_FOR_OTHER).Any();
            bool IsLike = EventData.DAppraisalVariblesData.Where(v => v.Key == OCCAppraisalVariables.LIKE).Any();
            bool IsGoal = EventData.DAppraisalVariblesData.Where(v => v.Key == OCCAppraisalVariables.GOALSUCCESSPROBABILITY).Any();
            string AppName = string.Empty;
            float AppValue = 0.0f;

            if (IsMoreAppVar) // for composed emotions
            {
                if (IsdesirabilityForOther) //OCCAppraisalVariables == DESIRABILITY and DESIRABILITY_FOR_OTHER
                {
                    var desirabilityForOther = EventData.DAppraisalVariblesData.Where(
                        e => e.Key == OCCAppraisalVariables.DESIRABILITY_FOR_OTHER).FirstOrDefault();
                    var desirability = EventData.DAppraisalVariblesData.Where(
                        e => e.Key == OCCAppraisalVariables.DESIRABILITY).FirstOrDefault();

                    AppValue = ((Math.Abs(desirabilityForOther.Value.value) + Math.Abs(desirability.Value.value)) * 0.5f);
                }
                else // OCCAppraisalVariables == DESIRABILITY and PRAISEWORTHINESS
                {
                    var AppValues = EventData.DAppraisalVariblesData.Select(e => e.Value.value);
                    AppValue = (Math.Abs(AppValues.Sum() * 0.5f));
                }
            }
            else
            {
                if (IsLike) // If OCCAppraisalVariables == LIKE
                {
                    var AppValues = EventData.DAppraisalVariblesData.Select(e => e.Value.value).FirstOrDefault();
                    const float magicFactor = -0.7f; ///funcionará igual si se utiliza el abs de appValue???
                    AppValue = AppValues * magicFactor; 
                }
                else if (IsGoal) 
                {
                    if (GoalSignificance < 0)
                        AppValue = 0;
                    else
                        AppValue = GoalSignificance;
                }
                else // If OCCAppraisalVariables == DESIRABILITY or PRAISEWORTHINESS
                {
                    var Value = EventData.DAppraisalVariblesData.Select(e => e.Value.value).FirstOrDefault(); // get the level of app. var. 
                    AppValue = Math.Abs(Value);
                }
            }
            return AppValue;
        }
    }
    #endregion
}
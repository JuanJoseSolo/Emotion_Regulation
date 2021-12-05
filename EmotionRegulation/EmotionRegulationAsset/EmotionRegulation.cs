using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WellFormedNames;
using EmotionalAppraisal;
using System.Collections;
using EmotionalDecisionMaking;
using ActionLibrary.DTOs;
using AutobiographicMemory;
using KnowledgeBase;
using ERconfiguration;

namespace EmotionRegulationAsset
{

    class EmotionRegulation
    {
        private dynamic iDActions;

        public Name EventFatima { get; set; }
        private bool EventAvoid { get; set; }
        public EmotionalAppraisalAsset NewEA_character { get; private set; }
        public EmotionalDecisionMakingAsset NewEdm_character { get; private set; }
        public AM NewAm_character { get; private set; }
        public List<Name> LalternativeEvents { get; private set; }

        public bool AppliedStrategy { get; private set; }

        private static string EventRelatedEvent { get; set; }
        private static Name EventWaction { get; set; }
        public Name eventWithActionID { get; set; }

        private dynamic IDActions { get => iDActions; set => iDActions = value; }
        private PersonalityTraits Personality { get; set; }
        public List<Name> ListEvents { get; private set; }
        public double ObjetiveEmotion { get; private set; }
        public Dictionary<string,string> Dic_RelatedActions { get; private set; }

        //public (string,string) RelatedEventsToFatima { get; private set; }

        public EmotionRegulation() { }

        public EmotionRegulation(
            /*List<Name> ListEvents_ER,*/
            EmotionalAppraisalAsset ea_character,
            EmotionalDecisionMakingAsset edm_Character,
            AM am_Character,
            PersonalityTraits personalityTraits,
            Dictionary<string, string> relatedActions,
            List<Name> AlternativeEvents,
            double objetiveEmotion)
        {
            this.NewEA_character = ea_character;
            this.NewEdm_character = edm_Character;
            this.NewAm_character = am_Character;
            this.Personality = personalityTraits;
            this.ObjetiveEmotion = objetiveEmotion;
            this.LalternativeEvents = AlternativeEvents;
            this.Dic_RelatedActions = relatedActions;            

            EventFatima = Name.NIL_SYMBOL;
            AppliedStrategy = false;
            EventAvoid = false;
        }

        private struct DataName
        {
            public Name EventTypeFatima { get; set; }
            public bool IsAvoided { get; set; }

        }
       
        private struct EventAppraisalValues
        {
            public string AppraisalType { get; set; }
            public float AppraisalValue { get; set; }
            public int index { get; set; }
        }
        //Situation Selection 
        public bool SituationSelection(Name events)
        {
            Console.WriteLine("\n---------------------Situation Selection-------------------------");
            AppliedStrategy = false;
            //this.Personality.DStrategyAndPower.Aggregate((strategy, power) => strategy.Key == "Situation Selection" ? strategy: power);
            var DSituationSelection = this.Personality.DStrategyAndPower.Where((strategy, power) => strategy.Key == "Situation Selection");
            var ExistStrategy = DSituationSelection.Any();
            var HighStrategyPower = DSituationSelection.Select(p => p.Value.Trim()=="Strongly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy && HighStrategyPower;

            var NewEvent = ReName(events);
            EventFatima = NewEvent.EventTypeFatima;

            Console.WriteLine("Event name: " + events.GetNTerm(3) +
                "                         Target: " + EventFatima.GetNTerm(4));
            Console.WriteLine("Can be avoided: " + NewEvent.IsAvoided);
           
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
            ///Console
            Console.WriteLine("\nSituation Selection could be applied: " + AppliedStrategy);
            return AppliedStrategy;
        }
        //Situation Modification 
        public bool SituationModification(Name events)
        {

            Console.WriteLine("\n--------------------Situation Modification-----------------------");
            AppliedStrategy = false;

            var DSituationSelection = this.Personality.DStrategyAndPower.Where((strategy, power) => strategy.Key == "Situation Modification");
            var ExistStrategy = DSituationSelection.Any();
            var HighStrategyPower = DSituationSelection.Select(p => p.Value.Trim() == "Strongly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy &&  HighStrategyPower;

            //RelatedEventsToFatima = (string.Empty, string.Empty);

            var newEvent = ReName(events);
            EventFatima = newEvent.EventTypeFatima;
            var target = EventFatima.GetNTerm(4).ToString();
            var eventName  = EventFatima.GetNTerm(3).ToString();
            var existRelatedActions = Dic_RelatedActions.Where(e => e.Value == eventName).Any();


            var ModifiedValue = float.NaN;

            //finds the appraisal value of event
            var Event = EventMatchingName(this.NewEA_character, (Name)eventName);

            Console.WriteLine("\nEvent name: " + eventName +
                "                          Target: "+ target);
            Console.WriteLine("\nCould do any actions ? : " + existRelatedActions);

            //Conditions
            if (ApplyStrategy && existRelatedActions && Event.AppraisalValue <= -5)
            {
                var DRelatedActions = Dic_RelatedActions.Where(e => e.Value == eventName);
                var LRelatedActions = DRelatedActions.Select(a => a.Key).ToList();
                List<ActionLibrary.DTOs.ActionRuleDTO> ActionsEDM = new();
               
                foreach(var act in LRelatedActions)
                {
                    ActionsEDM.Add( this.NewEdm_character.GetAllActionRules().Where(
                    a => a.Action.ToString() == act).FirstOrDefault());
                }
                
                var RelatedActions = this.NewEdm_character.GetAllActionRules().Where(
                    a => a.Action.ToString() == DRelatedActions.Select(ea => ea.Key).FirstOrDefault());
                
                Console.WriteLine(" \n In progress...  ");
                Console.WriteLine(" Evaluating actions value...  ");

                foreach (var AedmCharacter in ActionsEDM) 
                {
                    var avg = ((this.Personality.Neuroticism + this.Personality.Agreeableness) / 2);
                    var tanh = (Math.Abs((Event.AppraisalValue / 2)) * (Math.Tanh(-(2 * avg - 100) / 50))) - (Math.Abs(Event.AppraisalValue / 2));
                    ModifiedValue = (float)tanh;

                    var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(Event.index).EventMatchingTemplate;
                    
                    if (ModifiedValue >= this.ObjetiveEmotion)
                    {
                        var actionTo = AedmCharacter.Action.ToString();
                        Console.WriteLine(" Would decide: " + actionTo);
                        UpdateEmotionalAppraisal(this.NewEA_character, Event.AppraisalType, ModifiedValue, EventTemplate, Event.index);
                        AppliedStrategy = true;
                        //RelatedEventsToFatima = (eventName,actionTo);
                        //break;
                    }
                    else { Console.WriteLine("\n Strategy not applied due to : Emotion limit was achieved ===> " + (ModifiedValue >= this.ObjetiveEmotion)); }
                }
            }
            else { Console.WriteLine("\n Strategy not applied due to :\n Agent's personality can apply the strategy ===> " + ApplyStrategy +
                ", or : Exist Related Actions is ===> " + existRelatedActions+ ", or :\n " +
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
            var HighStrategyPower = DAttentionDeployment.Select(p => p.Value.Trim() == "Strongly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy && HighStrategyPower;

            var target = events.GetNTerm(4);
            var eventName = events.GetNTerm(3);

            var NewEvent = ReName(events);
            EventFatima = NewEvent.EventTypeFatima;
            
            Console.WriteLine("\nEvent name: " + eventName +
                "                           Target: " + this.EventFatima.GetNTerm(4));

            //Only when the current event is negative
            var CurrentEventValue = EventMatchingName(this.NewEA_character,events);
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
                
                //var average = ListEventsValue.Sum() / ListEventsValue.Count;
                var average = ListEventsValue.Average();

                var EventToER = EventMatchingName(this.NewEA_character, eventName);
                var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(
                    EventToER.index).EventMatchingTemplate;

                var tanh = (Math.Abs((EventToER.AppraisalValue / 2)) * (Math.Tanh((2 * average - 10) / 5))) - (Math.Abs(EventToER.AppraisalValue / 2));
                
                var ValueModified = (float)tanh;
                if (ValueModified >= this.ObjetiveEmotion)
                {
                    UpdateEmotionalAppraisal(this.NewEA_character, EventToER.AppraisalType, ValueModified, EventTemplate, EventToER.index);
                    AppliedStrategy = true;
                }
                else { Console.WriteLine("\n Strategy not applied due to : Emotion limit was achieved ===> " + (ValueModified >= this.ObjetiveEmotion)); }
            }
            else { Console.WriteLine("\n Strategy not applied due to :\n Agent's personality can apply the strategy ===> " + ApplyStrategy +
                                            ", or Exist Event Related is ===> " + ExistEventRelated); 
            }
            
            Console.WriteLine("\n Attention Deployment could be applied : " + AppliedStrategy);
            return AppliedStrategy;
        }

        // Cognitive Change
        public bool CognitiveChange(Name events, ConcreteEmotionalState emotionalStateCharacter)
        {

            Console.WriteLine("\n---------------------Cognitive Change------------------------");
            List<float> LAlternativeEventsValue = new();

            AppliedStrategy = false;
            //check personality
            var DAttentionDeployment = this.Personality.DStrategyAndPower.Where(
                (strategy, power) => strategy.Key == "Cognitive Change");
            var ExistStrategy = DAttentionDeployment.Any();
            var HighStrategyPower = DAttentionDeployment.Select(p => p.Value.Trim() == "Strongly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy && HighStrategyPower;
            //get event name and it construct the event
            var eventName = events.GetNTerm(3);
            var NewEvent = ReName(events);
            EventFatima = NewEvent.EventTypeFatima;

            //Bucar eventos para reinterpretar
            var AlternativeEventsName = this.LalternativeEvents.Where(
                e => e.GetNTerm(4)== EventFatima.GetNTerm(3));

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
                foreach (var Ae in AlternativeEventsName)
                {
                    var AlternativeEvents = this.NewEA_character.GetAllAppraisalRules().Select(
                    e => e.EventMatchingTemplate).Where(eM => eM.GetNTerm(3) == Ae.GetNTerm(3)).FirstOrDefault().GetNTerm(3);
                    var EventValues = EventMatchingName(this.NewEA_character, Ae.GetNTerm(3));
                    Console.WriteLine("\n '"+ Ae.GetNTerm(3)+"'");
                    LAlternativeEventsValue.Add(EventValues.AppraisalValue);
                }

                var averange = LAlternativeEventsValue.Average();
                var mood = emotionalStateCharacter.Mood;

                var testValor1 = (averange + (this.Personality.Openness * 0.1))/2;
                var ValueTest = (mood  + (-this.Personality.Neuroticism * 0.1))/2;

                var EventToER = EventMatchingName(this.NewEA_character,eventName);
                var tanh = (Math.Abs((EventToER.AppraisalValue / 2)) * (Math.Tanh((2 * testValor1 - 10) / 5))) - (Math.Abs(EventToER.AppraisalValue / 2));
                var ModifiedValue = (float)tanh + (float)ValueTest;

                var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(
                                    EventToER.index).EventMatchingTemplate;

                if (ModifiedValue >= this.ObjetiveEmotion)
                {
                    UpdateEmotionalAppraisal(this.NewEA_character, EventToER.AppraisalType, ModifiedValue, EventTemplate, EventToER.index);
                    AppliedStrategy = true;
                }
                else { Console.WriteLine("\n Strategy not applied due to : Emotion limit was achieved ===> " + (ModifiedValue >= this.ObjetiveEmotion)); }
            }
            else
            {
                Console.WriteLine("\n Strategy not applied due to :\n Agent's personality can apply the strategy ===> " + ApplyStrategy +
                                        ", or Exist Alternative Event is ===> " + existAlternativeEvents);
            }

            Console.WriteLine("\n Attention Deployment could be applied : " + AppliedStrategy);
            return AppliedStrategy;
        }

        //Response Modulation
        public bool ResponseModulation()
        {
            return false;
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
        
        private DataName ReName(Name events)
        {
            DataName dataName = new();

            if (events.NumberOfTerms > 5)
            {

                
                var EReventsVariables = events.GetTerms();
                var EventValues = string.Join(
                    "", EReventsVariables.Last().ToString().Split('[', ']')).Split("-");

                dataName.IsAvoided = bool.Parse(EventValues[0]);

                EventRelatedEvent = EventValues[1].FirstOrDefault().ToString();
                

                //dataName.IsAvoided = bool.Parse(events.GetNTerm(5).ToString());

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

        private EventAppraisalValues EventMatchingName(EmotionalAppraisalAsset ea_character, Name eventName)
        {
            EventAppraisalValues EventAppraisal = new();
            for (int j = 0; j < ea_character.GetAllAppraisalRules().ToList().Count; j++) //find a specific event
            {
                var EventTemplate = ea_character.GetAllAppraisalRules().ElementAt(j).EventMatchingTemplate;
                if ((EventTemplate.GetNTerm(3).Equals(eventName)))
                {
                    EventAppraisal.index = j;                                      
                }
            }
            
            //Apraisal variable and value
            var SplitAppraisalVar = ea_character.GetAllAppraisalRules().ElementAt(EventAppraisal.index).AppraisalVariables;
            var AppraisalVariable = SplitAppraisalVar.ToString().Split("=");
            EventAppraisal.AppraisalType = AppraisalVariable[0].Trim();
            EventAppraisal.AppraisalValue = float.Parse(AppraisalVariable[1].Trim());
            return EventAppraisal;
        }

    }
}
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
        public bool AppliedStrategy { get; private set; }

        private static string IdeAction { get; set; }
        private static Name EventWaction { get; set; }
        public Name eventWithActionID { get; set; }

        private dynamic IDActions { get => iDActions; set => iDActions = value; }
        private PersonalityTraits Personality { get; set; }
        public List<Name> ListEvents { get; private set; }
        public double ObjetiveEmotion { get; private set; }
        public Dictionary<string,string> Dic_RelatedActions { get; private set; }

        public (string,string) RelatedEventsToFatima { get; private set; }

        public EmotionRegulation(
            List<Name> ListEvents_ER, 
            EmotionalAppraisalAsset ea_character,
            PersonalityTraits personalityTraits,
            Dictionary<string,string> relatedActions, 
            double objetiveEmotion)
        {
            this.NewEA_character = ea_character;
            this.Personality = personalityTraits;
            this.ListEvents = ListEvents_ER;
            this.ObjetiveEmotion = objetiveEmotion;
            this.Dic_RelatedActions = relatedActions;
            this.NewEdm_character = new();
            

            EventFatima = Name.NIL_SYMBOL;
            AppliedStrategy = false;
            EventAvoid = false;
            IDActions = String.Empty;

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
            else { Console.WriteLine("\n Strategy not applied due to :\n Apply Strategy is ===> " 
                + ApplyStrategy + " or : Event Is Avoided is ===> " + NewEvent.IsAvoided); }
            ///Console
            Console.WriteLine("\nSituation Selection could be applied: " + AppliedStrategy);
            return AppliedStrategy;
        }
        //Situation Modification 
        public bool SituationModification(EmotionalDecisionMakingAsset edm_Character, Name events, float EmotionLimit)
        {

            Console.WriteLine("\n--------------------Situation Modification-----------------------"); 

            var DSituationSelection = this.Personality.DStrategyAndPower.Where((strategy, power) => strategy.Key == "Situation Modification");
            var ExistStrategy = DSituationSelection.Any();
            var HighStrategyPower = DSituationSelection.Select(p => p.Value.Trim() == "Strongly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy &&  HighStrategyPower;

            AppliedStrategy = false;
            RelatedEventsToFatima = (string.Empty, string.Empty);
            this.NewEdm_character = edm_Character;

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
            ////Situation Modification 
            //Conditions
            if (ApplyStrategy && existRelatedActions && Event.AppraisalValue <= -5)
            {
                var DRelatedActions = Dic_RelatedActions.Where(e => e.Value == eventName);
                var RelatedActions = this.NewEdm_character.GetAllActionRules().Where(a => a.Action.ToString() == DRelatedActions.Select(ea => ea.Key).FirstOrDefault());
                
                Console.WriteLine(" \n In progress...  ");
                Console.WriteLine(" Evaluating actions value...  ");

                foreach (var AedmCharacter in RelatedActions) 
                {
                    var avg = ((Personality.Neuroticism + Personality.Agreeableness) / 2);
                    var tanh = (Math.Abs((Event.AppraisalValue / 2)) * (Math.Tanh(-(2 * avg - 100) / 50))) - (Math.Abs(Event.AppraisalValue / 2));
                    ModifiedValue = (float)tanh;
                    /*
                    Console.WriteLine(" Average antagonist personality: " + avg);
                    Console.WriteLine(" Modified Value" + ModifiedValue);
                    Console.WriteLine(" Normal Value" + Event.AppraisalValue);
                    */
                    var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(Event.index).EventMatchingTemplate;
                    
                    if (ModifiedValue >= EmotionLimit)
                    {
                        var actionTo = AedmCharacter.Action.ToString();
                        Console.WriteLine(" Would decide: " + actionTo);
                        UpdateEmotionalAppraisal(this.NewEA_character, Event.AppraisalType, ModifiedValue, EventTemplate, Event.index);
                        AppliedStrategy = true;
                        RelatedEventsToFatima = (eventName,actionTo);
                        
                        break;
                    }
                    else { Console.WriteLine("\n Strategy not applied due to : Emotion limit was achieved ===> " + (ModifiedValue >= EmotionLimit)); }
                }
            }
            else { Console.WriteLine("\n Strategy not applied due to :\n Apply Strategy is ===> " + ApplyStrategy +
                ", or : Exist Related Actions is ===> " + existRelatedActions+ ", or :\n " +
                " Defined Event Appraisal Value is greater than -5 " + (Event.AppraisalValue < -5) + " is " + Event.AppraisalValue); }
            Console.WriteLine("\nSituation Modification could be applied: " + AppliedStrategy);
            return AppliedStrategy;
        }
        //Attention Deployment 
        public bool AttentionDeployment(Name Event, AM am_Character, float EmotionLimit)
        {
            Console.WriteLine("\n---------------------Attention Deployment------------------------");

            AppliedStrategy = false;
            var DAttentionDeployment = this.Personality.DStrategyAndPower.Where((strategy, power) => strategy.Key == "Attention Deployment");
            var ExistStrategy = DAttentionDeployment.Any();
            var HighStrategyPower = DAttentionDeployment.Select(p => p.Value.Trim() == "Strongly").FirstOrDefault();
            var ApplyStrategy = ExistStrategy && HighStrategyPower;

            var target = Event.GetNTerm(4);
            var eventName = Event.GetNTerm(3);

            var NewEvent = ReName(Event);
            EventFatima = NewEvent.EventTypeFatima;
            
            Console.WriteLine("\nEvent name: " + eventName +
                "                           Target: " + this.EventFatima.GetNTerm(4));
            /*
            //Eventos ocurridos 
            foreach (var recalledEvent in am_Character.RecallAllEvents())
            {
                Console.WriteLine("\nEventos en la memoria : " + recalledEvent.EventName);
                Console.WriteLine("Emoción registrada : " + recalledEvent.LinkedEmotions.Single());
                Console.WriteLine("Id : " + recalledEvent.Id);
            }*/

            var EventNameAM = am_Character.RecallAllEvents().Select(ep => ep.EventName);
            var RelatedEvent = EventNameAM.Where(e => e.GetNTerm(4) == target).Select(a => a.GetNTerm(3));

            var ExistEventRelated = RelatedEvent.Any();
            Console.WriteLine("Does Exist any event related ? : " + ExistEventRelated);

            foreach (var e in RelatedEvent)
                Console.WriteLine("Related events : " + e);
            //Conditions
            if (ApplyStrategy && ExistEventRelated)
            {
                List<float> ListEventsValue = new();
                Console.WriteLine(" \n In progress...  ");
                Console.WriteLine(" Evaluating past events...  ");

                foreach (var n in RelatedEvent)
                {   
                    //Console.WriteLine("\n Related event = " + n);
                    var EventValues = EventMatchingName(NewEA_character, n);
                    ListEventsValue.Add(EventValues.AppraisalValue);
                    //Console.WriteLine(" Event Value: " + EventValues.AppraisalValue);
                 }
                
                var average = ListEventsValue.Sum() / ListEventsValue.Count;
                //Console.WriteLine("\n average of the values " + average);

                var EventToER = EventMatchingName(this.NewEA_character, eventName);
                var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(EventToER.index).EventMatchingTemplate;

                var tanh = (Math.Abs((EventToER.AppraisalValue / 2)) * (Math.Tanh((2 * average - 10) / 5))) - (Math.Abs(EventToER.AppraisalValue / 2));
                
                //Console.WriteLine("\n Function Output = " + tanh);
                //Console.WriteLine("Defined emotion limit = " + EmotionLimit);
                
                var ValueModified = (float)tanh;
                if (ValueModified >= EmotionLimit)
                {
                    UpdateEmotionalAppraisal(this.NewEA_character, EventToER.AppraisalType, ValueModified, EventTemplate, EventToER.index);
                    AppliedStrategy = true;
                }
                else { Console.WriteLine("\n Strategy not applied due to : Emotion limit was achieved ===> " + (ValueModified >= EmotionLimit)); }
            }
            else { Console.WriteLine("\n Strategy not applied due to :\n Apply Strategy is ===> " + ApplyStrategy +
                                            ", or Exist Event Related is ===> " + ExistEventRelated); }
            
            Console.WriteLine("\n Attention Deployment could be applied : " + AppliedStrategy);
            return AppliedStrategy;
        }
        // Cognitive Change
        public bool CognitiveChange()
        {
            return false;
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
                if(events.GetNTerm(5).ToString().Length > 10) { IDActions = events.GetTerms().FirstOrDefault(n => n.ToString().Length == 36); }
                if(events.GetNTerm(5).ToString().Length < 6) { dataName.IsAvoided = bool.Parse(events.GetNTerm(5).ToString());}
                    
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

        private EventAppraisalValues EventMatchingName(EmotionalAppraisalAsset ea_character,Name eventName)
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

        private void EventAction(EmotionalDecisionMakingAsset edm_Character, Name E)
        {

            Console.WriteLine("\n------------------------void EventAction()------------------------\n");
            foreach(var Eve in this.NewEA_character.GetAllAppraisalRules()) 
            {

            }
            
            /*
            foreach(var RE in this.RelatedEvents)
                edm_Character.GetAllActionRules().Where(a => a.)
            */

            var idEvent = this.NewEA_character.GetAllAppraisalRules().Where(
                n => n.EventMatchingTemplate.GetNTerm(3).ToString().Equals("Bye")).FirstOrDefault().Id.ToString();

            //IdeAction = edm_Character.GetAllActionRules().Where(n => n.Action.ToString().Equals("ToHug")).FirstOrDefault());//.Id.ToString();

            var IDitem = E.GetLiterals().ToList();
            IDitem.Add(Name.BuildName(IdeAction));
            eventWithActionID = Name.BuildName(IDitem);
            return;

        }
    }
}
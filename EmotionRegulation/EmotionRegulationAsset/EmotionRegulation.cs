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
        public bool AppliedStategy { get; set; }

        private static string IdeAction { get; set; }
        private static Name EventWaction { get; set; }
        public Name eventWithActionID { get; set; }


        private dynamic IDActions { get => iDActions; set => iDActions = value; }
        private PersonalityTraits Trait { get; set; }
        public List<Name> ListEvents { get; private set; }
        public double ObjetiveEmotion { get; private set; }
        public Dictionary<string,string> RelatedEvents { get; private set; }
        //public Tuple<string, bool> RelatedEventsToFatima { get; private set; }
        public (string,string) RelatedEventsToFatima { get; private set; }
        public EmotionRegulation(
            List<Name> ListEvents_ER, EmotionalAppraisalAsset ea_character,
            PersonalityTraits trait, Dictionary<string,string> RelatedEvents, double objetiveEmotion)
        {
            this.NewEA_character = ea_character;
            this.Trait = trait;
            this.ListEvents = ListEvents_ER;
            this.ObjetiveEmotion = objetiveEmotion;
            this.RelatedEvents = RelatedEvents;
            this.NewEdm_character = new();
            

            EventFatima = Name.NIL_SYMBOL;
            AppliedStategy = false;
            EventAvoid = false;
            IDActions = String.Empty;

        }

        private struct DataName
        {
            public Name EventTypeFatima { get; set; }
            public bool EventIsAvoid { get; set; }

        }
        private struct EventAppraisalValues
        {
            public string AppraisalType { get; set; }
            public float AppraisalValue { get; set; }
            public int index { get; set; }
        }

        public void CheckEvents()
        {
            List<Name> NewEvenstList = new();
            var ERevents = this.ListEvents;
            /*
            var EventsToEvaluate = ERevents.Where(e => e.ToString().EndsWith("True)") || e.ToString().EndsWith("False)"));
            var index = 0;
            List<int> Indexs = new();
            foreach (var ev in ERevents)
            {
                if (ev.ToString().EndsWith("True)") || ev.ToString().EndsWith("False)"))
                    Indexs.Add(index); 
                index += 1;
            }
            index = 0;
            */
                foreach (var d in ERevents)
                {
                Console.WriteLine(d);
                SituationSelection(d); NewEvenstList.Add(EventFatima);

                
                //foreach (var e in EventsToEvaluate) { SituationSelection(e); NewEvenstList.Add(EventFatima); }
                }
            

            //foreach (var j in Indexs) { ERevents.RemoveAt(j);ERevents.Insert(j, NewEvenstList.ElementAt(index)); index += 1; }
            //this.ListEvents = ERevents;
        }

        public bool SituationSelection(Name events)
        {

            Console.WriteLine("\n---------------------SituationSelection-------------------------");
            AppliedStategy = false;
            var NewEvent = ReName(events);
            EventFatima = NewEvent.EventTypeFatima;
            Console.WriteLine("Event name: " + events.GetNTerm(3));
            Console.WriteLine("Defined value event: " + NewEvent.EventIsAvoid);
            Console.WriteLine("Personality type can apply: " + this.Trait.NameStrategy + "\n");
            

            if (this.Trait.NameStrategy == "Situation Selection" && NewEvent.EventIsAvoid)
            {
                var eventName = NewEvent.EventTypeFatima.GetNTerm(3);
                var Event = EventMatchingName(this.NewEA_character, eventName);
                var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(Event.index).EventMatchingTemplate;
                var ListEvent = NewEvent.EventTypeFatima.GetLiterals().ToList();

                //Build new eventTemplate with a word NOT
                var EmotionMatchTemplate_list = EventTemplate.GetTerms().ToList();
                ListEvent.RemoveAt(3);  //Event(Action-End, subject, *event*, target)
                EmotionMatchTemplate_list.RemoveAt(3);   //Event(Action-End, *, *event*, *)
                var NewEventName = Name.BuildName("Not-" + eventName);
                EmotionMatchTemplate_list.Insert(3, NewEventName);
                ListEvent.Insert(3, NewEventName);
                var NewEventMatchingTemplate = Name.BuildName(EmotionMatchTemplate_list);
                EventFatima = Name.BuildName(ListEvent);

                UpdateEmotionalAppraisal(this.NewEA_character, Event.AppraisalType, 0, NewEventMatchingTemplate, Event.index);
                AppliedStategy = true;
            }
            ///Console
            Console.WriteLine("\nSituation Selection was applied: " + AppliedStategy);
            return AppliedStategy;
        }

        public bool SituationModification(EmotionalDecisionMakingAsset edm_Character, Name events, float EmotionLimit)
        {
            RelatedEventsToFatima = (string.Empty, string.Empty);
            this.NewEdm_character = edm_Character;
            var newEvent = ReName(events);
            EventFatima = newEvent.EventTypeFatima;
            AppliedStategy = false;
            Console.WriteLine("\n--------------------SituationModification-----------------------");
            /*
            //shows if exists some definied actions for ER 
            if (this.RelatedEvents.Count != 0)
                foreach (var er in this.RelatedEvents) {
                    Console.WriteLine(" Event: " + er.Value + " ---> Action: " + er.Key); }
            else Console.WriteLine("False");
            */
            //finds the appraisal value of event
            var eventName = events.GetLiterals().ElementAt(3).ToString();
            var Event     = EventMatchingName(this.NewEA_character, (Name)eventName);

            Console.WriteLine("\nEvent name: " + eventName);
            Console.WriteLine("Personality type can apply: " + this.Trait.NameStrategy + "\n");
            //obtains a couple of event/action with current event
            var NameEventsRelated = this.RelatedEvents.Where(k => k.Value.Equals(eventName));
            Console.WriteLine("\nAre there related events ? : " + NameEventsRelated.FirstOrDefault().Key);
            
            //Console.WriteLine(this.Trait.NameStrategy +" "+  NameEventsRelated.ToList().Count + " " + Event.AppraisalValue);
            if (this.Trait.NameStrategy == "Situation Modification" && NameEventsRelated.ToList().Count != 0 && Event.AppraisalValue < -1)
            {
                //finds actions related with the specific event
                var RelatedActions = this.NewEdm_character.GetAllActionRules().Where(a => a.Action.ToString().Equals(NameEventsRelated.FirstOrDefault().Key));
                
                foreach (var AedmCharacter in RelatedActions) 
                {
                    Console.WriteLine(" \n Trying to apply strategy  ");
                    float prom = ((Trait.Neuroticism + Trait.Agreeableness) / 2);
                    var tanh = (Math.Abs((Event.AppraisalValue / 2)) * (Math.Tanh(-(2 * prom - 100) / 50))) - (Math.Abs(Event.AppraisalValue / 2));
                    var ValueModified = (float)tanh;
                    var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(Event.index).EventMatchingTemplate;
                    
                    if (ValueModified >= EmotionLimit)
                    {
                        var actionTo = AedmCharacter.Action.ToString();
                        Console.WriteLine(" Decision: " + actionTo);
                        UpdateEmotionalAppraisal(this.NewEA_character, Event.AppraisalType, ValueModified, EventTemplate, Event.index);
                        AppliedStategy = true;
                        RelatedEventsToFatima = (eventName,actionTo);
                        
                        break;
                    }
                    else { Console.WriteLine(" \n Failed due to the defined threshold"); }
                }
            }
            Console.WriteLine("\nSituation Modification was applied: " + AppliedStategy);
            return AppliedStategy;
        }

        public bool AttentionDeployment(Name Event, AM am_Character, float EmotionLimit)
        {
            Console.WriteLine("\n---------------------AttentionDeployment------------------------");

            var target = Event.GetNTerm(4);
            var NewEvent = ReName(Event);
            EventFatima = NewEvent.EventTypeFatima;
            Console.WriteLine("Personality type can apply: " + this.Trait.NameStrategy + "\n");

            if (this.Trait.NameStrategy == "Attentional Deployment")
            {

                //Eventos ocurridos 
                foreach (var recalledEvent in am_Character.RecallAllEvents())
                {
                    Console.WriteLine("\nEventos en la memoria : " + recalledEvent.EventName);
                    Console.WriteLine("Emoción registrada : " + recalledEvent.LinkedEmotions.Single());
                    Console.WriteLine("Id : " + recalledEvent.Id);
                }

                List<float> valuesEvents = new();
                var NameEvents = from E in am_Character.RecallAllEvents() select E.EventName;
                var EventRelated = NameEvents.Where(e => e.GetNTerm(4) == target).Select(a => a.GetNTerm(3));

                foreach (var n in EventRelated)
                {
                    Console.WriteLine("\n Evento relacionado con el target = " + n);
                    var EventValues = EventMatchingName(NewEA_character, n);
                    valuesEvents.Add(EventValues.AppraisalValue);
                    Console.WriteLine("Valor de los eventos relacionados: " + EventValues.AppraisalValue);
                }

                var calculado = valuesEvents.Sum() / valuesEvents.Count;
                Console.WriteLine("\n Valoración promedio de los eventos positivos " + calculado);

                var eventName = EventFatima.GetLiterals().ElementAt(3);
                var EventToER = EventMatchingName(this.NewEA_character, eventName);
                var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(EventToER.index).EventMatchingTemplate;

                var tanh = (Math.Abs((EventToER.AppraisalValue / 2)) * (Math.Tanh((2 * calculado - 10) / 5))) - (Math.Abs(EventToER.AppraisalValue / 2));

                Console.WriteLine("\nvalor de salida de la función Tanh = " + tanh);
                Console.WriteLine("valor máximo negativo del usuario  = " + EmotionLimit);

                var ValueModified = (float)tanh;
                if (ValueModified >= EmotionLimit)
                {
                    UpdateEmotionalAppraisal(this.NewEA_character, EventToER.AppraisalType, ValueModified, EventTemplate, EventToER.index);
                    AppliedStategy = true;
                }
            }
            Console.WriteLine("\nAttentional Deployment was applied: " + AppliedStategy);
            return AppliedStategy;
        }



















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
                if(events.GetNTerm(5).ToString().Length < 6) { dataName.EventIsAvoid = bool.Parse(events.GetNTerm(5).ToString());}
                    
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
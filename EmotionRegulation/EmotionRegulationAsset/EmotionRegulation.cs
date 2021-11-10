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

namespace EmotionRegulationAsset
{
    /// <summary>
    /// Class Emotion regulation Asset
    /// </summary>
    class EmotionRegulation
    {
        private dynamic iDActions;
        
        public Name EventFatima { get; set; }
        private bool EventAvoid { get; set; }
        public EmotionalAppraisalAsset NewEA_character { get; set; }
        public bool AppliedStategy { get; set; }
       

        private dynamic IDActions { get => iDActions; set => iDActions = value; }
        private PersonalityTraits Trait { get; set; }

        public EmotionRegulation(EmotionalAppraisalAsset ea_character, PersonalityTraits trait)
        {
            this.NewEA_character = ea_character;
            this.Trait = trait;
            EventFatima = Name.NIL_SYMBOL;
            AppliedStategy = false;
            EventAvoid = false;
            IDActions = String.Empty;

        }
        /// <summary>
        /// Estructura de datos para almacenar diferentes tipos de variables
        /// </summary>
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

        DataName dataName = new();
        EventAppraisalValues EventAppraisal = new();

        /// <summary>
        /// Strategy situation selection
        /// </summary>
        /// <param name="events"></param>
        /// <param name="ea_character"></param>
        /// <param name="trait"></param>
        /// <returns>True value if the strategy is applied, other wise, returns false.</returns>
        public bool SituationSelection(Name events)
        {
            Console.WriteLine("\n---------------------public bool SituationSelection()-------------------------");
  
            var NewEvent = ReName(events);
            EventFatima = NewEvent.EventTypeFatima;
            Console.WriteLine("\nEvent Value: " + NewEvent.EventIsAvoid);
            Console.WriteLine("Tried Apply Estrategy------> " + this.Trait.NameStrategy + "\n");
            

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

        public bool SituationModification(EmotionalDecisionMakingAsset edm, float EmotionLimit)
        {
            Console.WriteLine("\n---------------------public bool SituationModification()-------------------------");
            
            Program ActionIDtoEvent = new();//obtener los Id's de las acciones y eventos de main()
            var ActionID = ActionIDtoEvent.eventWithActionID.GetTerms().Where(n => n.ToString().Length == 36).FirstOrDefault();
            //find the event related to the id action
            var ExistsActionRelated = edm.GetAllActionRules().Select(r => r.Id.ToString().Equals(ActionID.ToString())).FirstOrDefault();

            var newEvent = ReName(ActionIDtoEvent.eventWithActionID);
            EventFatima = newEvent.EventTypeFatima;
            Console.WriteLine("\nAction related: " + EventFatima);
            Console.WriteLine("\nTried Apply Estrategy------> " + this.Trait.NameStrategy + "\n");
            var eventName = EventFatima.GetLiterals().ElementAt(3);
            var Event = EventMatchingName(this.NewEA_character, eventName);

            if (this.Trait.NameStrategy == "Situation Modification" && ExistsActionRelated && Event.AppraisalValue < -3)
            {
                var action = edm.GetAllActionRules();
                Console.WriteLine("\n Decisions: ");
                Console.WriteLine("\n Action is: " + action.FirstOrDefault().Action.ToString());

                var EventTemplate = this.NewEA_character.GetAllAppraisalRules().ElementAt(Event.index).EventMatchingTemplate;

                float prom = ((Trait.Neuroticism + Trait.Agreeableness) / 2);
      
                var sig = (Event.AppraisalValue) / (1 + Math.Exp(-(prom / Math.Abs(Event.AppraisalValue)) + (Math.Abs(Event.AppraisalValue))));
                var tanh = (Math.Abs((Event.AppraisalValue/2)) * (Math.Tanh(-(2*prom-100)/50))) - (Math.Abs(Event.AppraisalValue / 2));
                
                Console.WriteLine("Valor caluculado Tanh: " + tanh);
                Console.WriteLine("Valor caluculado Sigmoide: " + sig);

                var ValueModified = (float)tanh;
                if (ValueModified >= EmotionLimit)
                {
                   UpdateEmotionalAppraisal(this.NewEA_character, Event.AppraisalType, ValueModified, EventTemplate, Event.index);
                   AppliedStategy = true;
                }
            }
            /// Console
            Console.WriteLine("\nSituation Modification was applied: " + AppliedStategy);
            return AppliedStategy;
        }

        public bool AttentionDeployment(Name Event, AM am_Character)
        {
            Console.WriteLine("\n---------------------public bool AttentionDeployment()-------------------------");


            var NewEvent = ReName(Event);
            EventFatima = NewEvent.EventTypeFatima;
            Console.WriteLine("\nTried Apply Estrategy------> " + this.Trait.NameStrategy + "\n");

            foreach (var recalledEvent in am_Character.RecallAllEvents())
            {
                Console.WriteLine("\nEventos en la memoria : " + recalledEvent.EventName);
                Console.WriteLine("Emoción registrada : " + recalledEvent.LinkedEmotions.Single());
                Console.WriteLine("Id : " + recalledEvent.Id);

            }


            var EventoNuevo = from E in am_Character.RecallAllEvents() select E.EventName;

            var id = EventoNuevo.Where(e => e.GetNTerm(3) == (Name)"FallDown").FirstOrDefault();

            Console.WriteLine("\n\n Evento positivo = " + id);

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
        /*
        private AppraisalValues GetValuesAppraisal(EmotionalAppraisalAsset character, int index)
        {
           
            var SplitAppraisalVar = character.GetAllAppraisalRules().ElementAt(index).AppraisalVariables;
            var AppraisalVariable = SplitAppraisalVar.ToString().Split("=");
            appraisal.Type  = AppraisalVariable[0].Trim();
            appraisal.Value = float.Parse(AppraisalVariable[1].Trim());

            return appraisal;
        }
        */
        /// <summary>
        /// Rebuilt events name for sent to FAtiMA core.
        /// </summary>
        /// <param name="events"></param>
        /// <returns>New rebuilt name event: Event(Action-End, subject, event, target)</returns>
        private DataName ReName(Name events)
        {
            if (events.NumberOfTerms > 5)
            {
                dataName.EventIsAvoid = bool.Parse(events.GetNTerm(5).ToString());
                IDActions = events.GetTerms().FirstOrDefault(n => n.ToString().Length == 36);
                var ListEvent = events.GetLiterals().ToList();
                for (int j = 5; j <= ListEvent.Count; j++)
                {
                    ListEvent.RemoveAt(5);

                }
                dataName.EventTypeFatima = Name.BuildName(ListEvent);
            }
            return dataName;
        }

        /// <summary>
        /// Find the match event in the emotional appraisal character
        /// </summary>
        /// <param name="ea_character"></param>
        /// <param name="eventName">Name of the event to find in emotional appraisal character</param>
        /// <returns>The index by the event name</returns>
        private EventAppraisalValues EventMatchingName(EmotionalAppraisalAsset ea_character,Name eventName)
        {   
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
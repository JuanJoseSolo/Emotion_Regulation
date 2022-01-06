using System;
using System.Collections.Generic;
using System.Linq;
using FLS;

namespace EmotionRegulationAsset
{
    public class PersonalityTraits
    {
        public const string OPENNESS            = "Opennes";
        public const string CONSCIENTIOUSNESS   = "Conscientiousness";
        public const string EXTRAVERSION        = "Extraversion";
        public const string AGREEABLENESS       = "Agreeableness";
        public const string NEUROTICISM         = "Neuroticism";

        internal double Conscientiousness;
        internal double Extraversion;
        internal double Neuroticism;
        internal double Openness;
        internal double Agreeableness;
        internal List<double> BigFiveValues;

        public List<string> Personalities { get; protected set; } 
        public string DominantPersonality { get; protected set; }
        public List<KeyValuePair<string,string>> StrategiesToApply { get; protected set; }


        private LinguisticVariable Linguistic_Openness;
        private LinguisticVariable Linguistic_Conscientiousness;
        private LinguisticVariable Linguistic_Extraversion;
        private LinguisticVariable Linguistic_Neuroticism;
        private LinguisticVariable Linguistic_Agreeableness;

        private FLS.MembershipFunctions.IMembershipFunction low_Openness;
        private FLS.MembershipFunctions.IMembershipFunction middle_Openness;
        private FLS.MembershipFunctions.IMembershipFunction high_Openness;
        private FLS.MembershipFunctions.IMembershipFunction low_Conscientiousness;
        private FLS.MembershipFunctions.IMembershipFunction middle_Conscientiousness;
        private FLS.MembershipFunctions.IMembershipFunction high_Conscientiousness;
        private FLS.MembershipFunctions.IMembershipFunction low_Extraversion;
        private FLS.MembershipFunctions.IMembershipFunction middle_Extraversion;
        private FLS.MembershipFunctions.IMembershipFunction high_Extraversion;
        private FLS.MembershipFunctions.IMembershipFunction low_Agreeableness;
        private FLS.MembershipFunctions.IMembershipFunction middle_Agreeableness;
        private FLS.MembershipFunctions.IMembershipFunction high_Agreeableness;
        private FLS.MembershipFunctions.IMembershipFunction low_Neuroticism;
        private FLS.MembershipFunctions.IMembershipFunction middle_Neuroticism;
        private FLS.MembershipFunctions.IMembershipFunction high_Neuroticism;
        private PersonalityResults FuzzyMethodResult;

        public PersonalityTraits() { }

        public PersonalityTraits(
            float Openness, float Conscientiousness, float Extraversion, float Agreeableness, float Neuroticism)
        {
            this.Openness           = Openness;
            this.Conscientiousness  = Conscientiousness;
            this.Extraversion       = Extraversion;
            this.Agreeableness      = Agreeableness;
            this.Neuroticism        = Neuroticism;
            
            DominantPersonality = string.Empty;
            Personalities = new();
            StrategiesToApply = new();

            PersonalityConstruct();
        }


        private struct PersonalityResults
        {
            internal List<(string Trait, int ID)> PersonalitiesTraits;
            internal List<KeyValuePair<string, string>> StrategiesToApplied;
        }

        #region PersonalitieTraits
        private void PersonalitiesTraits()
        {
            const int Low       = 1;
            const int Middle    = 2;
            const int High      = 3;

            List<(string Trait, int ID)> PersonalityType = new();
  
            Linguistic_Conscientiousness = new LinguisticVariable("conscientiousness");
            Linguistic_Extraversion = new LinguisticVariable("extraversion");
            Linguistic_Neuroticism = new LinguisticVariable("neuroticism");
            Linguistic_Openness = new LinguisticVariable("openness");
            Linguistic_Agreeableness = new LinguisticVariable("agreeableness");

            low_Conscientiousness = Linguistic_Conscientiousness.MembershipFunctions.AddZShaped("low_Conscientiousness", 30, 10, 0, 100);
            middle_Conscientiousness = Linguistic_Conscientiousness.MembershipFunctions.AddGaussian("middleConscientiousness", 50, 10, 0, 100);
            high_Conscientiousness = Linguistic_Conscientiousness.MembershipFunctions.AddSShaped("highConscientiousness", 70, 10, 0, 100);

            low_Extraversion = Linguistic_Extraversion.MembershipFunctions.AddZShaped("lowExtraversion", 30, 10, 0, 100);
            middle_Extraversion = Linguistic_Extraversion.MembershipFunctions.AddGaussian("middleExtraversion", 50, 10, 0, 100);
            high_Extraversion = Linguistic_Extraversion.MembershipFunctions.AddSShaped("highExtraversion", 70, 10, 0, 100);

            low_Neuroticism = Linguistic_Neuroticism.MembershipFunctions.AddZShaped("lowNeuroticism", 30, 10, 0, 100);
            middle_Neuroticism = Linguistic_Neuroticism.MembershipFunctions.AddGaussian("middleNeuroticism", 50, 10, 0, 100);
            high_Neuroticism = Linguistic_Neuroticism.MembershipFunctions.AddSShaped("highNeuroticism", 70, 10, 0, 100);

            low_Openness = Linguistic_Openness.MembershipFunctions.AddZShaped("lowOpenness", 30, 10, 0, 100);
            middle_Openness = Linguistic_Openness.MembershipFunctions.AddGaussian("middle_Openness", 50, 10, 0, 100);
            high_Openness = Linguistic_Openness.MembershipFunctions.AddSShaped("high_Openness", 70, 10, 0, 100);

            low_Agreeableness = Linguistic_Agreeableness.MembershipFunctions.AddZShaped("lowAgreeableness", 30, 10, 0, 100);
            middle_Agreeableness = Linguistic_Agreeableness.MembershipFunctions.AddGaussian("middleAgreeableness", 50, 10, 0, 100);
            high_Agreeableness = Linguistic_Agreeableness.MembershipFunctions.AddSShaped("highAgreeableness", 70, 10, 0, 100);

            Dictionary<string, double> Dic_PersonalityType1 = new()
            {
                { "Low Conscientiousness", low_Conscientiousness.Fuzzify(Conscientiousness) },
                { "Middle Conscientiousness", middle_Conscientiousness.Fuzzify(Conscientiousness) },
                { "High Conscientiousness", high_Conscientiousness.Fuzzify(Conscientiousness) },
            };

            var Personality_1 = Dic_PersonalityType1.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
            
            if (Personality_1.Contains("Low")) PersonalityType.Add((Personality_1, Low));
            else if (Personality_1.Contains("Middle")) PersonalityType.Add((Personality_1, Middle));
            else PersonalityType.Add((Personality_1, High));

            Dictionary<string, double> Dic_PersonalityType2 = new()
            {
                { "Low Extraversion", low_Extraversion.Fuzzify(Extraversion) },
                { "Middle Extraversion",  middle_Extraversion.Fuzzify(Extraversion) },
                { "High Extraversion", high_Extraversion.Fuzzify(Extraversion) },
            };
            var Personality_2 = Dic_PersonalityType2.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
            
            if(Personality_2.Contains("Low")) PersonalityType.Add((Personality_2, Low));
            else if (Personality_2.Contains("Middle")) PersonalityType.Add((Personality_2, Middle));
            else PersonalityType.Add((Personality_2, High));


            Dictionary<string, double> Dic_PersonalityType3 = new()
            {
                { "Low Neuroticism", low_Neuroticism.Fuzzify(Neuroticism) },
                { "Middle Neuroticism", middle_Neuroticism.Fuzzify(Neuroticism) },
                { "High Neuroticism", high_Neuroticism.Fuzzify(Neuroticism) },
            };
            var Personality_3 = Dic_PersonalityType3.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
            
            if (Personality_3.Contains("Low")) PersonalityType.Add((Personality_3, Low));
            else if (Personality_3.Contains("Middle")) PersonalityType.Add((Personality_3, Middle));
            else PersonalityType.Add((Personality_3, High));


            Dictionary<string, double> Dic_PersonalityType4 = new()
            {
                { "Low Agreeableness", low_Agreeableness.Fuzzify(Agreeableness) },
                { "Middle Agreeableness", middle_Agreeableness.Fuzzify(Agreeableness) },
                { "High Agreeableness", high_Agreeableness.Fuzzify(Agreeableness) },
            };
            var Personality_4 = Dic_PersonalityType4.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
            
            if (Personality_4.Contains("Low")) PersonalityType.Add((Personality_4, Low));
            else if (Personality_4.Contains("Middle")) PersonalityType.Add((Personality_4, Middle));
            else PersonalityType.Add((Personality_4, High));

            Dictionary<string, double> PersonalityLinguisticResult5 = new()
            {
                { "Low Openness", low_Openness.Fuzzify(Openness) },
                { "Middle Openness", middle_Openness.Fuzzify(Openness) },
                { "High Openness", high_Openness.Fuzzify(Openness) },
            };
            var Personality_5 = PersonalityLinguisticResult5.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
            
            if (Personality_5.Contains("Low")) PersonalityType.Add((Personality_5, Low));
            else if (Personality_5.Contains("Middle")) PersonalityType.Add((Personality_5, Middle));
            else PersonalityType.Add((Personality_5, High));

            FuzzyMethodResult = new PersonalityResults()
            {
                PersonalitiesTraits = PersonalityType
            };
        }
        #endregion

        //situation selection
        private void FuzzySituationSelection()
        {

            List<KeyValuePair<string, string>> StrategyResults = new();
            FuzzyMethodResult.StrategiesToApplied = StrategyResults;

            var SituationSelection = new LinguisticVariable("SituationSelection");
            var WeaklyApplied = SituationSelection.MembershipFunctions.AddZShaped("WeaklyApplied", 3, 1, 0, 10);
            var LightlyApplied = SituationSelection.MembershipFunctions.AddGaussian("LightlyApplied", 5, 1, 0, 10);
            var StronglyApplied = SituationSelection.MembershipFunctions.AddSShaped("StronglyApplied", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            ///both personalities aren't opposites each other
            var rule1 = fuzzyEngine.Rules.If(
                Linguistic_Conscientiousness.Is(high_Conscientiousness).And(
                Linguistic_Neuroticism.Is(high_Neuroticism))).Then(SituationSelection.Is(StronglyApplied));
            var rule2 = fuzzyEngine.Rules.If(
                Linguistic_Conscientiousness.Is(middle_Conscientiousness).And(
                Linguistic_Neuroticism.Is(middle_Neuroticism))).Then(SituationSelection.Is(LightlyApplied));
            var rule3 = fuzzyEngine.Rules.If(
                Linguistic_Conscientiousness.Is(low_Conscientiousness).And(
                Linguistic_Neuroticism.Is(low_Neuroticism))).Then(SituationSelection.Is(WeaklyApplied));

            var rule4 = fuzzyEngine.Rules.If(
                Linguistic_Extraversion.Is(high_Extraversion).And(
                Linguistic_Openness.Is(high_Openness)).And(
                Linguistic_Agreeableness.Is(high_Agreeableness))).Then(SituationSelection.Is(WeaklyApplied));
            var rule5 = fuzzyEngine.Rules.If(
                Linguistic_Extraversion.Is(middle_Extraversion).And(
                Linguistic_Openness.Is(middle_Openness)).And(
                Linguistic_Agreeableness.Is(middle_Agreeableness))).Then(SituationSelection.Is(LightlyApplied));
            var rule6 = fuzzyEngine.Rules.If(
                Linguistic_Extraversion.Is(low_Extraversion).And(
                Linguistic_Openness.Is(low_Openness)).And(
                Linguistic_Agreeableness.Is(low_Agreeableness))).Then(SituationSelection.Is(StronglyApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6);

            var OutputDefuzzify = Convert.ToSingle(
                fuzzyEngine.Defuzzify(new
                {
                    conscientiousness = Conscientiousness,
                    extraversion = Extraversion,
                    neuroticism = Neuroticism,
                    agreeableness = Agreeableness,
                    openness = Openness
                }));

            Dictionary<string, double> DstrategyLinguisticResult = new();

            DstrategyLinguisticResult.Add("Weakly", WeaklyApplied.Fuzzify(OutputDefuzzify));
            DstrategyLinguisticResult.Add("Lightly", LightlyApplied.Fuzzify(OutputDefuzzify));
            DstrategyLinguisticResult.Add("Strongly", StronglyApplied.Fuzzify(OutputDefuzzify));

            var StrategyPower = DstrategyLinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;


            FuzzyMethodResult.StrategiesToApplied.Add(KeyValuePair.Create(EmotionRegulationAsset.SITUATION_SELECTION, StrategyPower));
        }

        //Situation modification
        private void FuzzySituationModification()
        {
            var SituationModification = new LinguisticVariable("SituationModification");
            var WeaklyApplied = SituationModification.MembershipFunctions.AddZShaped("WeaklyApplied", 3, 1, 0, 10);
            var LightlyApplied = SituationModification.MembershipFunctions.AddGaussian("LightlyApplied", 5, 1, 0, 10);
            var StronglyApplied = SituationModification.MembershipFunctions.AddSShaped("StronglyApplied", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            //both strategies are not opposites each other
            var rule1 = fuzzyEngine.Rules.If(
                Linguistic_Conscientiousness.Is(high_Conscientiousness).And(
                Linguistic_Extraversion.Is(high_Extraversion)).And(
                Linguistic_Openness.Is(high_Openness))).Then(SituationModification.Is(StronglyApplied));
            var rule2 = fuzzyEngine.Rules.If(
                Linguistic_Conscientiousness.Is(middle_Conscientiousness).And(
                Linguistic_Extraversion.Is(middle_Extraversion)).And(
                Linguistic_Openness.Is(middle_Openness))).Then(SituationModification.Is(LightlyApplied));
            var rule3 = fuzzyEngine.Rules.If(
                Linguistic_Conscientiousness.Is(low_Conscientiousness).And(
                Linguistic_Extraversion.Is(low_Extraversion)).And(
                Linguistic_Openness.Is(low_Openness))).Then(SituationModification.Is(WeaklyApplied));

            //both strategies are not opposites each other
            var rule4 = fuzzyEngine.Rules.If(
                Linguistic_Neuroticism.Is(high_Neuroticism).And(
                Linguistic_Agreeableness.Is(high_Agreeableness))).Then(SituationModification.Is(WeaklyApplied));
            var rule5 = fuzzyEngine.Rules.If(
                Linguistic_Neuroticism.Is(middle_Neuroticism).And(
                Linguistic_Agreeableness.Is(middle_Agreeableness))).Then(SituationModification.Is(LightlyApplied));
            var rule6 = fuzzyEngine.Rules.If(
                Linguistic_Neuroticism.Is(low_Neuroticism).And(
                Linguistic_Agreeableness.Is(low_Agreeableness))).Then(SituationModification.Is(StronglyApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6);

            var OutputDefuzzify = Convert.ToSingle(fuzzyEngine.Defuzzify(new
            {
                conscientiousness = Conscientiousness,
                extraversion = Extraversion,
                neuroticism = Neuroticism,
                agreeableness = Agreeableness,
                openness = Openness
            }));

            Dictionary<string, double> LinguisticResult = new()
            {
                { "Weakly", WeaklyApplied.Fuzzify(OutputDefuzzify) },
                { "Lightly", LightlyApplied.Fuzzify(OutputDefuzzify) },
                { "Strongly", StronglyApplied.Fuzzify(OutputDefuzzify) },
            };

            var StrategyPower = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            FuzzyMethodResult.StrategiesToApplied.Add(KeyValuePair.Create(EmotionRegulationAsset.SITUATION_MODIFICATION, StrategyPower));
        }

        ////Attention deployment
        private void FuzzyAttentionalDeployment()
        {
            var AttentionalDeployment = new LinguisticVariable("AttentionalDeployment");
            var WeaklyApplied = AttentionalDeployment.MembershipFunctions.AddZShaped("WeaklyApplied", 3, 1, 0, 10);
            var LightlyApplied = AttentionalDeployment.MembershipFunctions.AddGaussian("LightlyApplied", 5, 1, 0, 10);
            var StronglyApplied = AttentionalDeployment.MembershipFunctions.AddSShaped("StronglyApplied", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            //both strategies are not opposites each other
            var rule1 = fuzzyEngine.Rules.If(
                Linguistic_Openness.Is(high_Openness).And(
                Linguistic_Conscientiousness.Is(high_Conscientiousness)).And(
                Linguistic_Agreeableness.Is(high_Agreeableness)).And(
                Linguistic_Extraversion.Is(high_Extraversion))).Then(AttentionalDeployment.Is(StronglyApplied));
            var rule2 = fuzzyEngine.Rules.If(
                Linguistic_Openness.Is(middle_Openness).And(
                Linguistic_Conscientiousness.Is(middle_Conscientiousness)).And(
                Linguistic_Agreeableness.Is(middle_Agreeableness)).And(
                Linguistic_Extraversion.Is(middle_Extraversion))).Then(AttentionalDeployment.Is(LightlyApplied));
            var rule3 = fuzzyEngine.Rules.If(
                Linguistic_Openness.Is(low_Openness).And(
                Linguistic_Conscientiousness.Is(low_Conscientiousness)).And(
                Linguistic_Agreeableness.Is(low_Agreeableness)).And(
                Linguistic_Extraversion.Is(low_Extraversion))).Then(AttentionalDeployment.Is(WeaklyApplied));

            var rule4 = fuzzyEngine.Rules.If(Linguistic_Neuroticism.Is(high_Neuroticism)).Then(AttentionalDeployment.Is(WeaklyApplied));
            var rule5 = fuzzyEngine.Rules.If(Linguistic_Neuroticism.Is(middle_Neuroticism)).Then(AttentionalDeployment.Is(LightlyApplied));
            var rule6 = fuzzyEngine.Rules.If(Linguistic_Neuroticism.Is(low_Neuroticism)).Then(AttentionalDeployment.Is(StronglyApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6);

            var OutputDefuzzify = Convert.ToSingle(fuzzyEngine.Defuzzify(new
            {
                conscientiousness = Conscientiousness,
                extraversion = Extraversion,
                neuroticism = Neuroticism,
                agreeableness = Agreeableness,
                openness = Openness
            }));

            Dictionary<string, double> LinguisticResult = new()
            {
                { "Weakly", WeaklyApplied.Fuzzify(OutputDefuzzify) },
                { "Lightly", LightlyApplied.Fuzzify(OutputDefuzzify) },
                { "Strongly", StronglyApplied.Fuzzify(OutputDefuzzify) },
            };
            var StrategyPower = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            FuzzyMethodResult.StrategiesToApplied.Add(KeyValuePair.Create(EmotionRegulationAsset.ATTENTION_DEPLOYMENT, StrategyPower));
        }

        /////Cognitive change
        private void FuzzyCognitiveChange()
        {
            var CognitiveChange = new LinguisticVariable("CognitiveChange");
            var WeaklyApplied   = CognitiveChange.MembershipFunctions.AddZShaped("WeaklyApplied", 3, 1, 0, 10);
            var LightlyApplied  = CognitiveChange.MembershipFunctions.AddGaussian("LightlyApplied", 5, 1, 0, 10);
            var StronglyApplied = CognitiveChange.MembershipFunctions.AddSShaped("StronglyApplied", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            //both strategies are not opposites each other
            var rule1 = fuzzyEngine.Rules.If(Linguistic_Neuroticism.Is(high_Neuroticism)).Then(CognitiveChange.Is(WeaklyApplied));
            var rule2 = fuzzyEngine.Rules.If(Linguistic_Neuroticism.Is(middle_Neuroticism)).Then(CognitiveChange.Is(LightlyApplied));
            var rule3 = fuzzyEngine.Rules.If(Linguistic_Neuroticism.Is(low_Neuroticism)).Then(CognitiveChange.Is(StronglyApplied));
            
            var rule4 = fuzzyEngine.Rules.If(
                Linguistic_Openness.Is(high_Openness).And(
                Linguistic_Agreeableness.Is(high_Agreeableness)).And(
                Linguistic_Conscientiousness.Is(high_Conscientiousness)).And(
                Linguistic_Extraversion.Is(high_Extraversion))).Then(CognitiveChange.Is(StronglyApplied));
            var rule5 = fuzzyEngine.Rules.If(
                Linguistic_Openness.Is(middle_Openness).And(
                Linguistic_Agreeableness.Is(middle_Agreeableness)).And(
                Linguistic_Conscientiousness.Is(middle_Conscientiousness)).And(
                Linguistic_Extraversion.Is(middle_Extraversion))).Then(CognitiveChange.Is(LightlyApplied));
            var rule6 = fuzzyEngine.Rules.If(
                Linguistic_Openness.Is(low_Openness).And(
                Linguistic_Agreeableness.Is(low_Agreeableness)).And(
                Linguistic_Conscientiousness.Is(low_Conscientiousness)).And(
                Linguistic_Extraversion.Is(low_Extraversion))).Then(CognitiveChange.Is(WeaklyApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6);

            var OutputDefuzzify = Convert.ToSingle(fuzzyEngine.Defuzzify(new
            {
                conscientiousness = Conscientiousness,
                extraversion = Extraversion,
                neuroticism = Neuroticism,
                agreeableness = Agreeableness,
                openness = Openness
            }));

            Dictionary<string, double> LinguisticResult = new()
            {
                { "Weakly", WeaklyApplied.Fuzzify(OutputDefuzzify) },
                { "Lightly", LightlyApplied.Fuzzify(OutputDefuzzify) },
                { "Strongly", StronglyApplied.Fuzzify(OutputDefuzzify) },
            };
            var StrategyPower = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
           
            FuzzyMethodResult.StrategiesToApplied.Add(KeyValuePair.Create(EmotionRegulationAsset.COGNITIVE_CHANGE, StrategyPower));
        }

        ////Response modulation
        private void FuzzyResponseModulation()
        {
            var ResponseModulation = new LinguisticVariable("ResponseModulation");
            var WeaklyApplied = ResponseModulation.MembershipFunctions.AddZShaped("WeaklyApplied", 3, 1, 0, 10);
            var LightlyApplied = ResponseModulation.MembershipFunctions.AddGaussian("LightlyApplied", 5, 1, 0, 10);
            var StronglyApplied = ResponseModulation.MembershipFunctions.AddSShaped("StronglyApplied", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            //both strategies are not opposites each other
            var rule1 = fuzzyEngine.Rules.If(
                Linguistic_Extraversion.Is(high_Extraversion).And(
                Linguistic_Openness.Is(high_Openness)).And(
                Linguistic_Agreeableness.Is(high_Agreeableness)).And(
                Linguistic_Conscientiousness.Is(high_Conscientiousness)).And(
                Linguistic_Neuroticism.Is(high_Neuroticism))).Then(ResponseModulation.Is(WeaklyApplied));
            var rule2 = fuzzyEngine.Rules.If(
                Linguistic_Extraversion.Is(middle_Extraversion).And(
                Linguistic_Openness.Is(middle_Openness)).And(
                Linguistic_Agreeableness.Is(middle_Agreeableness)).And(
                Linguistic_Conscientiousness.Is(middle_Conscientiousness)).And(
                Linguistic_Neuroticism.Is(middle_Neuroticism))).Then(ResponseModulation.Is(LightlyApplied));
            var rule3 = fuzzyEngine.Rules.If(
                Linguistic_Extraversion.Is(low_Extraversion).And(
                Linguistic_Openness.Is(low_Openness)).And(
                Linguistic_Agreeableness.Is(low_Agreeableness)).And(
                Linguistic_Conscientiousness.Is(low_Conscientiousness)).And(
                Linguistic_Neuroticism.Is(low_Neuroticism))).Then(ResponseModulation.Is(StronglyApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3);

            var OutputDefuzzify = Convert.ToSingle(fuzzyEngine.Defuzzify(new
            {
                conscientiousness = Conscientiousness,
                extraversion = Extraversion,
                neuroticism = Neuroticism,
                agreeableness = Agreeableness,
                openness = Openness
            }));

            Dictionary<string, double> LinguisticResult = new()
            {
                { "Weakly", WeaklyApplied.Fuzzify(OutputDefuzzify) },
                { "Lightly", LightlyApplied.Fuzzify(OutputDefuzzify) },
                { "Strongly", StronglyApplied.Fuzzify(OutputDefuzzify) },
            };
            var StrategyPower = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            FuzzyMethodResult.StrategiesToApplied.Add(KeyValuePair.Create(EmotionRegulationAsset.RESPONSE_MODULATION, StrategyPower));
        }

        private void PersonalityConstruct()
        {
            PersonalitiesTraits();

            FuzzySituationSelection();
            FuzzySituationModification();
            FuzzyAttentionalDeployment();
            FuzzyCognitiveChange();
            FuzzyResponseModulation();

            FuzzyMethodResult.PersonalitiesTraits.ForEach(p => Personalities.Add(p.Trait));
            DominantPersonality = FuzzyMethodResult.PersonalitiesTraits.Aggregate((Trait, id)
                                                    => Trait.ID > id.ID ? Trait : id).Trait.Split(" ")[1];

            FuzzyMethodResult.StrategiesToApplied.ForEach(st => StrategiesToApply.Add(st));

            BigFiveValues = new List<double> { Openness, Conscientiousness, Extraversion, Agreeableness, Neuroticism };
        }


    }
}

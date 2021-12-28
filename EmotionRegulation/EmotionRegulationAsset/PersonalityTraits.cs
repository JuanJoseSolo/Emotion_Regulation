using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLS;

//ToDo: applying the new diffuse rules according to the new table of correlation personality traits

namespace EmotionRegulationAsset
{
    class PersonalityTraits
    {
        public string StrategyName { get; private set; }
        public double Conscientiousness { get; private set; }
        public double Extraversion { get; private set; }
        public double Neuroticism { get; private set; }
        public double Openness { get; private set; }
        public double Agreeableness { get; private set; }
        public string DominantPersonality { get; private set; }

        public string StrategyPower { get; private set; }
        public (List<string> PersonalityType, List<string> strategyName, List<string> strategyPower) TfuzzyResults
        { get; private set; }
        public Dictionary<string, string> DStrategyPower { get; private set; }


        public List<string> List_StrategyName { get; private set; }
        public List<string> List_StrategyPower { get; private set; }
        public List<string> List_PersonalityType { get; private set; }


        //public (string personalityType, Dictionary<string,double> dic) TdicPersonalityType { get; private set; }

        private float OutputDefuzzify { get; set; }

        public FLS.LinguisticVariable LVconscientiousness { get; private set; }
        public FLS.LinguisticVariable LVextraversion { get; private set; }
        public FLS.LinguisticVariable LVneuroticism { get; private set; }
        public FLS.LinguisticVariable LVopenness { get; private set; }
        public FLS.LinguisticVariable LVagreeableness { get; private set; }


        public FLS.MembershipFunctions.IMembershipFunction lowConscientiousness { get; private set; }
        public FLS.MembershipFunctions.IMembershipFunction middleConscientiousness { get; private set; }
        public FLS.MembershipFunctions.IMembershipFunction highConscientiousness { get; private set; }
        public FLS.MembershipFunctions.IMembershipFunction lowExtraversion { get; private set; }
        public FLS.MembershipFunctions.IMembershipFunction middleExtraversion { get; private set; }
        public FLS.MembershipFunctions.IMembershipFunction highExtraversion { get; private set; }
        public FLS.MembershipFunctions.IMembershipFunction lowNeuroticism { get; private set; }
        public FLS.MembershipFunctions.IMembershipFunction middleNeuroticism { get; private set; }
        public FLS.MembershipFunctions.IMembershipFunction highNeuroticism { get; private set; }
        public FLS.MembershipFunctions.IMembershipFunction lowOpenness { get; private set; }
        public FLS.MembershipFunctions.IMembershipFunction middleOpenness { get; private set; }
        public FLS.MembershipFunctions.IMembershipFunction highOpenness { get; private set; }
        public FLS.MembershipFunctions.IMembershipFunction lowAgreeableness { get; private set; }
        public FLS.MembershipFunctions.IMembershipFunction middleAgreeableness { get; private set; }
        public FLS.MembershipFunctions.IMembershipFunction highAgreeableness { get; private set; }

        public PersonalityTraits() { }

        public PersonalityTraits(
            float Conscientiousness,
            float Extraversion,
            float Neuroticism,
            float Openness,
            float Agreeableness)
        {

            this.Conscientiousness = Conscientiousness;
            this.Extraversion = Extraversion;
            this.Neuroticism = Neuroticism;
            this.Openness = Openness;
            this.Agreeableness = Agreeableness;

            this.List_StrategyName = new();
            this.List_StrategyPower = new();
            this.DominantPersonality = string.Empty;
            this.StrategyPower = string.Empty;
            this.List_PersonalityType = new();
            this.TfuzzyResults = (new(), new(), new());
            this.DStrategyPower = new();

            this.LVconscientiousness = null;
            this.LVextraversion = null;
            this.LVneuroticism = null;
            this.LVopenness = null;
            this.LVagreeableness = null;

            this.lowConscientiousness = null;
            this.middleConscientiousness = null;
            this.highConscientiousness = null;
            this.lowExtraversion = null;
            this.middleExtraversion = null;
            this.highExtraversion = null;
            this.lowNeuroticism = null;
            this.middleNeuroticism = null;
            this.highNeuroticism = null;
            this.lowOpenness = null;
            this.middleOpenness = null;
            this.highOpenness = null;
            this.lowAgreeableness = null;
            this.middleAgreeableness = null;
            this.highAgreeableness = null;


            StrategyName = string.Empty;
            OutputDefuzzify = 0.0f;

        }

        #region PersonalitieTraits
        internal List<string> PersonalitiesTraits()
        {
            this.LVconscientiousness = new LinguisticVariable("conscientiousness");
            this.LVextraversion = new LinguisticVariable("extraversion");
            this.LVneuroticism = new LinguisticVariable("neuroticism");
            this.LVopenness = new LinguisticVariable("openness");
            this.LVagreeableness = new LinguisticVariable("agreeableness");

            this.lowConscientiousness = this.LVconscientiousness.MembershipFunctions.AddZShaped("lowConscientiousness", 30, 10, 0, 100);
            this.middleConscientiousness = this.LVconscientiousness.MembershipFunctions.AddGaussian("middleConscientiousness", 50, 10, 0, 100);
            this.highConscientiousness = this.LVconscientiousness.MembershipFunctions.AddSShaped("highConscientiousness", 70, 10, 0, 100);

            this.lowExtraversion = this.LVextraversion.MembershipFunctions.AddZShaped("lowExtraversion", 30, 10, 0, 100);
            this.middleExtraversion = this.LVextraversion.MembershipFunctions.AddGaussian("middleExtraversion", 50, 10, 0, 100);
            this.highExtraversion = this.LVextraversion.MembershipFunctions.AddSShaped("highExtraversion", 70, 10, 0, 100);

            this.lowNeuroticism = this.LVneuroticism.MembershipFunctions.AddZShaped("lowNeuroticism", 30, 10, 0, 100);
            this.middleNeuroticism = this.LVneuroticism.MembershipFunctions.AddGaussian("middleNeuroticism", 50, 10, 0, 100);
            this.highNeuroticism = this.LVneuroticism.MembershipFunctions.AddSShaped("highNeuroticism", 70, 10, 0, 100);

            this.lowOpenness = this.LVopenness.MembershipFunctions.AddZShaped("lowOpenness", 30, 10, 0, 100);
            this.middleOpenness = this.LVopenness.MembershipFunctions.AddGaussian("middleOpenness", 50, 10, 0, 100);
            this.highOpenness = this.LVopenness.MembershipFunctions.AddSShaped("highOpenness", 70, 10, 0, 100);

            this.lowAgreeableness = this.LVagreeableness.MembershipFunctions.AddZShaped("lowAgreeableness", 30, 10, 0, 100);
            this.middleAgreeableness = this.LVagreeableness.MembershipFunctions.AddGaussian("middleAgreeableness", 50, 10, 0, 100);
            this.highAgreeableness = this.LVagreeableness.MembershipFunctions.AddSShaped("highAgreeableness", 70, 10, 0, 100);

            Dictionary<string, double> Dic_PersonalityType1 = new()
            {
                { "Low Conscientiousness", this.lowConscientiousness.Fuzzify(this.Conscientiousness) },
                { "Middle Conscientiousness", this.middleConscientiousness.Fuzzify(this.Conscientiousness) },
                { "High Conscientiousness", this.highConscientiousness.Fuzzify(this.Conscientiousness) },
            };

            var Personality_1 = Dic_PersonalityType1.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
            this.List_PersonalityType.Add(Personality_1);

            Dictionary<string, double> Dic_PersonalityType2 = new()
            {
                { "Low Extraversion", this.lowExtraversion.Fuzzify(this.Extraversion) },
                { "Middle Extraversion", this.middleExtraversion.Fuzzify(this.Extraversion) },
                { "High Extraversion", this.highExtraversion.Fuzzify(this.Extraversion) },
            };
            var Personality_2 = Dic_PersonalityType2.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
            this.List_PersonalityType.Add(Personality_2);


            Dictionary<string, double> Dic_PersonalityType3 = new()
            {
                { "Low Neuroticism", this.lowNeuroticism.Fuzzify(this.Neuroticism) },
                { "Middle Neuroticism", this.middleNeuroticism.Fuzzify(this.Neuroticism) },
                { "High Neuroticism", this.highNeuroticism.Fuzzify(this.Neuroticism) },
            };
            var Personality_3 = Dic_PersonalityType3.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
            this.List_PersonalityType.Add(Personality_3);


            Dictionary<string, double> Dic_PersonalityType4 = new()
            {
                { "Low Agreeableness", this.lowAgreeableness.Fuzzify(this.Agreeableness) },
                { "Middle Agreeableness", this.middleAgreeableness.Fuzzify(this.Agreeableness) },
                { "High Agreeableness", this.highAgreeableness.Fuzzify(this.Agreeableness) },
            };
            var Personality_4 = Dic_PersonalityType4.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
            this.List_PersonalityType.Add(Personality_4);

            Dictionary<string, double> PersonalityLinguisticResult5 = new()
            {
                { "Low Openness", this.lowOpenness.Fuzzify(this.Openness) },
                { "Middle Openness", this.middleOpenness.Fuzzify(this.Openness) },
                { "High Openness", this.highOpenness.Fuzzify(this.Openness) },
            };
            var Personality_5 = PersonalityLinguisticResult5.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
            this.List_PersonalityType.Add(Personality_5);

            return this.List_PersonalityType;
        }
        #endregion
        
        //situation selection
        public (List<string> personality, List<string> strategyName, List<string> strategyPower) FuzzySituationSelection()
        {
            this.StrategyName = string.Empty;
            this.StrategyPower = string.Empty;

            var SituationSelection = new LinguisticVariable("SituationSelection");
            var WeaklyApplied = SituationSelection.MembershipFunctions.AddZShaped("WeaklyApplied", 3, 1, 0, 10);
            var LightlyApplied = SituationSelection.MembershipFunctions.AddGaussian("LightlyApplied", 5, 1, 0, 10);
            var StronglyApplied = SituationSelection.MembershipFunctions.AddSShaped("StronglyApplied", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            ///both personalities aren't opposites each other
            var rule1 = fuzzyEngine.Rules.If(
                this.LVconscientiousness.Is(this.highConscientiousness).And(
                this.LVneuroticism.Is(this.highNeuroticism))).Then(SituationSelection.Is(StronglyApplied));
            var rule2 = fuzzyEngine.Rules.If(
                this.LVconscientiousness.Is(this.middleConscientiousness).And(
                this.LVneuroticism.Is(this.middleNeuroticism))).Then(SituationSelection.Is(LightlyApplied));
            var rule3 = fuzzyEngine.Rules.If(
                this.LVconscientiousness.Is(this.lowConscientiousness).And(
                this.LVneuroticism.Is(this.lowNeuroticism))).Then(SituationSelection.Is(WeaklyApplied));
            
            var rule4 = fuzzyEngine.Rules.If(
                this.LVextraversion.Is(this.highExtraversion).And(
                this.LVopenness.Is(this.highOpenness)).And(
                this.LVagreeableness.Is(this.highAgreeableness))).Then(SituationSelection.Is(WeaklyApplied));
            var rule5 = fuzzyEngine.Rules.If(
                this.LVextraversion.Is(this.middleExtraversion).And(
                this.LVopenness.Is(this.middleOpenness)).And(
                this.LVagreeableness.Is(this.middleAgreeableness))).Then(SituationSelection.Is(LightlyApplied));
            var rule6 = fuzzyEngine.Rules.If(
                this.LVextraversion.Is(this.lowExtraversion).And(
                this.LVopenness.Is(this.lowOpenness)).And(
                this.LVagreeableness.Is(this.lowAgreeableness))).Then(SituationSelection.Is(StronglyApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6);

            this.OutputDefuzzify = Convert.ToSingle(
                fuzzyEngine.Defuzzify(new 
                {
                    conscientiousness = this.Conscientiousness,
                    extraversion      = this.Extraversion,
                    neuroticism       = this.Neuroticism,
                    agreeableness     = this.Agreeableness,
                    openness          = this.Openness
                }));

            Dictionary<string, double> DstrategyLinguisticResult = new();

            DstrategyLinguisticResult.Add("Weakly", WeaklyApplied.Fuzzify((double)this.OutputDefuzzify));
            DstrategyLinguisticResult.Add("Lightly", LightlyApplied.Fuzzify((double)this.OutputDefuzzify));
            DstrategyLinguisticResult.Add("Strongly", StronglyApplied.Fuzzify((double)this.OutputDefuzzify));
            
            ///ToDo: la forma de elegir con que intensidad se aplica la estrategia falla cuando el tipo de personalidad 
            ///es alta en los cinco tipos, por lo que habrá que revisar cómo abordar este bug.
            this.StrategyPower = DstrategyLinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
            this.List_StrategyPower.Add(this.StrategyPower);

            this.StrategyName = "Situation Selection";
            this.List_StrategyName.Add(this.StrategyName);

            this.TfuzzyResults = (this.List_PersonalityType, this.List_StrategyName, this.List_StrategyPower);

            return this.TfuzzyResults;
        }

        //Situation modification
        public (List<string> personality, List<string> strategyName, List<string> strategyPower) FuzzySituationModification()
        {
            this.StrategyName = string.Empty;
            this.StrategyPower = string.Empty;

            var SituationModification = new LinguisticVariable("SituationModification");
            var WeaklyApplied = SituationModification.MembershipFunctions.AddZShaped("WeaklyApplied", 3, 1, 0, 10);
            var LightlyApplied = SituationModification.MembershipFunctions.AddGaussian("LightlyApplied", 5, 1, 0, 10);
            var StronglyApplied = SituationModification.MembershipFunctions.AddSShaped("StronglyApplied", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            //both strategies are not opposites each other
            var rule1 = fuzzyEngine.Rules.If(
                this.LVconscientiousness.Is(this.highConscientiousness).And(
                this.LVextraversion.Is(this.highExtraversion)).And(
                this.LVopenness.Is(this.highOpenness))).Then(SituationModification.Is(StronglyApplied));
            var rule2 = fuzzyEngine.Rules.If(
                this.LVconscientiousness.Is(this.middleConscientiousness).And(
                this.LVextraversion.Is(this.middleExtraversion)).And(
                this.LVopenness.Is(this.middleOpenness))).Then(SituationModification.Is(LightlyApplied));
            var rule3 = fuzzyEngine.Rules.If(
                this.LVconscientiousness.Is(this.lowConscientiousness).And(
                this.LVextraversion.Is(this.lowExtraversion)).And(
                this.LVopenness.Is(lowOpenness))).Then(SituationModification.Is(WeaklyApplied));

            //both strategies are not opposites each other
            var rule4 = fuzzyEngine.Rules.If(
                this.LVneuroticism.Is(this.highNeuroticism).And(
                this.LVagreeableness.Is(this.highAgreeableness))).Then(SituationModification.Is(WeaklyApplied));
            var rule5 = fuzzyEngine.Rules.If(
                this.LVneuroticism.Is(this.middleNeuroticism).And(
                this.LVagreeableness.Is(this.middleAgreeableness))).Then(SituationModification.Is(LightlyApplied));
            var rule6 = fuzzyEngine.Rules.If(
                this.LVneuroticism.Is(this.lowNeuroticism).And(
                this.LVagreeableness.Is(this.lowAgreeableness))).Then(SituationModification.Is(StronglyApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6);

            this.OutputDefuzzify = Convert.ToSingle(fuzzyEngine.Defuzzify(new
            {
                conscientiousness = this.Conscientiousness,
                extraversion = this.Extraversion,
                neuroticism = this.Neuroticism,
                agreeableness = this.Agreeableness,
                openness = this.Openness
            }));

            Dictionary<string, double> LinguisticResult = new()
            {
                { "Weakly", WeaklyApplied.Fuzzify((double)this.OutputDefuzzify) },
                { "Lightly", LightlyApplied.Fuzzify((double)this.OutputDefuzzify) },
                { "Strongly", StronglyApplied.Fuzzify((double)this.OutputDefuzzify) },
            };

            this.StrategyPower = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
            this.List_StrategyPower.Add(this.StrategyPower);

            this.StrategyName = "Situation Modification";
            this.List_StrategyName.Add(this.StrategyName);

            this.TfuzzyResults = (this.List_PersonalityType, this.List_StrategyName, this.List_StrategyPower);

            return this.TfuzzyResults;
        }

        ////Attention deployment
        public (List<string> personality, List<string> strategyName, List<string> strategyPower) FuzzyAttentionalDeployment()
        {
            this.StrategyName = string.Empty;
            this.StrategyPower = string.Empty;
            //this.List_StrategyPower = new();

            var AttentionalDeployment = new LinguisticVariable("AttentionalDeployment");
            var WeaklyApplied = AttentionalDeployment.MembershipFunctions.AddZShaped("WeaklyApplied", 3, 1, 0, 10);
            var LightlyApplied = AttentionalDeployment.MembershipFunctions.AddGaussian("LightlyApplied", 5, 1, 0, 10);
            var StronglyApplied = AttentionalDeployment.MembershipFunctions.AddSShaped("StronglyApplied", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            //both strategies are not opposites each other
            var rule1 = fuzzyEngine.Rules.If(
                this.LVopenness.Is(this.highOpenness).And(
                this.LVconscientiousness.Is(this.highConscientiousness)).And(
                this.LVagreeableness.Is(this.highAgreeableness)).And(
                this.LVextraversion.Is(this.highExtraversion))).Then(AttentionalDeployment.Is(StronglyApplied));
            var rule2 = fuzzyEngine.Rules.If(
                this.LVopenness.Is(this.middleOpenness).And(
                this.LVconscientiousness.Is(this.middleConscientiousness)).And(
                this.LVagreeableness.Is(this.middleAgreeableness)).And(
                this.LVextraversion.Is(this.middleExtraversion))).Then(AttentionalDeployment.Is(LightlyApplied));
            var rule3 = fuzzyEngine.Rules.If(
                this.LVopenness.Is(this.lowOpenness).And(
                this.LVconscientiousness.Is(this.lowConscientiousness)).And(
                this.LVagreeableness.Is(this.lowAgreeableness)).And(
                this.LVextraversion.Is(this.lowExtraversion))).Then(AttentionalDeployment.Is(WeaklyApplied));

            var rule4 = fuzzyEngine.Rules.If(this.LVneuroticism.Is(this.highNeuroticism)).Then(AttentionalDeployment.Is(WeaklyApplied));
            var rule5 = fuzzyEngine.Rules.If(this.LVneuroticism.Is(this.middleNeuroticism)).Then(AttentionalDeployment.Is(LightlyApplied));
            var rule6 = fuzzyEngine.Rules.If(this.LVneuroticism.Is(this.lowNeuroticism)).Then(AttentionalDeployment.Is(StronglyApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6);

            this.OutputDefuzzify = Convert.ToSingle(fuzzyEngine.Defuzzify(new
            {
                conscientiousness = this.Conscientiousness,
                extraversion = this.Extraversion,
                neuroticism = this.Neuroticism,
                agreeableness = this.Agreeableness,
                openness = this.Openness
            }));

            Dictionary<string, double> LinguisticResult = new()
            {
                { "Weakly", WeaklyApplied.Fuzzify((double)this.OutputDefuzzify) },
                { "Lightly", LightlyApplied.Fuzzify((double)this.OutputDefuzzify) },
                { "Strongly", StronglyApplied.Fuzzify((double)this.OutputDefuzzify) },
            };
            this.StrategyPower = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
            this.List_StrategyPower.Add(this.StrategyPower);

            this.StrategyName = "Attention Deployment";
            this.List_StrategyName.Add(this.StrategyName);


            this.TfuzzyResults = (this.List_PersonalityType, this.List_StrategyName, this.List_StrategyPower);

            return this.TfuzzyResults;
        }

        /////Cognitive change
        public (List<string> personality, List<string> strategyName, List<string> strategyPower) FuzzyCognitiveChange()
        {
            this.StrategyName = string.Empty;
            this.StrategyPower = string.Empty;
            //this.List_StrategyPower = new();

            var CognitiveChange = new LinguisticVariable("CognitiveChange");
            var WeaklyApplied   = CognitiveChange.MembershipFunctions.AddZShaped("WeaklyApplied", 3, 1, 0, 10);
            var LightlyApplied  = CognitiveChange.MembershipFunctions.AddGaussian("LightlyApplied", 5, 1, 0, 10);
            var StronglyApplied = CognitiveChange.MembershipFunctions.AddSShaped("StronglyApplied", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            //both strategies are not opposites each other
            var rule1 = fuzzyEngine.Rules.If(this.LVneuroticism.Is(this.highNeuroticism)).Then(CognitiveChange.Is(WeaklyApplied));
            var rule2 = fuzzyEngine.Rules.If(this.LVneuroticism.Is(this.middleNeuroticism)).Then(CognitiveChange.Is(LightlyApplied));
            var rule3 = fuzzyEngine.Rules.If(this.LVneuroticism.Is(this.lowNeuroticism)).Then(CognitiveChange.Is(StronglyApplied));
            
            var rule4 = fuzzyEngine.Rules.If(
                this.LVopenness.Is(this.highOpenness).And(
                this.LVagreeableness.Is(this.highAgreeableness)).And(
                this.LVconscientiousness.Is(this.highConscientiousness)).And(
                this.LVextraversion.Is(this.highExtraversion))).Then(CognitiveChange.Is(StronglyApplied));
            var rule5 = fuzzyEngine.Rules.If(
                this.LVopenness.Is(this.middleOpenness).And(
                this.LVagreeableness.Is(this.middleAgreeableness)).And(
                this.LVconscientiousness.Is(this.middleConscientiousness)).And(
                this.LVextraversion.Is(this.middleExtraversion))).Then(CognitiveChange.Is(LightlyApplied));
            var rule6 = fuzzyEngine.Rules.If(
                this.LVopenness.Is(this.lowOpenness).And(
                this.LVagreeableness.Is(this.lowAgreeableness)).And(
                this.LVconscientiousness.Is(this.lowConscientiousness)).And(
                this.LVextraversion.Is(this.lowExtraversion))).Then(CognitiveChange.Is(WeaklyApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6);

            this.OutputDefuzzify = Convert.ToSingle(fuzzyEngine.Defuzzify(new
            {
                conscientiousness = this.Conscientiousness,
                extraversion = this.Extraversion,
                neuroticism = this.Neuroticism,
                agreeableness = this.Agreeableness,
                openness = this.Openness
            }));

            Dictionary<string, double> LinguisticResult = new()
            {
                { "Weakly", WeaklyApplied.Fuzzify((double)this.OutputDefuzzify) },
                { "Lightly", LightlyApplied.Fuzzify((double)this.OutputDefuzzify) },
                { "Strongly", StronglyApplied.Fuzzify((double)this.OutputDefuzzify) },
            };
            this.StrategyPower = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
            this.List_StrategyPower.Add(this.StrategyPower);

            this.StrategyName = "Cognitive Change"; 
            this.List_StrategyName.Add(this.StrategyName);

            this.TfuzzyResults = (this.List_PersonalityType, this.List_StrategyName, this.List_StrategyPower);
            return this.TfuzzyResults;
        }

        ////Response modulation
        public (List<string> personality, List<string> strategyName, List<string> strategyPower) FuzzyResponseModulation()
        {
            //Recordatorio: Revisar accesivilidad de variables globales
            this.StrategyName = string.Empty;
            this.StrategyPower = string.Empty;

            var ResponseModulation = new LinguisticVariable("ResponseModulation");
            var WeaklyApplied = ResponseModulation.MembershipFunctions.AddZShaped("WeaklyApplied", 3, 1, 0, 10);
            var LightlyApplied = ResponseModulation.MembershipFunctions.AddGaussian("LightlyApplied", 5, 1, 0, 10);
            var StronglyApplied = ResponseModulation.MembershipFunctions.AddSShaped("StronglyApplied", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            //both strategies are not opposites each other
            var rule1 = fuzzyEngine.Rules.If(
                this.LVextraversion.Is(this.highExtraversion).And(
                this.LVopenness.Is(this.highOpenness)).And(
                this.LVagreeableness.Is(this.highAgreeableness)).And(
                this.LVconscientiousness.Is(this.highConscientiousness)).And(
                this.LVneuroticism.Is(this.highNeuroticism))).Then(ResponseModulation.Is(WeaklyApplied));
            var rule2 = fuzzyEngine.Rules.If(
                this.LVextraversion.Is(this.middleExtraversion).And(
                this.LVopenness.Is(this.middleOpenness)).And(
                this.LVagreeableness.Is(this.middleAgreeableness)).And(
                this.LVconscientiousness.Is(this.middleConscientiousness)).And(
                this.LVneuroticism.Is(this.middleNeuroticism))).Then(ResponseModulation.Is(LightlyApplied));
            var rule3 = fuzzyEngine.Rules.If(
                this.LVextraversion.Is(this.lowExtraversion).And(
                this.LVopenness.Is(this.lowOpenness)).And(
                this.LVagreeableness.Is(this.lowAgreeableness)).And(
                this.LVconscientiousness.Is(this.lowConscientiousness)).And(
                this.LVneuroticism.Is(this.lowNeuroticism))).Then(ResponseModulation.Is(StronglyApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3);

            this.OutputDefuzzify = Convert.ToSingle(fuzzyEngine.Defuzzify(new
            {
                conscientiousness = this.Conscientiousness,
                extraversion = this.Extraversion,
                neuroticism = this.Neuroticism,
                agreeableness = this.Agreeableness,
                openness = this.Openness
            }));

            //Personality openness have a slove to lightly
            Dictionary<string, double> LinguisticResult = new()
            {
                { "Weakly", WeaklyApplied.Fuzzify((double)this.OutputDefuzzify) },
                { "Lightly", LightlyApplied.Fuzzify((double)this.OutputDefuzzify) },
                { "Strongly", StronglyApplied.Fuzzify((double)this.OutputDefuzzify) },
            };
            this.StrategyPower = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            this.List_StrategyPower.Add(this.StrategyPower);

            this.StrategyName = "Response Modulation";
            this.List_StrategyName.Add(StrategyName);

            this.TfuzzyResults = (this.List_PersonalityType, this.List_StrategyName, this.List_StrategyPower);
            return this.TfuzzyResults;
        }

        #region estrategies for personality
        //Evaluation of personalities
        public void FuzzyAppliedStrategyTest()
        {
            Console.WriteLine("\n------------------------Strategy selection------------------------\n");
            
            var LpersonalityType = PersonalitiesTraits();
            var (_, _, _) = FuzzySituationSelection();
            var (_, _, _) = FuzzySituationModification();
            var (_, _, _) = FuzzyAttentionalDeployment();
            var (_, _, _) = FuzzyCognitiveChange();
            var (_, _, _) = FuzzyResponseModulation();

            Console.WriteLine("   Character Personality Traits : \n");
            foreach (var p in this.List_PersonalityType) { Console.WriteLine(" Personality: " + p); }

            var DominatPersonalities = LpersonalityType.Where(p => p.Contains("High")).ToList();
            var SplitPersonality = DominatPersonalities.Select(t => t.Split(" ")).ToList();
            var DominantTraits = SplitPersonality.Select(h => h[1]).ToList();

            ///if there is more than one dominant personality, 
            ///then personality chosen will be the last one within highest numerical value
            Dictionary<string, float> numericalPersonality = new()
            {
                { "Conscientiousness", (float)this.Conscientiousness },
                { "Extraversion", (float)this.Extraversion },
                { "Neuroticism", (float)this.Neuroticism },
                { "Openness", (float)this.Openness },
                { "Agreeableness", (float)this.Agreeableness }
            };
            if (DominantTraits.Count > 0) 
            {
                this.DominantPersonality = numericalPersonality.Aggregate((
                    personality, value) => personality.Value > value.Value ? personality:value).Key;
            }
            else
            {
                this.DominantPersonality = DominantTraits.FirstOrDefault();
            }
            Console.WriteLine(" \nDominant Personality is : " + this.DominantPersonality);

            var strategyCharacter = TfuzzyResults.strategyName;
            var strategyPower = TfuzzyResults.strategyPower;
            int index = 0;

            Console.WriteLine("\n Personality type can apply : \n");
            foreach (var Strategy in strategyCharacter)
            {
                Console.WriteLine("  " + Strategy + " ===> " + strategyPower.ElementAt(index));
                DStrategyPower.Add(Strategy, strategyPower.ElementAt(index));
                index++;
            }
        }
        #endregion
    }
}

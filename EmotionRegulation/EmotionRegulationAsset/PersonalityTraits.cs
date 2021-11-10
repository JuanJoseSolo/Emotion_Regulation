using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLS;

namespace EmotionRegulationAsset
{
    class PersonalityTraits
    {
        /// <summary>
        /// Selecciona el tipo de personalidad que se va a utilizar
        /// </summary>
        public float valorPertenenciaStrApp { get; set; }
        public string NameStrategy { get; set; }
        public bool ApplyEstrategy { get; set; }
        private double Cons { get; set; }
        private double Extra { get; set; }
        private double Neur { get; set; }
        private double Ope { get; set; }
        private double Agre { get; set; }

        private float OutputDefuzzify { get; set; }

        public PersonalityTraits(float Consientioness, float Extraversion, float Neuroticism, float Openness, float Agreeableness)
        {

            this.Cons = Consientioness;
            this.Extra = Extraversion;
            this.Neur = Neuroticism;
            this.Ope = Openness;
            this.Agre = Agreeableness;

            NameStrategy = string.Empty;
            ApplyEstrategy = false;
            OutputDefuzzify = 0.0f;
        }

        public float outputDefuzzify { get => OutputDefuzzify;}
        public float Consientioness { get => (float)this.Cons; }
        public float Extraversion { get => (float)this.Extra; }
        public float Neuroticism { get => (float)this.Neur; }
        public float Openness { get => (float)this.Ope; }
        public float Agreeableness { get => (float)this.Agre; }

        ////situation selection
        public string FuzzySituationSelection()
        {
            var Conscient = new LinguisticVariable("Conscient");
            var Extravers = new LinguisticVariable("Extravers");

            var lowCon = Conscient.MembershipFunctions.AddZShaped("lowCon", 30, 10, 0, 100);
            var middleCon = Conscient.MembershipFunctions.AddGaussian("middleCon", 50, 10, 0, 100);
            var highCon = Conscient.MembershipFunctions.AddSShaped("highCon", 70, 10, 0, 100);

            var lowExt = Extravers.MembershipFunctions.AddZShaped("lowExt", 30, 10, 0, 100);
            var middleExt = Extravers.MembershipFunctions.AddGaussian("middleExt", 50, 10, 0, 100);
            var highExt = Extravers.MembershipFunctions.AddSShaped("highExt", 70, 10, 0, 100);

            var SitSelec = new LinguisticVariable("SitSelec");
            var WeaApplied = SitSelec.MembershipFunctions.AddZShaped("WeaApp", 3, 1, 0, 10);
            var MidApplied = SitSelec.MembershipFunctions.AddGaussian("MidApp", 5, 1, 0, 10);
            var StrApplied = SitSelec.MembershipFunctions.AddSShaped("StrApp", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            var rule1 = fuzzyEngine.Rules.If(Conscient.Is(highCon)).Then(SitSelec.Is(StrApplied));
            var rule2 = fuzzyEngine.Rules.If(Conscient.Is(middleCon)).Then(SitSelec.Is(MidApplied));
            var rule3 = fuzzyEngine.Rules.If(Conscient.Is(lowCon)).Then(SitSelec.Is(WeaApplied));

            var rule4 = fuzzyEngine.Rules.If(Extravers.Is(highExt)).Then(SitSelec.Is(WeaApplied));
            var rule5 = fuzzyEngine.Rules.If(Extravers.Is(middleExt)).Then(SitSelec.Is(MidApplied));
            var rule6 = fuzzyEngine.Rules.If(Extravers.Is(lowExt)).Then(SitSelec.Is(StrApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule2, rule3, rule4, rule5, rule6);

            OutputDefuzzify = Convert.ToSingle(fuzzyEngine.Defuzzify(new { Conscient = this.Cons, Extravers = this.Extra }));

            Dictionary<string, double> LinguisticResult = new()
            {
                { "WeaApplied", WeaApplied.Fuzzify(OutputDefuzzify) },
                { "MidApplied", MidApplied.Fuzzify(OutputDefuzzify) },
                { "StrApplied", StrApplied.Fuzzify(OutputDefuzzify) },
            };

            var LinguisticOutput = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            
            return LinguisticOutput;
        }

        ///Situation modification
        public string FuzzySituationModification()
        {
            var Conscient = new LinguisticVariable("Conscient");
            var Extravers = new LinguisticVariable("Extravers");
            var Neurotici = new LinguisticVariable("Neurotici");
            var Agreeable = new LinguisticVariable("Agreeable");

            var lowCon    = Conscient.MembershipFunctions.AddZShaped("lowCon", 30, 10, 0, 100);
            var middleCon = Conscient.MembershipFunctions.AddGaussian("middleCon", 50, 10, 0, 100);
            var highCon   = Conscient.MembershipFunctions.AddSShaped("highCon", 70, 10, 0, 100);

            var lowExt    = Extravers.MembershipFunctions.AddZShaped("lowExt", 30, 10, 0, 100);
            var middleExt = Extravers.MembershipFunctions.AddGaussian("middleExt", 50, 10, 0, 100);
            var highExt   = Extravers.MembershipFunctions.AddSShaped("highExt", 70, 10, 0, 100);

            var lowNeu    = Neurotici.MembershipFunctions.AddZShaped("lowNeu", 30, 10, 0, 100);
            var middleNeu = Neurotici.MembershipFunctions.AddGaussian("middleNeu", 50, 10, 0, 100);
            var highNeu   = Neurotici.MembershipFunctions.AddSShaped("highNeu", 70, 10, 0, 100);

            var lowAgr    = Agreeable.MembershipFunctions.AddZShaped("lowAgr", 30, 10, 0, 100);
            var middleAgr = Agreeable.MembershipFunctions.AddGaussian("middleAgr", 50, 10, 0, 100);
            var highAgr   = Agreeable.MembershipFunctions.AddSShaped("highAgr", 70, 10, 0, 100);

            var SitMod = new LinguisticVariable("SitMod");
            var WeaApplied = SitMod.MembershipFunctions.AddZShaped("WeaApp", 3, 1, 0, 10);
            var MidApplied = SitMod.MembershipFunctions.AddGaussian("MidApp", 5, 1, 0, 10);
            var StrApplied = SitMod.MembershipFunctions.AddSShaped("StrApp", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            var rule1 = fuzzyEngine.Rules.If(Conscient.Is(highCon).Or(Extravers.Is(highExt))).Then(SitMod.Is(StrApplied));
            var rule2 = fuzzyEngine.Rules.If(Conscient.Is(middleCon).Or(Extravers.Is(middleExt))).Then(SitMod.Is(MidApplied));
            var rule3 = fuzzyEngine.Rules.If(Conscient.Is(lowCon).Or(Extravers.Is(lowExt))).Then(SitMod.Is(WeaApplied));

            var rule4 = fuzzyEngine.Rules.If(Neurotici.Is(highNeu).Or(Agreeable.Is(highAgr))).Then(SitMod.Is(WeaApplied));
            var rule5 = fuzzyEngine.Rules.If(Neurotici.Is(middleNeu).Or(Agreeable.Is(middleAgr))).Then(SitMod.Is(MidApplied));
            var rule6 = fuzzyEngine.Rules.If(Neurotici.Is(lowNeu).Or(Agreeable.Is(lowAgr))).Then(SitMod.Is(StrApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6);

            OutputDefuzzify = Convert.ToSingle(fuzzyEngine.Defuzzify(new { Conscient = this.Cons, Extravers = this.Extra, Neurotici = this.Neur, Agreeable = this.Agre }));
            
            Dictionary<string, double> LinguisticResult = new()
            {
                { "WeaApplied", WeaApplied.Fuzzify(OutputDefuzzify) },
                { "MidApplied", MidApplied.Fuzzify(OutputDefuzzify) },
                { "StrApplied", StrApplied.Fuzzify(OutputDefuzzify) },
            };

            var LinguisticOutput = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;
            
            valorPertenenciaStrApp = (float)LinguisticResult["StrApplied"];

            return LinguisticOutput;
        }

        ////Attention deployment
        public string FuzzyAttentionalDeployment(double Co, double Op, double Ne)
        {
            var Conscient = new LinguisticVariable("Conscient");
            var Neurotici = new LinguisticVariable("Neurotici");
            var Openness  = new LinguisticVariable("Openness");

            var lowCon    = Conscient.MembershipFunctions.AddZShaped("lowCon", 30, 10, 0, 100);
            var middleCon = Conscient.MembershipFunctions.AddGaussian("middleCon", 50, 10, 0, 100);
            var highCon   = Conscient.MembershipFunctions.AddSShaped("highCon", 70, 10, 0, 100);

            var lowNeu    = Neurotici.MembershipFunctions.AddZShaped("lowNeu", 30, 10, 0, 100);
            var middleNeu = Neurotici.MembershipFunctions.AddGaussian("middleNeu", 50, 10, 0, 100);
            var highNeu   = Neurotici.MembershipFunctions.AddSShaped("highNeu", 70, 10, 0, 100);

            var lowOpe    = Openness.MembershipFunctions.AddZShaped("lowOpe", 30, 10, 0, 100);
            var middleOpe = Openness.MembershipFunctions.AddGaussian("middleOpe", 50, 10, 0, 100);
            var highOpe   = Openness.MembershipFunctions.AddSShaped("highOpe", 70, 10, 0, 100);

            var AttenDepoly = new LinguisticVariable("AttenDepoly");
            var WeaApplied = AttenDepoly.MembershipFunctions.AddZShaped("WeaApp", 3, 1, 0, 10);
            var MidApplied = AttenDepoly.MembershipFunctions.AddGaussian("MidApp", 5, 1, 0, 10);
            var StrApplied = AttenDepoly.MembershipFunctions.AddSShaped("StrApp", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            var rule1 = fuzzyEngine.Rules.If(Conscient.Is(highCon).Or(Openness.Is(highOpe))).Then(AttenDepoly.Is(StrApplied));
            var rule2 = fuzzyEngine.Rules.If(Conscient.Is(middleCon).Or(Openness.Is(middleOpe))).Then(AttenDepoly.Is(MidApplied));
            var rule3 = fuzzyEngine.Rules.If(Conscient.Is(lowCon).Or(Openness.Is(lowOpe))).Then(AttenDepoly.Is(WeaApplied));

            var rule4 = fuzzyEngine.Rules.If(Neurotici.Is(highNeu)).Then(AttenDepoly.Is(WeaApplied));
            var rule5 = fuzzyEngine.Rules.If(Neurotici.Is(middleNeu)).Then(AttenDepoly.Is(MidApplied));
            var rule6 = fuzzyEngine.Rules.If(Neurotici.Is(lowNeu)).Then(AttenDepoly.Is(StrApplied));

            fuzzyEngine.Rules.Add(rule1, rule2, rule2, rule3, rule4, rule5, rule6);

            OutputDefuzzify = Convert.ToSingle(fuzzyEngine.Defuzzify(new { Conscient = Co, Openness = Op, Neurotici = Ne }));

            Dictionary<string, double> LinguisticResult = new()
            {
                { "WeaApplied", WeaApplied.Fuzzify(OutputDefuzzify) },
                { "MidApplied", MidApplied.Fuzzify(OutputDefuzzify) },
                { "StrApplied", StrApplied.Fuzzify(OutputDefuzzify) },
            };

            var LinguisticOutput = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            return LinguisticOutput;
        }

        /////Cognitive change
        public string FuzzyCognitiveChange(double Ne, double Op)
        {
            var Conscient = new LinguisticVariable("Conscient");
            var Extravers = new LinguisticVariable("Extravers");
            var Neurotici = new LinguisticVariable("Neurotici");
            var Openness  = new LinguisticVariable("Openness");
            var Agreeable = new LinguisticVariable("Agreeable");

            var lowNeu    = Neurotici.MembershipFunctions.AddZShaped("lowNeu", 30, 10, 0, 100);
            var middleNeu = Neurotici.MembershipFunctions.AddGaussian("middleNeu", 50, 10, 0, 100);
            var highNeu   = Neurotici.MembershipFunctions.AddSShaped("highNeu", 70, 10, 0, 100);

            var lowOpe    = Openness.MembershipFunctions.AddZShaped("lowOpe", 30, 10, 0, 100);
            var middleOpe = Openness.MembershipFunctions.AddGaussian("middleOpe", 50, 10, 0, 100);
            var highOpe   = Openness.MembershipFunctions.AddSShaped("highOpe", 70, 10, 0, 100);

            var CognChange = new LinguisticVariable("CognChange");
            var WeaApp = CognChange.MembershipFunctions.AddZShaped("WeaApp", 3, 1, 0, 10);
            var MidApp = CognChange.MembershipFunctions.AddGaussian("MidApp", 5, 1, 0, 10);
            var StrApp = CognChange.MembershipFunctions.AddSShaped("StrApp", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            var rule1 = fuzzyEngine.Rules.If(Neurotici.Is(highNeu)).Then(CognChange.Is(WeaApp));
            var rule2 = fuzzyEngine.Rules.If(Neurotici.Is(middleNeu)).Then(CognChange.Is(MidApp));
            var rule3 = fuzzyEngine.Rules.If(Neurotici.Is(lowNeu)).Then(CognChange.Is(StrApp));

            var rule4 = fuzzyEngine.Rules.If(Openness.Is(highOpe)).Then(CognChange.Is(StrApp));
            var rule5 = fuzzyEngine.Rules.If(Openness.Is(middleOpe)).Then(CognChange.Is(MidApp));
            var rule6 = fuzzyEngine.Rules.If(Openness.Is(lowOpe)).Then(CognChange.Is(WeaApp));

            fuzzyEngine.Rules.Add(rule1, rule2, rule2, rule3, rule4, rule5, rule6);

            OutputDefuzzify = Convert.ToSingle(fuzzyEngine.Defuzzify(new { Neurotici = Ne, Openness = Op }));

            Dictionary<string, double> LinguisticResult = new()
            {
                { "WeaApp", WeaApp.Fuzzify(OutputDefuzzify) },
                { "MidApp", MidApp.Fuzzify(OutputDefuzzify) },
                { "StrApp", StrApp.Fuzzify(OutputDefuzzify) },
            };

            var LinguisticOutput = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            return LinguisticOutput;
        }

        ////Response modulation
        public string FuzzyResponseModulation(double Op, double Ex)
        {
            var Conscient = new LinguisticVariable("Conscient");
            var Extravers = new LinguisticVariable("Extravers");
            var Neurotici = new LinguisticVariable("Neurotici");
            var Openness  = new LinguisticVariable("Openness");
            var Agreeable = new LinguisticVariable("Agreeable");

            var lowExt    = Extravers.MembershipFunctions.AddZShaped("lowExt", 30, 10, 0, 100);
            var middleExt = Extravers.MembershipFunctions.AddGaussian("middleExt", 50, 10, 0, 100);
            var highExt   = Extravers.MembershipFunctions.AddSShaped("highExt", 70, 10, 0, 100);

            var lowOpe    = Openness.MembershipFunctions.AddZShaped("lowOpe", 30, 10, 0, 100);
            var middleOpe = Openness.MembershipFunctions.AddGaussian("middleOpe", 50, 10, 0, 100);
            var highOpe   = Openness.MembershipFunctions.AddSShaped("highOpe", 70, 10, 0, 100);

            var RespModula = new LinguisticVariable("SitSelec");
            var WeaApp     = RespModula.MembershipFunctions.AddZShaped("WeaApp", 3, 1, 0, 10);
            var MidApp     = RespModula.MembershipFunctions.AddGaussian("MidApp", 5, 1, 0, 10);
            var StrApp     = RespModula.MembershipFunctions.AddSShaped("StrApp", 7, 1, 0, 10);

            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            var rule1 = fuzzyEngine.Rules.If(Openness.Is(highOpe).Or(Extravers.Is(highExt))).Then(RespModula.Is(WeaApp));
            var rule2 = fuzzyEngine.Rules.If(Openness.Is(middleOpe).Or(Extravers.Is(middleExt))).Then(RespModula.Is(WeaApp));
            var rule3 = fuzzyEngine.Rules.If(Openness.Is(lowOpe).Or(Extravers.Is(lowExt))).Then(RespModula.Is(MidApp));

            fuzzyEngine.Rules.Add(rule1, rule2, rule2, rule3);

            OutputDefuzzify = Convert.ToSingle(fuzzyEngine.Defuzzify(new { Openness = Op, Extravers = Ex }));

            Dictionary<string, double> LinguisticResult = new()
            {
                { "WeaApp", WeaApp.Fuzzify(OutputDefuzzify) },
                { "MidApp", MidApp.Fuzzify(OutputDefuzzify) },
                { "StrApp", StrApp.Fuzzify(OutputDefuzzify) },
            };

            var LinguisticOutput = LinguisticResult.Aggregate(
                (LinguisticVariableName, r) => LinguisticVariableName.Value > r.Value ? LinguisticVariableName : r).Key;

            return LinguisticOutput;
        }



        /// <summary>
        /// En construction......
        /// </summary>
        /// <param name="Consientioness"></param>
        /// <param name="Extraversion"></param>
        /// <param name="Neuroticism"></param>
        /// <param name="Openness"></param>
        /// <param name="Agreeableness"></param>
        public void FuzzyAppliedStrategy()
        {
            Console.WriteLine("\n------------------------public void FuzzyAppliedStrategy()------------------------\n");
            
            switch ((this.Cons, this.Extra, this.Neur, this.Ope, this.Agre))
            {

                case ( >= 0, >= 0, >= 0, >= 0, >= 0) when (this.Neur == 0) && (this.Ope == 0) && (this.Agre == 0):

                    string Situation_Selection = FuzzySituationSelection();
                    Console.WriteLine("Valor Defuzzify rules: " + outputDefuzzify);
                    Console.WriteLine("Situation Selection is: " + Situation_Selection);
                    Console.WriteLine("Valor de pertencia de StrApplied: " + valorPertenenciaStrApp);

                    if (Situation_Selection is "StrApplied")
                    {
                        NameStrategy   = "Situation Selection";
                        ApplyEstrategy = true;                       

                    }
                    else
                    {
                        NameStrategy   = "Situation Selection isn't applied";
                        ApplyEstrategy = false;
                    }
                    break;

                case ( >= 0, >= 0, >= 0, >= 0, >= 0) when (this.Ope == 0):

                    string Situation_Modification = FuzzySituationModification();

                    Console.WriteLine("Valor Defuzzify rules: " + outputDefuzzify);
                    Console.WriteLine("Situation Modification is: " + Situation_Modification);
                    Console.WriteLine("Valor de pertencia de StrApplied: " + valorPertenenciaStrApp);

                    if (Situation_Modification is "StrApplied")
                    {
                        NameStrategy = "Situation Modification";
                        ApplyEstrategy = true;
                    }
                    else
                    {
                        NameStrategy = "Situation Modification isn't applied";
                        ApplyEstrategy = false;
                    }
                    break;
               
                case ( >= 0, >= 0, >= 0, >= 0, >= 0) when (this.Ope != 0):

                    string Attentional_Deployment = FuzzySituationModification();

                    Console.WriteLine("Valor Defuzzify rules: " + outputDefuzzify);
                    Console.WriteLine("Attentional Deployment is: " + Attentional_Deployment);
                    Console.WriteLine("Valor de pertencia de StrApplied: " + valorPertenenciaStrApp);

                    if (Attentional_Deployment is "StrApplied")
                    {
                        NameStrategy = "Attentional Deployment";
                        ApplyEstrategy = true;
                    }
                    else
                    {
                        NameStrategy = "Attentional Deployment isn't applied";
                        ApplyEstrategy = false;
                    }
                    break;
            }
        }
    }
}

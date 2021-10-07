using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLS;




namespace Fuzzy_Personalities
{
    public class Strategies
    {

        
        public string NameStrategy_test = string.Empty;
        public bool Apply;

        public Strategies()
        {

        }

        //////////////////////////////////////SITUATION SELECTION//////////////////////////////////////////
        /////////////////////////////////////                     //////////////////////////////////////////
        public float SitSele(double Co, double Ex)
        {

            /////////////////////////PERSONALITIES//////////////////////
            var Conscient = new LinguisticVariable("Conscient");
            var Extravers = new LinguisticVariable("Extravers");
            var Neurotici = new LinguisticVariable("Neurotici");
            var Openness  = new LinguisticVariable("Openness" );
            var Agreeable = new LinguisticVariable("Agreeable");

            var lowCon    = Conscient.MembershipFunctions.AddZShaped("lowCon"    , 30, 10, 0, 100);
            var middleCon = Conscient.MembershipFunctions.AddGaussian("middleCon", 50, 10, 0, 100);
            var highCon   = Conscient.MembershipFunctions.AddSShaped("highCon"   , 70, 10, 0, 100);

            var lowExt    = Extravers.MembershipFunctions.AddZShaped("lowExt"    , 30, 10, 0, 100);
            var middleExt = Extravers.MembershipFunctions.AddGaussian("middleExt", 50, 10, 0, 100);
            var highExt   = Extravers.MembershipFunctions.AddSShaped("highExt"   , 70, 10, 0, 100);


            /////////////////////////ESTRATEGY///////////////////////
            var SitSelec = new LinguisticVariable("SitSelec");
            var WeaApp = SitSelec.MembershipFunctions.AddZShaped("WeaApp" , 3, 1, 0, 10);
            var MidApp = SitSelec.MembershipFunctions.AddGaussian("MidApp", 5, 1, 0, 10);
            var StrApp = SitSelec.MembershipFunctions.AddSShaped("StrApp" , 7, 1, 0, 10);

            ////////////////////////////RULES/////////////////////////
            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            var rule1 = fuzzyEngine.Rules.If(Conscient.Is(highCon)  ).Then(SitSelec.Is(StrApp));
            var rule2 = fuzzyEngine.Rules.If(Conscient.Is(middleCon)).Then(SitSelec.Is(MidApp));
            var rule3 = fuzzyEngine.Rules.If(Conscient.Is(lowCon)   ).Then(SitSelec.Is(WeaApp));

            var rule4 = fuzzyEngine.Rules.If(Extravers.Is(highExt)  ).Then(SitSelec.Is(WeaApp));
            var rule5 = fuzzyEngine.Rules.If(Extravers.Is(middleExt)).Then(SitSelec.Is(MidApp));
            var rule6 = fuzzyEngine.Rules.If(Extravers.Is(lowExt)   ).Then(SitSelec.Is(StrApp));

            fuzzyEngine.Rules.Add(rule1, rule2, rule2, rule3, rule4, rule5, rule6);


            var SitSel_Res = Convert.ToSingle(fuzzyEngine.Defuzzify(new { Conscient = Co, Extravers = Ex }));

            return SitSel_Res;
        }

        //////////////////////////////////////SITUTATION MODIFICATION//////////////////////////////////////////
        /////////////////////////////////////                        //////////////////////////////////////////
        public float SitModi(double Co, double Ex, double Ne, double Ag)
        {


            /////////////////////////PERSONALITIES//////////////////////
            var Conscient = new LinguisticVariable("Conscient");
            var Extravers = new LinguisticVariable("Extravers");
            var Neurotici = new LinguisticVariable("Neurotici");
            var Openness  = new LinguisticVariable("Openness" );
            var Agreeable = new LinguisticVariable("Agreeable");



            var lowCon    = Conscient.MembershipFunctions.AddZShaped("lowCon"    , 30, 10, 0, 100);
            var middleCon = Conscient.MembershipFunctions.AddGaussian("middleCon", 50, 10, 0, 100);
            var highCon   = Conscient.MembershipFunctions.AddSShaped("highCon"   , 70, 10, 0, 100);

            var lowExt    = Extravers.MembershipFunctions.AddZShaped("lowExt"    , 30, 10, 0, 100);
            var middleExt = Extravers.MembershipFunctions.AddGaussian("middleExt", 50, 10, 0, 100);
            var highExt   = Extravers.MembershipFunctions.AddSShaped("highExt"   , 70, 10, 0, 100);

            var lowNeu    = Neurotici.MembershipFunctions.AddZShaped("lowNeu"    , 30, 10, 0, 100);
            var middleNeu = Neurotici.MembershipFunctions.AddGaussian("middleNeu", 50, 10, 0, 100);
            var highNeu   = Neurotici.MembershipFunctions.AddSShaped("highNeu"   , 70, 10, 0, 100);

            var lowAgr    = Agreeable.MembershipFunctions.AddZShaped("lowAgr"    , 30, 10, 0, 100);
            var middleAgr = Agreeable.MembershipFunctions.AddGaussian("middleAgr", 50, 10, 0, 100);
            var highAgr   = Agreeable.MembershipFunctions.AddSShaped("highAgr"   , 70, 10, 0, 100);

            /////////////////////////ESTRATEGY///////////////////////
            var SitMod = new LinguisticVariable("SitMod");
            var WeaApp = SitMod.MembershipFunctions.AddZShaped("WeaApp" , 3, 1, 0, 10);
            var MidApp = SitMod.MembershipFunctions.AddGaussian("MidApp", 5, 1, 0, 10);
            var StrApp = SitMod.MembershipFunctions.AddSShaped("StrApp" , 7, 1, 0, 10);

            //////////////////////////RULES///////////////////////////
            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            var rule1 = fuzzyEngine.Rules.If(Conscient.Is(highCon).Or(Extravers.Is(highExt))    ).Then(SitMod.Is(StrApp));
            var rule2 = fuzzyEngine.Rules.If(Conscient.Is(middleCon).Or(Extravers.Is(middleExt))).Then(SitMod.Is(MidApp));
            var rule3 = fuzzyEngine.Rules.If(Conscient.Is(lowCon).Or(Extravers.Is(lowExt))      ).Then(SitMod.Is(WeaApp));

            var rule4 = fuzzyEngine.Rules.If(Neurotici.Is(highNeu).Or(Agreeable.Is(highAgr))    ).Then(SitMod.Is(WeaApp));
            var rule5 = fuzzyEngine.Rules.If(Neurotici.Is(middleNeu).Or(Agreeable.Is(middleAgr))).Then(SitMod.Is(MidApp));
            var rule6 = fuzzyEngine.Rules.If(Neurotici.Is(lowNeu).Or(Agreeable.Is(lowAgr))      ).Then(SitMod.Is(StrApp));


            fuzzyEngine.Rules.Add(rule1, rule2, rule3, rule4, rule5, rule6);


            var SitMod_Res = Convert.ToSingle(fuzzyEngine.Defuzzify(new { Conscient = Co, Extravers = Ex, Neurotici = Ne, Agreeable = Ag }));

            return SitMod_Res;
        }


        //////////////////////////////////////ATTENTION DEPLOYMENT//////////////////////////////////////////
        /////////////////////////////////////                     //////////////////////////////////////////
        public float Atten_Deploy(double Co, double Op, double Ne)
        {

            /////////////////////////PERSONALITIES//////////////////////
            var Conscient = new LinguisticVariable("Conscient");
            var Extravers = new LinguisticVariable("Extravers");
            var Neurotici = new LinguisticVariable("Neurotici");
            var Openness  = new LinguisticVariable("Openness" );
            var Agreeable = new LinguisticVariable("Agreeable");

            var lowCon    = Conscient.MembershipFunctions.AddZShaped("lowCon"    , 30, 10, 0, 100);
            var middleCon = Conscient.MembershipFunctions.AddGaussian("middleCon", 50, 10, 0, 100);
            var highCon   = Conscient.MembershipFunctions.AddSShaped("highCon"   , 70, 10, 0, 100);
             
            var lowNeu    = Neurotici.MembershipFunctions.AddZShaped("lowNeu"    , 30, 10, 0, 100);
            var middleNeu = Neurotici.MembershipFunctions.AddGaussian("middleNeu", 50, 10, 0, 100);
            var highNeu   = Neurotici.MembershipFunctions.AddSShaped("highNeu"   , 70, 10, 0, 100);

            var lowOpe    = Openness.MembershipFunctions.AddZShaped("lowOpe"    , 30, 10, 0, 100);
            var middleOpe = Openness.MembershipFunctions.AddGaussian("middleOpe", 50, 10, 0, 100);
            var highOpe   = Openness.MembershipFunctions.AddSShaped("highOpe"   , 70, 10, 0, 100);


            /////////////////////////ESTRATEGY///////////////////////
            var AttenDepoly = new LinguisticVariable("AttenDepoly");
            var WeaApp = AttenDepoly.MembershipFunctions.AddZShaped("WeaApp" , 3, 1, 0, 10);
            var MidApp = AttenDepoly.MembershipFunctions.AddGaussian("MidApp", 5, 1, 0, 10);
            var StrApp = AttenDepoly.MembershipFunctions.AddSShaped("StrApp" , 7, 1, 0, 10);

            /////////////////////////////RULES////////////////////////
            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            var rule1 = fuzzyEngine.Rules.If(Conscient.Is(highCon).Or(Openness.Is(highOpe))    ).Then(AttenDepoly.Is(StrApp));
            var rule2 = fuzzyEngine.Rules.If(Conscient.Is(middleCon).Or(Openness.Is(middleOpe))).Then(AttenDepoly.Is(MidApp));
            var rule3 = fuzzyEngine.Rules.If(Conscient.Is(lowCon).Or(Openness.Is(lowOpe))      ).Then(AttenDepoly.Is(WeaApp));

            var rule4 = fuzzyEngine.Rules.If(Neurotici.Is(highNeu)  ).Then(AttenDepoly.Is(WeaApp));
            var rule5 = fuzzyEngine.Rules.If(Neurotici.Is(middleNeu)).Then(AttenDepoly.Is(MidApp));
            var rule6 = fuzzyEngine.Rules.If(Neurotici.Is(lowNeu)   ).Then(AttenDepoly.Is(StrApp));

            fuzzyEngine.Rules.Add(rule1, rule2, rule2, rule3, rule4, rule5, rule6);


            var AttenDeploy_Res = Convert.ToSingle(fuzzyEngine.Defuzzify(new { Conscient = Co, Openness = Op, Neurotici = Ne }));

            return AttenDeploy_Res;
        }

        //////////////////////////////////////COGNITIVE CHANGE///////////////////////////////////////////
        /////////////////////////////////////                ///////////////////////////////////////////
        public float CognChange(double Ne, double Op)
        {

            /////////////////////////PERSONALITIES//////////////////////
            var Conscient = new LinguisticVariable("Conscient");
            var Extravers = new LinguisticVariable("Extravers");
            var Neurotici = new LinguisticVariable("Neurotici");
            var Openness  = new LinguisticVariable("Openness" );
            var Agreeable = new LinguisticVariable("Agreeable");

            var lowNeu    = Neurotici.MembershipFunctions.AddZShaped("lowNeu"    , 30, 10, 0, 100);
            var middleNeu = Neurotici.MembershipFunctions.AddGaussian("middleNeu", 50, 10, 0, 100);
            var highNeu   = Neurotici.MembershipFunctions.AddSShaped("highNeu"   , 70, 10, 0, 100);

            var lowOpe    = Openness.MembershipFunctions.AddZShaped("lowOpe"    , 30, 10, 0, 100);
            var middleOpe = Openness.MembershipFunctions.AddGaussian("middleOpe", 50, 10, 0, 100);
            var highOpe   = Openness.MembershipFunctions.AddSShaped("highOpe"   , 70, 10, 0, 100);



            /////////////////////////ESTRATEGY////////////////////////////
            var CognChange = new LinguisticVariable("CognChange");
            var WeaApp = CognChange.MembershipFunctions.AddZShaped("WeaApp" , 3, 1, 0, 10);
            var MidApp = CognChange.MembershipFunctions.AddGaussian("MidApp", 5, 1, 0, 10);
            var StrApp = CognChange.MembershipFunctions.AddSShaped("StrApp" , 7, 1, 0, 10);

            ///////////////////////////RULES////////////////////////////
            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            var rule1 = fuzzyEngine.Rules.If(Neurotici.Is(highNeu)  ).Then(CognChange.Is(WeaApp));
            var rule2 = fuzzyEngine.Rules.If(Neurotici.Is(middleNeu)).Then(CognChange.Is(MidApp));
            var rule3 = fuzzyEngine.Rules.If(Neurotici.Is(lowNeu)   ).Then(CognChange.Is(StrApp));

            var rule4 = fuzzyEngine.Rules.If(Openness.Is(highOpe)  ).Then(CognChange.Is(StrApp));
            var rule5 = fuzzyEngine.Rules.If(Openness.Is(middleOpe)).Then(CognChange.Is(MidApp));
            var rule6 = fuzzyEngine.Rules.If(Openness.Is(lowOpe)   ).Then(CognChange.Is(WeaApp));

            fuzzyEngine.Rules.Add(rule1, rule2, rule2, rule3, rule4, rule5, rule6);


            var CognChange_Res = Convert.ToSingle(fuzzyEngine.Defuzzify(new { Neurotici = Ne, Openness = Op }));

            return CognChange_Res;
        }

        //////////////////////////////////////RESPONSE MODULATION//////////////////////////////////////////
        /////////////////////////////////////                     ////////////////////////////////////////
        public float RespModula(double Op, double Ex)
        {

            /////////////////////////PERSONALITIES//////////////////////
            var Conscient = new LinguisticVariable("Conscient");
            var Extravers = new LinguisticVariable("Extravers");
            var Neurotici = new LinguisticVariable("Neurotici");
            var Openness  = new LinguisticVariable("Openness" );
            var Agreeable = new LinguisticVariable("Agreeable");

            var lowExt    = Extravers.MembershipFunctions.AddZShaped("lowExt"    , 30, 10, 0, 100);
            var middleExt = Extravers.MembershipFunctions.AddGaussian("middleExt", 50, 10, 0, 100);
            var highExt   = Extravers.MembershipFunctions.AddSShaped("highExt"   , 70, 10, 0, 100);

            var lowOpe    = Openness.MembershipFunctions.AddZShaped("lowOpe"    , 30, 10, 0, 100);
            var middleOpe = Openness.MembershipFunctions.AddGaussian("middleOpe", 50, 10, 0, 100);
            var highOpe   = Openness.MembershipFunctions.AddSShaped("highOpe"   , 70, 10, 0, 100);


            /////////////////////////ESTRATEGY///////////////////////
            var RespModula = new LinguisticVariable("SitSelec");
            var WeaApp = RespModula.MembershipFunctions.AddZShaped("WeaApp" , 3, 1, 0, 10);
            var MidApp = RespModula.MembershipFunctions.AddGaussian("MidApp", 5, 1, 0, 10);
            var StrApp = RespModula.MembershipFunctions.AddSShaped("StrApp" , 7, 1, 0, 10);

            ////////////////////////////RULES/////////////////////////
            IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

            var rule1 = fuzzyEngine.Rules.If(Openness.Is(highOpe).Or(Extravers.Is(highExt))    ).Then(RespModula.Is(WeaApp));
            var rule2 = fuzzyEngine.Rules.If(Openness.Is(middleOpe).Or(Extravers.Is(middleExt))).Then(RespModula.Is(WeaApp));
            var rule3 = fuzzyEngine.Rules.If(Openness.Is(lowOpe).Or(Extravers.Is(lowExt))      ).Then(RespModula.Is(MidApp));

            fuzzyEngine.Rules.Add(rule1, rule2, rule2, rule3);


            float RespModu_Res = Convert.ToSingle(fuzzyEngine.Defuzzify(new { Openness = Op, Extravers = Ex }));

            return RespModu_Res;
        }

        public Tuple<string, float> Personality (float Consientioness, float Extraversion, float Neouroticim, float Agreeable, float Opennes)
        {
            float Situation_Selection    = SitSele(Consientioness, Extraversion);
            float Situation_Modification = SitModi(Consientioness, Extraversion, Neouroticim, Agreeable);
            float Attention_Deployment   = Atten_Deploy(Consientioness, Opennes, Neouroticim);
            float Cognitive_Change       = CognChange(Neouroticim, Opennes);
            float Response_Modulation    = RespModula(Opennes, Extraversion);

            Dictionary<string, float> Strategies = new Dictionary<string, float>()
            {
                 { "Situation Selection" , Situation_Selection  },{ "Situation Modification", Situation_Modification },
                 { "Attention Deployment", Attention_Deployment },{ "Cognitive Change", Cognitive_Change },
                 { "Response Modulation" , Response_Modulation  }
            };

            var sortedDict = from entry in Strategies orderby entry.Value ascending select entry;
            var a =  Strategies.Keys;

             /*
            var myList = Strategies.ToList();

            foreach(var item in sortedDict)
            {
                Console.WriteLine("LISTA STRA ------> " + item);
            }
            */
            string NameStrategy = a.First();
            float value;
            Strategies.TryGetValue(NameStrategy, out value);

            float value_test; 
            Strategies.TryGetValue("Situation Selection", out value_test);
            string NameStrategy_test = "Situation Selection";


            return Tuple.Create(NameStrategy_test, value_test);





        }


        public void Personality_test(float Consientioness, float Extraversion)
        {
            float Situation_Selection = SitSele(Consientioness, Extraversion);
            
            if (Situation_Selection > 4.5)
            {
                NameStrategy_test = "Situation Selection is applied";
                Apply = true;

            }
            else
            {
                NameStrategy_test = "Situation Selection isn't applied";
                Apply = false;

            }
    
         return;



        }


    }
}

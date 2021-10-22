using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fuzzy_Personalities;
using KnowledgeBase;


namespace TestEmotion
{
    class AppliedStrategies
    {
        public struct ValueEst
        {
            public string StrategyName;
            public bool StrategyApplied;
            
        }

        public static Strategies strategies = new();
        public static ValueEst SelectStrategy(float Consientioness, float Extraversion,float Neuroticism, float Openness, float Agreeableness)
        {
            Console.WriteLine("\n-----------------------StrategyTest--------------------------");

            ValueEst valueEst = new();

            switch ((Consientioness, Extraversion, Neuroticism, Openness, Agreeableness))
            {

                case (>= 0, >= 0, >= 0, >=0, >= 0) when (Neuroticism == 0)&&( Openness == 0)&&(Agreeableness == 0):
                       
                    float Situation_Selection = strategies.SitSele(Consientioness, Extraversion);

                    if (Situation_Selection > 4.5)
                    {
                        valueEst.StrategyName = "Situation Selection";
                        valueEst.StrategyApplied = true;
                    }
                    else
                    {
                        valueEst.StrategyName = "Situation Selection isn't applied";
                        valueEst.StrategyApplied = false;
                    }
                    break;

                case ( >= 0, >= 0, >= 0, >= 0, >= 0) when (Openness == 0):
                                        
                    float Situation_Modification = strategies.SitModi(Consientioness, Extraversion, Neuroticism, Agreeableness);

                    if (Situation_Modification > 4.5)
                    {
                        valueEst.StrategyName = "Situation Modification";
                        valueEst.StrategyApplied = true;
                    }
                    else
                    {
                        valueEst.StrategyName = "Situation Modification isn't applied";
                        valueEst.StrategyApplied = false;

                    }
                    break;


            }

            return valueEst;
        }
    }
}

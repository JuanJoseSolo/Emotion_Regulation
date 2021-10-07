using System;

namespace Fuzzy_Personalities
{
    class FuzzyMain : Strategies
    {
        static void Main(string[] args)
        {
            var _Personalities = new Strategies();

            float Cons = 90, Extrav = 30;
            _Personalities.Personality_test(Cons, Extrav);

            Console.WriteLine("\n Variable Avoid------>> " + _Personalities.Apply + "\n Strategy---->> "
                                                           + _Personalities.NameStrategy_test);


            Console.WriteLine("\n Valor---->>" + _Personalities.SitSele(Cons, Extrav));



            /*
            Strategies _Personalities = new Strategies();
            float Cons = 0, Extrav = 90, Neuro = 0, Openn = 0, Agree = 0;
            _Personalities.Personality(Cons, Extrav);
            //string nameStrategy = _Personalities.Personality(Cons, Extrav, Neuro, Openn, Agree).Item1;
            //float valueStrategy = _Personalities.Personality(Cons, Extrav, Neuro, Openn, Agree).Item2;

            var Situation_Selection = _Personalities.SitSele(Cons, Extrav);
            var Situation_Modification = _Personalities.SitModi(Cons, Extrav, Neuro, Agree);
            var Attention_Deployment = _Personalities.Atten_Deploy(Cons, Openn, Neuro);
            var Cognitive_Change = _Personalities.CognChange(Neuro, Openn);
            var Response_Modulation = _Personalities.RespModula(Openn, Extrav);
            Console.WriteLine("\n   Situation Selection : " + Situation_Selection);
            Console.WriteLine("\n   Situation Modification : " + Situation_Modification);
            Console.WriteLine("\n   Attention Deployment : " + Attention_Deployment);
            Console.WriteLine("\n   Cognitive Change : " + Cognitive_Change);
            Console.WriteLine("\n   Response Modulation : " + Response_Modulation);
            Console.ReadKey();
            */


        }

    }


}

/*Emotion01.Character();
 * //float Cons=35, Extrav=80, Neuro=10, Openn=7, Agree= 35;
string EmotionPedro  = Emotion01.Emotion_Pedro;
float IntensityPedro = Emotion01.Intensity_Pedro;
float MoodPedro      = Emotion01.Mood_Pedro;


Console.WriteLine("\n <-----------------------------------------------------> ");
Console.WriteLine("\n Emotion Pedro----> " + EmotionPedro + " \n Intensity--------> " + IntensityPedro + 
                                                                 " \n Mood-------------> " + MoodPedro);


var Situation_Selection    = FuzzEstr.SitSele(              Cons, Extrav);
var Situation_Modification = FuzzEstr.SitModi(Cons, Extrav, Neuro, Agree);
var Attention_Deployment   = FuzzEstr.Atten_Deploy(   Cons, Openn, Neuro);
var Cognitive_Change       = FuzzEstr.CognChange(           Neuro, Openn);
var Response_Modulation    = FuzzEstr.RespModula(          Openn, Extrav);



//FuzzyMain.Water();
Console.WriteLine("\n   Situation Selection : "    +    Situation_Selection);
Console.WriteLine("\n   Situation Modification : " + Situation_Modification);
Console.WriteLine("\n   Attention Deployment : "   +   Attention_Deployment);
Console.WriteLine("\n   Cognitive Change : "       +       Cognitive_Change);
Console.WriteLine("\n   Response Modulation : "    +    Response_Modulation);
Console.ReadKey();
*/
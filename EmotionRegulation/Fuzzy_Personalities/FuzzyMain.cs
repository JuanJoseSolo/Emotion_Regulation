using System;

namespace Fuzzy_Personalities
{
    class FuzzyMain : Strategies
    {
        public static void Main(string[] args)
        {

            Strategies _Personalities = new Strategies();
            _Personalities.Personality(15, 20, 0, 0, 15);
                

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
using System;
using RolePlayCharacter;
using System.Linq;
using WellFormedNames;
using System.IO;
using GAIPS.Rage;
using EmotionalDecisionMaking;
using ActionLibrary.DTOs;
using KnowledgeBase;
using EmotionalAppraisal;


namespace RolePlayCharacterTutorial
{
    class Tests
    {

        public static void RolCharacter()
        {

            //AssetStorage
            /// var storage = AssetStorage.FromJson(File.ReadAllText("../../../../Examples/AssetStorage.json"));
            //Loading the asset
            var rpc = new RolePlayCharacterAsset();
            //rpc.LoadAssociatedAssets(storage
            //
            var kbb = rpc.CharacterName;

            Console.WriteLine("KBB" + kbb);
            rpc.ActivateIdentity(new Identity((Name)"Portuguese", (Name)"Culture", 1));
            Console.WriteLine("Starting Mood: " + rpc.Mood);
            var actions = rpc.Decide();
            var action = actions.FirstOrDefault();

            rpc.Update();

            Console.WriteLine("The name of the character loaded is: " + rpc.CharacterName);
            // Console.WriteLine("The following event was perceived: " + event1);
            Console.WriteLine("Mood after event: " + rpc.Mood);
            Console.WriteLine("Strongest emotion: " + rpc.GetStrongestActiveEmotion()?.EmotionType + "-" + rpc.GetStrongestActiveEmotion()?.Intensity);
            Console.WriteLine("First Response: " + action?.Name + ", Target:" + action?.Target.ToString());

            var busyAction = rpc.Decide().FirstOrDefault();

            Console.WriteLine("Second Response: " + busyAction?.Name + ", Target:" + action?.Target.ToString());

            var event3 = EventHelper.ActionEnd(rpc.CharacterName.ToString(), action?.Name.ToString(), "Player");

            rpc.Perceive(new[] { event3 });
            action = rpc.Decide().FirstOrDefault();

            Console.WriteLine("Third Response: " + action?.Name + ", Target:" + action?.Target.ToString());


            int x = 0;
            while (true)
            {

                Console.WriteLine("Mood after tick: " + rpc.Mood + " x: " + x + " tick: " + rpc.Tick);
                Console.WriteLine("Strongest emotion: " + rpc.GetStrongestActiveEmotion()?.EmotionType + "-" + rpc.GetStrongestActiveEmotion()?.Intensity);
                rpc.Update();
                Console.ReadLine();

                if (x == 10)
                {
                    var event1 = EventHelper.ActionEnd("Player", "Kick", rpc.CharacterName.ToString());

                    rpc.Perceive(new[] { event1 });
                    action = rpc.Decide().FirstOrDefault();
                    rpc.Update();
                }


                if (x == 11)
                {
                    rpc.ResetEmotionalState();
                }
                if (x == 25)
                {
                    var event1 = EventHelper.ActionEnd("Player", "Kick", rpc.CharacterName.ToString());

                    rpc.Perceive(new[] { event1 });
                    action = rpc.Decide().FirstOrDefault();
                    rpc.Update();
                }


                else if (x == 30)
                {
                    Console.WriteLine("Reloading " + rpc.GetStrongestActiveEmotion().Intensity + " " + rpc.GetStrongestActiveEmotion().EmotionType + " mood: " + rpc.Mood);

                    Console.WriteLine("Reloading result: " + rpc.GetStrongestActiveEmotion().Intensity + " " + rpc.GetStrongestActiveEmotion().EmotionType + " mood: " + rpc.Mood);

                }

                x++;



            }



        }

        public static void EmotionalDecision()
        {
            //First we construct a new instance of the EmotionalDecisionMakingAsset class
            var edm = new EmotionalDecisionMakingAsset();

            //We have to register an existing knowledge base 
            var kb_Pedro = new KB((Name)"Pedro");
            kb_Pedro.Tell((Name)"Likes(Maria)", (Name)"True");
            edm.RegisterKnowledgeBase(kb_Pedro);

            //create an action rule
            var actionRule = new ActionRuleDTO 
            { 
                Action = Name.BuildName("ToHug"), Priority = Name.BuildName("4"), Target = (Name)"Sarah" 
            };
            var id = edm.AddActionRule(actionRule);


            var rule = edm.GetActionRule(id);
            edm.AddRuleCondition(id, "Likes(Maria) = True");
            var actions = edm.Decide(Name.UNIVERSAL_SYMBOL);

            Console.WriteLine("RULE >>> "+rule.Action);


            Console.WriteLine("Decisions: ");
            foreach (var a in actions)
            {
                Console.WriteLine(a.Name.ToString() + " To: " + a.Target);
            }

        }


        static void Main(string[] args)
        {
            EmotionalDecision();
        }
    }
}

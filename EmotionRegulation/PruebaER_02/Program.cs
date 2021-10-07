using System;
using GAIPS.Rage;
using System.Linq;
using System.IO;
using IntegratedAuthoringTool;
using RolePlayCharacter;
using WorldModel;


namespace PruebaER_02
{
    class Program
    {


        // Store the iat file
        private IntegratedAuthoringToolAsset _iat;

        //Store the characters
        private RolePlayCharacterAsset _rpcList;

        //Store the World Model
        private WorldModelAsset _worldModel;



        static void Main(string[] args)
        {


        }
        /*
        void Start()
        {
            var storage = AssetStorage.FromJson(File.ReadAllText(
            @"C:\Users\JuanJoseAsus\source\repos\FAtiMA-Toolkit-master\EmotionRegulation\ReglasRobo.json"));

            //C: \Users\JuanJoseAsus\source\repos\FAtiMA - Toolkit - master\EmotionRegulation

            //Loading the asset
            var _iat = IntegratedAuthoringToolAsset.FromJson(File.ReadAllText(
           @"C:\Users\JuanJoseAsus\source\repos\FAtiMA-Toolkit-master\EmotionRegulation\EscenarioRobo.json"), storage);

            //Initialize the List
            _rpcList = new RolePlayCharacterAsset();
            
            foreach (var characterSouce in _iat.GetAllDialogueActions())
            {

                var rpc = RolePlayCharacterAsset.

                // RPC must load its "sub-assets"
                rpc.LoadAssociatedAssets();

                // Iat lets the RPC know all the existing Meta-Beliefs / Dynamic Properties
                _iat.BindToRegistry(rpc.DynamicPropertiesRegistry);

                // A debug message to make sure we are correctly loading the characters
                Debug.Log("Loaded Character " + rpc.CharacterName);

                _rpcList.Add(rpc);
            }


            //Loading the WorldModel
            _worldModel = WorldModelAsset.LoadFromFile(_iat.m_worldModelSource.Source);
        

        }


        void Update()
        {
            //a simple cycle to go through all the agents and get their decision
            foreach (var rpc in _rpcList)
            {

                // From all the decisions the rpc wants to perform we want the first one
                var decision = rpc.Decide().FirstOrDefault();

                if (decision != null)
                {

                    agentName = rpc.CharacterName.ToString();
                    finalDecision = decision;
                    break;
                }

            }

            //If there was a decision I want to print it
            if (finalDecision != null)
                Debug.Log(" The agent " + agentName + " decided to perform " + finalDecision.Name);

        }
        */
    }
}

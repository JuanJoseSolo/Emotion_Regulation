using System;
using Tests.EmotionalAppraisal;

namespace TESTofTEST
{
    class Program
    {

        
        static void Main(string[] args)
        {
            Tests.EmotionalAppraisal.EAAssetTests aset = new EAAssetTests();
            aset.Test_EA_RemoveAppraisalRules();
        }
    }
}

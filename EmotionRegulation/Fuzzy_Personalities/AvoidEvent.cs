using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conditions.DTOs;
using WellFormedNames;
using ActionLibrary.DTOs;

namespace Fuzzy_Personalities
{
    public class AvoidEvent : ActionRuleDTO
    {

        public ConditionSetDTO Aviod { get; set; }

    }
}

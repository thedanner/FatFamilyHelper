using System.Collections.Generic;

namespace FatFamilyHelper.SourceQuery.Rules
{
    public class NoopRuleParser : IRuleParser<Dictionary<string, string>>
    {
        public Dictionary<string, string> FromDictionary(Dictionary<string, string> rawRules) => rawRules;
    }
}

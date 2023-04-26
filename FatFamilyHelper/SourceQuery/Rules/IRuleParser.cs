using System.Collections.Generic;

namespace FatFamilyHelper.SourceQuery.Rules
{
    public interface IRuleParser<T>
    {
        T FromDictionary(Dictionary<string, string> rawRules);
    }
}

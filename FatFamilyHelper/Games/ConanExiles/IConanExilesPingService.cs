using FatFamilyHelper.SourceQuery;
using FatFamilyHelper.SourceQuery.Rules;
using System.Threading.Tasks;

namespace FatFamilyHelper.Games.ConanExiles;

public interface IConanExilesPingService
{
    Task<GameServer<ConanExilesRules>?> PingAsync(string hostname, ushort queryPort);
}

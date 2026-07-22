using SovereigntyBot.Modules.Results;
using SovereigntyBot.Services.Endpoints.Cache;

namespace SovereigntyBot.Services
{
    public class CacheService
    {
        // TODO: Upgrade to cache other data where needed.
        private Dictionary<ulong, Cache> _cacheData = new Dictionary<ulong, Cache>();
        public void Save(ulong id, Cache cache)
        {
            _cacheData[id] = cache;
        }

        public Cache Load(ulong id)
        {
            return _cacheData[id];
        }

        public void Delete(ulong id)
        {  
            _cacheData.Remove(id);
        }

        public void Reset()
        {
            _cacheData = new Dictionary<ulong, Cache>();
        }
    }
}
using OverkillDocs.Core.Constants;
using OverkillDocs.Core.Entities.Identity;
using static OverkillDocs.Core.Security.UserContext;

namespace OverkillDocs.Infrastructure.Cache
{
    public abstract class ObjectCache<T>
    {
        protected static readonly TimeSpan expirationTime = typeof(T) switch
        {
            _ => CacheConstants.DefaultObjectExpiration,
        };

        protected static string KeyOf(T value) => value switch
        {
            UserIdentity v => KeyFrom(v.Token),
            User v => KeyFrom(v.Id),
            _ => throw new InvalidOperationException("Tipo não mapeado para criação de chave")
        };

        protected static string KeyFrom(string id)
        {
            var name = typeof(T).Name;
            return $"{name}:{id}";
        }

        protected static string KeyFrom(int id) => KeyFrom(id.ToString());
    }
}

using OverkillDocs.Core.Constants;
using OverkillDocs.Core.DTOs.Document;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Infrastructure.CachedResults;
using System.Text.Json;
using static OverkillDocs.Core.Security.UserContext;

namespace OverkillDocs.Infrastructure.Cache;

internal abstract class ObjectCache<T>
{
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected static readonly TimeSpan expirationTime = typeof(T) switch
    {
        Type t when t == typeof(DocumentFragmentLockDto) => CacheConstants.DocumentFragmentLockExpiration,
        _ => CacheConstants.DefaultObjectExpiration,
    };

    protected static string KeyOf(T value) => value switch
    {
        DocumentFragmentHashIdsResult v => KeyFrom(v.DocumentId),
        DocumentFragmentIdsResult v => KeyFrom(v.DocumentId),
        DocumentFragmentLockDto v => KeyFrom(v.FragmentHashId),
        DocumentSummariesResult => KeyFrom(DocumentSummariesResult.DefaultId),
        UserIdentity v => KeyFrom(v.Token),
        User v => KeyFrom(v.Id),
        _ => throw new InvalidOperationException($"Tipo {typeof(T).Name} não mapeado para criação de chave")
    };

    protected static string KeyFrom(string id)
    {
        var name = typeof(T).Name;
        return $"{name}:{id}";
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Marcar membros como estáticos", Justification = "Método acessado por instância nos testes")]
    public string IdFrom(T value)
    {
        return KeyOf(value).Split(':', 2).Last();
    }

    protected static string KeyFrom(int id) => KeyFrom(id.ToString());

    protected static string EntityToJson(T value)
    {
        return JsonSerializer.Serialize(value, jsonOptions);
    }

    protected static T? JsonToEntity(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return default;
        return JsonSerializer.Deserialize<T>(json, jsonOptions);
    }
}

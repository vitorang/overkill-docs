using OverkillDocs.Core.DTOs.Users;
using OverkillDocs.Core.States;
using static OverkillDocs.Core.Security.UserContext;

namespace OverkillDocs.Core.Interfaces
{
    public interface IAppCache<T>
    {
        public Task<T?> Get(string key);
        public Task Set(T value);
        public Task Remove(T value);
    }

    public interface IDocumentStateCache : IAppCache<DocumentState>;
    public interface IEditorStateCache : IAppCache<EditorState>;
    public interface ISimpleUserCache : IAppCache<SimpleUserDto>;
    public interface IUserIdentityCache : IAppCache<UserIdentity>;
}

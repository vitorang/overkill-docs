using OverkillDocs.Core.Interfaces;
using OverkillDocs.Core.States;

namespace OverkillDocs.Infrastructure.Cache
{
    public partial class AppCache
    {
        Task<DocumentState?> IAppCache<DocumentState>.Get(string id) => GetValue<DocumentState>(id);
        Task IAppCache<DocumentState>.Set(DocumentState value) => SetValue(value);
        Task IAppCache<DocumentState>.Remove(DocumentState value) => RemoveValue(value);

        Task<EditorState?> IAppCache<EditorState>.Get(string id) => GetValue<EditorState>(id);
        Task IAppCache<EditorState>.Set(EditorState value) => SetValue(value);
        Task IAppCache<EditorState>.Remove(EditorState value) => RemoveValue(value);
    }
}

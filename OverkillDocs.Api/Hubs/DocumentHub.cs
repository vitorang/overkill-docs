using HashidsNet;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using OverkillDocs.Core.Extensions;
using OverkillDocs.Core.Security;
using OverkillDocs.Core.States;

namespace OverkillDocs.Api.Hubs
{
    public partial class DocumentHub(IMemoryCache cache, UserContext userContext, Hashids hashids) : MainHub
    {
        private const string editorPrefix = "editor:";
        private const string documentPrefix = "document:";

        public async Task JoinDocument(string documentHashId)
        {
            var editor = GetCurrentEditor();
            editor.DocumentHashId = documentHashId;

            var editorKey = $"{editorPrefix}{editor.EditorId}";
            SetCache(editorKey, editor);

            var documentKey = $"{documentPrefix}{documentHashId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, documentKey);
        }

        public async Task LeaveDocument(string documentHashId)
        {
            var key = $"{documentPrefix}{documentHashId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, key);
            await RemoveCurrentEditor();
        }

        public async Task LockFragment(string documentHashId, string fragmentHashId)
        {
            var (document, documentKey) = FragmentLockAction(documentHashId, fragmentHashId, (editor, document) =>
            {
                editor.FragmentHashId = fragmentHashId;
                document.FragmentEdits.Add(new(editor.EditorId, editor.UserHashId, fragmentHashId));
            });

            await Clients.Group(documentKey).SendAsync(nameof(DocumentState), document.ToDto());
        }

        public async Task UnlockFragment(string documentHashId, string fragmentHashId)
        {
            var (document, documentKey) = FragmentLockAction(documentHashId, fragmentHashId, (editor, document) =>
            {
                editor.FragmentHashId = null;
                document.FragmentEdits.Remove(new(editor.EditorId, editor.UserHashId, fragmentHashId));
            });

            await Clients.Group(documentKey).SendAsync(nameof(DocumentState), document.ToDto());
        }

        private (DocumentState, string) FragmentLockAction(string documentHashId, string fragmentHashId, Action<EditorState, DocumentState> action)
        {
            var editor = GetCurrentEditor();
            var editorKey = $"{editorPrefix}{editor.EditorId}";

            var document = GetDocument(documentHashId);
            var documentKey = $"{documentPrefix}{document.DocumentHashId}";

            action(editor, document);

            SetCache(editorKey, editor);
            SetCache(documentKey, document);

            return (document, documentKey);
        }

        private EditorState GetCurrentEditor()
        {
            var key = $"{editorPrefix}{Context.ConnectionId}";
            var state = cache.Get<EditorState>(key) ?? new EditorState
            {
                ConnectionId = Context.ConnectionId,
                UserHashId = hashids.Encode(userContext.UserId),
            };

            SetCache(key, state);
            return state;
        }

        private DocumentState GetDocument(string documentHashId)
        {
            var key = $"{documentPrefix}{documentHashId}";
            var state = cache.Get<DocumentState>(key) ?? new DocumentState
            {
                DocumentHashId = documentHashId
            };

            SetCache(key, state);
            return state;
        }

        protected override async Task RemoveCurrentEditor()
        {
            var editor = GetCurrentEditor();
            if (editor.DocumentHashId != null && editor.FragmentHashId != null)
                await UnlockFragment(editor.DocumentHashId, editor.FragmentHashId);

            var editorKey = $"{editorPrefix}{editor.EditorId}";
            cache.Remove(editorKey);
        }

        private void SetCache(string key, object value)
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
              .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            cache.Set(key, value, options);
        }
    }
}

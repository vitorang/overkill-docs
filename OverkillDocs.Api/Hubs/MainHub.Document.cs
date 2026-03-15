using Microsoft.AspNetCore.SignalR;
using OverkillDocs.Api.Constants;
using OverkillDocs.Core.Extensions;
using OverkillDocs.Core.States;

namespace OverkillDocs.Api.Hubs
{
    public partial class MainHub
    {
        private const string editorPrefix = "editor:";
        private const string documentPrefix = "document:";

        public async Task JoinDocument(string documentHashId)
        {
            var editor = await GetCurrentEditor();
            editor.DocumentHashId = documentHashId;
            editor.FragmentHashId = null;

            await editorStateCache.Set(editor, default);

            var group = $"{documentPrefix}{documentHashId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        public async Task LeaveDocument(string documentHashId)
        {
            var group = $"{documentPrefix}{documentHashId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
            await RemoveCurrentEditor();
        }

        public async Task LockFragment(string documentHashId, string fragmentHashId)
        {
            await FragmentLockAction(documentHashId, fragmentHashId, (editor, document) =>
            {
                editor.FragmentHashId = fragmentHashId;
                document.FragmentEdits.Add(new(editor.EditorId, editor.UserHashId, fragmentHashId));
            });
        }

        public async Task UnlockFragment(string documentHashId, string fragmentHashId)
        {
            await FragmentLockAction(documentHashId, fragmentHashId, (editor, document) =>
            {
                editor.FragmentHashId = null;
                document.FragmentEdits.Remove(new(editor.EditorId, editor.UserHashId, fragmentHashId));
            });
        }

        private async Task FragmentLockAction(string documentHashId, string fragmentHashId, Action<EditorState, DocumentState> action)
        {
            var editor = await GetCurrentEditor();
            var document = await GetDocument(documentHashId);

            action(editor, document);

            await Task.WhenAll(
                editorStateCache.Set(editor, default),
                documentStateCache.Set(document, default)
            );

            var documentGroup = $"{documentPrefix}{document.DocumentHashId}";
            await Clients.Group(documentGroup).SendAsync(HubEvents.Document.FragmentLockChanged, document.ToDto());
        }

        private async Task<EditorState> GetCurrentEditor()
        {
            var editorId = Context.ConnectionId;
            var state = await editorStateCache.Get(editorId, default) ?? new EditorState
                {
                    ConnectionId = editorId,
                    UserHashId = hashids.Encode(userContext.UserId),
                };

            return state;
        }

        private async Task<DocumentState> GetDocument(string documentHashId)
        {
            var state = await documentStateCache.Get(documentHashId, default) ?? new DocumentState
            {
                DocumentHashId = documentHashId
            };

            return state;
        }

        private async Task RemoveCurrentEditor()
        {
            var editor = await GetCurrentEditor();

            if (editor.DocumentHashId != null && editor.FragmentHashId != null)
                await UnlockFragment(editor.DocumentHashId, editor.FragmentHashId);

            await editorStateCache.Remove(editor, default);
        }
    }
}

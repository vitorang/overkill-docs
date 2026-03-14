using OverkillDocs.Core.DTOs.Document;
using OverkillDocs.Core.States;

namespace OverkillDocs.Core.Extensions
{
    public static class DocumentStateDtoExtensions
    {
        public static DocumentStateDto ToDto(this DocumentState state)
        {
            return new DocumentStateDto
            (
                FragmentUserMap: state.FragmentEdits.ToDictionary(e => e.FragmentHashId, e => e.UserHashId)
            );
        }
    }
}

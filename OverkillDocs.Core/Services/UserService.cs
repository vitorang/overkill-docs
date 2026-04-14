using HashidsNet;
using OverkillDocs.Core.DTOs.User;
using OverkillDocs.Core.Exceptions;
using OverkillDocs.Core.Extensions;
using OverkillDocs.Core.Interfaces.Repositories;
using OverkillDocs.Core.Interfaces.Services;
using OverkillDocs.Core.Security;

namespace OverkillDocs.Core.Services
{
    public class UserService(IUserRepository userRepository, UserContext userContext, IHashids hashids) : IUserService
    {
        public async Task<SimpleUserDto> GetByHashId(string hashId, CancellationToken ct)
        {
            var ids = hashids.Decode(hashId);
            if (ids.Length == 0)
                throw new NotFoundException($"HashId '{hashId}' inválido.");
            var id = ids[0];

            var user = await userRepository.FindById(id, ct);
            if (user == null)
                throw new NotFoundException($"Usuário id={id} não encontrado.");

            return user.ToSimpleDto(hashids);
        }

        public async Task<SimpleUserDto> GetCurrent(CancellationToken ct)
        {
            var user = await userRepository.FindById(userContext.UserId, ct);
            if (user == null)
                throw new NotFoundException($"Usuário id={userContext.UserId} não encontrado.");

            return user.ToSimpleDto(hashids);
        }

    }
}

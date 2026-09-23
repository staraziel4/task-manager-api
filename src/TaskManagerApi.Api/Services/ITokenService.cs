using TaskManagerApi.Api.Models;

namespace TaskManagerApi.Api.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}

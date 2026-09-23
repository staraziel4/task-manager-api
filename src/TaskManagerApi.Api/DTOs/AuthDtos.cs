using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Api.DTOs;

public record RegisterRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required, MinLength(6)] string Password
);

public record LoginRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password
);

public record AuthResponse(string Token, string Email, string Role, DateTime ExpiresAt);

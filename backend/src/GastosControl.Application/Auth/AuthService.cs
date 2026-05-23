using GastosControl.Application.Abstractions;
using GastosControl.Application.Abstractions.Repositories;
using GastosControl.Application.Abstractions.Security;
using GastosControl.Application.Common;
using GastosControl.Domain.Entities;

namespace GastosControl.Application.Auth;

public sealed class AuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCredentials(request.Email, request.Password);

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationAppException("Informe seu nome.");
        }

        var email = NormalizeEmail(request.Email);
        var existingUser = await _users.GetByEmailAsync(email, cancellationToken);
        if (existingUser is not null)
        {
            throw new ConflictAppException("Ja existe um usuario cadastrado com este email.");
        }

        var user = new User(request.Name, email, _passwordHasher.Hash(request.Password));
        await _users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCredentials(request.Email, request.Password);

        var email = NormalizeEmail(request.Email);
        var user = await _users.GetByEmailAsync(email, cancellationToken);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAppException("Email ou senha invalidos.");
        }

        return CreateAuthResponse(user);
    }

    private AuthResponse CreateAuthResponse(User user)
    {
        var token = _jwtTokenGenerator.CreateToken(user);
        var userDto = new UserDto(user.Id, user.Name, user.Email);
        return new AuthResponse(userDto, token.AccessToken, token.ExpiresAt);
    }

    private static void ValidateCredentials(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@', StringComparison.Ordinal))
        {
            throw new ValidationAppException("Informe um email valido.");
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            throw new ValidationAppException("A senha deve ter pelo menos 6 caracteres.");
        }
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}

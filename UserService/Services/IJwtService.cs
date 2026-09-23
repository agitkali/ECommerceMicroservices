namespace UserService.Services
{
    public interface IJwtService
    {

        string GenerateToken(Guid userId, string userName, string role);
    }
}

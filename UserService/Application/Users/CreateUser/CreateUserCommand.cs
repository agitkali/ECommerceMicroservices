using MediatR;
namespace UserService.Application.Users.CreateUser
{
    public class CreateUserCommand : IRequest<Guid>
    {
        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}

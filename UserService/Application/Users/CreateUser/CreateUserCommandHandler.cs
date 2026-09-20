using MediatR;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;


namespace UserService.Application.Users.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly UserDbContext _context;

        public CreateUserCommandHandler(UserDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync(
                cancellationToken);

            return user.Id;
        }
    }
}

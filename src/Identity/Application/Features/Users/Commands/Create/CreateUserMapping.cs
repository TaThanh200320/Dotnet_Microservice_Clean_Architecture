
using IdentityDomain.Aggregates.Users;
using static BCrypt.Net.BCrypt;

namespace IdentityApplication.Features.Users.Commands.Create;

public static class CreateUserMapping
{
    public static User ToUser(this CreateUserCommand command)
    {
        return new User(
            command.FirstName!,
            command.LastName!,
            command.Username!,
            HashPassword(command.Password!),
            command.Email!,
            command.PhoneNumber!
        )
        {
            Gender = command.Gender,
            Status = command.Status,
            DayOfBirth = command.DayOfBirth,
        };
    }

    public static CreateUserResponse ToCreateUserResponse(this User user)
    {
        var response = new CreateUserResponse();
        response.MappingFrom(user);
        return response;
    }
}

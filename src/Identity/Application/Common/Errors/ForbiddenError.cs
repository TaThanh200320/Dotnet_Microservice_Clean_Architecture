using Contracts.ApiWrapper;
using IdentityDomain.Aggregates.Users;
using Microsoft.AspNetCore.Http;
using SharedKernel.Common.Messages;

namespace IdentityApplication.Common.Errors;

public class ForbiddenError(string title, MessageResult? message = null)
    : ErrorDetails(
        title,
        message
            ?? Messenger
                .Create<User>()
                .Negative()
                .Message(
                    new CustomMessage(
                        "Forbidden",
                        new Dictionary<string, string>()
                        {
                            { "En", "forbidden" },
                            { "Vi", "đủ quyền" },
                        },
                        "forbidden"
                    )
                )
                .Build(),
        nameof(ForbiddenError),
        StatusCodes.Status403Forbidden
    );

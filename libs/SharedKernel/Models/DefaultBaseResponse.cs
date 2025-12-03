using System;

namespace SharedKernel.Models;

public class DefaultBaseResponse
{
    public Ulid Id { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }
}
using System;
using SharedKernel.Entities;

namespace SharedKernel.Models;

public class BaseResponse : DefaultBaseResponse, IBaseAuditable
{
    public string CreatedBy { get; set; } = string.Empty;

    public string? UpdatedBy { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
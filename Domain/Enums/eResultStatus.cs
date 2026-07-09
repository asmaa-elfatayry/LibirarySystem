using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums;

public enum eResultStatus
{
    Success,
    NotFound,
    ValidationError,
    Forbidden,
    Unauthorized,
    Error
}

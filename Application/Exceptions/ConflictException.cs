﻿using System.Net;

namespace Application.Exceptions;

public class ConflictException(List<string> errorMessages = default, HttpStatusCode statusCode = HttpStatusCode.Conflict) : Exception
{
    public List<string> ErrorMessages { get; set; } = errorMessages;
    public HttpStatusCode StatusCode { get; set; } = statusCode;
}

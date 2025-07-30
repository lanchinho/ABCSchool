﻿using System.Net;

namespace Application.Exceptions;

public class IdentityException(List<string> errorMessages = default, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : Exception
{
    public List<string> ErrorMessages { get; set; } = errorMessages;
    public HttpStatusCode StatusCode { get; set; } = statusCode;
}

using System;
using System.Collections.Generic;


namespace shellApi.Exceptions
{

    public class ErrorResponse
    {
        public ErrorResponse()
        {
            error = new Dictionary<string, string>();
        }
        public Dictionary<string, string> error { get; set; }
    }

    public class AllExceptions : Exception
    {
        public System.Net.HttpStatusCode code;
        public ErrorResponse errorResponse { get; set; }
    }

    // Hey, what am I supposed to do with these things?
    //    throw new NotFoundException(new ErrorResponse
    //    {
    //        error = new Dictionary<string, string>
    //        {
    //            { "message", "A new not found exception" }
    //        }
    //    });
    //
    // And:
    //    throw new NotFoundException(new ErrorResponse
    //    {
    //        error = new Dictionary<string, string>
    //        {
    //            { "message", "A new not found exception" },
    //            { "message2", "Wait! What?" }
    //        }
    //    });


    public class NotFoundException : AllExceptions
    {
        public NotFoundException() {
        }
        public NotFoundException(ErrorResponse er)
        {
            code = System.Net.HttpStatusCode.NotFound;
            errorResponse = er;
        }
    }

    public class UnauthorizedException : AllExceptions
    {
        public UnauthorizedException() {
        }
        public UnauthorizedException(ErrorResponse er)
        {
            code = System.Net.HttpStatusCode.Unauthorized;
            errorResponse = er;
        }
    }

    public class BadRequestException : AllExceptions
    {
        public BadRequestException()
        {
        }
        public BadRequestException(ErrorResponse er)
        {
            code = System.Net.HttpStatusCode.BadRequest;
            errorResponse = er;
        }
    }
}

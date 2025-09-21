using N8N.API.EnumTypes;

namespace N8N.API.ExceptionHandlers.Exceptions
{
    [Serializable]
    public class ProblemException : Exception
    {
        public string ErrorMessage { get; set; }

        public ProblemException(string errorMessage) {
            ErrorMessage = errorMessage;
        }
    }

    public class BadRequestException : ProblemException
    {
        public BadRequestException(string message) : base(message)
        {

        }
    }

    public class NotFoundException : ProblemException
    {   
        public NotFoundException(string message) : base(message) {
            
        }
    }

    public class ConflictException : ProblemException
    {
        public ConflictException(string message) : base(message)
        {

        }
    }

    public class UnauthorizedException : ProblemException
    {
        public UnauthorizedException(string message) : base(message)
        {

        }
    }
}

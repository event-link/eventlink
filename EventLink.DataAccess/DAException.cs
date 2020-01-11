using System;

namespace EventLink.DataAccess
{
    public class DAException : Exception
    {
        public DAException() { }
        public DAException(string message) : base(message) { }
        public DAException(string message, Exception inner) : base(message, inner) { }

    }

    public class DADocAlreadyExistsException : DAException
    {
        public DADocAlreadyExistsException() : base("Document already exists!") { }
        public DADocAlreadyExistsException(string message) : base(message) { }
        public DADocAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
    }

    public class DADocNotFoundException : DAException
    {
        public DADocNotFoundException() : base("Document is not found!") { }
        public DADocNotFoundException(string message) : base(message) { }
        public DADocNotFoundException(string message, Exception inner) : base(message, inner) { }
    }

    public class DANullOrEmptyIdException : DAException
    {
        public DANullOrEmptyIdException() : base("Id is null or empty!") { }
        public DANullOrEmptyIdException(string message) : base(message) { }
        public DANullOrEmptyIdException(string message, Exception inner) : base(message, inner) { }
    }

    public class DAInvalidIdException : DAException
    {
        public DAInvalidIdException() : base("Invalid Id format!") { }
        public DAInvalidIdException(string message) : base(message) { }
        public DAInvalidIdException(string message, Exception inner) : base(message, inner) { }
    }

}
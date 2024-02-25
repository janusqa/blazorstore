namespace BlazorStore.ApiAccess.Exceptions
{

    public class AuthenticationFailureException : Exception
    {
        public AuthenticationFailureException()
        {
        }

        public AuthenticationFailureException(string message) : base(message)
        {
        }

        public AuthenticationFailureException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
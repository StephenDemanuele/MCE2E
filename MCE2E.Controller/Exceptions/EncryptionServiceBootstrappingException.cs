using System;

namespace MCE2E.Controller.Exceptions
{
    public class EncryptionServiceBootstrappingException : Exception
    {
        public EncryptionServiceBootstrappingException(string message)
            :base(message)
        {
        }

        public EncryptionServiceBootstrappingException(string message, Exception exception)
            :base(message, exception)
        {
        }
    }
}

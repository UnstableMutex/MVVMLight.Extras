using System;

namespace MVVMLight.Extras
{
    [Serializable]
    public class WrongStringEndException : Exception
    {
        public WrongStringEndException() { }
        public WrongStringEndException(string message) : base(message) { }
        public WrongStringEndException(string message, Exception inner) : base(message, inner) { }
        protected WrongStringEndException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

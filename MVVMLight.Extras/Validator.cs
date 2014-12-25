using System;

namespace MVVMLight.Extras
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class Validator : Attribute
    {
        private readonly string _message;

        protected Validator(string message)
        {
            _message = message;
        }
        public string Message
        {
            get { return _message; }
        }
        public abstract bool Validate(object value);
    }
}
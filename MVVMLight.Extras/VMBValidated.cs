using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MVVMLight.Extras
{
    public class VMBValidated : VMB, INotifyDataErrorInfo
    {
        protected override sealed void SaveQuery(string propertyName)
        {
            Validate(propertyName);
            if (!HasErrors)
            {
                base.SaveQuery(propertyName);
            }
        }

        #region validating

        private readonly Dictionary<string, VError> _errorsDictionary = new Dictionary<string, VError>();

        public IEnumerable GetErrors(string propertyName)
        {
            if (_errorsDictionary.ContainsKey(propertyName))
                return _errorsDictionary[propertyName];
            return null;
        }

        public bool HasErrors
        {
            get { return _errorsDictionary.Count != 0; }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected bool Validate(string propertyName)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            PropertyInfo pi = GetType().GetProperty(propertyName);
            object valuetovalidate = pi.GetValue(this);
            object[] customattr = pi.GetCustomAttributes(true);
            IEnumerable<Validator> validators = customattr.OfType<Validator>();
            var errorList = new List<string>();
            foreach (Validator validator in validators)
            {
                bool b = validator.Validate(valuetovalidate);
                if (!b)
                {
                    errorList.Add(validator.Message);
                }
            }
            if (errorList.Count == 0)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                if (_errorsDictionary.ContainsKey(propertyName))
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    _errorsDictionary.Remove(propertyName);
                    RaiseErrorsChanged(propertyName);
                }
                return true;
            }
            // ReSharper disable once AssignNullToNotNullAttribute
            if (!_errorsDictionary.ContainsKey(propertyName))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                _errorsDictionary[propertyName] = new VError();
                _errorsDictionary[propertyName].Errors.AddRange(errorList);
                RaiseErrorsChanged(propertyName);
            }
            else
            {
                string curerrors = string.Join("", _errorsDictionary[propertyName]);
                string newerrors = string.Join("", errorList);
                if (curerrors != newerrors)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    _errorsDictionary[propertyName].Errors.Clear();
                    _errorsDictionary[propertyName].Errors.AddRange(errorList);
                    RaiseErrorsChanged(propertyName);
                }
            }
            return false;
        }

        protected virtual void RaiseErrorsChanged(string propertyName)
        {
            var e = new DataErrorsChangedEventArgs(propertyName);
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, e);
            }
        }

        #endregion
    }
}
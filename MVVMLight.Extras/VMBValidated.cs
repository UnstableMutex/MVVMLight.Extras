using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
namespace MVVMLight.Extras
{
    public class VMBValidated : VMB, INotifyDataErrorInfo
    {
        protected sealed override void SaveQuery(string propertyName)
        {
            Validate(propertyName);
            if (!HasErrors)
            {
                base.SaveQuery(propertyName);
            }
        }
        #region validating
        protected bool Validate(string propertyName)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var pi = GetType().GetProperty(propertyName);
            var valuetovalidate = pi.GetValue(this);
            var customattr = pi.GetCustomAttributes(true);
            var validators = customattr.OfType<Validator>();
            var errorList = new List<string>();
            foreach (var validator in validators)
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
            else
            {
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
                    var curerrors = string.Join("", _errorsDictionary[propertyName]);
                    var newerrors = string.Join("", errorList);
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
        }
        readonly Dictionary<string, VError> _errorsDictionary = new Dictionary<string, VError>();
        public IEnumerable GetErrors(string propertyName)
        {
            if (_errorsDictionary.ContainsKey(propertyName))
                return _errorsDictionary[propertyName];
            else
                return null;
        }
        public bool HasErrors
        {
            get { return _errorsDictionary.Count != 0; }
        }
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
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
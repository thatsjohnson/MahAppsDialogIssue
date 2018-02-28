using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace TestMahApps
{
    public abstract class BindableBase : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Fields 

        private readonly ConcurrentDictionary<string, object> _values = new ConcurrentDictionary<string, object>();

        #endregion

        #region Protected 

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            _values[propertyName] = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary> 
        /// Sets the value of a property. 
        /// </summary> 
        /// <typeparam name="T">The type of the property value.</typeparam> 
        /// <param name="propertySelector">Expression tree contains the property definition.</param> 
        /// <param name="value">The property value.</param> 
        protected bool SetValue<T>(Expression<Func<T>> propertySelector, T value)
        {
            string propertyName = GetPropertyName(propertySelector);

            return SetValue<T>(propertyName, value);
        }

        protected bool SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            return SetValue(propertyName, value);
        }

        /// <summary> 
        /// Sets the value of a property. 
        /// </summary> 
        /// <typeparam name="T">The type of the property value.</typeparam> 
        /// <param name="propertyName">The name of the property.</param> 
        /// <param name="value">The property value.</param> 
        protected bool SetValue<T>(string propertyName, T value)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Invalid property name", propertyName);
            }

            if (_values.ContainsKey(propertyName) && Equals(_values[propertyName], value))
            {
                return false;
            }

            // first time set, set to default value
            if (!_values.ContainsKey(propertyName) && Equals(value, default(T)))
            {
                //_logger.Debug($"{propertyName} set to default: {value}");

                _values[propertyName] = value;
                return true;
            }

            _values[propertyName] = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary> 
        /// Gets the value of a property. 
        /// </summary> 
        /// <typeparam name="T">The type of the property value.</typeparam> 
        /// <param name="propertySelector">Expression tree contains the property definition.</param> 
        /// <returns>The value of the property or default value if not exist.</returns> 
        protected T GetValue<T>(Expression<Func<T>> propertySelector)
        {
            string propertyName = GetPropertyName(propertySelector);

            return GetValue<T>(propertyName);
        }

        /// <summary> 
        /// Gets the value of a property. 
        /// </summary> 
        /// <typeparam name="T">The type of the property value.</typeparam> 
        /// <param name="propertyName">The name of the property.</param> 
        /// <returns>The value of the property or default value if not exist.</returns> 
        protected T GetValue<T>(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Invalid property name", propertyName);
            }

            object value;
            if (!_values.TryGetValue(propertyName, out value))
            {
                value = default(T);
                _values.TryAdd(propertyName, value);
            }

            return (T)value;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (Application.Current == null || Application.Current.Dispatcher.CheckAccess())
            {
                RaisePropertyChangedUnsafe(propertyName);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind,
                    new ThreadStart(() => RaisePropertyChangedUnsafe(propertyName)));
            }
        }

        protected void RaisePropertyChangedUnsafe(string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected bool IsPropertyValid([CallerMemberName] string propertyName = null, List<ValidationResult> results = null)
        {
            var value = GetValue(propertyName);
            return Validator.TryValidateProperty(
                value,
                new ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                },
                results);
        }

        /// <summary> 
        /// Validates current instance properties using Data Annotations. 
        /// </summary> 
        /// <param name="propertyName">This instance property to validate.</param> 
        /// <returns>Relevant error string on validation failure or <see cref="System.String.Empty"/> on validation success.</returns> 
        protected virtual string OnValidate(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Invalid property name", propertyName);
            }

            string error = string.Empty;
            var value = GetValue(propertyName);
            var results = new List<ValidationResult>(1);

            if (!IsPropertyValid(propertyName, results))
            {
                var validationResult = results.First();
                error = validationResult.ErrorMessage;
            }

            return error;
        }

        #endregion Protected 

        #region Data Validation 

        string IDataErrorInfo.Error
        {
            get
            {
                // System doesnt use IDataErrorInfo.Error so ignore.
                return string.Empty;
                //throw new NotSupportedException("IDataErrorInfo.Error is not supported, use IDataErrorInfo.this[propertyName] instead.");
            }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                return OnValidate(propertyName);
            }
        }

        #endregion

        #region Privates 

        private string GetPropertyName(LambdaExpression expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new InvalidOperationException();
            }

            return memberExpression.Member.Name;
        }

        private object GetValue(string propertyName)
        {
            object value;
            if (!_values.TryGetValue(propertyName, out value))
            {
                var propertyDescriptor = TypeDescriptor.GetProperties(GetType()).Find(propertyName, false);
                if (propertyDescriptor == null)
                {
                    throw new ArgumentException("Invalid property name", propertyName);
                }

                value = propertyDescriptor.GetValue(this);
                _values.TryAdd(propertyName, value);
            }

            return value;
        }

        #endregion

        #region Debugging 

        /// <summary> 
        /// Warns the developer if this object does not have 
        /// a public property with the specified name. This  
        /// method does not exist in a Release build. 
        /// </summary> 
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,   
            // public, instance property on this object. 
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        /// <summary> 
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used 
        /// when an invalid property name is passed to the VerifyPropertyName method. 
        /// The default value is false, but subclasses used by unit tests might  
        /// override this property's getter to return true. 
        /// </summary> 
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides 
    }
}

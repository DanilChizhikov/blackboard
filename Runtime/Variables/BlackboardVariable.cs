using System;
using System.Collections.Generic;
using UnityEngine;

namespace DTech.Blackboard
{
    /// <summary>
    /// Represents a named blackboard entry with a stable GUID and type-erased value access.
    /// </summary>
    [Serializable]
    public abstract class BlackboardVariable
    {
        [SerializeField] private SerializableGuid _guid = SerializableGuid.Generate();
        [SerializeField] private string _name = string.Empty;

        /// <summary>
        /// Gets the stable identifier for this variable.
        /// </summary>
        public SerializableGuid Guid
        {
            get => _guid;
            internal set => _guid = value;
        }

        /// <summary>
        /// Gets or sets the display name used as the lookup key.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Debug.LogError($"{nameof(BlackboardVariable)}.{nameof(Name)} cannot be null or empty");
                    return;
                }
                
                _name = value;
            }
        }
        
        /// <summary>
        /// Gets the runtime type of the stored value.
        /// </summary>
        public abstract Type ValueType { get; }

        /// <summary>
        /// Gets or sets the value through boxed object access.
        /// </summary>
        public abstract object ObjectValue { get; set; }
        
        /// <summary>
        /// Creates a typed blackboard variable for the provided value type and assigns its name.
        /// </summary>
        /// <param name="type">Value type for the created variable.</param>
        /// <param name="name">Variable name.</param>
        /// <returns>A new typed variable instance.</returns>
        public static BlackboardVariable CreateForType(Type type, string name)
        {
            var instance = Activator.CreateInstance(typeof(BlackboardVariable<>).MakeGenericType(type)) as BlackboardVariable;
            instance.Name = name;
            return instance;
        }
        
        /// <summary>
        /// Sets the value through boxed object access without invoking change notifications.
        /// </summary>
        /// <param name="value">New boxed value.</param>
        public abstract void SetObjectValueWithoutNotif(object value);
        
        /// <summary>
        /// Creates a copy of the variable preserving GUID, name, and value.
        /// </summary>
        /// <returns>A cloned variable instance.</returns>
        public abstract BlackboardVariable Clone();
    }

    /// <summary>
    /// Typed implementation of <see cref="BlackboardVariable"/> that stores and exposes a value of type <typeparamref name="T"/>.
    /// </summary>
    [Serializable]
    public class BlackboardVariable<T> : BlackboardVariable
    {
        /// <summary>
        /// Raised when <see cref="Value"/> changes to a new value.
        /// </summary>
        public event Action<SerializableGuid, T> OnValueChanged; 
        
        [SerializeField]
        private T _value;
        
        /// <summary>
        /// Gets the runtime type of <typeparamref name="T"/>.
        /// </summary>
        public override Type ValueType { get; } = typeof(T);

        /// <summary>
        /// Gets or sets the value via boxed access and applies notification rules of <see cref="Value"/>.
        /// </summary>
        public override object ObjectValue
        {
            get => Value;
            set => Value = (T)value;
        }

        /// <summary>
        /// Gets or sets the typed value and notifies subscribers when the value actually changes.
        /// </summary>
        public virtual T Value
        {
            get => _value;

            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value))
                {
                    return;
                }
                
                _value = value;
                OnValueChanged?.Invoke(Guid, value);
            }
        }

        /// <summary>
        /// Sets the typed value through boxed access without invoking <see cref="OnValueChanged"/>.
        /// </summary>
        /// <param name="value">New boxed value.</param>
        public override void SetObjectValueWithoutNotif(object value)
        {
            SetValueWithoutNotif((T)value);
        }
        
        /// <summary>
        /// Sets the typed value without invoking <see cref="OnValueChanged"/>.
        /// </summary>
        /// <param name="value">New typed value.</param>
        public virtual void SetValueWithoutNotif(T value)
        {
            if (EqualityComparer<T>.Default.Equals(_value, value))
            {
                return;
            }
            
            _value = value;
        }

        /// <summary>
        /// Creates a copy of this variable preserving GUID, name, and current value.
        /// </summary>
        /// <returns>A cloned typed variable.</returns>
        public override BlackboardVariable Clone()
        {
            return new BlackboardVariable<T>
            {
                Guid = Guid,
                Name = Name,
                _value = _value
            };
        }
    }
}

using System;
using UnityEngine;

namespace DTech.Blackboard
{
    [Serializable]
    public abstract class BlackboardVariable
    {
        [SerializeField] private SerializableGuid _guid = SerializableGuid.Generate();
        [SerializeField] private string _name = string.Empty;

        public SerializableGuid Guid
        {
            get => _guid;
            internal set => _guid = value;
        }

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
        
        public abstract Type ValueType { get; }
        public abstract object ObjectValue { get; set; }
        
        public static BlackboardVariable CreateForType(Type type, string name)
        {
            var instance = Activator.CreateInstance(typeof(BlackboardVariable<>).MakeGenericType(type)) as BlackboardVariable;
            instance.Name = name;
            return instance;
        }
        
        public abstract void SetObjectValueWithoutNotif(object value);
        
        public abstract BlackboardVariable Clone();
    }

    [Serializable]
    public class BlackboardVariable<T> : BlackboardVariable
    {
        public event Action<SerializableGuid, T> OnValueChanged; 
        
        [SerializeField]
        private T _value;
        
        public override Type ValueType { get; } = typeof(T);

        public override object ObjectValue
        {
            get => Value;
            set => Value = (T)value;
        }

        public virtual T Value
        {
            get => _value;

            set
            {
                if (_value.Equals(value))
                {
                    return;
                }
                
                _value = value;
                OnValueChanged?.Invoke(Guid, value);
            }
        }

        public override void SetObjectValueWithoutNotif(object value)
        {
            SetValueWithoutNotif((T)value);
        }
        
        public virtual void SetValueWithoutNotif(T value)
        {
            if (_value.Equals(value))
            {
                return;
            }
            
            _value = value;
        }

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
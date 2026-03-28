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
            protected set => _guid = value;
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
        
        public abstract BlackboardVariable Clone();
    }

    [Serializable]
    public class BlackboardVariable<T> : BlackboardVariable
    {
        [SerializeField]
        private T _value;
        
        public override Type ValueType { get; } = typeof(T);

        public override object ObjectValue
        {
            get => _value;
            set => _value = (T)value;
        }

        public T Value
        {
            get => _value;
            set => _value = value;
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
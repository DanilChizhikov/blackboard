using System;
using UnityEngine;

namespace DTech.Blackboard
{
    [Serializable]
    internal sealed class SerializableType : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string _serializableType;

        [NonSerialized]
        private Type _type;
        
        public string Text
        {
            get
            {
                if (string.IsNullOrEmpty(_serializableType) &&
                    _type != null)
                {
                    AssignTypeToString();
                }

                return _serializableType;
            }
        }
        
        public Type Type
        {
            get
            {
                if (_type == null && !string.IsNullOrEmpty(_serializableType))
                {
                    ReadTypeFromString();
                }

                return _type;
            }

            private set
            {
                _type = value;
                AssignTypeToString();
            }
        }
        
        public SerializableType(Type type)
        {
            _type = type;
            AssignTypeToString();
        }

        public SerializableType(string typeText)
        {
            _serializableType = typeText;
            ReadTypeFromString();
        }
        
        private SerializableType()
        {
        }

        public static implicit operator SerializableType(Type value)
        {
            return new SerializableType(value);
        }

        public static implicit operator Type(SerializableType value)
        {
            return !ReferenceEquals(value, null) ? value._type : null;
        }
        
        public static bool operator ==(SerializableType left, SerializableType right)
        {
            if (!ReferenceEquals(left, null))
            {
                return left.Equals(right);
            }

            if (!ReferenceEquals(right, null))
            {
                return right.Equals(left);
            }

            return true;
        }

        public static bool operator !=(SerializableType left, SerializableType right)
        {
            return !(left == right);
        }

        public void OnBeforeSerialize()
        {
            AssignTypeToString();
        }

        public void OnAfterDeserialize()
        {
            ReadTypeFromString();
        }

        private void AssignTypeToString()
        {
            if (_type != null)
            {
                _serializableType = _type.AssemblyQualifiedName;
            }
        }

        private void ReadTypeFromString()
        {
            _type = Type.GetType(_serializableType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            Type otherType = null;
            if (obj is SerializableType otherSerializableType)
            {
                otherType = otherSerializableType.Type;
            }
            else if (obj is Type sysType)
            {
                otherType = sysType;
            }
            else if (!ReferenceEquals(obj, null))
            {
                return false;
            }

            return Type == otherType;
        }

        public override int GetHashCode()
        {
            if (_type == null &&
                !string.IsNullOrEmpty(_serializableType))
            {
                ReadTypeFromString();
            }

            return _type != null ? _type.GetHashCode() : 0;
        }

        public override string ToString() => Text;
    }
}
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace DTech.Blackboard
{
    [StructLayout(LayoutKind.Explicit)]
    [Serializable]
    public struct SerializableGuid : IEquatable<SerializableGuid>
    {
        [SerializeField]
        [FieldOffset(0)]
        private ulong _value0;
        
        [SerializeField]
        [FieldOffset(8)]
        private ulong _value1;
        
        [FieldOffset(0)]
        private Hash128 _hash128;
        
        [Preserve]
        private long Value0Signed
        {
            get => (long)_value0 + long.MinValue;
            set => _value0 = (ulong)(value - long.MinValue);
        }

        [Preserve]
        private long Value1Signed
        {
            get => (long)_value1 + long.MinValue;
            set => _value1 = (ulong)(value - long.MinValue);
        }
        
        public bool IsValid => _hash128.isValid;

        public SerializableGuid(Hash128 hash)
        {
            _value0 = 0;
            _value1 = 0;
            _hash128 = hash;
        }
        
        public SerializableGuid(string hashString)
        {
            _value0 = 0;
            _value1 = 0;
            _hash128 = Hash128.Parse(hashString);
        }
        
        public SerializableGuid(ulong a, ulong b)
        {
            _hash128 = default;
            _value0 = a;
            _value1 = b;
        }
        
        public static implicit operator Hash128(SerializableGuid sGuid) => sGuid._hash128;
        
        public static implicit operator SerializableGuid(Hash128 hash) => new SerializableGuid(hash);
        
        public static bool operator ==(SerializableGuid left, SerializableGuid right)
        {
            return left._value0 == right._value0 && left._value1 == right._value1;
        }
        
        public static bool operator !=(SerializableGuid left, SerializableGuid right)
        {
            return left._value0 != right._value0 || left._value1 != right._value1;
        }

        public static SerializableGuid Generate()
        {
            return new SerializableGuid(Hash128.Compute(Guid.NewGuid().ToByteArray()));
        }
        
        public (ulong, ulong) ToParts()
        {
            return (_value0, _value1);
        }
        
        public override string ToString()
        {
            return _hash128.ToString();
        }

        public bool Equals(SerializableGuid other)
        {
            return _value0 == other._value0 && _value1 == other._value1;
        }
        
        public override bool Equals(object obj)
        {
            return obj is SerializableGuid other && Equals(other);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                return (_value0.GetHashCode() * 397) ^ _value0.GetHashCode();
            }
        }
    }
}
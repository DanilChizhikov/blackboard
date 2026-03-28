using System;
using System.Collections.Generic;
using UnityEngine;

namespace DTech.Blackboard
{
    internal sealed class BlackboardVariableHolder : IDisposable
    {
        private readonly Dictionary<string, SerializableGuid> _nameToGuid;
        private readonly Dictionary<SerializableGuid, BlackboardVariable> _guidToVariable;
        
        public BlackboardVariable this[string name]
        {
            get
            {
                if (!TryGetVariable(name, out BlackboardVariable variable))
                {
                    throw new KeyNotFoundException($"{nameof(BlackboardVariableHolder)}: Variable with name '{name}' not found");
                }

                return variable;
            }
        }
        
        public BlackboardVariable this[SerializableGuid guid]
        {
            get
            {
                if (!TryGetVariable(guid, out BlackboardVariable variable))
                {
                    throw new KeyNotFoundException($"{nameof(BlackboardVariableHolder)}: Variable with guid '{guid}' not found");
                }

                return variable;
            }
        }

        public BlackboardVariableHolder()
        {
            _nameToGuid = new Dictionary<string, SerializableGuid>();
            _guidToVariable = new Dictionary<SerializableGuid, BlackboardVariable>();
        }

        public BlackboardVariableHolder(int capacity)
        {
            _nameToGuid = new Dictionary<string, SerializableGuid>(capacity);
            _guidToVariable = new Dictionary<SerializableGuid, BlackboardVariable>(capacity);
        }

        public BlackboardVariableHolder(IEnumerable<BlackboardVariable> variables)
        {
            _nameToGuid = new Dictionary<string, SerializableGuid>();
            _guidToVariable = new Dictionary<SerializableGuid, BlackboardVariable>();
            foreach (BlackboardVariable variable in variables)
            {
                Add(variable);
            }
        }

        public bool Contains(string name)
        {
            if (!_nameToGuid.TryGetValue(name, out SerializableGuid guid))
            {
                return false;
            }
            
            return _guidToVariable.ContainsKey(guid);
        }
        
        public void Add(BlackboardVariable variable)
        {
            if (Contains(variable.Name))
            {
                Debug.LogError($"{nameof(BlackboardVariableHolder)}.{nameof(Add)}: Variable with name '{variable.Name}' already exists");
                return;
            }
            
            _nameToGuid.Add(variable.Name, variable.Guid);
            _guidToVariable.Add(variable.Guid, variable);
        }

        public void Replace(BlackboardVariable from, BlackboardVariable to)
        {
            if (!Contains(from.Name))
            {
                Debug.LogError($"{nameof(BlackboardVariableHolder)}.{nameof(Replace)}: Variable with name '{from.Name}' not found");
                return;
            }
            
            Remove(from.Name);
            Add(to);
        }

        public bool Remove(string name)
        {
            return Remove(name, out _);
        }
        
        public bool Remove(SerializableGuid guid)
        {
            return Remove(guid, out _);
        }

        public bool Remove(string name, out BlackboardVariable variable)
        {
            variable = null;
            if (!_nameToGuid.TryGetValue(name, out SerializableGuid guid))
            {
                return false;
            }
            
            return Remove(guid, out variable);
        }
        
        public bool Remove(SerializableGuid guid, out BlackboardVariable variable)
        {
            bool result = _guidToVariable.Remove(guid, out variable);
            if (result)
            {
                _nameToGuid.Remove(variable.Name);
            }
            
            return result;
        }

        public bool TryGetVariable(string name, out BlackboardVariable variable)
        {
            variable = null;
            if (!_nameToGuid.TryGetValue(name, out SerializableGuid guid))
            {
                return false;
            }
            
            return TryGetVariable(guid, out variable);
        }

        public bool TryGetVariable(SerializableGuid guid, out BlackboardVariable variable)
        {
            return _guidToVariable.TryGetValue(guid, out variable);
        }
        
        public void Dispose()
        {
            _nameToGuid.Clear();
            _guidToVariable.Clear();
        }
    }
}
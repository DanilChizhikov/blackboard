using System;
using System.Collections.Generic;
using UnityEngine;

namespace DTech.Blackboard
{
    internal sealed class BlackboardVariableHolder : IDisposable
    {
        private readonly Dictionary<string, SerializableGuid> _nameToGuid;
        private readonly Dictionary<SerializableGuid, BlackboardVariable> _guidToVariable;
        
        public int Count => _guidToVariable.Count;
        
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
            _nameToGuid = new Dictionary<string, SerializableGuid>(StringComparer.OrdinalIgnoreCase);
            _guidToVariable = new Dictionary<SerializableGuid, BlackboardVariable>();
        }

        public BlackboardVariableHolder(int capacity)
        {
            _nameToGuid = new Dictionary<string, SerializableGuid>(capacity, StringComparer.OrdinalIgnoreCase);
            _guidToVariable = new Dictionary<SerializableGuid, BlackboardVariable>(capacity);
        }

        public BlackboardVariableHolder(IEnumerable<BlackboardVariable> variables)
        {
            _nameToGuid = new Dictionary<string, SerializableGuid>(StringComparer.OrdinalIgnoreCase);
            _guidToVariable = new Dictionary<SerializableGuid, BlackboardVariable>();
            foreach (BlackboardVariable variable in variables)
            {
                Add(variable);
            }
        }

        public bool Contains(string name)
        {
            string normalizedName = BlackboardVariableNameValidator.Normalize(name);
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return false;
            }

            if (!_nameToGuid.TryGetValue(normalizedName, out SerializableGuid guid))
            {
                return false;
            }
            
            return _guidToVariable.ContainsKey(guid);
        }
        
        public void Add(BlackboardVariable variable)
        {
            if (variable == null)
            {
                Debug.LogError($"{nameof(BlackboardVariableHolder)}.{nameof(Add)}: Variable cannot be null");
                return;
            }

            if (!TryValidateName(variable.Name, null, out string normalizedName, out string errorMessage))
            {
                Debug.LogError($"{nameof(BlackboardVariableHolder)}.{nameof(Add)}: {errorMessage}");
                return;
            }
            
            _nameToGuid.Add(normalizedName, variable.Guid);
            _guidToVariable.Add(variable.Guid, variable);
        }

        public bool Replace(string sourceVariableName, BlackboardVariable to)
        {
            if (!TryGetVariable(sourceVariableName, out BlackboardVariable from))
            {
                Debug.LogError($"{nameof(BlackboardVariableHolder)}.{nameof(Replace)}: Variable with name '{sourceVariableName}' not found");
                return false;
            }

            if (!BlackboardVariableNameValidator.EqualsByPolicy(from.Name, to.Name))
            {
                Debug.LogError($"{nameof(BlackboardVariableHolder)}.{nameof(Replace)}: Variable with name '{from.Name}' cannot be replaced with '{to.Name}'");
                return false;
            }
            
            Remove(sourceVariableName);
            to.Guid = from.Guid;
            Add(to);
            return true;
        }
        
        public bool Replace(SerializableGuid sourceGuid, BlackboardVariable to)
        {
            if (!TryGetVariable(sourceGuid, out BlackboardVariable from))
            {
                Debug.LogError($"{nameof(BlackboardVariableHolder)}.{nameof(Replace)}: Variable with guid '{sourceGuid}' not found");
                return false;
            }

            if (!BlackboardVariableNameValidator.EqualsByPolicy(from.Name, to.Name))
            {
                Debug.LogError($"{nameof(BlackboardVariableHolder)}.{nameof(Replace)}: Variable with name '{from.Name}' cannot be replaced with '{to.Name}'");
                return false;
            }
            
            Remove(sourceGuid);
            to.Guid = sourceGuid;
            Add(to);
            return true;
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
            string normalizedName = BlackboardVariableNameValidator.Normalize(name);
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return false;
            }

            if (!_nameToGuid.TryGetValue(normalizedName, out SerializableGuid guid))
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
                _nameToGuid.Remove(BlackboardVariableNameValidator.Normalize(variable.Name));
            }
            
            return result;
        }

        public bool TryGetVariable(string name, out BlackboardVariable variable)
        {
            variable = null;
            string normalizedName = BlackboardVariableNameValidator.Normalize(name);
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return false;
            }

            if (!_nameToGuid.TryGetValue(normalizedName, out SerializableGuid guid))
            {
                return false;
            }
            
            return TryGetVariable(guid, out variable);
        }

        public bool TryGetVariable(SerializableGuid guid, out BlackboardVariable variable)
        {
            return _guidToVariable.TryGetValue(guid, out variable);
        }
        
        public int GetVariablesNonAlloc(BlackboardVariable[] variables)
        {
            int count = Math.Min(_guidToVariable.Count, variables.Length);

            int iterator = 0;
            foreach (var value in _guidToVariable.Values)
            {
                if (iterator >= count)
                {
                    break;
                }

                variables[iterator++] = value;
            }

            return count;
        }
        
        public void Dispose()
        {
            _nameToGuid.Clear();
            _guidToVariable.Clear();
        }

        private bool TryValidateName(
            string variableName,
            SerializableGuid? excludedGuid,
            out string normalizedName,
            out string errorMessage)
        {
            normalizedName = BlackboardVariableNameValidator.Normalize(variableName);
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                errorMessage = "Variable name cannot be empty.";
                return false;
            }

            if (_nameToGuid.TryGetValue(normalizedName, out SerializableGuid guid))
            {
                if (!excludedGuid.HasValue || guid != excludedGuid.Value)
                {
                    BlackboardVariable conflictVariable = _guidToVariable[guid];
                    errorMessage = $"Variable name '{normalizedName}' is already used by '{conflictVariable.Name}'.";
                    return false;
                }
            }

            errorMessage = null;
            return true;
        }
    }
}

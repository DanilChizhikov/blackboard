#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor.Callbacks;

namespace DTech.Blackboard
{
    public sealed partial class BlackboardAsset
    {
        /// <summary>
        /// Returns a read-only list of all variables in the blackboard. Works only in Editor.
        /// </summary>
        public IReadOnlyList<BlackboardVariable> Variables => _variables;
        
        /// <summary>
        /// Adds a new variable to the blackboard. Works only in Editor.
        /// </summary>
        /// <param name="variableName">Variable name</param>
        /// <param name="valueType">Type of the value</param>
        /// <returns>Guid of the new variable</returns>
        /// <exception cref="ArgumentException">Thrown when a variable with the same name already exists</exception>
        public SerializableGuid AddVariable(string variableName, Type valueType)
        {
            if (_variables == null)
            {
                _variables = new List<BlackboardVariable>();
            }
            
            if (!BlackboardVariableNameValidator.TryValidate(_variables, variableName, null, out string normalizedName, out string errorMessage))
            {
                throw new ArgumentException($"{nameof(BlackboardAsset)}.{nameof(AddVariable)}: {errorMessage}");
            }

            BlackboardVariable blackboardVariable = BlackboardVariable.CreateForType(valueType, normalizedName);
            _variables.Add(blackboardVariable);
            return blackboardVariable.Guid;
        }
        
        /// <summary>
        /// Removes a variable from the blackboard. Works only in Editor.
        /// </summary>
        /// <param name="variableName">Variable name</param>
        /// <returns>TRUE if the variable was removed, FALSE otherwise</returns>
        public bool RemoveVariable(string variableName)
        {
            if (_variables == null)
            {
                _variables = new List<BlackboardVariable>();
            }
            
            int removeIndex = -1;
            for (int i = 0; i < _variables.Count; i++)
            {
                BlackboardVariable variable = _variables[i];
                if (BlackboardVariableNameValidator.EqualsByPolicy(variable.Name, variableName))
                {
                    removeIndex = i;
                    break;
                }
            }

            if (removeIndex >= 0)
            {
                _variables.RemoveAt(removeIndex);
            }
            
            return removeIndex >= 0;
        }
    }
}
#endif

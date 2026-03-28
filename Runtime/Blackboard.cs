using System;
using System.Collections.Generic;

namespace DTech.Blackboard
{
    public sealed class Blackboard : IDisposable
    {
        private readonly BlackboardVariableHolder _holder;
        
        /// <summary>
        /// Gets the number of variables in the blackboard.
        /// </summary>
        public int VariableCount => _holder.Count;

        public Blackboard(IEnumerable<BlackboardVariable> variables)
        {
            if (variables == null)
            {
                _holder = new BlackboardVariableHolder();
            }
            else
            {
                _holder = new BlackboardVariableHolder(variables);
            }
        }

        public Blackboard(int capacity)
        {
            _holder = new BlackboardVariableHolder(capacity);
        }

        public Blackboard()
        {
            _holder = new BlackboardVariableHolder();
        }

        /// <summary>
        /// Adds a variable to the blackboard.
        /// </summary>
        /// <param name="variable">Blackboard variable</param>
        public void Add(BlackboardVariable variable)
        {
            _holder.Add(variable);
        }
        
        /// <summary>
        /// Replaces a variable in the blackboard.
        /// </summary>
        /// <param name="sourceVariableName">Variable name to replace</param>
        /// <param name="to">New variable</param>
        /// <returns>TRUE if the variable was replaced, FALSE otherwise</returns>
        public bool Replace(string sourceVariableName, BlackboardVariable to)
        {
            return _holder.Replace(sourceVariableName, to);
        }
        
        /// <summary>
        /// Replaces a variable in the blackboard.
        /// </summary>
        /// <param name="sourceGuid">Variable guid to replace</param>
        /// <param name="to">New variable</param>
        /// <returns>TRUE if the variable was replaced, FALSE otherwise</returns>
        public bool Replace(SerializableGuid sourceGuid, BlackboardVariable to)
        {
            return _holder.Replace(sourceGuid, to);
        }

        /// <summary>
        /// Removes a variable from the blackboard.
        /// </summary>
        /// <param name="name">Variable name to remove</param>
        /// <returns>TRUE if the variable was removed, FALSE otherwise</returns>
        public bool Remove(string name)
        {
            return _holder.Remove(name);
        }
        
        /// <summary>
        /// Removes a variable from the blackboard.
        /// </summary>
        /// <param name="guid">Variable guid to remove</param>
        /// <returns>TRUE if the variable was removed, FALSE otherwise</returns>
        public bool Remove(SerializableGuid guid)
        {
            return _holder.Remove(guid);
        }
        
        /// <summary>
        /// Removes a variable from the blackboard.
        /// </summary>
        /// <param name="name">Variable name to remove</param>
        /// <param name="variable">Removed Blackboard</param>
        /// <returns>TRUE if the variable was removed, FALSE otherwise</returns>
        public bool Remove(string name, out BlackboardVariable variable)
        {
            return _holder.Remove(name, out variable);
        }
        
        /// <summary>
        /// Removes a variable from the blackboard.
        /// </summary>
        /// <param name="guid">Variable guid to remove</param>
        /// <param name="variable">Removed Blackboard</param>
        /// <returns>TRUE if the variable was removed, FALSE otherwise</returns>
        public bool Remove(SerializableGuid guid, out BlackboardVariable variable)
        {
            return _holder.Remove(guid, out variable);
        }

        /// <summary>
        /// Tries to get a variable by name.
        /// </summary>
        /// <param name="name">Blackboard variable name</param>
        /// <param name="variable">Blackboard variable</param>
        /// <returns>TRUE if the variable was found, FALSE otherwise</returns>
        public bool TryGetVariable(string name, out BlackboardVariable variable)
        {
            return _holder.TryGetVariable(name, out variable);
        }

        /// <summary>
        /// Tries to get a variable by name with concrete type.
        /// </summary>
        /// <param name="name">Blackboard variable name</param>
        /// <param name="variable">Blackboard variable</param>
        /// <typeparam name="T">Value type for the variable</typeparam>
        /// <returns>TRUE if the variable was found, FALSE otherwise</returns>
        public bool TryGetVariable<T>(string name, out BlackboardVariable<T> variable)
        {
            variable = null;
            if (!_holder.TryGetVariable(name, out BlackboardVariable cachedVariable))
            {
                return false;
            }

            variable = cachedVariable.ValueType.IsAssignableFrom(typeof(T)) ? (BlackboardVariable<T>)cachedVariable : null;
            return variable != null;
        }
        
        /// <summary>
        /// Tries to get a variable by guid.
        /// </summary>
        /// <param name="name">Blackboard variable name</param>
        /// <param name="variable">Blackboard variable</param>
        /// <returns>TRUE if the variable was found, FALSE otherwise</returns>
        public bool TryGetVariable(SerializableGuid guid, out BlackboardVariable variable)
        {
            return _holder.TryGetVariable(guid, out variable);
        }

        /// <summary>
        /// Tries to get a variable by guid with concrete type.
        /// </summary>
        /// <param name="name">Blackboard variable name</param>
        /// <param name="variable">Blackboard variable</param>
        /// <typeparam name="T">Value type for the variable</typeparam>
        /// <returns>TRUE if the variable was found, FALSE otherwise</returns>
        public bool TryGetVariable<T>(SerializableGuid guid, out BlackboardVariable<T> variable)
        {
            variable = null;
            if (!_holder.TryGetVariable(guid, out BlackboardVariable cachedVariable))
            {
                return false;
            }

            variable = cachedVariable.ValueType.IsAssignableFrom(typeof(T)) ? (BlackboardVariable<T>)cachedVariable : null;
            return variable != null;
        }
        
        /// <summary>
        /// Gets all variables in the blackboard.
        /// </summary>
        /// <param name="variables">Array to store the variables</param>
        /// <returns>Number of variables</returns>
        public int GetVariablesNonAlloc(BlackboardVariable[] variables)
        {
            return _holder.GetVariablesNonAlloc(variables);
        }
        
        public void Dispose()
        {
            _holder.Dispose();
        }
    }
}
using System;
using System.Collections.Generic;

namespace DTech.Blackboard
{
    public sealed class Blackboard : IDisposable
    {
        private readonly BlackboardVariableHolder _holder;

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
        
        public void Dispose()
        {
            _holder.Dispose();
        }
    }
}
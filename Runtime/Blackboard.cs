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
        
		/// <summary>
		/// Gets a variable by name.
		/// </summary>
		/// <param name="name">The variable name.</param>
		/// <returns>The variable if found; otherwise, null.</returns>
		public BlackboardVariable GetVariable(string name)
		{
			if (!TryGetVariable(name, out BlackboardVariable variable))
			{
				return null;
			}
			
			return variable;
		}
		
		/// <summary>
		/// Gets a variable by GUID.
		/// </summary>
		/// <param name="guid">The variable GUID.</param>
		/// <returns>The variable if found; otherwise, null.</returns>
		public BlackboardVariable GetVariable(SerializableGuid guid)
		{
			if (!TryGetVariable(guid, out BlackboardVariable variable))
			{
				return null;
			}
			
			return variable;
		}
		
		/// <summary>
		/// Gets a typed variable by name.
		/// </summary>
		/// <typeparam name="T">The expected value type.</typeparam>
		/// <param name="name">The variable name.</param>
		/// <returns>The typed variable if found; otherwise, null.</returns>
		public BlackboardVariable<T> GetVariable<T>(string name)
		{
			if (!TryGetVariable(name, out BlackboardVariable<T> variable))
			{
				return null;
			}
			
			return variable;
		}
		
		/// <summary>
		/// Gets a typed variable by GUID.
		/// </summary>
		/// <typeparam name="T">The expected value type.</typeparam>
		/// <param name="guid">The variable GUID.</param>
		/// <returns>The typed variable if found; otherwise, null.</returns>
		public BlackboardVariable<T> GetVariable<T>(SerializableGuid guid)
		{
			if (!TryGetVariable(guid, out BlackboardVariable<T> variable))
			{
				return null;
			}
			
			return variable;
		}

		/// <summary>
		/// Gets the value of a typed variable by name.
		/// </summary>
		/// <typeparam name="T">The expected value type.</typeparam>
		/// <param name="name">The variable name.</param>
		/// <returns>The variable value.</returns>
		public T GetValue<T>(string name)
		{
			BlackboardVariable<T> variable = GetVariable<T>(name);
			return variable.Value;
		}
		
		/// <summary>
		/// Gets the value of a typed variable by GUID.
		/// </summary>
		/// <typeparam name="T">The expected value type.</typeparam>
		/// <param name="guid">The variable GUID.</param>
		/// <returns>The variable value.</returns>
		public T GetValue<T>(SerializableGuid guid)
		{
			BlackboardVariable<T> variable = GetVariable<T>(guid);
			return variable.Value;
		}
		
		/// <summary>
		/// Sets the value of a typed variable by name.
		/// </summary>
		/// <typeparam name="T">The value type.</typeparam>
		/// <param name="name">The variable name.</param>
		/// <param name="value">The value to set.</param>
		public void SetValue<T>(string name, T value)
		{
			BlackboardVariable<T> variable = GetVariable<T>(name);
			variable.Value = value;
		}
		
		/// <summary>
		/// Sets the value of a typed variable by name, without notification.
		/// </summary>
		/// <typeparam name="T">The value type.</typeparam>
		/// <param name="name">The variable name.</param>
		/// <param name="value">The value to set.</param>
		public void SetValueWithoutNotify<T>(string name, T value)
		{
			BlackboardVariable<T> variable = GetVariable<T>(name);
			variable.SetValueWithoutNotif(value);
		}
		
		/// <summary>
		/// Sets the value of a typed variable by GUID.
		/// </summary>
		/// <typeparam name="T">The value type.</typeparam>
		/// <param name="guid">The variable GUID.</param>
		/// <param name="value">The value to set.</param>
		public void SetValue<T>(SerializableGuid guid, T value)
		{
			BlackboardVariable<T> variable = GetVariable<T>(guid);
			variable.Value = value;
		}
		
		/// <summary>
		/// Sets the value of a typed variable by GUID, without notification.
		/// </summary>
		/// <typeparam name="T">The value type.</typeparam>
		/// <param name="guid">The variable GUID.</param>
		/// <param name="value">The value to set.</param>
		public void SetValueWithoutNotify<T>(SerializableGuid guid, T value)
		{
			BlackboardVariable<T> variable = GetVariable<T>(guid);
			variable.SetValueWithoutNotif(value);
		}
        
        public void Dispose()
        {
            _holder.Dispose();
        }
    }
}
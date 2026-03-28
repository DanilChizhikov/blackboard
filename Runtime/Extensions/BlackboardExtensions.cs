namespace DTech.Blackboard
{
	public static class BlackboardExtensions
	{
		/// <summary>
		/// Gets a variable by name.
		/// </summary>
		/// <param name="blackboard">The blackboard to search.</param>
		/// <param name="name">The variable name.</param>
		/// <returns>The variable if found; otherwise, null.</returns>
		public static BlackboardVariable GetVariable(this Blackboard blackboard, string name)
		{
			if (!blackboard.TryGetVariable(name, out BlackboardVariable variable))
			{
				return null;
			}
			
			return variable;
		}
		
		/// <summary>
		/// Gets a variable by GUID.
		/// </summary>
		/// <param name="blackboard">The blackboard to search.</param>
		/// <param name="guid">The variable GUID.</param>
		/// <returns>The variable if found; otherwise, null.</returns>
		public static BlackboardVariable GetVariable(this Blackboard blackboard, SerializableGuid guid)
		{
			if (!blackboard.TryGetVariable(guid, out BlackboardVariable variable))
			{
				return null;
			}
			
			return variable;
		}
		
		/// <summary>
		/// Gets a typed variable by name.
		/// </summary>
		/// <typeparam name="T">The expected value type.</typeparam>
		/// <param name="blackboard">The blackboard to search.</param>
		/// <param name="name">The variable name.</param>
		/// <returns>The typed variable if found; otherwise, null.</returns>
		public static BlackboardVariable<T> GetVariable<T>(this Blackboard blackboard, string name)
		{
			if (!blackboard.TryGetVariable<T>(name, out BlackboardVariable<T> variable))
			{
				return null;
			}
			
			return variable;
		}
		
		/// <summary>
		/// Gets a typed variable by GUID.
		/// </summary>
		/// <typeparam name="T">The expected value type.</typeparam>
		/// <param name="blackboard">The blackboard to search.</param>
		/// <param name="guid">The variable GUID.</param>
		/// <returns>The typed variable if found; otherwise, null.</returns>
		public static BlackboardVariable<T> GetVariable<T>(this Blackboard blackboard, SerializableGuid guid)
		{
			if (!blackboard.TryGetVariable<T>(guid, out BlackboardVariable<T> variable))
			{
				return null;
			}
			
			return variable;
		}

		/// <summary>
		/// Gets the value of a typed variable by name.
		/// </summary>
		/// <typeparam name="T">The expected value type.</typeparam>
		/// <param name="blackboard">The blackboard to search.</param>
		/// <param name="name">The variable name.</param>
		/// <returns>The variable value.</returns>
		public static T GetValue<T>(this Blackboard blackboard, string name)
		{
			BlackboardVariable<T> variable = blackboard.GetVariable<T>(name);
			return variable.Value;
		}
		
		/// <summary>
		/// Gets the value of a typed variable by GUID.
		/// </summary>
		/// <typeparam name="T">The expected value type.</typeparam>
		/// <param name="blackboard">The blackboard to search.</param>
		/// <param name="guid">The variable GUID.</param>
		/// <returns>The variable value.</returns>
		public static T GetValue<T>(this Blackboard blackboard, SerializableGuid guid)
		{
			BlackboardVariable<T> variable = blackboard.GetVariable<T>(guid);
			return variable.Value;
		}
		
		/// <summary>
		/// Sets the value of a typed variable by name.
		/// </summary>
		/// <typeparam name="T">The value type.</typeparam>
		/// <param name="blackboard">The blackboard containing the variable.</param>
		/// <param name="name">The variable name.</param>
		/// <param name="value">The value to set.</param>
		public static void SetValue<T>(this Blackboard blackboard, string name, T value)
		{
			BlackboardVariable<T> variable = blackboard.GetVariable<T>(name);
			variable.Value = value;
		}
		
		/// <summary>
		/// Sets the value of a typed variable by GUID.
		/// </summary>
		/// <typeparam name="T">The value type.</typeparam>
		/// <param name="blackboard">The blackboard containing the variable.</param>
		/// <param name="guid">The variable GUID.</param>
		/// <param name="value">The value to set.</param>
		public static void SetValue<T>(this Blackboard blackboard, SerializableGuid guid, T value)
		{
			BlackboardVariable<T> variable = blackboard.GetVariable<T>(guid);
			variable.Value = value;
		}
	}
}
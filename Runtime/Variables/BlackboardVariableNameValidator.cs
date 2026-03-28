using System;
using System.Collections.Generic;

namespace DTech.Blackboard
{
    internal static class BlackboardVariableNameValidator
    {
        private const string EmptyVariableNameError = "Variable name cannot be empty.";

        public static string Normalize(string variableName)
        {
            return string.IsNullOrWhiteSpace(variableName) ? string.Empty : variableName.Trim();
        }

        public static bool EqualsByPolicy(string left, string right)
        {
            return string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);
        }

        public static bool TryValidate(
            IReadOnlyList<BlackboardVariable> variables,
            string variableName,
            SerializableGuid? excludedGuid,
            out string normalizedName,
            out string errorMessage)
        {
            normalizedName = Normalize(variableName);
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                errorMessage = EmptyVariableNameError;
                return false;
            }

            if (TryGetVariableWithName(variables, normalizedName, excludedGuid, out BlackboardVariable conflictVariable))
            {
                errorMessage = $"Variable name '{normalizedName}' is already used by '{conflictVariable.Name}'.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        private static bool TryGetVariableWithName(
            IReadOnlyList<BlackboardVariable> variables,
            string variableName,
            SerializableGuid? excludedGuid,
            out BlackboardVariable result)
        {
            result = null;
            if (variables == null)
            {
                return false;
            }

            for (int i = 0; i < variables.Count; i++)
            {
                BlackboardVariable variable = variables[i];
                if (variable == null)
                {
                    continue;
                }

                if (excludedGuid.HasValue && variable.Guid == excludedGuid.Value)
                {
                    continue;
                }

                if (!EqualsByPolicy(variable.Name, variableName))
                {
                    continue;
                }

                result = variable;
                return true;
            }

            return false;
        }
    }
}

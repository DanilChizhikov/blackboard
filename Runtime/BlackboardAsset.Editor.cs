#if UNITY_EDITOR
using System;

namespace DTech.Blackboard
{
    public sealed partial class BlackboardAsset
    {
        public SerializableGuid AddVariable(string variableName, object value)
        {
            for (int i = 0; i < _variables.Count; i++)
            {
                BlackboardVariable variable = _variables[i];
                if (variable.Name == variableName)
                {
                    throw new ArgumentException($"{nameof(BlackboardAsset)}.{nameof(AddVariable)}: Variable with name '{variableName}' already exists");
                }
            }

            BlackboardVariable blackboardVariable = BlackboardVariable.CreateForType(value.GetType(), variableName);
            blackboardVariable.ObjectValue = value;
            _variables.Add(blackboardVariable);
            return blackboardVariable.Guid;
        }
        
        public bool RemoveVariable(string variableName, object value)
        {
            int removeIndex = -1;
            for (int i = 0; i < _variables.Count; i++)
            {
                BlackboardVariable variable = _variables[i];
                if (variable.Name == variableName)
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
using System;
using UnityEngine;

namespace DTech.Blackboard
{
    [Serializable]
    public abstract class BlackboardVariableReference
    {
        [SerializeField] private string _variableName;
        [SerializeField] private SerializableGuid _variableGuid;
    }

    [Serializable]
    public class BlackboardVariableReference<T> : BlackboardVariableReference
    {
        
    }
}
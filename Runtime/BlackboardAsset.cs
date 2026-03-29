using System.Collections.Generic;
using UnityEngine;

namespace DTech.Blackboard
{
    [CreateAssetMenu(fileName = "New Blackboard", menuName = "DTech/Blackboard/BlackboardAsset", order = 0)]
    public sealed partial class BlackboardAsset : ScriptableObject
    {
        [SerializeReference]
        private List<BlackboardVariable> _variables = new();
        
        public RuntimeBlackboard ToRuntimeBlackboard() => new(_variables);
    }
}
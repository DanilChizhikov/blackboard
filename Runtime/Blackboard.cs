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

        public bool TryGetVariable(string name, out BlackboardVariable variable)
        {
            return _holder.TryGetVariable(name, out variable);
        }

        public bool TryGetVariable<T>(string name, out T variable)
            where T : BlackboardVariable
        {
            variable = null;
            if (!_holder.TryGetVariable(name, out BlackboardVariable cachedVariable))
            {
                return false;
            }

            if (cachedVariable is not T genericVariable)
            {
                return false;
            }

            variable = genericVariable;
            return true;
        }
        
        public bool TryGetVariable(SerializableGuid guid, out BlackboardVariable variable)
        {
            return _holder.TryGetVariable(guid, out variable);
        }

        public bool TryGetVariable<T>(SerializableGuid guid, out T variable)
            where T : BlackboardVariable
        {
            variable = null;
            if (!_holder.TryGetVariable(guid, out BlackboardVariable cachedVariable))
            {
                return false;
            }

            if (cachedVariable is not T genericVariable)
            {
                return false;
            }

            variable = genericVariable;
            return true;
        }
        
        public void Dispose()
        {
            _holder.Dispose();
        }
    }
}
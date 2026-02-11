using GroveGames.BehaviourTree.Collections;
using UnityEngine;

namespace GroveGames.BehaviourTree.Unity
{
    public abstract class BehaviourTreeMono : MonoBehaviour
    {
        protected BehaviourTree _tree;
        public BehaviourTree Tree => _tree;

        protected abstract BehaviourTree CreateTree(Blackboard blackboard);

        protected virtual void Awake()
        {
            var blackboard = new Blackboard();
            _tree = CreateTree(blackboard);
        }
    }
}
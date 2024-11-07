namespace GroveGames.BehaviourTree.Nodes
{
    public class ChildAlreadyAttachedException : InvalidOperationException
    {
        public ChildAlreadyAttachedException()
            : base("Child node is already attached.") { }

        public ChildAlreadyAttachedException(string message)
            : base(message) { }

        public ChildAlreadyAttachedException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
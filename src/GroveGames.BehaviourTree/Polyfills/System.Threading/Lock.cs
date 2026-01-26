#if !NET9_0_OR_GREATER

namespace System.Threading;

internal sealed class Lock
{
    private readonly object _lock;

    public Lock()
    {
        _lock = new object();
    }

    public void Enter()
    {
        Monitor.Enter(_lock);
    }

    public void Exit()
    {
        Monitor.Exit(_lock);
    }

    public Scope EnterScope()
    {
        Monitor.Enter(_lock);
        return new Scope(this);
    }

    public ref struct Scope
    {
        private Lock? _lock;

        internal Scope(Lock @lock)
        {
            _lock = @lock;
        }

        public void Dispose()
        {
            _lock?.Exit();
            _lock = null;
        }
    }
}

#endif

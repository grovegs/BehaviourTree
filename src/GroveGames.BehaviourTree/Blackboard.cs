namespace GroveGames.BehaviourTree;

public class Blackboard
{
    public const string DEFAULT_BLACKBOARD_NAME = "DEFAULT";

    private readonly Dictionary<string, object> _defaultBlackboard = [];
    private readonly Dictionary<string, Dictionary<string, object>> _globalBlackboard = [];

    public Blackboard()
    {
        _globalBlackboard[DEFAULT_BLACKBOARD_NAME] = _defaultBlackboard;
    }

    public void SetValue(string key, object value, string blackboardName = DEFAULT_BLACKBOARD_NAME)
    {
        if (!_globalBlackboard.TryGetValue(blackboardName, out var blackboard))
        {
            var newBlackboard = new Dictionary<string, object>
            {
                [key] = value
            };

            _globalBlackboard.TryAdd(blackboardName, newBlackboard);

            return;
        }

        blackboard[key] = value;
    }

    public object GetValue(string key, object defaultValue = null, string blackboardName = DEFAULT_BLACKBOARD_NAME)
    {
        if (!_globalBlackboard.TryGetValue(blackboardName, out var blackboard))
        {
            return defaultValue;
        }

        return blackboard.TryGetValue(key, out var data) ? data : defaultValue;
    }

    public T GetValue<T>(string key, T defaultValue = default, string blackboardName = DEFAULT_BLACKBOARD_NAME)
    {
        if (!_globalBlackboard.TryGetValue(blackboardName, out var blackboard))
        {
            return defaultValue;
        }

        if (!blackboard.TryGetValue(key, out var data))
        {
            return defaultValue;
        }

        if (data == null)
        {
            return defaultValue;
        }

        return (T)data;
    }

    public void DeleteValue(string key, string blackboardName = DEFAULT_BLACKBOARD_NAME)
    {
        if (!_globalBlackboard.TryGetValue(blackboardName, out var blackboard))
        {
            return;
        }

        blackboard[key] = null;
    }

}

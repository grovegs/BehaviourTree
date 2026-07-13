using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Tests.Collections;

public class BlackboardTests
{
    [Fact]
    public void SetValue_NewKey_StoresValue()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();
        var key = new BlackboardKey<int>("testKey");

        // Act
        blackboard.SetValue(key, 42);

        // Assert
        Assert.True(blackboard.TryGetValue(key, out var value));
        Assert.Equal(42, value);
    }

    [Fact]
    public void SetValue_ExistingKey_UpdatesValue()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();
        var key = new BlackboardKey<int>("testKey");
        blackboard.SetValue(key, 42);

        // Act
        blackboard.SetValue(key, 84);

        // Assert
        Assert.True(blackboard.TryGetValue(key, out var value));
        Assert.Equal(84, value);
    }

    [Fact]
    public void TryGetValue_DifferentTypes_ReturnsCorrectValues()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();
        var stringKey = new BlackboardKey<string>("stringKey");
        var intKey = new BlackboardKey<int>("intKey");
        blackboard.SetValue(stringKey, "Hello");
        blackboard.SetValue(intKey, 100);

        // Act
        var stringFound = blackboard.TryGetValue(stringKey, out var stringValue);
        var intFound = blackboard.TryGetValue(intKey, out var intValue);

        // Assert
        Assert.True(stringFound);
        Assert.Equal("Hello", stringValue);
        Assert.True(intFound);
        Assert.Equal(100, intValue);
    }

    [Fact]
    public void TryGetValue_MissingKey_ReturnsFalse()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();
        var key = new BlackboardKey<int>("nonExistentKey");

        // Act
        var found = blackboard.TryGetValue(key, out var value);

        // Assert
        Assert.False(found);
        Assert.Equal(default, value);
    }

    [Fact]
    public void TryGetValue_SameNameDifferentType_ReturnsFalse()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();
        var stringKey = new BlackboardKey<string>("testKey");
        var intKey = new BlackboardKey<int>("testKey");
        blackboard.SetValue(stringKey, "This is a string");

        // Act
        var found = blackboard.TryGetValue(intKey, out _);

        // Assert
        Assert.False(found);
    }

    [Fact]
    public void DeleteValue_ExistingKey_RemovesValue()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();
        var key = new BlackboardKey<int>("testKey");
        blackboard.SetValue(key, 42);

        // Act
        blackboard.DeleteValue(key);

        // Assert
        Assert.False(blackboard.TryGetValue(key, out _));
    }

    [Fact]
    public void DeleteValue_MissingKey_DoesNotThrow()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();
        var key = new BlackboardKey<int>("nonExistentKey");

        // Act
        var exception = Record.Exception(() => blackboard.DeleteValue(key));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Clear_WithValues_RemovesAllValues()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();
        var intKey = new BlackboardKey<int>("intKey");
        var stringKey = new BlackboardKey<string>("stringKey");
        blackboard.SetValue(intKey, 42);
        blackboard.SetValue(stringKey, "Hello");

        // Act
        blackboard.Clear();

        // Assert
        Assert.False(blackboard.TryGetValue(intKey, out _));
        Assert.False(blackboard.TryGetValue(stringKey, out _));
    }

    [Fact]
    public void TryGetValue_ReferenceTypeWrongKeyType_ReturnsFalse()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();
        var stringKey = new BlackboardKey<string>("testKey");
        var arrayKey = new BlackboardKey<int[]>("testKey");
        blackboard.SetValue(stringKey, "Hello");

        // Act
        var found = blackboard.TryGetValue(arrayKey, out _);

        // Assert
        Assert.False(found);
    }

    [Fact]
    public void SetValue_SameNameDifferentReferenceType_OverwritesValue()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();
        var stringKey = new BlackboardKey<string>("testKey");
        var arrayKey = new BlackboardKey<int[]>("testKey");
        blackboard.SetValue(stringKey, "Hello");

        // Act
        blackboard.SetValue(arrayKey, [1, 2, 3]);

        // Assert
        Assert.False(blackboard.TryGetValue(stringKey, out _));
        Assert.True(blackboard.TryGetValue(arrayKey, out var array));
        Assert.Equal([1, 2, 3], array);
    }

    [Fact]
    public void DeleteValue_ReferenceTypeWrongKeyType_DoesNotRemoveValue()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();
        var stringKey = new BlackboardKey<string>("testKey");
        var arrayKey = new BlackboardKey<int[]>("testKey");
        blackboard.SetValue(stringKey, "Hello");

        // Act
        blackboard.DeleteValue(arrayKey);

        // Assert
        Assert.True(blackboard.TryGetValue(stringKey, out var value));
        Assert.Equal("Hello", value);
    }

    [Fact]
    public void Constructor_NullName_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BlackboardKey<int>(null!));
    }
}

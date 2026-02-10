using GroveGames.BehaviourTree.Collections;

namespace GroveGames.BehaviourTree.Tests.Collections;

public class BlackboardTests
{
    [Fact]
    public void SetValue_ShouldStoreValueInBlackboard()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();

        // Act
        blackboard.SetValue("testKey", 42);

        // Assert
        Assert.Equal(42, blackboard.GetValue<int>("testKey"));
    }

    [Fact]
    public void GetValue_ShouldReturnCorrectValueType()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();
        blackboard.SetValue("stringKey", "Hello");
        blackboard.SetValue("intKey", 100);

        // Act
        string stringValue = blackboard.GetValue<string>("stringKey")!;
        int intValue = blackboard.GetValue<int>("intKey");

        // Assert
        Assert.Equal("Hello", stringValue);
        Assert.Equal(100, intValue);
    }

    [Fact]
    public void GetValue_ShouldReturnDefaultValueIfKeyDoesNotExist()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();

        // Act
        int result = blackboard.GetValue<int>("nonExistentKey");

        // Assert
        Assert.Equal(default(int), result); // Should be 0 for int
    }

    [Fact]
    public void DeleteValue_ShouldRemoveKeyFromBlackboard()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();
        blackboard.SetValue("testKey", 42);

        // Act
        blackboard.DeleteValue("testKey");

        // Assert
        Assert.Equal(default(int), blackboard.GetValue<int>("testKey"));
    }

    [Fact]
    public void SetValue_ShouldUpdateExistingKey()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();
        blackboard.SetValue("testKey", 42);

        // Act
        blackboard.SetValue("testKey", 84);

        // Assert
        Assert.Equal(84, blackboard.GetValue<int>("testKey"));
    }

    [Fact]
    public void GetValue_ShouldReturnNullIfTypeMismatch()
    {
        // Arrange
        IBlackboard blackboard = new Blackboard();
        blackboard.SetValue("testKey", "This is a string");

        // Act and Assert
        Assert.Throws<InvalidCastException>(() => blackboard.GetValue<int>("testKey"));
    }
}

using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.UserMessages;
using Xunit;

namespace NativeTestsPlugin;

public class UserMessageTests
{
    [Fact]
    public void FindMessageIdByName_ReturnsValidId()
    {
        var messageId = UserMessage.FindIdByName("TextMsg");

        Assert.True(messageId > 0, $"TextMsg message ID should be positive, got {messageId}");
    }

    [Fact]
    public void CreateUserMessage_ReturnsValidPointer()
    {
        var messagePtr = NativeAPI.UsermessageCreate("TextMsg");

        try
        {
            Assert.NotEqual(IntPtr.Zero, messagePtr);
        }
        finally
        {
            if (messagePtr != IntPtr.Zero)
            {
                NativeAPI.UsermessageDelete(new UserMessage(messagePtr));
            }
        }
    }

    [Fact]
    public void CreateUserMessageById_ReturnsValidPointer()
    {
        var messageId = UserMessage.FindIdByName("TextMsg");
        var messagePtr = NativeAPI.UsermessageCreatebyid(messageId);

        try
        {
            Assert.NotEqual(IntPtr.Zero, messagePtr);
        }
        finally
        {
            if (messagePtr != IntPtr.Zero)
            {
                NativeAPI.UsermessageDelete(new UserMessage(messagePtr));
            }
        }
    }

    [Fact]
    public void GetUsermessageId_ReturnsCorrectId()
    {
        using var userMessage = UserMessage.FromPartialName("TextMsg");

        var id = userMessage.Id;
        var expectedId = UserMessage.FindIdByName("TextMsg");

        Assert.Equal(expectedId, id);
    }

    [Fact]
    public void GetUsermessageName_ReturnsCorrectName()
    {
        using var userMessage = UserMessage.FromPartialName("TextMsg");

        var name = userMessage.Name;

        Assert.NotNull(name);
        Assert.Contains("TextMsg", name);
    }

    [Fact]
    public void GetUsermessageType_ReturnsValidType()
    {
        using var userMessage = UserMessage.FromPartialName("TextMsg");

        var type = userMessage.Type;

        Assert.NotNull(type);
        Assert.NotEmpty(type);
    }

    [Fact]
    public void PbHasField_ReturnsFalseForMissingField()
    {
        using var userMessage = UserMessage.FromPartialName("TextMsg");

        var hasField = userMessage.HasField("nonexistent_field_xyz");

        Assert.False(hasField);
    }

    [Fact]
    public void PbSetAndReadInt_Works()
    {
        using var userMessage = UserMessage.FromPartialName("TextMsg");

        // TextMsg has a "dest" field which is an int
        var testValue = 3;
        userMessage.SetInt("dest", testValue);
        var result = userMessage.ReadInt("dest");

        Assert.Equal(testValue, result);
    }

    [Fact]
    public void PbSetAndReadBool_Works()
    {
        // Use a message with a bool field - SayText2 has chat field
        using var userMessage = UserMessage.FromPartialName("SayText2");

        var testValue = true;
        userMessage.SetBool("chat", testValue);
        var result = userMessage.ReadBool("chat");

        Assert.Equal(testValue, result);
    }

    [Fact]
    public void PbSetAndReadString_Works()
    {
        using var userMessage = UserMessage.FromPartialName("TextMsg");

        // TextMsg has a "param" repeated string field
        var testValue = "test_string";
        userMessage.AddString("param", testValue);
        var result = userMessage.ReadString("param", 0);

        Assert.Equal(testValue, result);
    }

    [Fact]
    public void PbGetRepeatedFieldCount_ReturnsZeroInitially()
    {
        using var userMessage = UserMessage.FromPartialName("TextMsg");

        var count = userMessage.GetRepeatedFieldCount("param");

        Assert.Equal(0, count);
    }

    [Fact]
    public void PbAddString_IncreasesFieldCount()
    {
        using var userMessage = UserMessage.FromPartialName("TextMsg");

        // Add strings to param (repeated field)
        var initialCount = userMessage.GetRepeatedFieldCount("param");
        userMessage.AddString("param", "test1");
        var newCount = userMessage.GetRepeatedFieldCount("param");

        Assert.Equal(initialCount + 1, newCount);
    }

    [Fact]
    public void UsermessageDelete_CleansUpMessage()
    {
        var userMessage = UserMessage.FromPartialName("TextMsg");

        // Should not throw
        var exception = Record.Exception(() => userMessage.Dispose());

        Assert.Null(exception);
    }

    [Fact]
    public void UserMessage_HighLevelApi_FromPartialName_Works()
    {
        using var userMessage = UserMessage.FromPartialName("TextMsg");

        Assert.NotNull(userMessage);
        Assert.NotEqual(IntPtr.Zero, userMessage.Handle);
    }

    [Fact]
    public void GetDebugString_ReturnsValidString()
    {
        using var userMessage = UserMessage.FromPartialName("TextMsg");

        // Set some values first
        userMessage.SetInt("dest", 1);

        var debugString = userMessage.DebugString;

        Assert.NotNull(debugString);
        // Debug string should contain something about the message
        Assert.True(debugString.Length > 0, "Debug string should not be empty");
    }
}

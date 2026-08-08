//namespace Cubusky.Stats.Tests;

//public class ValueStatTests
//{
//    // Concrete subclass for testing the abstract ValueStat<T>
//    private sealed class TestValueStat<TValue>(TValue value, IEqualityComparer<TValue>? comparer = null)
//        : ValueStat<TValue>(value, comparer);

//    [Fact]
//    public void Constructor_SetsInitialValue()
//    {
//        var stat = new TestValueStat<int>(42);
//        stat.Value.ShouldBe(42);
//    }

//    [Fact]
//    public void Constructor_UsesDefaultEqualityComparer_WhenNullComparerProvided()
//    {
//        var stat = new TestValueStat<int>(1, null);
//        stat.Comparer.ShouldBe(EqualityComparer<int>.Default);
//    }

//    [Fact]
//    public void Constructor_UsesProvidedEqualityComparer()
//    {
//        var comparer = StringComparer.OrdinalIgnoreCase;
//        var stat = new TestValueStat<string>("hello", comparer);
//        stat.Comparer.ShouldBe(comparer);
//    }

//    [Fact]
//    public void Value_Get_ReturnsCurrentValue()
//    {
//        var stat = new TestValueStat<int>(10);
//        stat.Value.ShouldBe(10);
//    }

//    [Fact]
//    public void Value_Set_UpdatesValue()
//    {
//        var stat = new TestValueStat<int>(1)
//        {
//            Value = 99
//        };
//        stat.Value.ShouldBe(99);
//    }

//    [Fact]
//    public void Value_Set_SameValue_DoesNotChangeValue()
//    {
//        var stat = new TestValueStat<int>(5)
//        {
//            Value = 5
//        };
//        stat.Value.ShouldBe(5);
//    }

//    [Fact]
//    public void PerformUpdateValueOp_DifferentValue_BroadcastsAndReturnsTrue()
//    {
//        var stat = new TestValueStat<int>(0);
//        var received = new List<int>();

//        using var binding = stat.Bind();
//        binding.OnValue(received.Add);

//        stat.Value = 7;

//        stat.Value.ShouldBe(7);
//        received.ShouldHaveSingleItem().ShouldBe(7);
//    }

//    [Fact]
//    public void PerformUpdateValueOp_SameValue_DoesNotBroadcast()
//    {
//        var stat = new TestValueStat<int>(3);
//        var received = new List<int>();

//        using var binding = stat.Bind();
//        binding.OnValue(received.Add);

//        stat.Value = 3;

//        received.ShouldBeEmpty();
//    }

//    [Fact]
//    public void PerformUpdateValueOp_WithCustomComparer_UsesThatComparer()
//    {
//        var stat = new TestValueStat<string>("Hello", StringComparer.OrdinalIgnoreCase);
//        var received = new List<string>();

//        using var binding = stat.Bind();
//        binding.OnValue(received.Add);

//        // "hello" is equal to "Hello" with OrdinalIgnoreCase, so no broadcast expected
//        stat.Value = "hello";
//        received.ShouldBeEmpty();
//        // Value should remain "Hello" since update was rejected
//        stat.Value.ShouldBe("Hello");
//    }

//    [Fact]
//    public void Bind_ReturnsBinding()
//    {
//        var stat = new TestValueStat<int>(0);
//        using var binding = stat.Bind();
//        binding.ShouldNotBeNull();
//    }

//    [Fact]
//    public void Bind_Callback_ReceivesMultipleUpdates()
//    {
//        var stat = new TestValueStat<int>(0);
//        var received = new List<int>();

//        using var binding = stat.Bind();
//        binding.OnValue(received.Add);

//        stat.Value = 1;
//        stat.Value = 2;
//        stat.Value = 3;

//        received.ShouldBe([1, 2, 3]);
//    }

//    [Fact]
//    public void ClearBindings_RemovesAllCallbacks()
//    {
//        var stat = new TestValueStat<int>(0);
//        var received = new List<int>();

//        var binding = stat.Bind();
//        binding.OnValue(received.Add);

//        stat.ClearBindings();

//        stat.Value = 10;

//        received.ShouldBeEmpty();
//    }

//    [Fact]
//    public void Dispose_AllowsGarbageCollection_WithoutError()
//    {
//        var stat = new TestValueStat<int>(0);
//        stat.Dispose();
//        // No exception should be thrown
//    }

//    [Fact]
//    public void Dispose_CanBeCalledMultipleTimes_WithoutError()
//    {
//        var stat = new TestValueStat<int>(0);
//        stat.Dispose();
//        stat.Dispose();
//    }

//    [Fact]
//    public void Binding_Constructor_CreatesBindingInstance()
//    {
//        var stat = new TestValueStat<int>(0);
//        using var binding = stat.Bind();
//        binding.ShouldNotBeNull();
//        binding.ShouldBeOfType<ValueStat<int>.Binding>();
//    }

//    [Fact]
//    public void OnValue_ReturnsBinding_ForChaining()
//    {
//        var stat = new TestValueStat<int>(0);
//        using var binding = stat.Bind();
//        var result = binding.OnValue(_ => { });
//        result.ShouldBeSameAs(binding);
//    }

//    [Fact]
//    public void OnValue_WithCondition_InvokesCallbackOnlyWhenConditionIsTrue()
//    {
//        var stat = new TestValueStat<int>(0);
//        var received = new List<int>();

//        using var binding = stat.Bind();
//        binding.OnValue(received.Add, v => v > 5);

//        stat.Value = 3;
//        stat.Value = 7;
//        stat.Value = 4;
//        stat.Value = 10;

//        received.ShouldBe([7, 10]);
//    }

//    [Fact]
//    public void OnValue_WithConditionAlwaysFalse_NeverInvokesCallback()
//    {
//        var stat = new TestValueStat<int>(0);
//        var received = new List<int>();

//        using var binding = stat.Bind();
//        binding.OnValue(received.Add, _ => false);

//        stat.Value = 1;
//        stat.Value = 2;

//        received.ShouldBeEmpty();
//    }
//}

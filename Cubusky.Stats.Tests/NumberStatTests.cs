//namespace Cubusky.Stats.Tests;

//public class NumberStatTests
//{
//    private sealed class TestNumberStat<TNumber> : NumberStat<TNumber>
//        where TNumber : System.Numerics.INumber<TNumber>
//    {
//        public TestNumberStat(TNumber value, TNumber min, TNumber max, bool clampMin = true, bool clampMax = true, IEqualityComparer<TNumber>? comparer = null)
//            : base(value, min, max, clampMin, clampMax, comparer) { }

//        public TestNumberStat(TNumber value, TNumber max, bool clampMax = true, IEqualityComparer<TNumber>? comparer = null)
//            : base(value, max, clampMax, comparer) { }
//    }

//    // ---- Constructors ----

//    [Fact]
//    public void Constructor_ValueMaxOverload_SetsDefaultsCorrectly()
//    {
//        var stat = new TestNumberStat<int>(5, 10);

//        stat.Min.ShouldBe(0);
//        stat.Max.ShouldBe(10);
//        stat.Value.ShouldBe(5);
//        stat.ClampMin.ShouldBeTrue();
//        stat.ClampMax.ShouldBeTrue();
//    }

//    [Fact]
//    public void Constructor_ValueMaxOverload_ClampMaxFalse_ValueNotClamped()
//    {
//        var stat = new TestNumberStat<int>(20, 10, clampMax: false);

//        stat.Value.ShouldBe(20);
//        stat.ClampMax.ShouldBeFalse();
//    }

//    // ---- Min / Max property get, set, broadcast ----

//    [Fact]
//    public void Min_Set_UpdatesMinAndBroadcasts()
//    {
//        var stat = new TestNumberStat<int>(5, 0, 10);
//        var received = new List<int>();
//        using var binding = stat.Bind();
//        binding.OnMin(received.Add);

//        stat.Min = 3;

//        stat.Min.ShouldBe(3);
//        received.ShouldHaveSingleItem().ShouldBe(3);
//    }

//    [Fact]
//    public void Max_Set_UpdatesMaxAndBroadcasts()
//    {
//        var stat = new TestNumberStat<int>(5, 0, 10);
//        var received = new List<int>();
//        using var binding = stat.Bind();
//        binding.OnMax(received.Add);

//        stat.Max = 15;

//        stat.Max.ShouldBe(15);
//        received.ShouldHaveSingleItem().ShouldBe(15);
//    }

//    [Fact]
//    public void Min_Set_SameValue_DoesNotBroadcast()
//    {
//        var stat = new TestNumberStat<int>(5, 2, 10);
//        var received = new List<int>();
//        using var binding = stat.Bind();
//        binding.OnMin(received.Add);

//        stat.Min = 2;
//        received.ShouldBeEmpty();
//    }

//    [Fact]
//    public void Max_Set_SameValue_DoesNotBroadcast()
//    {
//        var stat = new TestNumberStat<int>(5, 0, 10);
//        var received = new List<int>();
//        using var binding = stat.Bind();
//        binding.OnMax(received.Add);

//        stat.Max = 10;
//        received.ShouldBeEmpty();
//    }

//    // ---- Min/Max boundary enforcement ----

//    [Fact]
//    public void Min_Set_GreaterThanMax_ClampsToMax()
//    {
//        var stat = new TestNumberStat<int>(5, 0, 10) { Min = 15 };
//        stat.Min.ShouldBe(10);
//    }

//    [Fact]
//    public void Max_Set_LessThanMin_ClampsToMin()
//    {
//        var stat = new TestNumberStat<int>(5, 2, 10) { Max = -5 };
//        stat.Max.ShouldBe(2);
//    }

//    // ---- Value reclamped when Min/Max changes ----

//    [Fact]
//    public void Min_Set_AboveCurrentValue_ReclampsValueAndBroadcasts()
//    {
//        var stat = new TestNumberStat<int>(5, 0, 10);
//        var valueReceived = new List<int>();
//        using var binding = stat.Bind();
//        binding.OnValue(valueReceived.Add);

//        stat.Min = 8;

//        stat.Value.ShouldBe(8);
//        valueReceived.ShouldHaveSingleItem().ShouldBe(8);
//    }

//    [Fact]
//    public void Max_Set_BelowCurrentValue_ReclampsValueAndBroadcasts()
//    {
//        var stat = new TestNumberStat<int>(5, 0, 10);
//        var valueReceived = new List<int>();
//        using var binding = stat.Bind();
//        binding.OnValue(valueReceived.Add);

//        stat.Max = 3;

//        stat.Value.ShouldBe(3);
//        valueReceived.ShouldHaveSingleItem().ShouldBe(3);
//    }

//    // ---- Value clamping: all clampMin x clampMax combinations ----

//    [Theory]
//    [InlineData(true, true, -1, 2)]   // below min, clampMin      => clamped to min
//    [InlineData(true, true, 100, 10)]   // above max, clampMax      => clamped to max
//    [InlineData(false, true, -1, -1)]   // above max, clampMax only => clamped to max
//    [InlineData(false, true, 100, 10)]   // below min, no clampMin   => unchanged
//    [InlineData(true, false, -1, 2)]   // below min, clampMin only => clamped to min
//    [InlineData(true, false, 100, 100)]   // above max, no clampMax   => unchanged
//    [InlineData(false, false, -1, -1)]   // no clamping at all
//    [InlineData(false, false, 100, 100)]   // no clamping at all
//    public void Value_Set_Clamping(bool clampMin, bool clampMax, int input, int expected)
//    {
//        var stat = new TestNumberStat<int>(5, 2, 10, clampMin, clampMax) { Value = input };
//        stat.Value.ShouldBe(expected);
//    }

//    [Fact]
//    public void Value_Set_SameValue_DoesNotBroadcast()
//    {
//        var stat = new TestNumberStat<int>(5, 0, 10);
//        var received = new List<int>();
//        using var binding = stat.Bind();
//        binding.OnValue(received.Add);

//        stat.Value = 5;
//        received.ShouldBeEmpty();
//    }

//    // ---- MinClamped / MaxClamped broadcasts ----

//    [Theory]
//    [InlineData(true, true, 0, true)]   // below min, clampMin      => broadcasts
//    [InlineData(true, false, 0, true)]   // below min, clampMin only => broadcasts
//    [InlineData(true, true, 7, false)]  // within range             => no broadcast
//    [InlineData(false, true, -5, false)]  // clampMin=false           => no broadcast
//    public void Value_Set_MinClamped_Broadcast(bool clampMin, bool clampMax, int input, bool shouldBroadcast)
//    {
//        var stat = new TestNumberStat<int>(5, 2, 10, clampMin, clampMax);
//        var clamped = new List<int>();
//        using var binding = stat.Bind();
//        binding.OnMinClamped(clamped.Add);

//        stat.Value = input;

//        if (shouldBroadcast)
//            clamped.ShouldHaveSingleItem().ShouldBe(2);
//        else
//            clamped.ShouldBeEmpty();
//    }

//    [Theory]
//    [InlineData(true, true, 20, true)]   // above max, clampMax      => broadcasts
//    [InlineData(false, true, 99, true)]   // above max, clampMax only => broadcasts
//    [InlineData(true, true, 7, false)]  // within range             => no broadcast
//    [InlineData(true, false, 99, false)]  // clampMax=false           => no broadcast
//    public void Value_Set_MaxClamped_Broadcast(bool clampMin, bool clampMax, int input, bool shouldBroadcast)
//    {
//        var stat = new TestNumberStat<int>(5, 2, 10, clampMin, clampMax);
//        var clamped = new List<int>();
//        using var binding = stat.Bind();
//        binding.OnMaxClamped(clamped.Add);

//        stat.Value = input;

//        if (shouldBroadcast)
//            clamped.ShouldHaveSingleItem().ShouldBe(10);
//        else
//            clamped.ShouldBeEmpty();
//    }

//    // ---- Binding: type and fluent chaining ----

//    [Fact]
//    public void Bind_ReturnsNumberStatBinding()
//    {
//        var stat = new TestNumberStat<int>(5, 0, 10);
//        using var binding = stat.Bind();
//        binding.ShouldNotBeNull();
//        binding.ShouldBeOfType<NumberStat<int>.Binding>();
//    }

//    [Fact]
//    public void Binding_AllOnMethods_ReturnSameInstance()
//    {
//        var stat = new TestNumberStat<int>(5, 2, 10, clampMin: true, clampMax: true);
//        using var binding = stat.Bind();

//        binding.OnMin(_ => { }).ShouldBeSameAs(binding);
//        binding.OnMax(_ => { }).ShouldBeSameAs(binding);
//        binding.OnMinClamped(_ => { }).ShouldBeSameAs(binding);
//        binding.OnMaxClamped(_ => { }).ShouldBeSameAs(binding);
//    }

//    // ---- Binding condition filtering ----

//    [Theory]
//    [InlineData(true, 1)] // condition passes => callback fired
//    [InlineData(false, 0)] // condition fails  => callback suppressed
//    public void OnMin_WithCondition_FiltersCallback(bool condition, int expectedCount)
//    {
//        var stat = new TestNumberStat<int>(5, 0, 10);
//        var received = new List<int>();
//        using var binding = stat.Bind();
//        binding.OnMin(received.Add, _ => condition);

//        stat.Min = 3;
//        received.Count.ShouldBe(expectedCount);
//    }

//    [Theory]
//    [InlineData(true, 1)]
//    [InlineData(false, 0)]
//    public void OnMax_WithCondition_FiltersCallback(bool condition, int expectedCount)
//    {
//        var stat = new TestNumberStat<int>(5, 0, 10);
//        var received = new List<int>();
//        using var binding = stat.Bind();
//        binding.OnMax(received.Add, _ => condition);

//        stat.Max = 20;
//        received.Count.ShouldBe(expectedCount);
//    }

//    [Theory]
//    [InlineData(true, 1)]
//    [InlineData(false, 0)]
//    public void OnMinClamped_WithCondition_FiltersCallback(bool condition, int expectedCount)
//    {
//        var stat = new TestNumberStat<int>(5, 2, 10, clampMin: true);
//        var received = new List<int>();
//        using var binding = stat.Bind();
//        binding.OnMinClamped(received.Add, _ => condition);

//        stat.Value = 0; // triggers min clamp
//        received.Count.ShouldBe(expectedCount);
//    }

//    [Theory]
//    [InlineData(true, 1)]
//    [InlineData(false, 0)]
//    public void OnMaxClamped_WithCondition_FiltersCallback(bool condition, int expectedCount)
//    {
//        var stat = new TestNumberStat<int>(5, 2, 10, clampMax: true);
//        var received = new List<int>();
//        using var binding = stat.Bind();
//        binding.OnMaxClamped(received.Add, _ => condition);

//        stat.Value = 20; // triggers max clamp
//        received.Count.ShouldBe(expectedCount);
//    }
//}

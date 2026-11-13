using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Tests;

[CollectionDefinition("Native memory", DisableParallelization = true)]
public class NativeMemoryCollection
{
}

[Collection("Native memory")]
public class NativeHandleTrackerTests
{
    private static readonly TimeSpan SweepTime = TimeSpan.FromSeconds(10);

    [Fact]
    public void NewLiveAllocationsDoNotDisplaceOldDeadEntries()
    {
        var tracker = new NativeHandleTracker();
        AddDead(tracker, 768);
        Collect();
        var retained = new List<TestObject>();

        for (var iteration = 0; iteration < 3; iteration++)
        {
            AddRetained(tracker, retained);
            Assert.Equal(256, tracker.Cleanup(256, SweepTime));
        }

        Assert.Equal(retained.Count, tracker.Count);
        VerifySparse(retained);
        retained.Clear();
        Drain(tracker);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(255)]
    [InlineData(256)]
    [InlineData(257)]
    [InlineData(513)]
    public void SingleEntrySlicesPreserveMixedRecordsAcrossBatchBoundaries(int count)
    {
        var tracker = new NativeHandleTracker();
        for (var cycle = 0; cycle < 2; cycle++)
        {
            var retained = CreateRetained(tracker, count);
            var dead = 0;
            for (var index = 0; index < count; index++)
            {
                if (index % 3 == cycle)
                {
                    retained[index] = null!;
                    dead++;
                }
            }
            Collect();

            var released = 0;
            for (var index = 0; index < count; index++)
            {
                released += tracker.Cleanup(1, SweepTime);
            }
            Assert.Equal(dead, released);
            Assert.Equal(count - dead, tracker.Count);
            VerifySparse(retained);
            retained.Clear();
            Drain(tracker);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void VerifySparse(List<TestObject> retained)
    {
        for (var index = 0; index < retained.Count; index++)
        {
            if (retained[index] is { } owner)
            {
                Assert.Equal(index, Marshal.ReadInt32(owner.Handle));
            }
        }
        GC.KeepAlive(retained);
    }

    [Fact]
    public void FinalizationAndResurrectionKeepTheNativeAllocationAlive()
    {
        var tracker = new NativeHandleTracker();
        CreateResurrecting(tracker);
        Collect();
        Assert.Equal(0, tracker.Cleanup(int.MaxValue, SweepTime));
        Assert.Equal(123, ReadResurrected());
        ResurrectingObject.Saved = null;
        Drain(tracker);
    }

    [Fact]
    public void AllMathTypesAllocateLazilyAndShareOneHandleUnderContention()
    {
        NativeObject[] values =
        [
            new Vector(1, 2, 3), new QAngle(1, 2, 3), new Angle(1, 2, 3), new Vector2D(1, 2),
            new Vector4D(1, 2, 3, 4), new Quaternion(1, 2, 3, 4), new matrix3x4_t(1, 2, 3, 4)
        ];
        foreach (var value in values)
        {
            Assert.Equal(IntPtr.Zero, value.RawHandle);
            var handles = new IntPtr[32];
            Parallel.For(0, handles.Length, index => handles[index] = value.Handle);
            Assert.NotEqual(IntPtr.Zero, handles[0]);
            Assert.All(handles, handle => Assert.Equal(handles[0], handle));
            var components = new float[2];
            Marshal.Copy(handles[0], components, 0, components.Length);
            Assert.Equal(new[] { 1f, 2f }, components);
            GC.KeepAlive(value);
        }
        Array.Clear(values);
    }

    [Fact]
    public void FailedRegistrationLeavesInitializationRetryable()
    {
        var handle = IntPtr.Zero;
        Assert.Throws<ArgumentNullException>(() => NativeHandleTracker.Allocate(null!, ref handle, [1, 2, 3]));
        Assert.Equal(IntPtr.Zero, handle);

        var owner = new TestObject(IntPtr.Zero);
        var allocated = NativeHandleTracker.Allocate(owner, ref handle, [4, 5, 6]);
        Assert.Equal(allocated, handle);
        Assert.Equal(4f, BitConverter.Int32BitsToSingle(Marshal.ReadInt32(handle)));
        GC.KeepAlive(owner);
    }

    [Fact]
    public void BorrowedHandlesAreNeverRegistered()
    {
        var pointer = Marshal.AllocHGlobal(12);
        try
        {
            Marshal.WriteInt32(pointer, BitConverter.SingleToInt32Bits(123));
            CheckBorrowed(pointer);
            Collect();
            NativeHandleTracker.Shared.Cleanup(int.MaxValue, SweepTime);
            Assert.Equal(123f, BitConverter.Int32BitsToSingle(Marshal.ReadInt32(pointer)));
        }
        finally
        {
            Marshal.FreeHGlobal(pointer);
        }
    }

    [Fact]
    public void ScriptArgumentsKeepOwnersAliveUntilTheCallIsReset()
    {
        var context = new ScriptContext();
        var target = PushArgument(context);
        Collect();
        Assert.True(IsAlive(target));

        // A nested reset during a native invocation must not release the outer invocation's arguments.
        context.isCleanupLocked = true;
        context.Reset();
        Collect();
        Assert.True(IsAlive(target));
        context.isCleanupLocked = false;
        context.Reset();
        Collect();
        Assert.False(IsAlive(target));
        GC.KeepAlive(context);
    }

    private static void Drain(NativeHandleTracker tracker)
    {
        for (var attempt = 0; attempt < 4 && tracker.Count > 0; attempt++)
        {
            Collect();
            tracker.Cleanup(int.MaxValue, SweepTime);
        }
        Assert.Equal(0, tracker.Count);
    }

    private static void Collect()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static TestObject Create(NativeHandleTracker tracker, int value)
    {
        var pointer = Marshal.AllocHGlobal(12);
        Marshal.WriteInt32(pointer, value);
        var owner = new TestObject(pointer);
        try
        {
            tracker.Register(owner, pointer);
        }
        catch
        {
            Marshal.FreeHGlobal(pointer);
            throw;
        }

        return owner;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void AddDead(NativeHandleTracker tracker, int count)
    {
        for (var index = 0; index < count; index++)
        {
            GC.KeepAlive(Create(tracker, index));
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static List<TestObject> CreateRetained(NativeHandleTracker tracker, int count)
    {
        var result = new List<TestObject>(count);
        for (var index = 0; index < count; index++)
        {
            result.Add(Create(tracker, index));
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void AddRetained(NativeHandleTracker tracker, List<TestObject> retained)
    {
        retained.Add(Create(tracker, retained.Count));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CreateResurrecting(NativeHandleTracker tracker)
    {
        var pointer = Marshal.AllocHGlobal(12);
        Marshal.WriteInt32(pointer, 123);
        var owner = new ResurrectingObject(pointer);
        tracker.Register(owner, pointer);
        GC.KeepAlive(owner);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ReadResurrected()
    {
        return Marshal.ReadInt32(ResurrectingObject.Saved!.Handle);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void CheckBorrowed(IntPtr pointer)
    {
        var borrowed = new Vector(pointer);
        Assert.Equal(pointer, borrowed.Handle);
        Assert.Equal(123f, borrowed.X);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference<NativeObject> PushArgument(ScriptContext context)
    {
        var owner = new Vector(1, 2, 3);
        InputArgument argument = owner;
        context.Push(argument);
        return new WeakReference<NativeObject>(owner);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool IsAlive(WeakReference<NativeObject> target)
    {
        return target.TryGetTarget(out _);
    }

    private sealed class TestObject : NativeObject
    {
        public TestObject(IntPtr pointer) : base(pointer)
        {
        }
    }

    private sealed class ResurrectingObject : NativeObject
    {
        internal static ResurrectingObject? Saved;

        public ResurrectingObject(IntPtr pointer) : base(pointer)
        {
        }

        ~ResurrectingObject()
        {
            Saved = this;
        }
    }
}

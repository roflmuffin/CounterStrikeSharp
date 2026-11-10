using System.Diagnostics;
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
    public void KeepsLiveAllocationsAndReclaimsDeadOnes()
    {
        var tracker = new NativeHandleTracker();
        var retained = CreateRetained(tracker, 1000);
        AddDead(tracker, 10000);
        Collect();

        Assert.Equal(10000, tracker.Cleanup(int.MaxValue, SweepTime));
        Assert.Equal(retained.Count, tracker.Count);
        Verify(retained);
        retained.Clear();
        Drain(tracker);
    }

    [Fact]
    public void NewLiveAllocationsDoNotDisplaceOldDeadEntries()
    {
        var tracker = new NativeHandleTracker();
        AddDead(tracker, 30000);
        Collect();
        var retained = new List<TestObject>();

        for (var iteration = 0; iteration < 30; iteration++)
        {
            AddRetained(tracker, retained, 1000);
            Assert.Equal(1000, tracker.Cleanup(1000, SweepTime));
        }

        Assert.Equal(retained.Count, tracker.Count);
        Verify(retained);
        retained.Clear();
        Drain(tracker);
    }

    [Fact]
    public void SustainedAllocationsDrainWithinTheSameSweepBudget()
    {
        var tracker = new NativeHandleTracker();
        for (var second = 0; second < 20; second++)
        {
            AddDead(tracker, 32000);
            Collect();
            var released = 0;
            for (var slice = 0; slice < 64; slice++)
            {
                released += tracker.Cleanup(512, SweepTime);
            }

            Assert.Equal(32000, released);
            Assert.Equal(0, tracker.Count);
        }
    }

    [Fact]
    public async Task ConcurrentProducersAndCleanersReleaseEveryAllocation()
    {
        var tracker = new NativeHandleTracker();
        var stop = 0;
        var released = 0;
        var cleaners = Enumerable.Range(0, 2).Select(_ => Task.Run(() =>
        {
            while (Volatile.Read(ref stop) == 0)
            {
                Interlocked.Add(ref released, tracker.Cleanup(512, TimeSpan.FromMilliseconds(1)));
                Thread.Yield();
            }
        })).ToArray();

        try
        {
            await Task.WhenAll(Enumerable.Range(0, 4).Select(_ => Task.Run(() =>
            {
                for (var iteration = 0; iteration < 10; iteration++)
                {
                    var retained = CreateRetained(tracker, 1000);
                    Collect();
                    Verify(retained);
                    retained.Clear();
                }
            })));
        }
        finally
        {
            Volatile.Write(ref stop, 1);
            await Task.WhenAll(cleaners);
        }

        released += Drain(tracker);
        Assert.Equal(40000, released);
    }

    [Fact]
    public void RegistersCompactRecordsWithoutPerEntryManagedObjects()
    {
        var tracker = new NativeHandleTracker();
        RegisterAndVerifyCompactRecords(tracker, 4096);
        // Exit the owner's stack frame before collecting, including Debug JIT temporaries.
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

    [Fact]
    public void SparseSurvivorsMergeWithoutAllocatingAndRemainValidAcrossSweeps()
    {
        var tracker = new NativeHandleTracker();
        var retained = CreateRetained(tracker, 8192);
        for (var index = 0; index < retained.Count; index++)
        {
            if (index % 256 != 0)
            {
                retained[index] = null!;
            }
        }
        Collect();
        Assert.Equal(8160, tracker.Cleanup(int.MaxValue, SweepTime));
        Assert.Equal(32, tracker.Count);
        VerifySparse(retained);

        // Inspect storage only to guard against one long-lived owner pinning every original batch.
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        var batches = (System.Collections.ICollection)typeof(NativeHandleTracker).GetField("_batches", flags)!.GetValue(tracker)!;
        Assert.Equal(1, batches.Count);

        for (var sweep = 0; sweep < 3; sweep++)
        {
            Collect();
            var before = GC.GetAllocatedBytesForCurrentThread();
            var released = tracker.Cleanup(int.MaxValue, SweepTime);
            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
            Assert.Equal(0, released);
            Assert.Equal(0, allocated);
            VerifySparse(retained);
        }
        retained.Clear();
        Drain(tracker);
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

    [Fact]
    public async Task SharedTimerReclaimsAnIdleBacklog()
    {
        Drain(NativeHandleTracker.Shared);
        AddDead(NativeHandleTracker.Shared, 10000);
        Collect();
        var timeout = Stopwatch.StartNew();
        while (NativeHandleTracker.Shared.Count > 0 && timeout.Elapsed < TimeSpan.FromSeconds(10))
        {
            await Task.Delay(20);
        }

        Assert.Equal(0, NativeHandleTracker.Shared.Count);
    }

    private static int Drain(NativeHandleTracker tracker)
    {
        var released = 0;
        for (var attempt = 0; attempt < 4 && tracker.Count > 0; attempt++)
        {
            Collect();
            released += tracker.Cleanup(int.MaxValue, SweepTime);
        }
        Assert.Equal(0, tracker.Count);
        return released;
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
    private static void AddRetained(NativeHandleTracker tracker, List<TestObject> retained, int count)
    {
        var added = CreateRetained(tracker, count);
        retained.AddRange(added);
        added.Clear();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Verify(List<TestObject> retained)
    {
        for (var index = 0; index < retained.Count; index++)
        {
            Assert.Equal(index % 1000, Marshal.ReadInt32(retained[index].Handle));
        }

        GC.KeepAlive(retained);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void RegisterAndVerifyCompactRecords(NativeHandleTracker tracker, int count)
    {
        var owner = new TestObject(IntPtr.Zero);
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var index = 0; index < count; index++)
        {
            tracker.Register(owner, Marshal.AllocHGlobal(12));
        }

        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.InRange(allocatedBytes, (long)count * 2 * IntPtr.Size, (long)count * 3 * IntPtr.Size);
        Collect();
        Assert.Equal(0, tracker.Cleanup(int.MaxValue, SweepTime));
        GC.KeepAlive(owner);
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

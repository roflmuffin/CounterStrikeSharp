#include <gtest/gtest.h>
#include <khook.hpp>

namespace {
struct GameInterface {
    int calls = 0;
    virtual int Dispatch(int value) { ++calls; return value * 2; }
};
__attribute__((noinline)) int Dispatch(GameInterface* object, int value) {
    return object->Dispatch(value);
}
struct Listener {
    int preCalls = 0;
    int postCalls = 0;
    int original = 0;
    bool originalMissing = false;
    KHook::Return<int> Pre(GameInterface* object, int value) {
        ++preCalls;
        if (value == 99) return {KHook::Action::Supersede, 123};
        if (value < 0)
            return KHook::Recall(&GameInterface::Dispatch,
                                KHook::Return<int>{KHook::Action::Ignore, 0}, object, -value);
        return {KHook::Action::Ignore, 0};
    }
    KHook::Return<int> Post(GameInterface*, int) {
        ++postCalls;
        auto* result = static_cast<int*>(KHook::GetOriginalValuePtr());
        originalMissing = result == nullptr;
        original = result ? *result : 0;
        return {KHook::Action::Ignore, 0};
    }
};
}

TEST(CssMigration, RecallPreservesOnePrePostPairAndOriginalResult) {
    GameInterface object;
    Listener listener;
    KHook::Virtual<GameInterface, int, int> hook(&GameInterface::Dispatch, &listener, &Listener::Pre, &Listener::Post);
    hook.Add(&object);
    EXPECT_EQ(Dispatch(&object, -4), 8);
    EXPECT_EQ(object.calls, 1);
    EXPECT_EQ(listener.preCalls, 1);
    EXPECT_EQ(listener.postCalls, 1);
    EXPECT_EQ(listener.original, 8);
}

TEST(CssMigration, BlockedCallsStillRunPostWithoutOriginalStorage) {
    GameInterface object;
    Listener listener;
    KHook::Virtual<GameInterface, int, int> hook(&GameInterface::Dispatch, &listener, &Listener::Pre, &Listener::Post);
    hook.Add(&object);
    EXPECT_EQ(Dispatch(&object, 99), 123);
    EXPECT_EQ(object.calls, 0);
    EXPECT_EQ(listener.postCalls, 1);
    EXPECT_TRUE(listener.originalMissing);
}

TEST(CssMigration, InstanceHooksAndRemovalDoNotAffectOtherObjects) {
    GameInterface object, other;
    Listener listener;
    KHook::Virtual<GameInterface, int, int> hook(&GameInterface::Dispatch, &listener, &Listener::Pre, nullptr);
    hook.Add(&object);
    EXPECT_EQ(Dispatch(&object, 99), 123);
    EXPECT_EQ(Dispatch(&other, 99), 198);
    hook.Remove(&object);
    EXPECT_EQ(Dispatch(&object, 99), 198);
    EXPECT_EQ(listener.preCalls, 1);
}

TEST(CssMigration, RawVtableGlobalHookMatchesFutureInstancesAndRemoves) {
    GameInterface object, other;
    Listener listener;
    KHook::Virtual<GameInterface, int, int> hook;
    hook.Configure(KHook::GetVtableIndex(&GameInterface::Dispatch));
    hook.AddContext(&listener, &Listener::Pre, nullptr);
    void* table = *reinterpret_cast<void**>(&object);
    auto* tableCarrier = reinterpret_cast<GameInterface*>(&table);
    hook.AddGlobal(tableCarrier);
    EXPECT_EQ(Dispatch(&object, 99), 123);
    EXPECT_EQ(Dispatch(&other, 99), 123);
    hook.RemoveGlobal(tableCarrier);
    hook.RemoveContext(&listener);
    EXPECT_EQ(Dispatch(&object, 99), 198);
    EXPECT_EQ(Dispatch(&other, 99), 198);
}

TEST(CssMigration, ExplicitOriginalCallDoesNotReenterVirtualCallback) {
    GameInterface object;
    Listener listener;
    KHook::Virtual<GameInterface, int, int> hook(&GameInterface::Dispatch, &listener, &Listener::Pre, nullptr);
    hook.Add(&object);
    EXPECT_EQ(KHook::CallOriginal(&GameInterface::Dispatch, &object, 99), 198);
    EXPECT_EQ(listener.preCalls, 0);
    EXPECT_EQ(object.calls, 1);
}

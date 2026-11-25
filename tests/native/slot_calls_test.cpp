#include <doctest.h>

#include <string>

#include "core/slot_calls.h"

using namespace counterstrikesharp;

namespace {
struct ValueLike
{
    int a;
    int b;
};

class Interface
{
  public:
    virtual int First(int value) = 0;
    virtual int Second(int value) = 0;
    virtual std::string Third(const std::string& value) = 0;
    virtual ValueLike Fourth(ValueLike value) = 0;
};

class Implementation final : public Interface
{
  public:
    int First(int value) override { return value + 1; }
    int Second(int value) override { return value + 2; }
    std::string Third(const std::string& value) override { return value + "3"; }
    ValueLike Fourth(ValueLike value) override { return { value.a + 4, value.b + 4 + offset }; }
    int offset = 100;
};
} // namespace

TEST_CASE("CallSlot calls the method at the given slot index, not the compiled position of the method it is given")
{
    Implementation implementation;
    Interface* instance = &implementation;

    // At its own index, the call is the compiled one.
    CHECK(CallSlot(0, &Interface::First, instance, 10) == 11);
    // One slot on: the method passed only supplies the signature.
    CHECK(CallSlot(1, &Interface::First, instance, 10) == 12);
    CHECK(CallSlot(0, &Interface::Second, instance, 10) == 11);
}

TEST_CASE("CallSlot passes and returns class types by value and calls on the instance it is given")
{
    Implementation implementation;
    implementation.offset = 7;
    Interface* instance = &implementation;

    CHECK(CallSlot(2, &Interface::Third, instance, std::string("x")) == "x3");
    const auto result = CallSlot(3, &Interface::Fourth, instance, ValueLike{ 1, 2 });
    CHECK(result.a == 5);
    CHECK(result.b == 13);
}

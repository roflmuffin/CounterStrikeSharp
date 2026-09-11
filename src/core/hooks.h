#pragma once

#include <khook.hpp>
#include <memory>
#include <vector>
#include <stdexcept>

namespace counterstrikesharp {

// KHook owns the patches; this collection owns our callback contexts. Clear it
// at a safe point before destroying the manager's script callbacks.
template <class C, class R, class... Args> class VirtualHook final : public KHook::Virtual<C, R, Args...>
{
  public:
    using KHook::Virtual<C, R, Args...>::Virtual;
    void Add(C* instance)
    {
        InitializeReturn();
        KHook::Virtual<C, R, Args...>::Add(instance);
        CheckRegistration(*reinterpret_cast<void***>(instance));
    }
    void AddVTable(void** vtable)
    {
        if (!vtable) throw std::runtime_error("Cannot hook a null vtable");
        {
            std::lock_guard guard(this->_m_hooked_this);
            this->_hooked_global.insert(vtable);
        }
        InitializeReturn();
        this->_Setup(vtable);
        CheckRegistration(vtable);
    }

  private:
    void InitializeReturn()
    {
        if constexpr (!std::is_void_v<R>) *this->_fake_return = R{};
    }
    void CheckRegistration(void** vtable)
    {
        if (this->_vtbl_index < 0) throw std::runtime_error("Invalid virtual hook index");
        std::lock_guard guard(this->_hooks_stored);
        if (this->_addr_hook_ids.find(vtable + this->_vtbl_index) == this->_addr_hook_ids.end())
            throw std::runtime_error("KHook could not register virtual hook");
    }
};

template <class R, class... Args> class FunctionHook final : public KHook::Function<R, Args...>
{
  public:
    FunctionHook(R (*function)(Args...), KHook::Return<R> (*pre)(Args...), KHook::Return<R> (*post)(Args...))
        : KHook::Function<R, Args...>(pre, post)
    {
        if constexpr (!std::is_void_v<R>) *this->_fake_return = R{};
        this->Configure(function);
        if (this->_associated_hook_id == KHook::INVALID_HOOK) throw std::runtime_error("KHook could not register function hook");
    }
};

template <class F> struct VirtualHookType;
template <class C, class R, class... Args> struct VirtualHookType<R (C::*)(Args...)>
{
    using Type = VirtualHook<C, R, Args...>;
};
template <class C, class R, class... Args> struct VirtualHookType<R (C::*)(Args...) const>
{
    using Type = VirtualHook<C, R, Args...>;
};

class HookSet
{
  public:
    HookSet() { All().push_back(this); }
    ~HookSet()
    {
        Clear();
        auto& all = All();
        all.erase(std::remove(all.begin(), all.end(), this), all.end());
    }
    HookSet(const HookSet&) = delete;
    HookSet& operator=(const HookSet&) = delete;
    static void ClearAll()
    {
        for (auto* hooks : All())
            hooks->Clear();
    }

    template <class F, class Instance, class Context, class Pre, class Post>
    void Add(F function, Instance* instance, Context* context, Pre pre, Post post)
    {
        if (!instance) throw std::runtime_error("Cannot hook a null instance");
        auto hook = std::make_unique<typename VirtualHookType<F>::Type>(function, context, pre, post);
        hook->Add(instance);
        m_hooks.push_back(std::move(hook));
    }

    template <class F, class Context, class Pre, class Post> void AddGlobal(F function, void** vtable, Context* context, Pre pre, Post post)
    {
        auto hook = std::make_unique<typename VirtualHookType<F>::Type>(function, context, pre, post);
        hook->AddVTable(vtable);
        m_hooks.push_back(std::move(hook));
    }

    template <class C, class R, class... Args, class Context, class Pre, class Post>
    void AddGlobalIndex(int index, void** vtable, Context* context, Pre pre, Post post)
    {
        if (index < 0) throw std::runtime_error("Invalid virtual hook index");
        auto hook = std::make_unique<VirtualHook<C, R, Args...>>(static_cast<uint32_t>(index), context, pre, post);
        hook->AddVTable(vtable);
        m_hooks.push_back(std::move(hook));
    }

    template <class R, class... Args>
    void AddFunction(void* address, KHook::Return<R> (*pre)(Args...), KHook::Return<R> (*post)(Args...) = nullptr)
    {
        if (!address) throw std::runtime_error("Cannot hook a null function");
        m_hooks.push_back(std::make_unique<FunctionHook<R, Args...>>(reinterpret_cast<R (*)(Args...)>(address), pre, post));
    }

    void Clear() { m_hooks.clear(); }

  private:
    static std::vector<HookSet*>& All()
    {
        static std::vector<HookSet*> sets;
        return sets;
    }
    std::vector<std::unique_ptr<KHook::__Hook>> m_hooks;
};
} // namespace counterstrikesharp

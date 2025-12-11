#pragma once

#include <memory>

#include <spdlog/fmt/ostr.h>
#include <spdlog/spdlog.h>

namespace counterstrikesharp {
class Log
{
  public:
    static void Init();
    static void Close();

    static std::shared_ptr<spdlog::logger>& GetCoreLogger() { return m_core_logger; }

  private:
    static std::shared_ptr<spdlog::logger> m_core_logger;
};
} // namespace counterstrikesharp

#define CSSHARP_CORE_TRACE(...)    ::counterstrikesharp::Log::GetCoreLogger()->trace(__VA_ARGS__)
#define CSSHARP_CORE_DEBUG(...)    ::counterstrikesharp::Log::GetCoreLogger()->debug(__VA_ARGS__)
#define CSSHARP_CORE_INFO(...)     ::counterstrikesharp::Log::GetCoreLogger()->info(__VA_ARGS__)
#define CSSHARP_CORE_WARN(...)     ::counterstrikesharp::Log::GetCoreLogger()->warn(__VA_ARGS__)
#define CSSHARP_CORE_ERROR(...)    ::counterstrikesharp::Log::GetCoreLogger()->error(__VA_ARGS__)
#define CSSHARP_CORE_CRITICAL(...) ::counterstrikesharp::Log::GetCoreLogger()->critical(__VA_ARGS__)

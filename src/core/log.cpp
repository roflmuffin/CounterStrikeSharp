#include "core/log.h"

#include <spdlog/sinks/basic_file_sink.h>
#include <spdlog/sinks/stdout_color_sinks.h>
#include <spdlog/cfg/env.h>

namespace counterstrikesharp {
std::shared_ptr<spdlog::logger> Log::m_core_logger;

void Log::Init() {
    std::vector<spdlog::sink_ptr> log_sinks;
    auto color_sink = std::make_shared<spdlog::sinks::stderr_color_sink_mt>();
#if _WIN32
    color_sink->set_color(spdlog::level::trace, 6);
#else
    color_sink->set_color(spdlog::level::trace, color_sink->yellow);
#endif

    log_sinks.emplace_back(color_sink);
    log_sinks.emplace_back(
        std::make_shared<spdlog::sinks::basic_file_sink_mt>("counterstrikesharp.log", true));

    log_sinks[0]->set_pattern("%^[%T.%e] %n: %v%$");
    log_sinks[1]->set_pattern("[%T.%e] [%l] %n: %v");

    m_core_logger = std::make_shared<spdlog::logger>("CSSharp", begin(log_sinks), end(log_sinks));
    spdlog::register_logger(m_core_logger);
    m_core_logger->set_level(spdlog::level::info);
    m_core_logger->flush_on(spdlog::level::info);

    spdlog::cfg::load_env_levels();
}

void Log::Close() {
    spdlog::drop("CSSharp");
    m_core_logger = nullptr;
}
}  // namespace counterstrikesharp

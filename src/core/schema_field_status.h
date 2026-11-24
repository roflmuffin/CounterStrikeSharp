#pragma once

#include <cstdint>

namespace counterstrikesharp::bindings {

// Outcome of looking up a schema field ("Class::field"). Shared by the schema lookup (cs2_sdk/schema.h) and the pure
// resolver, which must not include SDK headers.
enum class SchemaFieldStatus : std::uint8_t
{
    Found,
    ClassMissing,
    FieldMissing,
};

} // namespace counterstrikesharp::bindings

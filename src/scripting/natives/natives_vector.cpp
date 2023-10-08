/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

#include <vector>

#include "scripting/autonative.h"
#include "scripting/script_engine.h"

namespace counterstrikesharp
{

CREATE_GETTER_FUNCTION(Vector, float, Length, Vector *, obj->Length());
CREATE_GETTER_FUNCTION(Vector, float, Length2D, Vector *, obj->Length2D());
CREATE_GETTER_FUNCTION(Vector, float, LengthSqr, Vector *, obj->LengthSqr());
CREATE_GETTER_FUNCTION(Vector, float, Length2DSqr, Vector *, obj->Length2DSqr());
CREATE_GETTER_FUNCTION(Vector, float, IsZero, Vector *, obj->IsZero());

CREATE_GETTER_FUNCTION(Vector, float, X, Vector *, obj->x);
CREATE_GETTER_FUNCTION(Vector, float, Y, Vector *, obj->y);
CREATE_GETTER_FUNCTION(Vector, float, Z, Vector *, obj->z);

CREATE_SETTER_FUNCTION(Vector, float, X, Vector *, obj->x = value);
CREATE_SETTER_FUNCTION(Vector, float, Y, Vector *, obj->y = value);
CREATE_SETTER_FUNCTION(Vector, float, Z, Vector *, obj->z = value);

std::vector<Vector *> managed_vectors;

Vector *VectorNew(ScriptContext &script_context)
{
    auto vec = new Vector();
    managed_vectors.push_back(vec);
    return vec;
}

QAngle *AngleNew(ScriptContext &script_context)
{
    return new QAngle();
}

void NativeVectorAngles(ScriptContext &script_context)
{
    auto vec = script_context.GetArgument<Vector *>(0);
    auto pseudoUpVector = script_context.GetArgument<Vector *>(1);
    auto outAngle = script_context.GetArgument<QAngle *>(2);

    if (!pseudoUpVector)
    {
        VectorAngles(*vec, *outAngle);
    }
    else
    {
        VectorAngles(*vec, *pseudoUpVector, *outAngle);
    }
}

void NativeAngleVectors(ScriptContext &script_context)
{
    auto vec = script_context.GetArgument<QAngle *>(0);
    auto fwd = script_context.GetArgument<Vector *>(1);
    auto right = script_context.GetArgument<Vector *>(2);
    auto up = script_context.GetArgument<Vector *>(3);

    AngleVectors(*vec, fwd, right, up);
}

REGISTER_NATIVES(vector, {
    ScriptEngine::RegisterNativeHandler("VECTOR_NEW", VectorNew);
    ScriptEngine::RegisterNativeHandler("ANGLE_NEW", AngleNew);

    ScriptEngine::RegisterNativeHandler("VECTOR_SET_X", VectorSetX);
    ScriptEngine::RegisterNativeHandler("VECTOR_SET_Y", VectorSetY);
    ScriptEngine::RegisterNativeHandler("VECTOR_SET_Z", VectorSetZ);

    ScriptEngine::RegisterNativeHandler("VECTOR_GET_X", VectorGetX);
    ScriptEngine::RegisterNativeHandler("VECTOR_GET_Y", VectorGetY);
    ScriptEngine::RegisterNativeHandler("VECTOR_GET_Z", VectorGetZ);

    ScriptEngine::RegisterNativeHandler("ANGLE_VECTORS", NativeAngleVectors);
    ScriptEngine::RegisterNativeHandler("VECTOR_ANGLES", NativeVectorAngles);
    ScriptEngine::RegisterNativeHandler("VECTOR_LENGTH", VectorGetLength);
    ScriptEngine::RegisterNativeHandler("VECTOR_LENGTH_2D", VectorGetLength2D);
    ScriptEngine::RegisterNativeHandler("VECTOR_LENGTH_SQR", VectorGetLengthSqr);
    ScriptEngine::RegisterNativeHandler("VECTOR_LENGTH_2D_SQR", VectorGetLength2DSqr);
    ScriptEngine::RegisterNativeHandler("VECTOR_IS_ZERO", VectorGetLengthSqr);
})
} // namespace counterstrikesharp
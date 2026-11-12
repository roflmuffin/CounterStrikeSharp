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
#include "UserMessage.h"

#include "networksystem/inetworkserializer.h"

using namespace google;

namespace counterstrikesharp {
int UserMessage::GetMessageID() { return msgSerializable->GetNetMessageInfo()->m_MessageId; }

std::string UserMessage::GetMessageName() { return NetMessageDisplayName(msgSerializable); }

std::string NetMessageDisplayName(const INetworkMessageInternal* pNetMessage)
{
    if (!pNetMessage) return {};

    // Since 14197 the engine names a message "<proto type> [<id>]" (CNetworkMessages::FindOrCreateNetMessage);
    // strip the id so plugins keep seeing the bare type name they compared against before.
    std::string name = pNetMessage->GetUnscopedName();
    auto open = name.rfind(" [");
    if (open != std::string::npos && name.size() > open + 3 && name.back() == ']'
        && name.find_first_not_of("-0123456789", open + 2) == name.size() - 1)
        name.erase(open);
    return name;
}

bool UserMessage::HasField(std::string fieldName)
{
    const google::protobuf::Descriptor* descriptor = msg->GetDescriptor();
    const google::protobuf::FieldDescriptor* field = descriptor->FindFieldByName(fieldName);

    if (field == nullptr || (field->label() == google::protobuf::FieldDescriptor::LABEL_REPEATED))
    {
        return false;
    }

    return this->msg->GetReflection()->HasField(*this->msg, field);
}

const CNetMessagePB<google::protobuf::Message>* UserMessage::GetProtobufMessage() { return msg; }
} // namespace counterstrikesharp

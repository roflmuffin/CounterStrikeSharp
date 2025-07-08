using CounterStrikeSharp.API.Modules.Memory;

namespace CounterStrikeSharp.API.Core;

public partial class CAttributeList
{
    /// <summary>
    /// Sets or adds a float attribute to the attribute list by name.
    /// <example>
    /// <code>
    /// attributeList.SetAttribute("kill eater", 1337);
    /// </code>
    /// </example>
    /// </summary>
    public void SetAttribute(string name, float value)
    {
        if (Manager?.Outer.Value == null)
            return;

        VirtualFunctions.CAttributeList_SetOrAddAttributeValueByName(Handle, name, value);
    }
}

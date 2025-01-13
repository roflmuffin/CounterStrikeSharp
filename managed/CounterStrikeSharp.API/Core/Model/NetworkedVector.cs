using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class NetworkedVector<T> : NativeObject, IReadOnlyCollection<T>
{
    public NetworkedVector(IntPtr pointer) : base(pointer)
    {
    }

    public unsafe uint Size => Unsafe.Read<uint>((void*)Handle);

    public unsafe int Count => NativeAPI.GetNetworkVectorSize(Handle);

    public T this[int index]
    {
        get
        {
            if (!typeof(T).IsGenericType || typeof(T).GetGenericTypeDefinition() != typeof(CHandle<>))
            {
                throw new NotSupportedException("Networked vectors currently only support CHandle<T>");
            }

            return (T)Activator.CreateInstance(typeof(T), NativeAPI.GetNetworkVectorElementAt(Handle, index));
        }
    }

    public void RemoveAll()
    {
        NativeAPI.RemoveAllNetworkVectorElements(Handle);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

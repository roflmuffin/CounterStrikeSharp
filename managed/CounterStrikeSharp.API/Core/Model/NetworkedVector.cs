using FastGenericNew;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class NetworkedVector<T> : NativeObject, IReadOnlyCollection<T>
{
    private enum ElementKind
    {
        Primitive,
        NativeObject,
        Handle,
        String
    }

    private readonly record struct ElementMetadata(ElementKind Kind, int Size);

    private static readonly Type ElementType = typeof(T);
    private static readonly Lazy<ElementMetadata> Metadata = new(CreateElementMetadata);

    public NetworkedVector(IntPtr pointer) : base(pointer)
    {
    }

    public unsafe uint Size => Unsafe.Read<uint>((void*)Handle);

    public unsafe int Count => NativeAPI.GetNetworkVectorSize(Handle);

    public T this[int index]
    {
        get
        {
            var count = Count;
            if ((uint)index >= (uint)count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index,
                    $"Index must be between 0 and {count - 1}.");
            }

            var metadata = Metadata.Value;
            var firstElement = NativeAPI.GetNetworkVectorElementAt(Handle, 0);
            var elementAddress = firstElement + checked(index * metadata.Size);

            if (metadata.Kind is ElementKind.NativeObject or ElementKind.Handle)
            {
                return FastNew.CreateInstance<T, IntPtr>(elementAddress);
            }

            if (metadata.Kind == ElementKind.String)
            {
                return (T)(object)Utilities.ReadStringUtf8(elementAddress);
            }

            unsafe
            {
                return Unsafe.Read<T>((void*)elementAddress);
            }
        }
    }

    private static ElementMetadata CreateElementMetadata()
    {
        if (ElementType.IsGenericType && ElementType.GetGenericTypeDefinition() == typeof(CHandle<>))
        {
            return new(ElementKind.Handle, sizeof(uint));
        }

        if (ElementType == typeof(string))
        {
            return new(ElementKind.String, IntPtr.Size);
        }

        if (ElementType.IsPrimitive || ElementType.IsEnum)
        {
            return new(ElementKind.Primitive, Unsafe.SizeOf<T>());
        }

        if (typeof(NativeObject).IsAssignableFrom(ElementType))
        {
            var nativeSize = GetKnownNativeObjectSize();
            if (nativeSize > 0)
            {
                return new(ElementKind.NativeObject, nativeSize);
            }

            var schemaClassSize = Schema.GetClassSize(ElementType.Name);
            if (schemaClassSize > 0)
            {
                return new(ElementKind.NativeObject, schemaClassSize);
            }
        }

        throw new NotSupportedException(
            $"Networked vectors do not support elements of type {ElementType.FullName}.");
    }

    // Forever paying for the decision to use NativeObjects as the base class even for simple structs like these :(
    private static int GetKnownNativeObjectSize()
    {
        if (ElementType == typeof(Vector2D))
        {
            return sizeof(float) * 2;
        }

        if (ElementType == typeof(Vector) || ElementType == typeof(QAngle) || ElementType == typeof(Angle))
        {
            return sizeof(float) * 3;
        }

        if (ElementType == typeof(Vector4D) || ElementType == typeof(Quaternion))
        {
            return sizeof(float) * 4;
        }

        if (ElementType == typeof(CTransform))
        {
            return sizeof(float) * 8;
        }

        return -1;
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

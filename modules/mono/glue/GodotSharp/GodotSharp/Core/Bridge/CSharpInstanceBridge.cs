using System;
using System.Runtime.InteropServices;
using Godot.NativeInterop;

namespace Godot.Bridge
{
    internal static class CSharpInstanceBridge
    {
        [UnmanagedCallersOnly]
        internal static unsafe godot_bool Call(IntPtr godotObjectGCHandle, godot_string_name* method,
            godot_variant** args, int argCount, godot_variant_call_error* refCallError, godot_variant* ret)
        {
            try
            {
                // Performance is not critical here as this will be replaced with source generators.
                var godotObject = (Object)GCHandle.FromIntPtr(godotObjectGCHandle).Target;

                if (godotObject == null)
                {
                    *ret = default;
                    (*refCallError).Error = godot_variant_call_error_error.GODOT_CALL_ERROR_CALL_ERROR_INSTANCE_IS_NULL;
                    return false.ToGodotBool();
                }

                NativeFuncs.godotsharp_string_name_as_string(out godot_string dest, CustomUnsafe.AsRef(method));
                string methodStr;
                using (dest)
                    methodStr = Marshaling.ConvertStringToManaged(dest);

                _ = godotObject.InternalGodotScriptCall(methodStr, new NativeVariantPtrArgs(args), argCount,
                    out godot_variant retValue);
                *ret = retValue;
                return true.ToGodotBool();
            }
            catch (Exception e)
            {
                ExceptionUtils.DebugPrintUnhandledException(e);
                *ret = default;
                return false.ToGodotBool();
            }
        }

        [UnmanagedCallersOnly]
        internal static unsafe godot_bool Set(IntPtr godotObjectGCHandle, godot_string_name* name, godot_variant* value)
        {
            try
            {
                // Performance is not critical here as this will be replaced with source generators.
                var godotObject = (Object)GCHandle.FromIntPtr(godotObjectGCHandle).Target;

                if (godotObject == null)
                    throw new InvalidOperationException();

                var nameManaged = StringName.CreateTakingOwnershipOfDisposableValue(
                    NativeFuncs.godotsharp_string_name_new_copy(CustomUnsafe.AsRef(name)));

                if (godotObject.InternalGodotScriptSetFieldOrPropViaReflection(
                        nameManaged.ToString(), CustomUnsafe.AsRef(value)))
                {
                    return true.ToGodotBool();
                }

                object valueManaged = Marshaling.ConvertVariantToManagedObject(CustomUnsafe.AsRef(value));

                return godotObject._Set(nameManaged, valueManaged).ToGodotBool();
            }
            catch (Exception e)
            {
                ExceptionUtils.DebugPrintUnhandledException(e);
                return false.ToGodotBool();
            }
        }

        [UnmanagedCallersOnly]
        internal static unsafe godot_bool Get(IntPtr godotObjectGCHandle, godot_string_name* name,
            godot_variant* outRet)
        {
            try
            {
                // Performance is not critical here as this will be replaced with source generators.
                var godotObject = (Object)GCHandle.FromIntPtr(godotObjectGCHandle).Target;

                if (godotObject == null)
                    throw new InvalidOperationException();

                var nameManaged = StringName.CreateTakingOwnershipOfDisposableValue(
                    NativeFuncs.godotsharp_string_name_new_copy(CustomUnsafe.AsRef(name)));

                if (godotObject.InternalGodotScriptGetFieldOrPropViaReflection(nameManaged.ToString(),
                        out godot_variant outRetValue))
                {
                    *outRet = outRetValue;
                    return true.ToGodotBool();
                }

                object ret = godotObject._Get(nameManaged);

                if (ret == null)
                {
                    *outRet = default;
                    return false.ToGodotBool();
                }

                *outRet = Marshaling.ConvertManagedObjectToVariant(ret);
                return true.ToGodotBool();
            }
            catch (Exception e)
            {
                ExceptionUtils.DebugPrintUnhandledException(e);
                *outRet = default;
                return false.ToGodotBool();
            }
        }

        [UnmanagedCallersOnly]
        internal static void CallDispose(IntPtr godotObjectGCHandle, godot_bool okIfNull)
        {
            try
            {
                var godotObject = (Object)GCHandle.FromIntPtr(godotObjectGCHandle).Target;

                if (okIfNull.ToBool())
                    godotObject?.Dispose();
                else
                    godotObject!.Dispose();
            }
            catch (Exception e)
            {
                ExceptionUtils.DebugPrintUnhandledException(e);
            }
        }

        [UnmanagedCallersOnly]
        internal static unsafe void CallToString(IntPtr godotObjectGCHandle, godot_string* outRes, godot_bool* outValid)
        {
            try
            {
                var self = (Object)GCHandle.FromIntPtr(godotObjectGCHandle).Target;

                if (self == null)
                {
                    *outRes = default;
                    *outValid = false.ToGodotBool();
                    return;
                }

                var resultStr = self.ToString();

                if (resultStr == null)
                {
                    *outRes = default;
                    *outValid = false.ToGodotBool();
                    return;
                }

                *outRes = Marshaling.ConvertStringToNative(resultStr);
                *outValid = true.ToGodotBool();
            }
            catch (Exception e)
            {
                ExceptionUtils.DebugPrintUnhandledException(e);
                *outRes = default;
                *outValid = false.ToGodotBool();
            }
        }
    }
}

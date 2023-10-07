
using System;

namespace CounterStrikeSharp.API.Core
{
    public class NativeAPI {
        
        public static void SetScriptContext(IntPtr context){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(context);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xD87B5E97);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static bool AddListener(string name, InputArgument callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x8E7D0305);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static bool RemoveListener(string name, InputArgument callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x47C507A2);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static IntPtr AddCommand(string name, string description, bool serverOnly, int flags, InputArgument callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.PushString(description);
			ScriptContext.GlobalScriptContext.Push(serverOnly);
			ScriptContext.GlobalScriptContext.Push(flags);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x807C6B9C);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static void RemoveCommand(string name, InputArgument callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xEC2412DB);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static int CommandGetArgCount(IntPtr command){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(command);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xAD28109C);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static string CommandGetArgString(IntPtr command){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(command);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2E52E8EA);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static string CommandGetCommandString(IntPtr command){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(command);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x8FABC059);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static string CommandGetArgByIndex(IntPtr command, int index){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(command);
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x3E8D9805);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static void IssueClientCommand(int entityIndex, string command){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.PushString(command);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xCA5BA982);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr FindConvar(string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x52254718);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr CreateConvar(string name, string value, string description, int flags, bool hasMinValue, float minValue, bool hasMaxValue, float maxValue){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.PushString(value);
			ScriptContext.GlobalScriptContext.PushString(description);
			ScriptContext.GlobalScriptContext.Push(flags);
			ScriptContext.GlobalScriptContext.Push(hasMinValue);
			ScriptContext.GlobalScriptContext.Push(minValue);
			ScriptContext.GlobalScriptContext.Push(hasMaxValue);
			ScriptContext.GlobalScriptContext.Push(maxValue);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xF22079B9);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static void HookConvarChange(IntPtr convar, InputArgument callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(convar);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xC7707707);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void UnhookConvarChange(IntPtr convar, InputArgument callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(convar);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xC4A3287C);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static int ConvarGetFlags(IntPtr convar){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(convar);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x63AA9DCB);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static void ConvarSetFlags(IntPtr convar, int flags){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(convar);
			ScriptContext.GlobalScriptContext.Push(flags);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x361E31DF);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static string ConvarGetStringValue(IntPtr convar){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(convar);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xE904F9F5);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static void ConvarSetStringValue(IntPtr convar, string value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(convar);
			ScriptContext.GlobalScriptContext.PushString(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xE1D0B161);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static string ConvarGetName(IntPtr convar){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(convar);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xF3856D93);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static void ConvarUnregister(IntPtr convar){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(convar);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x74D5874F);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void ConvarReset(IntPtr convar){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(convar);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xB140E6A8);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr EdictFromIndex(int index){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x996BECB2);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr EdictFromBasehandle(IntPtr baseHandle){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(baseHandle);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x5AE6C0F3);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr EdictFromInthandle(int intHandle){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(intHandle);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xE347E415);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr EdictFromBaseentity(IntPtr baseEntity){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(baseEntity);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x73963942);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr EdictFromUserid(int userId){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(userId);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xF45BC8B0);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr EdictFromPlayerinfo(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xFB2FCB71);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static int InthandleFromIndex(int index){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x6536D414);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int InthandleFromEdict(IntPtr edict){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(edict);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x66060615);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int InthandleFromBasehandle(IntPtr baseHandle){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(baseHandle);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x3AE3215);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int InthandleFromBaseentity(IntPtr baseEntity){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(baseEntity);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x13AE9E4);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int InthandleFromUserid(int userId){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(userId);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x4B53BBD6);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int InthandleFromPlayerinfo(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x6EB78F17);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static IntPtr BaseentityFromIndex(int index){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xAB458EC3);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr BaseentityFromEdict(IntPtr edict){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(edict);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xAB8EDAC2);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr BaseentityFromBasehandle(IntPtr baseHandle){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(baseHandle);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x1F04A2);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr BaseentityFromInthandle(int intHandle){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(intHandle);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xFAB2ED64);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr BaseentityFromUserid(int userId){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(userId);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xF92C34E1);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr BaseentityFromPlayerinfo(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xF77E87A0);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static int UseridFromIndex(int index){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x4BEAF5B1);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int UseridFromEdict(IntPtr edict){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(edict);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x4C366230);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int UseridFromBasehandle(IntPtr baseHandle){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(baseHandle);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xA35718D0);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int UseridFromInthandle(int intHandle){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(intHandle);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x52811556);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int UseridFromPlayerinfo(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x13C48E52);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int UseridFromBaseentity(IntPtr baseEntity){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(baseEntity);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xC090E9E1);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static IntPtr PlayerinfoFromIndex(int index){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x79AA3450);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr PlayerinfoFromEdict(IntPtr edict){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(edict);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x79F5A951);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr PlayerinfoFromBasehandle(IntPtr baseHandle){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(baseHandle);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xC7CD7C51);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr PlayerinfoFromInthandle(int intHandle){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(intHandle);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xB92C7AB7);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr PlayerinfoFromUserid(int userId){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(userId);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xDC1EE412);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr PlayerinfoFromBaseentity(IntPtr baseEntity){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(baseEntity);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xE98B7620);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static int IndexFromEdict(IntPtr edict){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(edict);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xCD941E52);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int IndexFromBasehandle(IntPtr baseHandle){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(baseHandle);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x4628A32);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int IndexFromInthandle(int intHandle){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(intHandle);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xFAD400F4);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int IndexFromBaseentity(IntPtr baseEntity){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(baseEntity);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x188E1BC3);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int IndexFromUserid(int userId){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(userId);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xA43C7271);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int IndexFromPlayerinfo(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x57D50D30);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int IndexFromName(string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xB0E293AA);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int IndexFromSteamid(string steamId){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(steamId);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x13BE07AE);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int IndexFromUniqueid(int uniqueId){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(uniqueId);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xD56E6793);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static IntPtr BasehandleFromIndex(int index){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x38CBB912);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr BasehandleFromEdict(IntPtr edict){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(edict);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x3910BC13);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr BasehandleFromInthandle(int intHandle){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(intHandle);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x83267FF5);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr BasehandleFromBaseentity(IntPtr baseEntity){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(baseEntity);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x3EB31222);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr BasehandleFromUserid(int userId){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(userId);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x7F711110);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr BasehandleFromPlayerinfo(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x951213D1);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static string GetMapName(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0x43C2ED68);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static bool IsMapValid(string mapname){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(mapname);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xD88A5CD5);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static string GetGameDirectory(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0xD8F03FD4);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static float GetTickInterval(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0x970CB1B9);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (float)ScriptContext.GlobalScriptContext.GetResult(typeof(float));
			}
		}

        public static int GetTickCount(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0xAB744EC5);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static float GetCurrentTime(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0xFDF24F);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (float)ScriptContext.GlobalScriptContext.GetResult(typeof(float));
			}
		}

        public static float GetGameframeTime(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0x143FED5);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (float)ScriptContext.GlobalScriptContext.GetResult(typeof(float));
			}
		}

        public static double GetEngineTime(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0x39A17C88);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (double)ScriptContext.GlobalScriptContext.GetResult(typeof(double));
			}
		}

        public static void IssueServerCommand(string command){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(command);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xA5901A5E);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void PrecacheModel(string command){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(command);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x77A0C6BE);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static bool PrecacheSound(string command, bool preload){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(command);
			ScriptContext.GlobalScriptContext.Push(preload);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x758F3FD2);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static bool IsSoundPrecached(string command){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(command);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xD4372AF3);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static float GetSoundDuration(string command){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(command);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x20BB05CE);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (float)ScriptContext.GlobalScriptContext.GetResult(typeof(float));
			}
		}

        public static void EmitSound(int client, int entity, int channel, string sound, float volume, float attenuation, int flags, int pitch, IntPtr origin, IntPtr direction){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(client);
			ScriptContext.GlobalScriptContext.Push(entity);
			ScriptContext.GlobalScriptContext.Push(channel);
			ScriptContext.GlobalScriptContext.PushString(sound);
			ScriptContext.GlobalScriptContext.Push(volume);
			ScriptContext.GlobalScriptContext.Push(attenuation);
			ScriptContext.GlobalScriptContext.Push(flags);
			ScriptContext.GlobalScriptContext.Push(pitch);
			ScriptContext.GlobalScriptContext.Push(origin);
			ScriptContext.GlobalScriptContext.Push(direction);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xCFB9CACC);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr CreateRay1(int rayType, IntPtr vec1, IntPtr vec2){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(rayType);
			ScriptContext.GlobalScriptContext.Push(vec1);
			ScriptContext.GlobalScriptContext.Push(vec2);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x7A3E109A);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr CreateRay2(IntPtr vec1, IntPtr vec2, IntPtr vec3, IntPtr vec4){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vec1);
			ScriptContext.GlobalScriptContext.Push(vec2);
			ScriptContext.GlobalScriptContext.Push(vec3);
			ScriptContext.GlobalScriptContext.Push(vec4);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x7A3E1099);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static void TraceRay(IntPtr ray, IntPtr outputTrace, IntPtr traceFilter, uint flags){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(ray);
			ScriptContext.GlobalScriptContext.Push(outputTrace);
			ScriptContext.GlobalScriptContext.Push(traceFilter);
			ScriptContext.GlobalScriptContext.Push(flags);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x35182751);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr NewSimpleTraceFilter(int indexToIgnore){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(indexToIgnore);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xC3572E09);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr NewTraceFilterProxy(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0x881F122B);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr NewTraceResult(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0x95B04711);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static void TraceFilterProxySetTraceTypeCallback(IntPtr traceFilter, IntPtr callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(traceFilter);
			ScriptContext.GlobalScriptContext.Push(callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xE907BCBA);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void TraceFilterProxySetShouldHitEntityCallback(IntPtr traceFilter, IntPtr callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(traceFilter);
			ScriptContext.GlobalScriptContext.Push(callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x3858171B);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static bool TraceDidHit(IntPtr traceResult){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(traceResult);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x7D2D8E58);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static IntPtr TraceResultEntity(IntPtr traceResult){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(traceResult);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x1F776F56);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static void QueueTaskForNextFrame(IntPtr function){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(function);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x9FE394D8);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static int EntityGetProp(int entityIndex, int offset, int size){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(offset);
			ScriptContext.GlobalScriptContext.Push(size);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x1C982875);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int EntityGetPropInt(int entityIndex, int type, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(type);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x84CF6759);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static float EntityGetPropFloat(int entityIndex, int type, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(type);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xF697527A);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (float)ScriptContext.GlobalScriptContext.GetResult(typeof(float));
			}
		}

        public static IntPtr EntityGetPropVector(int entityIndex, int type, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(type);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xEF6F2953);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static string EntityGetPropString(int entityIndex, int type, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(type);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xE78DD27F);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static int EntityGetPropEnt(int entityIndex, int type, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(type);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x84CF9755);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int EntityGetPropEntByOffset(int entityIndex, int offset){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(offset);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xFC759643);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static void EntitySetProp(int entityIndex, int offset, int size, int value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(offset);
			ScriptContext.GlobalScriptContext.Push(size);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x3CF47EE1);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void EntitySetPropInt(int entityIndex, int type, string name, int value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(type);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x8F1F01CD);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void EntitySetPropFloat(int entityIndex, int type, string name, float value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(type);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xD2AF9FEE);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void EntitySetPropVector(int entityIndex, int type, string name, IntPtr value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(type);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x3F0DE47);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void EntitySetPropString(int entityIndex, int type, string name, string value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(type);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.PushString(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xFC8626B);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void EntitySetPropEnt(int entityIndex, int type, string name, int value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(type);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x8F1F11C1);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static string EntityGetKeyvalue(int entityIndex, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x21FBF254);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static void EntitySetKeyvalue(int entityIndex, string name, string value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.PushString(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x7A71ACC0);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static int EntityCreateByClassname(string className){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(className);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xACBE0A77);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int EntityFindByClassname(int index, string className){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.PushString(className);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x698CFB56);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int EntityFindByNetclass(int index, string netClass){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.PushString(netClass);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x65E5B14E);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static string EntityGetClassname(int entityIndex){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x96A104C1);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static void EntitySpawn(int entityIndex){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xF1D8B43A);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static bool EntityIsPlayer(int entityIndex){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xC37C5877);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static bool EntityIsWeapon(int entityIndex){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xC0CCFC06);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static bool EntityIsNetworked(int entityIndex){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x58F7BC9B);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static bool EntityIsValid(int entityIndex){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x91ADC1B2);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static int GetMaxEntities(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0x900C325A);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int FindDatamapInfo(int entityIndex, string propertyName){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.PushString(propertyName);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xB124CB42);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static void AcceptInput(int entityIndex, string inputName){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.PushString(inputName);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x259E084C);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void HookEvent(string name, InputArgument callback, bool isPost){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.Push(isPost);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xE71F04D5);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void UnhookEvent(string name, InputArgument callback, bool isPost){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.Push(isPost);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2154AFAE);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr CreateEvent(string name, bool force){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.Push(force);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x7B472432);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static void FireEvent(IntPtr gameEvent, bool dontBroadcast){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameEvent);
			ScriptContext.GlobalScriptContext.Push(dontBroadcast);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2D52AEE);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void FireEventToClient(IntPtr gameEvent, int clientIndex){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameEvent);
			ScriptContext.GlobalScriptContext.Push(clientIndex);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x40B7C06C);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static string GetEventName(IntPtr gameEvent){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameEvent);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xDFF86998);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static bool GetEventBool(IntPtr gameEvent, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameEvent);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xDFFEE451);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static int GetEventInt(IntPtr gameEvent, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameEvent);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xB17427CC);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static float GetEventFloat(IntPtr gameEvent, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameEvent);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xDF96CB6F);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (float)ScriptContext.GlobalScriptContext.GetResult(typeof(float));
			}
		}

        public static string GetEventString(IntPtr gameEvent, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameEvent);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xB4EBC50A);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static void SetEventBool(IntPtr gameEvent, string name, bool value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameEvent);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x31859DC5);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void SetEventFloat(IntPtr gameEvent, string name, float value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameEvent);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x627CF47B);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void SetEventString(IntPtr gameEvent, string name, string value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameEvent);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.PushString(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xCB7E7B9E);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void SetEventInt(IntPtr gameEvent, string name, int value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameEvent);
			ScriptContext.GlobalScriptContext.PushString(name);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x4F1363D8);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static int LoadEventsFromFile(string path){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(path);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xED480293);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static IntPtr CreateVirtualFunction(IntPtr ptr, int vtableOffset, int numArguments, int returnType, object[] arguments){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(ptr);
			ScriptContext.GlobalScriptContext.Push(vtableOffset);
			ScriptContext.GlobalScriptContext.Push(numArguments);
			ScriptContext.GlobalScriptContext.Push(returnType);
			foreach (var obj in arguments)
			{
				ScriptContext.GlobalScriptContext.Push(obj);
			}
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2531DA2);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr CreateVirtualFunctionBySignature(IntPtr ptr, string binaryName, string signature, int numArguments, int returnType, object[] arguments){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(ptr);
			ScriptContext.GlobalScriptContext.PushString(binaryName);
			ScriptContext.GlobalScriptContext.PushString(signature);
			ScriptContext.GlobalScriptContext.Push(numArguments);
			ScriptContext.GlobalScriptContext.Push(returnType);
			foreach (var obj in arguments)
			{
				ScriptContext.GlobalScriptContext.Push(obj);
			}
			ScriptContext.GlobalScriptContext.SetIdentifier(0x8D25187D);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static void ExecuteVirtualFunction(IntPtr functionPtr, object[] arguments){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(functionPtr);
			foreach (var obj in arguments)
			{
				ScriptContext.GlobalScriptContext.Push(obj);
			}
			ScriptContext.GlobalScriptContext.SetIdentifier(0x376A0359);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void HookFunction(IntPtr functionPtr, int entityIndex, bool post, InputArgument callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(functionPtr);
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(post);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xA6C8BA9B);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void UnhookFunction(IntPtr functionPtr, int entityIndex, bool post, InputArgument callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(functionPtr);
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(post);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2051B00);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void ShowMenu(int entityIndex, IntPtr menu){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityIndex);
			ScriptContext.GlobalScriptContext.Push(menu);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x1734D86A);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr CreateMenu(string title){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(title);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x971D05CD);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static void DeleteMenu(IntPtr menu){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(menu);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x6E48F470);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr AddMenuOption(IntPtr menu, string display, string value, int flags){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(menu);
			ScriptContext.GlobalScriptContext.PushString(display);
			ScriptContext.GlobalScriptContext.PushString(value);
			ScriptContext.GlobalScriptContext.Push(flags);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xB4B1154);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static void AddMenuHandler(IntPtr menu, IntPtr callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(menu);
			ScriptContext.GlobalScriptContext.Push(callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xB8E70A6F);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr PlayerGetPlayerinfo(IntPtr entity){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entity);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xFE001EDD);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static string PlayerinfoGetName(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x67B55BE9);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static int PlayerinfoGetUserid(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x5F445F72);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static string PlayerinfoGetSteamid(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x9901B0D);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static int PlayerinfoGetTeam(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x67C18C33);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static void PlayerinfoSetTeam(IntPtr playerInfo, int team){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.Push(team);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x3EBD79A7);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static int PlayerinfoGetKills(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x5EBE3DBF);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int PlayerinfoGetDeaths(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x39033321);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static bool PlayerinfoIsConnected(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x3D6BC1D);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static int PlayerinfoGetArmor(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x5F30D56D);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static bool PlayerinfoIsHltv(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x246A38E4);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static bool PlayerinfoIsPlayer(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xFB9E6EB1);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static bool PlayerinfoIsFakeclient(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xDBCFC452);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static bool PlayerinfoIsDead(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2467F906);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static bool PlayerinfoIsInVehicle(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x22016B62);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static bool PlayerinfoIsObserver(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xDB6B59CA);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static string PlayerinfoGetWeaponName(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x723C4BD4);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static string PlayerinfoGetModelName(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x99C14DD9);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static int PlayerinfoGetHealth(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2F1DCF32);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int PlayerinfoGetMaxHealth(IntPtr playerInfo){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(playerInfo);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x1DC58059);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static bool ClientIsInGame(int index){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xA7094790);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static bool IsFakeClient(int index){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xCA1BD7CF);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static void AddToStringTable(int index, string value, string userdata){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.PushString(value);
			ScriptContext.GlobalScriptContext.PushString(userdata);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x589A7F4B);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static int FindStringIndex(int index, string searchValue){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.PushString(searchValue);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x275275EB);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int FindStringTable(string searchValue){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(searchValue);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x256C97AB);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int GetNumStringTables(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0x64FCC202);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static string GetStringTableData(int index, int stringIndex){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.Push(stringIndex);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x48668517);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static int GetStringTableDataLength(int index, int stringIndex){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.Push(stringIndex);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x4D0CD334);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static int GetStringTableMaxStrings(int index){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x8FAC99AA);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static string GetStringTableName(int index){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x486BED60);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static int GetStringTableNumStrings(int index){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x76FD48A8);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static bool LockStringTables(bool @lock){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(@lock);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x4001FEB6);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static string ReadStringTable(int index, int stringIndex){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.Push(stringIndex);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x43925BBC);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static void SetStringTableData(int index, int stringIndex, string value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.Push(stringIndex);
			ScriptContext.GlobalScriptContext.PushString(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xF6D23F03);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static double GetTickedTime(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0x84108452);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (double)ScriptContext.GlobalScriptContext.GetResult(typeof(double));
			}
		}

        public static IntPtr CreateTimer(float interval, InputArgument callback, int flags){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(interval);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.Push(flags);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x7A5BAE39);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static void KillTimer(IntPtr timer){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(timer);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x32313EDF);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void PrintToChat(int index, string message){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.PushString(message);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x5EC4ADF1);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void PrintToConsole(string message){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.PushString(message);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x7F033898);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void PrintToHint(int index, string message){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.PushString(message);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x5EC00D74);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void PrintToCenter(int index, string message){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.PushString(message);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2300CC64);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr VectorNew(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0xA67981DF);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr AngleNew(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0x11907167);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static float VectorGetX(IntPtr vector){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vector);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2A85CBB2);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (float)ScriptContext.GlobalScriptContext.GetResult(typeof(float));
			}
		}

        public static float VectorGetY(IntPtr vector){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vector);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2A85CBB3);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (float)ScriptContext.GlobalScriptContext.GetResult(typeof(float));
			}
		}

        public static float VectorGetZ(IntPtr vector){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vector);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2A85CBB0);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (float)ScriptContext.GlobalScriptContext.GetResult(typeof(float));
			}
		}

        public static void VectorSetX(IntPtr vector, float value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vector);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2B62AFA6);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void VectorSetY(IntPtr vector, float value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vector);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2B62AFA7);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void VectorSetZ(IntPtr vector, float value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vector);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2B62AFA4);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void VectorAngles(IntPtr vector, IntPtr pseudoUp, IntPtr outAngle){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vector);
			ScriptContext.GlobalScriptContext.Push(pseudoUp);
			ScriptContext.GlobalScriptContext.Push(outAngle);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x6E6886B1);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void AngleVectors(IntPtr vector, IntPtr forwardOut, IntPtr rightOut, IntPtr upOut){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vector);
			ScriptContext.GlobalScriptContext.Push(forwardOut);
			ScriptContext.GlobalScriptContext.Push(rightOut);
			ScriptContext.GlobalScriptContext.Push(upOut);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xF696A2F1);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static float VectorLength(IntPtr vector){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vector);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x94B5BA5F);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (float)ScriptContext.GlobalScriptContext.GetResult(typeof(float));
			}
		}

        public static float VectorLength2d(IntPtr vector){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vector);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xBAC81CD6);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (float)ScriptContext.GlobalScriptContext.GetResult(typeof(float));
			}
		}

        public static float VectorLengthSqr(IntPtr vector){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vector);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x13CB3150);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (float)ScriptContext.GlobalScriptContext.GetResult(typeof(float));
			}
		}

        public static float VectorLength2dSqr(IntPtr vector){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vector);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xEAF6FE79);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (float)ScriptContext.GlobalScriptContext.GetResult(typeof(float));
			}
		}

        public static bool VectorIsZero(IntPtr vector){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vector);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xA4B37BC4);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}
    }
}

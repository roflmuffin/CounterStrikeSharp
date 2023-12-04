
using System;
using CounterStrikeSharp.API.Modules.Memory;

namespace CounterStrikeSharp.API.Core
{
    public class NativeAPI {
        
        public static bool AddListener(string name, InputArgument callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(name);
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
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x47C507A2);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static void AddCommand(string name, string description, bool serveronly, int flags, InputArgument callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.Push(description);
			ScriptContext.GlobalScriptContext.Push(serveronly);
			ScriptContext.GlobalScriptContext.Push(flags);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x807C6B9C);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void RemoveCommand(string name, InputArgument callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xEC2412DB);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void AddCommandListener(string cmd, InputArgument callback, bool post){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(cmd);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.Push(post);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2D2D803D);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void RemoveCommandListener(string cmd, InputArgument callback, bool post){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(cmd);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.Push(post);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x34DBBF1A);
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

        public static void IssueClientCommand(int clientindex, string command){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(clientindex);
			ScriptContext.GlobalScriptContext.Push(command);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xCA5BA982);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr FindConvar(string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x52254718);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static void SetConvarStringValue(IntPtr convar, string value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(convar);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x9A736FC1);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static string GetClientConvarValue(int clientindex, string convarname){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(clientindex);
			ScriptContext.GlobalScriptContext.Push(convarname);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xAE4B1B79);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static void SetFakeClientConvarValue(int clientindex, string convarname, string convarvalue){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(clientindex);
			ScriptContext.GlobalScriptContext.Push(convarname);
			ScriptContext.GlobalScriptContext.Push(convarvalue);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x4C61E8BB);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static T DynamicHookGetReturn<T>(IntPtr hook, int datatype){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(hook);
			ScriptContext.GlobalScriptContext.Push(datatype);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x4F5B80D0);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (T)ScriptContext.GlobalScriptContext.GetResult(typeof(T));
			}
		}

        public static void DynamicHookSetReturn<T>(IntPtr hook, int datatype, T value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(hook);
			ScriptContext.GlobalScriptContext.Push(datatype);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xDB297E44);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static T DynamicHookGetParam<T>(IntPtr hook, int datatype, int paramindex){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(hook);
			ScriptContext.GlobalScriptContext.Push(datatype);
			ScriptContext.GlobalScriptContext.Push(paramindex);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x5F5ABDD5);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (T)ScriptContext.GlobalScriptContext.GetResult(typeof(T));
			}
		}

        public static void DynamicHookSetParam<T>(IntPtr hook, int datatype, int paramindex, T value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(hook);
			ScriptContext.GlobalScriptContext.Push(datatype);
			ScriptContext.GlobalScriptContext.Push(paramindex);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xA96CFBC1);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
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

        public static string GetGameDirectory(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0xD8F03FD4);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static bool IsMapValid(string mapname){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(mapname);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xD88A5CD5);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
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

        public static float GetCurrentTime(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0xFDF24F);
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

        public static float GetGameFrameTime(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0x97E331CA);
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
			ScriptContext.GlobalScriptContext.Push(command);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xA5901A5E);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void PrecacheModel(string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x77A0C6BE);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static bool PrecacheSound(string name, bool preload){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.Push(preload);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x758F3FD2);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static bool IsSoundPrecached(string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xD4372AF3);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static float GetSoundDuration(string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x20BB05CE);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (float)ScriptContext.GlobalScriptContext.GetResult(typeof(float));
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

        public static void TraceRay(IntPtr ray, IntPtr ptrace, IntPtr traceFilter, uint flags){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(ray);
			ScriptContext.GlobalScriptContext.Push(ptrace);
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

        public static IntPtr NewTraceResult(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0x95B04711);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
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

        public static void QueueTaskForNextFrame(IntPtr callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x9FE394D8);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void QueueTaskForNextWorldUpdate(IntPtr callback){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(callback);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xAD51A0C9);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr GetValveInterface(int interfacetype, string interfacename){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(interfacetype);
			ScriptContext.GlobalScriptContext.Push(interfacename);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xDFAED2BE);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static T GetCommandParamValue<T>(string param, DataType datatype, T defaultvalue){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(param);
			ScriptContext.GlobalScriptContext.Push(datatype);
			ScriptContext.GlobalScriptContext.Push(defaultvalue);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x748F302F);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (T)ScriptContext.GlobalScriptContext.GetResult(typeof(T));
			}
		}

        public static void PrintToServerConsole(string msg){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(msg);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x5D4EE1C2);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr GetEntityFromIndex(int index){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xD551EB1F);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static int GetUseridFromIndex(int index){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x83542138);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static string GetDesignerName(IntPtr pointer){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(pointer);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x28DCCD51);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static IntPtr GetEntityPointerFromHandle(IntPtr entityhandlepointer){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityhandlepointer);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xEE3A8DEF);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static uint GetRefFromEntityPointer(IntPtr entitypointer){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entitypointer);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xAF13DA94);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (uint)ScriptContext.GlobalScriptContext.GetResult(typeof(uint));
			}
		}

        public static IntPtr GetEntityPointerFromRef(uint entityref){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityref);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xDBC17174);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static IntPtr GetConcreteEntityListPointer(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0x5756DB36);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static bool IsRefValidEntity(uint entityref){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(entityref);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x6E38A1FC);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static void PrintToConsole(int index, string message){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.Push(message);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x7F033898);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr GetFirstActiveEntity(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0x3E50DC41);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static ulong GetPlayerAuthorizedSteamid(int slot){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(slot);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xD1F30B3B);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (ulong)ScriptContext.GlobalScriptContext.GetResult(typeof(ulong));
			}
		}

        public static string GetPlayerIpAddress(int slot){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(slot);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x46A45CB0);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static void HookEvent(string name, InputArgument callback, bool ispost){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.Push(ispost);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xE71F04D5);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void UnhookEvent(string name, InputArgument callback, bool ispost){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.Push((InputArgument)callback);
			ScriptContext.GlobalScriptContext.Push(ispost);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2154AFAE);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr CreateEvent(string name, bool force){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.Push(force);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x7B472432);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static void FireEvent(IntPtr gameevent, bool dontbroadcast){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(dontbroadcast);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2D52AEE);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void FireEventToClient(IntPtr gameevent, int clientindex){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(clientindex);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x40B7C06C);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static string GetEventName(IntPtr gameevent){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xDFF86998);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static bool GetEventBool(IntPtr gameevent, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xDFFEE451);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
			}
		}

        public static int GetEventInt(IntPtr gameevent, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xB17427CC);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static float GetEventFloat(IntPtr gameevent, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xDF96CB6F);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (float)ScriptContext.GlobalScriptContext.GetResult(typeof(float));
			}
		}

        public static string GetEventString(IntPtr gameevent, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xB4EBC50A);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (string)ScriptContext.GlobalScriptContext.GetResult(typeof(string));
			}
		}

        public static void SetEventBool(IntPtr gameevent, string name, bool value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x31859DC5);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void SetEventFloat(IntPtr gameevent, string name, float value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x627CF47B);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void SetEventString(IntPtr gameevent, string name, string value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xCB7E7B9E);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void SetEventInt(IntPtr gameevent, string name, int value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x4F1363D8);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static int LoadEventsFromFile(string path, bool searchall){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(path);
			ScriptContext.GlobalScriptContext.Push(searchall);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xED480293);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static IntPtr GetEventPlayerController(IntPtr gameevent, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x88E33F2F);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static void SetEventPlayerController(IntPtr gameevent, string name, IntPtr value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xE8A2033B);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void SetEventEntity(IntPtr gameevent, string name, IntPtr value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xAB420F50);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void SetEventEntityIndex(IntPtr gameevent, string name, int value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xAF9B1691);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr GetEventPlayerPawn(IntPtr gameevent, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x80D3545B);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static ulong GetEventUint64(IntPtr gameevent, string name){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xA5EADD5B);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (ulong)ScriptContext.GlobalScriptContext.GetResult(typeof(ulong));
			}
		}

        public static void SetEventUint64(IntPtr gameevent, string name, ulong value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(gameevent);
			ScriptContext.GlobalScriptContext.Push(name);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xD0C2D3CF);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static IntPtr CreateVirtualFunction(IntPtr pointer, int vtableoffset, int numarguments, int returntype, object[] arguments){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(pointer);
			ScriptContext.GlobalScriptContext.Push(vtableoffset);
			ScriptContext.GlobalScriptContext.Push(numarguments);
			ScriptContext.GlobalScriptContext.Push(returntype);
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

        public static IntPtr CreateVirtualFunctionBySignature(IntPtr pointer, string binaryname, string signature, int numarguments, int returntype, object[] arguments){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(pointer);
			ScriptContext.GlobalScriptContext.Push(binaryname);
			ScriptContext.GlobalScriptContext.Push(signature);
			ScriptContext.GlobalScriptContext.Push(numarguments);
			ScriptContext.GlobalScriptContext.Push(returntype);
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

        public static void HookFunction(IntPtr function, InputArgument hook, bool post){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(function);
			ScriptContext.GlobalScriptContext.Push((InputArgument)hook);
			ScriptContext.GlobalScriptContext.Push(post);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xA6C8BA9B);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void UnhookFunction(IntPtr function, InputArgument hook, bool post){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(function);
			ScriptContext.GlobalScriptContext.Push((InputArgument)hook);
			ScriptContext.GlobalScriptContext.Push(post);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x2051B00);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static T ExecuteVirtualFunction<T>(IntPtr function, object[] arguments){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(function);
			foreach (var obj in arguments)
			{
				ScriptContext.GlobalScriptContext.Push(obj);
			}
			ScriptContext.GlobalScriptContext.SetIdentifier(0x376A0359);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (T)ScriptContext.GlobalScriptContext.GetResult(typeof(T));
			}
		}

        public static IntPtr FindSignature(string modulepath, string signature){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(modulepath);
			ScriptContext.GlobalScriptContext.Push(signature);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xE9E1819B);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static int GetNetworkVectorSize(IntPtr vec){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vec);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xA585F34E);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static IntPtr GetNetworkVectorElementAt(IntPtr vec, int index){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vec);
			ScriptContext.GlobalScriptContext.Push(index);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x67A31E3F);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static short GetSchemaOffset(string classname, string propname){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(classname);
			ScriptContext.GlobalScriptContext.Push(propname);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x57B77D8F);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (short)ScriptContext.GlobalScriptContext.GetResult(typeof(short));
			}
		}

        public static T GetSchemaValueByName<T>(IntPtr instance, int returntype, string classname, string propname){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(instance);
			ScriptContext.GlobalScriptContext.Push(returntype);
			ScriptContext.GlobalScriptContext.Push(classname);
			ScriptContext.GlobalScriptContext.Push(propname);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xD01E4EB5);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (T)ScriptContext.GlobalScriptContext.GetResult(typeof(T));
			}
		}

        public static void SetSchemaValueByName<T>(IntPtr instance, int returntype, string classname, string propname, T value){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(instance);
			ScriptContext.GlobalScriptContext.Push(returntype);
			ScriptContext.GlobalScriptContext.Push(classname);
			ScriptContext.GlobalScriptContext.Push(propname);
			ScriptContext.GlobalScriptContext.Push(value);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xAB9AA921);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static int GetSchemaClassSize(string classname){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(classname);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x9CE4FC56);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (int)ScriptContext.GlobalScriptContext.GetResult(typeof(int));
			}
		}

        public static IntPtr GetEconItemSystem(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0x981E9B5B);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (IntPtr)ScriptContext.GlobalScriptContext.GetResult(typeof(IntPtr));
			}
		}

        public static bool IsServerPaused(){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.SetIdentifier(0xB216AAAC);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			return (bool)ScriptContext.GlobalScriptContext.GetResult(typeof(bool));
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

        public static void VectorAngles(IntPtr vector, IntPtr pseudoup, IntPtr outangle){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vector);
			ScriptContext.GlobalScriptContext.Push(pseudoup);
			ScriptContext.GlobalScriptContext.Push(outangle);
			ScriptContext.GlobalScriptContext.SetIdentifier(0x6E6886B1);
			ScriptContext.GlobalScriptContext.Invoke();
			ScriptContext.GlobalScriptContext.CheckErrors();
			}
		}

        public static void AngleVectors(IntPtr vector, IntPtr forwardout, IntPtr rightout, IntPtr upout){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(vector);
			ScriptContext.GlobalScriptContext.Push(forwardout);
			ScriptContext.GlobalScriptContext.Push(rightout);
			ScriptContext.GlobalScriptContext.Push(upout);
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

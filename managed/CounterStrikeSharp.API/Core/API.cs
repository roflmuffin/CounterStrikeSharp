
using System;

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

        public static void ServerCommand(string command){
			lock (ScriptContext.GlobalScriptContext.Lock) {
			ScriptContext.GlobalScriptContext.Reset();
			ScriptContext.GlobalScriptContext.Push(command);
			ScriptContext.GlobalScriptContext.SetIdentifier(0xCD0A5AB8);
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

import{_ as t,c as o,a2 as s,o as a}from"./chunks/framework.CQii86iU.js";const b=JSON.parse('{"title":"Class TcpStateObject","description":"","frontmatter":{},"headers":[],"relativePath":"docfx/Tool.Sockets.Kernels.TcpStateObject.md","filePath":"zh/docfx/Tool.Sockets.Kernels.TcpStateObject.md"}'),i={name:"docfx/Tool.Sockets.Kernels.TcpStateObject.md"};function n(l,e,r,c,_,p){return a(),o("div",null,e[0]||(e[0]=[s('<h1 id="class-tcpstateobject" tabindex="-1"><a id="Tool_Sockets_Kernels_TcpStateObject"></a> Class TcpStateObject <a class="header-anchor" href="#class-tcpstateobject" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_TcpStateObject&quot;&gt;&lt;/a&gt; Class TcpStateObject&quot;">​</a></h1><p>Namespace: <a href="./Tool.Sockets.Kernels.html">Tool.Sockets.Kernels</a><br> Assembly: Tool.Net.dll</p><p>对异步接收时的对象状态的封装，将socket与接收到的数据封装在一起</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> class</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> TcpStateObject</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> : </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">StateObject</span></span></code></pre></div><h4 id="inheritance" tabindex="-1">Inheritance <a class="header-anchor" href="#inheritance" aria-label="Permalink to &quot;Inheritance&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a> ← <a href="./Tool.Sockets.Kernels.StateObject.html">StateObject</a> ← <a href="./Tool.Sockets.Kernels.TcpStateObject.html">TcpStateObject</a></p><h4 id="inherited-members" tabindex="-1">Inherited Members <a class="header-anchor" href="#inherited-members" aria-label="Permalink to &quot;Inherited Members&quot;">​</a></h4><p><a href="./Tool.Sockets.Kernels.StateObject.html#Tool_Sockets_Kernels_StateObject_EmptyIpv4Port">StateObject.EmptyIpv4Port</a>, <a href="./Tool.Sockets.Kernels.StateObject.html#Tool_Sockets_Kernels_StateObject_HeadSize">StateObject.HeadSize</a>, <a href="./Tool.Sockets.Kernels.StateObject.html#Tool_Sockets_Kernels_StateObject_KeepAliveObj">StateObject.KeepAliveObj</a>, <a href="./Tool.Sockets.Kernels.StateObject.html#Tool_Sockets_Kernels_StateObject_CreateSocket_System_Boolean_Tool_Sockets_Kernels_NetBufferSize_">StateObject.CreateSocket(bool, NetBufferSize)</a>, <a href="./Tool.Sockets.Kernels.StateObject.html#Tool_Sockets_Kernels_StateObject_SocketIsDispose_System_Net_Sockets_Socket_">StateObject.SocketIsDispose(Socket)</a>, <a href="./Tool.Sockets.Kernels.StateObject.html#Tool_Sockets_Kernels_StateObject_HashCodeByte_System_Memory_System_Byte___">StateObject.HashCodeByte(in Memory&lt;byte&gt;)</a>, <a href="./Tool.Sockets.Kernels.StateObject.html#Tool_Sockets_Kernels_StateObject_QueueUserWorkItem__1_Tool_Sockets_Kernels_ReceiveEvent___0__Tool_Sockets_Kernels_ReceiveBytes___0__">StateObject.QueueUserWorkItem&lt;T&gt;(ReceiveEvent&lt;T&gt;, ReceiveBytes&lt;T&gt;)</a>, <a href="./Tool.Sockets.Kernels.StateObject.html#Tool_Sockets_Kernels_StateObject_ReceivedAsync__1_Tool_Sockets_Kernels_ReceiveEvent___0__Tool_Sockets_Kernels_ReceiveBytes___0__">StateObject.ReceivedAsync&lt;T&gt;(ReceiveEvent&lt;T&gt;, ReceiveBytes&lt;T&gt;)</a>, <a href="./Tool.Sockets.Kernels.StateObject.html#Tool_Sockets_Kernels_StateObject_GetIpPort_System_Net_HttpListenerContext_">StateObject.GetIpPort(HttpListenerContext)</a>, <a href="./Tool.Sockets.Kernels.StateObject.html#Tool_Sockets_Kernels_StateObject_GetIpPort_System_Net_Sockets_Socket_">StateObject.GetIpPort(Socket)</a>, <a href="./Tool.Sockets.Kernels.StateObject.html#Tool_Sockets_Kernels_StateObject_GetIpPort_System_Net_EndPoint_">StateObject.GetIpPort(EndPoint)</a>, <a href="./Tool.Sockets.Kernels.StateObject.html#Tool_Sockets_Kernels_StateObject_IsIpPort_System_String_Tool_Sockets_Kernels_Ipv4Port__">StateObject.IsIpPort(string, out Ipv4Port)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object)" target="_blank" rel="noreferrer">object.Equals(object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object-system-object)" target="_blank" rel="noreferrer">object.Equals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gethashcode" target="_blank" rel="noreferrer">object.GetHashCode()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gettype" target="_blank" rel="noreferrer">object.GetType()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone" target="_blank" rel="noreferrer">object.MemberwiseClone()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.referenceequals" target="_blank" rel="noreferrer">object.ReferenceEquals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.tostring" target="_blank" rel="noreferrer">object.ToString()</a></p><h4 id="extension-methods" tabindex="-1">Extension Methods <a class="header-anchor" href="#extension-methods" aria-label="Permalink to &quot;Extension Methods&quot;">​</a></h4><p><a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Add__1_System_Object_System_Object_System_Object_">ObjectExtension.Add&lt;T&gt;(object, object, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_CopyEntity_System_Object_System_Object_System_String___">ObjectExtension.CopyEntity(object, object, params string[])</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_">ObjectExtension.EntityToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_">ObjectExtension.EntityToJson(object, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_System_String_">ObjectExtension.EntityToJson(object, bool, string)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_GetDictionary_System_Object_">DictionaryExtension.GetDictionary(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, Type, string, out bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtr_System_Object_">ObjectExtension.GetIntPtr(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtrInt_System_Object_">ObjectExtension.GetIntPtrInt(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertieFind_System_Object_System_String_System_Boolean_">TypeExtension.GetPropertieFind(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetProperties_System_Object_">TypeExtension.GetProperties(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, Type, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_ComponentModel_PropertyDescriptor_">TypeExtension.GetValue(object, PropertyDescriptor)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_System_Boolean_">TypeExtension.GetValue(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_">TypeExtension.GetValue(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, object, int, int)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Int32_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, int, object, int, int)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_SetDictionary_System_Object_System_Collections_Generic_IDictionary_System_String_System_Object__">DictionaryExtension.SetDictionary(object, IDictionary&lt;string, object&gt;)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey__1_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetFieldKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey__1_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetPropertyKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_ComponentModel_PropertyDescriptor_System_Object_">TypeExtension.SetValue(object, PropertyDescriptor, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_System_Boolean_">TypeExtension.SetValue(object, string, object, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_">TypeExtension.SetValue(object, string, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBase64String_System_Object_">ObjectExtension.ToBase64String(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_">ObjectExtension.ToBytes(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_System_Type__">ObjectExtension.ToBytes(object, out Type)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary_System_Object_">DictionaryExtension.ToDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary__1_System_Object_">DictionaryExtension.ToDictionary&lt;T&gt;(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary_System_Object_">DictionaryExtension.ToIDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary__1_System_Object_">DictionaryExtension.ToIDictionary&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_">ObjectExtension.ToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_System_Text_Json_JsonSerializerOptions_">ObjectExtension.ToJson(object, JsonSerializerOptions)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_">ObjectExtension.ToJsonWeb(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_System_Action_System_Text_Json_JsonSerializerOptions__">ObjectExtension.ToJsonWeb(object, Action&lt;JsonSerializerOptions&gt;)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToTryVar__1_System_Object___0_">ObjectExtension.ToTryVar&lt;T&gt;(object, T)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar__1_System_Object_">ObjectExtension.ToVar&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_Type_System_Boolean_">ObjectExtension.ToVar(object, Type, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_String_">ObjectExtension.ToVar(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToXml_System_Object_">ObjectExtension.ToXml(object)</a></p><h2 id="remarks" tabindex="-1">Remarks <a class="header-anchor" href="#remarks" aria-label="Permalink to &quot;Remarks&quot;">​</a></h2><p>代码由逆血提供支持</p><h2 id="constructors" tabindex="-1">Constructors <a class="header-anchor" href="#constructors" aria-label="Permalink to &quot;Constructors&quot;">​</a></h2><h3 id="tcpstateobject-socket" tabindex="-1"><a id="Tool_Sockets_Kernels_TcpStateObject__ctor_System_Net_Sockets_Socket_"></a> TcpStateObject(Socket) <a class="header-anchor" href="#tcpstateobject-socket" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_TcpStateObject__ctor_System_Net_Sockets_Socket_&quot;&gt;&lt;/a&gt; TcpStateObject\\(Socket\\)&quot;">​</a></h3><p>构造包信息</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> TcpStateObject</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(Socket Client)</span></span></code></pre></div><h4 id="parameters" tabindex="-1">Parameters <a class="header-anchor" href="#parameters" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>Client</code> <a href="https://learn.microsoft.com/dotnet/api/system.net.sockets.socket" target="_blank" rel="noreferrer">Socket</a></p><p>对象</p><h3 id="tcpstateobject-socket-int-bool-receiveevent-socket" tabindex="-1"><a id="Tool_Sockets_Kernels_TcpStateObject__ctor_System_Net_Sockets_Socket_System_Int32_System_Boolean_Tool_Sockets_Kernels_ReceiveEvent_System_Net_Sockets_Socket__"></a> TcpStateObject(Socket, int, bool, ReceiveEvent&lt;Socket&gt;) <a class="header-anchor" href="#tcpstateobject-socket-int-bool-receiveevent-socket" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_TcpStateObject__ctor_System_Net_Sockets_Socket_System_Int32_System_Boolean_Tool_Sockets_Kernels_ReceiveEvent_System_Net_Sockets_Socket__&quot;&gt;&lt;/a&gt; TcpStateObject\\(Socket, int, bool, ReceiveEvent&lt;Socket\\&gt;\\)&quot;">​</a></h3><p>有参构造</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> TcpStateObject</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(Socket Client, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> DataLength, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">bool</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> OnlyData, ReceiveEvent</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">&lt;</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">Socket</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">&gt;</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> received)</span></span></code></pre></div><h4 id="parameters-1" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-1" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>Client</code> <a href="https://learn.microsoft.com/dotnet/api/system.net.sockets.socket" target="_blank" rel="noreferrer">Socket</a></p><p>对象</p><p><code>DataLength</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>包的大小</p><p><code>OnlyData</code> <a href="https://learn.microsoft.com/dotnet/api/system.boolean" target="_blank" rel="noreferrer">bool</a></p><p>数据唯一标识</p><p><code>received</code> <a href="./Tool.Sockets.Kernels.ReceiveEvent-1.html">ReceiveEvent</a>&lt;<a href="https://learn.microsoft.com/dotnet/api/system.net.sockets.socket" target="_blank" rel="noreferrer">Socket</a>&gt;</p><p>委托函数</p><h2 id="properties" tabindex="-1">Properties <a class="header-anchor" href="#properties" aria-label="Permalink to &quot;Properties&quot;">​</a></h2><h3 id="client" tabindex="-1"><a id="Tool_Sockets_Kernels_TcpStateObject_Client"></a> Client <a class="header-anchor" href="#client" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_TcpStateObject_Client&quot;&gt;&lt;/a&gt; Client&quot;">​</a></h3><p>为 TCP 网络服务提供客户端连接。</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> Socket Client { get; set; }</span></span></code></pre></div><h4 id="property-value" tabindex="-1">Property Value <a class="header-anchor" href="#property-value" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.net.sockets.socket" target="_blank" rel="noreferrer">Socket</a></p><h3 id="datalength" tabindex="-1"><a id="Tool_Sockets_Kernels_TcpStateObject_DataLength"></a> DataLength <a class="header-anchor" href="#datalength" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_TcpStateObject_DataLength&quot;&gt;&lt;/a&gt; DataLength&quot;">​</a></h3><p>可用最大空间</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> int</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> DataLength { get; }</span></span></code></pre></div><h4 id="property-value-1" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-1" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><h3 id="ipport" tabindex="-1"><a id="Tool_Sockets_Kernels_TcpStateObject_IpPort"></a> IpPort <a class="header-anchor" href="#ipport" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_TcpStateObject_IpPort&quot;&gt;&lt;/a&gt; IpPort&quot;">​</a></h3><p>当前对象唯一的IP：端口</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> UserKey IpPort { get; set; }</span></span></code></pre></div><h4 id="property-value-2" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-2" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="./Tool.Sockets.Kernels.UserKey.html">UserKey</a></p><h3 id="onlydata" tabindex="-1"><a id="Tool_Sockets_Kernels_TcpStateObject_OnlyData"></a> OnlyData <a class="header-anchor" href="#onlydata" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_TcpStateObject_OnlyData&quot;&gt;&lt;/a&gt; OnlyData&quot;">​</a></h3><p>是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> bool</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> OnlyData { get; }</span></span></code></pre></div><h4 id="property-value-3" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-3" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.boolean" target="_blank" rel="noreferrer">bool</a></p><h2 id="methods" tabindex="-1">Methods <a class="header-anchor" href="#methods" aria-label="Permalink to &quot;Methods&quot;">​</a></h2><h3 id="clientclose" tabindex="-1"><a id="Tool_Sockets_Kernels_TcpStateObject_ClientClose"></a> ClientClose() <a class="header-anchor" href="#clientclose" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_TcpStateObject_ClientClose&quot;&gt;&lt;/a&gt; ClientClose\\(\\)&quot;">​</a></h3><p>关闭连接</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ClientClose</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h3 id="close" tabindex="-1"><a id="Tool_Sockets_Kernels_TcpStateObject_Close"></a> Close() <a class="header-anchor" href="#close" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_TcpStateObject_Close&quot;&gt;&lt;/a&gt; Close\\(\\)&quot;">​</a></h3><p>回收对象所以资源</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Close</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h3 id="isconnected-socket" tabindex="-1"><a id="Tool_Sockets_Kernels_TcpStateObject_IsConnected_System_Net_Sockets_Socket_"></a> IsConnected(Socket) <a class="header-anchor" href="#isconnected-socket" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_TcpStateObject_IsConnected_System_Net_Sockets_Socket_&quot;&gt;&lt;/a&gt; IsConnected\\(Socket\\)&quot;">​</a></h3><p>根据Socket获取当前连接是否已经断开</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> bool</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> IsConnected</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Socket</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Client</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-2" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-2" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>Client</code> <a href="https://learn.microsoft.com/dotnet/api/system.net.sockets.socket" target="_blank" rel="noreferrer">Socket</a></p><h4 id="returns" tabindex="-1">Returns <a class="header-anchor" href="#returns" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.boolean" target="_blank" rel="noreferrer">bool</a></p>',66)]))}const y=t(i,[["render",n]]);export{b as __pageData,y as default};
import{_ as t,c as s,a2 as a,o as i}from"./chunks/framework.CQii86iU.js";const k=JSON.parse('{"title":"Class NetworkConnect<ISocket>","description":"","frontmatter":{},"headers":[],"relativePath":"docfx/Tool.Sockets.Kernels.NetworkConnect-1.md","filePath":"zh/docfx/Tool.Sockets.Kernels.NetworkConnect-1.md"}'),o={name:"docfx/Tool.Sockets.Kernels.NetworkConnect-1.md"};function n(l,e,r,h,c,p){return i(),s("div",null,e[0]||(e[0]=[a('<h1 id="class-networkconnect-isocket" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1"></a> Class NetworkConnect&lt;ISocket&gt; <a class="header-anchor" href="#class-networkconnect-isocket" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1&quot;&gt;&lt;/a&gt; Class NetworkConnect&lt;ISocket\\&gt;&quot;">​</a></h1><p>Namespace: <a href="./Tool.Sockets.Kernels.html">Tool.Sockets.Kernels</a><br> Assembly: Tool.Net.dll</p><p>通信公共模板抽象类（客户端版）</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> abstract</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> class</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> NetworkConnect</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">ISocket</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; : </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">EnClientEventDrive</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">INetworkConnect</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">ISocket</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt;, </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">INetworkConnect</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">INetworkCore</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">IDisposable</span></span></code></pre></div><h4 id="type-parameters" tabindex="-1">Type Parameters <a class="header-anchor" href="#type-parameters" aria-label="Permalink to &quot;Type Parameters&quot;">​</a></h4><p><code>ISocket</code></p><h4 id="inheritance" tabindex="-1">Inheritance <a class="header-anchor" href="#inheritance" aria-label="Permalink to &quot;Inheritance&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a> ← <a href="./Tool.Sockets.Kernels.EnClientEventDrive.html">EnClientEventDrive</a> ← <a href="./Tool.Sockets.Kernels.NetworkConnect-1.html">NetworkConnect&lt;ISocket&gt;</a></p><h4 id="implements" tabindex="-1">Implements <a class="header-anchor" href="#implements" aria-label="Permalink to &quot;Implements&quot;">​</a></h4><p><a href="./Tool.Sockets.Kernels.INetworkConnect-1.html">INetworkConnect&lt;ISocket&gt;</a>, <a href="./Tool.Sockets.Kernels.INetworkConnect.html">INetworkConnect</a>, <a href="./Tool.Sockets.Kernels.INetworkCore.html">INetworkCore</a>, <a href="https://learn.microsoft.com/dotnet/api/system.idisposable" target="_blank" rel="noreferrer">IDisposable</a></p><h4 id="inherited-members" tabindex="-1">Inherited Members <a class="header-anchor" href="#inherited-members" aria-label="Permalink to &quot;Inherited Members&quot;">​</a></h4><p><a href="./Tool.Sockets.Kernels.EnClientEventDrive.html#Tool_Sockets_Kernels_EnClientEventDrive_OnInterceptor_Tool_Sockets_Kernels_EnClient_System_Boolean_">EnClientEventDrive.OnInterceptor(EnClient, bool)</a>, <a href="./Tool.Sockets.Kernels.EnClientEventDrive.html#Tool_Sockets_Kernels_EnClientEventDrive_OnIsQueue_Tool_Sockets_Kernels_EnClient_System_Boolean_">EnClientEventDrive.OnIsQueue(EnClient, bool)</a>, <a href="./Tool.Sockets.Kernels.EnClientEventDrive.html#Tool_Sockets_Kernels_EnClientEventDrive_IsEvent_Tool_Sockets_Kernels_EnClient_">EnClientEventDrive.IsEvent(EnClient)</a>, <a href="./Tool.Sockets.Kernels.EnClientEventDrive.html#Tool_Sockets_Kernels_EnClientEventDrive_IsQueue_Tool_Sockets_Kernels_EnClient_">EnClientEventDrive.IsQueue(EnClient)</a>, <a href="./Tool.Sockets.Kernels.EnClientEventDrive.html#Tool_Sockets_Kernels_EnClientEventDrive_OpenAllEvent">EnClientEventDrive.OpenAllEvent()</a>, <a href="./Tool.Sockets.Kernels.EnClientEventDrive.html#Tool_Sockets_Kernels_EnClientEventDrive_OpenAllQueue">EnClientEventDrive.OpenAllQueue()</a>, <a href="./Tool.Sockets.Kernels.EnClientEventDrive.html#Tool_Sockets_Kernels_EnClientEventDrive_CloseAllEvent">EnClientEventDrive.CloseAllEvent()</a>, <a href="./Tool.Sockets.Kernels.EnClientEventDrive.html#Tool_Sockets_Kernels_EnClientEventDrive_CloseAllQueue">EnClientEventDrive.CloseAllQueue()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object)" target="_blank" rel="noreferrer">object.Equals(object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object-system-object)" target="_blank" rel="noreferrer">object.Equals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gethashcode" target="_blank" rel="noreferrer">object.GetHashCode()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gettype" target="_blank" rel="noreferrer">object.GetType()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone" target="_blank" rel="noreferrer">object.MemberwiseClone()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.referenceequals" target="_blank" rel="noreferrer">object.ReferenceEquals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.tostring" target="_blank" rel="noreferrer">object.ToString()</a></p><h4 id="extension-methods" tabindex="-1">Extension Methods <a class="header-anchor" href="#extension-methods" aria-label="Permalink to &quot;Extension Methods&quot;">​</a></h4><p><a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Add__1_System_Object_System_Object_System_Object_">ObjectExtension.Add&lt;T&gt;(object, object, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_CopyEntity_System_Object_System_Object_System_String___">ObjectExtension.CopyEntity(object, object, params string[])</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_">ObjectExtension.EntityToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_">ObjectExtension.EntityToJson(object, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_System_String_">ObjectExtension.EntityToJson(object, bool, string)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_GetDictionary_System_Object_">DictionaryExtension.GetDictionary(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, Type, string, out bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtr_System_Object_">ObjectExtension.GetIntPtr(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtrInt_System_Object_">ObjectExtension.GetIntPtrInt(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertieFind_System_Object_System_String_System_Boolean_">TypeExtension.GetPropertieFind(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetProperties_System_Object_">TypeExtension.GetProperties(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, Type, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_ComponentModel_PropertyDescriptor_">TypeExtension.GetValue(object, PropertyDescriptor)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_System_Boolean_">TypeExtension.GetValue(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_">TypeExtension.GetValue(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, object, int, int)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Int32_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, int, object, int, int)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_SetDictionary_System_Object_System_Collections_Generic_IDictionary_System_String_System_Object__">DictionaryExtension.SetDictionary(object, IDictionary&lt;string, object&gt;)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey__1_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetFieldKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey__1_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetPropertyKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_ComponentModel_PropertyDescriptor_System_Object_">TypeExtension.SetValue(object, PropertyDescriptor, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_System_Boolean_">TypeExtension.SetValue(object, string, object, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_">TypeExtension.SetValue(object, string, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBase64String_System_Object_">ObjectExtension.ToBase64String(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_">ObjectExtension.ToBytes(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_System_Type__">ObjectExtension.ToBytes(object, out Type)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary_System_Object_">DictionaryExtension.ToDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary__1_System_Object_">DictionaryExtension.ToDictionary&lt;T&gt;(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary_System_Object_">DictionaryExtension.ToIDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary__1_System_Object_">DictionaryExtension.ToIDictionary&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_">ObjectExtension.ToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_System_Text_Json_JsonSerializerOptions_">ObjectExtension.ToJson(object, JsonSerializerOptions)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_">ObjectExtension.ToJsonWeb(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_System_Action_System_Text_Json_JsonSerializerOptions__">ObjectExtension.ToJsonWeb(object, Action&lt;JsonSerializerOptions&gt;)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToTryVar__1_System_Object___0_">ObjectExtension.ToTryVar&lt;T&gt;(object, T)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar__1_System_Object_">ObjectExtension.ToVar&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_Type_System_Boolean_">ObjectExtension.ToVar(object, Type, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_String_">ObjectExtension.ToVar(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToXml_System_Object_">ObjectExtension.ToXml(object)</a></p><h2 id="remarks" tabindex="-1">Remarks <a class="header-anchor" href="#remarks" aria-label="Permalink to &quot;Remarks&quot;">​</a></h2><p>代码由逆血提供支持</p><h2 id="constructors" tabindex="-1">Constructors <a class="header-anchor" href="#constructors" aria-label="Permalink to &quot;Constructors&quot;">​</a></h2><h3 id="networkconnect" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1__ctor"></a> NetworkConnect() <a class="header-anchor" href="#networkconnect" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1__ctor&quot;&gt;&lt;/a&gt; NetworkConnect\\(\\)&quot;">​</a></h3><p>默认构造（公共模板信息）</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">protected</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> NetworkConnect</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h2 id="properties" tabindex="-1">Properties <a class="header-anchor" href="#properties" aria-label="Permalink to &quot;Properties&quot;">​</a></h2><h3 id="buffersize" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_BufferSize"></a> BufferSize <a class="header-anchor" href="#buffersize" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_BufferSize&quot;&gt;&lt;/a&gt; BufferSize&quot;">​</a></h3><p>表示通讯的包大小</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> virtual</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> NetBufferSize BufferSize { get; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">protected</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> init</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">; }</span></span></code></pre></div><h4 id="property-value" tabindex="-1">Property Value <a class="header-anchor" href="#property-value" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="./Tool.Sockets.Kernels.NetBufferSize.html">NetBufferSize</a></p><h3 id="connected" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_Connected"></a> Connected <a class="header-anchor" href="#connected" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_Connected&quot;&gt;&lt;/a&gt; Connected&quot;">​</a></h3><p>获取一个值，该值指示 Client 的基础 Socket 是否已连接到远程主机。</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> abstract</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> bool</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> Connected { get; }</span></span></code></pre></div><h4 id="property-value-1" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-1" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.boolean" target="_blank" rel="noreferrer">bool</a></p><h3 id="isclose" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_IsClose"></a> IsClose <a class="header-anchor" href="#isclose" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_IsClose&quot;&gt;&lt;/a&gt; IsClose&quot;">​</a></h3><p>标识客户端是否关闭，改状态为调用关闭方法后的状态。</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> abstract</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> bool</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> IsClose { get; }</span></span></code></pre></div><h4 id="property-value-2" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-2" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.boolean" target="_blank" rel="noreferrer">bool</a></p><h3 id="isthreadpool" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_IsThreadPool"></a> IsThreadPool <a class="header-anchor" href="#isthreadpool" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_IsThreadPool&quot;&gt;&lt;/a&gt; IsThreadPool&quot;">​</a></h3><p>是否使用线程池调度接收后的数据 默认 true 开启</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> virtual</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> bool</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> IsThreadPool { get; set; }</span></span></code></pre></div><h4 id="property-value-3" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-3" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.boolean" target="_blank" rel="noreferrer">bool</a></p><h3 id="localpoint" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_LocalPoint"></a> LocalPoint <a class="header-anchor" href="#localpoint" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_LocalPoint&quot;&gt;&lt;/a&gt; LocalPoint&quot;">​</a></h3><p>当前设备的连接信息</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> abstract</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> Ipv4Port LocalPoint { get; }</span></span></code></pre></div><h4 id="property-value-4" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-4" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="./Tool.Sockets.Kernels.Ipv4Port.html">Ipv4Port</a></p><h3 id="millisecond" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_Millisecond"></a> Millisecond <a class="header-anchor" href="#millisecond" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_Millisecond&quot;&gt;&lt;/a&gt; Millisecond&quot;">​</a></h3><p>监听控制毫秒</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> abstract</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> int</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> Millisecond { get; set; }</span></span></code></pre></div><h4 id="property-value-5" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-5" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><h3 id="server" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_Server"></a> Server <a class="header-anchor" href="#server" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_Server&quot;&gt;&lt;/a&gt; Server&quot;">​</a></h3><p>服务器的连接信息</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> abstract</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> UserKey Server { get; }</span></span></code></pre></div><h4 id="property-value-6" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-6" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="./Tool.Sockets.Kernels.UserKey.html">UserKey</a></p><h2 id="methods" tabindex="-1">Methods <a class="header-anchor" href="#methods" aria-label="Permalink to &quot;Methods&quot;">​</a></h2><h3 id="close" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_Close"></a> Close() <a class="header-anchor" href="#close" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_Close&quot;&gt;&lt;/a&gt; Close\\(\\)&quot;">​</a></h3><p>TCP关闭</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> abstract</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Close</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h3 id="connectasync-string-int" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_ConnectAsync_System_String_System_Int32_"></a> ConnectAsync(string, int) <a class="header-anchor" href="#connectasync-string-int" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_ConnectAsync_System_String_System_Int32_&quot;&gt;&lt;/a&gt; ConnectAsync\\(string, int\\)&quot;">​</a></h3><p>异步连接</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> abstract</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Task</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ConnectAsync</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">string</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ip</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> port</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters" tabindex="-1">Parameters <a class="header-anchor" href="#parameters" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>ip</code> <a href="https://learn.microsoft.com/dotnet/api/system.string" target="_blank" rel="noreferrer">string</a></p><p>要连接的服务器的ip地址</p><p><code>port</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>要连接的服务器的端口</p><h4 id="returns" tabindex="-1">Returns <a class="header-anchor" href="#returns" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.task" target="_blank" rel="noreferrer">Task</a></p><h3 id="createsendbytes-int" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_CreateSendBytes_System_Int32_"></a> CreateSendBytes(int) <a class="header-anchor" href="#createsendbytes-int" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_CreateSendBytes_System_Int32_&quot;&gt;&lt;/a&gt; CreateSendBytes\\(int\\)&quot;">​</a></h3><p>创建数据发送空间</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> abstract</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> SendBytes</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">ISocket</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">CreateSendBytes</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> length</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-1" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-1" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>length</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>数据大小</p><h4 id="returns-1" tabindex="-1">Returns <a class="header-anchor" href="#returns-1" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="./Tool.Sockets.Kernels.SendBytes-1.html">SendBytes</a>&lt;ISocket&gt;</p><h3 id="dispose" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_Dispose"></a> Dispose() <a class="header-anchor" href="#dispose" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_Dispose&quot;&gt;&lt;/a&gt; Dispose\\(\\)&quot;">​</a></h3><p>关闭连接，回收相关资源</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> abstract</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Dispose</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h3 id="oncomplete-in-userkey-enclient" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_OnComplete_Tool_Sockets_Kernels_UserKey__Tool_Sockets_Kernels_EnClient_"></a> OnComplete(in UserKey, EnClient) <a class="header-anchor" href="#oncomplete-in-userkey-enclient" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_OnComplete_Tool_Sockets_Kernels_UserKey__Tool_Sockets_Kernels_EnClient_&quot;&gt;&lt;/a&gt; OnComplete\\(in UserKey, EnClient\\)&quot;">​</a></h3><p>可供开发重写的事件方法</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> abstract</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ValueTask</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">IGetQueOnEnum</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">OnComplete</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">in</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> UserKey</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> key</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">EnClient</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> enAction</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-2" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-2" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>key</code> <a href="./Tool.Sockets.Kernels.UserKey.html">UserKey</a></p><p>IP：端口</p><p><code>enAction</code> <a href="./Tool.Sockets.Kernels.EnClient.html">EnClient</a></p><p>消息类型</p><h4 id="returns-2" tabindex="-1">Returns <a class="header-anchor" href="#returns-2" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask-1" target="_blank" rel="noreferrer">ValueTask</a>&lt;<a href="./Tool.Sockets.Kernels.IGetQueOnEnum.html">IGetQueOnEnum</a>&gt;</p><h3 id="reconnection" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_Reconnection"></a> Reconnection() <a class="header-anchor" href="#reconnection" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_Reconnection&quot;&gt;&lt;/a&gt; Reconnection\\(\\)&quot;">​</a></h3><p>重连，返回是否重连，如果没有断开是不会重连的</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> abstract</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Task</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">bool</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Reconnection</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h4 id="returns-3" tabindex="-1">Returns <a class="header-anchor" href="#returns-3" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1" target="_blank" rel="noreferrer">Task</a>&lt;<a href="https://learn.microsoft.com/dotnet/api/system.boolean" target="_blank" rel="noreferrer">bool</a>&gt;</p><h3 id="sendasync-sendbytes-isocket" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_SendAsync_Tool_Sockets_Kernels_SendBytes__0__"></a> SendAsync(SendBytes&lt;ISocket&gt;) <a class="header-anchor" href="#sendasync-sendbytes-isocket" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_SendAsync_Tool_Sockets_Kernels_SendBytes__0__&quot;&gt;&lt;/a&gt; SendAsync\\(SendBytes&lt;ISocket\\&gt;\\)&quot;">​</a></h3><p>异步发送消息</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> abstract</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ValueTask</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> SendAsync</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">SendBytes</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">ISocket</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">sendBytes</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-3" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-3" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>sendBytes</code> <a href="./Tool.Sockets.Kernels.SendBytes-1.html">SendBytes</a>&lt;ISocket&gt;</p><p>数据包</p><h4 id="returns-4" tabindex="-1">Returns <a class="header-anchor" href="#returns-4" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask" target="_blank" rel="noreferrer">ValueTask</a></p><h3 id="setcompleted-completedevent-enclient" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_SetCompleted_Tool_Sockets_Kernels_CompletedEvent_Tool_Sockets_Kernels_EnClient__"></a> SetCompleted(CompletedEvent&lt;EnClient&gt;) <a class="header-anchor" href="#setcompleted-completedevent-enclient" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_SetCompleted_Tool_Sockets_Kernels_CompletedEvent_Tool_Sockets_Kernels_EnClient__&quot;&gt;&lt;/a&gt; SetCompleted\\(CompletedEvent&lt;EnClient\\&gt;\\)&quot;">​</a></h3><p>连接、发送、关闭事件</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> abstract</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> SetCompleted</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">CompletedEvent</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">EnClient</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Completed</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-4" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-4" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>Completed</code> <a href="./Tool.Sockets.Kernels.CompletedEvent-1.html">CompletedEvent</a>&lt;<a href="./Tool.Sockets.Kernels.EnClient.html">EnClient</a>&gt;</p><h3 id="setreceived-receiveevent-isocket" tabindex="-1"><a id="Tool_Sockets_Kernels_NetworkConnect_1_SetReceived_Tool_Sockets_Kernels_ReceiveEvent__0__"></a> SetReceived(ReceiveEvent&lt;ISocket&gt;) <a class="header-anchor" href="#setreceived-receiveevent-isocket" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_NetworkConnect_1_SetReceived_Tool_Sockets_Kernels_ReceiveEvent__0__&quot;&gt;&lt;/a&gt; SetReceived\\(ReceiveEvent&lt;ISocket\\&gt;\\)&quot;">​</a></h3><p>接收到数据事件</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> abstract</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> SetReceived</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">ReceiveEvent</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">ISocket</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Received</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-5" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-5" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>Received</code> <a href="./Tool.Sockets.Kernels.ReceiveEvent-1.html">ReceiveEvent</a>&lt;ISocket&gt;</p>',114)]))}const d=t(o,[["render",n]]);export{k as __pageData,d as default};
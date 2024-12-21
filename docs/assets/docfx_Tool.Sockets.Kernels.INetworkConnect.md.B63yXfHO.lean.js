import{_ as i,c as l,a2 as n,G as o,j as t,a,B as r,o as h}from"./chunks/framework.CQii86iU.js";const E=JSON.parse('{"title":"Interface INetworkConnect","description":"","frontmatter":{},"headers":[],"relativePath":"docfx/Tool.Sockets.Kernels.INetworkConnect.md","filePath":"zh/docfx/Tool.Sockets.Kernels.INetworkConnect.md"}'),_={name:"docfx/Tool.Sockets.Kernels.INetworkConnect.md"};function c(p,e,d,k,y,b){const s=r("xref");return h(),l("div",null,[e[2]||(e[2]=n('<h1 id="interface-inetworkconnect" tabindex="-1"><a id="Tool_Sockets_Kernels_INetworkConnect"></a> Interface INetworkConnect <a class="header-anchor" href="#interface-inetworkconnect" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_INetworkConnect&quot;&gt;&lt;/a&gt; Interface INetworkConnect&quot;">​</a></h1><p>Namespace: <a href="./Tool.Sockets.Kernels.html">Tool.Sockets.Kernels</a><br> Assembly: Tool.Net.dll</p><p>连接通信模型</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> interface</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> INetworkConnect</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> : </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">INetworkCore</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">IDisposable</span></span></code></pre></div><h4 id="implements" tabindex="-1">Implements <a class="header-anchor" href="#implements" aria-label="Permalink to &quot;Implements&quot;">​</a></h4><p><a href="./Tool.Sockets.Kernels.INetworkCore.html">INetworkCore</a>, <a href="https://learn.microsoft.com/dotnet/api/system.idisposable" target="_blank" rel="noreferrer">IDisposable</a></p><h4 id="extension-methods" tabindex="-1">Extension Methods <a class="header-anchor" href="#extension-methods" aria-label="Permalink to &quot;Extension Methods&quot;">​</a></h4><p><a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Add__1_System_Object_System_Object_System_Object_">ObjectExtension.Add&lt;T&gt;(object, object, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_CopyEntity_System_Object_System_Object_System_String___">ObjectExtension.CopyEntity(object, object, params string[])</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_">ObjectExtension.EntityToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_">ObjectExtension.EntityToJson(object, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_System_String_">ObjectExtension.EntityToJson(object, bool, string)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_GetDictionary_System_Object_">DictionaryExtension.GetDictionary(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, Type, string, out bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtr_System_Object_">ObjectExtension.GetIntPtr(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtrInt_System_Object_">ObjectExtension.GetIntPtrInt(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertieFind_System_Object_System_String_System_Boolean_">TypeExtension.GetPropertieFind(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetProperties_System_Object_">TypeExtension.GetProperties(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, Type, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_ComponentModel_PropertyDescriptor_">TypeExtension.GetValue(object, PropertyDescriptor)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_System_Boolean_">TypeExtension.GetValue(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_">TypeExtension.GetValue(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, object, int, int)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Int32_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, int, object, int, int)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_SetDictionary_System_Object_System_Collections_Generic_IDictionary_System_String_System_Object__">DictionaryExtension.SetDictionary(object, IDictionary&lt;string, object&gt;)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey__1_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetFieldKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey__1_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetPropertyKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_ComponentModel_PropertyDescriptor_System_Object_">TypeExtension.SetValue(object, PropertyDescriptor, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_System_Boolean_">TypeExtension.SetValue(object, string, object, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_">TypeExtension.SetValue(object, string, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBase64String_System_Object_">ObjectExtension.ToBase64String(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_">ObjectExtension.ToBytes(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_System_Type__">ObjectExtension.ToBytes(object, out Type)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary_System_Object_">DictionaryExtension.ToDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary__1_System_Object_">DictionaryExtension.ToDictionary&lt;T&gt;(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary_System_Object_">DictionaryExtension.ToIDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary__1_System_Object_">DictionaryExtension.ToIDictionary&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_">ObjectExtension.ToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_System_Text_Json_JsonSerializerOptions_">ObjectExtension.ToJson(object, JsonSerializerOptions)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_">ObjectExtension.ToJsonWeb(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_System_Action_System_Text_Json_JsonSerializerOptions__">ObjectExtension.ToJsonWeb(object, Action&lt;JsonSerializerOptions&gt;)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToTryVar__1_System_Object___0_">ObjectExtension.ToTryVar&lt;T&gt;(object, T)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar__1_System_Object_">ObjectExtension.ToVar&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_Type_System_Boolean_">ObjectExtension.ToVar(object, Type, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_String_">ObjectExtension.ToVar(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToXml_System_Object_">ObjectExtension.ToXml(object)</a></p><h2 id="properties" tabindex="-1">Properties <a class="header-anchor" href="#properties" aria-label="Permalink to &quot;Properties&quot;">​</a></h2><h3 id="connected" tabindex="-1"><a id="Tool_Sockets_Kernels_INetworkConnect_Connected"></a> Connected <a class="header-anchor" href="#connected" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_INetworkConnect_Connected&quot;&gt;&lt;/a&gt; Connected&quot;">​</a></h3><p>是否连接中</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">bool</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> Connected { get; }</span></span></code></pre></div><h4 id="property-value" tabindex="-1">Property Value <a class="header-anchor" href="#property-value" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.boolean" target="_blank" rel="noreferrer">bool</a></p><h3 id="localpoint" tabindex="-1"><a id="Tool_Sockets_Kernels_INetworkConnect_LocalPoint"></a> LocalPoint <a class="header-anchor" href="#localpoint" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_INetworkConnect_LocalPoint&quot;&gt;&lt;/a&gt; LocalPoint&quot;">​</a></h3><p>本机通信IP</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">Ipv4Port LocalPoint { get; }</span></span></code></pre></div><h4 id="property-value-1" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-1" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="./Tool.Sockets.Kernels.Ipv4Port.html">Ipv4Port</a></p><h2 id="methods" tabindex="-1">Methods <a class="header-anchor" href="#methods" aria-label="Permalink to &quot;Methods&quot;">​</a></h2><h3 id="close" tabindex="-1"><a id="Tool_Sockets_Kernels_INetworkConnect_Close"></a> Close() <a class="header-anchor" href="#close" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_INetworkConnect_Close&quot;&gt;&lt;/a&gt; Close\\(\\)&quot;">​</a></h3><p>关闭当前连接</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Close</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h3 id="connectasync-string-int" tabindex="-1"><a id="Tool_Sockets_Kernels_INetworkConnect_ConnectAsync_System_String_System_Int32_"></a> ConnectAsync(string, int) <a class="header-anchor" href="#connectasync-string-int" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_INetworkConnect_ConnectAsync_System_String_System_Int32_&quot;&gt;&lt;/a&gt; ConnectAsync\\(string, int\\)&quot;">​</a></h3><p>连接服务器</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Task</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ConnectAsync</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">string</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ip</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> port</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters" tabindex="-1">Parameters <a class="header-anchor" href="#parameters" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>ip</code> <a href="https://learn.microsoft.com/dotnet/api/system.string" target="_blank" rel="noreferrer">string</a></p><p><code>port</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><h4 id="returns" tabindex="-1">Returns <a class="header-anchor" href="#returns" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.task" target="_blank" rel="noreferrer">Task</a></p><h3 id="isevent-enclient" tabindex="-1"><a id="Tool_Sockets_Kernels_INetworkConnect_IsEvent_Tool_Sockets_Kernels_EnClient_"></a> IsEvent(EnClient) <a class="header-anchor" href="#isevent-enclient" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_INetworkConnect_IsEvent_Tool_Sockets_Kernels_EnClient_&quot;&gt;&lt;/a&gt; IsEvent\\(EnClient\\)&quot;">​</a></h3><p>获取该事件是否会触发</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">bool</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> IsEvent</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">EnClient</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> enClient</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-1" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-1" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>enClient</code> <a href="./Tool.Sockets.Kernels.EnClient.html">EnClient</a></p>',36)),o(s,{href:"Tool.Sockets.Kernels.EnClient","data-throw-if-not-resolved":"false"}),e[3]||(e[3]=t("h4",{id:"returns-1",tabindex:"-1"},[a("Returns "),t("a",{class:"header-anchor",href:"#returns-1","aria-label":'Permalink to "Returns"'},"​")],-1)),e[4]||(e[4]=t("p",null,[t("a",{href:"https://learn.microsoft.com/dotnet/api/system.boolean",target:"_blank",rel:"noreferrer"},"bool")],-1)),o(s,{href:"System.Boolean","data-throw-if-not-resolved":"false"}),e[5]||(e[5]=n('<h3 id="isqueue-enclient" tabindex="-1"><a id="Tool_Sockets_Kernels_INetworkConnect_IsQueue_Tool_Sockets_Kernels_EnClient_"></a> IsQueue(EnClient) <a class="header-anchor" href="#isqueue-enclient" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_INetworkConnect_IsQueue_Tool_Sockets_Kernels_EnClient_&quot;&gt;&lt;/a&gt; IsQueue\\(EnClient\\)&quot;">​</a></h3><p>获取该事件是否在队列任务中运行</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">bool</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> IsQueue</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">EnClient</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> enClient</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-2" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-2" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>enClient</code> <a href="./Tool.Sockets.Kernels.EnClient.html">EnClient</a></p>',5)),o(s,{href:"Tool.Sockets.Kernels.EnClient","data-throw-if-not-resolved":"false"}),e[6]||(e[6]=t("h4",{id:"returns-2",tabindex:"-1"},[a("Returns "),t("a",{class:"header-anchor",href:"#returns-2","aria-label":'Permalink to "Returns"'},"​")],-1)),e[7]||(e[7]=t("p",null,[t("a",{href:"https://learn.microsoft.com/dotnet/api/system.boolean",target:"_blank",rel:"noreferrer"},"bool")],-1)),o(s,{href:"System.Boolean","data-throw-if-not-resolved":"false"}),e[8]||(e[8]=n('<h3 id="oncomplete-in-userkey-enclient" tabindex="-1"><a id="Tool_Sockets_Kernels_INetworkConnect_OnComplete_Tool_Sockets_Kernels_UserKey__Tool_Sockets_Kernels_EnClient_"></a> OnComplete(in UserKey, EnClient) <a class="header-anchor" href="#oncomplete-in-userkey-enclient" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_INetworkConnect_OnComplete_Tool_Sockets_Kernels_UserKey__Tool_Sockets_Kernels_EnClient_&quot;&gt;&lt;/a&gt; OnComplete\\(in UserKey, EnClient\\)&quot;">​</a></h3><p>可重写的事件</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">ValueTask</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">IGetQueOnEnum</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">OnComplete</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">in</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> UserKey</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> key</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">EnClient</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> enAction</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-3" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-3" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>key</code> <a href="./Tool.Sockets.Kernels.UserKey.html">UserKey</a></p><p><code>enAction</code> <a href="./Tool.Sockets.Kernels.EnClient.html">EnClient</a></p><h4 id="returns-3" tabindex="-1">Returns <a class="header-anchor" href="#returns-3" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask-1" target="_blank" rel="noreferrer">ValueTask</a>&lt;<a href="./Tool.Sockets.Kernels.IGetQueOnEnum.html">IGetQueOnEnum</a>&gt;</p><h3 id="oninterceptor-enclient-bool" tabindex="-1"><a id="Tool_Sockets_Kernels_INetworkConnect_OnInterceptor_Tool_Sockets_Kernels_EnClient_System_Boolean_"></a> OnInterceptor(EnClient, bool) <a class="header-anchor" href="#oninterceptor-enclient-bool" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_INetworkConnect_OnInterceptor_Tool_Sockets_Kernels_EnClient_System_Boolean_&quot;&gt;&lt;/a&gt; OnInterceptor\\(EnClient, bool\\)&quot;">​</a></h3><p>设置开启或关闭不想收到的消息事件</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">bool</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> OnInterceptor</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">EnClient</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> enClient</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">bool</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> state</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-4" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-4" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>enClient</code> <a href="./Tool.Sockets.Kernels.EnClient.html">EnClient</a></p>',13)),o(s,{href:"Tool.Sockets.Kernels.EnClient","data-throw-if-not-resolved":"false"}),e[9]||(e[9]=n('<p><code>state</code> <a href="https://learn.microsoft.com/dotnet/api/system.boolean" target="_blank" rel="noreferrer">bool</a></p><p>等于true时生效，将关闭一切的相关事件</p><h4 id="returns-4" tabindex="-1">Returns <a class="header-anchor" href="#returns-4" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.boolean" target="_blank" rel="noreferrer">bool</a></p><p>返回true时表示设置成功！</p><h3 id="onisqueue-enclient-bool" tabindex="-1"><a id="Tool_Sockets_Kernels_INetworkConnect_OnIsQueue_Tool_Sockets_Kernels_EnClient_System_Boolean_"></a> OnIsQueue(EnClient, bool) <a class="header-anchor" href="#onisqueue-enclient-bool" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_INetworkConnect_OnIsQueue_Tool_Sockets_Kernels_EnClient_System_Boolean_&quot;&gt;&lt;/a&gt; OnIsQueue\\(EnClient, bool\\)&quot;">​</a></h3>',6)),t("p",null,[e[0]||(e[0]=a("设置将")),o(s,{href:"Tool.Sockets.Kernels.EnClient","data-throw-if-not-resolved":"false"}),e[1]||(e[1]=a("事件，载入或不载入"))]),e[10]||(e[10]=n('<div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">bool</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> OnIsQueue</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">EnClient</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> enClient</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">bool</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> state</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-5" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-5" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>enClient</code> <a href="./Tool.Sockets.Kernels.EnClient.html">EnClient</a></p>',3)),o(s,{href:"Tool.Sockets.Kernels.EnClient","data-throw-if-not-resolved":"false"}),e[11]||(e[11]=n('<p><code>state</code> <a href="https://learn.microsoft.com/dotnet/api/system.boolean" target="_blank" rel="noreferrer">bool</a></p><p>等于true时，事件由队列线程完成，false时交由任务线程自行完成</p><h4 id="returns-5" tabindex="-1">Returns <a class="header-anchor" href="#returns-5" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.boolean" target="_blank" rel="noreferrer">bool</a></p><p>返回true时表示设置成功！</p><h3 id="reconnection" tabindex="-1"><a id="Tool_Sockets_Kernels_INetworkConnect_Reconnection"></a> Reconnection() <a class="header-anchor" href="#reconnection" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_INetworkConnect_Reconnection&quot;&gt;&lt;/a&gt; Reconnection\\(\\)&quot;">​</a></h3><p>重连</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Task</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">bool</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Reconnection</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h4 id="returns-6" tabindex="-1">Returns <a class="header-anchor" href="#returns-6" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1" target="_blank" rel="noreferrer">Task</a>&lt;<a href="https://learn.microsoft.com/dotnet/api/system.boolean" target="_blank" rel="noreferrer">bool</a>&gt;</p><h3 id="setcompleted-completedevent-enclient" tabindex="-1"><a id="Tool_Sockets_Kernels_INetworkConnect_SetCompleted_Tool_Sockets_Kernels_CompletedEvent_Tool_Sockets_Kernels_EnClient__"></a> SetCompleted(CompletedEvent&lt;EnClient&gt;) <a class="header-anchor" href="#setcompleted-completedevent-enclient" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Sockets_Kernels_INetworkConnect_SetCompleted_Tool_Sockets_Kernels_CompletedEvent_Tool_Sockets_Kernels_EnClient__&quot;&gt;&lt;/a&gt; SetCompleted\\(CompletedEvent&lt;EnClient\\&gt;\\)&quot;">​</a></h3><p>相关事件委托</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> SetCompleted</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">CompletedEvent</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">EnClient</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Completed</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-6" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-6" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>Completed</code> <a href="./Tool.Sockets.Kernels.CompletedEvent-1.html">CompletedEvent</a>&lt;<a href="./Tool.Sockets.Kernels.EnClient.html">EnClient</a>&gt;</p>',15))])}const T=i(_,[["render",c]]);export{E as __pageData,T as default};

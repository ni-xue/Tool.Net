import{_ as e,c as s,a2 as i,o as a}from"./chunks/framework.CQii86iU.js";const y=JSON.parse('{"title":"Class IntPtrHelper","description":"","frontmatter":{},"headers":[],"relativePath":"docfx/Tool.Utils.IntPtrHelper.md","filePath":"zh/docfx/Tool.Utils.IntPtrHelper.md"}'),n={name:"docfx/Tool.Utils.IntPtrHelper.md"};function r(o,t,l,p,h,_){return a(),s("div",null,t[0]||(t[0]=[i('<h1 id="class-intptrhelper" tabindex="-1"><a id="Tool_Utils_IntPtrHelper"></a> Class IntPtrHelper <a class="header-anchor" href="#class-intptrhelper" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_IntPtrHelper&quot;&gt;&lt;/a&gt; Class IntPtrHelper&quot;">​</a></h1><p>Namespace: <a href="./Tool.Utils.html">Tool.Utils</a><br> Assembly: Tool.Net.dll</p><p>关于内存地址读写的操作帮助类</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> class</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> IntPtrHelper</span></span></code></pre></div><h4 id="inheritance" tabindex="-1">Inheritance <a class="header-anchor" href="#inheritance" aria-label="Permalink to &quot;Inheritance&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a> ← <a href="./Tool.Utils.IntPtrHelper.html">IntPtrHelper</a></p><h4 id="inherited-members" tabindex="-1">Inherited Members <a class="header-anchor" href="#inherited-members" aria-label="Permalink to &quot;Inherited Members&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object)" target="_blank" rel="noreferrer">object.Equals(object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object-system-object)" target="_blank" rel="noreferrer">object.Equals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gethashcode" target="_blank" rel="noreferrer">object.GetHashCode()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gettype" target="_blank" rel="noreferrer">object.GetType()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone" target="_blank" rel="noreferrer">object.MemberwiseClone()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.referenceequals" target="_blank" rel="noreferrer">object.ReferenceEquals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.tostring" target="_blank" rel="noreferrer">object.ToString()</a></p><h4 id="extension-methods" tabindex="-1">Extension Methods <a class="header-anchor" href="#extension-methods" aria-label="Permalink to &quot;Extension Methods&quot;">​</a></h4><p><a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Add__1_System_Object_System_Object_System_Object_">ObjectExtension.Add&lt;T&gt;(object, object, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_CopyEntity_System_Object_System_Object_System_String___">ObjectExtension.CopyEntity(object, object, params string[])</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_">ObjectExtension.EntityToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_">ObjectExtension.EntityToJson(object, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_System_String_">ObjectExtension.EntityToJson(object, bool, string)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_GetDictionary_System_Object_">DictionaryExtension.GetDictionary(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, Type, string, out bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtr_System_Object_">ObjectExtension.GetIntPtr(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtrInt_System_Object_">ObjectExtension.GetIntPtrInt(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertieFind_System_Object_System_String_System_Boolean_">TypeExtension.GetPropertieFind(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetProperties_System_Object_">TypeExtension.GetProperties(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, Type, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_ComponentModel_PropertyDescriptor_">TypeExtension.GetValue(object, PropertyDescriptor)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_System_Boolean_">TypeExtension.GetValue(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_">TypeExtension.GetValue(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, object, int, int)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Int32_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, int, object, int, int)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_SetDictionary_System_Object_System_Collections_Generic_IDictionary_System_String_System_Object__">DictionaryExtension.SetDictionary(object, IDictionary&lt;string, object&gt;)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey__1_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetFieldKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey__1_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetPropertyKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_ComponentModel_PropertyDescriptor_System_Object_">TypeExtension.SetValue(object, PropertyDescriptor, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_System_Boolean_">TypeExtension.SetValue(object, string, object, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_">TypeExtension.SetValue(object, string, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBase64String_System_Object_">ObjectExtension.ToBase64String(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_">ObjectExtension.ToBytes(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_System_Type__">ObjectExtension.ToBytes(object, out Type)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary_System_Object_">DictionaryExtension.ToDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary__1_System_Object_">DictionaryExtension.ToDictionary&lt;T&gt;(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary_System_Object_">DictionaryExtension.ToIDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary__1_System_Object_">DictionaryExtension.ToIDictionary&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_">ObjectExtension.ToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_System_Text_Json_JsonSerializerOptions_">ObjectExtension.ToJson(object, JsonSerializerOptions)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_">ObjectExtension.ToJsonWeb(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_System_Action_System_Text_Json_JsonSerializerOptions__">ObjectExtension.ToJsonWeb(object, Action&lt;JsonSerializerOptions&gt;)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToTryVar__1_System_Object___0_">ObjectExtension.ToTryVar&lt;T&gt;(object, T)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar__1_System_Object_">ObjectExtension.ToVar&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_Type_System_Boolean_">ObjectExtension.ToVar(object, Type, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_String_">ObjectExtension.ToVar(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToXml_System_Object_">ObjectExtension.ToXml(object)</a></p><h2 id="remarks" tabindex="-1">Remarks <a class="header-anchor" href="#remarks" aria-label="Permalink to &quot;Remarks&quot;">​</a></h2><p>代码由逆血提供支持</p><h2 id="methods" tabindex="-1">Methods <a class="header-anchor" href="#methods" aria-label="Permalink to &quot;Methods&quot;">​</a></h2><h3 id="getpidbyprocessname-string" tabindex="-1"><a id="Tool_Utils_IntPtrHelper_GetPidByProcessName_System_String_"></a> GetPidByProcessName(string) <a class="header-anchor" href="#getpidbyprocessname-string" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_IntPtrHelper_GetPidByProcessName_System_String_&quot;&gt;&lt;/a&gt; GetPidByProcessName\\(string\\)&quot;">​</a></h3><p>根据进程名称获取进程ID</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> GetPidByProcessName</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">string</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> processName</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters" tabindex="-1">Parameters <a class="header-anchor" href="#parameters" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>processName</code> <a href="https://learn.microsoft.com/dotnet/api/system.string" target="_blank" rel="noreferrer">string</a></p><p>进程名字</p><h4 id="returns" tabindex="-1">Returns <a class="header-anchor" href="#returns" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><h3 id="readmemoryvalue-int-string" tabindex="-1"><a id="Tool_Utils_IntPtrHelper_ReadMemoryValue_System_Int32_System_String_"></a> ReadMemoryValue(int, string) <a class="header-anchor" href="#readmemoryvalue-int-string" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_IntPtrHelper_ReadMemoryValue_System_Int32_System_String_&quot;&gt;&lt;/a&gt; ReadMemoryValue\\(int, string\\)&quot;">​</a></h3><p>读取内存中的值</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ReadMemoryValue</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> baseAddress</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">string</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> processName</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-1" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-1" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>baseAddress</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>内存地址</p><p><code>processName</code> <a href="https://learn.microsoft.com/dotnet/api/system.string" target="_blank" rel="noreferrer">string</a></p><p>进程名</p><h4 id="returns-1" tabindex="-1">Returns <a class="header-anchor" href="#returns-1" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><h3 id="readmemoryvalue-int-int" tabindex="-1"><a id="Tool_Utils_IntPtrHelper_ReadMemoryValue_System_Int32_System_Int32_"></a> ReadMemoryValue(int, int) <a class="header-anchor" href="#readmemoryvalue-int-int" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_IntPtrHelper_ReadMemoryValue_System_Int32_System_Int32_&quot;&gt;&lt;/a&gt; ReadMemoryValue\\(int, int\\)&quot;">​</a></h3><p>读取内存中的值</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ReadMemoryValue</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> baseAddress</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> processId</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-2" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-2" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>baseAddress</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>内存地址</p><p><code>processId</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>进程ID</p><h4 id="returns-2" tabindex="-1">Returns <a class="header-anchor" href="#returns-2" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><h3 id="readmemoryvalue-intptr-string" tabindex="-1"><a id="Tool_Utils_IntPtrHelper_ReadMemoryValue_System_IntPtr_System_String_"></a> ReadMemoryValue(IntPtr, string) <a class="header-anchor" href="#readmemoryvalue-intptr-string" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_IntPtrHelper_ReadMemoryValue_System_IntPtr_System_String_&quot;&gt;&lt;/a&gt; ReadMemoryValue\\(IntPtr, string\\)&quot;">​</a></h3><p>读取内存中的值</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ReadMemoryValue</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">IntPtr</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> baseAddress</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">string</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> processName</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-3" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-3" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>baseAddress</code> <a href="https://learn.microsoft.com/dotnet/api/system.intptr" target="_blank" rel="noreferrer">IntPtr</a></p><p>内存地址</p><p><code>processName</code> <a href="https://learn.microsoft.com/dotnet/api/system.string" target="_blank" rel="noreferrer">string</a></p><p>进程名</p><h4 id="returns-3" tabindex="-1">Returns <a class="header-anchor" href="#returns-3" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><h3 id="readmemoryvalue-intptr-int" tabindex="-1"><a id="Tool_Utils_IntPtrHelper_ReadMemoryValue_System_IntPtr_System_Int32_"></a> ReadMemoryValue(IntPtr, int) <a class="header-anchor" href="#readmemoryvalue-intptr-int" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_IntPtrHelper_ReadMemoryValue_System_IntPtr_System_Int32_&quot;&gt;&lt;/a&gt; ReadMemoryValue\\(IntPtr, int\\)&quot;">​</a></h3><p>读取内存中的值</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ReadMemoryValue</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">IntPtr</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> baseAddress</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> processId</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-4" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-4" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>baseAddress</code> <a href="https://learn.microsoft.com/dotnet/api/system.intptr" target="_blank" rel="noreferrer">IntPtr</a></p><p>内存地址</p><p><code>processId</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>进程ID</p><h4 id="returns-4" tabindex="-1">Returns <a class="header-anchor" href="#returns-4" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><h3 id="writememoryvalue-int-string-int" tabindex="-1"><a id="Tool_Utils_IntPtrHelper_WriteMemoryValue_System_Int32_System_String_System_Int32_"></a> WriteMemoryValue(int, string, int) <a class="header-anchor" href="#writememoryvalue-int-string-int" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_IntPtrHelper_WriteMemoryValue_System_Int32_System_String_System_Int32_&quot;&gt;&lt;/a&gt; WriteMemoryValue\\(int, string, int\\)&quot;">​</a></h3><p>将值写入指定内存地址中</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> WriteMemoryValue</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> baseAddress</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">string</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> processName</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> value</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-5" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-5" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>baseAddress</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>内存地址</p><p><code>processName</code> <a href="https://learn.microsoft.com/dotnet/api/system.string" target="_blank" rel="noreferrer">string</a></p><p>进程名</p><p><code>value</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>写入的值</p><h3 id="writememoryvalue-int-int-int" tabindex="-1"><a id="Tool_Utils_IntPtrHelper_WriteMemoryValue_System_Int32_System_Int32_System_Int32_"></a> WriteMemoryValue(int, int, int) <a class="header-anchor" href="#writememoryvalue-int-int-int" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_IntPtrHelper_WriteMemoryValue_System_Int32_System_Int32_System_Int32_&quot;&gt;&lt;/a&gt; WriteMemoryValue\\(int, int, int\\)&quot;">​</a></h3><p>将值写入指定内存地址中</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> WriteMemoryValue</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> baseAddress</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> processId</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> value</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-6" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-6" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>baseAddress</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>内存地址</p><p><code>processId</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>进程ID</p><p><code>value</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>写入的值</p><h3 id="writememoryvalue-intptr-string-int" tabindex="-1"><a id="Tool_Utils_IntPtrHelper_WriteMemoryValue_System_IntPtr_System_String_System_Int32_"></a> WriteMemoryValue(IntPtr, string, int) <a class="header-anchor" href="#writememoryvalue-intptr-string-int" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_IntPtrHelper_WriteMemoryValue_System_IntPtr_System_String_System_Int32_&quot;&gt;&lt;/a&gt; WriteMemoryValue\\(IntPtr, string, int\\)&quot;">​</a></h3><p>将值写入指定内存地址中</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> WriteMemoryValue</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">IntPtr</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> baseAddress</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">string</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> processName</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> value</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-7" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-7" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>baseAddress</code> <a href="https://learn.microsoft.com/dotnet/api/system.intptr" target="_blank" rel="noreferrer">IntPtr</a></p><p>内存地址</p><p><code>processName</code> <a href="https://learn.microsoft.com/dotnet/api/system.string" target="_blank" rel="noreferrer">string</a></p><p>进程名</p><p><code>value</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>写入的值</p><h3 id="writememoryvalue-intptr-int-int" tabindex="-1"><a id="Tool_Utils_IntPtrHelper_WriteMemoryValue_System_IntPtr_System_Int32_System_Int32_"></a> WriteMemoryValue(IntPtr, int, int) <a class="header-anchor" href="#writememoryvalue-intptr-int-int" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_IntPtrHelper_WriteMemoryValue_System_IntPtr_System_Int32_System_Int32_&quot;&gt;&lt;/a&gt; WriteMemoryValue\\(IntPtr, int, int\\)&quot;">​</a></h3><p>将值写入指定内存地址中</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> WriteMemoryValue</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">IntPtr</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> baseAddress</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> processId</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> value</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-8" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-8" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>baseAddress</code> <a href="https://learn.microsoft.com/dotnet/api/system.intptr" target="_blank" rel="noreferrer">IntPtr</a></p><p>内存地址</p><p><code>processId</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>进程ID</p><p><code>value</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>写入的值</p>',101)]))}const m=e(n,[["render",r]]);export{y as __pageData,m as default};
import{_ as o,c as r,a2 as i,j as t,a as s,G as n,B as l,o as h}from"./chunks/framework.CQii86iU.js";const b=JSON.parse('{"title":"Class MemorySegment<T>","description":"","frontmatter":{},"headers":[],"relativePath":"docfx/Tool.Utils.MemorySegment-1.md","filePath":"zh/docfx/Tool.Utils.MemorySegment-1.md"}'),p={name:"docfx/Tool.Utils.MemorySegment-1.md"};function y(_,e,c,m,d,g){const a=l("xref");return h(),r("div",null,[e[6]||(e[6]=i('<h1 id="class-memorysegment-t" tabindex="-1"><a id="Tool_Utils_MemorySegment_1"></a> Class MemorySegment&lt;T&gt; <a class="header-anchor" href="#class-memorysegment-t" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1&quot;&gt;&lt;/a&gt; Class MemorySegment&lt;T\\&gt;&quot;">​</a></h1><p>Namespace: <a href="./Tool.Utils.html">Tool.Utils</a><br> Assembly: Tool.Net.dll</p><p>提供内存连续模型</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> sealed</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> class</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> MemorySegment</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; : </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">ReadOnlySequenceSegment</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt;</span></span></code></pre></div><h4 id="type-parameters" tabindex="-1">Type Parameters <a class="header-anchor" href="#type-parameters" aria-label="Permalink to &quot;Type Parameters&quot;">​</a></h4><p><code>T</code></p><p>类型</p><h4 id="inheritance" tabindex="-1">Inheritance <a class="header-anchor" href="#inheritance" aria-label="Permalink to &quot;Inheritance&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a> ← <a href="https://learn.microsoft.com/dotnet/api/system.buffers.readonlysequencesegment-1" target="_blank" rel="noreferrer">ReadOnlySequenceSegment&lt;T&gt;</a> ← <a href="./Tool.Utils.MemorySegment-1.html">MemorySegment&lt;T&gt;</a></p><h4 id="inherited-members" tabindex="-1">Inherited Members <a class="header-anchor" href="#inherited-members" aria-label="Permalink to &quot;Inherited Members&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.buffers.readonlysequencesegment-1.memory" target="_blank" rel="noreferrer">ReadOnlySequenceSegment&lt;T&gt;.Memory</a>, <a href="https://learn.microsoft.com/dotnet/api/system.buffers.readonlysequencesegment-1.next" target="_blank" rel="noreferrer">ReadOnlySequenceSegment&lt;T&gt;.Next</a>, <a href="https://learn.microsoft.com/dotnet/api/system.buffers.readonlysequencesegment-1.runningindex" target="_blank" rel="noreferrer">ReadOnlySequenceSegment&lt;T&gt;.RunningIndex</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object)" target="_blank" rel="noreferrer">object.Equals(object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object-system-object)" target="_blank" rel="noreferrer">object.Equals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gethashcode" target="_blank" rel="noreferrer">object.GetHashCode()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gettype" target="_blank" rel="noreferrer">object.GetType()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.referenceequals" target="_blank" rel="noreferrer">object.ReferenceEquals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.tostring" target="_blank" rel="noreferrer">object.ToString()</a></p><h4 id="extension-methods" tabindex="-1">Extension Methods <a class="header-anchor" href="#extension-methods" aria-label="Permalink to &quot;Extension Methods&quot;">​</a></h4><p><a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Add__1_System_Object_System_Object_System_Object_">ObjectExtension.Add&lt;T&gt;(object, object, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_CopyEntity_System_Object_System_Object_System_String___">ObjectExtension.CopyEntity(object, object, params string[])</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_">ObjectExtension.EntityToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_">ObjectExtension.EntityToJson(object, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_System_String_">ObjectExtension.EntityToJson(object, bool, string)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_GetDictionary_System_Object_">DictionaryExtension.GetDictionary(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, Type, string, out bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtr_System_Object_">ObjectExtension.GetIntPtr(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtrInt_System_Object_">ObjectExtension.GetIntPtrInt(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertieFind_System_Object_System_String_System_Boolean_">TypeExtension.GetPropertieFind(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetProperties_System_Object_">TypeExtension.GetProperties(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, Type, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_ComponentModel_PropertyDescriptor_">TypeExtension.GetValue(object, PropertyDescriptor)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_System_Boolean_">TypeExtension.GetValue(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_">TypeExtension.GetValue(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, object, int, int)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Int32_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, int, object, int, int)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_SetDictionary_System_Object_System_Collections_Generic_IDictionary_System_String_System_Object__">DictionaryExtension.SetDictionary(object, IDictionary&lt;string, object&gt;)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey__1_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetFieldKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey__1_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetPropertyKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_ComponentModel_PropertyDescriptor_System_Object_">TypeExtension.SetValue(object, PropertyDescriptor, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_System_Boolean_">TypeExtension.SetValue(object, string, object, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_">TypeExtension.SetValue(object, string, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBase64String_System_Object_">ObjectExtension.ToBase64String(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_">ObjectExtension.ToBytes(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_System_Type__">ObjectExtension.ToBytes(object, out Type)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary_System_Object_">DictionaryExtension.ToDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary__1_System_Object_">DictionaryExtension.ToDictionary&lt;T&gt;(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary_System_Object_">DictionaryExtension.ToIDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary__1_System_Object_">DictionaryExtension.ToIDictionary&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_">ObjectExtension.ToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_System_Text_Json_JsonSerializerOptions_">ObjectExtension.ToJson(object, JsonSerializerOptions)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_">ObjectExtension.ToJsonWeb(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_System_Action_System_Text_Json_JsonSerializerOptions__">ObjectExtension.ToJsonWeb(object, Action&lt;JsonSerializerOptions&gt;)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToTryVar__1_System_Object___0_">ObjectExtension.ToTryVar&lt;T&gt;(object, T)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar__1_System_Object_">ObjectExtension.ToVar&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_Type_System_Boolean_">ObjectExtension.ToVar(object, Type, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_String_">ObjectExtension.ToVar(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToXml_System_Object_">ObjectExtension.ToXml(object)</a></p><h2 id="remarks" tabindex="-1">Remarks <a class="header-anchor" href="#remarks" aria-label="Permalink to &quot;Remarks&quot;">​</a></h2><p>代码由逆血提供支持</p><h2 id="constructors" tabindex="-1">Constructors <a class="header-anchor" href="#constructors" aria-label="Permalink to &quot;Constructors&quot;">​</a></h2><h3 id="memorysegment" tabindex="-1"><a id="Tool_Utils_MemorySegment_1__ctor"></a> MemorySegment() <a class="header-anchor" href="#memorysegment" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1__ctor&quot;&gt;&lt;/a&gt; MemorySegment\\(\\)&quot;">​</a></h3><p>创建单一连续内存</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> MemorySegment</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h3 id="memorysegment-t" tabindex="-1"><a id="Tool_Utils_MemorySegment_1__ctor__0___"></a> MemorySegment(T[]) <a class="header-anchor" href="#memorysegment-t" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1__ctor__0___&quot;&gt;&lt;/a&gt; MemorySegment\\(T\\[\\]\\)&quot;">​</a></h3><p>创建单一连续内存</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> MemorySegment</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(T[] array)</span></span></code></pre></div><h4 id="parameters" tabindex="-1">Parameters <a class="header-anchor" href="#parameters" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>array</code> T[]</p><p>内存</p><h3 id="memorysegment-t-int-int" tabindex="-1"><a id="Tool_Utils_MemorySegment_1__ctor__0___System_Int32_System_Int32_"></a> MemorySegment(T[], int, int) <a class="header-anchor" href="#memorysegment-t-int-int" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1__ctor__0___System_Int32_System_Int32_&quot;&gt;&lt;/a&gt; MemorySegment\\(T\\[\\], int, int\\)&quot;">​</a></h3><p>创建单一连续内存</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> MemorySegment</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(T[] array, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> start, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> length)</span></span></code></pre></div><h4 id="parameters-1" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-1" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>array</code> T[]</p><p>内存</p><p><code>start</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>开始读取位置</p><p><code>length</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>读取长度</p><h3 id="memorysegment-in-readonlymemory-t" tabindex="-1"><a id="Tool_Utils_MemorySegment_1__ctor_System_ReadOnlyMemory__0___"></a> MemorySegment(in ReadOnlyMemory&lt;T&gt;) <a class="header-anchor" href="#memorysegment-in-readonlymemory-t" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1__ctor_System_ReadOnlyMemory__0___&quot;&gt;&lt;/a&gt; MemorySegment\\(in ReadOnlyMemory&lt;T\\&gt;\\)&quot;">​</a></h3><p>创建单一连续内存</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> MemorySegment</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">in</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> ReadOnlyMemory</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">&lt;</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">T</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">&gt;</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> memory)</span></span></code></pre></div><h4 id="parameters-2" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-2" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>memory</code> <a href="https://learn.microsoft.com/dotnet/api/system.readonlymemory-1" target="_blank" rel="noreferrer">ReadOnlyMemory</a>&lt;T&gt;</p><p>内存</p><h2 id="properties" tabindex="-1">Properties <a class="header-anchor" href="#properties" aria-label="Permalink to &quot;Properties&quot;">​</a></h2><h3 id="endnext" tabindex="-1"><a id="Tool_Utils_MemorySegment_1_EndNext"></a> EndNext <a class="header-anchor" href="#endnext" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1_EndNext&quot;&gt;&lt;/a&gt; EndNext&quot;">​</a></h3><p>获取节点的最底层</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> MemorySegment</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">&lt;</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">T</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">&gt;</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> EndNext { get; }</span></span></code></pre></div><h4 id="property-value" tabindex="-1">Property Value <a class="header-anchor" href="#property-value" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="./Tool.Utils.MemorySegment-1.html">MemorySegment</a>&lt;T&gt;</p><h3 id="isempty" tabindex="-1"><a id="Tool_Utils_MemorySegment_1_IsEmpty"></a> IsEmpty <a class="header-anchor" href="#isempty" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1_IsEmpty&quot;&gt;&lt;/a&gt; IsEmpty&quot;">​</a></h3><p>获取连续内存是否为空</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> bool</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> IsEmpty { get; }</span></span></code></pre></div><h4 id="property-value-1" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-1" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.boolean" target="_blank" rel="noreferrer">bool</a></p><h3 id="length" tabindex="-1"><a id="Tool_Utils_MemorySegment_1_Length"></a> Length <a class="header-anchor" href="#length" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1_Length&quot;&gt;&lt;/a&gt; Length&quot;">​</a></h3><p>获取连续内存的总长度</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> int</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> Length { get; }</span></span></code></pre></div><h4 id="property-value-2" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-2" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><h3 id="longlength" tabindex="-1"><a id="Tool_Utils_MemorySegment_1_LongLength"></a> LongLength <a class="header-anchor" href="#longlength" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1_LongLength&quot;&gt;&lt;/a&gt; LongLength&quot;">​</a></h3><p>获取连续内存的总长度</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> long</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> LongLength { get; }</span></span></code></pre></div><h4 id="property-value-3" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-3" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.int64" target="_blank" rel="noreferrer">long</a></p><h3 id="rank" tabindex="-1"><a id="Tool_Utils_MemorySegment_1_Rank"></a> Rank <a class="header-anchor" href="#rank" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1_Rank&quot;&gt;&lt;/a&gt; Rank&quot;">​</a></h3><p>层级数</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> int</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> Rank { get; }</span></span></code></pre></div><h4 id="property-value-4" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-4" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><h2 id="methods" tabindex="-1">Methods <a class="header-anchor" href="#methods" aria-label="Permalink to &quot;Methods&quot;">​</a></h2><h3 id="append-in-readonlymemory-t" tabindex="-1"><a id="Tool_Utils_MemorySegment_1_Append_System_ReadOnlyMemory__0___"></a> Append(in ReadOnlyMemory&lt;T&gt;) <a class="header-anchor" href="#append-in-readonlymemory-t" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1_Append_System_ReadOnlyMemory__0___&quot;&gt;&lt;/a&gt; Append\\(in ReadOnlyMemory&lt;T\\&gt;\\)&quot;">​</a></h3><p>添加连接的内存数据</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> MemorySegment</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Append</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">in</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ReadOnlyMemory</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">memory</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-3" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-3" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>memory</code> <a href="https://learn.microsoft.com/dotnet/api/system.readonlymemory-1" target="_blank" rel="noreferrer">ReadOnlyMemory</a>&lt;T&gt;</p><p>内存</p><h4 id="returns" tabindex="-1">Returns <a class="header-anchor" href="#returns" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="./Tool.Utils.MemorySegment-1.html">MemorySegment</a>&lt;T&gt;</p><p>新的连续内存</p><h3 id="append-t" tabindex="-1"><a id="Tool_Utils_MemorySegment_1_Append__0___"></a> Append(T[]) <a class="header-anchor" href="#append-t" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1_Append__0___&quot;&gt;&lt;/a&gt; Append\\(T\\[\\]\\)&quot;">​</a></h3><p>添加连接的内存数据</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> MemorySegment</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Append</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">[] </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">memory</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-4" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-4" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>memory</code> T[]</p><p>内存</p><h4 id="returns-1" tabindex="-1">Returns <a class="header-anchor" href="#returns-1" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="./Tool.Utils.MemorySegment-1.html">MemorySegment</a>&lt;T&gt;</p><p>新的连续内存</p><h3 id="copy-in-readonlymemory-t" tabindex="-1"><a id="Tool_Utils_MemorySegment_1_Copy_System_ReadOnlyMemory__0___"></a> Copy(in ReadOnlyMemory&lt;T&gt;) <a class="header-anchor" href="#copy-in-readonlymemory-t" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1_Copy_System_ReadOnlyMemory__0___&quot;&gt;&lt;/a&gt; Copy\\(in ReadOnlyMemory&lt;T\\&gt;\\)&quot;">​</a></h3><p>复制一份内存到连续内存中</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Copy</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">in</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ReadOnlyMemory</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">memory</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-5" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-5" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>memory</code> <a href="https://learn.microsoft.com/dotnet/api/system.readonlymemory-1" target="_blank" rel="noreferrer">ReadOnlyMemory</a>&lt;T&gt;</p><p>内存</p><h3 id="empty" tabindex="-1"><a id="Tool_Utils_MemorySegment_1_Empty"></a> Empty() <a class="header-anchor" href="#empty" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1_Empty&quot;&gt;&lt;/a&gt; Empty\\(\\)&quot;">​</a></h3><p>清空当前连续内存</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Empty</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h3 id="toreadonlysequence" tabindex="-1"><a id="Tool_Utils_MemorySegment_1_ToReadOnlySequence"></a> ToReadOnlySequence() <a class="header-anchor" href="#toreadonlysequence" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1_ToReadOnlySequence&quot;&gt;&lt;/a&gt; ToReadOnlySequence\\(\\)&quot;">​</a></h3>',96)),t("p",null,[e[0]||(e[0]=s("创建可读的连续")),n(a,{href:"System.Buffers.ReadOnlySequence%601","data-throw-if-not-resolved":"false"}),e[1]||(e[1]=s("（顺序串联）"))]),e[7]||(e[7]=i('<div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ReadOnlySequence</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">ToReadOnlySequence</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h4 id="returns-2" tabindex="-1">Returns <a class="header-anchor" href="#returns-2" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.buffers.readonlysequence-1" target="_blank" rel="noreferrer">ReadOnlySequence</a>&lt;T&gt;</p>',3)),t("p",null,[e[2]||(e[2]=s("返回")),n(a,{href:"System.Buffers.ReadOnlySequence%601","data-throw-if-not-resolved":"false"})]),e[8]||(e[8]=t("h3",{id:"toreadonlysequence-int-int",tabindex:"-1"},[t("a",{id:"Tool_Utils_MemorySegment_1_ToReadOnlySequence_System_Int32_System_Int32_"}),s(" ToReadOnlySequence(int, int) "),t("a",{class:"header-anchor",href:"#toreadonlysequence-int-int","aria-label":'Permalink to "<a id="Tool_Utils_MemorySegment_1_ToReadOnlySequence_System_Int32_System_Int32_"></a> ToReadOnlySequence\\(int, int\\)"'},"​")],-1)),t("p",null,[e[3]||(e[3]=s("创建可读的连续")),n(a,{href:"System.Buffers.ReadOnlySequence%601","data-throw-if-not-resolved":"false"}),e[4]||(e[4]=s("（顺序串联）"))]),e[9]||(e[9]=i('<div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ReadOnlySequence</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">ToReadOnlySequence</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> startIndex</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> endIndex</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-6" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-6" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>startIndex</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>开始位置</p><p><code>endIndex</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p>结尾位置</p><h4 id="returns-3" tabindex="-1">Returns <a class="header-anchor" href="#returns-3" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.buffers.readonlysequence-1" target="_blank" rel="noreferrer">ReadOnlySequence</a>&lt;T&gt;</p>',8)),t("p",null,[e[5]||(e[5]=s("返回")),n(a,{href:"System.Buffers.ReadOnlySequence%601","data-throw-if-not-resolved":"false"})]),e[10]||(e[10]=i('<h3 id="tostring" tabindex="-1"><a id="Tool_Utils_MemorySegment_1_ToString"></a> ToString() <a class="header-anchor" href="#tostring" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_MemorySegment_1_ToString&quot;&gt;&lt;/a&gt; ToString\\(\\)&quot;">​</a></h3><p>获取相关描述</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> override</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> string</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ToString</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h4 id="returns-4" tabindex="-1">Returns <a class="header-anchor" href="#returns-4" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.string" target="_blank" rel="noreferrer">string</a></p>',5))])}const E=o(p,[["render",y]]);export{b as __pageData,E as default};

import{_ as s,c as i,a2 as t,G as o,B as n,o as r}from"./chunks/framework.CQii86iU.js";const k=JSON.parse('{"title":"Class TaskQueue","description":"","frontmatter":{},"headers":[],"relativePath":"docfx/Tool.Utils.ThreadQueue.TaskQueue.md","filePath":"zh/docfx/Tool.Utils.ThreadQueue.TaskQueue.md"}'),l={name:"docfx/Tool.Utils.ThreadQueue.TaskQueue.md"};function h(p,e,c,_,d,y){const a=n("xref");return r(),i("div",null,[e[0]||(e[0]=t('<h1 id="class-taskqueue" tabindex="-1"><a id="Tool_Utils_ThreadQueue_TaskQueue"></a> Class TaskQueue <a class="header-anchor" href="#class-taskqueue" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_ThreadQueue_TaskQueue&quot;&gt;&lt;/a&gt; Class TaskQueue&quot;">​</a></h1><p>Namespace: <a href="./Tool.Utils.ThreadQueue.html">Tool.Utils.ThreadQueue</a><br> Assembly: Tool.Net.dll</p><p>一个消息队列任务模型（异步处理任务·线程安全）</p><table><tbody></tbody></table><table><tbody></tbody></table><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> class</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> TaskQueue</span></span></code></pre></div><h4 id="inheritance" tabindex="-1">Inheritance <a class="header-anchor" href="#inheritance" aria-label="Permalink to &quot;Inheritance&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a> ← <a href="./Tool.Utils.ThreadQueue.TaskQueue.html">TaskQueue</a></p><h4 id="inherited-members" tabindex="-1">Inherited Members <a class="header-anchor" href="#inherited-members" aria-label="Permalink to &quot;Inherited Members&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object)" target="_blank" rel="noreferrer">object.Equals(object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object-system-object)" target="_blank" rel="noreferrer">object.Equals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gethashcode" target="_blank" rel="noreferrer">object.GetHashCode()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gettype" target="_blank" rel="noreferrer">object.GetType()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone" target="_blank" rel="noreferrer">object.MemberwiseClone()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.referenceequals" target="_blank" rel="noreferrer">object.ReferenceEquals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.tostring" target="_blank" rel="noreferrer">object.ToString()</a></p><h4 id="extension-methods" tabindex="-1">Extension Methods <a class="header-anchor" href="#extension-methods" aria-label="Permalink to &quot;Extension Methods&quot;">​</a></h4><p><a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Add__1_System_Object_System_Object_System_Object_">ObjectExtension.Add&lt;T&gt;(object, object, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_CopyEntity_System_Object_System_Object_System_String___">ObjectExtension.CopyEntity(object, object, params string[])</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_">ObjectExtension.EntityToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_">ObjectExtension.EntityToJson(object, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_System_String_">ObjectExtension.EntityToJson(object, bool, string)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_GetDictionary_System_Object_">DictionaryExtension.GetDictionary(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, Type, string, out bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtr_System_Object_">ObjectExtension.GetIntPtr(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtrInt_System_Object_">ObjectExtension.GetIntPtrInt(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertieFind_System_Object_System_String_System_Boolean_">TypeExtension.GetPropertieFind(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetProperties_System_Object_">TypeExtension.GetProperties(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, Type, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_ComponentModel_PropertyDescriptor_">TypeExtension.GetValue(object, PropertyDescriptor)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_System_Boolean_">TypeExtension.GetValue(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_">TypeExtension.GetValue(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, object, int, int)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Int32_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, int, object, int, int)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_SetDictionary_System_Object_System_Collections_Generic_IDictionary_System_String_System_Object__">DictionaryExtension.SetDictionary(object, IDictionary&lt;string, object&gt;)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey__1_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetFieldKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey__1_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetPropertyKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_ComponentModel_PropertyDescriptor_System_Object_">TypeExtension.SetValue(object, PropertyDescriptor, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_System_Boolean_">TypeExtension.SetValue(object, string, object, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_">TypeExtension.SetValue(object, string, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBase64String_System_Object_">ObjectExtension.ToBase64String(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_">ObjectExtension.ToBytes(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_System_Type__">ObjectExtension.ToBytes(object, out Type)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary_System_Object_">DictionaryExtension.ToDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary__1_System_Object_">DictionaryExtension.ToDictionary&lt;T&gt;(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary_System_Object_">DictionaryExtension.ToIDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary__1_System_Object_">DictionaryExtension.ToIDictionary&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_">ObjectExtension.ToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_System_Text_Json_JsonSerializerOptions_">ObjectExtension.ToJson(object, JsonSerializerOptions)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_">ObjectExtension.ToJsonWeb(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_System_Action_System_Text_Json_JsonSerializerOptions__">ObjectExtension.ToJsonWeb(object, Action&lt;JsonSerializerOptions&gt;)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToTryVar__1_System_Object___0_">ObjectExtension.ToTryVar&lt;T&gt;(object, T)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar__1_System_Object_">ObjectExtension.ToVar&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_Type_System_Boolean_">ObjectExtension.ToVar(object, Type, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_String_">ObjectExtension.ToVar(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToXml_System_Object_">ObjectExtension.ToXml(object)</a></p><h2 id="remarks" tabindex="-1">Remarks <a class="header-anchor" href="#remarks" aria-label="Permalink to &quot;Remarks&quot;">​</a></h2><p>代码由逆血提供支持</p><h2 id="properties" tabindex="-1">Properties <a class="header-anchor" href="#properties" aria-label="Permalink to &quot;Properties&quot;">​</a></h2><h3 id="completecount" tabindex="-1"><a id="Tool_Utils_ThreadQueue_TaskQueue_CompleteCount"></a> CompleteCount <a class="header-anchor" href="#completecount" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_ThreadQueue_TaskQueue_CompleteCount&quot;&gt;&lt;/a&gt; CompleteCount&quot;">​</a></h3><p>累计已完成的任务数</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> ulong</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> CompleteCount { get; }</span></span></code></pre></div><h4 id="property-value" tabindex="-1">Property Value <a class="header-anchor" href="#property-value" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.uint64" target="_blank" rel="noreferrer">ulong</a></p><h3 id="count" tabindex="-1"><a id="Tool_Utils_ThreadQueue_TaskQueue_Count"></a> Count <a class="header-anchor" href="#count" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_ThreadQueue_TaskQueue_Count&quot;&gt;&lt;/a&gt; Count&quot;">​</a></h3><p>当前活动的任务数</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> int</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> Count { get; }</span></span></code></pre></div><h4 id="property-value-1" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-1" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><h3 id="totalcount" tabindex="-1"><a id="Tool_Utils_ThreadQueue_TaskQueue_TotalCount"></a> TotalCount <a class="header-anchor" href="#totalcount" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_ThreadQueue_TaskQueue_TotalCount&quot;&gt;&lt;/a&gt; TotalCount&quot;">​</a></h3><p>累计已有的任务数</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> ulong</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> TotalCount { get; }</span></span></code></pre></div><h4 id="property-value-2" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-2" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.uint64" target="_blank" rel="noreferrer">ulong</a></p><h2 id="methods" tabindex="-1">Methods <a class="header-anchor" href="#methods" aria-label="Permalink to &quot;Methods&quot;">​</a></h2><h3 id="enqueue-object-delegate-params-object" tabindex="-1"><a id="Tool_Utils_ThreadQueue_TaskQueue_Enqueue_System_Object_System_Delegate_System_Object___"></a> Enqueue(object, Delegate, params object[]) <a class="header-anchor" href="#enqueue-object-delegate-params-object" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_ThreadQueue_TaskQueue_Enqueue_System_Object_System_Delegate_System_Object___&quot;&gt;&lt;/a&gt; Enqueue\\(object, Delegate, params object\\[\\]\\)&quot;">​</a></h3><p>添加一个新的任务（他会排队一个一个完成）</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Task</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Enqueue</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">object</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> callClass</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Delegate</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> action</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">params</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> object</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">[] </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">args</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters" tabindex="-1">Parameters <a class="header-anchor" href="#parameters" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>callClass</code> <a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a></p><p>调用类信息</p><p><code>action</code> <a href="https://learn.microsoft.com/dotnet/api/system.delegate" target="_blank" rel="noreferrer">Delegate</a></p><p>任务</p><p><code>args</code> <a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a>[]</p><p>任务需要的参数</p><h4 id="returns" tabindex="-1">Returns <a class="header-anchor" href="#returns" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.task" target="_blank" rel="noreferrer">Task</a></p><h4 id="remarks-1" tabindex="-1">Remarks <a class="header-anchor" href="#remarks-1" aria-label="Permalink to &quot;Remarks&quot;">​</a></h4><p>可等待的结果</p><h3 id="enqueue-t-object-delegate-params-object" tabindex="-1"><a id="Tool_Utils_ThreadQueue_TaskQueue_Enqueue__1_System_Object_System_Delegate_System_Object___"></a> Enqueue&lt;T&gt;(object, Delegate, params object[]) <a class="header-anchor" href="#enqueue-t-object-delegate-params-object" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_ThreadQueue_TaskQueue_Enqueue__1_System_Object_System_Delegate_System_Object___&quot;&gt;&lt;/a&gt; Enqueue&lt;T\\&gt;\\(object, Delegate, params object\\[\\]\\)&quot;">​</a></h3><p>添加一个新的任务（他会排队一个一个完成）</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Task</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Enqueue</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt;(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">object</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> callClass</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Delegate</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> func</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">params</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> object</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">[] </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">args</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-1" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-1" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>callClass</code> <a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a></p><p>调用类信息</p><p><code>func</code> <a href="https://learn.microsoft.com/dotnet/api/system.delegate" target="_blank" rel="noreferrer">Delegate</a></p><p>任务</p><p><code>args</code> <a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a>[]</p><p>任务需要的参数</p><h4 id="returns-1" tabindex="-1">Returns <a class="header-anchor" href="#returns-1" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1" target="_blank" rel="noreferrer">Task</a>&lt;T&gt;</p><h4 id="type-parameters" tabindex="-1">Type Parameters <a class="header-anchor" href="#type-parameters" aria-label="Permalink to &quot;Type Parameters&quot;">​</a></h4><p><code>T</code></p><h4 id="remarks-2" tabindex="-1">Remarks <a class="header-anchor" href="#remarks-2" aria-label="Permalink to &quot;Remarks&quot;">​</a></h4><p>可等待的结果</p><h3 id="getoradd-delegate" tabindex="-1"><a id="Tool_Utils_ThreadQueue_TaskQueue_GetOrAdd_System_Delegate_"></a> GetOrAdd(Delegate) <a class="header-anchor" href="#getoradd-delegate" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_ThreadQueue_TaskQueue_GetOrAdd_System_Delegate_&quot;&gt;&lt;/a&gt; GetOrAdd\\(Delegate\\)&quot;">​</a></h3><p>获取一个可以动态传递的委托（可复用保证线程安全）</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> IActionDispatcher</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> GetOrAdd</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Delegate</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> func</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-2" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-2" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>func</code> <a href="https://learn.microsoft.com/dotnet/api/system.delegate" target="_blank" rel="noreferrer">Delegate</a></p><p>任务需要的参数</p><h4 id="returns-2" tabindex="-1">Returns <a class="header-anchor" href="#returns-2" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="./Tool.Utils.ActionDelegate.IActionDispatcher.html">IActionDispatcher</a></p><h4 id="remarks-3" tabindex="-1">Remarks <a class="header-anchor" href="#remarks-3" aria-label="Permalink to &quot;Remarks&quot;">​</a></h4>',70)),o(a,{href:"Tool.Utils.ActionDelegate.IActionDispatcher","data-throw-if-not-resolved":"false"}),e[1]||(e[1]=t('<h3 id="staticenqueue-delegate-params-object" tabindex="-1"><a id="Tool_Utils_ThreadQueue_TaskQueue_StaticEnqueue_System_Delegate_System_Object___"></a> StaticEnqueue(Delegate, params object[]) <a class="header-anchor" href="#staticenqueue-delegate-params-object" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_ThreadQueue_TaskQueue_StaticEnqueue_System_Delegate_System_Object___&quot;&gt;&lt;/a&gt; StaticEnqueue\\(Delegate, params object\\[\\]\\)&quot;">​</a></h3><p>添加一个新的任务（他会排队一个一个完成）</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Task</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> StaticEnqueue</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Delegate</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> action</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">params</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> object</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">[] </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">args</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-3" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-3" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>action</code> <a href="https://learn.microsoft.com/dotnet/api/system.delegate" target="_blank" rel="noreferrer">Delegate</a></p><p>任务</p><p><code>args</code> <a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a>[]</p><p>任务需要的参数</p><h4 id="returns-3" tabindex="-1">Returns <a class="header-anchor" href="#returns-3" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.task" target="_blank" rel="noreferrer">Task</a></p><h4 id="remarks-4" tabindex="-1">Remarks <a class="header-anchor" href="#remarks-4" aria-label="Permalink to &quot;Remarks&quot;">​</a></h4><p>可等待的结果</p><h3 id="staticenqueue-t-delegate-params-object" tabindex="-1"><a id="Tool_Utils_ThreadQueue_TaskQueue_StaticEnqueue__1_System_Delegate_System_Object___"></a> StaticEnqueue&lt;T&gt;(Delegate, params object[]) <a class="header-anchor" href="#staticenqueue-t-delegate-params-object" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_ThreadQueue_TaskQueue_StaticEnqueue__1_System_Delegate_System_Object___&quot;&gt;&lt;/a&gt; StaticEnqueue&lt;T\\&gt;\\(Delegate, params object\\[\\]\\)&quot;">​</a></h3><p>添加一个新的任务（他会排队一个一个完成）</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Task</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">StaticEnqueue</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt;(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">Delegate</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> func</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">params</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> object</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">[] </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">args</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-4" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-4" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>func</code> <a href="https://learn.microsoft.com/dotnet/api/system.delegate" target="_blank" rel="noreferrer">Delegate</a></p><p>任务</p><p><code>args</code> <a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a>[]</p><p>任务需要的参数</p><h4 id="returns-4" tabindex="-1">Returns <a class="header-anchor" href="#returns-4" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1" target="_blank" rel="noreferrer">Task</a>&lt;T&gt;</p><h4 id="type-parameters-1" tabindex="-1">Type Parameters <a class="header-anchor" href="#type-parameters-1" aria-label="Permalink to &quot;Type Parameters&quot;">​</a></h4><p><code>T</code></p><h4 id="remarks-5" tabindex="-1">Remarks <a class="header-anchor" href="#remarks-5" aria-label="Permalink to &quot;Remarks&quot;">​</a></h4><p>可等待的结果</p>',26))])}const m=s(l,[["render",h]]);export{k as __pageData,m as default};
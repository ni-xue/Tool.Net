import{_ as s,c as o,a2 as a,G as i,B as n,o as r}from"./chunks/framework.CQii86iU.js";const b=JSON.parse('{"title":"Class TaskWithTimeout<T>","description":"","frontmatter":{},"headers":[],"relativePath":"docfx/Tool.Utils.TaskHelper.TaskWithTimeout-1.md","filePath":"zh/docfx/Tool.Utils.TaskHelper.TaskWithTimeout-1.md"}'),l={name:"docfx/Tool.Utils.TaskHelper.TaskWithTimeout-1.md"};function c(p,t,_,h,m,T){const e=n("xref");return r(),o("div",null,[t[0]||(t[0]=a('<h1 id="class-taskwithtimeout-t" tabindex="-1"><a id="Tool_Utils_TaskHelper_TaskWithTimeout_1"></a> Class TaskWithTimeout&lt;T&gt; <a class="header-anchor" href="#class-taskwithtimeout-t" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_TaskHelper_TaskWithTimeout_1&quot;&gt;&lt;/a&gt; Class TaskWithTimeout&lt;T\\&gt;&quot;">​</a></h1><p>Namespace: <a href="./Tool.Utils.TaskHelper.html">Tool.Utils.TaskHelper</a><br> Assembly: Tool.Net.dll</p><p>允许自定义触发异步任务</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> class</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> TaskWithTimeout</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; : </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">TaskCompletionSource</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt;, </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">IDisposable</span></span></code></pre></div><h4 id="type-parameters" tabindex="-1">Type Parameters <a class="header-anchor" href="#type-parameters" aria-label="Permalink to &quot;Type Parameters&quot;">​</a></h4><p><code>T</code></p><p>返回值</p><h4 id="inheritance" tabindex="-1">Inheritance <a class="header-anchor" href="#inheritance" aria-label="Permalink to &quot;Inheritance&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a> ← <a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcompletionsource-1" target="_blank" rel="noreferrer">TaskCompletionSource&lt;T&gt;</a> ← <a href="./Tool.Utils.TaskHelper.TaskWithTimeout-1.html">TaskWithTimeout&lt;T&gt;</a></p><h4 id="implements" tabindex="-1">Implements <a class="header-anchor" href="#implements" aria-label="Permalink to &quot;Implements&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.idisposable" target="_blank" rel="noreferrer">IDisposable</a></p><h4 id="inherited-members" tabindex="-1">Inherited Members <a class="header-anchor" href="#inherited-members" aria-label="Permalink to &quot;Inherited Members&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcompletionsource-1.setcanceled#system-threading-tasks-taskcompletionsource-1-setcanceled" target="_blank" rel="noreferrer">TaskCompletionSource&lt;T&gt;.SetCanceled()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcompletionsource-1.setcanceled#system-threading-tasks-taskcompletionsource-1-setcanceled(system-threading-cancellationtoken)" target="_blank" rel="noreferrer">TaskCompletionSource&lt;T&gt;.SetCanceled(CancellationToken)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcompletionsource-1.setexception#system-threading-tasks-taskcompletionsource-1-setexception(system-collections-generic-ienumerable((system-exception)))" target="_blank" rel="noreferrer">TaskCompletionSource&lt;T&gt;.SetException(IEnumerable&lt;Exception&gt;)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcompletionsource-1.setexception#system-threading-tasks-taskcompletionsource-1-setexception(system-exception)" target="_blank" rel="noreferrer">TaskCompletionSource&lt;T&gt;.SetException(Exception)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcompletionsource-1.setresult" target="_blank" rel="noreferrer">TaskCompletionSource&lt;T&gt;.SetResult(T)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcompletionsource-1.trysetcanceled#system-threading-tasks-taskcompletionsource-1-trysetcanceled" target="_blank" rel="noreferrer">TaskCompletionSource&lt;T&gt;.TrySetCanceled()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcompletionsource-1.trysetcanceled#system-threading-tasks-taskcompletionsource-1-trysetcanceled(system-threading-cancellationtoken)" target="_blank" rel="noreferrer">TaskCompletionSource&lt;T&gt;.TrySetCanceled(CancellationToken)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcompletionsource-1.trysetexception#system-threading-tasks-taskcompletionsource-1-trysetexception(system-collections-generic-ienumerable((system-exception)))" target="_blank" rel="noreferrer">TaskCompletionSource&lt;T&gt;.TrySetException(IEnumerable&lt;Exception&gt;)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcompletionsource-1.trysetexception#system-threading-tasks-taskcompletionsource-1-trysetexception(system-exception)" target="_blank" rel="noreferrer">TaskCompletionSource&lt;T&gt;.TrySetException(Exception)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcompletionsource-1.trysetresult" target="_blank" rel="noreferrer">TaskCompletionSource&lt;T&gt;.TrySetResult(T)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcompletionsource-1.task" target="_blank" rel="noreferrer">TaskCompletionSource&lt;T&gt;.Task</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object)" target="_blank" rel="noreferrer">object.Equals(object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object-system-object)" target="_blank" rel="noreferrer">object.Equals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gethashcode" target="_blank" rel="noreferrer">object.GetHashCode()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gettype" target="_blank" rel="noreferrer">object.GetType()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone" target="_blank" rel="noreferrer">object.MemberwiseClone()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.referenceequals" target="_blank" rel="noreferrer">object.ReferenceEquals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.tostring" target="_blank" rel="noreferrer">object.ToString()</a></p><h4 id="extension-methods" tabindex="-1">Extension Methods <a class="header-anchor" href="#extension-methods" aria-label="Permalink to &quot;Extension Methods&quot;">​</a></h4><p><a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Add__1_System_Object_System_Object_System_Object_">ObjectExtension.Add&lt;T&gt;(object, object, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_CopyEntity_System_Object_System_Object_System_String___">ObjectExtension.CopyEntity(object, object, params string[])</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_">ObjectExtension.EntityToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_">ObjectExtension.EntityToJson(object, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_System_String_">ObjectExtension.EntityToJson(object, bool, string)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_GetDictionary_System_Object_">DictionaryExtension.GetDictionary(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, Type, string, out bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtr_System_Object_">ObjectExtension.GetIntPtr(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtrInt_System_Object_">ObjectExtension.GetIntPtrInt(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertieFind_System_Object_System_String_System_Boolean_">TypeExtension.GetPropertieFind(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetProperties_System_Object_">TypeExtension.GetProperties(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, Type, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_ComponentModel_PropertyDescriptor_">TypeExtension.GetValue(object, PropertyDescriptor)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_System_Boolean_">TypeExtension.GetValue(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_">TypeExtension.GetValue(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, object, int, int)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Int32_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, int, object, int, int)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_SetDictionary_System_Object_System_Collections_Generic_IDictionary_System_String_System_Object__">DictionaryExtension.SetDictionary(object, IDictionary&lt;string, object&gt;)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey__1_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetFieldKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey__1_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetPropertyKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_ComponentModel_PropertyDescriptor_System_Object_">TypeExtension.SetValue(object, PropertyDescriptor, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_System_Boolean_">TypeExtension.SetValue(object, string, object, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_">TypeExtension.SetValue(object, string, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBase64String_System_Object_">ObjectExtension.ToBase64String(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_">ObjectExtension.ToBytes(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_System_Type__">ObjectExtension.ToBytes(object, out Type)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary_System_Object_">DictionaryExtension.ToDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary__1_System_Object_">DictionaryExtension.ToDictionary&lt;T&gt;(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary_System_Object_">DictionaryExtension.ToIDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary__1_System_Object_">DictionaryExtension.ToIDictionary&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_">ObjectExtension.ToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_System_Text_Json_JsonSerializerOptions_">ObjectExtension.ToJson(object, JsonSerializerOptions)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_">ObjectExtension.ToJsonWeb(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_System_Action_System_Text_Json_JsonSerializerOptions__">ObjectExtension.ToJsonWeb(object, Action&lt;JsonSerializerOptions&gt;)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToTryVar__1_System_Object___0_">ObjectExtension.ToTryVar&lt;T&gt;(object, T)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar__1_System_Object_">ObjectExtension.ToVar&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_Type_System_Boolean_">ObjectExtension.ToVar(object, Type, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_String_">ObjectExtension.ToVar(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToXml_System_Object_">ObjectExtension.ToXml(object)</a></p><h2 id="remarks" tabindex="-1">Remarks <a class="header-anchor" href="#remarks" aria-label="Permalink to &quot;Remarks&quot;">​</a></h2><p>代码由逆血提供支持</p><h2 id="constructors" tabindex="-1">Constructors <a class="header-anchor" href="#constructors" aria-label="Permalink to &quot;Constructors&quot;">​</a></h2><h3 id="taskwithtimeout-timespan" tabindex="-1"><a id="Tool_Utils_TaskHelper_TaskWithTimeout_1__ctor_System_TimeSpan_"></a> TaskWithTimeout(TimeSpan) <a class="header-anchor" href="#taskwithtimeout-timespan" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_TaskHelper_TaskWithTimeout_1__ctor_System_TimeSpan_&quot;&gt;&lt;/a&gt; TaskWithTimeout\\(TimeSpan\\)&quot;">​</a></h3><p>初始化</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> TaskWithTimeout</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(TimeSpan timeout)</span></span></code></pre></div><h4 id="parameters" tabindex="-1">Parameters <a class="header-anchor" href="#parameters" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>timeout</code> <a href="https://learn.microsoft.com/dotnet/api/system.timespan" target="_blank" rel="noreferrer">TimeSpan</a></p><p>超时时间</p><h3 id="taskwithtimeout-timespan-object" tabindex="-1"><a id="Tool_Utils_TaskHelper_TaskWithTimeout_1__ctor_System_TimeSpan_System_Object_"></a> TaskWithTimeout(TimeSpan, object) <a class="header-anchor" href="#taskwithtimeout-timespan-object" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_TaskHelper_TaskWithTimeout_1__ctor_System_TimeSpan_System_Object_&quot;&gt;&lt;/a&gt; TaskWithTimeout\\(TimeSpan, object\\)&quot;">​</a></h3><p>初始化</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> TaskWithTimeout</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(TimeSpan timeout, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">object</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> state)</span></span></code></pre></div><h4 id="parameters-1" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-1" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>timeout</code> <a href="https://learn.microsoft.com/dotnet/api/system.timespan" target="_blank" rel="noreferrer">TimeSpan</a></p><p>超时时间</p><p><code>state</code> <a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a></p><p>带传递值</p><h3 id="taskwithtimeout-timespan-taskcreationoptions" tabindex="-1"><a id="Tool_Utils_TaskHelper_TaskWithTimeout_1__ctor_System_TimeSpan_System_Threading_Tasks_TaskCreationOptions_"></a> TaskWithTimeout(TimeSpan, TaskCreationOptions) <a class="header-anchor" href="#taskwithtimeout-timespan-taskcreationoptions" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_TaskHelper_TaskWithTimeout_1__ctor_System_TimeSpan_System_Threading_Tasks_TaskCreationOptions_&quot;&gt;&lt;/a&gt; TaskWithTimeout\\(TimeSpan, TaskCreationOptions\\)&quot;">​</a></h3><p>初始化</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> TaskWithTimeout</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(TimeSpan timeout, TaskCreationOptions creationOptions)</span></span></code></pre></div><h4 id="parameters-2" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-2" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>timeout</code> <a href="https://learn.microsoft.com/dotnet/api/system.timespan" target="_blank" rel="noreferrer">TimeSpan</a></p><p>超时时间</p><p><code>creationOptions</code> <a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcreationoptions" target="_blank" rel="noreferrer">TaskCreationOptions</a></p><p>任务枚举类型</p><h3 id="taskwithtimeout-timespan-object-taskcreationoptions" tabindex="-1"><a id="Tool_Utils_TaskHelper_TaskWithTimeout_1__ctor_System_TimeSpan_System_Object_System_Threading_Tasks_TaskCreationOptions_"></a> TaskWithTimeout(TimeSpan, object, TaskCreationOptions) <a class="header-anchor" href="#taskwithtimeout-timespan-object-taskcreationoptions" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_TaskHelper_TaskWithTimeout_1__ctor_System_TimeSpan_System_Object_System_Threading_Tasks_TaskCreationOptions_&quot;&gt;&lt;/a&gt; TaskWithTimeout\\(TimeSpan, object, TaskCreationOptions\\)&quot;">​</a></h3><p>初始化</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> TaskWithTimeout</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(TimeSpan timeout, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">object</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> state, TaskCreationOptions creationOptions)</span></span></code></pre></div><h4 id="parameters-3" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-3" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>timeout</code> <a href="https://learn.microsoft.com/dotnet/api/system.timespan" target="_blank" rel="noreferrer">TimeSpan</a></p><p>超时时间</p><p><code>state</code> <a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a></p><p>带传递值</p><p><code>creationOptions</code> <a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcreationoptions" target="_blank" rel="noreferrer">TaskCreationOptions</a></p><p>任务枚举类型</p><h2 id="properties" tabindex="-1">Properties <a class="header-anchor" href="#properties" aria-label="Permalink to &quot;Properties&quot;">​</a></h2><h3 id="iscancellationrequested" tabindex="-1"><a id="Tool_Utils_TaskHelper_TaskWithTimeout_1_IsCancellationRequested"></a> IsCancellationRequested <a class="header-anchor" href="#iscancellationrequested" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_TaskHelper_TaskWithTimeout_1_IsCancellationRequested&quot;&gt;&lt;/a&gt; IsCancellationRequested&quot;">​</a></h3><p>是否被取消</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> bool</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> IsCancellationRequested { get; }</span></span></code></pre></div><h4 id="property-value" tabindex="-1">Property Value <a class="header-anchor" href="#property-value" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.boolean" target="_blank" rel="noreferrer">bool</a></p><h3 id="timeout" tabindex="-1"><a id="Tool_Utils_TaskHelper_TaskWithTimeout_1_Timeout"></a> Timeout <a class="header-anchor" href="#timeout" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_TaskHelper_TaskWithTimeout_1_Timeout&quot;&gt;&lt;/a&gt; Timeout&quot;">​</a></h3><p>超时时间</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> TimeSpan Timeout { get; }</span></span></code></pre></div><h4 id="property-value-1" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-1" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.timespan" target="_blank" rel="noreferrer">TimeSpan</a></p><h2 id="methods" tabindex="-1">Methods <a class="header-anchor" href="#methods" aria-label="Permalink to &quot;Methods&quot;">​</a></h2><h3 id="dispose" tabindex="-1"><a id="Tool_Utils_TaskHelper_TaskWithTimeout_1_Dispose"></a> Dispose() <a class="header-anchor" href="#dispose" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_TaskHelper_TaskWithTimeout_1_Dispose&quot;&gt;&lt;/a&gt; Dispose\\(\\)&quot;">​</a></h3><p>回收资源</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Dispose</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h3 id="getawaiter" tabindex="-1"><a id="Tool_Utils_TaskHelper_TaskWithTimeout_1_GetAwaiter"></a> GetAwaiter() <a class="header-anchor" href="#getawaiter" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_TaskHelper_TaskWithTimeout_1_GetAwaiter&quot;&gt;&lt;/a&gt; GetAwaiter\\(\\)&quot;">​</a></h3><p>返回任务调度器</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> TaskAwaiter</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&lt;</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">T</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">&gt; </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">GetAwaiter</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h4 id="returns" tabindex="-1">Returns <a class="header-anchor" href="#returns" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.runtime.compilerservices.taskawaiter-1" target="_blank" rel="noreferrer">TaskAwaiter</a>&lt;T&gt;</p>',70)),i(e,{href:"System.Runtime.CompilerServices.TaskAwaiter%601","data-throw-if-not-resolved":"false"})])}const k=s(l,[["render",c]]);export{b as __pageData,k as default};
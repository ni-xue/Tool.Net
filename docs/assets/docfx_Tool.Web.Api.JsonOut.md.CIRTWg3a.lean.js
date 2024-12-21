import{_ as i,c as a,j as e,a as o,G as n,a2 as r,B as l,o as _}from"./chunks/framework.CQii86iU.js";const d=JSON.parse('{"title":"Class JsonOut","description":"","frontmatter":{},"headers":[],"relativePath":"docfx/Tool.Web.Api.JsonOut.md","filePath":"zh/docfx/Tool.Web.Api.JsonOut.md"}'),p={name:"docfx/Tool.Web.Api.JsonOut.md"};function c(h,t,b,y,m,u){const s=l("xref");return _(),a("div",null,[t[2]||(t[2]=e("h1",{id:"class-jsonout",tabindex:"-1"},[e("a",{id:"Tool_Web_Api_JsonOut"}),o(" Class JsonOut "),e("a",{class:"header-anchor",href:"#class-jsonout","aria-label":'Permalink to "<a id="Tool_Web_Api_JsonOut"></a> Class JsonOut"'},"​")],-1)),t[3]||(t[3]=e("p",null,[o("Namespace: "),e("a",{href:"./Tool.Web.Api.html"},"Tool.Web.Api"),e("br"),o(" Assembly: Tool.Net.dll")],-1)),e("p",null,[t[0]||(t[0]=o("系统默认 ")),n(s,{href:"Tool.Web.Api.ApiOut","data-throw-if-not-resolved":"false"}),t[1]||(t[1]=o(" 输出对象的实现类，JSON格式处理"))]),t[4]||(t[4]=r('<div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> class</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> JsonOut</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> : </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">ApiOut</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">IApiOut</span></span></code></pre></div><h4 id="inheritance" tabindex="-1">Inheritance <a class="header-anchor" href="#inheritance" aria-label="Permalink to &quot;Inheritance&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a> ← <a href="./Tool.Web.Api.ApiOut.html">ApiOut</a> ← <a href="./Tool.Web.Api.JsonOut.html">JsonOut</a></p><h4 id="implements" tabindex="-1">Implements <a class="header-anchor" href="#implements" aria-label="Permalink to &quot;Implements&quot;">​</a></h4><p><a href="./Tool.Web.Api.IApiOut.html">IApiOut</a></p><h4 id="inherited-members" tabindex="-1">Inherited Members <a class="header-anchor" href="#inherited-members" aria-label="Permalink to &quot;Inherited Members&quot;">​</a></h4><p><a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_ContentType">ApiOut.ContentType</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_StatusCode">ApiOut.StatusCode</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_ExecuteOutAsync_Tool_Web_Routing_AshxRouteData_">ApiOut.ExecuteOutAsync(AshxRouteData)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_Json_System_Object_">ApiOut.Json(object)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_JsonAsync_System_Object_">ApiOut.JsonAsync(object)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_Json_System_Object_System_Text_Json_JsonSerializerOptions_">ApiOut.Json(object, JsonSerializerOptions)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_JsonAsync_System_Object_System_Text_Json_JsonSerializerOptions_">ApiOut.JsonAsync(object, JsonSerializerOptions)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_Write_System_Object_">ApiOut.Write(object)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_WriteAsync_System_Object_">ApiOut.WriteAsync(object)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_View">ApiOut.View()</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_View_System_String_">ApiOut.View(string)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_PathView_System_String_">ApiOut.PathView(string)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_ViewAsync">ApiOut.ViewAsync()</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_ViewAsync_System_String_">ApiOut.ViewAsync(string)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_PathViewAsync_System_String_">ApiOut.PathViewAsync(string)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_File_System_String_System_IO_Stream_">ApiOut.File(string, Stream)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_FileAsync_System_String_System_IO_Stream_">ApiOut.FileAsync(string, Stream)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_File_System_String_System_Byte___">ApiOut.File(string, byte[])</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_FileAsync_System_String_System_Byte___">ApiOut.FileAsync(string, byte[])</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_Redirect_System_String_">ApiOut.Redirect(string)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_RedirectAsync_System_String_">ApiOut.RedirectAsync(string)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_NoContent">ApiOut.NoContent()</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_NoContentAsync">ApiOut.NoContentAsync()</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_EventStream_System_Func_Tool_Web_EventStream_System_Threading_Tasks_Task__System_Int32_">ApiOut.EventStream(Func&lt;EventStream, Task&gt;, int)</a>, <a href="./Tool.Web.Api.ApiOut.html#Tool_Web_Api_ApiOut_EventStreamAsync_System_Func_Tool_Web_EventStream_System_Threading_Tasks_Task__System_Int32_">ApiOut.EventStreamAsync(Func&lt;EventStream, Task&gt;, int)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object)" target="_blank" rel="noreferrer">object.Equals(object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object-system-object)" target="_blank" rel="noreferrer">object.Equals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gethashcode" target="_blank" rel="noreferrer">object.GetHashCode()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gettype" target="_blank" rel="noreferrer">object.GetType()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone" target="_blank" rel="noreferrer">object.MemberwiseClone()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.referenceequals" target="_blank" rel="noreferrer">object.ReferenceEquals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.tostring" target="_blank" rel="noreferrer">object.ToString()</a></p><h4 id="extension-methods" tabindex="-1">Extension Methods <a class="header-anchor" href="#extension-methods" aria-label="Permalink to &quot;Extension Methods&quot;">​</a></h4><p><a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Add__1_System_Object_System_Object_System_Object_">ObjectExtension.Add&lt;T&gt;(object, object, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_CopyEntity_System_Object_System_Object_System_String___">ObjectExtension.CopyEntity(object, object, params string[])</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_">ObjectExtension.EntityToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_">ObjectExtension.EntityToJson(object, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_System_String_">ObjectExtension.EntityToJson(object, bool, string)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_GetDictionary_System_Object_">DictionaryExtension.GetDictionary(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, Type, string, out bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtr_System_Object_">ObjectExtension.GetIntPtr(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtrInt_System_Object_">ObjectExtension.GetIntPtrInt(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertieFind_System_Object_System_String_System_Boolean_">TypeExtension.GetPropertieFind(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetProperties_System_Object_">TypeExtension.GetProperties(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, Type, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_ComponentModel_PropertyDescriptor_">TypeExtension.GetValue(object, PropertyDescriptor)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_System_Boolean_">TypeExtension.GetValue(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_">TypeExtension.GetValue(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, object, int, int)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Int32_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, int, object, int, int)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_SetDictionary_System_Object_System_Collections_Generic_IDictionary_System_String_System_Object__">DictionaryExtension.SetDictionary(object, IDictionary&lt;string, object&gt;)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey__1_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetFieldKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey__1_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetPropertyKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_ComponentModel_PropertyDescriptor_System_Object_">TypeExtension.SetValue(object, PropertyDescriptor, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_System_Boolean_">TypeExtension.SetValue(object, string, object, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_">TypeExtension.SetValue(object, string, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBase64String_System_Object_">ObjectExtension.ToBase64String(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_">ObjectExtension.ToBytes(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_System_Type__">ObjectExtension.ToBytes(object, out Type)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary_System_Object_">DictionaryExtension.ToDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary__1_System_Object_">DictionaryExtension.ToDictionary&lt;T&gt;(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary_System_Object_">DictionaryExtension.ToIDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary__1_System_Object_">DictionaryExtension.ToIDictionary&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_">ObjectExtension.ToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_System_Text_Json_JsonSerializerOptions_">ObjectExtension.ToJson(object, JsonSerializerOptions)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_">ObjectExtension.ToJsonWeb(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_System_Action_System_Text_Json_JsonSerializerOptions__">ObjectExtension.ToJsonWeb(object, Action&lt;JsonSerializerOptions&gt;)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToTryVar__1_System_Object___0_">ObjectExtension.ToTryVar&lt;T&gt;(object, T)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar__1_System_Object_">ObjectExtension.ToVar&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_Type_System_Boolean_">ObjectExtension.ToVar(object, Type, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_String_">ObjectExtension.ToVar(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToXml_System_Object_">ObjectExtension.ToXml(object)</a></p><h2 id="remarks" tabindex="-1">Remarks <a class="header-anchor" href="#remarks" aria-label="Permalink to &quot;Remarks&quot;">​</a></h2><p>代码由逆血提供支持</p><h2 id="constructors" tabindex="-1">Constructors <a class="header-anchor" href="#constructors" aria-label="Permalink to &quot;Constructors&quot;">​</a></h2><h3 id="jsonout-object" tabindex="-1"><a id="Tool_Web_Api_JsonOut__ctor_System_Object_"></a> JsonOut(object) <a class="header-anchor" href="#jsonout-object" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Web_Api_JsonOut__ctor_System_Object_&quot;&gt;&lt;/a&gt; JsonOut\\(object\\)&quot;">​</a></h3><p>创建Json输出对象</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> JsonOut</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">object</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> data)</span></span></code></pre></div><h4 id="parameters" tabindex="-1">Parameters <a class="header-anchor" href="#parameters" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>data</code> <a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a></p><p>可被序列化的数据源</p><h3 id="jsonout-object-jsonserializeroptions" tabindex="-1"><a id="Tool_Web_Api_JsonOut__ctor_System_Object_System_Text_Json_JsonSerializerOptions_"></a> JsonOut(object, JsonSerializerOptions) <a class="header-anchor" href="#jsonout-object-jsonserializeroptions" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Web_Api_JsonOut__ctor_System_Object_System_Text_Json_JsonSerializerOptions_&quot;&gt;&lt;/a&gt; JsonOut\\(object, JsonSerializerOptions\\)&quot;">​</a></h3><p>创建Json输出对象</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> JsonOut</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">object</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> data, JsonSerializerOptions jsonOptions)</span></span></code></pre></div><h4 id="parameters-1" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-1" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>data</code> <a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a></p><p>可被序列化的数据源</p><p><code>jsonOptions</code> <a href="https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions" target="_blank" rel="noreferrer">JsonSerializerOptions</a></p><p>Json 转换条件</p><h2 id="properties" tabindex="-1">Properties <a class="header-anchor" href="#properties" aria-label="Permalink to &quot;Properties&quot;">​</a></h2><h3 id="contenttype" tabindex="-1"><a id="Tool_Web_Api_JsonOut_ContentType"></a> ContentType <a class="header-anchor" href="#contenttype" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Web_Api_JsonOut_ContentType&quot;&gt;&lt;/a&gt; ContentType&quot;">​</a></h3><p>输出类型</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> override</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> string</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> ContentType { get; set; }</span></span></code></pre></div><h4 id="property-value" tabindex="-1">Property Value <a class="header-anchor" href="#property-value" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.string" target="_blank" rel="noreferrer">string</a></p><h3 id="data" tabindex="-1"><a id="Tool_Web_Api_JsonOut_Data"></a> Data <a class="header-anchor" href="#data" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Web_Api_JsonOut_Data&quot;&gt;&lt;/a&gt; Data&quot;">​</a></h3><p>输出结果数据</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> object</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> Data { get; set; }</span></span></code></pre></div><h4 id="property-value-1" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-1" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a></p><h3 id="jsonoptions" tabindex="-1"><a id="Tool_Web_Api_JsonOut_JsonOptions"></a> JsonOptions <a class="header-anchor" href="#jsonoptions" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Web_Api_JsonOut_JsonOptions&quot;&gt;&lt;/a&gt; JsonOptions&quot;">​</a></h3><p>Json 转换条件</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> JsonSerializerOptions JsonOptions { get; set; }</span></span></code></pre></div><h4 id="property-value-2" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-2" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions" target="_blank" rel="noreferrer">JsonSerializerOptions</a></p><h3 id="statuscode" tabindex="-1"><a id="Tool_Web_Api_JsonOut_StatusCode"></a> StatusCode <a class="header-anchor" href="#statuscode" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Web_Api_JsonOut_StatusCode&quot;&gt;&lt;/a&gt; StatusCode&quot;">​</a></h3><p>HTTP 返回 Code</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> override</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> int</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> StatusCode { get; set; }</span></span></code></pre></div><h4 id="property-value-3" tabindex="-1">Property Value <a class="header-anchor" href="#property-value-3" aria-label="Permalink to &quot;Property Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><h2 id="methods" tabindex="-1">Methods <a class="header-anchor" href="#methods" aria-label="Permalink to &quot;Methods&quot;">​</a></h2><h3 id="executeoutasync-ashxroutedata" tabindex="-1"><a id="Tool_Web_Api_JsonOut_ExecuteOutAsync_Tool_Web_Routing_AshxRouteData_"></a> ExecuteOutAsync(AshxRouteData) <a class="header-anchor" href="#executeoutasync-ashxroutedata" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Web_Api_JsonOut_ExecuteOutAsync_Tool_Web_Routing_AshxRouteData_&quot;&gt;&lt;/a&gt; ExecuteOutAsync\\(AshxRouteData\\)&quot;">​</a></h3><p>实现JSON格式的输出</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> override</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Task</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ExecuteOutAsync</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">AshxRouteData</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> ashxRoute</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-2" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-2" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>ashxRoute</code> <a href="./Tool.Web.Routing.AshxRouteData.html">AshxRouteData</a></p><p>当前请求对象</p><h4 id="returns" tabindex="-1">Returns <a class="header-anchor" href="#returns" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.threading.tasks.task" target="_blank" rel="noreferrer">Task</a></p>',56))])}const O=i(p,[["render",c]]);export{d as __pageData,O as default};

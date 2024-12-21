import{_ as e,c as s,a2 as a,o as i}from"./chunks/framework.CQii86iU.js";const _=JSON.parse('{"title":"Class Crc32","description":"","frontmatter":{},"headers":[],"relativePath":"docfx/Tool.Utils.Crc32.md","filePath":"zh/docfx/Tool.Utils.Crc32.md"}'),o={name:"docfx/Tool.Utils.Crc32.md"};function r(n,t,l,h,c,p){return i(),s("div",null,t[0]||(t[0]=[a('<h1 id="class-crc32" tabindex="-1"><a id="Tool_Utils_Crc32"></a> Class Crc32 <a class="header-anchor" href="#class-crc32" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_Crc32&quot;&gt;&lt;/a&gt; Class Crc32&quot;">​</a></h1><p>Namespace: <a href="./Tool.Utils.html">Tool.Utils</a><br> Assembly: Tool.Net.dll</p><p>提供 CRC32 算法的实现</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> class</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Crc32</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> : </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">HashAlgorithm</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">ICryptoTransform</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">IDisposable</span></span></code></pre></div><h4 id="inheritance" tabindex="-1">Inheritance <a class="header-anchor" href="#inheritance" aria-label="Permalink to &quot;Inheritance&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.object" target="_blank" rel="noreferrer">object</a> ← <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm" target="_blank" rel="noreferrer">HashAlgorithm</a> ← <a href="./Tool.Utils.Crc32.html">Crc32</a></p><h4 id="implements" tabindex="-1">Implements <a class="header-anchor" href="#implements" aria-label="Permalink to &quot;Implements&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.icryptotransform" target="_blank" rel="noreferrer">ICryptoTransform</a>, <a href="https://learn.microsoft.com/dotnet/api/system.idisposable" target="_blank" rel="noreferrer">IDisposable</a></p><h4 id="inherited-members" tabindex="-1">Inherited Members <a class="header-anchor" href="#inherited-members" aria-label="Permalink to &quot;Inherited Members&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.hashsizevalue" target="_blank" rel="noreferrer">HashAlgorithm.HashSizeValue</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.hashvalue" target="_blank" rel="noreferrer">HashAlgorithm.HashValue</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.state" target="_blank" rel="noreferrer">HashAlgorithm.State</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.clear" target="_blank" rel="noreferrer">HashAlgorithm.Clear()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.computehash#system-security-cryptography-hashalgorithm-computehash(system-byte())" target="_blank" rel="noreferrer">HashAlgorithm.ComputeHash(byte[])</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.computehash#system-security-cryptography-hashalgorithm-computehash(system-byte()-system-int32-system-int32)" target="_blank" rel="noreferrer">HashAlgorithm.ComputeHash(byte[], int, int)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.computehash#system-security-cryptography-hashalgorithm-computehash(system-io-stream)" target="_blank" rel="noreferrer">HashAlgorithm.ComputeHash(Stream)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.computehashasync" target="_blank" rel="noreferrer">HashAlgorithm.ComputeHashAsync(Stream, CancellationToken)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.create#system-security-cryptography-hashalgorithm-create" target="_blank" rel="noreferrer">HashAlgorithm.Create()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.create#system-security-cryptography-hashalgorithm-create(system-string)" target="_blank" rel="noreferrer">HashAlgorithm.Create(string)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.dispose#system-security-cryptography-hashalgorithm-dispose" target="_blank" rel="noreferrer">HashAlgorithm.Dispose()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.dispose#system-security-cryptography-hashalgorithm-dispose(system-boolean)" target="_blank" rel="noreferrer">HashAlgorithm.Dispose(bool)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.hashcore#system-security-cryptography-hashalgorithm-hashcore(system-byte()-system-int32-system-int32)" target="_blank" rel="noreferrer">HashAlgorithm.HashCore(byte[], int, int)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.hashcore#system-security-cryptography-hashalgorithm-hashcore(system-readonlyspan((system-byte)))" target="_blank" rel="noreferrer">HashAlgorithm.HashCore(ReadOnlySpan&lt;byte&gt;)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.hashfinal" target="_blank" rel="noreferrer">HashAlgorithm.HashFinal()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.initialize" target="_blank" rel="noreferrer">HashAlgorithm.Initialize()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.transformblock" target="_blank" rel="noreferrer">HashAlgorithm.TransformBlock(byte[], int, int, byte[]?, int)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.transformfinalblock" target="_blank" rel="noreferrer">HashAlgorithm.TransformFinalBlock(byte[], int, int)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.trycomputehash" target="_blank" rel="noreferrer">HashAlgorithm.TryComputeHash(ReadOnlySpan&lt;byte&gt;, Span&lt;byte&gt;, out int)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.tryhashfinal" target="_blank" rel="noreferrer">HashAlgorithm.TryHashFinal(Span&lt;byte&gt;, out int)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.canreusetransform" target="_blank" rel="noreferrer">HashAlgorithm.CanReuseTransform</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.cantransformmultipleblocks" target="_blank" rel="noreferrer">HashAlgorithm.CanTransformMultipleBlocks</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.hash" target="_blank" rel="noreferrer">HashAlgorithm.Hash</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.hashsize" target="_blank" rel="noreferrer">HashAlgorithm.HashSize</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.inputblocksize" target="_blank" rel="noreferrer">HashAlgorithm.InputBlockSize</a>, <a href="https://learn.microsoft.com/dotnet/api/system.security.cryptography.hashalgorithm.outputblocksize" target="_blank" rel="noreferrer">HashAlgorithm.OutputBlockSize</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object)" target="_blank" rel="noreferrer">object.Equals(object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.equals#system-object-equals(system-object-system-object)" target="_blank" rel="noreferrer">object.Equals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gethashcode" target="_blank" rel="noreferrer">object.GetHashCode()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.gettype" target="_blank" rel="noreferrer">object.GetType()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone" target="_blank" rel="noreferrer">object.MemberwiseClone()</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.referenceequals" target="_blank" rel="noreferrer">object.ReferenceEquals(object?, object?)</a>, <a href="https://learn.microsoft.com/dotnet/api/system.object.tostring" target="_blank" rel="noreferrer">object.ToString()</a></p><h4 id="extension-methods" tabindex="-1">Extension Methods <a class="header-anchor" href="#extension-methods" aria-label="Permalink to &quot;Extension Methods&quot;">​</a></h4><p><a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Add__1_System_Object_System_Object_System_Object_">ObjectExtension.Add&lt;T&gt;(object, object, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_CopyEntity_System_Object_System_Object_System_String___">ObjectExtension.CopyEntity(object, object, params string[])</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_">ObjectExtension.EntityToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_">ObjectExtension.EntityToJson(object, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_EntityToJson_System_Object_System_Boolean_System_String_">ObjectExtension.EntityToJson(object, bool, string)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_GetDictionary_System_Object_">DictionaryExtension.GetDictionary(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetFieldKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetFieldKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetFieldKey(object, Type, string, out bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtr_System_Object_">ObjectExtension.GetIntPtr(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_GetIntPtrInt_System_Object_">ObjectExtension.GetIntPtrInt(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertieFind_System_Object_System_String_System_Boolean_">TypeExtension.GetPropertieFind(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetProperties_System_Object_">TypeExtension.GetProperties(object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey__1_System_Object_System_String_System_Boolean__">TypeExtension.GetPropertyKey&lt;T&gt;(object, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetPropertyKey_System_Object_System_Type_System_String_System_Boolean__">TypeExtension.GetPropertyKey(object, Type, string, out bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_ComponentModel_PropertyDescriptor_">TypeExtension.GetValue(object, PropertyDescriptor)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_System_Boolean_">TypeExtension.GetValue(object, string, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_GetValue_System_Object_System_String_">TypeExtension.GetValue(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, object, int, int)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_Read__1_System_Object_System_Int32_System_Object_System_Int32_System_Int32_">ObjectExtension.Read&lt;T&gt;(object, int, object, int, int)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_SetDictionary_System_Object_System_Collections_Generic_IDictionary_System_String_System_Object__">DictionaryExtension.SetDictionary(object, IDictionary&lt;string, object&gt;)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey__1_System_Object_System_String_System_Object_">TypeExtension.SetFieldKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetFieldKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetFieldKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey__1_System_Object_System_String_System_Object_">TypeExtension.SetPropertyKey&lt;T&gt;(object, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetPropertyKey_System_Object_System_Type_System_String_System_Object_">TypeExtension.SetPropertyKey(object, Type, string, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_ComponentModel_PropertyDescriptor_System_Object_">TypeExtension.SetValue(object, PropertyDescriptor, object)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_System_Boolean_">TypeExtension.SetValue(object, string, object, bool)</a>, <a href="./Tool.Utils.TypeExtension.html#Tool_Utils_TypeExtension_SetValue_System_Object_System_String_System_Object_">TypeExtension.SetValue(object, string, object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBase64String_System_Object_">ObjectExtension.ToBase64String(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_">ObjectExtension.ToBytes(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToBytes_System_Object_System_Type__">ObjectExtension.ToBytes(object, out Type)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary_System_Object_">DictionaryExtension.ToDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToDictionary__1_System_Object_">DictionaryExtension.ToDictionary&lt;T&gt;(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary_System_Object_">DictionaryExtension.ToIDictionary(object)</a>, <a href="./Tool.Utils.Data.DictionaryExtension.html#Tool_Utils_Data_DictionaryExtension_ToIDictionary__1_System_Object_">DictionaryExtension.ToIDictionary&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_">ObjectExtension.ToJson(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJson_System_Object_System_Text_Json_JsonSerializerOptions_">ObjectExtension.ToJson(object, JsonSerializerOptions)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_">ObjectExtension.ToJsonWeb(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToJsonWeb_System_Object_System_Action_System_Text_Json_JsonSerializerOptions__">ObjectExtension.ToJsonWeb(object, Action&lt;JsonSerializerOptions&gt;)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToTryVar__1_System_Object___0_">ObjectExtension.ToTryVar&lt;T&gt;(object, T)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar__1_System_Object_">ObjectExtension.ToVar&lt;T&gt;(object)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_Type_System_Boolean_">ObjectExtension.ToVar(object, Type, bool)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToVar_System_Object_System_String_">ObjectExtension.ToVar(object, string)</a>, <a href="./Tool.ObjectExtension.html#Tool_ObjectExtension_ToXml_System_Object_">ObjectExtension.ToXml(object)</a></p><h2 id="remarks" tabindex="-1">Remarks <a class="header-anchor" href="#remarks" aria-label="Permalink to &quot;Remarks&quot;">​</a></h2><p>代码由逆血提供支持</p><h2 id="constructors" tabindex="-1">Constructors <a class="header-anchor" href="#constructors" aria-label="Permalink to &quot;Constructors&quot;">​</a></h2><h3 id="crc32" tabindex="-1"><a id="Tool_Utils_Crc32__ctor"></a> Crc32() <a class="header-anchor" href="#crc32" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_Crc32__ctor&quot;&gt;&lt;/a&gt; Crc32\\(\\)&quot;">​</a></h3><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Crc32</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h3 id="crc32-uint-uint" tabindex="-1"><a id="Tool_Utils_Crc32__ctor_System_UInt32_System_UInt32_"></a> Crc32(uint, uint) <a class="header-anchor" href="#crc32-uint-uint" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_Crc32__ctor_System_UInt32_System_UInt32_&quot;&gt;&lt;/a&gt; Crc32\\(uint, uint\\)&quot;">​</a></h3><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Crc32</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">uint</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> polynomial, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">uint</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;"> seed)</span></span></code></pre></div><h4 id="parameters" tabindex="-1">Parameters <a class="header-anchor" href="#parameters" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>polynomial</code> <a href="https://learn.microsoft.com/dotnet/api/system.uint32" target="_blank" rel="noreferrer">uint</a></p><p><code>seed</code> <a href="https://learn.microsoft.com/dotnet/api/system.uint32" target="_blank" rel="noreferrer">uint</a></p><h2 id="fields" tabindex="-1">Fields <a class="header-anchor" href="#fields" aria-label="Permalink to &quot;Fields&quot;">​</a></h2><h3 id="defaultpolynomial" tabindex="-1"><a id="Tool_Utils_Crc32_DefaultPolynomial"></a> DefaultPolynomial <a class="header-anchor" href="#defaultpolynomial" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_Crc32_DefaultPolynomial&quot;&gt;&lt;/a&gt; DefaultPolynomial&quot;">​</a></h3><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> const</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> uint</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> DefaultPolynomial</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> =</span><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;"> 3988292384</span></span></code></pre></div><h4 id="field-value" tabindex="-1">Field Value <a class="header-anchor" href="#field-value" aria-label="Permalink to &quot;Field Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.uint32" target="_blank" rel="noreferrer">uint</a></p><h3 id="defaultseed" tabindex="-1"><a id="Tool_Utils_Crc32_DefaultSeed"></a> DefaultSeed <a class="header-anchor" href="#defaultseed" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_Crc32_DefaultSeed&quot;&gt;&lt;/a&gt; DefaultSeed&quot;">​</a></h3><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> const</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> uint</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> DefaultSeed</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> =</span><span style="--shiki-light:#005CC5;--shiki-dark:#79B8FF;"> 4294967295</span></span></code></pre></div><h4 id="field-value-1" tabindex="-1">Field Value <a class="header-anchor" href="#field-value-1" aria-label="Permalink to &quot;Field Value&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.uint32" target="_blank" rel="noreferrer">uint</a></p><h2 id="methods" tabindex="-1">Methods <a class="header-anchor" href="#methods" aria-label="Permalink to &quot;Methods&quot;">​</a></h2><h3 id="compute-byte" tabindex="-1"><a id="Tool_Utils_Crc32_Compute_System_Byte___"></a> Compute(byte[]) <a class="header-anchor" href="#compute-byte" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_Crc32_Compute_System_Byte___&quot;&gt;&lt;/a&gt; Compute\\(byte\\[\\]\\)&quot;">​</a></h3><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> uint</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Compute</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">byte</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">[] </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">buffer</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-1" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-1" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>buffer</code> <a href="https://learn.microsoft.com/dotnet/api/system.byte" target="_blank" rel="noreferrer">byte</a>[]</p><h4 id="returns" tabindex="-1">Returns <a class="header-anchor" href="#returns" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.uint32" target="_blank" rel="noreferrer">uint</a></p><h3 id="compute-uint-byte" tabindex="-1"><a id="Tool_Utils_Crc32_Compute_System_UInt32_System_Byte___"></a> Compute(uint, byte[]) <a class="header-anchor" href="#compute-uint-byte" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_Crc32_Compute_System_UInt32_System_Byte___&quot;&gt;&lt;/a&gt; Compute\\(uint, byte\\[\\]\\)&quot;">​</a></h3><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> uint</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Compute</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">uint</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> seed</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">byte</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">[] </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">buffer</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-2" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-2" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>seed</code> <a href="https://learn.microsoft.com/dotnet/api/system.uint32" target="_blank" rel="noreferrer">uint</a></p><p><code>buffer</code> <a href="https://learn.microsoft.com/dotnet/api/system.byte" target="_blank" rel="noreferrer">byte</a>[]</p><h4 id="returns-1" tabindex="-1">Returns <a class="header-anchor" href="#returns-1" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.uint32" target="_blank" rel="noreferrer">uint</a></p><h3 id="compute-uint-uint-byte" tabindex="-1"><a id="Tool_Utils_Crc32_Compute_System_UInt32_System_UInt32_System_Byte___"></a> Compute(uint, uint, byte[]) <a class="header-anchor" href="#compute-uint-uint-byte" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_Crc32_Compute_System_UInt32_System_UInt32_System_Byte___&quot;&gt;&lt;/a&gt; Compute\\(uint, uint, byte\\[\\]\\)&quot;">​</a></h3><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> static</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> uint</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Compute</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">uint</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> polynomial</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">uint</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> seed</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">byte</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">[] </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">buffer</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-3" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-3" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>polynomial</code> <a href="https://learn.microsoft.com/dotnet/api/system.uint32" target="_blank" rel="noreferrer">uint</a></p><p><code>seed</code> <a href="https://learn.microsoft.com/dotnet/api/system.uint32" target="_blank" rel="noreferrer">uint</a></p><p><code>buffer</code> <a href="https://learn.microsoft.com/dotnet/api/system.byte" target="_blank" rel="noreferrer">byte</a>[]</p><h4 id="returns-2" tabindex="-1">Returns <a class="header-anchor" href="#returns-2" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.uint32" target="_blank" rel="noreferrer">uint</a></p><h3 id="hashcore-byte-int-int" tabindex="-1"><a id="Tool_Utils_Crc32_HashCore_System_Byte___System_Int32_System_Int32_"></a> HashCore(byte[], int, int) <a class="header-anchor" href="#hashcore-byte-int-int" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_Crc32_HashCore_System_Byte___System_Int32_System_Int32_&quot;&gt;&lt;/a&gt; HashCore\\(byte\\[\\], int, int\\)&quot;">​</a></h3><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">protected</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> override</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> HashCore</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">(</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">byte</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">[] </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">buffer</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> start</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">, </span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">int</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> length</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">)</span></span></code></pre></div><h4 id="parameters-4" tabindex="-1">Parameters <a class="header-anchor" href="#parameters-4" aria-label="Permalink to &quot;Parameters&quot;">​</a></h4><p><code>buffer</code> <a href="https://learn.microsoft.com/dotnet/api/system.byte" target="_blank" rel="noreferrer">byte</a>[]</p><p><code>start</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><p><code>length</code> <a href="https://learn.microsoft.com/dotnet/api/system.int32" target="_blank" rel="noreferrer">int</a></p><h3 id="hashfinal" tabindex="-1"><a id="Tool_Utils_Crc32_HashFinal"></a> HashFinal() <a class="header-anchor" href="#hashfinal" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_Crc32_HashFinal&quot;&gt;&lt;/a&gt; HashFinal\\(\\)&quot;">​</a></h3><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">protected</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> override</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> byte</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">[] </span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;">HashFinal</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div><h4 id="returns-3" tabindex="-1">Returns <a class="header-anchor" href="#returns-3" aria-label="Permalink to &quot;Returns&quot;">​</a></h4><p><a href="https://learn.microsoft.com/dotnet/api/system.byte" target="_blank" rel="noreferrer">byte</a>[]</p><h3 id="initialize" tabindex="-1"><a id="Tool_Utils_Crc32_Initialize"></a> Initialize() <a class="header-anchor" href="#initialize" aria-label="Permalink to &quot;&lt;a id=&quot;Tool_Utils_Crc32_Initialize&quot;&gt;&lt;/a&gt; Initialize\\(\\)&quot;">​</a></h3><p>初始化参数</p><div class="language-csharp vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">csharp</span><pre class="shiki shiki-themes github-light github-dark vp-code" tabindex="0"><code><span class="line"><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;">public</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> override</span><span style="--shiki-light:#D73A49;--shiki-dark:#F97583;"> void</span><span style="--shiki-light:#6F42C1;--shiki-dark:#B392F0;"> Initialize</span><span style="--shiki-light:#24292E;--shiki-dark:#E1E4E8;">()</span></span></code></pre></div>',66)]))}const m=e(o,[["render",r]]);export{_ as __pageData,m as default};
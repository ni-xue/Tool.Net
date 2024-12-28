﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Utils;
using Tool.Utils.ActionDelegate;
using Tool.Utils.Data;
using static System.Net.Mime.MediaTypeNames;

namespace Tool.Web.Api.ApiCore
{
    /// <summary>
    /// 获取当前方法的状态,子二级对象
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    internal class AshxExtension //: IDisposable
    {
        /// <summary>
        /// 当前可以被控制器调起的方法信息
        /// </summary>
        internal IReadOnlyDictionary<string, Ashx> Ashxes;//Dictionary

        /// <summary>
        /// 当前对象的AshxType
        /// </summary>
        internal Type AshxType { get; private set; }

        /// <summary>
        /// 当前控制器的 New 对象
        /// </summary>
        private ClassDispatcher<IHttpAsynApi> AshxClassDelegater { get; set; }

        /// <summary>
        /// 开始就实现的API类
        /// </summary>
        private IMinHttpAsynApi MinHttpAsynApi { get; set; }

        /// <summary>
        /// 当前对象的AshxType 是不是最小轻量级的
        /// </summary>
        internal bool IsMin { get; private set; }

        ///// <summary>
        ///// 当前进程的可被调起的所有的一般处理程序
        ///// </summary>
        //internal static Dictionary<string, AshxExtension> AshxKeys = new Dictionary<string, AshxExtension>();

        //System.Collections.Hashtable hashtable = new System.Collections.Hashtable(100);


        /// <summary>
        /// 安全锁
        /// </summary>
        private static readonly object state = new();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isMin"></param>
        internal AshxExtension(Type type, bool isMin)//, object args
        {
            lock (state)
            {
                this.IsMin = isMin;
                this.AshxType = type;
                if (this.IsMin)
                {
                    try
                    {
                        MinHttpAsynApi = Activator.CreateInstance(this.AshxType) as IMinHttpAsynApi;
                    }
                    catch
                    {
                        throw new Exception(string.Format(System.Globalization.CultureInfo.CurrentCulture, "在创建时发生异常，应该是使用有参构造函数，类信息: {0}", this.AshxType.FullName));
                    }
                }
                else
                {
                    var constructorInfos = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

                    if (constructorInfos.Length == 0) throw new Exception(string.Format("类：{0}，继承了【ApiAshx】类，但是无法获取到公开的构造函数，无法创建消息体。", type.FullName));

                    if (constructorInfos.Length > 1) throw new Exception(string.Format("类：{0}，继承了【ApiAshx】类，但是存在多个构造函数，无法创建消息体。", type.FullName));

                    //var par =  constructorInfos[0].GetParameters();
                    // if (par.Length > 0) 
                    // {
                    //     //List<System.Linq.Expressions.Expression> expressions = new();
                    //     //for (int i = 0; i < par.Length; i++)
                    //     //{
                    //     //    expressions.Add(System.Linq.Expressions.Expression.Parameter(par[i].ParameterType, par[i].Name));
                    //     //}

                    //     //var as1 = System.Linq.Expressions.Expression.New(constructorInfos[0], expressions);


                    //     var as0 = System.Linq.Expressions.Expression.Parameter(typeof(object[]), "parameters");

                    //     ActionDispatcher.GetParameter(out List<System.Linq.Expressions.Expression> expressions, as0, par);

                    //     var as1 = System.Linq.Expressions.Expression.New(constructorInfos[0], expressions);

                    //     //System.Linq.Expressions.MethodCallExpression methodCall = System.Linq.Expressions.Expression.Call(as1, null, expressions);//(MethodInfo)(MethodBase)constructorInfos[0]

                    //     var as2 = System.Linq.Expressions.Expression.Lambda<NewClass<object>>(as1, as0);

                    //     //constructorInfos[0].DeclaringType

                    //     var as3 = as2.Compile(); //_newclass(null);



                    // }

                    //var as0 = ClassDelegater<object>.GetClass(constructorInfos[0]);

                    AshxClassDelegater = new ClassDispatcher<IHttpAsynApi>(constructorInfos[0]);

                    //var as4 = as0(new object[] { null });

                    //var as5 = AshxClassDelegater.Invoke(new object[] { null });
                }
                this.Ashxes = GetApiAction(type, this.IsMin)?.AsReadOnly();


                if (this.Ashxes == null || this.Ashxes.Count == 0)
                {
                    throw new Exception(string.Format(System.Globalization.CultureInfo.CurrentCulture, "【{0}】控制器不包含任何一个可访问接口，请检查！", this.AshxType.FullName));
                }
            }
        }

        /// <summary>
        /// 判断当前方法是否可以被调起,获取对象
        /// </summary>
        /// <param name="method"></param>
        /// <param name="ashx">返回可以被调用的实体类</param>
        /// <returns></returns>
        internal bool IsMethod(string method, out Ashx ashx)
        {
            if (string.IsNullOrWhiteSpace(method))
            {
                ashx = null;
                return false;
            }
            if (Ashxes == null)
            {
                ashx = null;
                return false;
            }

            bool IsAshx = Ashxes.TryGetValue(method, out Ashx _ashx);

            if (IsAshx)
            {
                ashx = _ashx;
                return true;
            }
            else
            {
                ashx = null;
                return false;
            }
        }

        /// <summary>
        /// 判断该方法是不是属于发起方的请求方式
        /// </summary>
        /// <param name="httpMethod"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        internal static bool HttpMethod(string httpMethod, AshxState State)
        {
            //return State switch
            //{
            //    AshxState.All => true,
            //    AshxState.Get => httpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase),
            //    AshxState.Post => httpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase),
            //    _ => false,
            //};

            if (State == AshxState.All) return true;

            bool httpmt()
            {
                return State switch
                {
                    //AshxState.All => true,
                    AshxState.Get => httpMethod.EqualsNotCase("GET"),
                    AshxState.Post => httpMethod.EqualsNotCase("POST"),
                    AshxState.Put => httpMethod.EqualsNotCase("PUT"),
                    AshxState.Patch => httpMethod.EqualsNotCase("PATCH"),
                    AshxState.Delete => httpMethod.EqualsNotCase("DELETE"),
                    AshxState.Head => httpMethod.EqualsNotCase("HEAD"),
                    _ => false,
                };
            }

            return httpmt();

            //switch (State)
            //{
            //    case AshxState.All:
            //        return true;
            //    case AshxState.Get:
            //        return httpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase);
            //    case AshxState.Post:
            //        return httpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase);
            //    default:
            //        return false;
            //}

            //if (State == AshxState.All)
            //{
            //    return true;
            //}

            //AshxState _State = new AshxState();

            //if (httpMethod.Equals("GET"))
            //{
            //    _State = AshxState.Get;
            //}
            //else if (httpMethod.Equals("POST"))
            //{
            //    _State = AshxState.Post;
            //}
            //else
            //{
            //    _State = AshxState.All;
            //}

            //if (_State == State)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        internal static async ValueTask<object> GetParameterObj(HttpRequest request, ApiParameter parameter)
        {
            return parameter.GetVal switch
            {
                Val.AllData => QueryOrForm(0),
                Val.Query => QueryOrForm(1),
                Val.Form => QueryOrForm(2),
                Val.AllMode => Mode(0),
                Val.QueryMode => Mode(1),
                Val.FormMode => Mode(2),
                Val.Header => GetHeader(),//request.Headers.TryGetValue(parameter.Name, out Microsoft.Extensions.Primitives.StringValues value1);
                                          //return value1.ToString().ToVar(parameter.ParameterType, false);
                Val.Cookie => request.Cookies.TryGetValue(parameter.KeyName, out string value2) ? value2.ToVar(parameter.ParameterType, false) : null,//request.Cookies.TryGetValue(parameter.Name, out string value2);
                                                                                                                                                      //return value2.ToVar(parameter.ParameterType, false);
                Val.File => GetFile(),// request.HasFormContentType ? request.Form.Files.GetFile(parameter.Name) : null;
                Val.Files => request.HasFormContentType ? request.Form.Files : null,
                Val.Service => request.HttpContext.RequestServices.GetService(parameter.ParameterType),
                Val.Session => GetSession(),
                Val.RouteKey => request.RouteValues.TryGetValue(parameter.KeyName, out object value4) ? value4.ToVar(parameter.ParameterType, false) : null,
                //(parameter.DefaultValue is System.DBNull) ? parameter.ParameterObj : parameter.DefaultValue;
                Val.Body => await GetBody(),
                Val.BodyJson => await GetBodyJson(),
                Val.BodyString => await GetBodyString(),
                _ => null,
            };

            object Mode(int state)
            {
                object value = state switch
                {
                    1 => HttpRequestExtension.GetToMode(request, parameter.ParameterType, false),
                    2 => HttpRequestExtension.PostToMode(request, parameter.ParameterType, false),
                    _ => HttpRequestExtension.ALLToMode(request, parameter.ParameterType, false),
                };
                return value;
            }

            object QueryOrForm(int state)
            {
                string value = state switch
                {
                    1 => request.GetQueryString(parameter.KeyName),
                    2 => request.GetFormString(parameter.KeyName),
                    _ => request.GetString(parameter.KeyName),
                };
                return string.IsNullOrWhiteSpace(value) ? null : value.ToVar(parameter.ParameterType, false);
            }

            object GetHeader()
            {
                if (request.Headers.TryGetValue(parameter.KeyName, out Microsoft.Extensions.Primitives.StringValues value1))
                {
                    if (value1.Count != 1)
                    {
                        return null;
                    }
                    return value1[0].ToVar(parameter.ParameterType, false);
                }
                return null;
            }

            object GetSession()
            {
                try
                {
                    ISession session = request.HttpContext.Session;

                    if (typeof(byte[]) == parameter.ParameterType)
                    {
                        session.TryGetValue(parameter.KeyName, out byte[] value3);
                        return value3;
                    }
                    else if (parameter.IsType)//(typeof(string) == parameter.ParameterType)
                    {
                        if (session.TryGetValue(parameter.KeyName, out string value3)) return value3.ToVar(parameter.ParameterType, false); else return null;
                    }
                    else //if(!parameter.IsType)
                    {
                        session.TryGetValue(parameter.KeyName, parameter.ParameterType, out object value3);
                        return value3;
                    }
                }
                catch (Exception)
                {
                    return null;
                }

                //return null;//(parameter.DefaultValue is System.DBNull) ? parameter.ParameterObj : parameter.DefaultValue;
            }

            object GetFile()
            {
                try
                {
                    return request.HasFormContentType ? request.Form.Files.GetFile(parameter.KeyName) : null;
                }
                catch (Exception ex)
                {
                    return new TryFormFile(ex);
                }
            }

            async ValueTask<object> GetBody()
            {
                if (parameter.ParameterType == typeof(Stream))
                {
                    return request.Body;
                }
                else if (parameter.ParameterType == typeof(PipeReader))
                {
                    return request.BodyReader;
                }
                else if (request.ContentLength.HasValue)
                {
                    request.EnableBuffering(1024 * 100);

                    var owner = MemoryPool<byte>.Shared.Rent((int)request.ContentLength);
                    request.HttpContext.Response.RegisterForDispose(owner);
                    int length = await request.Body.ReadAsync(owner.Memory);
                    request.Body.Seek(0, SeekOrigin.Begin);
                    return owner.Memory[..length];

                    //BytesCore bytesCore = new((int)request.ContentLength);
                    //request.HttpContext.Response.RegisterForDispose(bytesCore);
                    //await request.Body.ReadAsync(bytesCore.Memory);
                    //request.Body.Seek(0, SeekOrigin.Begin);
                    //return bytesCore.Memory;

                }
                return null;
            }

           async ValueTask<object> GetBodyJson()
            {
                if (request.ContentLength.HasValue)
                {
                    object _obj = null;
                    request.EnableBuffering(1024 * 100);
                    if (parameter.ParameterType == typeof(JsonVar))
                    {
                        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(request.Body);
                        _obj = new JsonVar(JsonHelper.GetReturn(jsonDocument.RootElement));
                    }
                    else
                    {
                        _obj = await JsonSerializer.DeserializeAsync(request.Body, parameter.ParameterType);
                    }
                    request.Body.Seek(0, SeekOrigin.Begin);
                    return _obj;
                }
                return null;
            }

            async ValueTask<string> GetBodyString()
            {
                if (request.ContentLength.HasValue)
                {
                    request.EnableBuffering(1024 * 100);
                    using StreamReader stream = new(request.Body);
                    return await stream.ReadToEndAsync();
                }
                return null;
            }
        }

        internal static async ValueTask<ValueTuple<object[], Exception>> GetParameterObjs(Ashx ashx, HttpRequest request, int index, int length, object[] _objs)
        {
            //int index = 0;
            //int length = ashx.Parameters.Length;

            //object[] _objs = new object[length];

            //if (isMin)
            //{
            //    if (ashx.Parameters[index].ParameterType == typeof(HttpContext))
            //    {
            //        _objs[0] = request.HttpContext;
            //        index = 1;
            //    }
            //}

            try
            {
                while (index < length)
                {
                    ApiParameter parameter = ashx.Parameters[index];

                    object obj = null;

                    obj = await GetParameterObj(request, parameter);

                    //if (parameter.IsType)
                    //{
                    //    void paramType()
                    //    {
                    //        string value = string.Empty;

                    //        switch (ashx.State)
                    //        {
                    //            //case AshxState.All:
                    //            //    value = request.GetString(parameter.Name);
                    //            //    break;
                    //            case AshxState.Get:
                    //                value = request.GetQueryString(parameter.Name);
                    //                break;
                    //            case AshxState.Post:
                    //                value = request.GetFormString(parameter.Name);
                    //                break;
                    //            default:
                    //                value = request.GetString(parameter.Name);
                    //                break;
                    //        }

                    //        if (!string.IsNullOrWhiteSpace(value))
                    //        {
                    //            //TypeCode typeCode = Convert.GetTypeCode(obj);
                    //            obj = value.ToVar(parameter.ParameterType, false); //.Add(value.ToVar(parameter.ParameterType, false));
                    //        }
                    //    }

                    //    paramType();
                    //}
                    //else if (ashx.IsMode)
                    //{
                    //    void Mode()
                    //    {
                    //        object value = null;

                    //        switch (ashx.State)
                    //        {
                    //            //case AshxState.All:
                    //            //    value = HttpRequestExtension.ALLToMode(request, parameter.ParameterType, false);
                    //            //    break;
                    //            case AshxState.Get:
                    //                value = HttpRequestExtension.GetToMode(request, parameter.ParameterType, false);
                    //                break;
                    //            case AshxState.Post:
                    //                value = HttpRequestExtension.PostToMode(request, parameter.ParameterType, false);
                    //                break;
                    //            default:
                    //                value = HttpRequestExtension.ALLToMode(request, parameter.ParameterType, false);
                    //                break;
                    //        }

                    //        if (value != null)
                    //        {
                    //            obj = value; //.Add(value);
                    //        }
                    //    }

                    //    Mode();
                    //}

                    //if (obj == null)
                    //{
                    //    if (!(parameter.DefaultValue is System.DBNull))
                    //    {
                    //        obj = parameter.DefaultValue; //.Add(parameter.DefaultValue);
                    //    }
                    //    else
                    //    {
                    //        obj = parameter.ParameterObj;
                    //    }
                    //}

                    obj ??= parameter.ValueOrObj;

                    _objs[index] = obj;

                    index++;
                }

                return new ValueTuple<object[], Exception>(_objs, null);
            }
            catch (Exception e)
            {
                Log.Fatal("Api调用异常（参数错误）:", e, "Log/Tool");
                return new ValueTuple<object[], Exception>(_objs, e);
            }
        }

        const string AllowOrigin = "Access-Control-Allow-Origin";
        const string AllowCredentials = "Access-Control-Allow-Credentials";
        const string AllowMethods = "Access-Control-Allow-Methods";
        const string AllowHeaders = "Access-Control-Allow-Headers";

        /// <summary>
        /// 根据信息设置是否跨域请求
        /// </summary>
        /// <param name="response"></param>
        /// <param name="ashx"></param>
        internal static bool CrossDomain(HttpResponse response, Ashx ashx)
        {

            //指定允许其他域名访问
            //'Access-Control-Allow-Origin:http://172.20.0.206'//一般用法（*，指定域，动态设置），3是因为*不允许携带认证头和cookies
            //是否允许后续请求携带认证信息（cookies）,该值只能是true,否则不返回
            //'Access-Control-Allow-Credentials:true'
            //预检结果缓存时间,也就是上面说到的缓存啦
            //'Access-Control-Max-Age: 1800'
            //允许的请求类型
            //'Access-Control-Allow-Methods:GET,POST,PUT,POST'
            //允许的请求头字段
            //'Access-Control-Allow-Headers:x-requested-with,content-type'

            Tool.Web.Api.CrossDomain crossDomain = ashx.CrossDomain;

            if (crossDomain != null)
            {
                void crossdomain()
                {
                    if (!response.Headers.ContainsKey(AllowOrigin))
                    {
                        response.AppendHeader(AllowOrigin, crossDomain.Origin ?? "*");
                    }
                    if (!response.Headers.ContainsKey(AllowCredentials) && crossDomain.Credentials == true)
                    {
                        response.AppendHeader(AllowCredentials, "true");
                    }
                    if (!string.IsNullOrWhiteSpace(crossDomain.Methods) && !response.Headers.ContainsKey(AllowMethods))
                    {
                        response.AppendHeader(AllowMethods, crossDomain.Methods);
                    }
                    if (!string.IsNullOrWhiteSpace(crossDomain.Headers) && !response.Headers.ContainsKey(AllowHeaders))
                    {
                        response.AppendHeader(AllowHeaders, crossDomain.Headers);
                    }
                    //if (crossDomain.MaxAge > 0 && !response.Headers.ContainsKey("Access-Control-Max-Age"))
                    //{
                    //    response.AppendHeader("Access-Control-Max-Age", crossDomain.MaxAge.ToString());
                    //}
                }
                crossdomain();

                return true;
            }
            return false;
        }

        /// <summary>
        /// 创建 新的 控制器对象
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        internal IHttpAsynApi NewClassAshx(IServiceProvider service)
        {
            object[] getParameters()
            {
                int length = AshxClassDelegater.Parameters.Length;
                if (length == 0) return null;
                object[] parameters = new object[length];
                for (int i = 0; i < length; i++)
                {
                    var parameter = AshxClassDelegater.Parameters[i];
                    if (parameter.IsType)
                    {
                        parameters[i] = parameter.ValueOrObj;
                    }
                    else
                    {
                        object _obj = service.GetService(parameter.ParameterType);
                        if (_obj == null)
                        {
                            parameters[i] = parameter.ValueOrObj;
                        }
                        parameters[i] = _obj;
                    }
                }
                return parameters;
            }

            return AshxClassDelegater.Invoke(getParameters());
        }

        internal IMinHttpAsynApi ClassMinApi() => MinHttpAsynApi;

        ///// <summary>
        ///// 判断该方法是不是属于发起方的请求方式
        ///// </summary>
        ///// <param name="request"></param>
        ///// <param name="_method"></param>
        ///// <returns></returns>
        //internal bool HttpMethod(HttpRequest request, string _method)
        //{
        //    if (Ashxes[_method].State == AshxState.All)
        //    {
        //        return true;
        //    }

        //    AshxState _State = new AshxState();

        //    if (request.HttpMethod == "GET")
        //    {
        //        _State = AshxState.Get;
        //    }
        //    else if (request.HttpMethod == "POST")
        //    {
        //        _State = AshxState.Post;
        //    }
        //    else
        //    {
        //        _State = AshxState.All;
        //    }

        //    if (_State == Ashxes[_method].State)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// 调起该方法(“包含非公开”、“包含实例成员”和“包含公开”)
        ///// </summary>
        ///// <param name="method">方法对象</param>
        ///// <returns>返回该方法执行后的结果</returns>
        //internal object Invoke(MethodInfo method)
        //{
        //    if (method == null)
        //    {
        //        throw new ArgumentNullException("未找到此方法！");
        //    }
        //    var data = method?.Invoke(args, null);

        //    return data;
        //}

        ///// <summary>
        ///// 调起该方法(“包含非公开”、“包含实例成员”和“包含公开”)
        ///// </summary>
        ///// <param name="method">对象</param>
        ///// <param name="parameters">该方法的指定参数</param>
        ///// <returns>返回该方法执行后的结果</returns>
        //internal object Invoke(MethodInfo method, params object[] parameters)
        //{
        //    if (parameters == null || parameters.Length == 0)
        //    {
        //        throw new ArgumentNullException("方法参数不能为空！");
        //    }
        //    if (method == null)
        //    {
        //        throw new ArgumentNullException("未找到此方法！");
        //    }

        //    var data = method?.Invoke(args, parameters);

        //    return data;
        //}

        ///// <summary>
        ///// 调起该方法(“包含非公开”、“包含实例成员”和“包含公开”)
        ///// </summary>
        ///// <param name="method">方法对象</param>
        ///// <param name="args">对象</param>
        ///// <returns>返回该方法执行后的结果</returns>
        //internal static object Invoke(MethodInfo method, object args)
        //{
        //    //if (method == null)
        //    //{
        //    //    throw new ArgumentNullException("未找到此方法！");
        //    //}
        //    var data = method?.Invoke(args, null);

        //    return data;
        //}

        ///// <summary>
        ///// 调起该方法(“包含非公开”、“包含实例成员”和“包含公开”)
        ///// </summary>
        ///// <param name="method">对象</param>
        ///// <param name="args">对象</param>
        ///// <param name="parameters">该方法的指定参数</param>
        ///// <returns>返回该方法执行后的结果</returns>
        //internal static object Invoke(MethodInfo method, object args, params object[] parameters)
        //{
        //    //if (parameters == null || parameters.Length == 0)
        //    //{
        //    //    throw new ArgumentNullException("方法参数不能为空！");
        //    //}
        //    //if (method == null)
        //    //{
        //    //    throw new ArgumentNullException("未找到此方法！");
        //    //}

        //    var data = method?.Invoke(args, parameters);

        //    return data;
        //}

        /// <summary>
        /// 获取一个内部的调用规则
        /// </summary>
        /// <returns>返回一个类型实体</returns>
        private Dictionary<string, Ashx> GetApiAction(Type type, bool isMin)// where T : new()
        {
            MethodInfo[] method = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            //List<MethodInfo> _method = method.ToList();
            if (method.Length < 1 || method == null)
            {
                return null;
            }
            Dictionary<string, Ashx> keyValues = new(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < method.Length; i++)
            {
                MethodInfo info = method[i];

                char c = info.Name[0];

                if (isMin)
                {
                    //if (info.ReturnType != typeof(Task<IApiOut>) && info.ReturnType != typeof(IApiOut))//.BaseType
                    //{
                    //    //_method.Remove(info);
                    //    continue;
                    //}

                    if (!IsTaskIApiOut(info) && !IsApiOut(info.ReturnType))
                    {
                        continue;
                    }

                    //if (info.ReturnType != typeof(Task<IApiOut>))//.BaseType
                    //{
                    //    //_method.Remove(info);
                    //    throw new ArgumentNullException("方法重名提示：", $"警告：对外调用方法不能存在重名的（异常方法名：{info.Name}）！");
                    //}
                }
                else if (!IsTaskAshxApi(info.ReturnType) && !IsAshxApi(info.ReturnType))//.BaseType
                {
                    //_method.Remove(info);
                    continue;
                }

                if ('a' <= c && c <= 'z')
                {
                    //_method.Remove(info);
                    continue;
                }

                Ashx hobbyAttr;
                Api.CrossDomain cross;

                try
                {
                    hobbyAttr = Attribute.GetCustomAttribute(info, typeof(Ashx), false) as Ashx;
                    cross = Attribute.GetCustomAttribute(info, typeof(Api.CrossDomain), false) as Api.CrossDomain;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Fail("Ashx路由模式异常", $"在访问：{info.DeclaringType.FullName},{info.Name} 接口时失败了！");
                    //hobbyAttr = null;
                    throw new Exception("初始化API接口化时出现异常情况，导致无法继续！", e);
                }
                //System.Diagnostics.Debug.Fail("Ashx路由模式异常", "有一个无法使用的接口");
                if (hobbyAttr != null && !hobbyAttr.IsMethods)
                {
                    continue;
                }

                if (keyValues.ContainsKey(info.Name))
                {
                    throw new Exception($"方法重名提示：警告：对外调用方法不能存在重名的（异常方法名：{info.Name}）！");
                }
                else
                {
                    hobbyAttr ??= new Ashx();

                    //if (typeof(IAshxAction).IsAssignableFrom(type))
                    {
                        hobbyAttr.SetAction(new AahxCore(NewClassAshx, ClassMinApi, GetDispatcher(info, isMin), hobbyAttr.State));
                    }

                    hobbyAttr.GetKeyAttributes(isMin, cross);

                    string MethodsName = info.Name;

                    if (!string.IsNullOrWhiteSpace(hobbyAttr.ID))
                    {
                        if (keyValues.ContainsKey(hobbyAttr.ID))
                        {
                            throw new Exception($"警告：对外调用方法不能存在重名的（异常方法名：{info.Name}）(对外访问名：{hobbyAttr.ID})！");
                        }
                        MethodsName = hobbyAttr.ID;
                    }
                    keyValues.TryAdd(MethodsName, hobbyAttr);
                }
            }
            return keyValues;
        }

        private static IActionDispatcher<IAshxAction> GetDispatcher(MethodInfo info, bool isMin)
        {
            if (isMin)
            {
                return new ActionDispatcher<IAshxAction, IApiOut>(info);
            }
            if (info.ReturnType == typeof(OnAshxEvent))
            {
                return new ActionDispatcher<IAshxAction, OnAshxEvent>(info);
            }
            return new ActionDispatcher<IAshxAction>(info);
        }

        private static bool IsAshxApi(Type ReturnType) => DispatcherCore.IsVoid(ReturnType);

        internal static bool IsTaskAshxApi(Type ReturnType) => DispatcherCore.IsTask(ReturnType) || ReturnType == typeof(OnAshxEvent);

        private static bool IsApiOut(Type ReturnType) => DispatcherCore.IsAssignableFrom<IApiOut>(ReturnType);

        internal static bool IsTaskApiOut(Type ReturnType) => DispatcherCore.IsTaskTuple(ReturnType);

        private static bool IsTaskIApiOut(MethodInfo method) => DispatcherCore.IsTask<IApiOut>(method);

        ///// <summary>
        ///// 通过 PushFrame（进入一个新的消息循环）的方式来同步等待一个必须使用 await 才能等待的异步操作。
        ///// 由于使用了消息循环，所以并不会阻塞 UI 线程。<para/>
        ///// 此方法适用于将一个 async/await 模式的异步代码转换为同步代码。<para/>
        ///// </summary>
        ///// <remarks>
        ///// 此方法适用于任何线程，包括 UI 线程、非 UI 线程、STA 线程、MTA 线程。
        ///// </remarks>
        ///// <typeparam name="TResult">
        ///// 异步方法返回值的类型。
        ///// 我们认为只有包含返回值的方法才会出现无法从异步转为同步的问题，所以必须要求异步方法返回一个值。
        ///// </typeparam>
        ///// <param name="task">异步的带有返回值的任务。</param>
        ///// <returns>异步方法在同步返回过程中的返回值。</returns>
        //public static TResult AwaitByPushFrame<TResult>(Task<TResult> task)
        //{
        //    if (task == null) throw new ArgumentNullException(nameof(task));
        //    Contract.EndContractBlock();

        //    PushFrame

        //    var frame = new DispatcherFrame();
        //    task.ContinueWith(t =>
        //    {
        //        frame.Continue = false;
        //    });
        //    Dispatcher.PushFrame(frame);
        //    return task.Result;
        //}

        ///// <summary>
        ///// 通过 PushFrame（进入一个新的消息循环）的方式来同步等待一个必须使用 await 才能等待的异步操作。
        ///// 由于使用了消息循环，所以并不会阻塞 UI 线程。<para/>
        ///// 此方法适用于将一个 async/await 模式的异步代码转换为同步代码。<para/>
        ///// </summary>
        ///// <remarks>
        ///// 此方法适用于任何线程，包括 UI 线程、非 UI 线程、STA 线程、MTA 线程。
        ///// </remarks>
        ///// <typeparam name="TResult">
        ///// 异步方法返回值的类型。
        ///// 我们认为只有包含返回值的方法才会出现无法从异步转为同步的问题，所以必须要求异步方法返回一个值。
        ///// </typeparam>
        ///// <param name="task">异步的带有返回值的任务。</param>
        ///// <returns>异步方法在同步返回过程中的返回值。</returns>
        //public static TResult AwaitByPushFrame<TResult>(Task<TResult> task)
        //{
        //    if (task == null) throw new ArgumentNullException(nameof(task));
        //    Contract.EndContractBlock();

        //    var frame = new DispatcherFrame();
        //    task.ContinueWith(t =>
        //    {
        //        frame.Continue = false;
        //    });
        //    Dispatcher.PushFrame(frame);
        //    return task.Result;
        //}

        //Enumerator<Parameter> enumerator = ashx.Parameters.GetEnumerator<Parameter>();//System.Collections.

        //while (enumerator.MoveNext())
        //{
        //    Parameter parameter = enumerator.Current; //(Parameter)enumerator.Current;

        //    if (parameter.IsType)//(AshxExtension.IsType(parameter.ParameterType))
        //    {
        //        string value = string.Empty;

        //        if (ashx.State == AshxState.All)
        //        {
        //            value = GameRequest.GetString(request, parameter.Name);
        //        }
        //        else if (ashx.State == AshxState.Get)
        //        {
        //            value = GameRequest.GetQueryString(request, parameter.Name);
        //        }
        //        else if (ashx.State == AshxState.Post)
        //        {
        //            value = GameRequest.GetFormString(request, parameter.Name);
        //        }

        //        if (!string.IsNullOrWhiteSpace(value))
        //        {
        //            //TypeCode typeCode = Convert.GetTypeCode(obj);
        //            _objs[enumerator.Index] = value.ToVar(parameter.ParameterType, false); //.Add(value.ToVar(parameter.ParameterType, false));
        //        }
        //        else if(!(parameter.DefaultValue is System.DBNull))
        //        {
        //            _objs[enumerator.Index] = parameter.DefaultValue; //.Add(parameter.DefaultValue);
        //        }
        //    }
        //    else if (ashx.IsMode)
        //    {
        //        object value = null;

        //        if (ashx.State == AshxState.All)
        //        {
        //            value = AshxExtension.ALLToMode(request, parameter.ParameterType);
        //        }
        //        else if (ashx.State == AshxState.Get)
        //        {
        //            value = AshxExtension.GetToMode(request, parameter.ParameterType);
        //        }
        //        else if (ashx.State == AshxState.Post)
        //        {
        //            value = AshxExtension.PostToMode(request, parameter.ParameterType);
        //        }

        //        if (value != null)
        //        {
        //            _objs[enumerator.Index] = value; //.Add(value);
        //        }
        //        else if (!(parameter.DefaultValue is System.DBNull))
        //        {
        //            _objs[enumerator.Index] = parameter.DefaultValue; //.Add(parameter.DefaultValue);
        //        }
        //    }
        //    else if (!(parameter.DefaultValue is System.DBNull))
        //    {
        //        _objs[enumerator.Index] = parameter.DefaultValue; //.Add(parameter.DefaultValue);
        //    }
        //}










        //foreach (Parameter parameter in ashx.Parameters)
        //{
        //    if (AshxExtension.IsType(parameter.ParameterType))
        //    {
        //        string value = string.Empty;

        //        if (ashx.State == AshxState.All)
        //        {
        //            value = GameRequest.GetString(context.Request, parameter.Name);
        //        }
        //        else if (ashx.State == AshxState.Get)
        //        {
        //            value = GameRequest.GetQueryString(context.Request, parameter.Name);
        //        }
        //        else if (ashx.State == AshxState.Post)
        //        {
        //            value = GameRequest.GetFormString(context.Request, parameter.Name);
        //        }

        //        if (string.IsNullOrWhiteSpace(value))
        //        {
        //            if (parameter.DefaultValue is System.DBNull)
        //            {
        //                _objs.Add(null);
        //            }
        //            else
        //            {
        //                _objs.Add(parameter.DefaultValue);
        //            }
        //        }
        //        else
        //        {
        //            //TypeCode typeCode = Convert.GetTypeCode(obj);
        //            _objs.Add(value.ToVar(parameter.ParameterType, false));
        //        }
        //    }
        //    else if (ashx.IsMode)
        //    {
        //        object value = null;

        //        if (ashx.State == AshxState.All)
        //        {
        //            value = AshxExtension.ALLToMode(context.Request, parameter.ParameterType);
        //        }
        //        else if (ashx.State == AshxState.Get)
        //        {
        //            value = AshxExtension.GetToMode(context.Request, parameter.ParameterType);
        //        }
        //        else if (ashx.State == AshxState.Post)
        //        {
        //            value = AshxExtension.PostToMode(context.Request, parameter.ParameterType);
        //        }

        //        if (value == null)
        //        {
        //            if (parameter.DefaultValue.GetType() != typeof(System.DBNull))
        //            {
        //                _objs.Add(parameter.DefaultValue);
        //            }
        //            else
        //            {
        //                _objs.Add(null);
        //            }
        //        }
        //        else
        //        {
        //            _objs.Add(value);
        //        }
        //    }
        //    else
        //    {
        //        if (parameter.DefaultValue.GetType() != typeof(System.DBNull))
        //        {
        //            _objs.Add(parameter.DefaultValue);
        //        }
        //        else
        //        {
        //            _objs.Add(null);
        //        }
        //    }
        //}










        //int index = 0;
        //int length = ashx.Parameters.Length;

        //object[] _objs = new object[length];
        //    try
        //    {
        //        while (index<length)
        //        {
        //            Parameter parameter = ashx.Parameters[index];

        //            if (parameter.IsType)
        //            {
        //                string value = string.Empty;

        //                if (ashx.State == AshxState.All)
        //                {
        //                    value = GameRequest.GetString(request, parameter.Name);
        //                }
        //                else if (ashx.State == AshxState.Get)
        //                {
        //                    value = GameRequest.GetQueryString(request, parameter.Name);
        //                }
        //                else if (ashx.State == AshxState.Post)
        //                {
        //                    value = GameRequest.GetFormString(request, parameter.Name);
        //                }

        //                if (!string.IsNullOrWhiteSpace(value))
        //                {
        //                    //TypeCode typeCode = Convert.GetTypeCode(obj);
        //                    _objs[index] = value.ToVar(parameter.ParameterType, false); //.Add(value.ToVar(parameter.ParameterType, false));
        //                }
        //                else if (!(parameter.DefaultValue is System.DBNull))
        //                {
        //                    _objs[index] = parameter.DefaultValue; //.Add(parameter.DefaultValue);
        //                }
        //            }
        //            else if (ashx.IsMode)
        //            {
        //                object value = null;

        //                if (ashx.State == AshxState.All)
        //                {
        //                    value = AshxExtension.ALLToMode(request, parameter.ParameterType);
        //                }
        //                else if (ashx.State == AshxState.Get)
        //                {
        //                    value = AshxExtension.GetToMode(request, parameter.ParameterType);
        //                }
        //                else if (ashx.State == AshxState.Post)
        //                {
        //                    value = AshxExtension.PostToMode(request, parameter.ParameterType);
        //                }

        //                if (value != null)
        //                {
        //                    _objs[index] = value; //.Add(value);
        //                }
        //                else if (!(parameter.DefaultValue is System.DBNull))
        //                {
        //                    _objs[index] = parameter.DefaultValue; //.Add(parameter.DefaultValue);
        //                }
        //            }
        //            else if (!(parameter.DefaultValue is System.DBNull))
        //            {
        //                _objs[index] = parameter.DefaultValue; //.Add(parameter.DefaultValue);
        //            }
        //            if (_objs[index] == null)
        //            {
        //                _objs[index] = parameter.ParameterObj;
        //            }
        //            index++;
        //        }

        //        isException = false;

        //        return _objs;
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Fatal("GetParameterObjs:异常了。", e, "Log/Tool");

        //        isException = true;

        //        return null;
        //    }

        private class TryFormFile : IFormFile
        {
            public TryFormFile(Exception tryExc)
            {
                TryExc = tryExc;
            }
            public Exception TryExc { get; set; }
            string IFormFile.ContentType => "异常";

            string IFormFile.ContentDisposition => "异常";

            IHeaderDictionary IFormFile.Headers => throw TryExc;

            long IFormFile.Length => -1;

            string IFormFile.Name => "无法获取IFormFile对象值，请查看错误日志TryExc";

            string IFormFile.FileName => throw TryExc;

            void IFormFile.CopyTo(Stream target)
            {
                throw TryExc;
            }

            Task IFormFile.CopyToAsync(Stream target, CancellationToken cancellationToken)
            {
                throw TryExc;
            }

            Stream IFormFile.OpenReadStream()
            {
                throw TryExc;
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Utils;
using Tool.Utils.ActionDelegate;
using Tool.Utils.Data;

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
        /// 开始就是实力的API类
        /// </summary>
        internal IMinHttpAsynApi MinHttpAsynApi { get; private set; }

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
        private static readonly object state = new object();

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
                this.Ashxes = AshxExtension.GetApiAction(type, this.IsMin).AsReadOnly();
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
                    AshxState.Get => httpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase),
                    AshxState.Post => httpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase),
                    AshxState.Put => httpMethod.Equals("PUT", StringComparison.OrdinalIgnoreCase),
                    AshxState.Patch => httpMethod.Equals("PATCH", StringComparison.OrdinalIgnoreCase),
                    AshxState.Delete => httpMethod.Equals("DELETE", StringComparison.OrdinalIgnoreCase),
                    AshxState.Head => httpMethod.Equals("HEAD", StringComparison.OrdinalIgnoreCase),
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

        internal static object GetParameterObj(HttpRequest request, ApiParameter parameter)
        {
            switch (parameter.GetVal)
            {
                case Val.AllData:
                    return QueryOrForm(0);
                case Val.Query:
                    return QueryOrForm(1);
                case Val.Form:
                    return QueryOrForm(2);

                case Val.AllMode:
                    return Mode(0);
                case Val.QueryMode:
                    return Mode(1);
                case Val.FormMode:
                    return Mode(2);

                case Val.Header:
                    //request.Headers.TryGetValue(parameter.Name, out Microsoft.Extensions.Primitives.StringValues value1);
                    //return value1.ToString().ToVar(parameter.ParameterType, false);

                    return request.Headers.TryGetValue(parameter.KeyName, out Microsoft.Extensions.Primitives.StringValues value1) ? value1.ToString().ToVar(parameter.ParameterType, false) : null;
                case Val.Cookie:
                    //request.Cookies.TryGetValue(parameter.Name, out string value2);
                    //return value2.ToVar(parameter.ParameterType, false);

                    return request.Cookies.TryGetValue(parameter.KeyName, out string value2) ? value2.ToVar(parameter.ParameterType, false) : null;
                case Val.File:
                    return GetFile();// request.HasFormContentType ? request.Form.Files.GetFile(parameter.Name) : null;
                case Val.Service:
                    return request.HttpContext.RequestServices.GetService(parameter.ParameterType);
                case Val.Session:
                    return GetSession();
                case Val.RouteKey:
                    return request.RouteValues.TryGetValue(parameter.KeyName, out object value4) ? value4.ToVar(parameter.ParameterType, false) : null;
                //(parameter.DefaultValue is System.DBNull) ? parameter.ParameterObj : parameter.DefaultValue;
                default:
                    return null;
            }

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

            object GetSession()
            {
                ISession session;
                try
                {
                    session = request.HttpContext.Session;
                }
                catch (Exception)
                {
                    return null;
                }
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
        }

        internal static object[] GetParameterObjs(Ashx ashx, HttpRequest request, int index, int length, object[] _objs, out bool isException)
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

                    obj = GetParameterObj(request, parameter);

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

                    if (obj == null) obj = (parameter.DefaultValue is System.DBNull) ? parameter.ParameterObj : parameter.DefaultValue;

                    _objs[index] = obj;

                    index++;
                }

                isException = false;

                return _objs;
            }
            catch (Exception e)
            {
                Log.Fatal("GetParameterObjs:异常了。", e, "Log/Tool");

                isException = true;

                return null;
            }
        }

        /// <summary>
        /// 根据信息设置是否跨域请求
        /// </summary>
        /// <param name="response"></param>
        /// <param name="ashx"></param>
        internal static void CrossDomain(HttpResponse response, Ashx ashx)
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
                    if (Microsoft.Extensions.Primitives.StringValues.IsNullOrEmpty(response.Headers["Access-Control-Allow-Origin"]))
                    {
                        response.AppendHeader("Access-Control-Allow-Origin", crossDomain.Origin ?? "*");
                    }
                    if (Microsoft.Extensions.Primitives.StringValues.IsNullOrEmpty(response.Headers["Access-Control-Allow-Credentials"]) && crossDomain.Credentials == true)
                    {
                        response.AppendHeader("Access-Control-Allow-Credentials", "true");
                    }
                    if (!string.IsNullOrWhiteSpace(crossDomain.Methods) && Microsoft.Extensions.Primitives.StringValues.IsNullOrEmpty(response.Headers["Access-Control-Allow-Methods"]))
                    {
                        response.AppendHeader("Access-Control-Allow-Methods", crossDomain.Methods);
                    }
                    if (!string.IsNullOrWhiteSpace(crossDomain.Headers) && Microsoft.Extensions.Primitives.StringValues.IsNullOrEmpty(response.Headers["Access-Control-Allow-Headers"]))
                    {
                        response.AppendHeader("Access-Control-Allow-Headers", crossDomain.Headers);
                    }
                }
                crossdomain();
            }
        }

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
        internal static Dictionary<string, Ashx> GetApiAction(Type type, bool isMin)// where T : new()
        {
            MethodInfo[] method = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            //List<MethodInfo> _method = method.ToList();
            if (method.Length < 1 || method == null)
            {
                return null;
            }
            Dictionary<string, Ashx> keyValues = new Dictionary<string, Ashx>(StringComparer.OrdinalIgnoreCase);

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
                else
                {
                    if (info.ReturnType != typeof(Task) && info.ReturnType != typeof(OnAshxEvent) && info.ReturnType != typeof(void))//.BaseType
                    {
                        //_method.Remove(info);
                        continue;
                    }
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
                    throw new ArgumentNullException("方法重名提示：", $"警告：对外调用方法不能存在重名的（异常方法名：{info.Name}）！");
                }
                else
                {
                    if (hobbyAttr == null)
                    {
                        hobbyAttr = new Ashx();
                    }

                    if (typeof(IAshxAction).IsAssignableFrom(type))
                    {
                        hobbyAttr.Action = new ActionDispatcher<IAshxAction>(info);

                        hobbyAttr.Parameters = GetParameter();

                        ApiParameter[] GetParameter()
                        {
                            List<ApiParameter> apis = new();
                            foreach (var item in hobbyAttr.Action.Parameters)
                            {
                                apis.Add(new ApiParameter(item, hobbyAttr.State));
                            }

                            return apis.ToArray();
                        }
                    }

                    if (cross != null)
                    {
                        hobbyAttr.CrossDomain = cross;
                    }
                    //hobbyAttr.IsTask = (info.ReturnType == typeof(Task) || info.ReturnType == typeof(OnAshxEvent) ? true : false);

                    //hobbyAttr.IsOnAshxEvent = (info.ReturnType == typeof(OnAshxEvent) ? true : false);

                    //hobbyAttr.Methods = info.Name;

                    //hobbyAttr.Pethod = GetMethod(info);

                    //hobbyAttr.Parameters = GetParameter(info);

                    //hobbyAttr.Method = info;

                    hobbyAttr.GetKeyAttributes(isMin);

                    string MethodsName = info.Name;

                    if (!string.IsNullOrWhiteSpace(hobbyAttr.ID))
                    {
                        if (keyValues.ContainsKey(hobbyAttr.ID))
                        {
                            throw new ArgumentNullException($"警告：对外调用方法不能存在重名的（异常方法名：{info.Name}）(对外访问名：{hobbyAttr.ID})！", "异常提示：");
                        }
                        MethodsName = hobbyAttr.ID;
                    }

                    keyValues.TryAdd(MethodsName, hobbyAttr);
                }
            }
            return keyValues;
        }

        /// <summary>
        /// 是否Min合法
        /// </summary>
        /// <returns></returns>
        internal static bool IsApiOut(Type ReturnType)
        {
            return typeof(IApiOut).IsAssignableFrom(ReturnType);
        }

        /// <summary>
        /// 是否异步合法
        /// </summary>
        /// <returns></returns>
        internal static bool IsTaskApiOut(Type ReturnType)
        {
            if (!object.Equals(ReturnType.BaseType, null) && object.Equals(ReturnType.BaseType, typeof(Task)))
            {
                if (ReturnType.GenericTypeArguments.Length > 0)
                {
                    return typeof(IApiOut).IsAssignableFrom(ReturnType.GenericTypeArguments[0]);
                }
            }
            return false;
        }

        internal static bool IsTaskIApiOut(MethodInfo method)
        {
            Type ReturnType = method.ReturnType;
            if (IsTaskApiOut(ReturnType))
            {
                Type isiApiOut = ReturnType.GenericTypeArguments[0];
                if (typeof(IApiOut).Equals(isiApiOut))
                {
                    return true;
                }
                else
                {
                    throw new ArgumentNullException("返回值类型规定提示：", $"警告：在使用异步返回值的情况下，强制采用（Task<IApiOut>）其他均视为违规操作。\n异常方法名：{method.DeclaringType.Name}.Task<{isiApiOut.Name}> {method.Name}()");
                }
            }
            return false;
        }

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

        //// <summary>
        //// 销毁类时，会调用析构函数
        //// </summary>
        //~AshxExtension()
        //{
        //    Dispose(false);
        //}

        //// <summary>
        //// 回收资源
        //// </summary>
        //public void Dispose()
        //{
        //    //asyncInvoke.Dispose();

        //    Dispose(true);
        //    //GC.Collect();

        //    //GC.SuppressFinalize(this);
        //    //GC.ReRegisterForFinalize(this);
        //    //Ashxes = null;
        //}

        //// <summary>
        //// 回收资源
        //// </summary>
        //// <param name="disposing"></param>
        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!disposing)
        //    {
        //        return;
        //    }
        //    Ashxes = null;
        //    //GC.Collect();
        //    GC.SuppressFinalize(this);
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

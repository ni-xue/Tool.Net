using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tool.Utils.Data;
using Tool.Web;

namespace Tool.Utils
{
    /// <summary>
    /// 各种验证类，包含正则表达式
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public sealed class Validate
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        private Validate()
        {
        }

        /// <summary>
        /// 根据 Agent 判断当前请求用户的设备名
        ///</summary>    
        ///<param name="UserAgent">平台信息，为空时，获取默认信息</param>
        ///<returns><see cref="UserSystem"/>枚举</returns>    
        public static UserSystem CheckAgent(string UserAgent = null)
        {
            string agent = UserAgent ?? HttpContextExtension.Current?.Request.Headers["User-Agent"];

            foreach (string name in Enum.GetNames(typeof(UserSystem)))
            {
                if (agent.Contains(name))
                {
                    return Enum.Parse(typeof(UserSystem), name).ToVar<UserSystem>();
                }
            }

            return UserSystem.Unknown;
        }

        /// <summary>
        /// 根据IP获取所在城市地区（该秘钥有dll内部提供，如有问题请联系dll开发者）（百度）
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>返回所在城市地区</returns>
        public static dynamic GetIpRegion(string ip)
        {
            return GetIpRegion(ip, "abDsBedrGw46lo1CyQuwZs9magjV5gSf");
        }

        /// <summary>
        /// 根据IP获取所在城市地区（百度）
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="ak">百度秘钥</param>
        /// <returns>返回所在城市地区</returns>
        public static dynamic GetIpRegion(string ip, string ak)
        {
            try
            {
                string url = $"http://api.map.baidu.com/location/ip?ak={ak}&ip={ip}";

                string reval = HttpHelpers.GetString(url); //Utility.GetWebContent(url);

                return reval.JsonDynamic();
            }
            catch (Exception e)
            {
                throw new Exception("方法出现异常，已无法获取所在城市地区信息，请联系管理员！", e);
            }
        }

        /// <summary>
        /// 根据IP获取所在城市地区（该秘钥有dll内部提供，如有问题请联系dll开发者）（高德）
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>返回所在城市地区</returns>
        public static dynamic GetGdIpRegion(string ip)
        {
            return GetGdIpRegion(ip, "f7c24979cb38edd738bb0bbaea139c90");
        }

        /// <summary>
        /// 根据IP获取所在城市地区（高德）
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="key">高德秘钥</param>
        /// <returns>返回所在城市地区</returns>
        public static dynamic GetGdIpRegion(string ip, string key)
        {
            try
            {
                string url = $"https://restapi.amap.com/v3/ip?ip={ip}&key={key}";

                string reval = HttpHelpers.GetString(url); //Utility.GetWebContent(url);

                return reval.JsonDynamic();
            }
            catch (Exception e)
            {
                throw new Exception("方法出现异常，已无法获取所在城市地区信息，请联系管理员！", e);
            }
        }

        /// <summary>
        /// 淘宝获取IP详细信息（不举建使用该API）
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>返回所在城市地区</returns>
        public static dynamic GetTbIpRegion(string ip)
        {
            try
            {
                string url = $"http://ip.taobao.com/service/getIpInfo.php?ip={ip}";//

                string reval = HttpHelpers.GetString(url); //Utility.GetWebContent(url);

                return reval.JsonDynamic();
            }
            catch (Exception e)
            {
                throw new Exception("方法出现异常，已无法获取所在城市地区信息，请联系管理员！", e);
            }
        }

        /// <summary>
        /// 根据银行卡获取信息
        /// </summary>
        /// <param name="cardNo">银行卡</param>
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public static CardDetail GetCardDetail(string cardNo)
        {
            string url = $"https://ccdcapi.alipay.com/validateAndCacheCardInfo.json?_input_charset=utf-8&cardNo={cardNo}&cardBinCheck=true";
            try
            {
                string reval = HttpHelpers.GetString(url); //Utility.GetWebContent(url);

                CardDetail cardDetail;

                Dictionary<string, object> str = reval.Json();
                if (bool.Parse(str["validated"].ToString()))
                {
                    cardDetail = new CardDetail(str["validated"].ToVar<bool>(), cardNo, str["bank"].ToString(), Getownparentbanks(str["bank"].ToString()), GetCardDetailimg(str["bank"].ToString()), str["cardType"].ToString(), "验证成功！");
                }
                else
                {
                    cardDetail = new CardDetail(str["validated"].ToVar<bool>(), "您输入银行卡不正确！");
                }

                return cardDetail;
            }
            catch (Exception e)
            {
                throw new Exception("银行验证出现问题了，请联系管理员进行修缮。", e);
            }

            /// <summary>
            /// 根据银行缩写,获得银行图片
            /// </summary>
            /// <param name="bank">银行编号</param>
            /// <returns>返回图片对象</returns>
            static Image GetCardDetailimg(string bank)
            {
                Image iSource = null;

                using (Stream stream = HttpHelpers.Get($"https://apimg.alipay.com/combo.png?d=cashier&t={bank}"))
                {
                    iSource = Image.FromStream(stream);
                }
                return iSource;
            }
        }

        /// <summary>
        /// 根据银行缩写,获得银行名称
        /// </summary>
        /// <param name="bank">银行编号</param>
        /// <returns>返回银行名称</returns>
        private static string Getownparentbanks(string bank)
        {
            string JSON = "{\"SRCB\": \"深圳农村商业银行\",\"BGB\": \"广西北部湾银行\",\"SHRCB\": \"上海农村商业银行\",\"BJBANK\": \"北京银行\",\"WHCCB\": \"威海市商业银行\"," +
                "\"BOZK\": \"周口银行\",\"KORLABANK\": \"库尔勒市商业银行\",\"SPABANK\": \"平安银行\",\"SDEB\": \"顺德农商银行\",\"HURCB\": \"湖北省农村信用社\"," +
                "\"WRCB\": \"无锡农村商业银行\",\"BOCY\": \"朝阳银行\",\"CZBANK\": \"浙商银行\",\"HDBANK\": \"邯郸银行\",\"BOC\": \"中国银行\",\"BOD\": \"东莞银行\"," +
                "\"CCB\": \"中国建设银行\",\"ZYCBANK\": \"遵义市商业银行\",\"SXCB\": \"绍兴银行\",\"GZRCU\": \"贵州省农村信用社\",\"ZJKCCB\": \"张家口市商业银行\"," +
                "\"BOJZ\": \"锦州银行\",\"BOP\": \"平顶山银行\",\"HKB\": \"汉口银行\",\"SPDB\": \"上海浦东发展银行\",\"NXRCU\": \"宁夏黄河农村商业银行\"," +
                "\"NYNB\": \"广东南粤银行\",\"GRCB\": \"广州农商银行\",\"BOSZ\": \"苏州银行\",\"HZCB\": \"杭州银行\",\"HSBK\": \"衡水银行\",\"HBC\": \"湖北银行\"," +
                "\"JXBANK\": \"嘉兴银行\",\"HRXJB\": \"华融湘江银行\",\"BODD\": \"丹东银行\",\"AYCB\": \"安阳银行\",\"EGBANK\": \"恒丰银行\",\"CDB\": \"国家开发银行\"," +
                "\"TCRCB\": \"江苏太仓农村商业银行\",\"NJCB\": \"南京银行\",\"ZZBANK\": \"郑州银行\",\"DYCB\": \"德阳商业银行\",\"YBCCB\": \"宜宾市商业银行\"," +
                "\"SCRCU\": \"四川省农村信用\",\"KLB\": \"昆仑银行\",\"LSBANK\": \"莱商银行\",\"YDRCB\": \"尧都农商行\",\"CCQTGB\": \"重庆三峡银行\",\"FDB\": \"富滇银行\"," +
                "\"JSRCU\": \"江苏省农村信用联合社\",\"JNBANK\": \"济宁银行\",\"CMB\": \"招商银行\",\"JINCHB\": \"晋城银行JCBANK\",\"FXCB\": \"阜新银行\"," +
                "\"WHRCB\": \"武汉农村商业银行\",\"HBYCBANK\": \"湖北银行宜昌分行\",\"TZCB\": \"台州银行\",\"TACCB\": \"泰安市商业银行\",\"XCYH\": \"许昌银行\"," +
                "\"CEB\": \"中国光大银行\",\"NXBANK\": \"宁夏银行\",\"HSBANK\": \"徽商银行\",\"JJBANK\": \"九江银行\",\"NHQS\": \"农信银清算中心\"," +
                "\"MTBANK\": \"浙江民泰商业银行\",\"LANGFB\": \"廊坊银行\",\"ASCB\": \"鞍山银行\",\"KSRB\": \"昆山农村商业银行\",\"YXCCB\": \"玉溪市商业银行\"," +
                "\"DLB\": \"大连银行\",\"DRCBCL\": \"东莞农村商业银行\",\"GCB\": \"广州银行\",\"NBBANK\": \"宁波银行\",\"BOYK\": \"营口银行\",\"SXRCCU\": \"陕西信合\"," +
                "\"GLBANK\": \"桂林银行\",\"BOQH\": \"青海银行\",\"CDRCB\": \"成都农商银行\",\"QDCCB\": \"青岛银行\",\"HKBEA\": \"东亚银行\"," +
                "\"HBHSBANK\": \"湖北银行黄石分行\",\"WZCB\": \"温州银行\",\"TRCB\": \"天津农商银行\",\"QLBANK\": \"齐鲁银行\",\"GDRCC\": \"广东省农村信用社联合社\"," +
                "\"ZJTLCB\": \"浙江泰隆商业银行\",\"GZB\": \"赣州银行\",\"GYCB\": \"贵阳市商业银行\",\"CQBANK\": \"重庆银行\",\"DAQINGB\": \"龙江银行\"," +
                "\"CGNB\": \"南充市商业银行\",\"SCCB\": \"三门峡银行\",\"CSRCB\": \"常熟农村商业银行\",\"SHBANK\": \"上海银行\",\"JLBANK\": \"吉林银行\"," +
                "\"CZRCB\": \"常州农村信用联社\",\"BANKWF\": \"潍坊银行\",\"ZRCBANK\": \"张家港农村商业银行\",\"FJHXBC\": \"福建海峡银行\"," +
                "\"ZJNX\": \"浙江省农村信用社联合社\",\"LZYH\": \"兰州银行\",\"JSB\": \"晋商银行\",\"BOHAIB\": \"渤海银行\",\"CZCB\": \"浙江稠州商业银行\"," +
                "\"YQCCB\": \"阳泉银行\",\"SJBANK\": \"盛京银行\",\"XABANK\": \"西安银行\",\"BSB\": \"包商银行\",\"JSBANK\": \"江苏银行\",\"FSCB\": \"抚顺银行\"," +
                "\"HNRCU\": \"河南省农村信用\",\"COMM\": \"交通银行\",\"XTB\": \"邢台银行\",\"CITIC\": \"中信银行\",\"HXBANK\": \"华夏银行\"," +
                "\"HNRCC\": \"湖南省农村信用社\",\"DYCCB\": \"东营市商业银行\",\"ORBANK\": \"鄂尔多斯银行\",\"BJRCB\": \"北京农村商业银行\",\"XYBANK\": \"信阳银行\"," +
                "\"ZGCCB\": \"自贡市商业银行\",\"CDCB\": \"成都银行\",\"HANABANK\": \"韩亚银行\",\"CMBC\": \"中国民生银行\",\"LYBANK\": \"洛阳银行\"," +
                "\"GDB\": \"广东发展银行\",\"ZBCB\": \"齐商银行\",\"CBKF\": \"开封市商业银行\",\"H3CB\": \"内蒙古银行\",\"CIB\": \"兴业银行\"," +
                "\"CRCBANK\": \"重庆农村商业银行\",\"SZSBK\": \"石嘴山银行\",\"DZBANK\": \"德州银行\",\"SRBANK\": \"上饶银行\",\"LSCCB\": \"乐山市商业银行\"," +
                "\"JXRCU\": \"江西省农村信用\",\"ICBC\": \"中国工商银行\",\"JZBANK\": \"晋中市商业银行\",\"HZCCB\": \"湖州市商业银行\",\"NHB\": \"南海农村信用联社\"," +
                "\"XXBANK\": \"新乡银行\",\"JRCB\": \"江苏江阴农村商业银行\",\"YNRCC\": \"云南省农村信用社\",\"ABC\": \"中国农业银行\",\"GXRCU\": \"广西省农村信用\"," +
                "\"PSBC\": \"中国邮政储蓄银行\",\"BZMD\": \"驻马店银行\",\"ARCU\": \"安徽省农村信用社\",\"GSRCU\": \"甘肃省农村信用\",\"LYCB\": \"辽阳市商业银行\"," +
                "\"JLRCU\": \"吉林农信\",\"URMQCCB\": \"乌鲁木齐市商业银行\",\"XLBANK\": \"中山小榄村镇银行\",\"CSCB\": \"长沙银行\",\"JHBANK\": \"金华银行\"," +
                "\"BHB\": \"河北银行\",\"NBYZ\": \"鄞州银行\",\"LSBC\": \"临商银行\",\"BOCD\": \"承德银行\",\"SDRCU\": \"山东农信\",\"NCB\": \"南昌银行\"," +
                "\"TCCB\": \"天津银行\",\"WJRCB\": \"吴江农商银行\",\"CBBQS\": \"城市商业银行资金清算中心\",\"HBRCU\": \"河北省农村信用社\"}";
            Dictionary<string, object> str = JSON.Json();
            return str[bank].ToString();
        }

        /// <summary>
        /// 银行卡信息
        /// </summary>
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public class CardDetail
        {
            /// <summary>
            /// 有参构造 验证成功时使用
            /// </summary>
            /// <param name="Validated">银行卡验证结果</param>
            /// <param name="CardNumber">银行卡卡号</param>
            /// <param name="Bank">银行标记名</param>
            /// <param name="BankName">银行名称</param>
            /// <param name="BankImage">银行图片</param>
            /// <param name="CardType">银行卡类型编号</param>
            /// <param name="VerificationResult">验证结果</param>
            public CardDetail(bool Validated, string CardNumber, string Bank, string BankName, Image BankImage, string CardType, string VerificationResult)
            {
                this.Validated = Validated;
                this.CardNumber = CardNumber;
                this.Bank = Bank;
                this.BankName = BankName;
                this.BankImage = BankImage;

                using (MemoryStream ms = new())
                {
                    BankImage.Save(ms, BankImage.RawFormat);
                    this.BankImagebytes = ms.ToArray();
                }

                this.CardType = CardType;
                this.VerificationResult = VerificationResult;
            }

            /// <summary>
            /// 有参构造 验证失败时使用
            /// </summary>
            /// <param name="Validated"></param>
            /// <param name="VerificationResult"></param>
            public CardDetail(bool Validated, string VerificationResult)
            {
                this.Validated = Validated;
                this.VerificationResult = VerificationResult;
            }

            /// <summary>
            /// 银行卡验证结果
            /// </summary>
            public bool Validated { get; }

            /// <summary>
            /// 银行卡卡号
            /// </summary>
            public string CardNumber { get; }

            /// <summary>
            /// 银行标记名
            /// </summary>
            public string Bank { get; }

            /// <summary>
            /// 银行名称
            /// </summary>
            public string BankName { get; }

            /// <summary>
            /// 银行图片
            /// </summary>
            public Image BankImage { get; }

            /// <summary>
            /// 银行图片字节流
            /// </summary>
            public byte[] BankImagebytes { get; }

            /// <summary>
            /// 银行卡类型编号
            /// </summary>
            public string CardType { get; }

            /// <summary>
            /// 银行卡类型
            /// </summary>
            public string CardTypeName { get { return CardType.Equals("DC") ? "借记卡" : CardType.Equals("CC") ? "信用卡" : "无法识别"; } }

            /// <summary>
            /// 验证结果
            /// </summary>
            public string VerificationResult { get; }
        }

        /// <summary>
        /// 判断<see cref="DataSet"/>对象中的是否为空，行为空，表为空，对象为空
        /// </summary>
        /// <param name="ds"><see cref="DataSet"/>对象</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool CheckedDataSet(DataSet ds)
        {
            return !ds.IsEmpty();
        }

        /// <summary>
        /// 判断<see cref="DataTable"/>对象中的是否为空，行为空，对象为空
        /// </summary>
        /// <param name="dt"><see cref="DataTable"/>对象</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool CheckedDataTable(DataTable dt)
        {
            return !dt.IsEmpty();
        }

        /// <summary>
        /// 判断<see cref="DataRow"/>对象中的是否为空，行为空，对象为空
        /// </summary>
        /// <param name="dr"><see cref="DataRow"/>对象</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool CheckedDataRow(DataRow dr)
        {
            return !dr.IsEmpty();
        }

        /// <summary>
        /// 判断<see cref="Array"/>对象中的是否为空
        /// </summary>
        /// <param name="obj">数组</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool CheckedObjcetArray(object[] obj)
        {
            return !object.Equals(obj, null) && obj.Length != 0 && !object.Equals(obj[0], null);
        }

        /// <summary>
        /// 判断是否是 Base64 格式的字符串
        /// </summary>
        /// <param name="expression">字符串</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool IsBase64String(string expression)
        {
            return Regex.IsMatch(expression, "[A-Za-z0-9\\+\\/\\=]");
        }

        /// <summary>
        /// 判断是否是 Char 类型数据
        /// </summary>
        /// <param name="expression">字符串</param>
        /// <returns></returns>
        public static bool IsCnChar(string expression)
        {
            return Regex.IsMatch(expression, "^(?:[一-龥])+$");
        }

        /// <summary>
        /// 判断是否是 Char 类型数据
        /// </summary>
        /// <param name="expression">字符串</param>
        /// <returns></returns>
        public static bool IsCnCharAndWordAndNum(string expression)
        {
            return Regex.IsMatch(expression, "^[0-9a-zA-Z一-龥]+$");
        }

        /// <summary>
        /// 判断是否是时间类型数据
        /// </summary>
        /// <param name="dateval">字符串</param>
        /// <returns></returns>
        public static bool IsDate(string dateval)
        {
            return Regex.IsMatch(dateval, "^((((1[6-9]|[2-9]\\d)\\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\\d|3[01]))|(((1[6-9]|[2-9]\\d)\\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\\d|30))|(((1[6-9]|[2-9]\\d)\\d{2})-0?2-(0?[1-9]|1\\d|2[0-8]))|(((1[6-9]|[2-9]\\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$", RegexOptions.Compiled);
        }

        /// <summary>
        /// 判断是否是十进制分数
        /// </summary>
        /// <param name="expression">字符串</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool IsDecimalFraction(string expression)
        {
            return Regex.IsMatch(expression, "^([0-9]{1,10})\\.([0-9]{1,10})$", RegexOptions.Compiled);
        }

        /// <summary>
        /// 判断是否是电子邮件格式
        /// </summary>
        /// <param name="strEmail">字符串</param>
        /// <returns></returns>
        public static bool IsDoEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, "^@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([\\w-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$");
        }

        /// <summary>
        /// 判断是否是域名格式
        /// </summary>
        /// <param name="strHost"></param>
        /// <returns></returns>
        public static bool IsDomain(string strHost)
        {
            //Regex regex = new Regex("^\\d+$");
            return strHost.IndexOf(".") != -1 && !Regex.IsMatch(strHost.Replace(".", string.Empty), "^\\d+$");
        }

        /// <summary>
        /// 判断是否是Double格式
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object expression)
        {
            return expression != null && Regex.IsMatch(expression.ToString(), "^([0-9])[0-9]*(\\.\\w*)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// 判断值是不是邮箱格式
        /// </summary>
        /// <param name="strEmail">判断值</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool IsEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, "^([\\w-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([\\w-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$");
        }

        /// <summary>
        /// 判断值是不是文件名
        /// </summary>
        /// <param name="filename">判断值</param>
        /// <returns></returns>
        public static bool IsFileName(string filename)
        {
            return !Regex.IsMatch(filename, "[<>/\";#$*%]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// 判断值是不是合法的身份证
        /// </summary>
        /// <param name="strIDCard">判断值</param>
        /// <returns></returns>
        public static bool IsIDCard(string strIDCard)
        {
            if (string.IsNullOrEmpty(strIDCard))
            {
                return false;
            }
            if (strIDCard.Length != 15 && strIDCard.Length != 18)
            {
                return false;
            }
            Regex regex;
            string[] array;
            bool result;
            if (strIDCard.Length == 15)
            {
                regex = new Regex("^(\\d{6})(\\d{2})(\\d{2})(\\d{2})(\\d{3})$");
                if (!regex.Match(strIDCard).Success)
                {
                    return false;
                }
                array = regex.Split(strIDCard);
                try
                {
                    new DateTime(int.Parse("19" + array[2]), int.Parse(array[3]), int.Parse(array[4]));
                    result = true;
                    return result;
                }
                catch
                {
                    result = false;
                    return result;
                }
            }
            regex = new Regex("^(\\d{6})(\\d{4})(\\d{2})(\\d{2})(\\d{3})([0-9Xx])$");
            if (!regex.Match(strIDCard).Success)
            {
                return false;
            }
            array = regex.Split(strIDCard);
            try
            {
                new DateTime(int.Parse(array[2]), int.Parse(array[3]), int.Parse(array[4]));
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 判断值是不是图像
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <returns></returns>
        public static bool IsImage(string filename)
        {
            return Regex.IsMatch(filename, "\\.(gif|jpg|bmp|png|jpeg)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// 验证IP地址是否合法
        /// </summary>
        /// <param name="ipval">待验证的IP</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool IsIP(string ipval)
        {
            return Regex.IsMatch(ipval, "^((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)$");
        }

        /// <summary>
        /// 是不是有效的IP和端口
        /// </summary>
        /// <param name="ipval">判断值</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool IsIPAndPort(string ipval)
        {
            return Regex.IsMatch(ipval, "^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9]),\\d{1,5}?$");
        }

        /// <summary>
        /// 是不是有效的IP
        /// </summary>
        /// <param name="ipval">判断值</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool IsIPSect(string ipval)
        {
            return Regex.IsMatch(ipval, "^((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){2}((2[0-4]\\d|25[0-5]|[01]?\\d\\d?|\\*)\\.)(2[0-4]\\d|25[0-5]|[01]?\\d\\d?|\\*)$");
        }

        /// <summary>
        /// 是不是长日期
        /// </summary>
        /// <param name="dateval">字符串</param>
        /// <returns></returns>
        public static bool IsLongDate(string dateval)
        {
            return Regex.IsMatch(dateval, "^((((1[6-9]|[2-9]\\d)\\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\\d|3[01]))|(((1[6-9]|[2-9]\\d)\\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\\d|30))|(((1[6-9]|[2-9]\\d)\\d{2})-0?2-(0?[1-9]|1\\d|2[0-8]))|(((1[6-9]|[2-9]\\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\\d):[0-5]?\\d:[0-5]?\\d$", RegexOptions.Compiled);
        }

        /// <summary>
        /// 是不是移动电话号码
        /// </summary>
        /// <param name="strMobile">字符串</param>
        /// <returns></returns>
        public static bool IsMobileCode(string strMobile)
        {
            return Regex.IsMatch(strMobile, "^13|15|18\\d{9}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// s是不是负整数
        /// </summary>
        /// <param name="expression">字符串</param>
        /// <returns></returns>
        public static bool IsNegativeInt(string expression)
        {
            return Regex.IsMatch(expression, "^-\\d+$", RegexOptions.Compiled) && long.Parse(expression) >= -2147483648L;
        }

        /// <summary>
        /// 是否是昵称
        /// </summary>
        /// <param name="strVal">字符串</param>
        /// <returns></returns>
        public static bool IsNickName(string strVal)
        {
            return Regex.IsMatch(strVal, "^[a-zA-Z\\u4e00-\\u9fa5\\d_]+$", RegexOptions.Compiled);
        }

        /// <summary>
        /// 判断expVal里面是否有值
        /// </summary>
        /// <param name="expVal">判断值</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool IsNotNull(object expVal)
        {
            return !Validate.IsNull(expVal);
        }

        /// <summary>
        /// 判断expVal里面是否为空
        /// </summary>
        /// <param name="expVal">判断值</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool IsNull(object expVal)
        {
            if (expVal == null)
            {
                return true;
            }
            string name = expVal.GetType().Name;
            if (name != null && name == "String[]")
            {
                string[] array = (string[])expVal;
                return array.Length == 0;
            }
            string text = expVal.ToString();
            return text == null || text == "";
        }

        /// <summary>
        /// 判断是不是数字
        /// </summary>
        /// <param name="expression">判断值</param>
        /// <returns>返回<see cref="bool"/>类型</returns>
        public static bool IsNumeric(object expression)
        {
            if (expression != null)
            {
                string text = expression.ToString();
                if (text.Length > 0 && text.Length <= 11 && Regex.IsMatch(text, "^[-]?[0-9]*[.]?[0-9]*$", RegexOptions.Compiled) && (text.Length < 10 || (text.Length == 10 && text[0] == '1') || (text.Length == 11 && text[0] == '-' && text[1] == '1')))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 验证这个string数组是否全是可以强转为int的对象
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns></returns>
        public static bool IsNumericArray(string[] strNumber)
        {
            if (strNumber == null)
            {
                return false;
            }
            if (strNumber.Length < 1)
            {
                return false;
            }
            for (int i = 0; i < strNumber.Length; i++)
            {
                string expression = strNumber[i];
                if (!Validate.IsNumeric(expression))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 验证是否是合格的电话号码
        /// </summary>
        /// <param name="strPhone">号码</param>
        /// <returns></returns>
        public static bool IsPhoneCode(string strPhone)
        {
            return Regex.IsMatch(strPhone, "^(86)?(-)?(0\\d{2,3})?(-)?(\\d{7,8})(-)?(\\d{3,5})?$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证是否是物理路径
        /// </summary>
        /// <param name="s">路径</param>
        /// <returns></returns>
        public static bool IsPhysicalPath(string s)
        {
            string pattern = "^\\s*[a-zA-Z]:.*$";
            return Regex.IsMatch(s, pattern);
        }

        /// <summary>
        /// 验证是否是正整数
        /// </summary>
        /// <param name="expression">字符串</param>
        /// <returns></returns>
        public static bool IsPositiveInt(string expression)
        {
            return Regex.IsMatch(expression, "^\\d+$", RegexOptions.Compiled) && long.Parse(expression) <= 2147483647L;
        }

        /// <summary>
        /// 验证是否是正整数64位的
        /// </summary>
        /// <param name="expression">字符串</param>
        /// <returns></returns>
        public static bool IsPositiveInt64(string expression)
        {
            return Regex.IsMatch(expression, "^\\d+$", RegexOptions.Compiled) && long.Parse(expression) <= 9223372036854775807L;
        }

        /// <summary>
        /// 验证是否是邮政编码
        /// </summary>
        /// <param name="strPostalCode">字符串</param>
        /// <returns></returns>
        public static bool IsPostalCode(string strPostalCode)
        {
            return Regex.IsMatch(strPostalCode, "^\\d{6}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证是否是相对路径
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns></returns>
        public static bool IsRelativePath(string s)
        {
            return s != null && !(s == string.Empty) && !s.StartsWith("/") && !s.StartsWith("?") && !Regex.IsMatch(s, "^\\s*[a-zA-Z]{1,10}:.*$");
        }

        /// <summary>
        /// 验证是否是安全的输入词
        /// </summary>
        /// <param name="expression">字符串</param>
        /// <returns></returns>
        public static bool IsSafeInputWords(string expression)
        {
            return Regex.IsMatch(expression, "/^\\s*$|^c:\\\\con\\\\con$|[%,\\*\"\\s\\t\\<\\>\\&]|$guestexp/is");
        }

        /// <summary>
        /// 验证是否是安全的Sql字符串
        /// </summary>
        /// <param name="expression">字符串</param>
        /// <returns></returns>
        public static bool IsSafeSqlString(string expression)
        {
            return !Regex.IsMatch(expression, "[-|;|,|\\/|\\(|\\)|\\[|\\]|\\}|\\{|%|@|\\*|!|\\']");
        }

        /// <summary>
        /// 验证是否是安全的Sql字符串
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns></returns>
        public static bool IsSafety(string s)
        {
            string input = Regex.Replace(s.Replace("%20", " "), "\\s", " ");
            string pattern = "select |insert |delete from |count\\(|drop table|update |truncate |asc\\(|mid\\(|char\\(|xp_cmdshell|exec master|net localgroup administrators|:|net user|\"|\\'| or ";
            return !Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证字符串是不是短日期
        /// </summary>
        /// <param name="dateval">要检测的字符串</param>
        /// <returns>返回结果</returns>
        public static bool IsShortDate(string dateval)
        {
            return Regex.IsMatch(dateval, "^((((1[6-9]|[2-9]\\d)\\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\\d|3[01]))|(((1[6-9]|[2-9]\\d)\\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\\d|30))|(((1[6-9]|[2-9]\\d)\\d{2})-0?2-(0?[1-9]|1\\d|2[0-8]))|(((1[6-9]|[2-9]\\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$", RegexOptions.Compiled);
        }

        /// <summary>
        ///  检测字符串是不是指定单词和数字
        /// </summary>
        /// <param name="expression">要检测的字符串</param>
        /// <param name="start">开始下标</param>
        /// <param name="end">结束下标</param>
        /// <returns>返回结果</returns>
        public static bool IsSpecifyWordAndNum(string expression, int start, int end)
        {
            return !string.IsNullOrEmpty(expression) && start <= end && Regex.IsMatch(expression, string.Format("^[0-9a-zA-Z]{{0},{1}}$", start, end));
        }

        /// <summary>
        /// 检测字符串中是否包含SQL注入
        /// </summary>
        /// <param name="sqlExpression">要检测的字符串</param>
        /// <returns>返回结果</returns>
        public static bool IsSQL(string sqlExpression)
        {
            return Regex.IsMatch(sqlExpression, "\\?|select%20|select\\s+|insert%20|insert\\s+|delete%20|delete\\s+|count\\(|drop%20|drop\\s+|update%20|update\\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// 验证字符串是不是时间格式
        /// </summary>
        /// <param name="timeval">要检测的字符串</param>
        /// <returns>返回结果</returns>
        public static bool IsTime(string timeval)
        {
            return Regex.IsMatch(timeval, "^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$");
        }

        /// <summary>
        /// 验证字符串是不是Unicode编码格式
        /// </summary>
        /// <param name="s">要检测的字符串</param>
        /// <returns>返回结果</returns>
        public static bool IsUnicode(string s)
        {
            string pattern = "^[\\u4E00-\\u9FA5\\uE815-\\uFA29]+$";
            return Regex.IsMatch(s, pattern);
        }

        /// <summary>
        /// 验证字符串是不是有效的Url链接
        /// </summary>
        /// <param name="strUrl">要检测的字符串</param>
        /// <returns>返回结果</returns>
        public static bool IsURL(string strUrl)
        {
            return Regex.IsMatch(strUrl, "^(http|https)\\://([a-zA-Z0-9\\.\\-]+(\\:[a-zA-Z0-9\\.&%\\$\\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\\-]+\\.)*[a-zA-Z0-9\\-]+\\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\\:[0-9]+)*(/($|[a-zA-Z0-9\\.\\,\\?\\'\\\\\\+&%\\$#\\=~_\\-]+))*$");
        }

        /// <summary>
        /// 验证字符串是不是中文名称
        /// </summary>
        /// <param name="strVal">要检测的字符串</param>
        /// <returns>返回结果</returns>
        public static bool IsUserName(string strVal)
        {
            return Regex.IsMatch(strVal, "^[a-zA-Z\\d_]+$", RegexOptions.Compiled);
        }

        /// <summary>
        /// 检测字符串是不是单词和数字
        /// </summary>
        /// <param name="expression">要检测的字符串</param>
        /// <returns>返回结果</returns>
        public static bool IsWordAndNum(string expression)
        {
            return Regex.IsMatch(expression, "[0-9a-zA-Z]?");
        }

        //private static readonly Regex regex_ImgFormat = new Regex("\\.(gif|jpg|bmp|png|jpeg)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        //private static readonly Regex regex_IsDate = new Regex("^((((1[6-9]|[2-9]\\d)\\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\\d|3[01]))|(((1[6-9]|[2-9]\\d)\\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\\d|30))|(((1[6-9]|[2-9]\\d)\\d{2})-0?2-(0?[1-9]|1\\d|2[0-8]))|(((1[6-9]|[2-9]\\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$", RegexOptions.Compiled);

        //private static readonly Regex regex_IsDecimalFraction = new Regex("^([0-9]{1,10})\\.([0-9]{1,10})$", RegexOptions.Compiled);

        //private static readonly Regex regex_IsDouble = new Regex("^([0-9])[0-9]*(\\.\\w*)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        //private static readonly Regex regex_IsLongDate = new Regex("^((((1[6-9]|[2-9]\\d)\\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\\d|3[01]))|(((1[6-9]|[2-9]\\d)\\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\\d|30))|(((1[6-9]|[2-9]\\d)\\d{2})-0?2-(0?[1-9]|1\\d|2[0-8]))|(((1[6-9]|[2-9]\\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\\d):[0-5]?\\d:[0-5]?\\d$", RegexOptions.Compiled);

        //private static readonly Regex regex_IsNegativeInt = new Regex("^-\\d+$", RegexOptions.Compiled);

        //private static readonly Regex regex_IsNickName = new Regex("^[a-zA-Z\\u4e00-\\u9fa5\\d_]+$", RegexOptions.Compiled);

        //private static readonly Regex regex_IsNumeric = new Regex("^[-]?[0-9]*[.]?[0-9]*$", RegexOptions.Compiled);

        //private static readonly Regex regex_IsPositiveInt = new Regex("^\\d+$", RegexOptions.Compiled);

        //private static readonly Regex regex_IsShortDate = new Regex("^((((1[6-9]|[2-9]\\d)\\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\\d|3[01]))|(((1[6-9]|[2-9]\\d)\\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\\d|30))|(((1[6-9]|[2-9]\\d)\\d{2})-0?2-(0?[1-9]|1\\d|2[0-8]))|(((1[6-9]|[2-9]\\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$", RegexOptions.Compiled);

        //private static readonly Regex regex_IsUserName = new Regex("^[a-zA-Z\\d_]+$", RegexOptions.Compiled);

        //private static readonly Regex regex_SqlFormat = new Regex("\\?|select%20|select\\s+|insert%20|insert\\s+|delete%20|delete\\s+|count\\(|drop%20|drop\\s+|update%20|update\\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    ///<summary>    
    /// 根据 Agent 判断当前请求用户的设备名 枚举 
    ///</summary>    
    public enum UserSystem
    {
        /// <summary>
        /// 安卓系统
        /// </summary>
        Android,
        /// <summary>
        /// 苹果系统
        /// </summary>
        iPhone,
        /// <summary>
        /// iPod是苹果公司设计和销售的系列便携式多功能数字多媒体播放器。
        /// </summary>
        iPod,
        /// <summary>
        /// 苹果平板
        /// </summary>
        iPad,
        /// <summary>
        /// Windows Phone(简称为WP)是微软于2010年10月21日正式发布的一款手机操作系统
        /// </summary>
        Windows_Phone,
        /// <summary>
        /// QQ、微信、QQ浏览器
        /// </summary>
        MQQBrowser,
        /// <summary>
        /// Windows NT是纯32,64位操作系统
        /// </summary>
        Windows,
        /// <summary>
        /// 简介：麦金塔电脑（Macintosh，简称Mac，香港俗称Mac机，大陆亦有人称作苹果机或麦金塔电脑），是苹果电脑其中一系列的个人电脑。
        /// </summary>
        Macintosh,
        /// <summary>
        /// 未知操作设备
        /// </summary>
        Unknown
    }
}

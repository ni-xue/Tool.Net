using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Sockets.TcpFrame
{
    /*
         核心协定密码用于提高，包的质量和安全性。
             */
    internal class CoreCode
    {
        internal readonly static Dictionary<char, byte> CodeStrKeys = new Dictionary<char, byte>()
            {
                {'A' , 255 },
                {'a' , 254 },
                {'B' , 253 },
                {'b' , 252 },
                {'C' , 251 },
                {'c' , 250 },
                {'D' , 249 },
                {'d' , 248 },
                {'E' , 247 },
                {'e' , 246 },
                {'F' , 245 },
                {'f' , 244 },
                {'G' , 243 },
                {'g' , 242 },
                {'H' , 241 },
                {'h' , 240 },
                {'I' , 239 },
                {'i' , 238 },
                {'J' , 237 },
                {'j' , 236 },
                {'K' , 235 },
                {'k' , 234 },
                {'L' , 233 },
                {'l' , 232 },
                {'M' , 231 },
                {'m' , 230 },
                {'N' , 229 },
                {'n' , 228 },
                {'O' , 227 },
                {'o' , 226 },
                {'P' , 225 },
                {'p' , 224 },
                {'Q' , 223 },
                {'q' , 222 },
                {'R' , 221 },
                {'r' , 220 },
                {'S' , 219 },
                {'s' , 218 },
                {'T' , 217 },
                {'t' , 216 },
                {'U' , 215 },
                {'u' , 214 },
                {'V' , 213 },
                {'v' , 212 },
                {'W' , 211 },
                {'w' , 210 },
                {'X' , 209 },
                {'x' , 208 },
                {'Y' , 207 },
                {'y' , 206 },
                {'Z' , 205 },
                {'z' , 204 },
                {'0' , 203 },
                {'1' , 202 },
                {'2' , 201 },
                {'3' , 200 },
                {'4' , 199 },
                {'5' , 198 },
                {'6' , 197 },
                {'7' , 196 },
                {'8' , 195 },
                {'9' , 194 },
                {'[' , 193 },
                {']' , 192 },
                {'#' , 191 },
                {'&' , 190 },
                {'@' , 189 },
                {'.' , 188 },
                {'/' , 187 },
            };

        internal readonly static Dictionary<byte, char> CodeIntKeys = new Dictionary<byte, char>()
            {
                {255 , 'A' },
                {254 , 'a' },
                {253 , 'B' },
                {252 , 'b' },
                {251 , 'C' },
                {250 , 'c' },
                {249 , 'D' },
                {248 , 'd' },
                {247 , 'E' },
                {246 , 'e' },
                {245 , 'F' },
                {244 , 'f' },
                {243 , 'G' },
                {242 , 'g' },
                {241 , 'H' },
                {240 , 'h' },
                {239 , 'I' },
                {238 , 'i' },
                {237 , 'J' },
                {236 , 'j' },
                {235 , 'K' },
                {234 , 'k' },
                {233 , 'L' },
                {232 , 'l' },
                {231 , 'M' },
                {230 , 'm' },
                {229 , 'N' },
                {228 , 'n' },
                {227 , 'O' },
                {226 , 'o' },
                {225 , 'P' },
                {224 , 'p' },
                {223 , 'Q' },
                {222 , 'q' },
                {221 , 'R' },
                {220 , 'r' },
                {219 , 'S' },
                {218 , 's' },
                {217 , 'T' },
                {216 , 't' },
                {215 , 'U' },
                {214 , 'u' },
                {213 , 'V' },
                {212 , 'v' },
                {211 , 'W' },
                {210 , 'w' },
                {209 , 'X' },
                {208 , 'x' },
                {207 , 'Y' },
                {206 , 'y' },
                {205 , 'Z' },
                {204 , 'z' },
                {203 , '0' },
                {202 , '1' },
                {201 , '2' },
                {200 , '3' },
                {199 , '4' },
                {198 , '5' },
                {197 , '6' },
                {196 , '7' },
                {195 , '8' },
                {194 , '9' },
                {193 , '[' },
                {192 , ']' },
                {191 , '#' },
                {190 , '&' },
                {189 , '@' },
                {188 , '.' },
                {187 , '/' },
            };

        internal static char CodeInt(byte by)
        {
            try
            {
                return CodeIntKeys[by];
            }
            catch
            {
                return '-';
            }
            //switch (by)
            //{
            //    case 203:
            //        return '0';
            //    case 202:
            //        return '1';
            //    case 201:
            //        return '2';
            //    case 200:
            //        return '3';
            //    case 199:
            //        return '4';
            //    case 198:
            //        return '5';
            //    case 197:
            //        return '6';
            //    case 196:
            //        return '7';
            //    case 195:
            //        return '8';
            //    case 194:
            //        return '9';
            //    case 193:
            //        return '[';
            //    case 192:
            //        return ']';
            //    case 191:
            //        return '#';
            //    case 190:
            //        return '&';
            //    case 189:
            //        return '@';
            //    case 188:
            //        return '.';
            //    case 187:
            //        return '/';
            //    default:
            //        string str = Enum.GetName(typeof(TcpCode), by);
            //        return str == null ? '-' : str[0];
            //}
        }

        internal static byte CodeStr(char by)
        {
            try
            {
                return CodeStrKeys[by];
            }
            catch
            {
                return 0;
            }
            //switch (by)
            //{
            //    case '0':
            //        return 203;
            //    case '1':
            //        return 202;
            //    case '2':
            //        return 201;
            //    case '3':
            //        return 200;
            //    case '4':
            //        return 199;
            //    case '5':
            //        return 198;
            //    case '6':
            //        return 197;
            //    case '7':
            //        return 196;
            //    case '8':
            //        return 195;
            //    case '9':
            //        return 194;
            //    case '[':
            //        return 193;
            //    case ']':
            //        return 192;
            //    case '#':
            //        return 191;
            //    case '&':
            //        return 190;
            //    case '@':
            //        return 189;
            //    case '.':
            //        return 188;
            //    case '/':
            //        return 187;
            //    default:
            //        //byte str = (byte)Enum.ToObject(typeof(TcpCode), by);
            //        if (Enum.TryParse<TcpCode>(by.ToString(), false, out TcpCode code))
            //        {
            //            return (byte)code;
            //        }
            //        else
            //        {
            //            return 0;
            //        }
            //}
        }

        internal static byte[] GetBytes(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            int i = 0;
            int length = data.Length;
            byte[] Baotou = new byte[length];

            unsafe
            {
                fixed (byte* bytes = Baotou)
                {
                    while (i < length)
                    {
                        bytes[i] = CoreCode.CodeStr(data[i]);
                        i++;
                    }
                }
            }

            //for (i = 0; i < Baotou.Length; i++)
            //{
            //    Baotou[i] = CoreCode.CodeStr(data[i]);
            //}
            return Baotou;
        }

        internal static string GetStrings(byte[] bys)
        {
            if (bys == null) return null;
            int i = 0;
            int length = bys.Length;
            char[] Baotou = new char[length];

            unsafe
            {
                fixed (char* bytes = Baotou)
                {
                    while (i < length)
                    {
                        bytes[i] = CoreCode.CodeInt(bys[i]);
                        i++;
                    }
                }
            }

            //while (i < length)
            //{
            //    Baotou[i] = CoreCode.CodeInt(bys[i]);
            //    i++;
            //}

            //for (i = 0; i < length; i++)
            //{
            //    Baotou[i] = CoreCode.CodeInt(bys[i]);
            //}
            return new string(Baotou);
        }

        //internal enum TcpCode : byte
        //{
        //    A = 255,
        //    a = 254,
        //    B = 253,
        //    b = 252,
        //    C = 251,
        //    c = 250,
        //    D = 249,
        //    d = 248,
        //    E = 247,
        //    e = 246,
        //    F = 245,
        //    f = 244,
        //    G = 243,
        //    g = 242,
        //    H = 241,
        //    h = 240,
        //    I = 239,
        //    i = 238,
        //    J = 237,
        //    j = 236,
        //    K = 235,
        //    k = 234,
        //    L = 233,
        //    l = 232,
        //    M = 231,
        //    m = 230,
        //    N = 229,
        //    n = 228,
        //    O = 227,
        //    o = 226,
        //    P = 225,
        //    p = 224,
        //    Q = 223,
        //    q = 222,
        //    R = 221,
        //    r = 220,
        //    S = 219,
        //    s = 218,
        //    T = 217,
        //    t = 216,
        //    U = 215,
        //    u = 214,
        //    V = 213,
        //    v = 212,
        //    W = 211,
        //    w = 210,
        //    X = 209,
        //    x = 208,
        //    Y = 207,
        //    y = 206,
        //    Z = 205,
        //    z = 204,
        //}
    }
}

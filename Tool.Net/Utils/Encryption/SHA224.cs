using System;
using System.Security.Cryptography;

namespace Tool.Utils.Encryption
{
    /// <summary>
    /// SHA224加密算法
    /// </summary>
    public class SHA224 : HashAlgorithm
    {
        private const int BlockSize = 64; // 512 bits
        private readonly uint[] _H = new uint[8];
        private readonly byte[] _buffer = new byte[BlockSize];
        private readonly uint[] _W = new uint[64];

        private ulong _count;
        private int _bufferLength;


        private SHA224()
        {
            Initialize();
            HashSizeValue = 224;
        }

        /// <summary>
        /// 初始化加密算法值
        /// </summary>
        public override void Initialize()
        {
            _count = 0;
            _bufferLength = 0;

            // FIPS PUB 180-4 — SHA-224 Initial Hash Values
            _H[0] = 0xc1059ed8;
            _H[1] = 0x367cd507;
            _H[2] = 0x3070dd17;
            _H[3] = 0xf70e5939;
            _H[4] = 0xffc00b31;
            _H[5] = 0x68581511;
            _H[6] = 0x64f98fa7;
            _H[7] = 0xbefa4fa4;
        }

        /// <summary>
        /// 哈希核心
        /// </summary>
        /// <param name="array"></param>
        /// <param name="ibStart"></param>
        /// <param name="cbSize"></param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            int i = ibStart;
            int len = cbSize;

            while (len > 0)
            {
                int toCopy = Math.Min(len, BlockSize - _bufferLength);
                Buffer.BlockCopy(array, i, _buffer, _bufferLength, toCopy);
                _bufferLength += toCopy;
                i += toCopy;
                len -= toCopy;

                if (_bufferLength == BlockSize)
                {
                    ProcessBlock(_buffer, 0);
                    _count += BlockSize;
                    _bufferLength = 0;
                }
            }
        }

        /// <summary>
        /// 哈希最终结果
        /// </summary>
        /// <returns></returns>
        protected override byte[] HashFinal()
        {
            _count += (ulong)_bufferLength;

            ulong bitLength = _count * 8;

            _buffer[_bufferLength++] = 0x80;

            if (_bufferLength > 56)
            {
                Array.Clear(_buffer, _bufferLength, BlockSize - _bufferLength);
                ProcessBlock(_buffer, 0);
                _bufferLength = 0;
            }

            Array.Clear(_buffer, _bufferLength, 56 - _bufferLength);

            for (int i = 0; i < 8; i++)
                _buffer[56 + i] = (byte)(bitLength >> ((7 - i) * 8));

            ProcessBlock(_buffer, 0);

            byte[] hash = new byte[28]; // 224 bits = 28 bytes
            for (int i = 0; i < 7; i++)
            {
                hash[i * 4] = (byte)(_H[i] >> 24);
                hash[i * 4 + 1] = (byte)(_H[i] >> 16);
                hash[i * 4 + 2] = (byte)(_H[i] >> 8);
                hash[i * 4 + 3] = (byte)(_H[i]);
            }

            return hash;
        }

        private void ProcessBlock(byte[] block, int offset)
        {
            uint[] W = _W;

            for (int i = 0; i < 16; i++)
            {
                W[i] = (uint)((block[offset + 4 * i] << 24)
                            | (block[offset + 4 * i + 1] << 16)
                            | (block[offset + 4 * i + 2] << 8)
                            | (block[offset + 4 * i + 3]));
            }

            for (int i = 16; i < 64; i++)
            {
                uint s0 = RotR(W[i - 15], 7) ^ RotR(W[i - 15], 18) ^ (W[i - 15] >> 3);
                uint s1 = RotR(W[i - 2], 17) ^ RotR(W[i - 2], 19) ^ (W[i - 2] >> 10);
                W[i] = unchecked(W[i - 16] + s0 + W[i - 7] + s1);
            }

            uint a = _H[0], b = _H[1], c = _H[2], d = _H[3];
            uint e = _H[4], f = _H[5], g = _H[6], h = _H[7];

            for (int i = 0; i < 64; i++)
            {
                uint S1 = RotR(e, 6) ^ RotR(e, 11) ^ RotR(e, 25);
                uint ch = (e & f) ^ (~e & g);
                uint temp1 = unchecked(h + S1 + ch + K[i] + W[i]);
                uint S0 = RotR(a, 2) ^ RotR(a, 13) ^ RotR(a, 22);
                uint maj = (a & b) ^ (a & c) ^ (b & c);
                uint temp2 = unchecked(S0 + maj);

                h = g;
                g = f;
                f = e;
                e = unchecked(d + temp1);
                d = c;
                c = b;
                b = a;
                a = unchecked(temp1 + temp2);
            }

            _H[0] = unchecked(_H[0] + a);
            _H[1] = unchecked(_H[1] + b);
            _H[2] = unchecked(_H[2] + c);
            _H[3] = unchecked(_H[3] + d);
            _H[4] = unchecked(_H[4] + e);
            _H[5] = unchecked(_H[5] + f);
            _H[6] = unchecked(_H[6] + g);
            _H[7] = unchecked(_H[7] + h);
        }

        private static uint RotR(uint x, int n) => (x >> n) | (x << (32 - n));

        private static readonly uint[] K =
        {
            0x428a2f98,0x71374491,0xb5c0fbcf,0xe9b5dba5,0x3956c25b,0x59f111f1,0x923f82a4,0xab1c5ed5,
            0xd807aa98,0x12835b01,0x243185be,0x550c7dc3,0x72be5d74,0x80deb1fe,0x9bdc06a7,0xc19bf174,
            0xe49b69c1,0xefbe4786,0x0fc19dc6,0x240ca1cc,0x2de92c6f,0x4a7484aa,0x5cb0a9dc,0x76f988da,
            0x983e5152,0xa831c66d,0xb00327c8,0xbf597fc7,0xc6e00bf3,0xd5a79147,0x06ca6351,0x14292967,
            0x27b70a85,0x2e1b2138,0x4d2c6dfc,0x53380d13,0x650a7354,0x766a0abb,0x81c2c92e,0x92722c85,
            0xa2bfe8a1,0xa81a664b,0xc24b8b70,0xc76c51a3,0xd192e819,0xd6990624,0xf40e3585,0x106aa070,
            0x19a4c116,0x1e376c08,0x2748774c,0x34b0bcb5,0x391c0cb3,0x4ed8aa4a,0x5b9cca4f,0x682e6ff3,
            0x748f82ee,0x78a5636f,0x84c87814,0x8cc70208,0x90befffa,0xa4506ceb,0xbef9a3f7,0xc67178f2
        };

        /// <summary>
        /// 创建一个SHA224加密
        /// </summary>
        /// <returns></returns>
        public static new SHA224 Create() => new SHA224();

        /// <summary>
        /// SHA224加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] HashData(byte[] source) 
        {
            using var sha224 = Create();
            return sha224.ComputeHash(source);
        }

        /// <summary>
        /// SHA224加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] HashData(ReadOnlySpan<byte> source) 
        {
            using var sha224 = Create();
            return sha224.ComputeHash(source.ToArray());
        }

        /// <summary>
        /// SHA224加密
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static int HashData(ReadOnlySpan<byte> source, Span<byte> destination) 
        {
            if (TryHashData(source, destination, out int bytesWritten))
            {
                return bytesWritten;
            }
            return bytesWritten;
        }

        /// <summary>
        /// SHA224加密
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="bytesWritten"></param>
        /// <returns></returns>
        public static bool TryHashData(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten) 
        {
            using var sha224 = Create();
            return sha224.TryComputeHash(source, destination, out bytesWritten);
        }
    }
}

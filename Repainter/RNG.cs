using Crypto = System.Security.Cryptography;
using Text = System.Text;

namespace Haiku.Repainter
{
    internal class RNG
    {
        private byte[] entropy;
        private uint p;

        public RNG(string seed)
        {
            var encSeed = new Text.UTF8Encoding().GetBytes(seed);
            var sha = Crypto.SHA512.Create();
            entropy = sha.ComputeHash(encSeed);
            p = 0;
        }

        public float NextFloat()
        {
            if (p + 3 > entropy.Length)
            {
                throw new InvalidOperationException("out of entropy");
            }
            var mantissa = (uint)entropy[p] | ((uint)entropy[p+1] << 8) | ((uint)entropy[p+2] << 16);
            p += 3;
            return (float)mantissa / (float)(1 << 24);
        }

        public bool NextBool()
        {
            if (p + 1 > entropy.Length)
            {
                throw new InvalidOperationException("out of entropy");
            }
            var b = entropy[p];
            p++;
            return b < 128;
        }
    }
}
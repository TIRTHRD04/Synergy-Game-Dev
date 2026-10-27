using System;
using System.Security.Cryptography;
using System.Text;

namespace AdaptiveOpponent.Game
{
    /// <summary>
    /// The commitment that proves the Opponent chose its response before seeing the action: SHA-256 of
    /// "response|nonce", where response is the response index and nonce is 16 unseeded secure random bytes in hex.
    /// The hash is shown while the Opponent is locked in; the nonce is shown after the reveal. Anyone can check it:
    /// printf '%s' "3|&lt;nonce&gt;" | sha256sum. The nonce never touches the Opponent's seeded stream.
    /// </summary>
    public sealed class Commitment
    {
        private const int NonceBytes = 16;

        private Commitment(string hash, string nonce)
        {
            Hash = hash;
            Nonce = nonce;
        }

        /// <summary>The full SHA-256 hash, lowercase hex.</summary>
        public string Hash { get; }

        /// <summary>The nonce, lowercase hex.</summary>
        public string Nonce { get; }

        /// <summary>The first 16 hex characters of the hash, as shown on screen.</summary>
        public string ShortHash => Hash.Substring(0, 16);

        /// <summary>Commits to <paramref name="response"/> with a fresh secure nonce.</summary>
        public static Commitment For(int response)
        {
            var nonce = new byte[NonceBytes];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(nonce);
            }
            string nonceHex = Hex(nonce);
            return new Commitment(HashOf(response, nonceHex), nonceHex);
        }

        /// <summary>The hash of "response|nonce" (also used to verify a logged commitment).</summary>
        public static string HashOf(int response, string nonceHex)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return Hex(sha.ComputeHash(Encoding.UTF8.GetBytes(response + "|" + nonceHex)));
            }
        }

        /// <summary>A fresh unseeded secure 53-bit session seed (fits a JSON number exactly).</summary>
        public static ulong NewSessionSeed()
        {
            var bytes = new byte[8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return BitConverter.ToUInt64(bytes, 0) & ((1UL << 53) - 1);
        }

        private static string Hex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}

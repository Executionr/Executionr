using System;
using System.Security.Cryptography;
using System.Linq;

namespace Executionr.Agent.IO
{
    public class Sha256Hasher : IHasher
    {
        #region IHasher implementation

        public byte[] ComputeHash(System.IO.Stream stream)
        {
            return SHA256.Create().ComputeHash(stream);
        }

        public bool ValidateHash(System.IO.Stream stream, byte[] expected)
        {
            var actual = ComputeHash(stream);

            return expected.SequenceEqual(actual);
        }

        public bool ValidateHash(System.IO.Stream stream, string expected)
        {
            return BitConverter.ToString(ComputeHash(stream)).Equals(expected);
        }

        #endregion
    }
}


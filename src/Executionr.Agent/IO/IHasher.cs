using System;
using System.IO;

namespace Executionr.Agent.IO
{
    public interface IHasher
    {
        byte[] ComputeHash(Stream stream);
        bool ValidateHash(Stream stream, byte[] expected);
        bool ValidateHash(Stream stream, string expected);
    }
}


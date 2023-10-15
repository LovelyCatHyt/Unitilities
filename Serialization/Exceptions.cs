using System;

namespace Unitilities.Serialization
{

    [Serializable]
    public class SizeOfByteNotMatchException : Exception
    {
        public SizeOfByteNotMatchException(int dataSize, int expectedSize) : 
            base($"Size of data not match! Expected {expectedSize}, but read {dataSize}") 
        { }
    }

    [Serializable]
    public class PathFormatException : Exception
    {
        public PathFormatException(string path) : 
            base($"\"{path}\" is not a valid path.")
        { }
    }
}

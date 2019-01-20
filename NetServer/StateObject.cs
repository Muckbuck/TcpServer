using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NetServer
{
    public struct StateObject
    {
        public int _bufferSize;
        public byte[] _buffer;
        public Socket _clientSocket;
       

        public StateObject(int bufferSize, Socket clientSocket)
        {
            _bufferSize = bufferSize;
            _buffer = new byte[bufferSize];
            _clientSocket = clientSocket;
            
        }
    }
}
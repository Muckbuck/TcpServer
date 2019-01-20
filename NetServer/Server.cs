using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace NetServer
{


    class serverSocket
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public List<Socket> clientList = new List<Socket>();

        public void Start()
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress hostAddress = (Dns.Resolve("localhost")).AddressList[0];
            Console.WriteLine(hostAddress);
            IPEndPoint iep = new IPEndPoint(hostAddress, 8030);
            serverSocket.Bind(iep);
            serverSocket.Listen(10);


            while (true)
            {
                allDone.Reset();

                serverSocket.BeginAccept(new AsyncCallback(AcceptConnection),serverSocket);

                allDone.WaitOne();
            }
            
        }


        void AcceptConnection(IAsyncResult ar)
        {
            allDone.Set();

            Socket newConnection = (Socket)ar.AsyncState;
            Socket handler = newConnection.EndAccept(ar);

            clientList.Add(handler);

            Console.WriteLine("{0} has connected", handler.RemoteEndPoint);
            Console.WriteLine("---------------------");

            StateObject state = new StateObject(256, handler);

            handler.BeginReceive(
                state._buffer,
                0,
                state._bufferSize,
                SocketFlags.None,
                new AsyncCallback(Receive),
                state);
        }

        void Receive(IAsyncResult ar)
        {
           
            StateObject state = (StateObject)ar.AsyncState; 
            Socket handler = state._clientSocket;

            int bytesRead = handler.EndReceive(ar);

            if(bytesRead > 0)
            {
                string content = Encoding.ASCII.GetString(state._buffer, 0, bytesRead);

                Console.WriteLine(content);


                Console.WriteLine("Read {0} bytes from socket. \n Data : {1}");

                foreach (Socket receiver in clientList)
                {
                    if (receiver.RemoteEndPoint != handler.RemoteEndPoint)
                    {
                        Console.WriteLine(receiver.RemoteEndPoint);
                        Console.WriteLine(handler.RemoteEndPoint);
                        receiver.BeginSend(state._buffer, 0, state._bufferSize, 0, new AsyncCallback(Send), state);
                    }

                }
            }
            else
            {
                handler.BeginReceive(state._buffer, 0, state._bufferSize, SocketFlags.None, new AsyncCallback(Receive), state);
            }
                
            
                


            //handler.BeginReceive(state._buffer, 0, state._bufferSize, SocketFlags.None,new AsyncCallback(Receive), state);
                

            
            
            
           

        }

        void Send(IAsyncResult ar)
        {
           

       
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state._clientSocket;

            


            handler.BeginReceive(state._buffer, 0 , state._bufferSize, SocketFlags.None, new AsyncCallback(Receive), state);
            

        }
    }
}

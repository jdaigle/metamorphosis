using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Metamorphosis.Console
{
    public static class TcpSocket
    {
        /// <summary>
        /// From the specified socket, asynchronously try and connect to the specified addr at the specified port.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="addr"></param>
        /// <param name="port"></param>
        public static Task ConnectAsync(Socket socket, IPAddress addr, int port)
        {
            var tcs = new TaskCompletionSource<int>();
            var args = new SocketAsyncEventArgs(); // don't need to cache since clients shouldn't make a ton of connections
            args.RemoteEndPoint = new IPEndPoint(addr, port);
            args.UserToken = tcs;
            args.Completed += (s, a) =>
            {
                CompleteAsyncIOOperation(((TaskCompletionSource<int>)a.UserToken), a, args0 => 0);
                a.Dispose();
            };
            if (!socket.ConnectAsync(args))
            {
                CompleteAsyncIOOperation(tcs, args, args0 => 0);
                args.Dispose();
            }

            return tcs.Task;
        }

        public static void CompleteAsyncIOOperation<T>(TaskCompletionSource<T> tcs, SocketAsyncEventArgs args, Func<SocketAsyncEventArgs, T> getResult)
        {
            args.UserToken = null;
            if (args.SocketError != SocketError.Success)
            {
                tcs.SetException(new SocketException((int)args.SocketError));
            }
            else
            {
                tcs.SetResult(getResult(args));
            }
        }
    }
}

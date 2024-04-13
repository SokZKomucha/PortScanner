using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;

namespace NET20240413_PortScanner
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // Customizable
            int threadAmount = 16;      // Amount of threads to run on
            int timeout = 100;          // Request timeout (ms)
            short port = 25565;         // Port to scan for
            IPAddress addressStart = IPAddress.Parse("130.61.0.0");     // Start IP address, inclusive
            IPAddress addressEnd = IPAddress.Parse("130.61.255.255");   // End IP address, inclusive



            StreamWriter sw = new StreamWriter("./out.txt", false, Encoding.UTF8);
            Thread[] threads = new Thread[threadAmount];
            sw.AutoFlush = false;       // true for "realtime" results, false for accuracy

            uint decimalStart = BitConverter.ToUInt32(addressStart.GetAddressBytes().Reverse().ToArray());
            uint decimalEnd = BitConverter.ToUInt32(addressEnd.GetAddressBytes().Reverse().ToArray());
            uint[] segments = DivideEvenly(decimalEnd - decimalStart + 1, (uint)threadAmount);
            int foundCount = 0;

            // Spawns `threadAmount` amount new threads,
            // each scanning `segments[index]` IPs
            for (int i = 0; i < threadAmount; i++)
            {
                int index = i;
                uint sum = (uint)new ArraySegment<uint>(segments, 0, index).ToArray().Sum(x => x);

                threads[index] = new Thread(() =>
                {
                    for (uint j = decimalStart + sum; j < decimalStart + sum + segments[index]; j++)
                    {
                        IPAddress address = new IPAddress(BitConverter.GetBytes(j).Reverse().ToArray());
                        if (CheckPort(address, port, timeout))
                        {
                            sw.Write($"{address}\n");
                            foundCount++;
                        }
                    }
                });
                threads[index].Start();
            }

            foreach (Thread t in threads)
                t.Join();

            sw.Close();
            Console.WriteLine($"Scanned for port {port} on {decimalEnd - decimalStart + 1} IPs");
            Console.WriteLine($"Found {foundCount} results");

        }

        /// <summary>
        /// Checks whether a certain port is open on machine
        /// with specified IP address.
        /// </summary>
        /// <param name="address">IP address to check on</param>
        /// <param name="port">Port to check for</param>
        /// <param name="timeout">Optional; request timeout (in ms)</param>
        /// <returns></returns>
        public static bool CheckPort(IPAddress address, short port, int timeout = 100)
        {
            TcpClient client = new TcpClient();
            try
            {
                return client.ConnectAsync(address, port).Wait(timeout);
            }
            catch
            {
                Console.Error.WriteLine($"An error occured; Address {address} not scanned");
                return false;
            }
            finally
            {
                client.Close();
            }
        }

        /// <summary>
        /// Divides a number x into n sections. Despite it's name, 
        /// it does NOT divide evenly. It should work somewhat like
        /// Math.DivRem(), but when I was writing this, I didn't know
        /// about existence of said method. Won't bother changing it either.
        /// </summary>
        /// <param name="totalCount">Total number to divide</param>
        /// <param name="segmentCount">Numbers of segments produced</param>
        /// <returns></returns>
        public static uint[] DivideEvenly(uint totalCount, uint segmentCount)
        {
            uint rest = totalCount % segmentCount;
            uint totalWithoutRest = totalCount - rest;
            uint[] segments = new uint[segmentCount];

            for (int i = 0; i < segmentCount; i++)
                segments[i] = i < rest ? totalWithoutRest / segmentCount + 1 : totalWithoutRest / segmentCount;
            return segments;
        }
    }
}


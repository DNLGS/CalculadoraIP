using System.Net;

namespace CalculadoraIP.Services
{
    public class IPv4Calculator
    {
        private static void ValidateIp(string ip)
        {
            if (!IPAddress.TryParse(ip, out var ipAddress) ||
                ipAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            {
                throw new ArgumentException(
                    $"IPv4 '{ip}' inválido.",
                    nameof(ip)
                );
            }
        }

        public static string IPv4ToBinary(string ip)
        {
            ValidateIp(ip);
            var octetos = IPAddress.Parse(ip).GetAddressBytes();
            var binary = string.Join(".", octetos.Select(octet => Convert.ToString(octet, 2).PadLeft(8, '0')));
            return binary;
        }

        public static string GetMask(uint prefixLength)
        {
            if (prefixLength > 32)
                throw new ArgumentOutOfRangeException(
                    nameof(prefixLength),
                    "O prefixo CIDR deve estar entre 0 e 32.");

            var mask = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                if (prefixLength >= 8)
                {
                    mask[i] = 255;
                    prefixLength -= 8;
                }
                else
                {
                    mask[i] = (byte)(255 << (8 - (int)prefixLength));
                    prefixLength = 0;
                }
            }
            return new IPAddress(mask).ToString();
        }

        public static string GetNetworkAddress(string ip, uint prefixLength)
        {
            ValidateIp(ip);
            var mask = GetMask(prefixLength);
            var address = System.Net.IPAddress.Parse(ip);
            var octetosMask = System.Net.IPAddress.Parse(mask).GetAddressBytes();
            var octetosAddress = address.GetAddressBytes();
            var networkAddress = new byte[4];
            for (int i = 0; i< 4; i++)
            {
                networkAddress[i] = (byte)(octetosAddress[i] & octetosMask[i]);
            }
            return new IPAddress(networkAddress).ToString();
        }

        public static string GetBroadcastAddress(string networkIp, uint prefixLength)
        {
            ValidateIp(networkIp);
            var mask = GetMask(prefixLength);
            var addressMask = IPAddress.Parse(mask).GetAddressBytes();
            var addressNetwork = IPAddress.Parse(networkIp).GetAddressBytes();

            byte[] broadcast = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                broadcast[i] = (byte)(addressNetwork[i] | ~addressMask[i]);
            }

            return new IPAddress(broadcast).ToString();
        }

        public static string GetFirstHost(string networkIp)
        {
            uint network = IpToUInt(networkIp);

            return UIntToIp(network + 1);
        }

        public static string GetLastHost(string broadcastIp)
        {
            uint broadcast = IpToUInt(broadcastIp);

            return UIntToIp(broadcast - 1);
        }

        private static uint IpToUInt(string ip)
        {
            ValidateIp(ip);

            byte[] bytes = IPAddress.Parse(ip).GetAddressBytes();

            return ((uint)bytes[0] << 24) |
                   ((uint)bytes[1] << 16) |
                   ((uint)bytes[2] << 8) |
                   bytes[3];
        }

        private static string UIntToIp(uint ip)
        {
            byte[] bytes =
            [
                (byte)(ip >> 24),
                (byte)(ip >> 16),
                (byte)(ip >> 8),
                (byte)ip
            ];

            return new IPAddress(bytes).ToString();
        }
    }
}
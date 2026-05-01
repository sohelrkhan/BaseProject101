namespace SadaqaAccounting.Application.Configurations
{
    public static class IPHelper
    {
        public static string GetIpAddress(HttpContext? context)
        {
            if (context == null)
                return "Unknown";

            // First try X-Forwarded-For (proxy support)
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress?.ToString();

                if (ip == "::1")
                {
                    ip = GetLocalIpAddress();
                }
            }

            return ip ?? "Unknown";
        }
        
        private static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}

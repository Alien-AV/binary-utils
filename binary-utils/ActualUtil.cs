using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;

namespace binary_utils
{
    internal class ActualUtil
    {
        public static bool ReverseHex()
        {
            var original = Clipboard.GetText();
            var cleaned = Regex.Replace(original, @"\s+", "");
            var parseResult = int.TryParse(cleaned, NumberStyles.HexNumber, null, out var parsedHex);
            if (parseResult == false)
            {
                return false;
            }
            var reversed = System.Net.IPAddress.NetworkToHostOrder(parsedHex);
            Clipboard.SetText(reversed.ToString("x8"));
            return true;
        }
    }
}

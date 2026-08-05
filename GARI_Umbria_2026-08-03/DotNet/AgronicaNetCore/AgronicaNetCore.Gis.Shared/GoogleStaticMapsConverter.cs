using System.Security.Cryptography;
using System.Text;

namespace AgronicaNetCore.Gis.Shared
{
    /// <summary>
    /// Utilità per la Google Static Maps API: codifica Encoded Polyline e firma URL.
    /// </summary>
    public static class GoogleStaticMapsConverter
    {
        public sealed class CoordinateEntity
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        /// <summary>
        /// Parses a WKT POLYGON string and returns the encoded polyline string
        /// using the Google Encoded Polyline Algorithm.
        /// </summary>
        public static string GeneraEncodedPolyline(string wkt)
        {
            var points = ParseWktToCoordinates(wkt);
            return Encode(points);
        }

        /// <summary>
        /// Parses a WKT POLYGON or MULTIPOLYGON string into a list of coordinates.
        /// Only the outer ring of the first polygon is used.
        /// Expected format: POLYGON ((lon lat, lon lat, ...))
        /// </summary>
        private static List<CoordinateEntity> ParseWktToCoordinates(string wkt)
        {
            int start = wkt.IndexOf('(');
            int end = wkt.LastIndexOf(')');
            if (start < 0 || end < 0)
                throw new ArgumentException($"WKT non valido: nessuna parentesi trovata. WKT: {wkt}");

            // Strip outer rings for POLYGON (( ... )) — find the inner coordinates
            string inner = wkt.Substring(start).TrimStart('(').TrimEnd(')');
            // Take only up to the first closing paren (outer ring)
            int firstClose = inner.IndexOf(')');
            if (firstClose >= 0)
                inner = inner.Substring(0, firstClose);
            inner = inner.Trim().TrimStart('(').Trim();

            var points = new List<CoordinateEntity>();
            foreach (var pair in inner.Split(','))
            {
                var parts = pair.Trim().Split(' ');
                if (parts.Length < 2)
                    continue;
                if (!double.TryParse(parts[0], System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out double lon))
                    continue;
                if (!double.TryParse(parts[1], System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out double lat))
                    continue;
                points.Add(new CoordinateEntity { Latitude = lat, Longitude = lon });
            }

            return points;
        }

        /// <summary>
        /// Encodes a sequence of coordinates using the Google Encoded Polyline Algorithm.
        /// </summary>
        public static string Encode(IEnumerable<CoordinateEntity> points)
        {
            var str = new StringBuilder();

            Action<int> encodeDiff = diff =>
            {
                int shifted = diff << 1;
                if (diff < 0)
                    shifted = ~shifted;

                int rem = shifted;
                while (rem >= 0x20)
                {
                    str.Append((char)((0x20 | (rem & 0x1f)) + 63));
                    rem >>= 5;
                }
                str.Append((char)(rem + 63));
            };

            int lastLat = 0;
            int lastLng = 0;

            foreach (var point in points)
            {
                int lat = (int)Math.Round(point.Latitude * 1E5);
                int lng = (int)Math.Round(point.Longitude * 1E5);

                encodeDiff(lat - lastLat);
                encodeDiff(lng - lastLng);

                lastLat = lat;
                lastLng = lng;
            }

            return str.ToString();
        }

        /// <summary>
        /// Signs a Google Static Maps URL with the provided private key using HMAC-SHA1.
        /// </summary>
        public static string Sign(string url, string privateKey)
        {
            string usableKey = privateKey.Replace("-", "+").Replace("_", "/");
            byte[] privateKeyBytes = Convert.FromBase64String(usableKey);

            var uri = new Uri(url);
            byte[] pathAndQueryBytes = Encoding.ASCII.GetBytes(uri.LocalPath + uri.Query);

            using var algorithm = new HMACSHA1(privateKeyBytes);
            byte[] hash = algorithm.ComputeHash(pathAndQueryBytes);

            string signature = Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_");

            return uri.Scheme + "://" + uri.Host + uri.LocalPath + uri.Query + "&signature=" + signature;
        }
    }
}

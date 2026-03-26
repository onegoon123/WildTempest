// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("LkzznvYTMcNJi6ACBhbp4TneLvVunsS4FF4zV6xrXCt1QB6K217myhRd6o00uOeAsQi/08bxUZ7iMHsrMVGZ1/0i9WIWgT+AuEUOB71nZlcypa/OcSPk7e6B4NfQXKWnd7IuY2G7CHu+uL2z5YulI5BMDRE0dpKjK6imqZkrqKOrK6ioqQ+UgVaktZfyyYPu3L8Shw1jTz3T1K/K1K1RZPgvShivhm8yFHPaLPlSAPfDzSWOsvnv6A+5+WEaT1eFLh4mjZcckw0iZ9OTTekJepuFFrnc+OMs/3Sew61sc1JSyKBRe3CXtTfXcAkOB8mBESiCIjnmRryFM/Z3gFeEuQTKnEmZK6iLmaSvoIMv4S9epKioqKypqpWmV3sjTRJ2yKuqqKmo");
        private static int[] order = new int[] { 8,4,11,12,5,7,7,7,11,12,12,12,13,13,14 };
        private static int key = 169;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}

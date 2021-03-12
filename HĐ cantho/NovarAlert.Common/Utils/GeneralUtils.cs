namespace NovaAlert.Common.Utils
{
    public static class GeneralUtils
    {
        public static string FormatPhoneNumber(string areaCode, string number)
        {
            if (string.IsNullOrEmpty(areaCode))
            {
                return number;
            }

            return string.Format("{0}.{1}", areaCode, number);
        }
    }
}

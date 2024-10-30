namespace BlazorApp_TeleCRM.Helper
{
    public class UtilityConvet
    {
        public string FormatPhoneNumber(string? phoneNumber)
        {

            if (string.IsNullOrEmpty(phoneNumber)) {
                return "";
            }

            if (phoneNumber.Length == 10)
            {
                return $"{phoneNumber.Substring(0, 3)}-{phoneNumber.Substring(3, 4)}-{phoneNumber.Substring(7, 3)}";
            }

            return phoneNumber; // คืนค่าเดิมถ้าไม่ตรงกับความยาวที่ต้องการ
        }

        public string FormatPhoneNumberWithCountryCode(string? phoneNumber)
        {

            if (string.IsNullOrEmpty(phoneNumber))
            {
                return "";
            }

            if (phoneNumber.StartsWith("0") && phoneNumber.Length == 10)
            {
                return $"+66{phoneNumber.Substring(1)}";
            }

            return phoneNumber; // คืนค่าเดิมถ้าไม่ตรงกับความยาวที่ต้องการ
        }

    }
}

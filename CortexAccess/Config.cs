namespace CortexAccess
{
    static class Config
    {
        /*
         * To get a client id and a client secret, you must connect to your Emotiv
         * account on emotiv.com and create a Cortex app.
         * https://www.emotiv.com/my-account/cortex-apps/
         */
        public static string AppClientId = "DcJ4tJaanxMdDciT78xvMYAd2Icdwq9WcSQZRiWP";
        public static string AppClientSecret = "t7zFY2NxTp3KZ337IniRrEJJfk4Q3dTgxJR5nJfYaBGaYGYk7C1SG8Pc8XAhjlrdfnkHCn9oOrfWckMGPH1FeferlVWmAe5LWEZpUZjqqL9cwTnjl6tKXSgMj5h9P4nj";

        // If you use an Epoc Flex headset, then you must put your configuration here
        public static string FlexMapping = @"{
                                  'CMS':'TP8', 'DRL':'P6',
                                  'RM':'TP10','RN':'P4','RO':'P8'}";

    }

    public static class WarningCode
    {
        public const int StreamStop = 0;
        public const int SessionAutoClosed = 1;
        public const int UserLogin = 2;
        public const int UserLogout = 3;
        public const int ExtenderExportSuccess = 4;
        public const int ExtenderExportFailed = 5;
        public const int UserNotAcceptLicense = 6;
        public const int UserNotHaveAccessRight = 7;
        public const int UserRequestAccessRight = 8;
        public const int AccessRightGranted = 9;
        public const int AccessRightRejected = 10;
        public const int CannotDetectOSUSerInfo = 11;
        public const int CannotDetectOSUSername = 12;
        public const int ProfileLoaded = 13;
        public const int ProfileUnloaded = 14;
        public const int CortexAutoUnloadProfile = 15;
        public const int UserLoginOnAnotherOsUser = 16;
        public const int EULAAccepted = 17;
        public const int StreamWritingClosed = 18;
        public const int HeadsetWrongInformation = 100;
        public const int HeadsetCannotConnected = 101;
        public const int HeadsetConnectingTimeout = 102;
        public const int HeadsetDataTimeOut = 103;
        public const int HeadsetConnected = 104;
        public const int HeadsetScanFinished = 142;
    }
}

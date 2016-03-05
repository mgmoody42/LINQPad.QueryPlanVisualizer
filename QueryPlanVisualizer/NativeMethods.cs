using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Visualizer
{
    class NativeMethods
    {
        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern uint AssocQueryString(AssocF flags, AssocStr str, string pszAssoc, string pszExtra, [Out] StringBuilder pszOut, ref uint pcchOut);

        public static string AssocQueryString(AssocStr association, string extension)
        {
            const int S_OK = 0;
            const int S_FALSE = 1;

            uint length = 0;
            var result = AssocQueryString(AssocF.None, association, extension, null, null, ref length);
            if (result != S_FALSE)
            {
                return null;
            }

            var stringBuilder = new StringBuilder((int)length);
            result = AssocQueryString(AssocF.None, association, extension, null, stringBuilder, ref length);
            if (result != S_OK)
            {
                return null;
            }

            return stringBuilder.ToString();
        }

        [Flags]
        enum AssocF : uint
        {
            None = 0,
            Init_NoRemapCLSID = 0x1,
            Init_ByExeName = 0x2,
            Open_ByExeName = 0x2,
            Init_DefaultToStar = 0x4,
            Init_DefaultToFolder = 0x8,
            NoUserSettings = 0x10,
            NoTruncate = 0x20,
            Verify = 0x40,
            RemapRunDll = 0x80,
            NoFixUps = 0x100,
            IgnoreBaseClass = 0x200,
            Init_IgnoreUnknown = 0x400,
            Init_FixedProgId = 0x800,
            IsProtocol = 0x1000,
            InitForFile = 0x2000,
        }

        internal enum AssocStr
        {
            Command = 1,
            Executable,
            FriendlyDocName,
            FriendlyAppName,
            NoOpen,
            ShellNewValue,
            DDECommand,
            DDEIfExec,
            DDEApplication,
            DDETopic,
            InfoTip,
            QuickTip,
            TileInfo,
            ContentType,
            DefaultIcon,
            ShellExtension,
            DropTarget,
            DelegateExecute,
            SupportedUriProtocols,
            Max,
        }
    }
}
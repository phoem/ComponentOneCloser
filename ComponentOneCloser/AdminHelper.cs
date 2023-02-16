using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

namespace ComponentOneCloser
{
    // The following provided and suggested by DH. Thanks DH! - For privilege checking/escalation (UAC)
    class AdminHelper
    {
        #region "Constants"
        const UInt32 TOKEN_QUERY = 0x0008;
        const int INT_SIZE = 4;
        #endregion

        private enum TOKEN_ELEVATION_TYPE
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull,
            TokenElevationTypeLimited
        }

        public enum TOKEN_INFORMATION_CLASS
        {
            /// <summary>
            /// The buffer receives a TOKEN_USER structure that contains the user account of the token.
            /// </summary>
            TokenUser = 1,

            /// <summary>
            /// The buffer receives a TOKEN_GROUPS structure that contains the group accounts associated with the token.
            /// </summary>
            TokenGroups,

            /// <summary>
            /// The buffer receives a TOKEN_PRIVILEGES structure that contains the privileges of the token.
            /// </summary>
            TokenPrivileges,

            /// <summary>
            /// The buffer receives a TOKEN_OWNER structure that contains the default owner security identifier (SID) for newly created objects.
            /// </summary>
            TokenOwner,

            /// <summary>
            /// The buffer receives a TOKEN_PRIMARY_GROUP structure that contains the default primary group SID for newly created objects.
            /// </summary>
            TokenPrimaryGroup,

            /// <summary>
            /// The buffer receives a TOKEN_DEFAULT_DACL structure that contains the default DACL for newly created objects.
            /// </summary>
            TokenDefaultDacl,

            /// <summary>
            /// The buffer receives a TOKEN_SOURCE structure that contains the source of the token. TOKEN_QUERY_SOURCE access is needed to retrieve this information.
            /// </summary>
            TokenSource,

            /// <summary>
            /// The buffer receives a TOKEN_TYPE value that indicates whether the token is a primary or impersonation token.
            /// </summary>
            TokenType,

            /// <summary>
            /// The buffer receives a SECURITY_IMPERSONATION_LEVEL value that indicates the impersonation level of the token. If the access token is not an impersonation token, the function fails.
            /// </summary>
            TokenImpersonationLevel,

            /// <summary>
            /// The buffer receives a TOKEN_STATISTICS structure that contains various token statistics.
            /// </summary>
            TokenStatistics,

            /// <summary>
            /// The buffer receives a TOKEN_GROUPS structure that contains the list of restricting SIDs in a restricted token.
            /// </summary>
            TokenRestrictedSids,

            /// <summary>
            /// The buffer receives a DWORD value that indicates the Terminal Services session identifier that is associated with the token. 
            /// </summary>
            TokenSessionId,

            /// <summary>
            /// The buffer receives a TOKEN_GROUPS_AND_PRIVILEGES structure that contains the user SID, the group accounts, the restricted SIDs, and the authentication ID associated with the token.
            /// </summary>
            TokenGroupsAndPrivileges,

            /// <summary>
            /// Reserved.
            /// </summary>
            TokenSessionReference,

            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if the token includes the SANDBOX_INERT flag.
            /// </summary>
            TokenSandBoxInert,

            /// <summary>
            /// Reserved.
            /// </summary>
            TokenAuditPolicy,

            /// <summary>
            /// The buffer receives a TOKEN_ORIGIN value. 
            /// </summary>
            TokenOrigin,

            /// <summary>
            /// The buffer receives a TOKEN_ELEVATION_TYPE value that specifies the elevation level of the token.
            /// </summary>
            TokenElevationType,

            /// <summary>
            /// The buffer receives a TOKEN_LINKED_TOKEN structure that contains a handle to another token that is linked to this token.
            /// </summary>
            TokenLinkedToken,

            /// <summary>
            /// The buffer receives a TOKEN_ELEVATION structure that specifies whether the token is elevated.
            /// </summary>
            TokenElevation,

            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if the token has ever been filtered.
            /// </summary>
            TokenHasRestrictions,

            /// <summary>
            /// The buffer receives a TOKEN_ACCESS_INFORMATION structure that specifies security information contained in the token.
            /// </summary>
            TokenAccessInformation,

            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if virtualization is allowed for the token.
            /// </summary>
            TokenVirtualizationAllowed,

            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if virtualization is enabled for the token.
            /// </summary>
            TokenVirtualizationEnabled,

            /// <summary>
            /// The buffer receives a TOKEN_MANDATORY_LABEL structure that specifies the token's integrity level. 
            /// </summary>
            TokenIntegrityLevel,

            /// <summary>
            /// The buffer receives a DWORD value that is nonzero if the token has the UIAccess flag set.
            /// </summary>
            TokenUIAccess,

            /// <summary>
            /// The buffer receives a TOKEN_MANDATORY_POLICY structure that specifies the token's mandatory integrity policy.
            /// </summary>
            TokenMandatoryPolicy,

            /// <summary>
            /// The buffer receives the token's logon security identifier (SID).
            /// </summary>
            TokenLogonSid,

            /// <summary>
            /// The maximum value for this enumeration
            /// </summary>
            MaxTokenInfoClass
        }

        #region "Windows API Function DLL Imports"

        [DllImport("KERNEL32.DLL")]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("ADVAPI32.DLL", SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("ADVAPI32.DLL", SetLastError = true)]
        public static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation, uint TokenInformationLength, out uint ReturnLength);

        #endregion

        #region "Public Methods"

        /// <summary>
        /// Attempts to run itself with administrative privileges
        /// </summary>
        public static void RestartAsAdmin()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Application.ExecutablePath;
            startInfo.Verb = "runas";
            try
            {
                Process p = Process.Start(startInfo);
                Application.Exit();
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Debug.WriteLine("Cannot restart as admin: ");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("Error Code: ");
                Debug.WriteLine(ex.ErrorCode.ToString());
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.Source);
                MessageBox.Show("Sorry utility requires elevated privileges to complete");
                return;
            }
        }

        /// <summary>
        /// Checks if the code is running as an Administrator
        /// </summary>
        /// <returns>true if we are running as admin, false if we are not</returns>
        public static bool IsRunningAsAdmin()
        {
            WindowsIdentity MyIdentity = WindowsIdentity.GetCurrent();
            WindowsPrincipal MyPrincipal = new WindowsPrincipal(MyIdentity);
            return MyPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Do we have the ability and permissions to Elevate to Administrator privileges (UAC)
        /// </summary>
        /// <returns>True if we can elevate to administrator privileges, false if we cannot</returns>
        public static bool CanElevateToAdmin()
        {
            bool IsAdmin = IsRunningAsAdmin();

            // If we are already an admin, then return true and stop
            if (IsAdmin == true)
                return true;

            // Check for VISTA or higher
            if (Environment.OSVersion.Version.Major > 5)
            {
                IntPtr myToken;
                TOKEN_ELEVATION_TYPE elevationType;
                uint dwSize;
                IntPtr pElevationType = Marshal.AllocHGlobal(INT_SIZE);

                // Get a token reference for the user running this process
                OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY, out myToken);

                // Get the elevation information for this token
                GetTokenInformation(myToken, TOKEN_INFORMATION_CLASS.TokenElevationType, pElevationType, INT_SIZE, out dwSize);

                elevationType = (TOKEN_ELEVATION_TYPE)Marshal.ReadInt32(pElevationType);

                // Free allocated unmanaged memory
                Marshal.FreeHGlobal(pElevationType);

                // Determine the result of the elevation check
                // Full - user has a split token and the process is running elevated
                // Limited - User has a split token but the process is not running elevated
                // Default - User is not using a split token.
                return ((elevationType == TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited) || (elevationType == TOKEN_ELEVATION_TYPE.TokenElevationTypeFull));
            }
            else
            {
                return IsAdmin; // Prior to vista we only check if the user is in the admin group.
            }
        }

        /// <summary>
        /// Are we running in standard mode, but can elevate?
        /// </summary>
        /// <returns>true if we are not running as admin and we can elevate, false if we are running as admin already or we cannot elevate</returns>
        public static bool RunningStandardButCanElevate()
        {
            return (CanElevateToAdmin() && (IsRunningAsAdmin() != true));
        }

        #endregion
    }
}
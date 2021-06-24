using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DockerForm
{
    public enum Constructor
    {
        Intel = 0,
        Nvidia = 1,
        AMD = 2
    }

    public enum Type
    {
        Internal = 0,
        Discrete = 1
    }

    public class VideoController
    {
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        const int BUFFER_SIZE = 1024;

        enum RemovalPolicy : uint
        {
            CM_REMOVAL_POLICY_EXPECT_NO_REMOVAL = 1,
            CM_REMOVAL_POLICY_EXPECT_ORDERLY_REMOVAL = 2,
            CM_REMOVAL_POLICY_EXPECT_SURPRISE_REMOVAL = 3
        }

        enum SetupDiGetDeviceRegistryPropertyEnum : uint
        {
            SPDRP_DEVICEDESC = 0x00000000, // DeviceDesc (R/W)
            SPDRP_HARDWAREID = 0x00000001, // HardwareID (R/W)
            SPDRP_COMPATIBLEIDS = 0x00000002, // CompatibleIDs (R/W)
            SPDRP_UNUSED0 = 0x00000003, // unused
            SPDRP_SERVICE = 0x00000004, // Service (R/W)
            SPDRP_UNUSED1 = 0x00000005, // unused
            SPDRP_UNUSED2 = 0x00000006, // unused
            SPDRP_CLASS = 0x00000007, // Class (R--tied to ClassGUID)
            SPDRP_CLASSGUID = 0x00000008, // ClassGUID (R/W)
            SPDRP_DRIVER = 0x00000009, // Driver (R/W)
            SPDRP_CONFIGFLAGS = 0x0000000A, // ConfigFlags (R/W)
            SPDRP_MFG = 0x0000000B, // Mfg (R/W)
            SPDRP_FRIENDLYNAME = 0x0000000C, // FriendlyName (R/W)
            SPDRP_LOCATION_INFORMATION = 0x0000000D, // LocationInformation (R/W)
            SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0x0000000E, // PhysicalDeviceObjectName (R)
            SPDRP_CAPABILITIES = 0x0000000F, // Capabilities (R)
            SPDRP_UI_NUMBER = 0x00000010, // UiNumber (R)
            SPDRP_UPPERFILTERS = 0x00000011, // UpperFilters (R/W)
            SPDRP_LOWERFILTERS = 0x00000012, // LowerFilters (R/W)
            SPDRP_BUSTYPEGUID = 0x00000013, // BusTypeGUID (R)
            SPDRP_LEGACYBUSTYPE = 0x00000014, // LegacyBusType (R)
            SPDRP_BUSNUMBER = 0x00000015, // BusNumber (R)
            SPDRP_ENUMERATOR_NAME = 0x00000016, // Enumerator Name (R)
            SPDRP_SECURITY = 0x00000017, // Security (R/W, binary form)
            SPDRP_SECURITY_SDS = 0x00000018, // Security (W, SDS form)
            SPDRP_DEVTYPE = 0x00000019, // Device Type (R/W)
            SPDRP_EXCLUSIVE = 0x0000001A, // Device is exclusive-access (R/W)
            SPDRP_CHARACTERISTICS = 0x0000001B, // Device Characteristics (R/W)
            SPDRP_ADDRESS = 0x0000001C, // Device Address (R)
            SPDRP_UI_NUMBER_DESC_FORMAT = 0X0000001D, // UiNumberDescFormat (R/W)
            SPDRP_DEVICE_POWER_DATA = 0x0000001E, // Device Power Data (R)
            SPDRP_REMOVAL_POLICY = 0x0000001F, // Removal Policy (R)
            SPDRP_REMOVAL_POLICY_HW_DEFAULT = 0x00000020, // Hardware Removal Policy (R)
            SPDRP_REMOVAL_POLICY_OVERRIDE = 0x00000021, // Removal Policy Override (RW)
            SPDRP_INSTALL_STATE = 0x00000022, // Device Install State (R)
            SPDRP_LOCATION_PATHS = 0x00000023, // Device Location Paths (R)
            SPDRP_BASE_CONTAINERID = 0x00000024  // Base ContainerID (R)
        }

        [Flags]
        public enum DiGetClassFlags : uint
        {
            DIGCF_DEFAULT = 0x00000001,  // only valid with DIGCF_DEVICEINTERFACE
            DIGCF_PRESENT = 0x00000002,
            DIGCF_ALLCLASSES = 0x00000004,
            DIGCF_PROFILE = 0x00000008,
            DIGCF_DEVICEINTERFACE = 0x00000010,
        }

        public enum RegType : uint
        {
            REG_BINARY = 3,
            REG_DWORD = 4,
            REG_EXPAND_SZ = 2,
            REG_MULTI_SZ = 7,
            REG_SZ = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SP_DEVICE_INTERFACE_DATA
        {
            public Int32 cbSize;
            public Guid interfaceClassGuid;
            public Int32 flags;
            private UIntPtr reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SP_DEVINFO_DATA
        {
            public UInt32 cbSize;
            public Guid ClassGuid;
            public UInt32 DevInst;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = BUFFER_SIZE)]
            public byte[] DevicePath;
        }

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SetupDiGetClassDevs(IntPtr ClassGuid,
                                                 [MarshalAs(UnmanagedType.LPWStr)] string Enumerator,
                                                 IntPtr hwndParent,
                                                 uint Flags);

        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInfo, ref Guid interfaceClassGuid, UInt32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, UInt32 deviceInterfaceDetailDataSize, ref UInt32 requiredSize, ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, out UInt32 propertyRegDataType, byte[] propertyBuffer, uint propertyBufferSize, out UInt32 requiredSize);

        [DllImport("setupapi.dll")]
        static extern Int32 SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        static uint getDWORDProp(IntPtr h, SP_DEVINFO_DATA da, SetupDiGetDeviceRegistryPropertyEnum prop)
        {
            UInt32 requiredSize;
            UInt32 regType;
            byte[] ptrBuf = new byte[4];
            if (!SetupDiGetDeviceRegistryProperty(h, ref da, (uint)prop, out regType, ptrBuf, 4, out requiredSize))
                throw new InvalidOperationException("Error getting DWORD property");

            if (regType != (uint)RegType.REG_DWORD || requiredSize != 4)
                throw new InvalidOperationException("Property is not a REG_DWORD");

            return BitConverter.ToUInt32(ptrBuf, 0);
        }

        static bool isGpuRemovable(string pnpDeviceID)
        {
            Guid gpuGuid = new Guid("{5b45201d-f2f2-4f3b-85bb-30ff1f953599}"); // GUID_DEVINTERFACE_DISPLAY_ADAPTER
            IntPtr h = SetupDiGetClassDevs(IntPtr.Zero, pnpDeviceID, IntPtr.Zero, (uint)(DiGetClassFlags.DIGCF_ALLCLASSES | DiGetClassFlags.DIGCF_PRESENT | DiGetClassFlags.DIGCF_DEVICEINTERFACE));
            if (h == INVALID_HANDLE_VALUE)
                return false;

            try {
                for (uint i = 0; ; i++) {
                    SP_DEVICE_INTERFACE_DATA interfaceData = new SP_DEVICE_INTERFACE_DATA();
                    interfaceData.cbSize = Marshal.SizeOf(interfaceData);
                    if (!SetupDiEnumDeviceInterfaces(h, IntPtr.Zero, ref gpuGuid, i, ref interfaceData))
                        break;

                    SP_DEVINFO_DATA devinfoData = new SP_DEVINFO_DATA();
                    devinfoData.cbSize = (uint)Marshal.SizeOf(devinfoData);

                    SP_DEVICE_INTERFACE_DETAIL_DATA interfaceDetailData = new SP_DEVICE_INTERFACE_DETAIL_DATA();
                    interfaceDetailData.cbSize = (IntPtr.Size == 4) ? 4 + Marshal.SystemDefaultCharSize : 8;

                    uint nRequiredSize = 0;
                    uint nBytes = BUFFER_SIZE;
                    if (!SetupDiGetDeviceInterfaceDetail(h, ref interfaceData, ref interfaceDetailData, nBytes, ref nRequiredSize, ref devinfoData))
                        break;

                    uint removalPolicy = getDWORDProp(h, devinfoData, SetupDiGetDeviceRegistryPropertyEnum.SPDRP_REMOVAL_POLICY);
                    if (removalPolicy != (uint)RemovalPolicy.CM_REMOVAL_POLICY_EXPECT_NO_REMOVAL)
                        return true;
                }
            } finally {
                SetupDiDestroyDeviceInfoList(h);
            }

            return false;
        }

        public string PNPDeviceID;
        public string Name;
        public string Description;
        public uint ErrorCode;
        public Constructor Constructor;
        public Type Type;

        internal void Initialize()
        {
            if (Name.ToLower().Contains("nvidia"))
                Constructor = Constructor.Nvidia;
            else if (Name.ToLower().Contains("amd"))
                Constructor = Constructor.AMD;
            else if (Name.ToLower().Contains("intel"))
                Constructor = Constructor.Intel;

            Type = isGpuRemovable(PNPDeviceID) ? Type.Discrete : Type.Internal;
        }

        public bool IsDisabled()
        {
            return ErrorCode == 22;
        }

        public bool IsEnabled()
        {
            return ErrorCode == 0;
        }

        public bool EnableDevice(string devconPath)
        {
            if (IsEnabled())
                return false;

            using (var EnableProcess = Process.Start(new ProcessStartInfo(devconPath, $" /enable \"{Name}\"")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                Verb = "runas"
            }))
            {
                string content = string.Format(MainForm.CurrentResource.GetString("DeviceEnable"), Name);
                LogManager.UpdateLog(content);
                return true;
            }
        }

        public bool DisableDevice(string devconPath)
        {
            if (IsDisabled())
                return false;

            using (var EnableProcess = Process.Start(new ProcessStartInfo(devconPath, $" /disable \"{Name}\"")
            { 
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                Verb = "runas"
            }))
            {
                string content = string.Format(MainForm.CurrentResource.GetString("DeviceDisable"), Name);
                LogManager.UpdateLog(content);
                ErrorCode = 22;
                return true;
            }
        }
    }
}

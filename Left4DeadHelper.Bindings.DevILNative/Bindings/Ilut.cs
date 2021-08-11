namespace Left4DeadHelper.Bindings.DevILNative.Bindings
{
    internal static class Ilut
    {
        #region Constants

        public const ushort Version_1_8_0 = 1;
        public const ushort Version = 180;

        #endregion

        #region Enumerations

        public enum Attributes : uint
        {
            OpenGlBit = 0x00000001,
            D3dBit = 0x00000002,
            AllAttribBits = 0x000FFFFF,
        }

        public enum ErrorType : ushort
        {
            InvalidEnum = 0x0501,
            OutOfMemory = 0x0502,
            InvalidValue = 0x0505,
            IllegalOperation = 0x0506,
            InvalidParam = 0x0509,
            CouldNotOpenFile = 0x050A,
            StackOverflow = 0x050E,
            StackUnderflow = 0x050F,
            BadDimensions = 0x0511,
            NotSupported = 0x0550,
        }

        public enum State : ushort
        {
            PaletteMode = 0x0600,
            OpenglConv = 0x0610,
            D3dMiplevels = 0x0620,
            MaxtexWidth = 0x0630,
            MaxtexHeight = 0x0631,
            MaxtexDepth = 0x0632,
            GlUseS3tc = 0x0634,
            D3dUseDxtc = 0x0634,
            GlGenS3tc = 0x0635,
            D3dGenDxtc = 0x0635,
            S3tcFormat = 0x0705,
            DxtcFormat = 0x0705,
            D3dPool = 0x0706,
            D3dAlphaKeyColor = 0x0707,
            D3dAlphaKeyColuor = D3dAlphaKeyColor,
            ForceIntegerFormat = 0x0636,
            //This new state does automatic texture target detection
            //if enabled. Currently, only cubemap detection is supported.
            //if the current image is no cubemap, the 2d texture is chosen.
            GlAutodetectTextureTarget = 0x0807,
        }

        public enum StringName : ushort
        {
            Vendor = Il.Ext.Vendor,
            VersionNum = Il.Value.VersionNum
        }

        public const ushort VersionNum = Il.Version;

        public enum RenderingApi : ushort
        {
            // The different rendering api's...more to be added later?
            Opengl = 0,
            Allegro = 1,
            Win32 = 2,
            Direct3d8 = 3,
            Direct3d9 = 4,
            X11 = 5,
            Direct3d10 = 6,
        }

        #endregion

        #region Functions



        private static class x86
        {
            //[DllImport("ILUT.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        }

        private static class x64
        {
            //[DllImport("ILUT.x64.dll", CallingConvention = CallingConvention.Cdecl)]
        }

        #endregion
    }
}

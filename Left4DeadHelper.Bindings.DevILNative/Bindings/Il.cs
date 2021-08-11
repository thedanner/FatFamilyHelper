using System;
using System.Runtime.InteropServices;

namespace Left4DeadHelper.Bindings.DevILNative.Bindings
{
    internal static class Il
    {
        #region Constants

        public const ushort Version_1_8_0 = 1;
        public const ushort Version = 180;

        #endregion
        
        #region Enumerations

        public enum DataFormat : ushort
        {
            ColorIndex = 0x1900,
            Alpha = 0x1906,
            Rgb = 0x1907,
            Rgba = 0x1908,
            Bgr = 0x80E0,
            Bgra = 0x80E1,
            Luminance = 0x1909,
            LuminanceAlpha = 0x190A,
        }

        public enum DataType : ushort
        {
            Byte = 0x1400,
            UnsignedByte = 0x1401,
            Short = 0x1402,
            UnsignedShort = 0x1403,
            Int = 0x1404,
            UnsignedInt = 0x1405,
            Float = 0x1406,
            Double = 0x140A,
            Half = 0x140B,
        }

        public enum Ext : ushort
        {
            Vendor = 0x1F00,
            LoadExt = 0x1F01,
            SaveExt = 0x1F02,
        }

        [Flags]
        public enum Attributes : uint
        {
            Origin = 0x00000001,
            File = 0x00000002,
            Pal = 0x00000004,
            Format = 0x00000008,
            Typwe = 0x00000010,
            Compress = 0x00000020,
            Loadfail = 0x00000040,
            FormatSpecific = 0x00000080,
            AllAttribs = 0x000FFFFF,
        }

        public enum PaletteType : ushort
        {
            PalNone = 0x0400,
            PalRgb24 = 0x0401,
            PalRgb32 = 0x0402,
            PalRgba32 = 0x0403,
            PalBgr24 = 0x0404,
            PalBgr32 = 0x0405,
            PalBgra2 = 0x0406,
        }

        public enum ImageType : ushort
        {
            TypeUnknown = 0x0000,
            Bmp = 0x0420,  //!< Microsoft Windows Bitmap - .bmp extension
            Cut = 0x0421,  //!< Dr. Halo - .cut extension
            Doom = 0x0422,  //!< DooM walls - no specific extension
            DoomFlat = 0x0423,  //!< DooM flats - no specific extension
            Ico = 0x0424,  //!< Microsoft Windows Icons and Cursors - .ico and .cur extensions
            Jpg = 0x0425,  //!< JPEG - .jpg, .jpe and .jpeg extensions
            Jfif = 0x0425,  //!<
            Ilbm = 0x0426,  //!< Amiga IFF (FORM ILBM) - .iff, .ilbm, .lbm extensions
            Pcd = 0x0427,  //!< Kodak PhotoCD - .pcd extension
            Pcx = 0x0428,  //!< ZSoft PCX - .pcx extension
            Pic = 0x0429,  //!< PIC - .pic extension
            Png = 0x042A,  //!< Portable Network Graphics - .png extension
            Pnm = 0x042B,  //!< Portable Any Map - .pbm, .pgm, .ppm and .pnm extensions
            Sgi = 0x042C,  //!< Silicon Graphics - .sgi, .bw, .rgb and .rgba extensions
            Tga = 0x042D,  //!< TrueVision Targa File - .tga, .vda, .icb and .vst extensions
            Tif = 0x042E,  //!< Tagged Image File Format - .tif and .tiff extensions
            Chead = 0x042F,  //!< C-Style Header - .h extension
            Raw = 0x0430,  //!< Raw Image Data - any extension
            Mdl = 0x0431,  //!< Half-Life Model Texture - .mdl extension
            Wal = 0x0432,  //!< Quake 2 Texture - .wal extension
            Lif = 0x0434,  //!< Homeworld Texture - .lif extension
            Mng = 0x0435,  //!< Multiple-image Network Graphics - .mng extension
            Jng = 0x0435,  //!< 
            Gif = 0x0436,  //!< Graphics Interchange Format - .gif extension
            Dds = 0x0437,  //!< DirectDraw Surface - .dds extension
            Dcx = 0x0438,  //!< ZSoft Multi-PCX - .dcx extension
            Psd = 0x0439,  //!< Adobe PhotoShop - .psd extension
            Exif = 0x043A,  //!< 
            Psp = 0x043B,  //!< PaintShop Pro - .psp extension
            Pix = 0x043C,  //!< PIX - .pix extension
            Pxr = 0x043D,  //!< Pixar - .pxr extension
            Xpm = 0x043E,  //!< X Pixel Map - .xpm extension
            Hdr = 0x043F,  //!< Radiance High Dynamic Range - .hdr extension
            Icns = 0x0440,  //!< Macintosh Icon - .icns extension
            Jp2 = 0x0441,  //!< Jpeg 2000 - .jp2 extension
            Exr = 0x0442,  //!< OpenEXR - .exr extension
            Wdp = 0x0443,  //!< Microsoft HD Photo - .wdp and .hdp extension
            Vtf = 0x0444,  //!< Valve Texture Format - .vtf extension
            Wbmp = 0x0445,  //!< Wireless Bitmap - .wbmp extension
            Sun = 0x0446,  //!< Sun Raster - .sun, .ras, .rs, .im1, .im8, .im24 and .im32 extensions
            Iff = 0x0447,  //!< Interchange File Format - .iff extension
            Tpl = 0x0448,  //!< Gamecube Texture - .tpl extension
            Fits = 0x0449,  //!< Flexible Image Transport System - .fit and .fits extensions
            Dicom = 0x044A,  //!< Digital Imaging and Communications in Medicine (DICOM) - .dcm and .dicom extensions
            Iwi = 0x044B,  //!< Call of Duty Infinity Ward Image - .iwi extension
            Blp = 0x044C,  //!< Blizzard Texture Format - .blp extension
            Ftx = 0x044D,  //!< Heavy Metal: FAKK2 Texture - .ftx extension
            Rot = 0x044E,  //!< Homeworld 2 - Relic Texture - .rot extension
            Texture = 0x044F,  //!< Medieval II: Total War Texture - .texture extension
            Dpx = 0x0450,  //!< Digital Picture Exchange - .dpx extension
            Utx = 0x0451,  //!< Unreal (and Unreal Tournament) Texture - .utx extension
            Mp3 = 0x0452,  //!< MPEG-1 Audio Layer 3 - .mp3 extension
            Ktx = 0x0453,  //!< Khronos Texture - .ktx extension

            JascPal = 0x0475  //!< PaintShop Pro Palette
        }

        public enum ErrorType : ushort
        {
            // Error Types
            NoError = 0x0000,
            InvalidEnum = 0x0501,
            OutOfMemory = 0x0502,
            FormatNotSupported = 0x0503,
            InternalError = 0x0504,
            InvalidValue = 0x0505,
            IllegalOperation = 0x0506,
            IllegalFileValue = 0x0507,
            InvalidFileHeader = 0x0508,
            InvalidPARAM = 0x0509,
            CouldNotOpenFile = 0x050A,
            InvalidExtension = 0x050B,
            FileAlreadyExists = 0x050C,
            OutFormatSame = 0x050D,
            StackOverflow = 0x050E,
            StackUnderflow = 0x050F,
            InvalidConversion = 0x0510,
            BadDimensions = 0x0511,
            FileReadError = 0x0512,  // 05/12/2002: Addition by Sam.
            FileWriteError = 0x0512,

            LibGifError = 0x05E1,
            LibJpegError = 0x05E2,
            LibPngError = 0x05E3,
            LibTiffError = 0x05E4,
            LibMngError = 0x05E5,
            LibJp2Error = 0x05E6,
            LibExrError = 0x05E7,
            UnknownError = 0x05FF,
        }

        public enum OriginDefinition : ushort
        {
            Set = 0x0600,
            LowerLeft = 0x0601,
            UpperLeft = 0x0602,
            Mode = 0x0603
        }

        public enum FormatAndTypeModeDefinition : ushort
        {
            FormatSet = 0x0610,
            FormatMode = 0x0611,
            TypeSet = 0x0612,
            TypeMode = 0x0613,
        }

        public enum FileDefinition : ushort
        {
            Overwrite = 0x0620,
            Mode = 0x0621,
        }

        public enum PaletteDefinition : ushort
        {
            ConvPal = 0x0630
        }

        public enum LoadFailDefinition : ushort
        {
            DefaultOnFail = 0x0632
        }

        public enum KeyColourAndAlphaDefinition : ushort
        {
            UseKeyColor = 0x0635,
            BlitBlend = 0x0636
        }

        public enum InterlaceDefinition : ushort
        {
            SaveInterlaced = 0x0639,
            InterlaceMode = 0x063A,
        }

        public enum QuantizationDefinition : ushort
        {
            QuantizationMode = 0x0640,
            WuQuant = 0x0641,
            NeuQuant = 0x0642,
            NeuQuantSample = 0x0643,
            MaxQuantIndices = 0x0644, // Redefined, since the above #define is misspelled
        }

        public enum Hint : ushort
        {
            Fastest = 0x0660,
            LessMem = 0x0661,
            DontCare = 0x0662,
            MemSpeedHint = 0x0665,
            UseCompression = 0x0666,
            NoCompression = 0x0667,
            CompressionHint = 0x0668,
        }

        public enum Compression : ushort
        {
            NvidiaCompress = 0x0670,
            SquishCompress = 0x0671,
        }

        public enum SubimageType : ushort
        {
            SubNext = 0x0680,
            SubMipmap = 0x0681,
            SubLayer = 0x0682,
        }

        public enum CompressionDefinition : ushort
        {
            Mode = 0x0700,
            None = 0x0701,
            Rle = 0x0702,
            Lzo = 0x0703,
            Zlib = 0x0704,
        }

        public enum FileFormatSpecificValue : ushort
        {
            TgaCreateStamp = 0x0710,
            JpgQuality = 0x0711,
            PngInterlace = 0x0712,
            TgaRle = 0x0713,
            BmpRle = 0x0714,
            SgiRle = 0x0715,
            TgaIdString = 0x0717,
            TgaAuthnameString = 0x0718,
            TgaAuthcommentString = 0x0719,
            PngAuthnameString = 0x071A,
            PngTtileString = 0x071B,
            PngDescriptionString = 0x071C,
            TifDescriptionString = 0x071D,
            TifHostcomputerString = 0x071E,
            TifDocumentnameString = 0x071F,
            TifAuthnameString = 0x0720,
            JpgSaveFormat = 0x0721,
            CheadHeaderString = 0x0722,
            PcdPicnum = 0x0723,
            PngALPHA_INDEX = 0x0724, // currently has no effect!
            JpgProgressive = 0x0725,
            VtfComp = 0x0726,
        }

        public enum DxtcDefinition : ushort
        {
            DxtcFormat = 0x0705,
            Dxt1 = 0x0706,
            Dxt2 = 0x0707,
            Dxt3 = 0x0708,
            Dxt4 = 0x0709,
            Dxt5 = 0x070A,
            DxtNoComp = 0x070B,
            KeepDxtcData = 0x070C,
            DctcDataFormat = 0x070D,
            _3DC = 0x070E,
            Rxgb = 0x070F,
            Ati1N = 0x0710,
            Dxt1A = 0x0711,  // Normally the same as IL_DXT1, except for nVidia Texture Tools.
        }

        public enum EnvironmentMapDefinition : uint
        {
            CubemapPositiveZX = 0x00000400,
            CubemapNegativeX = 0x00000800,
            CubemapPositiveZY = 0x00001000,
            CubemapNegativeY = 0x00002000,
            CubemapPositiveZZ = 0x00004000,
            CubemapNegativeZ = 0x00008000,
            Spheremap = 0x00010000,
        }

        public enum Value : ushort
        {
            VersionNum = 0x0DE2,
            ImageWidth = 0x0DE4,
            ImageHeight = 0x0DE5,
            ImageDepth = 0x0DE6,
            ImageSizeOfData = 0x0DE7,
            ImageBpp = 0x0DE8,
            ImageBytesPerPixel = 0x0DE8,
            ImageBitsPerPixel = 0x0DE9,
            ImageFormat = 0x0DEA,
            ImageType = 0x0DEB,
            PaletteType = 0x0DEC,
            PaletteSize = 0x0DED,
            PaletteBpp = 0x0DEE,
            PaletteNumCols = 0x0DEF,
            PaletteBaseType = 0x0DF0,
            NumFaces = 0x0DE1,
            NumImages = 0x0DF1,
            NumMipmaps = 0x0DF2,
            NumLayers = 0x0DF3,
            ActiveImage = 0x0DF4,
            ActiveMipmap = 0x0DF5,
            ActiveLayer = 0x0DF6,
            ActiveFace = 0x0E00,
            CurImage = 0x0DF7,
            ImageDuration = 0x0DF8,
            ImagePlanesize = 0x0DF9,
            ImageBpc = 0x0DFA,
            ImageOffx = 0x0DFB,
            ImageOffy = 0x0DFC,
            ImageCubeflags = 0x0DFD,
            ImageOrigin = 0x0DFE,
            ImageChannels = 0x0DFF,
        }

        public enum FeatureControls : uint
        {
            OriginSet = OriginDefinition.Set,
            FormatSet = FormatAndTypeModeDefinition.FormatSet,
            TypeSet = FormatAndTypeModeDefinition.TypeSet,
            FileOverwrite = FileDefinition.Overwrite,
            ConvPal = PaletteDefinition.ConvPal,
            DefaultOnFail = LoadFailDefinition.DefaultOnFail,
            UseKeyColor = KeyColourAndAlphaDefinition.UseKeyColor,
            BlitBlend = KeyColourAndAlphaDefinition.BlitBlend,
            SaveInterlaced = InterlaceDefinition.SaveInterlaced,
            JpgProgressive = FileFormatSpecificValue.JpgProgressive,
            NvidiaCompress = Compression.NvidiaCompress,
        }

        #endregion
        
        #region Callback Functions

        // Callback functions for file reading
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fCloseRProc(IntPtr handle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public delegate bool fEofProc(IntPtr handle);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int fGetcProc(IntPtr handle);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr fOpenRProc([MarshalAs(UnmanagedType.LPWStr)] string str);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int fReadProc(IntPtr ptr1, uint a, uint b, IntPtr ptr2);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int fSeekRProc(IntPtr handle, int a, int b);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int fTellRProc(IntPtr handle);

        // Callback functions for file writing
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void fCloseWProc(IntPtr handle);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr fOpenWProc([MarshalAs(UnmanagedType.LPWStr)] string str);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int fPutcProc(char c, IntPtr handle);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int fSeekWProc(IntPtr handle, int a, int b);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int fTellWProc(IntPtr handle);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int fWriteProc(IntPtr ptr1, uint a, uint b, IntPtr ptr2);

        // Callback functions for allocation and deallocation
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr mAlloc(ulong size);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void mFree(IntPtr ptr);

        // Registered format procedures
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint IL_LOADPROC([MarshalAs(UnmanagedType.LPWStr)] string str);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate uint IL_SAVEPROC([MarshalAs(UnmanagedType.LPWStr)] string str);

        #endregion

        #region Functions

        // ImageLib Functions
        public unsafe static uint ActiveFace(uint number)
        {
            return Common.IsWow64() ? x64.ilActiveFace(number) : x86.ilActiveFace(number);
        }

        public unsafe static bool ActiveImage(uint number)
        {
            return Common.IsWow64() ? x64.ilActiveImage(number) : x86.ilActiveImage(number);
        }

        public unsafe static bool ActiveLayer(uint number)
        {
            return Common.IsWow64() ? x64.ilActiveLayer(number) : x86.ilActiveLayer(number);
        }

        public unsafe static bool ActiveMipmap(uint number)
        {
            return Common.IsWow64() ? x64.ilActiveMipmap(number) : x86.ilActiveMipmap(number);
        }

        public unsafe static bool ApplyPal(string fileName)
        {
            return Common.IsWow64() ? x64.ilApplyPal(fileName) : x86.ilApplyPal(fileName);
        }

        public unsafe static bool ApplyProfile(string inProfile, string outProfile)
        {
            return Common.IsWow64() ? x64.ilApplyProfile(inProfile, outProfile) : x86.ilApplyProfile(inProfile, outProfile);
        }
        public unsafe static void BindImage(uint image)
        {
            if (Common.IsWow64()) x64.ilBindImage(image); else x86.ilBindImage(image);
        }

        public unsafe static bool Blit(uint source,
            int destX, int destY, int destZ,
            uint srcX, uint srcY, uint srcZ,
            uint width, uint height, uint depth)
        {
            return Common.IsWow64()
                ? x64.ilBlit(source,
                    destX, destY, destZ,
                    srcX, srcY, srcZ,
                    width, height, depth)
                : x86.ilBlit(source,
                    destX, destY, destZ,
                    srcX, srcY, srcZ,
                    width, height, depth);
        }

        public unsafe static bool ClampNTSC()
        {
            return Common.IsWow64() ? x64.ilClampNTSC() : x86.ilClampNTSC();
        }

        public unsafe static void ClearColour(float red, float green, float blue, float alpha)
        {
            if (Common.IsWow64()) x64.ilClearColour(red, green, blue, alpha); else x86.ilClearColour(red, green, blue, alpha);
        }

        public unsafe static bool ClearImage()
        {
            return Common.IsWow64() ? x64.ilClearImage() : x86.ilClearImage();
        }

        public unsafe static uint CloneCurImage()
        {
            return Common.IsWow64() ? x64.ilCloneCurImage() : x86.ilCloneCurImage();
        }

        public unsafe static byte* CompressDXT(byte* data,
            uint width, uint height, uint depth, DxtcDefinition dxtcFormat, uint* dxtcSize)
        {
            return Common.IsWow64()
                ? x64.ilCompressDXT(data, width, height, depth, dxtcFormat, dxtcSize)
                : x86.ilCompressDXT(data, width, height, depth, dxtcFormat, dxtcSize);
        }

        public unsafe static bool CompressFunc(Compression mode)
        {
            return Common.IsWow64() ? x64.ilCompressFunc(mode) : x86.ilCompressFunc(mode);
        }

        public unsafe static bool ConvertImage(DataFormat destFormat, DataType destType)
        {
            return Common.IsWow64() ? x64.ilConvertImage(destFormat, destType) : x86.ilConvertImage(destFormat, destType);
        }

        public unsafe static bool ConvertPal(PaletteType destFormat)
        {
            return Common.IsWow64() ? x64.ilConvertPal(destFormat) : x86.ilConvertPal(destFormat);
        }

        public unsafe static bool CopyImage(uint src)
        {
            return Common.IsWow64() ? x64.ilCopyImage(src) : x86.ilCopyImage(src);
        }

        public unsafe static uint CopyPixels(
            uint xOff, uint yOff, uint zOff,
            uint width, uint height, uint depth,
            DataFormat format, DataType type, IntPtr data)
        {
            return Common.IsWow64()
                ? x64.ilCopyPixels(
                    xOff, yOff, zOff,
                    width, height, depth,
                    format, type, data)
                : x86.ilCopyPixels(
                    xOff, yOff, zOff,
                    width, height, depth,
                    format, type, data);
        }

        public unsafe static uint CreateSubImage(SubimageType type, uint num)
        {
            return Common.IsWow64() ? x64.ilCreateSubImage(type, num) : x86.ilCreateSubImage(type, num);
        }

        public unsafe static bool DefaultImage()
        {
            return Common.IsWow64() ? x64.ilDefaultImage() : x86.ilDefaultImage();
        }

        public unsafe static void DeleteImage(uint num)
        {
            if (Common.IsWow64()) x64.ilDeleteImage(num); else x86.ilDeleteImage(num);
        }

        public unsafe static void DeleteImages(UIntPtr num, uint* images)
        {
            if (Common.IsWow64()) x64.ilDeleteImages(num, images); else x86.ilDeleteImages(num, images);
        }

        public unsafe static ImageType DetermineType(string fileName)
        {
            return Common.IsWow64() ? x64.ilDetermineType(fileName) : x86.ilDetermineType(fileName);
        }

        public unsafe static ImageType DetermineTypeF(IntPtr fileHandle)
        {
            return Common.IsWow64() ? x64.ilDetermineTypeF(fileHandle) : x86.ilDetermineTypeF(fileHandle);
        }

        public unsafe static ImageType DetermineTypeL(IntPtr lump, uint size)
        {
            return Common.IsWow64() ? x64.ilDetermineTypeL(lump, size) : x86.ilDetermineTypeL(lump, size);
        }

        public unsafe static bool Disable(uint mode)
        {
            return Common.IsWow64() ? x64.ilDisable(mode) : x86.ilDisable(mode);
        }

        public unsafe static bool Disable(FeatureControls mode)
        {
            return Disable((uint) mode);
        }

        public unsafe static bool DxtcDataToImage()
        {
            return Common.IsWow64() ? x64.ilDxtcDataToImage() : x86.ilDxtcDataToImage();
        }

        public unsafe static bool DxtcDataToSurface()
        {
            return Common.IsWow64() ? x64.ilDxtcDataToSurface() : x86.ilDxtcDataToSurface();
        }

        public unsafe static bool Enable(uint mode)
        {
            return Common.IsWow64() ? x64.ilEnable(mode) : x86.ilEnable(mode);
        }

        public unsafe static bool Enable(FeatureControls mode)
        {
            return Enable((uint) mode);
        }

        public unsafe static void FlipSurfaceDxtcData()
        {
            if (Common.IsWow64()) x64.ilFlipSurfaceDxtcData(); else x86.ilFlipSurfaceDxtcData();
        }

        public unsafe static bool FormatFunc(DataFormat mode)
        {
            return Common.IsWow64() ? x64.ilFormatFunc(mode) : x86.ilFormatFunc(mode);
        }

        public unsafe static void GenImages(UIntPtr num, uint* images)
        {
            if (Common.IsWow64()) x64.ilGenImages(num, images); else x86.ilGenImages(num, images);
        }

        public unsafe static uint GenImage()
        {
            return Common.IsWow64() ? x64.ilGenImage() : x86.ilGenImage();
        }

        public unsafe static byte* GetAlpha(DataType type)
        {
            return Common.IsWow64() ? x64.ilGetAlpha(type) : x86.ilGetAlpha(type);
        }

        public unsafe static bool GetBoolean(uint mode)
        {
            return Common.IsWow64() ? x64.ilGetBoolean(mode) : x86.ilGetBoolean(mode);
        }

        public unsafe static bool GetBoolean(FeatureControls mode)
        {
            return GetBoolean((uint) mode);
        }

        public unsafe static void GetBooleanv(uint mode, out bool param)
        {
            if (Common.IsWow64()) x64.ilGetBooleanv(mode, out param); else x86.ilGetBooleanv(mode, out param);
        }

        public unsafe static void GetBooleanv(FeatureControls mode, out bool param)
        {
            GetBooleanv((uint)mode, out param);
        }

        public unsafe static byte* GetData()
        {
            return Common.IsWow64() ? x64.ilGetData() : x86.ilGetData();
        }

        public unsafe static uint GetDXTCData(IntPtr buffer, uint bufferSize, DxtcDefinition dxtcFormat)
        {
            return Common.IsWow64() ? x64.ilGetDXTCData(buffer, bufferSize, dxtcFormat) : x86.ilGetDXTCData(buffer, bufferSize, dxtcFormat);
        }

        public unsafe static ErrorType GetError()
        {
            return Common.IsWow64() ? (ErrorType) x64.ilGetError() : (ErrorType) x86.ilGetError();
        }

        public unsafe static uint GetErrorUint()
        {
            return Common.IsWow64() ? x64.ilGetError() : x86.ilGetError();
        }

        public unsafe static int GetInteger(FileFormatSpecificValue mode)
        {
            return Common.IsWow64() ? x64.ilGetInteger((uint)mode) : x86.ilGetInteger((uint)mode);
        }

        public unsafe static int GetInteger(Value mode)
        {
            return Common.IsWow64() ? x64.ilGetInteger((uint)mode) : x86.ilGetInteger((uint)mode);
        }

        public unsafe static void GetIntegerv(Value mode, out int param)
        {
            if (Common.IsWow64()) x64.ilGetIntegerv(mode, out param); else x86.ilGetIntegerv(mode, out param);
        }

        public unsafe static uint GetLumpPos()
        {
            return Common.IsWow64() ? x64.ilGetLumpPos() : x86.ilGetLumpPos();
        }

        public unsafe static byte* GetPalette()
        {
            return Common.IsWow64() ? x64.ilGetPalette() : x86.ilGetPalette();
        }

        public unsafe static string GetString(uint stringName)
        {
            return Common.IsWow64() ? x64.ilGetString(stringName) : x86.ilGetString(stringName);
        }

        public unsafe static void SetHint(Hint target, Hint mode)
        {
            if (Common.IsWow64()) x64.ilHint(target, mode); else x86.ilHint(target, mode);
        }

        public unsafe static bool InvertSurfaceDxtcDataAlpha()
        {
            return Common.IsWow64() ? x64.ilInvertSurfaceDxtcDataAlpha() : x86.ilInvertSurfaceDxtcDataAlpha();
        }

        public unsafe static void Init()
        {
            if (Common.IsWow64()) x64.ilInit(); else x86.ilInit();
        }

        public unsafe static bool ImageToDxtcData(DxtcDefinition format)
        {
            return Common.IsWow64() ? x64.ilImageToDxtcData(format) : x86.ilImageToDxtcData(format);
        }

        public unsafe static bool IsDisabled(uint mode)
        {
            return Common.IsWow64() ? x64.ilIsDisabled(mode) : x86.ilIsDisabled(mode);
        }

        public unsafe static bool IsEnabled(uint mode)
        {
            return Common.IsWow64() ? x64.ilIsEnabled(mode) : x86.ilIsEnabled(mode);
        }

        public unsafe static bool IsImage(uint image)
        {
            return Common.IsWow64() ? x64.ilIsImage(image) : x86.ilIsImage(image);
        }

        public unsafe static bool IsValid(ImageType type, string fileName)
        {
            return Common.IsWow64() ? x64.ilIsValid(type, fileName) : x86.ilIsValid(type, fileName);
        }

        public unsafe static bool IsValidF(ImageType type, IntPtr fileHandle)
        {
            return Common.IsWow64() ? x64.ilIsValidF(type, fileHandle) : x86.ilIsValidF(type, fileHandle);
        }

        public unsafe static bool IsValidL(ImageType type, IntPtr lump, uint size)
        {
            return Common.IsWow64() ? x64.ilIsValidL(type, lump, size) : x86.ilIsValidL(type, lump, size);
        }

        public unsafe static void KeyColour(float red, float green, float blue, float alpha)
        {
            if (Common.IsWow64()) x64.ilKeyColour(red, green, blue, alpha); else x86.ilKeyColour(red, green, blue, alpha);
        }

        public unsafe static bool Load(ImageType type, string fileName)
        {
            return Common.IsWow64() ? x64.ilLoad(type, fileName) : x86.ilLoad(type, fileName);
        }

        public unsafe static bool LoadF(ImageType type, IntPtr fileHandle)
        {
            return Common.IsWow64() ? x64.ilLoadF(type, fileHandle) : x86.ilLoadF(type, fileHandle);
        }

        public unsafe static bool LoadImage(string fileName)
        {
            return Common.IsWow64() ? x64.ilLoadImage(fileName) : x86.ilLoadImage(fileName);
        }

        public unsafe static bool LoadL(ImageType type, IntPtr lump, uint size)
        {
            return Common.IsWow64() ? x64.ilLoadL(type, lump, size) : x86.ilLoadL(type, lump, size);
        }

        public unsafe static bool LoadPal(string fileName)
        {
            return Common.IsWow64() ? x64.ilLoadPal(fileName) : x86.ilLoadPal(fileName);
        }

        public unsafe static void ModAlpha(double alphaValue)
        {
            if (Common.IsWow64()) x64.ilModAlpha(alphaValue); else x86.ilModAlpha(alphaValue);
        }

        public unsafe static bool OriginFunc(OriginDefinition mode)
        {
            return Common.IsWow64() ? x64.ilOriginFunc(mode) : x86.ilOriginFunc(mode);
        }

        public unsafe static bool OverlayImage(uint source, int xCoord, int yCoord, int zCoord)
        {
            return Common.IsWow64() ? x64.ilOverlayImage(source, xCoord, yCoord, zCoord) : x86.ilOverlayImage(source, xCoord, yCoord, zCoord);
        }

        public unsafe static void PopAttrib()
        {
            if (Common.IsWow64()) x64.ilPopAttrib(); else x86.ilPopAttrib();
        }

        public unsafe static void PushAttrib(uint bits)
        {
            if (Common.IsWow64()) x64.ilPushAttrib(bits); else x86.ilPushAttrib(bits);
        }

        public unsafe static void RegisterFormat(DataFormat format)
        {
            if (Common.IsWow64()) x64.ilRegisterFormat(format); else x86.ilRegisterFormat(format);
        }

        public unsafe static bool RegisterLoad(string ext, IL_LOADPROC loadCb)
        {
            return Common.IsWow64() ? x64.ilRegisterLoad(ext, loadCb) : x86.ilRegisterLoad(ext, loadCb);
        }

        public unsafe static bool RegisterMipNum(uint num)
        {
            return Common.IsWow64() ? x64.ilRegisterMipNum(num) : x86.ilRegisterMipNum(num);
        }

        public unsafe static bool RegisterNumFaces(uint num)
        {
            return Common.IsWow64() ? x64.ilRegisterNumFaces(num) : x86.ilRegisterNumFaces(num);
        }

        public unsafe static bool RegisterNumImages(uint num)
        {
            return Common.IsWow64() ? x64.ilRegisterNumImages(num) : x86.ilRegisterNumImages(num);
        }

        public unsafe static void RegisterOrigin(OriginDefinition origin)
        {
            if (Common.IsWow64()) x64.ilRegisterOrigin(origin); else x86.ilRegisterOrigin(origin);
        }

        public unsafe static void RegisterPal(IntPtr pal, uint size, PaletteType type)
        {
            if (Common.IsWow64()) x64.ilRegisterPal(pal, size, type); else x86.ilRegisterPal(pal, size, type);
        }

        public unsafe static bool RegisterSave(string ext, IL_SAVEPROC saveCb)
        {
            return Common.IsWow64() ? x64.ilRegisterSave(ext, saveCb) : x86.ilRegisterSave(ext, saveCb);
        }

        public unsafe static void RegisterType(DataType type)
        {
            if (Common.IsWow64()) x64.ilRegisterType(type); else x86.ilRegisterType(type);
        }

        public unsafe static bool RemoveLoad(string ext)
        {
            return Common.IsWow64() ? x64.ilRemoveLoad(ext) : x86.ilRemoveLoad(ext);
        }

        public unsafe static bool RemoveSave(string ext)
        {
            return Common.IsWow64() ? x64.ilRemoveSave(ext) : x86.ilRemoveSave(ext);
        }

        public unsafe static void ResetMemory() // Deprecated
        {
            if (Common.IsWow64()) x64.ilResetMemory(); else x86.ilResetMemory();
        }

        public unsafe static void ResetRead()
        {
            if (Common.IsWow64()) x64.ilResetRead(); else x86.ilResetRead();
        }

        public unsafe static void ResetWrite()
        {
            if (Common.IsWow64()) x64.ilResetWrite(); else x86.ilResetWrite();
        }

        public unsafe static bool Save(ImageType type, string fileName)
        {
            return Common.IsWow64() ? x64.ilSave(type, fileName) : x86.ilSave(type, fileName);
        }

        public unsafe static uint SaveF(ImageType type, IntPtr fileHandle)
        {
            return Common.IsWow64() ? x64.ilSaveF(type, fileHandle) : x86.ilSaveF(type, fileHandle);
        }

        public unsafe static bool SaveImage(string fileName)
        {
            return Common.IsWow64() ? x64.ilSaveImage(fileName) : x86.ilSaveImage(fileName);
        }

        public unsafe static uint SaveL(ImageType type, IntPtr lump, uint size)
        {
            return Common.IsWow64() ? x64.ilSaveL((uint)type, lump, size) : x86.ilSaveL((uint)type, lump, size);
        }

        public unsafe static bool SavePal(string fileName)
        {
            return Common.IsWow64() ? x64.ilSavePal(fileName) : x86.ilSavePal(fileName);
        }

        public unsafe static bool SetAlpha(double alphaValue)
        {
            return Common.IsWow64() ? x64.ilSetAlpha(alphaValue) : x86.ilSetAlpha(alphaValue);
        }

        public unsafe static bool SetData(IntPtr data)
        {
            return Common.IsWow64() ? x64.ilSetData(data) : x86.ilSetData(data);
        }

        public unsafe static bool SetDuration(uint duration)
        {
            return Common.IsWow64() ? x64.ilSetDuration(duration) : x86.ilSetDuration(duration);
        }

        public unsafe static void SetInteger(uint mode, int param)
        {
            if (Common.IsWow64()) x64.ilSetInteger(mode, param); else x86.ilSetInteger(mode, param);
        }

        public unsafe static void SetMemory(
            mAlloc mAllocCb,
            mFree mFreeCb)
        {
            if (Common.IsWow64()) x64.ilSetMemory(mAllocCb, mFreeCb); else x86.ilSetMemory(mAllocCb, mFreeCb);
        }

        public unsafe static void SetPixels(
            int xOff, int yOff, int zOff,
            uint width, uint height, uint depth,
            DataFormat format, DataType type, IntPtr data)
        {
            if (Common.IsWow64())
                x64.ilSetPixels(
                    xOff, yOff, zOff,
                    width, height, depth,
                    format, type, data);
            else
                x86.ilSetPixels(
                    xOff, yOff, zOff,
                    width, height, depth,
                    format, type, data);
        }

        public unsafe static void SetRead(
            fOpenRProc fOpenRProcCb,
            fCloseRProc fCloseRProcCb,
            fEofProc fEofProcCb,
            fGetcProc fGetcProcCb,
            fReadProc fReadProcCb,
            fSeekRProc fSeekRProcCb,
            fTellRProc fTellRProcCb)
        {
            if (Common.IsWow64())
                x64.ilSetRead(
                    fOpenRProcCb, fCloseRProcCb, fEofProcCb,
                    fGetcProcCb, fReadProcCb, fSeekRProcCb,
                    fTellRProcCb);
            else
                x86.ilSetRead(
                    fOpenRProcCb, fCloseRProcCb, fEofProcCb,
                    fGetcProcCb, fReadProcCb, fSeekRProcCb,
                    fTellRProcCb);
        }

        public unsafe static void SetString(FileFormatSpecificValue mode, string str)
        {
            if (Common.IsWow64()) x64.ilSetString(mode, str); else x86.ilSetString(mode, str);
        }

        public unsafe static void SetWrite(
            fOpenWProc fOpenWProcCb,
            fCloseWProc fCloseWProcCb,
            fPutcProc fPutcProcCb,
            fSeekWProc fSeekWProcCb,
            fTellWProc fTellWProcCb,
            fWriteProc fWriteProcCb)
        {
            if (Common.IsWow64())
                x64.ilSetWrite(
                    fOpenWProcCb, fCloseWProcCb, fPutcProcCb,
                    fSeekWProcCb, fTellWProcCb, fWriteProcCb);
            else
                x86.ilSetWrite(
                    fOpenWProcCb, fCloseWProcCb, fPutcProcCb,
                    fSeekWProcCb, fTellWProcCb, fWriteProcCb);
        }

        public unsafe static void ShutDown()
        {
            if (Common.IsWow64()) x64.ilShutDown(); else x86.ilShutDown();
        }

        public unsafe static bool SurfaceToDxtcData(DxtcDefinition format)
        {
            return Common.IsWow64() ? x64.ilSurfaceToDxtcData(format) : x86.ilSurfaceToDxtcData(format);
        }

        public unsafe static bool TexImage(
            uint width, uint height, uint depth,
            byte numChannels, DataFormat dataFormat, DataType dataType,
            IntPtr dataPtr)
        {
            return Common.IsWow64()
                ? x64.ilTexImage(
                    width, height, depth,
                    numChannels, dataFormat, dataType,
                    dataPtr)
                : x86.ilTexImage(
                    width, height, depth,
                    numChannels, dataFormat, dataType,
                    dataPtr);
        }

        public unsafe static bool TexImageDxtc(int w, int h, int d, DxtcDefinition dxtcFormat, byte* data)
        {
            return Common.IsWow64() ? x64.ilTexImageDxtc(w, h, d, dxtcFormat, data) : x86.ilTexImageDxtc(w, h, d, dxtcFormat, data);
        }

        public unsafe static ImageType TypeFromExt(string fileName)
        {
            return Common.IsWow64() ? x64.ilTypeFromExt(fileName) : x86.ilTypeFromExt(fileName);
        }

        public unsafe static bool TypeFunc(DataFormat mode)
        {
            return Common.IsWow64() ? x64.ilTypeFunc(mode) : x86.ilTypeFunc(mode);
        }

        public unsafe static bool LoadData(
            string fileName,
            uint width, uint height, uint depth, byte bpp)
        {
            return Common.IsWow64()
                ? x64.ilLoadData(fileName, width, height, depth, bpp)
                : x86.ilLoadData(fileName, width, height, depth, bpp);
        }

        public unsafe static bool LoadDataF(IntPtr fileHandle,
            uint width, uint height, uint depth, byte bpp)
        {
            return Common.IsWow64()
                ? x64.ilLoadDataF(fileHandle, width, height, depth, bpp)
                : x86.ilLoadDataF(fileHandle, width, height, depth, bpp);
        }

        public unsafe static bool LoadDataL(IntPtr lump,
            uint size, uint width, uint height, uint depth, byte bpp)
        {
            return Common.IsWow64()
                ? x64.ilLoadDataL(lump, size, width, height, depth, bpp)
                : x86.ilLoadDataL(lump, size, width, height, depth, bpp);
        }

        public unsafe static bool SaveData(string fileName)
        {
            return Common.IsWow64() ? x64.ilSaveData(fileName) : x86.ilSaveData(fileName);
        }

        private static class x86
        {
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilActiveFace(uint number);

            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilActiveImage(uint number);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilActiveLayer(uint number);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilActiveMipmap(uint number);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilApplyPal([MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilApplyProfile(
                [MarshalAs(UnmanagedType.LPWStr)] string inProfile,
                [MarshalAs(UnmanagedType.LPWStr)] string outProfile);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilBindImage(uint image);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilBlit(uint source,
                int destX, int destY, int destZ,
                uint srcX, uint srcY, uint srcZ,
                uint width, uint height, uint depth);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilClampNTSC();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilClearColour(float red, float green, float blue, float alpha);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilClearImage();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilCloneCurImage();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern byte* ilCompressDXT(byte* data,
                uint width, uint height, uint depth, DxtcDefinition dxtcFormat, uint* dxtcSize);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilCompressFunc(Compression mode);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilConvertImage(DataFormat destFormat, DataType destType);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilConvertPal(PaletteType destFormat);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilCopyImage(uint src);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilCopyPixels(
                uint xOff, uint yOff, uint zOff,
                uint width, uint height, uint depth,
                DataFormat format, DataType type, IntPtr data);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilCreateSubImage(SubimageType type, uint num);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilDefaultImage();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilDeleteImage(uint num);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilDeleteImages(UIntPtr num, uint* images);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern ImageType ilDetermineType([MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern ImageType ilDetermineTypeF(IntPtr fileHandle);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern ImageType ilDetermineTypeL(IntPtr lump, uint size);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilDisable(uint mode);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilDxtcDataToImage();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilDxtcDataToSurface();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilEnable(uint mode);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilFlipSurfaceDxtcData();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilFormatFunc(DataFormat mode);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilGenImages(UIntPtr num, uint* images);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilGenImage();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern byte* ilGetAlpha(DataType type);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilGetBoolean(uint mode);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilGetBooleanv(uint mode, out bool param);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern byte* ilGetData();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilGetDXTCData(IntPtr buffer, uint bufferSize, DxtcDefinition dxtcFormat);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilGetError();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern int ilGetInteger(uint mode);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilGetIntegerv(Value mode, out int param);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilGetLumpPos();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern byte* ilGetPalette();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPWStr)]
            public unsafe static extern string ilGetString(uint stringName);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilHint(Hint target, Hint mode);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilInvertSurfaceDxtcDataAlpha();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilInit();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilImageToDxtcData(DxtcDefinition format);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilIsDisabled(uint mode);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilIsEnabled(uint mode);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilIsImage(uint image);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilIsValid(ImageType type, [MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilIsValidF(ImageType type, IntPtr fileHandle);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilIsValidL(ImageType type, IntPtr lump, uint size);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilKeyColour(float red, float green, float blue, float alpha);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoad(ImageType type, [MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoadF(ImageType type, IntPtr fileHandle);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoadImage([MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoadL(ImageType type, IntPtr lump, uint size);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoadPal([MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilModAlpha(double alphaValue);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilOriginFunc(OriginDefinition mode);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilOverlayImage(uint source, int xCoord, int yCoord, int zCoord);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilPopAttrib();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilPushAttrib(uint bits);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilRegisterFormat(DataFormat format);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilRegisterLoad(
                [MarshalAs(UnmanagedType.LPWStr)] string ext,
                [MarshalAs(UnmanagedType.FunctionPtr)] IL_LOADPROC loadCb);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilRegisterMipNum(uint num);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilRegisterNumFaces(uint num);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilRegisterNumImages(uint num);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilRegisterOrigin(OriginDefinition origin);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilRegisterPal(IntPtr pal, uint size, PaletteType type);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilRegisterSave(
                [MarshalAs(UnmanagedType.LPWStr)] string ext,
                [MarshalAs(UnmanagedType.FunctionPtr)] IL_SAVEPROC saveCb);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilRegisterType(DataType type);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilRemoveLoad([MarshalAs(UnmanagedType.LPWStr)] string ext);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilRemoveSave([MarshalAs(UnmanagedType.LPWStr)] string ext);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilResetMemory(); // Deprecated
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilResetRead();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilResetWrite();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSave(ImageType type, [MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilSaveF(ImageType type, IntPtr fileHandle);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSaveImage([MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilSaveL(uint type, IntPtr lump, uint size);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSavePal([MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSetAlpha(double alphaValue);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSetData(IntPtr data);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSetDuration(uint duration);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilSetInteger(uint mode, int param);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilSetMemory(
                [MarshalAs(UnmanagedType.FunctionPtr)] mAlloc mAllocCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] mFree mFreeCb);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilSetPixels(
                int xOff, int yOff, int zOff,
                uint width, uint height, uint depth,
                 DataFormat format, DataType type, IntPtr data);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilSetRead(
                [MarshalAs(UnmanagedType.FunctionPtr)] fOpenRProc fOpenRProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fCloseRProc fCloseRProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fEofProc fEofProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fGetcProc fGetcProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fReadProc fReadProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fSeekRProc fSeekRProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fTellRProc fTellRProcCb);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilSetString(FileFormatSpecificValue mode, [MarshalAs(UnmanagedType.LPStr)] string str);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilSetWrite(
                [MarshalAs(UnmanagedType.FunctionPtr)] fOpenWProc fOpenWProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fCloseWProc fCloseWProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fPutcProc fPutcProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fSeekWProc fSeekWProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fTellWProc fTellWProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fWriteProc fWriteProcCb);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilShutDown();
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSurfaceToDxtcData(DxtcDefinition format);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilTexImage(
                uint width, uint height, uint depth,
                byte numChannels, DataFormat dataFormat, DataType dataType,
                IntPtr dataPtr);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilTexImageDxtc(int w, int h, int d, DxtcDefinition dxtcFormat, byte* data);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern ImageType ilTypeFromExt([MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilTypeFunc(DataFormat mode);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoadData(
                [MarshalAs(UnmanagedType.LPWStr)] string fileName,
                uint width, uint height, uint depth, byte bpp);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoadDataF(IntPtr fileHandle,
                uint width, uint height, uint depth, byte bpp);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoadDataL(IntPtr lump,
                uint size, uint width, uint height, uint depth, byte bpp);
            [DllImport("DevIL.x86.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSaveData([MarshalAs(UnmanagedType.LPWStr)] string fileName);
        }

        private static class x64
        {
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilActiveFace(uint number);

            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilActiveImage(uint number);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilActiveLayer(uint number);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilActiveMipmap(uint number);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilApplyPal([MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilApplyProfile(
                [MarshalAs(UnmanagedType.LPWStr)] string inProfile,
                [MarshalAs(UnmanagedType.LPWStr)] string outProfile);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilBindImage(uint image);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilBlit(uint source,
                int destX, int destY, int destZ,
                uint srcX, uint srcY, uint srcZ,
                uint width, uint height, uint depth);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilClampNTSC();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilClearColour(float red, float green, float blue, float alpha);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilClearImage();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilCloneCurImage();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern byte* ilCompressDXT(byte* data,
                uint width, uint height, uint depth, DxtcDefinition dxtcFormat, uint* dxtcSize);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilCompressFunc(Compression mode);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilConvertImage(DataFormat destFormat, DataType destType);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilConvertPal(PaletteType destFormat);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilCopyImage(uint src);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilCopyPixels(
                uint xOff, uint yOff, uint zOff,
                uint width, uint height, uint depth,
                DataFormat format, DataType type, IntPtr data);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilCreateSubImage(SubimageType type, uint num);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilDefaultImage();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilDeleteImage(uint num);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilDeleteImages(UIntPtr num, uint* images);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern ImageType ilDetermineType([MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern ImageType ilDetermineTypeF(IntPtr fileHandle);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern ImageType ilDetermineTypeL(IntPtr lump, uint size);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilDisable(uint mode);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilDxtcDataToImage();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilDxtcDataToSurface();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilEnable(uint mode);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilFlipSurfaceDxtcData();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilFormatFunc(DataFormat mode);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilGenImages(UIntPtr num, uint* images);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilGenImage();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern byte* ilGetAlpha(DataType type);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilGetBoolean(uint mode);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilGetBooleanv(uint mode, out bool param);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern byte* ilGetData();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilGetDXTCData(IntPtr buffer, uint bufferSize, DxtcDefinition dxtcFormat);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilGetError();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern int ilGetInteger(uint mode);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilGetIntegerv(Value mode, out int param);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilGetLumpPos();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern byte* ilGetPalette();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPWStr)]
            public unsafe static extern string ilGetString(uint stringName);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilHint(Hint target, Hint mode);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilInvertSurfaceDxtcDataAlpha();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilInit();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilImageToDxtcData(DxtcDefinition format);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilIsDisabled(uint mode);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilIsEnabled(uint mode);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilIsImage(uint image);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilIsValid(ImageType type, [MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilIsValidF(ImageType type, IntPtr fileHandle);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilIsValidL(ImageType type, IntPtr lump, uint size);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilKeyColour(float red, float green, float blue, float alpha); 
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoad(ImageType type, [MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoadF(ImageType type, IntPtr fileHandle);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoadImage([MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoadL(ImageType type, IntPtr lump, uint size);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoadPal([MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilModAlpha(double alphaValue);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilOriginFunc(OriginDefinition mode);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilOverlayImage(uint source, int xCoord, int yCoord, int zCoord);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilPopAttrib();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilPushAttrib(uint bits);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilRegisterFormat(DataFormat format);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilRegisterLoad(
                [MarshalAs(UnmanagedType.LPWStr)] string ext,
                [MarshalAs(UnmanagedType.FunctionPtr)] IL_LOADPROC Load);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilRegisterMipNum(uint num);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilRegisterNumFaces(uint num);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilRegisterNumImages(uint num);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilRegisterOrigin(OriginDefinition origin);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilRegisterPal(IntPtr pal, uint size, PaletteType type);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilRegisterSave(
                [MarshalAs(UnmanagedType.LPWStr)] string ext,
                [MarshalAs(UnmanagedType.FunctionPtr)] IL_SAVEPROC saveCb);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilRegisterType(DataType type);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilRemoveLoad([MarshalAs(UnmanagedType.LPWStr)] string ext);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilRemoveSave([MarshalAs(UnmanagedType.LPWStr)] string ext);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilResetMemory(); // Deprecated
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilResetRead();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilResetWrite();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSave(ImageType type, [MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilSaveF(ImageType type, IntPtr fileHandle);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSaveImage([MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern uint ilSaveL(uint type, IntPtr lump, uint size);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSavePal([MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSetAlpha(double alphaValue);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSetData(IntPtr data);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSetDuration(uint duration);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilSetInteger(uint mode, int param);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilSetMemory(
                [MarshalAs(UnmanagedType.FunctionPtr)] mAlloc mAllocCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] mFree mFreeCb);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilSetPixels(
                int xOff, int yOff, int zOff,
                uint width, uint height, uint depth,
                 DataFormat format, DataType type, IntPtr data);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilSetRead(
                [MarshalAs(UnmanagedType.FunctionPtr)] fOpenRProc fOpenRProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fCloseRProc fCloseRProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fEofProc fEofProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fGetcProc fGetcProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fReadProc fReadProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fSeekRProc fSeekRProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fTellRProc fTellRProcCb);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilSetString(FileFormatSpecificValue mode, [MarshalAs(UnmanagedType.LPStr)] string str);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilSetWrite(
                [MarshalAs(UnmanagedType.FunctionPtr)] fOpenWProc fOpenWProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fCloseWProc fCloseWProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fPutcProc fPutcProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fSeekWProc fSeekWProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fTellWProc fTellWProcCb,
                [MarshalAs(UnmanagedType.FunctionPtr)] fWriteProc fWriteProcCb);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern void ilShutDown();
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSurfaceToDxtcData(DxtcDefinition format);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilTexImage(
                uint width, uint height, uint depth,
                byte numChannels, DataFormat dataFormat, DataType dataType,
                IntPtr dataPtr);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilTexImageDxtc(int w, int h, int d, DxtcDefinition dxtcFormat, byte* data);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            public unsafe static extern ImageType ilTypeFromExt([MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilTypeFunc(DataFormat mode);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoadData(
                [MarshalAs(UnmanagedType.LPWStr)] string fileName,
                uint width, uint height, uint depth, byte bpp);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoadDataF(IntPtr fileHandle,
                uint width, uint height, uint depth, byte bpp);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilLoadDataL(IntPtr lump,
                uint size, uint width, uint height, uint depth, byte bpp);
            [DllImport("DevIL.x64.dll", CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilSaveData([MarshalAs(UnmanagedType.LPWStr)] string fileName);
        }

        #endregion
    }
}

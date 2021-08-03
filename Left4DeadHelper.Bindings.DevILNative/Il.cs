using System;
using System.Collections.Generic;
using System.Text;

namespace Left4DeadHelper.Bindings.DevILNative
{
    internal static class Il
    {
        #region Wow64
        public static bool IsWow64()
        {
            return Environment.Is64BitProcess;
        }
        #endregion

        #region Constants
        
        public const int Version_1_8_0 = 1;
        public const int Version = 180;

        #endregion

        #region Enumerations

        public enum DataFormats : ushort
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

        public enum DataTypes : ushort
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

        // TODO new name pls
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
            BMP = 0x0420,  //!< Microsoft Windows Bitmap - .bmp extension
            CUT = 0x0421,  //!< Dr. Halo - .cut extension
            DOOM = 0x0422,  //!< DooM walls - no specific extension
            DOOM_FLAT = 0x0423,  //!< DooM flats - no specific extension
            ICO = 0x0424,  //!< Microsoft Windows Icons and Cursors - .ico and .cur extensions
            JPG = 0x0425,  //!< JPEG - .jpg, .jpe and .jpeg extensions
            JFIF = 0x0425,  //!<
            ILBM = 0x0426,  //!< Amiga IFF (FORM ILBM) - .iff, .ilbm, .lbm extensions
            PCD = 0x0427,  //!< Kodak PhotoCD - .pcd extension
            PCX = 0x0428,  //!< ZSoft PCX - .pcx extension
            PIC = 0x0429,  //!< PIC - .pic extension
            PNG = 0x042A,  //!< Portable Network Graphics - .png extension
            PNM = 0x042B,  //!< Portable Any Map - .pbm, .pgm, .ppm and .pnm extensions
            SGI = 0x042C,  //!< Silicon Graphics - .sgi, .bw, .rgb and .rgba extensions
            TGA = 0x042D,  //!< TrueVision Targa File - .tga, .vda, .icb and .vst extensions
            TIF = 0x042E,  //!< Tagged Image File Format - .tif and .tiff extensions
            CHEAD = 0x042F,  //!< C-Style Header - .h extension
            RAW = 0x0430,  //!< Raw Image Data - any extension
            MDL = 0x0431,  //!< Half-Life Model Texture - .mdl extension
            WAL = 0x0432,  //!< Quake 2 Texture - .wal extension
            LIF = 0x0434,  //!< Homeworld Texture - .lif extension
            MNG = 0x0435,  //!< Multiple-image Network Graphics - .mng extension
            JNG = 0x0435,  //!< 
            GIF = 0x0436,  //!< Graphics Interchange Format - .gif extension
            DDS = 0x0437,  //!< DirectDraw Surface - .dds extension
            DCX = 0x0438,  //!< ZSoft Multi-PCX - .dcx extension
            PSD = 0x0439,  //!< Adobe PhotoShop - .psd extension
            EXIF = 0x043A,  //!< 
            PSP = 0x043B,  //!< PaintShop Pro - .psp extension
            PIX = 0x043C,  //!< PIX - .pix extension
            PXR = 0x043D,  //!< Pixar - .pxr extension
            XPM = 0x043E,  //!< X Pixel Map - .xpm extension
            HDR = 0x043F,  //!< Radiance High Dynamic Range - .hdr extension
            ICNS = 0x0440,  //!< Macintosh Icon - .icns extension
            JP2 = 0x0441,  //!< Jpeg 2000 - .jp2 extension
            EXR = 0x0442,  //!< OpenEXR - .exr extension
            WDP = 0x0443,  //!< Microsoft HD Photo - .wdp and .hdp extension
            VTF = 0x0444,  //!< Valve Texture Format - .vtf extension
            WBMP = 0x0445,  //!< Wireless Bitmap - .wbmp extension
            SUN = 0x0446,  //!< Sun Raster - .sun, .ras, .rs, .im1, .im8, .im24 and .im32 extensions
            IFF = 0x0447,  //!< Interchange File Format - .iff extension
            TPL = 0x0448,  //!< Gamecube Texture - .tpl extension
            FITS = 0x0449,  //!< Flexible Image Transport System - .fit and .fits extensions
            DICOM = 0x044A,  //!< Digital Imaging and Communications in Medicine (DICOM) - .dcm and .dicom extensions
            IWI = 0x044B,  //!< Call of Duty Infinity Ward Image - .iwi extension
            BLP = 0x044C,  //!< Blizzard Texture Format - .blp extension
            FTX = 0x044D,  //!< Heavy Metal: FAKK2 Texture - .ftx extension
            ROT = 0x044E,  //!< Homeworld 2 - Relic Texture - .rot extension
            TEXTURE = 0x044F,  //!< Medieval II: Total War Texture - .texture extension
            DPX = 0x0450,  //!< Digital Picture Exchange - .dpx extension
            UTX = 0x0451,  //!< Unreal (and Unreal Tournament) Texture - .utx extension
            MP3 = 0x0452,  //!< MPEG-1 Audio Layer 3 - .mp3 extension
            KTX = 0x0453,  //!< Khronos Texture - .ktx extension
            
            JASC_PAL = 0x0475  //!< PaintShop Pro Palette
        }

        public enum ErrorTypes : ushort
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

        public enum OriginDefinitions : ushort
        {
            Set = 0x0600,
            LowerLeft = 0x0601,
            UpperLeft = 0x0602,
            Mode = 0x0603
        }

        public enum FormatAndTypeModeDefinitions : ushort
        {
            FormatSet = 0x0610,
            FormatMode = 0x0611,
            TypeSet = 0x0612,
            TypeMode = 0x0613,
        }

        public enum FileDefinitions : ushort
        {
            Overwrite = 0x0620,
            Mode = 0x0621,
        }

        public enum PaletteDefinitions : ushort
        {
            ConvPal = 0x0630
        }

        public enum LoadFailDefinitions : ushort
        {
            DefaultOnFail = 0x0632
        }


        public enum KeyColourAndAlphaDefinitions : ushort
        {
            UseKeyColor = 0x0635,
            BlitBlend = 0x0636
        }
        public enum InterlaceDefinitions : ushort
        {
            SaveInterlaced = 0x0639,
            InterlaceMode = 0x063A,
        }

        public enum QuantizationDefinitions : ushort
        {
            QuantizationMode = 0x0640,
            WuQuant = 0x0641,
            NeuQuant = 0x0642,
            NeuQuantSample = 0x0643,
            MaxQuantIndices = 0x0644, // Redefined, since the above #define is misspelled
        }

        public enum Hints : ushort
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
            Nvidia_Compress =0x0670,
            Squish_Compress =0x0671,
        }

        // Subimage types
#define IL_SUB_NEXT   0x0680
#define IL_SUB_MIPMAP 0x0681
#define IL_SUB_LAYER  0x0682


        // Compression definitions
#define IL_COMPRESS_MODE 0x0700
#define IL_COMPRESS_NONE 0x0701
#define IL_COMPRESS_RLE  0x0702
#define IL_COMPRESS_LZO  0x0703
#define IL_COMPRESS_ZLIB 0x0704


        // File format-specific values
#define IL_TGA_CREATE_STAMP        0x0710
#define IL_JPG_QUALITY             0x0711
#define IL_PNG_INTERLACE           0x0712
#define IL_TGA_RLE                 0x0713
#define IL_BMP_RLE                 0x0714
#define IL_SGI_RLE                 0x0715
#define IL_TGA_ID_STRING           0x0717
#define IL_TGA_AUTHNAME_STRING     0x0718
#define IL_TGA_AUTHCOMMENT_STRING  0x0719
#define IL_PNG_AUTHNAME_STRING     0x071A
#define IL_PNG_TITLE_STRING        0x071B
#define IL_PNG_DESCRIPTION_STRING  0x071C
#define IL_TIF_DESCRIPTION_STRING  0x071D
#define IL_TIF_HOSTCOMPUTER_STRING 0x071E
#define IL_TIF_DOCUMENTNAME_STRING 0x071F
#define IL_TIF_AUTHNAME_STRING     0x0720
#define IL_JPG_SAVE_FORMAT         0x0721
#define IL_CHEAD_HEADER_STRING     0x0722
#define IL_PCD_PICNUM              0x0723
#define IL_PNG_ALPHA_INDEX 0x0724 // currently has no effect!
#define IL_JPG_PROGRESSIVE         0x0725
#define IL_VTF_COMP                0x0726


        // DXTC definitions
#define IL_DXTC_FORMAT      0x0705
#define IL_DXT1             0x0706
#define IL_DXT2             0x0707
#define IL_DXT3             0x0708
#define IL_DXT4             0x0709
#define IL_DXT5             0x070A
#define IL_DXT_NO_COMP      0x070B
#define IL_KEEP_DXTC_DATA   0x070C
#define IL_DXTC_DATA_FORMAT 0x070D
#define IL_3DC              0x070E
#define IL_RXGB             0x070F
#define IL_ATI1N            0x0710
#define IL_DXT1A            0x0711  // Normally the same as IL_DXT1, except for nVidia Texture Tools.

        // Environment map definitions
#define IL_CUBEMAP_POSITIVEX 0x00000400
#define IL_CUBEMAP_NEGATIVEX 0x00000800
#define IL_CUBEMAP_POSITIVEY 0x00001000
#define IL_CUBEMAP_NEGATIVEY 0x00002000
#define IL_CUBEMAP_POSITIVEZ 0x00004000
#define IL_CUBEMAP_NEGATIVEZ 0x00008000
#define IL_SPHEREMAP         0x00010000


        // Values
#define IL_VERSION_NUM           0x0DE2
#define IL_IMAGE_WIDTH           0x0DE4
#define IL_IMAGE_HEIGHT          0x0DE5
#define IL_IMAGE_DEPTH           0x0DE6
#define IL_IMAGE_SIZE_OF_DATA    0x0DE7
#define IL_IMAGE_BPP             0x0DE8
#define IL_IMAGE_BYTES_PER_PIXEL 0x0DE8
#define IL_IMAGE_BPP             0x0DE8
#define IL_IMAGE_BITS_PER_PIXEL  0x0DE9
#define IL_IMAGE_FORMAT          0x0DEA
#define IL_IMAGE_TYPE            0x0DEB
#define IL_PALETTE_TYPE          0x0DEC
#define IL_PALETTE_SIZE          0x0DED
#define IL_PALETTE_BPP           0x0DEE
#define IL_PALETTE_NUM_COLS      0x0DEF
#define IL_PALETTE_BASE_TYPE     0x0DF0
#define IL_NUM_FACES             0x0DE1
#define IL_NUM_IMAGES            0x0DF1
#define IL_NUM_MIPMAPS           0x0DF2
#define IL_NUM_LAYERS            0x0DF3
#define IL_ACTIVE_IMAGE          0x0DF4
#define IL_ACTIVE_MIPMAP         0x0DF5
#define IL_ACTIVE_LAYER          0x0DF6
#define IL_ACTIVE_FACE           0x0E00
#define IL_CUR_IMAGE             0x0DF7
#define IL_IMAGE_DURATION        0x0DF8
#define IL_IMAGE_PLANESIZE       0x0DF9
#define IL_IMAGE_BPC             0x0DFA
#define IL_IMAGE_OFFX            0x0DFB
#define IL_IMAGE_OFFY            0x0DFC
#define IL_IMAGE_CUBEFLAGS       0x0DFD
#define IL_IMAGE_ORIGIN          0x0DFE
#define IL_IMAGE_CHANNELS        0x0DFF

        #endregion

        #region Helper methods

        public static uint Limit(uint x, uint min, uint max)
        {
            return x < min ? min : x > max ? max : x;
        }

        public static uint Clamp(uint x) => Limit(x, 0, 1);

        #endregion

        #region Structures

        [StructLayout(LayoutKind.Sequential, Pack = 1)]


        #endregion

    }
}

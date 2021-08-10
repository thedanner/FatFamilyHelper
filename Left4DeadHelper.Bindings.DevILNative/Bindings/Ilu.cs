using System;
using System.Runtime.InteropServices;

namespace Left4DeadHelper.Bindings.DevILNative.Bindings
{
    internal static class Ilu
    {
        #region Constants

        public const ushort Version_1_8_0 = 1;
        public const ushort Version = 180;

        #endregion

        #region Enumerations

        public enum Filter : ushort
        {
            Filter = 0x2600,
            Nearest = 0x2601,
            Linear = 0x2602,
            Bilinear = 0x2603,
            ScaleBox = 0x2604,
            ScaleTriangle = 0x2605,
            ScaleBell = 0x2606,
            ScaleBspline = 0x2607,
            ScaleLanczos3 = 0x2608,
        }

        public enum ErrorType : ushort
        {
            InvalidEnum = 0x0501,
            OutOfMemory = 0x0502,
            InternalError = 0x0504,
            InvalidValue = 0x0505,
            IllegalOperation = 0x0506,
            InvalidParam = 0x0509,
        }

        public enum Placement : ushort
        {
            Placement = 0x0700,
            LowerLeft = 0x0701,
            LowerRight = 0x0702,
            UpperLeft = 0x0703,
            UpperRight = 0x0704,
            Center = 0x0705,
            ConvolutionalMatrix = 0x0710,
        }

        public const ushort VersionNum = Il.Version;

        public enum Language : ushort
        {
            English = 0x0800,
            Arabic = 0x0801,
            Dutch = 0x0802,
            Japanese = 0x0803,
            Spanish = 0x0804,
            German = 0x0805,
            French = 0x0806,
            Italian = 0x0807,
        }

        public enum FilterNameEnum : ushort
        {
            Name = Filter.Filter
        }

        public enum PlacementNameEnum : ushort
        {
            Name = Placement.Placement
        }

        public enum StringName : ushort
        {
            Vendor = Il.Ext.Vendor,
            VersionNum = Il.Value.VersionNum
        }

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential, Size = 80)]
        public struct ILinfo
        {
            public uint Id;
            public IntPtr Data;
            public uint Width;
            public uint Height;
            public uint Depth;
            public byte Bpp;
            public uint SizeOfData;
            public uint Format;
            public uint Type;
            public uint Origin;
            public IntPtr Palette;
            public uint PalType;
            public uint PalSize;
            public uint CubeFlags;
            public uint NumNext;
            public uint NumMips;
            public uint NumLayers;
        }

        [StructLayout(LayoutKind.Sequential, Size = 8)]
        public struct ILpointf
        {
            public float X;
            public float Y;
        }

        [StructLayout(LayoutKind.Sequential, Size = 8)]
        public struct ILpointi
        {
            public int x;
            public int y;
        }

        #endregion

        #region Functions

        public unsafe static bool iluAlienify()
        {
            return Common.Wow64() ? x64.iluAlienify() : x86.iluAlienify();
        }

        public unsafe static bool iluBlurAvg(uint iter)
        {
            return Common.Wow64() ? x64.iluBlurAvg(iter) : x86.iluBlurAvg(iter);
        }

        public unsafe static bool iluBlurGaussian(uint iter)
        {
            return Common.Wow64() ? x64.iluBlurGaussian(iter) : x86.iluBlurGaussian(iter);
        }

        public unsafe static bool iluBuildMipmaps()
        {
            return Common.Wow64() ? x64.iluBuildMipmaps() : x86.iluBuildMipmaps();
        }

        public unsafe static uint iluColoursUsed()
        {
            return Common.Wow64() ? x64.iluColoursUsed() : x86.iluColoursUsed();
        }

        public unsafe static bool iluCompareImage(uint comp)
        {
            return Common.Wow64() ? x64.iluCompareImage(comp) : x86.iluCompareImage(comp);
        }

        public unsafe static bool iluContrast(float contrast)
        {
            return Common.Wow64() ? x64.iluContrast(contrast) : x86.iluContrast(contrast);
        }

        public unsafe static bool iluCrop(
            uint xOff, uint yOff, uint zOff,
            uint width, uint height, uint depth)
        {
            return Common.Wow64()
                ? x64.iluCrop(
                    xOff, yOff, zOff,
                    width, height, depth)
                : x86.iluCrop(
                    xOff, yOff, zOff,
                    width, height, depth);
        }

        [Obsolete("Deprecated per comment in header.")]
        public unsafe static void iluDeleteImage(uint id) // Deprecated
        {
            if (Common.Wow64()) x64.iluDeleteImage(id); else x86.iluDeleteImage(id);
        }

        public unsafe static bool iluEdgeDetectE()
        {
            return Common.Wow64() ? x64.iluEdgeDetectE() : x86.iluEdgeDetectE();
        }

        public unsafe static bool iluEdgeDetectP()
        {
            return Common.Wow64() ? x64.iluEdgeDetectP() : x86.iluEdgeDetectP();
        }

        public unsafe static bool iluEdgeDetectS()
        {
            return Common.Wow64() ? x64.iluEdgeDetectS() : x86.iluEdgeDetectS();
        }

        public unsafe static bool iluEmboss()
        {
            return Common.Wow64() ? x64.iluEmboss() : x86.iluEmboss();
        }

        public unsafe static bool iluEnlargeCanvas(uint width, uint height, uint depth)
        {
            return Common.Wow64()
                ? x64.iluEnlargeCanvas(width, height, depth)
                : x86.iluEnlargeCanvas(width, height, depth);
        }

        public unsafe static bool iluEnlargeImage(float xDim, float yDim, float zDim)
        {
            return Common.Wow64()
                ? x64.iluEnlargeImage(xDim, yDim, zDim)
                : x86.iluEnlargeImage(xDim, yDim, zDim);
        }

        public unsafe static bool iluEqualize()
        {
            return Common.Wow64() ? x64.iluEqualize() : x86.iluEqualize();
        }

        public unsafe static bool iluEqualize2()
        {
            return Common.Wow64() ? x64.iluEqualize2() : x86.iluEqualize2();
        }

        public unsafe static string iluErrorString(Il.ErrorType error)
        {
            return Common.Wow64() ? x64.iluErrorString(error) : x86.iluErrorString(error);
        }

        public unsafe static bool iluConvolution(int* matrix, int scale, int bias)
        {
            return Common.Wow64()
                ? x64.iluConvolution(matrix, scale, bias)
                : x86.iluConvolution(matrix, scale, bias);
        }

        public unsafe static bool iluFlipImage()
        {
            return Common.Wow64() ? x64.iluFlipImage() : x86.iluFlipImage();
        }

        public unsafe static bool iluGammaCorrect(float gamma)
        {
            return Common.Wow64() ? x64.iluGammaCorrect(gamma) : x86.iluGammaCorrect(gamma);
        }

        [Obsolete("Deprecated per comment in header.")]
        public unsafe static uint iluGenImage() // Deprecated
        {
            return Common.Wow64() ? x64.iluGenImage() : x86.iluGenImage();
        }

        public unsafe static void iluGetImageInfo(ILinfo* info)
        {
            if (Common.Wow64()) x64.iluGetImageInfo(info); else x86.iluGetImageInfo(info);
        }

        public unsafe static int iluGetInteger(uint mode)
        {
            return Common.Wow64() ? x64.iluGetInteger(mode) : x86.iluGetInteger(mode);
        }

        public unsafe static void iluGetIntegerv(uint mode, out int param)
        {
            if (Common.Wow64()) x64.iluGetIntegerv(mode, out param); else x86.iluGetIntegerv(mode, out param);
        }

        public unsafe static string iluGetString(StringName stringName)
        {
            return Common.Wow64() ? x64.iluGetString(stringName) : x86.iluGetString(stringName);
        }

        public unsafe static void iluImageParameter(FilterNameEnum pName, Filter param)
        {
            if (Common.Wow64()) x64.iluImageParameter(pName, param); else x86.iluImageParameter(pName, param);
        }

        public unsafe static void iluImageParameter(PlacementNameEnum pName, Placement param)
        {
            if (Common.Wow64()) x64.iluImageParameter(pName, param); else x86.iluImageParameter(pName, param);
        }

        public unsafe static void iluInit()
        {
            if (Common.Wow64()) x64.iluInit(); else x86.iluInit();
        }

        public unsafe static bool iluInvertAlpha()
        {
            return Common.Wow64() ? x64.iluInvertAlpha() : x86.iluInvertAlpha();
        }

        public unsafe static uint iluLoadImage(string fileName)
        {
            return Common.Wow64() ? x64.iluLoadImage(fileName) : x86.iluLoadImage(fileName);
        }

        public unsafe static bool iluMirror()
        {
            return Common.Wow64() ? x64.iluMirror() : x86.iluMirror();
        }

        public unsafe static bool iluNegative()
        {
            return Common.Wow64() ? x64.iluNegative() : x86.iluNegative();
        }

        public unsafe static bool iluNoisify(float tolerance)
        {
            return Common.Wow64() ? x64.iluNoisify(tolerance) : x86.iluNoisify(tolerance);
        }

        public unsafe static bool iluPixelize(uint pixSize)
        {
            return Common.Wow64() ? x64.iluPixelize(pixSize) : x86.iluPixelize(pixSize);
        }

        public unsafe static void iluRegionfv(ILpointf* points, uint n)
        {
            if (Common.Wow64()) x64.iluRegionfv(points, n); else x86.iluRegionfv(points, n);
        }

        public unsafe static void iluRegioniv(ILpointi* points, uint n)
        {
            if (Common.Wow64()) x64.iluRegioniv(points, n); else x86.iluRegioniv(points, n);
        }

        public unsafe static bool iluReplaceColour(
            byte red, byte green, byte blue, float tolerance)
        {
            return Common.Wow64()
                ? x64.iluReplaceColour(red, green, blue, tolerance)
                : x86.iluReplaceColour(red, green, blue, tolerance);
        }

        public unsafe static bool iluRotate(float angle)
        {
            return Common.Wow64() ? x64.iluRotate(angle) : x86.iluRotate(angle);
        }

        public unsafe static bool iluRotate3D(float x, float y, float z, float angle)
        {
            return Common.Wow64() ? x64.iluRotate3D(x, y, z, angle) : x86.iluRotate3D(x, y, z, angle);
        }

        public unsafe static bool iluSaturate1f(float saturation)
        {
            return Common.Wow64() ? x64.iluSaturate1f(saturation) : x86.iluSaturate1f(saturation);
        }

        public unsafe static bool iluSaturate4f(float r, float g, float b, float saturation)
        {
            return Common.Wow64() ? x64.iluSaturate4f(r, g, b, saturation) : x86.iluSaturate4f(r, g, b, saturation);
        }

        public unsafe static bool iluScale(uint width, uint height, uint depth)
        {
            return Common.Wow64() ? x64.iluScale(width, height, depth) : x86.iluScale(width, height, depth);
        }

        public unsafe static bool iluScaleAlpha(float scale)
        {
            return Common.Wow64() ? x64.iluScaleAlpha(scale) : x86.iluScaleAlpha(scale);
        }

        public unsafe static bool iluScaleColours(float r, float g, float b)
        {
            return Common.Wow64() ? x64.iluScaleColours(r, g, b) : x86.iluScaleColours(r, g, b);
        }

        public unsafe static bool iluSepia()
        {
            return Common.Wow64() ? x64.iluSepia() : x86.iluSepia();
        }

        public unsafe static bool iluSetLanguage(Language language)
        {
            return Common.Wow64() ? x64.iluSetLanguage(language) : x86.iluSetLanguage(language);
        }

        public unsafe static bool iluSharpen(float factor, uint iter)
        {
            return Common.Wow64() ? x64.iluSharpen(factor, iter) : x86.iluSharpen(factor, iter);
        }

        public unsafe static bool iluSwapColours()
        {
            return Common.Wow64() ? x64.iluSwapColours() : x86.iluSwapColours();
        }

        public unsafe static bool iluWave(float angle)
        {
            return Common.Wow64() ? x64.iluWave(angle) : x86.iluWave(angle);
        }

        private static class x86
        {
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluAlienify();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluBlurAvg(uint iter);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluBlurGaussian(uint iter);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluBuildMipmaps();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern uint iluColoursUsed();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluCompareImage(uint comp);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluContrast(float contrast);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluCrop(
                uint xOff, uint yOff, uint zOff,
                uint width, uint height, uint depth);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [Obsolete("Deprecated per comment in header.")]
            public unsafe static extern void iluDeleteImage(uint id); // Deprecated
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEdgeDetectE();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEdgeDetectP();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEdgeDetectS();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEmboss();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEnlargeCanvas(uint width, uint height, uint depth);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEnlargeImage(float xDim, float yDim, float zDim);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEqualize();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEqualize2();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.LPWStr)]
            public unsafe static extern string iluErrorString(Il.ErrorType error);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluConvolution(int* matrix, int scale, int bias);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluFlipImage();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluGammaCorrect(float gamma);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [Obsolete("Deprecated per comment in header.")]
            public unsafe static extern uint iluGenImage(); // Deprecated
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void iluGetImageInfo(ILinfo* info);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern int iluGetInteger(uint mode);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void iluGetIntegerv(uint mode, out int param);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.LPWStr)]
            public unsafe static extern string iluGetString(StringName stringName);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void iluImageParameter(
                FilterNameEnum pName, Filter param);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void iluImageParameter(
                PlacementNameEnum pName, Placement param);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void iluInit();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluInvertAlpha();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern uint iluLoadImage(
                [MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluMirror();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluNegative();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluNoisify(float tolerance);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluPixelize(uint pixSize);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void iluRegionfv(ILpointf* points, uint n);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void iluRegioniv(ILpointi* points, uint n);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluReplaceColour(
                byte red, byte green, byte blue, float tolerance);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluRotate(float angle);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluRotate3D(float x, float y, float z, float angle);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluSaturate1f(float saturation);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluSaturate4f(float r, float g, float b, float saturation);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluScale(uint width, uint height, uint depth);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluScaleAlpha(float scale);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluScaleColours(float r, float g, float b);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluSepia();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluSetLanguage(Language language);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluSharpen(float factor, uint iter);
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluSwapColours();
            [DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluWave(float angle);
        }

        private static class x64
        {
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluAlienify();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluBlurAvg(uint iter);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluBlurGaussian(uint iter);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluBuildMipmaps();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern uint iluColoursUsed();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluCompareImage(uint comp);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluContrast(float contrast);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluCrop(
                uint xOff, uint yOff, uint zOff,
                uint width, uint height, uint depth);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [Obsolete("Deprecated per comment in header.")]
            public unsafe static extern void iluDeleteImage(uint id); // Deprecated
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEdgeDetectE();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEdgeDetectP();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEdgeDetectS();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEmboss();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEnlargeCanvas(uint width, uint height, uint depth);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEnlargeImage(float xDim, float yDim, float zDim);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEqualize();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluEqualize2();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.LPWStr)]
            public unsafe static extern string iluErrorString(Il.ErrorType error);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluConvolution(int* matrix, int scale, int bias);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluFlipImage();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluGammaCorrect(float gamma);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [Obsolete("Deprecated per comment in header.")]
            public unsafe static extern uint iluGenImage(); // Deprecated
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void iluGetImageInfo(ILinfo* info);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern int iluGetInteger(uint mode);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void iluGetIntegerv(uint mode, out int param);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.LPWStr)]
            public unsafe static extern string iluGetString(StringName stringName);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void iluImageParameter(
                FilterNameEnum pName, Filter param);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void iluImageParameter(
                PlacementNameEnum pName, Placement param);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void iluInit();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluInvertAlpha();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern uint iluLoadImage(
                [MarshalAs(UnmanagedType.LPWStr)] string fileName);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluMirror();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluNegative();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluNoisify(float tolerance);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluPixelize(uint pixSize);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void iluRegionfv(ILpointf* points, uint n);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void iluRegioniv(ILpointi* points, uint n);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluReplaceColour(
                byte red, byte green, byte blue, float tolerance);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluRotate(float angle);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluRotate3D(float x, float y, float z, float angle);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluSaturate1f(float saturation);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluSaturate4f(float r, float g, float b, float saturation);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluScale(uint width, uint height, uint depth);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluScaleAlpha(float scale);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluScaleColours(float r, float g, float b);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluSepia();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluSetLanguage(Language language);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluSharpen(float factor, uint iter);
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluSwapColours();
            [DllImport("ILU.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool iluWave(float angle);
        }

        #endregion
    }
}

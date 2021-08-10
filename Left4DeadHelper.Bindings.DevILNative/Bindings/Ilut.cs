using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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
            //[DllImport("ILU.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        }

        private static class x64
        {
            // ImageLib Utility Toolkit Functions

            [DllImport("ILUT.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilutDisable(ILenum Mode);

            [DllImport("ILUT.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilutEnable(ILenum Mode);
            [DllImport("ILUT.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilutGetBoolean(ILenum Mode);
            [DllImport("ILUT.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void ilutGetBooleanv(ILenum Mode, ILboolean* Param);
            [DllImport("ILUT.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern int ilutGetInteger(ILenum Mode);
            [DllImport("ILUT.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void ilutGetIntegerv(ILenum Mode, ILint* Param);
            [DllImport("ILUT.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.LPWStr)]
            public unsafe static extern string ilutGetString(ILenum StringName);
            [DllImport("ILUT.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void ilutInit();
            [DllImport("ILUT.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilutIsDisabled(ILenum Mode);
            [DllImport("ILUT.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilutIsEnabled(ILenum Mode);
            [DllImport("ILUT.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void ilutPopAttrib();
            [DllImport("ILUT.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void ilutPushAttrib(ILuint Bits);
            [DllImport("ILUT.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern void ilutSetInteger(ILenum Mode, ILint Param);

            [DllImport("ILUT.x64.dll", CallingConvention = CallingConvention.Cdecl)]
            [return: MarshalAs(UnmanagedType.U1)]
            public unsafe static extern bool ilutRenderer(ILenum Renderer);


            // ImageLib Utility Toolkit's OpenGL Functions
            public unsafe static extern GLuint    ilutGLBindTexImage();
            public unsafe static extern GLuint    ilutGLBindMipmaps();
            public unsafe static extern ILboolean ilutGLBuildMipmaps();
            public unsafe static extern GLuint    ilutGLLoadImage(ILstring FileName);
            public unsafe static extern ILboolean ilutGLScreen();
            public unsafe static extern ILboolean ilutGLScreenie();
            public unsafe static extern ILboolean ilutGLSaveImage(ILstring FileName, GLuint TexID);
            public unsafe static extern ILboolean ilutGLSubTex2D(uint TexID, ILuint XOff, ILuint YOff);
            public unsafe static extern ILboolean ilutGLSubTex3D(uint TexID, ILuint XOff, ILuint YOff, ILuint ZOff);
            public unsafe static extern ILboolean ilutGLSetTex2D(uint TexID);
            public unsafe static extern ILboolean ilutGLSetTex3D(uint TexID);
            public unsafe static extern ILboolean ilutGLTexImage(uint Level);
            public unsafe static extern ILboolean ilutGLSubTex(uint TexID, ILuint XOff, ILuint YOff);
            public unsafe static extern 
            public unsafe static extern ILboolean ilutGLSetTex(uint TexID);  // Deprecated - use ilutGLSetTex2D.
            public unsafe static extern ILboolean ilutGLSubTex(uint TexID, ILuint XOff, ILuint YOff);  // Use ilutGLSubTex2D.

	BITMAP* ilutAllegLoadImage(ILstring FileName);
            BITMAP* ilutConvertToAlleg(PALETTE Pal);
#endif//ILUT_USE_ALLEGRO


            // ImageLib Utility Toolkit's SDL Functions
# ifdef ILUT_USE_SDL
            struct SDL_Surface* ilutConvertToSDLSurface(unsigned int flags);
            struct SDL_Surface* ilutSDLSurfaceLoadImage(ILstring FileName);
            ILboolean    ilutSDLSurfaceFromBitmap(struct SDL_Surface *Bitmap);
#endif//ILUT_USE_SDL


// ImageLib Utility Toolkit's BeOS Functions
#ifdef  ILUT_USE_BEOS
	BBitmap ilutConvertToBBitmap(void);
#endif//ILUT_USE_BEOS


            // ImageLib Utility Toolkit's Win32 GDI Functions
# ifdef ILUT_USE_WIN32
            HBITMAP   ilutConvertToHBitmap(HDC hDC);
            HBITMAP   ilutConvertSliceToHBitmap(HDC hDC, ILuint slice);
            void ilutFreePaddedData(ILubyte* Data);
            void ilutGetBmpInfo(BITMAPINFO* Info);
            HPALETTE  ilutGetHPal(void);
            ILubyte*	ilutGetPaddedData(void);
            ILboolean ilutGetWinClipboard(void);
            ILboolean ilutLoadResource(HINSTANCE hInst, ILint ID, ILstring ResourceType, ILenum Type);
            ILboolean ilutSetHBitmap(HBITMAP Bitmap);
            ILboolean ilutSetHPal(HPALETTE Pal);
            ILboolean ilutSetWinClipboard(void);
            HBITMAP   ilutWinLoadImage(ILstring FileName, HDC hDC);
            ILboolean ilutWinLoadUrl(ILstring Url);
            ILboolean ilutWinPrint(ILuint XPos, ILuint YPos, ILuint Width, ILuint Height, HDC hDC);
            ILboolean ilutWinSaveImage(ILstring FileName, HBITMAP Bitmap);
#endif//ILUT_USE_WIN32

            // ImageLib Utility Toolkit's DirectX 8 Functions
# ifdef ILUT_USE_DIRECTX8
            //	void	ilutD3D8MipFunc(ILuint NumLevels);
            struct IDirect3DTexture8* ilutD3D8Texture(struct IDirect3DDevice8 *Device);
	struct IDirect3DVolumeTexture8* ilutD3D8VolumeTexture(struct IDirect3DDevice8 *Device);
	ILboolean ilutD3D8TexFromFile(struct IDirect3DDevice8 *Device, char* FileName, struct IDirect3DTexture8 **Texture);
	ILboolean ilutD3D8VolTexFromFile(struct IDirect3DDevice8 *Device, char* FileName, struct IDirect3DVolumeTexture8 **Texture);
	ILboolean ilutD3D8TexFromFileInMemory(struct IDirect3DDevice8 *Device, void* Lump, ILuint Size, struct IDirect3DTexture8 **Texture);
	ILboolean ilutD3D8VolTexFromFileInMemory(struct IDirect3DDevice8 *Device, void* Lump, ILuint Size, struct IDirect3DVolumeTexture8 **Texture);
	ILboolean ilutD3D8TexFromFileHandle(struct IDirect3DDevice8 *Device, ILHANDLE File, struct IDirect3DTexture8 **Texture);
	ILboolean ilutD3D8VolTexFromFileHandle(struct IDirect3DDevice8 *Device, ILHANDLE File, struct IDirect3DVolumeTexture8 **Texture);
	// These two are not tested yet.
	ILboolean ilutD3D8TexFromResource(struct IDirect3DDevice8 *Device, HMODULE SrcModule, char* SrcResource, struct IDirect3DTexture8 **Texture);
	ILboolean ilutD3D8VolTexFromResource(struct IDirect3DDevice8 *Device, HMODULE SrcModule, char* SrcResource, struct IDirect3DVolumeTexture8 **Texture);
	ILboolean ilutD3D8LoadSurface(struct IDirect3DDevice8 *Device, struct IDirect3DSurface8 *Surface);
#endif//ILUT_USE_DIRECTX8

#ifdef ILUT_USE_DIRECTX9
	#pragma warning(push)
	#pragma warning(disable : 4115)  // Disables 'named type definition in parentheses' warning
//	void  ilutD3D9MipFunc(ILuint NumLevels);
	struct IDirect3DTexture9*       ilutD3D9Texture(struct IDirect3DDevice9* Device);
	struct IDirect3DVolumeTexture9* ilutD3D9VolumeTexture(struct IDirect3DDevice9* Device);
    struct IDirect3DCubeTexture9*       ilutD3D9CubeTexture(struct IDirect3DDevice9* Device);

    ILboolean ilutD3D9CubeTexFromFile(struct IDirect3DDevice9 *Device, ILconst_string FileName, struct IDirect3DCubeTexture9 **Texture);
    ILboolean ilutD3D9CubeTexFromFileInMemory(struct IDirect3DDevice9 *Device, void* Lump, ILuint Size, struct IDirect3DCubeTexture9 **Texture);
    ILboolean ilutD3D9CubeTexFromFileHandle(struct IDirect3DDevice9 *Device, ILHANDLE File, struct IDirect3DCubeTexture9 **Texture);
    ILboolean ilutD3D9CubeTexFromResource(struct IDirect3DDevice9 *Device, HMODULE SrcModule, ILconst_string SrcResource, struct IDirect3DCubeTexture9 **Texture);

	ILboolean ilutD3D9TexFromFile(struct IDirect3DDevice9 *Device, ILconst_string FileName, struct IDirect3DTexture9 **Texture);
	ILboolean ilutD3D9VolTexFromFile(struct IDirect3DDevice9 *Device, ILconst_string FileName, struct IDirect3DVolumeTexture9 **Texture);
	ILboolean ilutD3D9TexFromFileInMemory(struct IDirect3DDevice9 *Device, void* Lump, ILuint Size, struct IDirect3DTexture9 **Texture);
	ILboolean ilutD3D9VolTexFromFileInMemory(struct IDirect3DDevice9 *Device, void* Lump, ILuint Size, struct IDirect3DVolumeTexture9 **Texture);
	ILboolean ilutD3D9TexFromFileHandle(struct IDirect3DDevice9 *Device, ILHANDLE File, struct IDirect3DTexture9 **Texture);
	ILboolean ilutD3D9VolTexFromFileHandle(struct IDirect3DDevice9 *Device, ILHANDLE File, struct IDirect3DVolumeTexture9 **Texture);

	// These three are not tested yet.
	ILboolean ilutD3D9TexFromResource(struct IDirect3DDevice9 *Device, HMODULE SrcModule, ILconst_string SrcResource, struct IDirect3DTexture9 **Texture);
	ILboolean ilutD3D9VolTexFromResource(struct IDirect3DDevice9 *Device, HMODULE SrcModule, ILconst_string SrcResource, struct IDirect3DVolumeTexture9 **Texture);
	ILboolean ilutD3D9LoadSurface(struct IDirect3DDevice9 *Device, struct IDirect3DSurface9 *Surface);
	#pragma warning(pop)
#endif//ILUT_USE_DIRECTX9

#ifdef ILUT_USE_DIRECTX10
	ID3D10Texture2D* ilutD3D10Texture(ID3D10Device* Device);
            ILboolean ilutD3D10TexFromFile(ID3D10Device* Device, ILconst_string FileName, ID3D10Texture2D** Texture);
            ILboolean ilutD3D10TexFromFileInMemory(ID3D10Device* Device, void* Lump, ILuint Size, ID3D10Texture2D** Texture);
            ILboolean ilutD3D10TexFromResource(ID3D10Device* Device, HMODULE SrcModule, ILconst_string SrcResource, ID3D10Texture2D** Texture);
            ILboolean ilutD3D10TexFromFileHandle(ID3D10Device* Device, ILHANDLE File, ID3D10Texture2D** Texture);
#endif//ILUT_USE_DIRECTX10



# ifdef ILUT_USE_X11
            XImage * ilutXCreateImage(Display* );
            Pixmap ilutXCreatePixmap(Display*, Drawable );
            XImage * ilutXLoadImage(Display*, char* );
            Pixmap ilutXLoadPixmap(Display*, Drawable, char* );
# ifdef ILUT_USE_XSHM
            XImage * ilutXShmCreateImage(Display*, XShmSegmentInfo* );
            void ilutXShmDestroyImage(Display*, XImage*, XShmSegmentInfo* );
            Pixmap ilutXShmCreatePixmap(Display*, Drawable, XShmSegmentInfo* );
            void ilutXShmFreePixmap(Display*, Pixmap, XShmSegmentInfo* );
            XImage * ilutXShmLoadImage(Display*, char*, XShmSegmentInfo* );
            Pixmap ilutXShmLoadPixmap(Display*, Drawable, char*, XShmSegmentInfo* );
#endif//ILUT_USE_XSHM
#endif//ILUT_USE_X11
        }

        #endregion
    }
}

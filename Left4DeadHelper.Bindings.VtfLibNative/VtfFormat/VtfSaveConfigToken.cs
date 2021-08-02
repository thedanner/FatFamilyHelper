using System;

namespace Left4DeadHelper.Bindings.VtfLibNative.VtfFormat
{
    internal enum VtfSaveConfigTemplate
    {
        VtfTemplateSpray,
        VtfTemplateSprayWithAlpha,
    }

	[Serializable]
    internal class VtfSaveConfigToken : ICloneable
    {
        public VtfLib.ImageFormat eImageFormat;
        public uint uiImageFlags;

        public bool bGenerateMipmaps;
        public VtfLib.MipmapFilter eMipmapFilter;
        public VtfLib.SharpenFilter eMipmapSharpenFilter;

        public bool bAnimateLayers;
        public bool bGenerateThumbnail;

        public bool bGenerateNormalmap;
        public VtfLib.KernelFilter eNormalmapKernelFilter;
        public VtfLib.HeightConversionMethod eNormalmapHeightConversionMethod;
        public VtfLib.NormalAlphaResult eNormalmapAlphaResult;
        public decimal fNormalmapScale;
        public bool bNormalmapWrap;

        public uint uiVersionMajor;
        public uint uiVersionMinor;

        public VtfSaveConfigToken()
		{
            eImageFormat = VtfLib.ImageFormat.ImageFormatABGR8888;
            uiImageFlags = 0;

            bGenerateMipmaps = true;
            eMipmapFilter = VtfLib.MipmapFilter.MipmapFilterBox;
            eMipmapSharpenFilter = VtfLib.SharpenFilter.SharpenFilterSharpenSoft;

            bAnimateLayers = false;
            bGenerateThumbnail = true;

            bGenerateNormalmap = false;
            eNormalmapKernelFilter = VtfLib.KernelFilter.KernelFilter3x3;
            eNormalmapHeightConversionMethod = VtfLib.HeightConversionMethod.HeightConversionMethodAverageRGB;
            eNormalmapAlphaResult = VtfLib.NormalAlphaResult.NormalAlphaResultNoChange;
            fNormalmapScale = (decimal)2.0;
            bNormalmapWrap = false;

            uiVersionMajor = 7;
            uiVersionMinor = 2;
		}

        public VtfSaveConfigToken(VtfSaveConfigTemplate Template)
        {
            eImageFormat = VtfLib.ImageFormat.ImageFormatABGR8888;
            uiImageFlags = 0;

            bGenerateMipmaps = true;
            eMipmapFilter = VtfLib.MipmapFilter.MipmapFilterBox;
            eMipmapSharpenFilter = VtfLib.SharpenFilter.SharpenFilterSharpenSoft;

            bAnimateLayers = false;
            bGenerateThumbnail = true;

            bGenerateNormalmap = false;
            eNormalmapKernelFilter = VtfLib.KernelFilter.KernelFilter3x3;
            eNormalmapHeightConversionMethod = VtfLib.HeightConversionMethod.HeightConversionMethodAverageRGB;
            eNormalmapAlphaResult = VtfLib.NormalAlphaResult.NormalAlphaResultNoChange;
            fNormalmapScale = (decimal)2.0;
            bNormalmapWrap = false;

            switch (Template)
            {
                case VtfSaveConfigTemplate.VtfTemplateSpray:
                    eImageFormat = VtfLib.ImageFormat.ImageFormatDXT1;
                    uiImageFlags = (uint)VtfLib.ImageFlag.ImageFlagClampS | (uint)VtfLib.ImageFlag.ImageFlagClampT | (uint)VtfLib.ImageFlag.ImageFlagNoMIP | (uint)VtfLib.ImageFlag.ImageFlagNoLOD;

                    bGenerateMipmaps = false;
                    break;
                case VtfSaveConfigTemplate.VtfTemplateSprayWithAlpha:
                    eImageFormat = VtfLib.ImageFormat.ImageFormatDXT5;
                    uiImageFlags = (uint)VtfLib.ImageFlag.ImageFlagClampS | (uint)VtfLib.ImageFlag.ImageFlagClampT | (uint)VtfLib.ImageFlag.ImageFlagNoMIP | (uint)VtfLib.ImageFlag.ImageFlagNoLOD;

                    bGenerateMipmaps = false;
                    break;
            }
        }

        protected VtfSaveConfigToken(VtfSaveConfigToken Token)
		{
            eImageFormat = Token.eImageFormat;
            uiImageFlags = Token.uiImageFlags;

            bGenerateMipmaps = Token.bGenerateMipmaps;
            eMipmapFilter = Token.eMipmapFilter;
            eMipmapSharpenFilter = Token.eMipmapSharpenFilter;

            bAnimateLayers = Token.bAnimateLayers;
            bGenerateThumbnail = Token.bGenerateThumbnail;

            bGenerateNormalmap = Token.bGenerateNormalmap;
            eNormalmapKernelFilter = Token.eNormalmapKernelFilter;
            eNormalmapHeightConversionMethod = Token.eNormalmapHeightConversionMethod;
            eNormalmapAlphaResult = Token.eNormalmapAlphaResult;
            fNormalmapScale = Token.fNormalmapScale;
            bNormalmapWrap = Token.bNormalmapWrap;

            uiVersionMajor = Token.uiVersionMajor;
            uiVersionMinor = Token.uiVersionMinor;
		}

        public object Clone()
        {
            return new VtfSaveConfigToken(this);
        }

        public unsafe VtfLib.CreateOptions GetCreateOptions()
		{
            VtfLib.CreateOptions CreateOptions;
            VtfLib.vlImageCreateDefaultCreateStructure(out CreateOptions);

            CreateOptions.uiVersionMajor = uiVersionMajor;
            CreateOptions.uiVersionMinor = uiVersionMinor;

			CreateOptions.eImageFormat = eImageFormat;
            CreateOptions.uiFlags = uiImageFlags;

            CreateOptions.bMipmaps = bGenerateMipmaps;
            CreateOptions.eMipmapFilter = eMipmapFilter;
            CreateOptions.eSharpenFilter = eMipmapSharpenFilter;

            CreateOptions.bThumbnail = bGenerateThumbnail;

            CreateOptions.bResize = true;

            CreateOptions.bNormalMap = bGenerateNormalmap;
            CreateOptions.eKernelFilter = eNormalmapKernelFilter;
            CreateOptions.eHeightConversionMethod = eNormalmapHeightConversionMethod;
            CreateOptions.eNormalAlphaResult = eNormalmapAlphaResult;
            CreateOptions.fBumpScale = (float)fNormalmapScale;
            CreateOptions.bNormalWrap = bNormalmapWrap;

			return CreateOptions;
		}

		public void Validate()
		{
			if (eImageFormat == VtfLib.ImageFormat.ImageFormatNone || eImageFormat >= VtfLib.ImageFormat.ImageFormatCount)
			{
                throw new ArgumentOutOfRangeException("Unrecognised image format (" + eImageFormat + ").");
			}

            if (eMipmapFilter >= VtfLib.MipmapFilter.MipmapFilterCount)
            {
                throw new ArgumentOutOfRangeException("Unrecognised mipmap filter (" + eMipmapFilter + ").");
            }

            if (eMipmapSharpenFilter >= VtfLib.SharpenFilter.SharpenFilterCount)
            {
                throw new ArgumentOutOfRangeException("Unrecognised mipmap sharpen filter (" + eMipmapSharpenFilter + ").");
            }

            if (eNormalmapKernelFilter >= VtfLib.KernelFilter.KernelFilterCount)
            {
                throw new ArgumentOutOfRangeException("Unrecognised normal kernel filter (" + eNormalmapKernelFilter + ").");
            }

            if (eNormalmapHeightConversionMethod >= VtfLib.HeightConversionMethod.HeightConversionMethodCount)
            {
                throw new ArgumentOutOfRangeException("Unrecognised normal height conversion method (" + eNormalmapHeightConversionMethod + ").");
            }

            if (eNormalmapAlphaResult >= VtfLib.NormalAlphaResult.NormalAlphaResultCount)
            {
                throw new ArgumentOutOfRangeException("Unrecognised normal alpha result (" + eNormalmapAlphaResult + ").");
            }

            if (fNormalmapScale <= 0)
            {
                throw new ArgumentOutOfRangeException("Invalid normal scale (" + fNormalmapScale + ").");
            }

            if (uiVersionMajor != VtfLib.uiMajorVersion || uiVersionMinor > VtfLib.uiMinorVersion)
            {
                throw new ArgumentOutOfRangeException("Invalid VTF version (" + uiVersionMajor + "." + uiVersionMinor + ").");
            }
		}
	}
}

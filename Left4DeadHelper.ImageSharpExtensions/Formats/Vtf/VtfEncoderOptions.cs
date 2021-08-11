namespace Left4DeadHelper.ImageSharpExtensions.Formats.Vtf
{
    internal class VtfEncoderOptions : IVtfEncoderOptions
    {
        public VtfEncoderOptions(IVtfEncoderOptions source)
        {
            DxtFormat = source.DxtFormat;
        }

        public DxtFormat? DxtFormat { get; set; }
    }
}

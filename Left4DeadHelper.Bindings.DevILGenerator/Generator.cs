using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Passes;
using System.IO;

namespace Left4DeadHelper.Bindings.DevILGenerator
{
    public class Generator : ILibrary
    {
        public override void Setup(Driver driver)
        {
            var options = driver.Options;
            options.GeneratorKind = GeneratorKind.CSharp;
            var module = options.AddModule("DevIL");

            var includeDir = @"sdks\1.8.0\include";
            module.IncludeDirs.Add(Path.GetFullPath(includeDir));
            //foreach (var headerFile in Directory.GetFiles(Path.Combine(includeDir, "IL"), "*.h"))
            //{
            //    module.Headers.Add(Path.Combine("IL", Path.GetFileName(headerFile)));
            //}
            //foreach (var headerFile in Directory.GetFiles(Path.Combine(includeDir, "IL"), "*.hpp"))
            //{
            //    module.Headers.Add(Path.Combine("IL", Path.GetFileName(headerFile)));
            //}
            module.Headers.Add(@"IL\devil_internal_exports.h");
            module.Headers.Add(@"IL\il.h");
            module.Headers.Add(@"IL\ilu.h");
            //module.Headers.Add(@"IL\ilu_region.h");
            module.Headers.Add(@"IL\ilut_config.h");
            module.Headers.Add(@"IL\ilut.h");
            //module.Headers.Add(@"IL\il_wrap.h");
            //module.Headers.Add(@"IL\devil_cpp_wrapper.hpp");

            var libDir = @"sdks\1.8.0\lib\x64\unicode\Release";
            module.LibraryDirs.Add(Path.GetFullPath(libDir));
            //foreach (var lib in Directory.GetFiles(libDir, "*.lib"))
            //{
            //    module.Libraries.Add(Path.GetFileName(lib));
            //}
            module.Libraries.Add(@"DevIL.lib");
            module.Libraries.Add(@"ILU.lib");
            module.Libraries.Add(@"ILUT.lib");

            //libDir = @"C:\lib\DevIL Windows SDK\1.8.0\lib\x86\unicode\Release";
            //module.LibraryDirs.Add(libDir);
            //foreach (var lib in Directory.GetFiles(libDir, "*.lib"))
            //{
            //    module.Libraries.Add(Path.GetFileName(lib));
            //}
        }

        public override void SetupPasses(Driver driver)
        {
            driver.Context.TranslationUnitPasses.RenameDeclsUpperCase(RenameTargets.Any);
            driver.Context.TranslationUnitPasses.AddPass(new FunctionToInstanceMethodPass());
        }

        public override void Postprocess(Driver driver, ASTContext ctx)
        {
            
        }

        public override void Preprocess(Driver driver, ASTContext ctx)
        {
            
        }
    }
}

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.Helpers
{
    public class EvalCodeHelper
    {

        /// <summary>
        /// 执行C#代码，没有参数也没有返回值
        /// </summary>
        /// <param name="strCode">代码字符串</param>
        public static void Eval(string strCode)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("C#", new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
            CompilerParameters options = new CompilerParameters();
            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add("System.Core.dll");
            options.ReferencedAssemblies.Add("WPF\\WindowsBase.dll");
            options.ReferencedAssemblies.Add("WPF\\PresentationFramework.dll");
            options.ReferencedAssemblies.Add("WPF\\PresentationCore.dll");
            options.IncludeDebugInformation = false;
            options.GenerateExecutable = false;
            options.GenerateInMemory = true;

            options.CompilerOptions = "/t:library";
            options.GenerateInMemory = true;

            StringBuilder sources = new("");
            sources.AppendLine("using System;");
            sources.AppendLine("using System.Collections.Generic;");
            sources.AppendLine("using System.Linq;");
            sources.AppendLine("using System.Text;");
            sources.AppendLine("using System.Threading.Tasks;");
            sources.AppendLine("using System.Windows;");


            sources.AppendLine("namespace CodeEvaler{");
            sources.AppendLine("public class CodeEvaler{");
            sources.AppendLine("public void EvalCode(){" + strCode + "}");
            sources.AppendLine("}");
            sources.Append("}");

            CompilerResults results = provider.CompileAssemblyFromSource(options, sources.ToString());
            if (results.Errors.Count > 0)
            {
                MessageBox.Show("ERROR: " + results.Errors[0].ErrorText, "Error evaluating cs code");
            }

            Assembly assembly = results.CompiledAssembly;
            object codeEvaler = assembly.CreateInstance("CodeEvaler.CodeEvaler");
            codeEvaler.GetType().GetMethod("EvalCode").Invoke(codeEvaler, null);
        }

        /// <summary>
        /// 执行C#代码，传入调用者的引用，没有参数也没有返回值
        /// </summary>
        /// <param name="strCode">代码字符串</param>
        /// <param name="reference">对调用者的引用</param>
        public static void Eval(string strCode, object reference)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("C#", new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
            CompilerParameters options = new();
            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add("System.Core.dll");
            options.ReferencedAssemblies.Add("System.Xaml.dll");
            options.ReferencedAssemblies.Add("WPF\\WindowsBase.dll");
            options.ReferencedAssemblies.Add("WPF\\PresentationFramework.dll");
            options.ReferencedAssemblies.Add("WPF\\PresentationCore.dll");

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // TODO: 考虑是否允许用户添加程序集路径
            // 添加组件所需要类库
            options.ReferencedAssemblies.Add(Path.Combine(baseDirectory, "Hjmos.Lcdp.VisualEditor.Core.dll"));
            options.ReferencedAssemblies.Add(Path.Combine(baseDirectory, "Hjmos.Lcdp.VisualEditor.Core.dll"));
            options.ReferencedAssemblies.Add(Path.Combine(baseDirectory, "Prism.dll"));

            // 调用者程序集的路径
            options.ReferencedAssemblies.Add(reference.GetType().Assembly.Location);
            // 不包含调试信息
            options.IncludeDebugInformation = false;
            // 不生成可执行文件
            options.GenerateExecutable = false;
            // 程序集生成到内存中
            options.GenerateInMemory = true;

            options.CompilerOptions = "/t:library";
            options.GenerateInMemory = true;

            // TODO：考虑是否允许用户添加using
            StringBuilder sources = new("");
            sources.AppendLine("using System;");
            sources.AppendLine("using System.Collections.Generic;");
            sources.AppendLine("using System.Linq;");
            sources.AppendLine("using System.Text;");
            sources.AppendLine("using System.Threading.Tasks;");
            sources.AppendLine("using System.Windows;");
            sources.AppendLine("using System.Windows.Media;");
            sources.AppendLine("using System.Windows.Threading;");
            sources.AppendLine("using Hjmos.Lcdp.VisualEditor.Core.Helpers;");
            sources.AppendLine("using Hjmos.Lcdp.Plugins.NccControl.Controls;");

            // 添加reference和reference对应的ViewModel
            // TODO：需要验证下有没有ViewModel这个命名空间存在
            string refNameSpace = reference.GetType().Namespace;
            string refViewModelNameSpace = refNameSpace.Substring(0, refNameSpace.LastIndexOf(".")) + ".ViewModels";
            sources.AppendLine($"using {refNameSpace};");
            sources.AppendLine($"using {refViewModelNameSpace};");



            sources.AppendLine("namespace CodeEvaler{");
            sources.AppendLine("public class CodeEvaler{");
            sources.AppendLine($"private {reference.GetType().Name} Current{{get;set;}}");
            sources.AppendLine("public void EvalCode(object reference){");
            sources.AppendLine($"Current = reference as {reference.GetType().Name};");
            sources.AppendLine(strCode);
            sources.AppendLine("}");
            sources.AppendLine("}");
            sources.Append("}");

            CompilerResults results = provider.CompileAssemblyFromSource(options, sources.ToString());
            if (results.Errors.Count > 0)
            {
                MessageBox.Show("ERROR: " + results.Errors[0].ErrorText, "Error evaluating cs code");
            }

            Assembly assembly = results.CompiledAssembly;
            object codeEvaler = assembly.CreateInstance("CodeEvaler.CodeEvaler");
            codeEvaler.GetType().GetMethod("EvalCode").Invoke(codeEvaler, new object[] { reference });
        }
    }
}

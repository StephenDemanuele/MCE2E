using System;
using System.IO;
using System.Linq;
using System.Text;
using MCE2E.Contracts;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MCE2E.Controller.Bootstrapping
{
    public class EncryptionServiceProviderBuilder
    {
        public IServiceProvider Build(string pluginLibraryPath = "")
        {
            var bootstrapperLog = new StringBuilder();
            var serviceCollection = new ServiceCollection();

            string[] pluginFiles;
            if (!string.IsNullOrEmpty(pluginLibraryPath) && Directory.Exists(pluginLibraryPath))
            {
                pluginFiles = Directory.GetFiles(pluginLibraryPath, "*.dll", SearchOption.AllDirectories);
            }
            else
            {
                pluginFiles = new string[] { Path.Combine(Environment.CurrentDirectory, "MCE2E.AESEncryption.dll") };
            }

            bootstrapperLog.AppendLine(string.Join(Environment.NewLine, pluginFiles.Select(f => Path.GetFileName(f))));
            foreach (var file in pluginFiles)
            {
                bootstrapperLog.AppendLine();
                bootstrapperLog.AppendLine($"Loading assembly from {file}.");
                var assembly = Assembly.LoadFrom(file);
                bootstrapperLog.AppendLine($"Discovering {nameof(IBootstrapper)} implementations from assembly {assembly.GetName()}");

                var bootstrapperTypes = assembly.DefinedTypes.Where(definedType => ImplementsIBootstrapper(definedType)).ToList();
                if (!bootstrapperTypes.Any())
                {
                    bootstrapperLog.AppendLine("No implementations found.");
                    bootstrapperLog.AppendLine();
                    continue;
                }

                foreach (var bootstrapperType in bootstrapperTypes)
                {
                    bootstrapperLog.AppendLine($"Registering {bootstrapperType.FullName}");
                    var bootstrapperInstance = Activator.CreateInstance(bootstrapperType) as IBootstrapper;
                    bootstrapperInstance.Register(serviceCollection);
                    bootstrapperLog.AppendLine($"Registered {bootstrapperType.FullName}.");
                }
            }

            new MCE2E.Controller.Bootstrapping.DefaultBootstrapper().Register(serviceCollection);

            return serviceCollection.BuildServiceProvider(validateScopes: true);
        }

        private bool ImplementsIBootstrapper(TypeInfo typeInfo)
            => typeInfo.ImplementedInterfaces.Any(x => x.Name.Equals("IBootstrapper"));
    }
}

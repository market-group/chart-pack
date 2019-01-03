using McMaster.Extensions.CommandLineUtils;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace chart_pack
{
    class Program
    {
        private static CommandOption<string> _path;
        private static CommandOption<string> _outDir;
        private static CommandOption<string> _version;
        private static CommandOption<string> _name;
        private static CommandOption<bool> _override;
        private static CommandOption<string> _dependencies;

        static int Main(string[] args)
        {
            try
            {
                

                var app = new CommandLineApplication(true)
                {
                    Description = "CLI tool for packing charts",
                    Name = "chart-pack",
                };

                app.HelpOption("--help");

                _path = app.Option<string>("--path|-p", "Path to the chart directory", CommandOptionType.SingleValue)
                            .IsRequired();

                _outDir = app.Option<string>("--out-dir|-od", "Directory to output nupkg", CommandOptionType.SingleValue)
                                .IsRequired();

                _version = app.Option<string>("--version|-v", "Name of the chart", CommandOptionType.SingleValue)
                                .IsRequired();

                _name = app.Option<string>("--name|-n", "Version of the chart", CommandOptionType.SingleValue)
                                .IsRequired();


                _override = app.Option<bool>("--override|-o", "[Optional] Override if exists. Default: false", CommandOptionType.NoValue);

                _dependencies = app.Option<string>("--dependencies|-d", "[Optional] Path to a dependencies.yaml", CommandOptionType.SingleValue);
                
                app.OnExecute(() => Execute());


                return app.Execute(args);
            }
            catch (Exception cpe)
            {
                Console.WriteLine("Execution failed, due to: " + cpe.ToString());
                return 1;
            }
        }

        private static int Execute()
        {
            PackageDependencyGroup[] dependenciesGroups = null;

            if (_dependencies.HasValue())
            {

                if (!File.Exists(_dependencies.ParsedValue))
                {
                    Console.WriteLine($"File {_dependencies.ParsedValue} does not exists");
                    return 1;
                }

                var dependencies = ParseDependencies(_dependencies.ParsedValue);

                dependenciesGroups = new PackageDependencyGroup[]
                {
                    new PackageDependencyGroup(NuGetFramework.AnyFramework,dependencies)
                };
            }
            
            

            var metadata = new ManifestMetadata
            {
                Id = _name.ParsedValue,
                Authors = new[] { "Market" },
                Description = $"A Chart package",
                Version = NuGetVersion.Parse(_version.ParsedValue),
                DependencyGroups = dependenciesGroups ?? new PackageDependencyGroup[0],
            };

            BuildPackage(_path.ParsedValue, metadata, _outDir.ParsedValue, _override.HasValue());

            return 0;
        }


        private static PackageDependency[] ParseDependencies(string filepath)
        {
            var deserailizer = new DeserializerBuilder()
                                .WithNamingConvention(new CamelCaseNamingConvention())
                                .Build();

            var reqs =deserailizer.Deserialize<RequirementsFile>(File.ReadAllText(filepath));


            var dependencies = new List<PackageDependency>();

            foreach(var pair in reqs.Dependencies)
            {
                if(!VersionRange.TryParse(pair.Version,out VersionRange version))
                {
                    throw new Exception($"Failed to parse version '{pair.Version}' for depndency '{pair.Name}'");
                }

                dependencies.Add(new PackageDependency(pair.Name, version));
            }

            return dependencies.ToArray();
        }


        class Dependency
        {
            public string Name { get; set; }
            public string Version { get; set; }
        }

        class RequirementsFile
        {
            public Dependency[] Dependencies { get; set; }
        }

        private static void BuildPackage(string basePath,
                               ManifestMetadata metadata,
                               string outFolder,
                               bool overwrite)
        {
            var nugetPkgBuilder = new PackageBuilder();

            nugetPkgBuilder.PopulateFiles(basePath, new ManifestFile[]
            {
                 new ManifestFile
                 {
                      Source = "**",
                      Target = "chart"
                 }
            });

            nugetPkgBuilder.Populate(metadata);

            var filename = $"{metadata.Id}.{metadata.Version}.nupkg";
            var output = Path.Combine(outFolder, filename);

            if (File.Exists(output) && !overwrite)
                throw new Exception("The package file already exists and --overwrite was not specified");

            Directory.CreateDirectory(Path.GetDirectoryName(output));

            using (var outStream = File.OpenWrite(output))
                nugetPkgBuilder.Save(outStream);

        }

    }
}

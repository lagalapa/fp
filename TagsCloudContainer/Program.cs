﻿using System;
using Autofac;
using CommandLine;

namespace TagsCloudContainer
{
    public class Options
    {
        [Option("file", HelpText = "File with text (txt, docx, rtf, html)", Required = true )]
        public string Filename { get; set; }
        [Option("exclude", HelpText = ".txt file with words to exclude", Required = false)]
        public string Exclude { get; set; }
        [Option("color", HelpText = "Words color", Default = "DarkGreen")]
        public string Color { get; set; }
        [Option("font", HelpText = "Words font family", Default = "Arial")]
        public string FontFamilyName { get; set; }
        [Option("format", HelpText = "Image format", Default = "Png")]
        public string Format { get; set; }
        [Option("height", HelpText = "Image height", Default = "1080")]
        public string Height { get; set; }
        [Option("width", HelpText = "Image width", Default = "1920")]
        public string Width { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
            {
                var source = new DocumentReader(o.Filename);
                var excludedWordsSource = new TextFileReader(o.Exclude);
                var cloudOptions = new CloudOptions(o.Color, o.FontFamilyName, o.Format, o.Height, o.Width);

                var builder = new ContainerBuilder();
                builder.RegisterType<WordsPreprocessor>().As<IWordsPreprocessor>();
                builder.RegisterType<SimpleCloudConfigurator>().As<ICloudConfigurator>();
                builder.RegisterType<CloudVisualizer>().As<ICloudVisualizer>();
                builder.RegisterInstance(source).As<ISource>();
                builder.RegisterInstance(excludedWordsSource).As<TextFileReader>();
                builder.RegisterInstance(cloudOptions).As<CloudOptions>();

                Result.Of(() => builder.Build())
                    .Then(container => container.Resolve<ICloudVisualizer>())
                    .Then(client => client.VisualizeCloud())
                    .OnFail(Console.WriteLine);
            });
        }
    }
}

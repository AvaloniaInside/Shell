using System.Runtime.CompilerServices;
using Avalonia.Metadata;

[assembly: XmlnsDefinition("https://github.com/avaloniaui", "AvaloniaInside.Shell")]
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "AvaloniaInside.Shell.Data")]
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "AvaloniaInside.Shell.Platform")]

[assembly: InternalsVisibleTo("AvaloniaInside.Shell.Tests")]

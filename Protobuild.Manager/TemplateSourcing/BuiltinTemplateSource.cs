using System.Collections.Generic;

namespace Protobuild.Manager
{
    public class BuiltinTemplateSource : ITemplateSource
    {
        public List<TemplateInfo> GetTemplates()
        {
            return new List<TemplateInfo>
            {
                new TemplateInfo
                {
                    TemplateName = "Protogame 2D Platformer",
                    TemplateDescription = "A cross-platform Protogame project that runs a 2D platformer.",
                    TemplateURI = "https-nuget-v3://api.nuget.org/v3/index.json|Protogame.Template.Platformer",
                    AdditionalProtobuildVariants = new Dictionary<string, VariantOverlay>(),
                    AdditionalStandardProjectVariants = new Dictionary<string, VariantOverlay>(),
                    OptionVariants = new List<OptionalVariant>(),
                    AdditionalPlatforms = new List<string>(),
                    Defaults = new Dictionary<string, string>(),
                },
                new TemplateInfo
                {
                    TemplateName = "Protogame Asteroids",
                    TemplateDescription = "An implementation of the classic game Asteroids in Protogame.",
                    TemplateURI = "https-nuget-v3://api.nuget.org/v3/index.json|Protogame.Template.Asteroids",
                    AdditionalProtobuildVariants = new Dictionary<string, VariantOverlay>(),
                    AdditionalStandardProjectVariants = new Dictionary<string, VariantOverlay>(),
                    OptionVariants = new List<OptionalVariant>(),
                    AdditionalPlatforms = new List<string>(),
                    Defaults = new Dictionary<string, string>(),
                },
                new TemplateInfo
                {
                    TemplateName = "Protogame Panning Camera",
                    TemplateDescription = "An example of how to do a 2D panning camera in Protogame.",
                    TemplateURI = "https-nuget-v3://api.nuget.org/v3/index.json|Protogame.Template.PanningCamera",
                    AdditionalProtobuildVariants = new Dictionary<string, VariantOverlay>(),
                    AdditionalStandardProjectVariants = new Dictionary<string, VariantOverlay>(),
                    OptionVariants = new List<OptionalVariant>(),
                    AdditionalPlatforms = new List<string>(),
                    Defaults = new Dictionary<string, string>(),
                },
                new TemplateInfo
                {
                    TemplateName = "Protogame Empty Game",
                    TemplateDescription = "A cross-platform Protogame project that renders an blank screen.",
                    TemplateURI = "https-nuget-v3://api.nuget.org/v3/index.json|Protogame.Template.Blank",
                    AdditionalProtobuildVariants = new Dictionary<string, VariantOverlay>(),
                    AdditionalStandardProjectVariants = new Dictionary<string, VariantOverlay>(),
                    OptionVariants = new List<OptionalVariant>(),
                    AdditionalPlatforms = new List<string>(),
                    Defaults = new Dictionary<string, string>(),
                },
                new TemplateInfo
                {
                    TemplateName = "C# Console",
                    TemplateDescription = "A cross-platform console application that can work on Windows, Mac OS and Linux.",
                    TemplateURI = "https-nuget-v3://api.nuget.org/v3/index.json|Protobuild.Template.Console",
                    AdditionalProtobuildVariants = new Dictionary<string, VariantOverlay>(),
                    AdditionalStandardProjectVariants = new Dictionary<string, VariantOverlay>(),
                    OptionVariants = new List<OptionalVariant>(),
                    AdditionalPlatforms = new List<string>(),
                    Defaults = new Dictionary<string, string>(),
                },
                new TemplateInfo
                {
                    TemplateName = "C# Library",
                    TemplateDescription = "A cross-platform C# library that can work across desktop and mobile platforms.",
                    TemplateURI = "https-nuget-v3://api.nuget.org/v3/index.json|Protobuild.Template.Library",
                    AdditionalProtobuildVariants = new Dictionary<string, VariantOverlay>(),
                    AdditionalStandardProjectVariants = new Dictionary<string, VariantOverlay>(),
                    OptionVariants = new List<OptionalVariant>(),
                    AdditionalPlatforms = new List<string>(),
                    Defaults = new Dictionary<string, string>(),
                },
                new TemplateInfo
                {
                    TemplateName = "MonoGame 2D Platformer",
                    TemplateDescription = "A cross-platform MonoGame project that runs a 2D platformer.",
                    TemplateURI = "https-nuget-v3://api.nuget.org/v3/index.json|MonoGame.Template.Platformer2D",
                    AdditionalProtobuildVariants = new Dictionary<string, VariantOverlay>(),
                    AdditionalStandardProjectVariants = new Dictionary<string, VariantOverlay>(),
					OptionVariants = new List<OptionalVariant>(),
                    AdditionalPlatforms = new List<string>(),
                    Defaults = new Dictionary<string, string>(),
                },
            };
        }
    }
}
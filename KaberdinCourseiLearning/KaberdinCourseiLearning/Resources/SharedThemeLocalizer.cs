using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Resources
{
    public class SharedThemeLocalizer
    {
        private readonly IStringLocalizer localizer;

        public SharedThemeLocalizer(IStringLocalizerFactory factory)
        {
            var assemblyName = new AssemblyName(typeof(Resources.ThemesResource).GetTypeInfo().Assembly.FullName);
            localizer = factory.Create("ThemesResource", assemblyName.Name);
        }

        public string this[string key] => localizer[key];

    }
}

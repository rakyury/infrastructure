﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace Spark.Infrastructure.Logging
{
    /// <summary>
    /// Creates new instances of <see cref="ILog"/> objects.
    /// </summary>
    public static class LogManager
    {
        private static readonly IReadOnlyDictionary<String, SourceLevels> ConfiguredSwitches;
        private static readonly SourceLevels DefaultLevel;

        /// <summary>
        /// Initializes the configured switches for <see cref="LogManager"/>.
        /// </summary>
        static LogManager()
        {
            const String defaultSwitchName = "default";

            var diagnosticsSection = (ConfigurationSection)ConfigurationManager.GetSection("system.diagnostics");
            if (diagnosticsSection == null)
                return;

            var switchSection = diagnosticsSection.ElementInformation.Properties["switches"];
            if (switchSection == null)
                return;

            var switches = switchSection.Value as ConfigurationElementCollection;
            if (switches == null)
                return;

            ConfiguredSwitches = GetConfiguredSwitches(switches.OfType<ConfigurationElement>());
            DefaultLevel = ConfiguredSwitches.ContainsKey(defaultSwitchName) ? ConfiguredSwitches[defaultSwitchName] : SourceLevels.Warning;
        }

        /// <summary>
        /// Get the configured trace switches from the application configuration file.
        /// </summary>
        /// <param name="switchElements">The collection of <see cref="ConfigurationElement"/> instances to attempt to parse the <see cref="SourceLevels"/> value.</param>
        private static IReadOnlyDictionary<String, SourceLevels> GetConfiguredSwitches(IEnumerable<ConfigurationElement> switchElements)
        {
            var configuredSwitches = new Dictionary<String, SourceLevels>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var element in switchElements)
            {
                var nameAttribute = element.ElementInformation.Properties["name"];
                var valueAttribute = element.ElementInformation.Properties["value"];

                if (nameAttribute == null || valueAttribute == null)
                    continue;

                var name = nameAttribute.Value as String;
                if (String.IsNullOrWhiteSpace(name))
                    continue;

                SourceLevels level;
                if (!Enum.TryParse(valueAttribute.Value as String ?? String.Empty, true, out level))
                    continue;
                
                configuredSwitches[name] = level;
            }

            return new ReadOnlyDictionary<String, SourceLevels>(configuredSwitches);
        }

        /// <summary>
        /// Gets a <see cref="ILog"/> instance named after the caller's declaring or reflected type.
        /// </summary>
        public static ILog GetCurrentClassLogger()
        {
            var caller = new StackFrame(1, false).GetMethod();

            return GetLogger((caller.DeclaringType ?? caller.ReflectedType).FullName);
        }

        /// <summary>
        /// Gets the specified named <see cref="ILog"/> instance.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        public static ILog GetLogger(String name)
        {
            do
            {
                if (ConfiguredSwitches.ContainsKey(name))
                    return new Logger(name, ConfiguredSwitches[name]);

                // Use `.` as a hierarchical separator and travel up the name looking for a match.
                name = name.Substring(0, Math.Max(0, name.LastIndexOf('.'))); 
            } while (!String.IsNullOrWhiteSpace(name));

            return new Logger(name, DefaultLevel);
        }
    }
}

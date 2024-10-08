﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerPropertyGrid
{
    /// <summary>
    /// Helper class with static methods to get the list of available properties/events.
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Gets the available properties common to all input types.
        /// </summary>
        /// <param name="types">List of input types. The list must have at least one element.</param>
        public static IEnumerable<PropertyDescriptor> GetCommonAvailableProperties(IEnumerable<Type> types)
        {
            foreach (PropertyDescriptor pd1 in GetAvailableProperties(types.First()))
            {
                bool propertyOk = true;
                foreach (Type type in types.Skip(1))
                {
                    bool typeOk = false;
                    foreach (PropertyDescriptor pd2 in GetAvailableProperties(type))
                    {
                        if (pd1 == pd2)
                        {
                            typeOk = true;
                            break;
                        }
                    }
                    if (!typeOk)
                    {
                        propertyOk = false;
                        break;
                    }
                }
                if (propertyOk) yield return pd1;
            }
        }

        /// <summary>
        /// Gets the available properties for the type.
        /// </summary>
        public static IEnumerable<PropertyDescriptor> GetAvailableProperties(Type forType)
        {
            foreach (PropertyDescriptor p in TypeDescriptor.GetProperties(forType))
            {
                if (!p.IsBrowsable) continue;
                if (p.IsReadOnly) continue;
                if (p.Attributes.OfType<ObsoleteAttribute>().Count() != 0) continue;
                if (p.Name.Contains(".")) continue;
                yield return p;
            }
        }

        /// <summary>
        /// Gets the available events for the type.
        /// </summary>
        public static IEnumerable<EventDescriptor> GetAvailableEvents(Type forType)
        {
            foreach (EventDescriptor e in TypeDescriptor.GetEvents(forType))
            {
                if (!e.IsBrowsable) continue;
                if (e.Attributes.OfType<ObsoleteAttribute>().Count() != 0) continue;
                if (e.Name.Contains(".")) continue;
                yield return e;
            }
        }

        private static string[] hiddenPropertiesOnWindow = new[] { "ClipToBounds" };
        /// <summary>
        /// Gets available properties for an object, includes attached properties also.
        /// </summary>
        public static IEnumerable<PropertyDescriptor> GetAvailableProperties(object element)
        {
            if (element.GetType().FullName == "Hjmos.Lcdp.VisualEditor.Controls.DesignerControls.WindowClone")
            {
                foreach (PropertyDescriptor p in TypeDescriptor.GetProperties(element))
                {
                    if (!p.IsBrowsable) continue;
                    if (p.IsReadOnly && !typeof(ICollection).IsAssignableFrom(p.PropertyType)) continue;
                    if (hiddenPropertiesOnWindow.Contains(p.Name)) continue;
                    if (p.Attributes.OfType<ObsoleteAttribute>().Count() != 0) continue;
                    yield return p;
                }
            }
            else
            {
                foreach (PropertyDescriptor p in TypeDescriptor.GetProperties(element))
                {
                    if (!p.IsBrowsable) continue;
                    if (p.IsReadOnly && !typeof(ICollection).IsAssignableFrom(p.PropertyType)) continue;
                    if (p.Attributes.OfType<ObsoleteAttribute>().Count() != 0) continue;
                    yield return p;
                }
            }
        }

        /// <summary>
        /// Gets common properties between <paramref name="elements"/>. Includes attached properties too.
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyDescriptor> GetCommonAvailableProperties(IEnumerable<object> elements)
        {
            var properties = TypeDescriptor.GetProperties(elements.First()).Cast<PropertyDescriptor>();
            foreach (var element in elements.Skip(1))
            {
                var currentProperties = TypeDescriptor.GetProperties(element).Cast<PropertyDescriptor>();
                properties = Enumerable.Intersect(properties, currentProperties);
            }

            return properties;
        }
    }
}

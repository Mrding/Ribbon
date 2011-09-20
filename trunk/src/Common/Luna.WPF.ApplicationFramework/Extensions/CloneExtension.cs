/**
 * <pre>
 *
 *  Work Force Management
 *  File: CloneExtension.cs
 *
 *  Grandsys, Inc.
 *  Copyright (C): 2009
 *
 *  Description:
 *  Init CloneExtension
 *
 *  Note
 *  Created By: Prime Li at 7/10/2009 4:09:20 PM
 *
 * </pre>
 */

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Luna.WPF.ApplicationFramework.Extensions
{
    /// <summary>
    /// The Extension of Clone class
    /// </summary>
    public static class CloneExtension
    {
        /// <summary>
        /// Store the instance IFormatter
        /// </summary>
        private static readonly IFormatter Formatter = new BinaryFormatter();

        /// <summary>
        /// Clones the specified obj instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objInstance">The obj instance.</param>
        /// <returns></returns>
        public static T Clone<T>(this T objInstance)
        {
            using (Stream stream = new MemoryStream())
            {
                Formatter.Serialize(stream, objInstance);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)Formatter.Deserialize(stream);
            }
        }
    }
}

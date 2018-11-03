using System;
using System.Collections.Generic;
using System.Xml;

namespace SmoothDownloader.Smooth
{
    /// <summary>
    /// </summary>
    internal class Parser
    {
        /// <summary>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IDictionary<string, string> Attributes(XmlNode element)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (XmlAttribute xmlAttribute in element.Attributes)
            {
                dictionary.Add(xmlAttribute.Name, xmlAttribute.Value);
            }

            return dictionary;
        }

        /// <summary>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IDictionary<string, string> CustomAttributes(XmlNode element)
        {
            var dictionary = new Dictionary<string, string>();
            var xmlNode = element.SelectSingleNode("CustomAttributes");

            if (xmlNode == null)
            {
                return dictionary;
            }

            foreach (XmlNode xmlNode2 in xmlNode.SelectNodes("Attribute"))
            {
                dictionary.Add(xmlNode2.Attributes.GetNamedItem("Name").Value, xmlNode2.Attributes.GetNamedItem("Value").Value);
            }

            return dictionary;
        }

        /// <summary>
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string StringAttribute(IDictionary<string, string> attributes, string key)
        {
            if (attributes.ContainsKey(key))
            {
                return attributes[key];
            }

            throw new Exception();
        }

        /// <summary>
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool BoolAttribute(IDictionary<string, string> attributes, string key)
        {
            if (bool.TryParse(StringAttribute(attributes, key), out var result))
            {
                return result;
            }

            throw new Exception();
        }

        /// <summary>
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ushort UInt16Attribute(IDictionary<string, string> attributes, string key)
        {
            if (ushort.TryParse(StringAttribute(attributes, key), out var result))
            {
                return result;
            }

            throw new Exception();
        }

        /// <summary>
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static uint UInt32Attribute(IDictionary<string, string> attributes, string key)
        {
            if (uint.TryParse(StringAttribute(attributes, key), out var result))
            {
                return result;
            }

            throw new Exception();
        }

        /// <summary>
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ulong UInt64Attribute(IDictionary<string, string> attributes, string key)
        {
            if (ulong.TryParse(StringAttribute(attributes, key), out var result))
            {
                return result;
            }

            throw new Exception();
        }

        /// <summary>
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] HexStringAttribute(IDictionary<string, string> attributes, string key)
        {
            return Utils.HexDecodeString(StringAttribute(attributes, key));
        }

        /// <summary>
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static MediaStreamType MediaStreamTypeAttribute(IDictionary<string, string> attributes, string key)
        {
            switch (StringAttribute(attributes, key))
            {
            case "video":
                return MediaStreamType.Video;
            case "audio":
                return MediaStreamType.Audio;
            case "text":
                return MediaStreamType.Script;
            default:
                throw new Exception();
            }
        }
    }
}
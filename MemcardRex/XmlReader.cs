using System;
using System.Collections.Generic;
using System.Xml;

namespace MemcardRex
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal class XmlReader : IDisposable
    {
        /// <summary>
        /// </summary>
        private readonly XmlTextReader _xmlReader;

        /// <summary>
        /// </summary>
        private readonly List<string> _xmlValues = new List<string>();

        /// <summary>
        /// </summary>
        private readonly List<string> _xmlElements = new List<string>();

        /// <summary>
        /// </summary>
        /// <param name="fileName"></param>
        public XmlReader(string fileName)
        {
            _xmlReader = new XmlTextReader(fileName);

            while (_xmlReader.Read())
            {
                switch (_xmlReader.NodeType)
                {
                case XmlNodeType.Element:
                    _xmlElements.Add(_xmlReader.Name);
                    break;
                case XmlNodeType.Text:
                    _xmlValues.Add(_xmlReader.Value);
                    break;
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void Dispose()
        {
            _xmlReader.Close();
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Read(string key)
        {
            if (_xmlElements.Contains(key))
            {
                return _xmlValues[_xmlElements.IndexOf(key) - 1];
            }
     
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public int ReadInteger(string key, int minimum, int maximum)
        {
            string xmlValue = null;

            if (_xmlElements.Contains(key))
            {
                xmlValue = _xmlValues[_xmlElements.IndexOf(key) - 1];
            }

            if (int.TryParse(xmlValue, out var value) == false)
            {
                return value;
            }

            if (value < minimum)
            {
                return minimum;
            }

            if (value > maximum)
            {
                return maximum;
            }

            return value;
        }
    }
}
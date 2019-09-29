using System;
using System.Text;
using System.Xml;

namespace MemcardRex
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal class XmlWriter : IDisposable
    {
        /// <summary>
        /// </summary>
        private readonly XmlTextWriter _xmlWriter;

        /// <summary>
        /// </summary>
        /// <param name="fileName"></param>
        public XmlWriter(string fileName)
        {
            _xmlWriter = new XmlTextWriter(fileName, Encoding.UTF8);
            _xmlWriter.WriteStartDocument();
            _xmlWriter.WriteStartElement("Settings");
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void Dispose()
        {
            _xmlWriter.WriteEndElement();
            _xmlWriter.WriteEndDocument();
            _xmlWriter.Close();
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Write(string key, string value)
        {
            _xmlWriter.WriteStartElement(key);
            _xmlWriter.WriteString(value);
            _xmlWriter.WriteEndElement();
        }
    }
}
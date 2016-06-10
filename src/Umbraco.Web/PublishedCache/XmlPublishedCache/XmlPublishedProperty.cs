using System;
using System.Xml;
using System.Xml.Serialization;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Xml;

namespace Umbraco.Web.PublishedCache.XmlPublishedCache
{

	/// <summary>
	/// Represents an IDocumentProperty which is created based on an Xml structure.
	/// </summary>
	[Serializable]
	[XmlType(Namespace = "http://umbraco.org/webservices/")]
	internal class XmlPublishedProperty : PublishedPropertyBase
	{
		private readonly string _sourceValue; // the raw, xml node value

        // fixme - copy v6 optimizations here!
	    private readonly Lazy<object> _interValue;
        private readonly Lazy<object> _objectValue;
        private readonly Lazy<object> _xpathValue;
	    private readonly bool _isPreviewing;

        /// <summary>
        /// Gets the raw value of the property.
        /// </summary>
        public override object SourceValue => _sourceValue;

	    // in the Xml cache, everything is a string, and to have a value
        // you want to have a non-null, non-empty string.
	    public override bool HasValue => _sourceValue.Trim().Length > 0;

	    public override object Value => _objectValue.Value;
	    public override object XPathValue => _xpathValue.Value;

	    public XmlPublishedProperty(PublishedPropertyType propertyType, bool isPreviewing, XmlNode propertyXmlData)
            : this(propertyType, isPreviewing)
		{
		    if (propertyXmlData == null)
		        throw new ArgumentNullException(nameof(propertyXmlData), "Property xml source is null");
		    _sourceValue = XmlHelper.GetNodeValue(propertyXmlData);
        }

        public XmlPublishedProperty(PublishedPropertyType propertyType, bool isPreviewing, string propertyData)
            : this(propertyType, isPreviewing)
        {
            if (propertyData == null)
                throw new ArgumentNullException(nameof(propertyData));
            _sourceValue = propertyData;
        }

        public XmlPublishedProperty(PublishedPropertyType propertyType, bool isPreviewing)
            : base(propertyType, PropertyCacheLevel.Unknown) // cache level is ignored
        {
            _sourceValue = string.Empty;
            _isPreviewing = isPreviewing;

            _interValue = new Lazy<object>(() => PropertyType.ConvertSourceToInter(_sourceValue, _isPreviewing));
            _objectValue = new Lazy<object>(() => PropertyType.ConvertInterToObject(PropertyCacheLevel.Unknown, _interValue.Value, _isPreviewing));
            _xpathValue = new Lazy<object>(() => PropertyType.ConvertInterToXPath(PropertyCacheLevel.Unknown, _interValue.Value, _isPreviewing));
        }
	}
}
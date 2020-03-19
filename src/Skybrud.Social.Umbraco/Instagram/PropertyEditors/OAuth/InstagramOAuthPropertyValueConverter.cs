using System;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Skybrud.Social.Umbraco.Instagram.PropertyEditors.OAuth {

    public class InstagramOAuthPropertyValueConverter : IPropertyValueConverter {

        public const string PropertyEditorAlias = "Skybrud.Social.Instagram.OAuth";

        public bool IsConverter(PublishedPropertyType propertyType) {
            return propertyType.PropertyEditorAlias == PropertyEditorAlias;
        }

        public object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview) {
            return source;
        }

        public object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview) {

            // Get the value as a string
            string str = source as string;

            // Deserialize the string
            return String.IsNullOrWhiteSpace(str) ? null : InstagramOAuthData.Deserialize(str);

        }

        public object ConvertSourceToXPath(PublishedPropertyType propertyType, object source, bool preview) {
            return null;
        }
    
    }

}
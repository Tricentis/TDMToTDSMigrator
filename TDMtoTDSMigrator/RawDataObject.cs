using System.Collections.Generic;
using System.Text;
using System.Xml;

using Newtonsoft.Json.Linq;

using TestDataContract.TestData;

namespace TDMtoTDSMigrator {
    public class RawDataObject {
        private string categoryId;

        private string categoryName;

        private readonly List<string[]> attributes;

        public RawDataObject() {
            attributes = new List<string[]>();
        }

        public void AddAttribute(string attributeId, string attributeValue) {
            attributes.Add(new[] { attributeId, attributeValue });
        }

        public string FindCategoryName(string categoryId, XmlNode metaInfoTypes) {
            foreach (XmlNode metaInfoType in metaInfoTypes.ChildNodes) {
                if (metaInfoType.Attributes?[0].Value == categoryId) {
                    return metaInfoType.Attributes?[1].Value;
                }
            }
            return "Category not found";
        }

        public string FindAttributeName(string attributeId, XmlNode metaInfoAttributes) {
            foreach (XmlNode metaInfoAttribute in metaInfoAttributes.ChildNodes) {
                if (metaInfoAttribute.Attributes?[0].Value == attributeId) {
                    return metaInfoAttribute.Attributes?[1].Value;
                }
            }
            return "Attribute not found";
        }

        public void SetAttributeNames(XmlNode metaInfoAttributes) {
            foreach (string[] attribute in attributes) {
                attribute[0] = FindAttributeName(attribute[0], metaInfoAttributes);
            }
        }

        public void SetCategoryName(XmlNode metaInfoTypes) {
            categoryName = FindCategoryName(categoryId, metaInfoTypes);
        }

        public void SetAllAttributes(XmlNode stringAttributes, XmlNode metaInfoTypes, XmlNode metaInfoAttributes) {
            List<string[]> categoryInfos = XmlParser.GetCategoriesInfos(metaInfoAttributes);
            foreach (string[] categoryInfo in categoryInfos) {
                if (attributes[0][0] != categoryInfo[0]) {
                    continue;
                }
                categoryId = categoryInfo[2];
                break;
            }
            SetCategoryName(metaInfoTypes);
            SetAttributeNames(metaInfoAttributes);
        }

        public TestDataObject ConvertIntoTestDataObject() {
            return new TestDataObject { Data = JObject.Parse(ConvertAttributesIntoJsonString()), Category = categoryName, Consumed = false };
        }

        public string ConvertAttributesIntoJsonString() {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            foreach (string[] attribute in attributes) {
                builder.Append("\"" + attribute[0] + "\":\"" + attribute[1] + "\",");
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append("}");
            return builder.ToString();
        }
    }
}
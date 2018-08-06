using System;
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

        public string FindCategoryName(XmlNode metaInfoTypes) {
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

        public void SetCategoryName(XmlNode metaInfoTypes, XmlNode metaInfoAttributes) {
            SetCategoryId(metaInfoAttributes);
            categoryName = FindCategoryName(metaInfoTypes);
        }

        public void SetCategoryId(XmlNode metaInfoAttributes) {
            List<string[]> categoryInfos = XmlParser.GetCategoriesInfos(metaInfoAttributes);
            foreach (string[] categoryInfo in categoryInfos)
            {
                if (attributes[0][0] != categoryInfo[0])
                {
                    continue;
                }
                categoryId = categoryInfo[2];
                break;
            }
        }

        public void SetCategoryId(string id) {
            categoryId = id;
        }

        public void SetAllAttributes(XmlNode metaInfoTypes, XmlNode metaInfoAttributes) {
            SetCategoryName(metaInfoTypes, metaInfoAttributes);
            SetAttributeNames(metaInfoAttributes);
        }

        public TestDataObject ConvertIntoTestDataObject() {
            return new TestDataObject { Data = JObject.Parse(ConvertAttributesIntoJsonString()), Category = categoryName, Consumed = false };
        }

        public string ConvertAttributesIntoJsonString() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            foreach (string[] attribute in attributes) {
                stringBuilder.Append("\"" + attribute[0] + "\":\"" + attribute[1] + "\",");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }


    }
}
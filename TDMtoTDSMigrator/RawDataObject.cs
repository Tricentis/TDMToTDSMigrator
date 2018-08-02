using System.Collections.Generic;
using System.Xml;

using Newtonsoft.Json.Linq;

using TestDataContract.TestData;
using System.Text;

namespace TDMtoTDSMigrator {
    public class RawDataObject {

        private string categoryId;

        private string categoryName;

        private readonly List<string[]> attributes;

        public RawDataObject() {
            attributes = new List<string[]>();
        }

        public RawDataObject(RawDataObject obj) {
            attributes = obj.GetAttributes();
            categoryId = obj.GetTypeId();
            categoryName = obj.GetCategoryName();
        }

        public void AddAttribute(string attributeId, string attributeValue) {
            attributes.Add(new[] { attributeId, attributeValue });
        }

        public string GetTypeId() {
            return categoryId;
        }

        public string GetCategoryName() {
            return categoryName;
        }

        public List<string[]> GetAttributes() {
            return attributes;
        }

        public string FindCategoryName(string typeId, XmlNode metaInfoTypes) {
            foreach (XmlNode metaInfoType in metaInfoTypes.ChildNodes) {
                if (metaInfoType.Attributes?[0].Value == typeId) {
                    return metaInfoType.Attributes?[1].Value;
                }
            }
            return "Category not found";
        }

        public string FindAttributeName(string attributeId, XmlNode metaInfoAttributes) {
            foreach (XmlNode metaInfoAttribute in metaInfoAttributes.ChildNodes)
            {
                if (metaInfoAttribute.Attributes?[0].Value == attributeId) {
                    return metaInfoAttribute.Attributes?[1].Value;
                }
            }
            return "Attribute not found";
        }

        public void SetAttributeNames(XmlNode metaInfoAttributes) {
            foreach (string[] attributeInfo in attributes) {
                attributeInfo[0] = FindAttributeName(attributeInfo[0], metaInfoAttributes);
            }
        }

        public void SetCategoryName(XmlNode metaInfoTypes) {
            categoryName = FindCategoryName(categoryId, metaInfoTypes);
        }

        public void SetTypeId(string typeId) {
            categoryId = typeId;
        }

        public TestDataObject ConvertIntoTestDataObject() {
            return new TestDataObject() { Data = JObject.Parse(ConvertAttributesIntoJsonString()), Category = categoryName, Consumed = false};
        }


        public string ConvertAttributesIntoJsonString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{ ");

            for (int i = 0; i < attributes.Count - 1; i++)
            {
                builder.Append("\"" + attributes[i][0] + "\":\"" + attributes[i][1] + "\" , ");
            }
            builder.Append("\"" + attributes[attributes.Count - 1][0] + "\":\"" + attributes[attributes.Count - 1][1] + "\"");
            builder.Append("}\n");

            return builder.ToString();
        }

    }
}
using System.Collections.Generic;
using System.Xml;

namespace TDMtoTDSMigrator {
    public class DataRow {
        private string categoryId;

        private string categoryName;

        private readonly List<string[]> attributes;

        public DataRow() {
            attributes = new List<string[]>();
        }

        public DataRow(DataRow obj) {
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
            for (int i = 0; i < metaInfoTypes.ChildNodes.Count; i++) {
                if (metaInfoTypes.ChildNodes[i].Attributes?[0].Value == typeId) {
                    return metaInfoTypes.ChildNodes[i].Attributes?[1].Value;
                }
            }
            return "Category not found";
        }

        public string FindAttributeName(string attributeId, XmlNode metaInfoAttributes) {
            for (int i = 0; i < metaInfoAttributes.ChildNodes.Count; i++) {
                if (metaInfoAttributes.ChildNodes[i].Attributes?[0].Value == attributeId) {
                    return metaInfoAttributes.ChildNodes[i].Attributes?[1].Value;
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
    }
}
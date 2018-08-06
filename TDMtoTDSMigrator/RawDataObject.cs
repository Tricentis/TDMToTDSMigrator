using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;

using TestDataContract.TestData;

namespace TDMtoTDSMigrator {
    public class RawDataObject {
        #region Fields

        public string CategoryId;

        public string CategoryName;

        private readonly List<string[]> attributes;

        #endregion

        #region Constructors and Destructors

        public RawDataObject() {
            attributes = new List<string[]>();
        }

        #endregion

        #region Public Methods and Operators

        public void AddAttribute(string attributeId, string attributeValue) {
            attributes.Add(new[] { attributeId, attributeValue });
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

        public TestDataObject ConvertIntoTestDataObject() {
            return new TestDataObject { Data = JObject.Parse(ConvertAttributesIntoJsonString()), Category = CategoryName, Consumed = false };
        }

        public string FindAttributeName(string attributeId, TdmDataDocument doc) {
            foreach (MetaInfoAttribute metaInfoAttribute in doc.MetaInfoAttributes) {
                if (metaInfoAttribute.AttributeId == attributeId) {
                    return metaInfoAttribute.AttributeName;
                }
            }
            return "Attribute not found";
        }

        public string FindCategoryName(TdmDataDocument doc) {
            foreach (MetaInfoType metaInfoType in doc.MetaInfoTypes) {
                if (metaInfoType.CategoryId == CategoryId) {
                    return metaInfoType.CategoryName;
                }
            }
            return "Category not found";
        }

        public void SetAllAttributes(TdmDataDocument doc) {
            SetCategoryName(doc);
            SetAttributeNames(doc);
        }

        public void SetAttributeNames(TdmDataDocument doc) {
            foreach (string[] attribute in attributes) {
                attribute[0] = FindAttributeName(attribute[0], doc);
            }
        }

        public void SetCategoryId(TdmDataDocument doc) {
            foreach (MetaInfoAttribute metaInfoAttribute in doc.MetaInfoAttributes) {
                if (attributes[0][0] != metaInfoAttribute.AttributeId) {
                    continue;
                }
                CategoryId = metaInfoAttribute.AssociatedCategoryId;
                break;
            }
        }

        public void SetCategoryId(string id) {
            CategoryId = id;
        }

        public void SetCategoryName(TdmDataDocument doc) {
            SetCategoryId(doc);
            CategoryName = FindCategoryName(doc);
        }

        #endregion
    }
}
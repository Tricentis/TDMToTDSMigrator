using System.Xml;

namespace TDMtoTDSMigrator {
    public class StringAttribute {
        private readonly TdmDataDocument tdmDataSheet;

        public string ObjectId;

        public string AttributeId;

        public string AttributeValue;

        public string AttributeName => AttributeId != null ? tdmDataSheet.FindAttributeName(AttributeId) : "Not Found";

        public string CategoryName => AttributeId != null ? tdmDataSheet.FindCategoryName(tdmDataSheet.FindCategoryId(AttributeId)) : "Not Found";

        public StringAttribute(TdmDataDocument tdmDataSheet, XmlNode stringAttribute) {
            this.tdmDataSheet = tdmDataSheet;
            ObjectId = stringAttribute.Attributes?[0].Value; 
            AttributeId = stringAttribute.Attributes?[1].Value; 
            AttributeValue = stringAttribute.Attributes?[2].Value; 
        }
    }
}
using System.Xml;

namespace TDMtoTDSMigrator {
    public class MetaInfoType {
        public string CategoryId;

        public string CategoryName;

        public MetaInfoType(XmlNode metaInfoType) {
            CategoryId = metaInfoType.Attributes?[0].Value; //ex:"4"
            CategoryName = metaInfoType.Attributes?[1].Value; //ex:"Person"
        }
    }
}
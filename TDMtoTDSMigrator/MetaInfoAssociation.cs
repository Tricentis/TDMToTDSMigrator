using System.Xml;

namespace TDMtoTDSMigrator {
    public class MetaInfoAssociation {
        public string AssociationId;

        public string CategoryName;

        public string CategoryId;

        public string PartnerId;

        public string PartnerName;

        public MetaInfoAssociation(XmlNode metaInfoAssociation) {
            AssociationId = metaInfoAssociation.Attributes?[0].Value; //ex:"1"
            CategoryName = metaInfoAssociation.Attributes?[1].Value; //ex:"Person"
            CategoryId = metaInfoAssociation.Attributes?[2].Value; //ex:"4"
            PartnerId = metaInfoAssociation.Attributes?[3].Value; //ex:"2"
            PartnerName = metaInfoAssociation.Attributes?[4].Value; //ex:"City"
        }
    }
}
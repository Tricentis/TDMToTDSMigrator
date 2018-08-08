using System.Xml;

namespace TDMtoTDSMigrator {
    public class MetaInfoAssociation {
        public string AssociationId;

        public string CategoryName;

        public string CategoryId;

        public string PartnerId;

        public string PartnerName;

        public MetaInfoAssociation(XmlNode metaInfoAssociation) {
            AssociationId = metaInfoAssociation.Attributes?[0].Value;
            CategoryName = metaInfoAssociation.Attributes?[1].Value;
            CategoryId = metaInfoAssociation.Attributes?[2].Value;
            PartnerId = metaInfoAssociation.Attributes?[3].Value;
            PartnerName = metaInfoAssociation.Attributes?[4].Value;
        }
    }
}